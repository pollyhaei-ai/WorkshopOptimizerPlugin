using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using System;
using System.Numerics;
using WorkshopOptimizerPlugin.Utils;

namespace WorkshopOptimizerPlugin.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Configuration configuration;

    public ConfigWindow(Plugin plugin) : base(
        "開拓工坊排程設定",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(520, 360),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.configuration = plugin.Configuration;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Draw()
    {
        var saveConfig = false;

        var defaultRestCycle1 = configuration.DefaultRestCycle1 + 1;
        ImGui.Text("預設休息日");
        ImGui.Indent(20);
        ImGui.SetNextItemWidth(100);
        if (ImGui.InputInt("第1個", ref defaultRestCycle1, 1))
        {
            configuration.DefaultRestCycle1 = UIUtils.FixValue(ref defaultRestCycle1, 1, 7) - 1;
            saveConfig = true;
        }
        var defaultRestCycle2 = configuration.DefaultRestCycle2 + 1;
        ImGui.SetNextItemWidth(100);
        if (ImGui.InputInt("第2個", ref defaultRestCycle2, 1))
        {
            configuration.DefaultRestCycle2 = UIUtils.FixValue(ref defaultRestCycle2, 1, 7) - 1;
            saveConfig = true;
        }
        ImGui.Unindent(20);

        ImGui.Spacing();
        var defaultTopValues = configuration.DefaultTopValues;
        ImGui.SetNextItemWidth(100);
        if (ImGui.InputInt("預設顯示筆數", ref defaultTopValues, 5))
        {
            configuration.DefaultTopValues = UIUtils.FixValue(ref defaultTopValues, 1, 2000);
            saveConfig = true;
        }

        var itemGenerationCutoff = configuration.ItemGenerationCutoff;
        ImGui.SetNextItemWidth(100);
        if (ImGui.InputInt("物品生成截止", ref itemGenerationCutoff, 5))
        {
            configuration.ItemGenerationCutoff = UIUtils.FixValue(ref itemGenerationCutoff, 1, 10);
            saveConfig = true;
        }

        ImGui.Spacing();
        var defaultMultiCycleLimit = configuration.DefaultMultiCycleLimit;
        ImGui.SetNextItemWidth(100);
        if (ImGui.InputInt("預設跨生產日上限", ref defaultMultiCycleLimit, 1, 10))
        {
            configuration.DefaultMultiCycleLimit = UIUtils.FixValue(ref defaultMultiCycleLimit, 0, 999);
            saveConfig = true;
        }

        ImGui.Spacing();
        var defaultAllowMissingCommonMaterials = configuration.DefaultAllowMissingCommonMaterials;
        if (ImGui.Checkbox("預設允許缺少常見素材", ref defaultAllowMissingCommonMaterials))
        {
            configuration.DefaultAllowMissingCommonMaterials = defaultAllowMissingCommonMaterials;
            saveConfig = true;
        }

        if (saveConfig)
        {
            configuration.Save();
        }
    }
}
