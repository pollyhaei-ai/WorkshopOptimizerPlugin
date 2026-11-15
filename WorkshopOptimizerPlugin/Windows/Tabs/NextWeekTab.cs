using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkshopOptimizerPlugin.Data;
using WorkshopOptimizerPlugin.Optimizer;
using WorkshopOptimizerPlugin.Windows.Utils;

namespace WorkshopOptimizerPlugin.Windows.Tabs;

internal class NextWeekTab : ITab
{
    private readonly UIDataSource data;

    public NextWeekTab(UIDataSource uiDataSource)
    {
        this.data = uiDataSource;
    }

    public void OnOpen() { }

    public void Draw()
    {
        ImGui.Spacing();
        if (ImGui.BeginTable("Next Week's Items", 4, ImGuiTableFlags.RowBg))
        {
            ImGui.TableSetupColumn("產品");
            ImGui.TableSetupColumn("下週期受歡迎度");
            ImGui.TableSetupColumn("稀少開拓素材");
            ImGui.TableSetupColumn("探索目的地");
            ImGui.TableHeadersRow();

            var gItems = new List<Tuple<GranaryItemData, Item>>();
            foreach (var gItem in GranaryItemData.Items)
            {
                gItems.Add(new Tuple<GranaryItemData, Item>(gItem, data.CurrentItemCache[ItemStaticData.Get(gItem.Id)]));
            }

            foreach (var i in gItems.OrderBy(o => o.Item2.NextPopularity))
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text(i.Item2.Name);
                ImGui.TableNextColumn();
                ImGui.Text(Zh.Popularity(i.Item2.NextPopularity));
                ImGui.TableNextColumn();
                ImGui.Text(i.Item1.Component);
                ImGui.TableNextColumn();
                ImGui.Text(i.Item1.Mission);
            }
            ImGui.EndTable();
        }
    }

}
