using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Numerics;
using WorkshopOptimizerPlugin.Data;
using WorkshopOptimizerPlugin.Utils;
using WorkshopOptimizerPlugin.Windows.Tabs;
using WorkshopOptimizerPlugin.Windows.Utils;

namespace WorkshopOptimizerPlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly Plugin plugin;

    private readonly ItemSetsCache[] itemSetsCaches;
    private readonly UIDataSource uiDataSource;
    private readonly CommonInterfaceElements commonInterfaceElements;

    private readonly List<Tuple<string, ITab>> tabs;

    public MainWindow(Plugin plugin) : base(
        "開拓工坊排程", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 600),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.plugin = plugin;

        itemSetsCaches = new ItemSetsCache[Constants.MaxSeasons] { new ItemSetsCache(), new ItemSetsCache() };
        uiDataSource = UIDataSource.Load(plugin.Configuration);
        for (var i = 0; i < Constants.MaxSeasons; i++)
        {
            uiDataSource.AddListener(itemSetsCaches[i]);
        }
        commonInterfaceElements = new CommonInterfaceElements(plugin.Icons, plugin.Configuration, uiDataSource);

        tabs = new()
        {
            new Tuple<string, ITab>("工坊", new WorkshopsTab(uiDataSource, commonInterfaceElements, itemSetsCaches)),
            new Tuple<string, ITab>("生產計畫", new ProducedTab(uiDataSource, commonInterfaceElements)),
            new Tuple<string, ITab>("下週期", new NextWeekTab(uiDataSource)),
            new Tuple<string, ITab>("產品", new ItemsTab(plugin.Icons, uiDataSource, commonInterfaceElements)),
            new Tuple<string, ITab>("供需型態", new PatternsTab(uiDataSource, commonInterfaceElements)),
            new Tuple<string, ITab>("組合", new CombinationsTab(uiDataSource, commonInterfaceElements, itemSetsCaches)),
            new Tuple<string, ITab>("貓耳小員的請求", new FavorsTab(uiDataSource)),
#if DEBUG
            new Tuple<string, ITab>("素材", new MaterialsTab()),
#endif
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void OnOpen()
    {
        base.OnOpen();
        foreach (var tab in tabs)
        {
            tab.Item2.OnOpen();
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        if (uiDataSource.Dirty)
        {
            uiDataSource.Save();
        }
    }

    public override void Draw()
    {
        if (!IsSameSeason())
        {
            if (SeasonUtils.IsPreviousSeason(uiDataSource.DataSource.SeasonStart))
            {
                uiDataSource.NextWeek();
            }
            else
            {
                uiDataSource.Reset(plugin.Configuration);
            }
        }

        var cycle = SeasonUtils.GetCycle();
        var rank = IslandProvider.GetIslandRank();
        var str_rank = rank > 0 ? rank.ToString() : "未知";
        ImGui.Text($"週期：{uiDataSource.DataSource.SeasonStart:yyyy-MM-dd}～{uiDataSource.DataSource.SeasonStart.AddDays(Constants.MaxCycles):yyyy-MM-dd}。生產日={cycle + 1}。島嶼等級={str_rank}。");
        ImGui.SameLine();
        PopulateDataIfPossible();
#if DEBUG
        ImGui.SameLine();
        if (ImGui.Button("重置"))
        {
            uiDataSource.Reset(plugin.Configuration);
        }
#endif
        ImGui.SameLine();
        var indent = ImGui.GetWindowWidth() - 45;
        ImGui.Indent(indent);
        if (UIUtils.ImageButton(plugin.Icons.Settings, "設定"))
        {
            plugin.OpenConfigUI();
        }
        ImGui.Unindent(indent);
        ImGui.Spacing();

        if (rank == 0)
        {
            ImGui.TextColored(new Vector4(0.75f, 0, 0, 1), "請先前往自己的無人島。");
            return;
        }

        if (ImGui.BeginTabBar("Tabs"))
        {
            foreach (var tab in tabs)
            {
                if (ImGui.BeginTabItem(tab.Item1))
                {
                    tab.Item2.Draw();
                    ImGui.EndTabItem();
                }
            }
            ImGui.EndTabBar();
        }
    }

    private void PopulateDataIfPossible()
    {
        var cycle = SeasonUtils.GetCycle();
        if (uiDataSource.DataSource.DataCollectionTime[cycle] == null)
        {
            var manager = ManagerProvider.GetManager();
            if (!manager.IsValid)
            {
                ImGui.TextColored(new Vector4(0.75f, 0, 0, 1), "請先前往自己的無人島，並打開「海島產品需求調查」。");
                return;
            }

            PopulateJsonData(manager);

            if (commonInterfaceElements.Cycle == cycle)
            {
                commonInterfaceElements.NextCycle();
            }
        }
        ImGui.Text("");
    }

    private void PopulateJsonData(Manager manager)
    {
        var popIndex = manager.CurrentPopularityIndex;
        var nextPopIndex = manager.NextPopularityIndex;
        var cycle = SeasonUtils.GetCycle();
        for (uint i = 0; i < Constants.MaxItems; i++)
        {
            var pop = PopularityTable.GetItemPopularity(popIndex, i);
            if (pop == Popularity.Unknown) continue;

            var item = uiDataSource.DataSource.CurrentDynamicData[(int)i];
            item.Popularity = pop;
            item.NextPopularity = PopularityTable.GetItemPopularity(nextPopIndex, i);
            item.Supply[cycle] = manager.GetSupplyForCraftwork(i);
            item.Demand[cycle] = manager.GetDemandShiftForCraftwork(i);
        }
        uiDataSource.DataSource.DataCollectionTime[cycle] = DateTime.UtcNow;
        uiDataSource.DataChanged(cycle);
    }

    private bool IsSameSeason()
    {
        return SeasonUtils.IsSameSeason(uiDataSource.DataSource.SeasonStart);
    }

}
