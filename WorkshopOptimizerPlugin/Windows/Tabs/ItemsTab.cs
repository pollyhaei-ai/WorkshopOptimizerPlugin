using Dalamud.Bindings.ImGui;
using System;
using System.Numerics;
using WorkshopOptimizerPlugin.Data;
using WorkshopOptimizerPlugin.Utils;
using WorkshopOptimizerPlugin.Windows.Utils;

namespace WorkshopOptimizerPlugin.Windows.Tabs;

internal class ItemsTab : ITab
{
    private readonly UIDataSource uiDataSource;
    private readonly CommonInterfaceElements ifData;
    private readonly Icons icons;

    private int mSetWhen;
    private readonly int[] mWhenOveerides;
    public bool hasWhenOverrides;

    public ItemsTab(Icons icons, UIDataSource uiDataSource, CommonInterfaceElements ifData)
    {
        this.uiDataSource = uiDataSource;
        this.ifData = ifData;
        this.icons = icons;

        mWhenOveerides = new int[Constants.MaxItems];
        for (var i = 0; i < Constants.MaxItems; i++)
        {
            mWhenOveerides[i] = (int)uiDataSource.WhenOverrides[(uint)i];
        }
        hasWhenOverrides = false;
    }

    public void OnOpen() { }

    public void Draw()
    {
        ifData.DrawBasicControls();
        var cycle = ifData.Cycle;
        ImGui.SameLine();
        DrawActionsBar();
        ImGui.Spacing();
        DrawCollectionHint(cycle);
        ImGui.Spacing();

        if (ImGui.BeginTable("Data", 9, ImGuiTableFlags.ScrollY | ImGuiTableFlags.RowBg))
        {
            ImGui.TableSetupColumn("產品", ImGuiTableColumnFlags.WidthFixed, 220);
            ImGui.TableSetupColumn("小時", ImGuiTableColumnFlags.WidthFixed, 50);
            ImGui.TableSetupColumn("受歡迎度", ImGuiTableColumnFlags.WidthFixed, 110);
            ImGui.TableSetupColumn("目前市場需求", ImGuiTableColumnFlags.WidthFixed, 150);
            ImGui.TableSetupColumn("此前需求變化", ImGuiTableColumnFlags.WidthFixed, 150);
            ImGui.TableSetupColumn("型態", ImGuiTableColumnFlags.WidthFixed, 350);
            ImGui.TableSetupColumn("基礎銷售價", ImGuiTableColumnFlags.WidthFixed, 100);
            ImGui.TableSetupColumn("產值", ImGuiTableColumnFlags.WidthFixed, 80);
            ImGui.TableSetupColumn("使用時機", ImGuiTableColumnFlags.WidthFixed, 120);
            ImGui.TableHeadersRow();

            var itemCache = ifData.IsCurrentSeason() ? uiDataSource.CurrentItemCache : uiDataSource.PreviousItemCache;
            var disabled = ifData.IsPreviousSeason();
            for (uint i = 0; i < Constants.MaxItems; i++)
            {
                var staticData = ItemStaticData.Get(i);
                if (!staticData.IsValid()) { continue; }

                var item = itemCache[staticData];
                var patterns = item.FindPatterns(cycle);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text(item.Name);
                ImGui.TableNextColumn();
                ImGui.Text($"{item.Hours}");
                ImGui.TableNextColumn();
                ImGui.Text(Zh.Popularity(item.Popularity));
                ImGui.TableNextColumn();
                if (item.Supply[cycle] != Supply.Unknown || patterns.Count != 1)
                {
                    ImGui.Text(Zh.Supply(item.Supply[cycle]));
                }
                else
                {
                    ImGui.Text($"{Zh.Supply(patterns[0].SupplyPattern[cycle])}*");
                }
                ImGui.TableNextColumn();
                if (item.Demand[cycle] != Demand.Unknown || patterns.Count != 1)
                {
                    ImGui.Text(Zh.Demand(item.Demand[cycle]));
                } else
                {
                    ImGui.Text($"{Zh.Demand(patterns[0].DemandPattern[cycle])}*");
                }
                ImGui.TableNextColumn();
                var spatterns = string.Join("/", patterns.ConvertAll(p => p.Name));
                ImGui.Text(spatterns);
                ImGui.TableNextColumn();
                ImGui.Text(item.Value.ToString());
                ImGui.TableNextColumn();
                ImGui.Text($"{item.EffectiveValue(cycle):F2}");
                ImGui.TableNextColumn();
                if (disabled) { ImGui.BeginDisabled(); }
                if (ImGui.Combo($"###Item {i}", ref mWhenOveerides[i], WhenUtils.WhenAsStrings, WhenUtils.WhenAsStrings.Length))
                {
                    uiDataSource.WhenOverrides[i] = (When)mWhenOveerides[i];
                    uiDataSource.OptimizationParameterChanged();
                    hasWhenOverrides = true;
                }
                if (disabled) { ImGui.EndDisabled(); }
            }
            ImGui.EndTable();
        }
    }

    private void DrawCollectionHint(int cycle)
    {
        if (!ifData.IsCurrentSeason())
        {
            return;
        }

        var today = SeasonUtils.GetCycle();
        var times = uiDataSource.DataSource.DataCollectionTime;
        var viewingCollected = times[cycle] != null;
        if (viewingCollected && cycle == today)
        {
            ImGui.TextColored(new Vector4(0.45f, 0.8f, 0.45f, 1), "「目前市場需求」與「此前需求變化」是今天從海島產品需求調查讀到的。標 * 的是預測。");
            return;
        }
        if (times[today] != null && cycle != today)
        {
            ImGui.TextColored(new Vector4(0.9f, 0.75f, 0.2f, 1), $"第{today + 1}日已收集。把生產日調回第{today + 1}日，就能看到今天的目前市場需求與此前需求變化。");
            return;
        }
        if (!viewingCollected)
        {
            ImGui.Text("此日若還沒收集，供需標 * 的是預測。");
        }
    }

    private void DrawActionsBar()
    {
        var indent = 780;
        ImGui.Indent(indent);
        ImGui.Text("將");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(120);
        ImGui.Combo("###SET_TYPE", ref mSetWhen, new string[]{"全部", "常見素材", "開拓素材"}, 3);
        Func<MaterialSource, bool>[] checkMaterials = {
            s => true,
            s => s == MaterialSource.Gatherable || s == MaterialSource.Crop || s == MaterialSource.Dropping,
            s => s == MaterialSource.Gatherable,
        };
        ImGui.SameLine();
        ImGui.Text("設為");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(100);
        if (ImGui.BeginCombo("###SET_WHEN", ""))
        {
            foreach (var w in new When[] { When.Never, When.Weak, When.Strong, When.Either, When.Always, When.Required })
            {
                if (ImGui.Selectable(Zh.When(w)))
                {
                    for (uint i = 0; i < Constants.MaxItems; i++)
                    {
                        var itemData = ItemStaticData.Get(i);
                        if (!itemData.IsValid()) continue;

                        var matches = true;
                        foreach (var materials in itemData.Materials)
                        {
                            matches &= checkMaterials[mSetWhen](materials.Material.Source);
                        }
                        if (matches)
                        {
                            uiDataSource.WhenOverrides[i] = w;
                            mWhenOveerides[i] = (int)w;
                        }
                    }
                    uiDataSource.OptimizationParameterChanged();
                    hasWhenOverrides = true;
                }
            }
            ImGui.EndCombo();
        }
        ImGui.SameLine();
        if (UIUtils.ImageButton(icons.ResetToDefaults, "恢復預設", hasWhenOverrides))
        {
            uiDataSource.WhenOverrides.Reset();
            for (uint i = 0; i < Constants.MaxItems; i++)
            {
                mWhenOveerides[i] = (int)uiDataSource.WhenOverrides[i];
            }
            uiDataSource.OptimizationParameterChanged();
            hasWhenOverrides = false;
        }
        ImGui.Unindent(indent);
    }
}
