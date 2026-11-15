using Dalamud.Bindings.ImGui;
using System.Linq;
using WorkshopOptimizerPlugin.Data;
using WorkshopOptimizerPlugin.Windows.Utils;

namespace WorkshopOptimizerPlugin.Windows.Tabs;

internal class FavorsTab : ITab
{
    private readonly UIDataSource uiDataSource;


    public FavorsTab(UIDataSource uiDataSource)
    {
        this.uiDataSource = uiDataSource;
    }

    public void OnOpen() { }

    public void Draw()
    {
        var itemCache = uiDataSource.CurrentItemCache;
        var cycle = SeasonUtils.GetCycle() + 1;
        if (cycle > Constants.MaxCycles) cycle = Constants.MaxCycles;

        ImGui.Spacing();
        if (ImGui.BeginTable("Felicitous Favors Items", 7, ImGuiTableFlags.RowBg))
        {
            ImGui.TableSetupColumn("產品");
            ImGui.TableSetupColumn("小時");
            ImGui.TableSetupColumn("所需出口數");
            ImGui.TableSetupColumn("已生產");
            ImGui.TableSetupColumn("受歡迎度");
            ImGui.TableSetupColumn("型態");
            ImGui.TableSetupColumn("必須");
            ImGui.TableHeadersRow();

            for (var i = 0; i < Constants.MaxFelicitousFavors; i++)
            {
                ItemStaticData? selectedItemStaticData = null;
                if (uiDataSource.DataSource.CurrentFelicitousFavors[i] is uint selectedIndex)
                {
                    selectedItemStaticData = ItemStaticData.Get(selectedIndex);
                }
                var selectedItem = itemCache.Get(selectedItemStaticData);

                var hours = 4 + (i * 2);
                var needed = i == 1 ? 6 : 8;
                var done = selectedItem?.AccumulatedProduced(cycle) ?? 0;

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                if (ImGui.BeginCombo($"##Item_{i}", selectedItem?.Name ?? ""))
                {
                    if (ImGui.Selectable(""))
                    {
                        if (selectedItem != null)
                        {
                            uiDataSource.WhenOverrides[selectedItem.Id] = ItemStaticData.Get(selectedItem.Id).When;
                            uiDataSource.OptimizationParameterChanged();
                        }
                        uiDataSource.DataSource.CurrentFelicitousFavors[i] = null;
                        uiDataSource.NeedsSave();
                    }
                    foreach (var item in ItemStaticData.GetAllItems().Where(i => i.IsValid() && i.Hours == hours).OrderBy(i => i.Name))
                    {
                        if (ImGui.Selectable(item.Name))
                        {
                            uiDataSource.DataSource.CurrentFelicitousFavors[i] = item.Id;
                            uiDataSource.NeedsSave();
                        }
                    }
                    ImGui.EndCombo();
                }
                ImGui.TableNextColumn();
                ImGui.Text($"{hours}");
                ImGui.TableNextColumn();
                ImGui.Text($"{needed}");
                ImGui.TableNextColumn();
                ImGui.Text($"{done}");
                ImGui.TableNextColumn();
                ImGui.Text(selectedItem == null ? "" : Zh.Popularity(selectedItem.Popularity));
                ImGui.TableNextColumn();
                {
                    var patterns = selectedItem?.FindPatterns(cycle);
                    if (patterns != null)
                    {
                        var spatterns = string.Join("/", patterns.ConvertAll(p => p.Name));
                        ImGui.Text(spatterns);
                    }
                    else
                    {
                        ImGui.Text("");
                    }
                }
                ImGui.TableNextColumn();
                if (selectedItem != null)
                {
                    if (done >= needed)
                    {
                        ImGui.Text("已達成");
                    }
                    else if (uiDataSource.WhenOverrides[selectedItem.Id] == When.Required)
                    {
                        if (ImGui.Button($"恢復預設##Reset_{i}"))
                        {
                            uiDataSource.WhenOverrides[selectedItem.Id] = ItemStaticData.Get(selectedItem.Id).When;
                            uiDataSource.OptimizationParameterChanged();
                        }
                    }
                    else
                    {
                        if (ImGui.Button($"必須##Require_{i}"))
                        {
                            uiDataSource.WhenOverrides[selectedItem.Id] = When.Required;
                            uiDataSource.OptimizationParameterChanged();
                        }
                    }
                }
            }
            ImGui.EndTable();
        }
    }
}
