using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using WorkshopOptimizerPlugin.Data;
using WorkshopOptimizerPlugin.Optimizer;
using WorkshopOptimizerPlugin.Windows.Utils;

namespace WorkshopOptimizerPlugin.Windows.Tabs;

internal class PatternsTab : ITab
{
    private readonly UIDataSource uiDataSource;
    private readonly CommonInterfaceElements ifData;

    public PatternsTab(UIDataSource uiDataSource, CommonInterfaceElements ifData)
    {
        this.uiDataSource = uiDataSource;
        this.ifData = ifData;
    }

    public void OnOpen() { }

    public void Draw()
    {
        ifData.DrawBasicControls();
        var cycle = ifData.Cycle;
        var startGroove = ifData.GetStartGroove();
        ImGui.Spacing();

        if (ImGui.BeginTable("Patterns", 6, ImGuiTableFlags.ScrollY | ImGuiTableFlags.RowBg))
        {
            ImGui.TableSetupColumn("產品", ImGuiTableColumnFlags.WidthFixed, 240);
            ImGui.TableSetupColumn("型態", ImGuiTableColumnFlags.WidthFixed, 90);
            ImGui.TableSetupColumn("預期市場需求", ImGuiTableColumnFlags.WidthFixed, 220);
            ImGui.TableSetupColumn("受歡迎度", ImGuiTableColumnFlags.WidthFixed, 110);
            ImGui.TableSetupColumn("產值", ImGuiTableColumnFlags.WidthFixed, 150);
            ImGui.TableSetupColumn("含氣勢產值", ImGuiTableColumnFlags.WidthFixed, 150);
            ImGui.TableHeadersRow();

            var itemCache = ifData.IsCurrentSeason()? uiDataSource.CurrentItemCache : uiDataSource.PreviousItemCache;
            var items = new List<Item>();
            for (var i = 0; i < Constants.MaxItems; i++)
            {
                var staticData = ItemStaticData.Get(i);
                if (!staticData.IsValid()) { continue; }

                var item = itemCache[staticData];
                var (pattern, _) = item.FindPattern(cycle);

                if (pattern != null)
                {
                    items.Add(item);
                }
            }

            foreach (var item in items.OrderByDescending(o => o.EffectiveValue(cycle)).ToList())
            {
                var (pattern, _) = item.FindPattern(cycle);
                if (pattern == null) { continue; }

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text(item.Name);
                ImGui.TableNextColumn();
                ImGui.Text(pattern.Name);
                ImGui.TableNextColumn();
                var expectedSupply = pattern.SupplyPattern[cycle]; ;
                var actualSupply = item.Supply[cycle];
                if (actualSupply == Supply.Unknown || actualSupply == expectedSupply)
                {
                    ImGui.Text(Zh.Supply(expectedSupply));
                }
                else
                {
                    ImGui.TextColored(new Vector4(0.75f, 0.5f, 0, 1), $"{Zh.Supply(expectedSupply)}（實際：{Zh.Supply(actualSupply)}）");
                }
                ImGui.TableNextColumn();
                ImGui.Text(Zh.Popularity(item.Popularity));
                ImGui.TableNextColumn();
                var effValue = item.EffectiveValue(cycle);
                ImGui.Text(string.Format("{0:F2}", effValue));
                ImGui.TableNextColumn();
                ImGui.Text(string.Format("{0:F2}", effValue * startGroove.Multiplier()));
            }
            ImGui.EndTable();
        }
    }
}
