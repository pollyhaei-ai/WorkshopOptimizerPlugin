using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Numerics;
using WorkshopOptimizerPlugin.Data;
using WorkshopOptimizerPlugin.Optimizer;
using WorkshopOptimizerPlugin.Windows.Utils;

namespace WorkshopOptimizerPlugin.Windows.Tabs;

internal class WeekPlanTab : ITab, IUIDataSourceListener
{
    private static readonly string[] DayNames = { "第1日", "第2日", "第3日", "第4日", "第5日", "第6日", "第7日" };
    private static readonly Vector4 WarningColor = new(0.9f, 0.75f, 0.2f, 1);
    private static readonly Vector4 CollectedColor = new(0.45f, 0.8f, 0.45f, 1);

    private readonly UIDataSource uiDataSource;
    private readonly CommonInterfaceElements ifData;
    private readonly Configuration configuration;

    private WeekPlan? plan;
    private WeekOptimizer? running;
    private readonly bool[] locked = new bool[Constants.MaxCycles];
    private int rest1;
    private int rest2;
    private bool restLocked;
    private bool restReady;
    private bool suppress;
    private string status = "";

    public WeekPlanTab(UIDataSource uiDataSource, CommonInterfaceElements ifData, Configuration configuration)
    {
        this.uiDataSource = uiDataSource;
        this.ifData = ifData;
        this.configuration = configuration;
        uiDataSource.AddListener(this);
    }

    public void OnOpen() { }

    public void OnDataChange(int cycle)
    {
        if (suppress)
        {
            return;
        }
        running = null;
        if (plan != null)
        {
            plan.Stale = true;
        }
    }

    public void OnOptimizationParameterChange()
    {
        OnDataChange(0);
    }

    public void Draw()
    {
        EnsureRest();
        Pump();
        DrawIntro();
        DrawRestControls();
        DrawActions();
        if (status.Length > 0)
        {
            ImGui.TextWrapped(status);
        }
        if (running != null)
        {
            var percent = Math.Floor(running.Progress * 20) * 5;
            ImGui.Text($"計算中，請稍候……第{running.CurrentDay + 1}日 {percent:F0}%%");
        }

        var shown = plan ?? BuildPreview();
        var today = SeasonUtils.GetCycle();
        if (shown.IsPreview)
        {
            ImGui.Text("尚未計算。下面是目前寫在生產計畫裡的內容。");
        }
        else if (shown.Stale)
        {
            ImGui.TextColored(WarningColor, "供需或條件已變，這份排程還沒重算。重新計算只改還沒過、也沒鎖定的日子。");
        }
        ImGui.Text(shown.IsPreview ? $"目前生產計畫合計：{shown.TotalValue:F2}" : $"本週合計產值：{shown.TotalValue:F2}");
        ImGui.Spacing();
        DrawTable(shown, today);
    }

    private void Pump()
    {
        if (running == null)
        {
            return;
        }
        var done = running.Advance(TimeSpan.FromMilliseconds(8));
        if (done == null)
        {
            return;
        }
        plan = done;
        running = null;
        var note = status.StartsWith("尚未鎖定", StringComparison.Ordinal) ? status : "";
        status = note + "整週排程算完了。套用後才會寫進生產計畫，遊戲裡的工坊生產計畫仍要自己填。";
    }

    private void DrawIntro()
    {
        ImGui.TextWrapped("這是整週預測，只算你自己排進去的產量。其他玩家的出貨也會改變世界供給，遊戲讀不到。若之後打開「海島產品需求調查」發現數值不同，請重新計算還沒過的日子。已經過的生產日無法從遊戲補回。");
        var today = SeasonUtils.GetCycle();
        if (today == 0 || PatternsWide(today))
        {
            ImGui.TextColored(WarningColor, "目前能對上的供需型態還很多，後面幾天是預測（標 *）。第1日到第4日都收集過會比較準，現在仍可先排。");
        }
        if (today == 0 && rest1 != 0 && rest2 != 0)
        {
            var restWarning = restLocked
                ? "第1日沒有排休息。這一天的型態通常還不準。"
                : "第1日沒有排休息。尚未鎖定休息日，計算時會改為第1日休息。";
            ImGui.TextColored(WarningColor, restWarning);
        }

        var collected = new List<string>();
        var times = uiDataSource.DataSource.DataCollectionTime;
        for (var d = 0; d < Constants.MaxCycles; d++)
        {
            if (times[d] != null)
            {
                collected.Add(DayNames[d]);
            }
        }
        ImGui.Text(collected.Count == 0 ? "本週還沒讀到海島產品需求調查。" : "已收集：" + string.Join("、", collected) + "。");
        ImGui.Spacing();
    }

    private void DrawRestControls()
    {
        var today = SeasonUtils.GetCycle();
        ImGui.SetNextItemWidth(120);
        if (ImGui.Combo("休息日", ref rest1, DayNames, DayNames.Length))
        {
            if (rest1 == rest2)
            {
                rest2 = (rest2 + 1) % Constants.MaxCycles;
            }
            RestChanged(today);
        }
        ImGui.SameLine();
        ImGui.SetNextItemWidth(120);
        if (ImGui.Combo("與", ref rest2, DayNames, DayNames.Length))
        {
            if (rest2 == rest1)
            {
                rest1 = (rest1 + 1) % Constants.MaxCycles;
            }
            RestChanged(today);
        }
        ImGui.SameLine();
        ImGui.Checkbox("鎖定休息日", ref restLocked);
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("鎖定後，計算會照你選的兩天。取消鎖定且現在是第1日、型態還很多時，會改回第1日休息。");
        }

        if (CountPlannedRests(today) != 2)
        {
            ImGui.TextColored(WarningColor, "這週的休息日不是兩天。已過的休息日無法改回，請確認還沒過的日子。");
        }
    }

    private void DrawActions()
    {
        var busy = running != null;
        if (busy)
        {
            ImGui.BeginDisabled();
        }
        var recalcLabel = plan == null ? "計算整週" : "重新計算剩餘天數";
        if (ImGui.Button(recalcLabel))
        {
            StartSearch(false);
        }
        ImGui.SameLine();
        if (ImGui.Button("全部重算"))
        {
            StartSearch(true);
        }
        ImGui.SameLine();
        if (plan == null)
        {
            ImGui.BeginDisabled();
        }
        if (ImGui.Button("套用整週到生產計畫"))
        {
            ApplyAll();
        }
        if (plan == null)
        {
            ImGui.EndDisabled();
        }
        if (busy)
        {
            ImGui.EndDisabled();
        }
        ImGui.SameLine();
        ifData.DrawFilteringControls();
    }

    private void DrawTable(WeekPlan shown, int today)
    {
        var flags = ImGuiTableFlags.ScrollY | ImGuiTableFlags.ScrollX | ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit;
        if (!ImGui.BeginTable("WeekPlan", 9, flags, new Vector2(0, ImGui.GetContentRegionAvail().Y)))
        {
            return;
        }

        ImGui.TableSetupColumn("日", ImGuiTableColumnFlags.WidthFixed, 90);
        ImGui.TableSetupColumn("狀態", ImGuiTableColumnFlags.WidthFixed, 150);
        ImGui.TableSetupColumn("開拓工坊", ImGuiTableColumnFlags.WidthFixed, 420);
        ImGui.TableSetupColumn("小時", ImGuiTableColumnFlags.WidthFixed, 160);
        ImGui.TableSetupColumn("型態", ImGuiTableColumnFlags.WidthFixed, 220);
        ImGui.TableSetupColumn("供需", ImGuiTableColumnFlags.WidthFixed, 420);
        ImGui.TableSetupColumn("氣勢", ImGuiTableColumnFlags.WidthFixed, 110);
        ImGui.TableSetupColumn("產值", ImGuiTableColumnFlags.WidthFixed, 90);
        ImGui.TableSetupColumn("操作", ImGuiTableColumnFlags.WidthFixed, 200);
        ImGui.TableSetupScrollFreeze(0, 1);
        ImGui.TableHeadersRow();

        var times = uiDataSource.DataSource.DataCollectionTime;
        for (var d = 0; d < Constants.MaxCycles; d++)
        {
            var day = shown.Days[d];
            var past = d < today;
            var collected = times[d] != null;
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text(d == today ? DayNames[d] + "（今日）" : DayNames[d]);

            ImGui.TableNextColumn();
            var state = day.Schedule.Rest ? "休息" : "工作";
            if (past)
            {
                state = "已過·" + state;
            }
            else if (locked[d] && !day.Schedule.Rest)
            {
                state = "已鎖定·" + state;
            }
            state += collected ? "·已收集" : "·預測*";
            ImGui.TextColored(collected ? CollectedColor : WarningColor, state);

            ImGui.TableNextColumn();
            DrawLines(day.WorkshopItems);
            ImGui.TableNextColumn();
            DrawLines(day.WorkshopHours);
            ImGui.TableNextColumn();
            DrawLines(day.WorkshopPatterns);
            ImGui.TableNextColumn();
            DrawLines(day.WorkshopMarkets);
            ImGui.TableNextColumn();
            ImGui.Text($"{day.StartGroove} → {day.EndGroove}");
            ImGui.TableNextColumn();
            ImGui.Text($"{day.Value:F2}");
            ImGui.TableNextColumn();
            DrawDayActions(d, past, shown);
        }
        ImGui.EndTable();
    }

    private void DrawDayActions(int day, bool past, WeekPlan shown)
    {
        if (past)
        {
            ImGui.Text("已過");
            return;
        }
        var busy = running != null;
        if (busy)
        {
            ImGui.BeginDisabled();
        }
        if (!shown.IsPreview)
        {
            if (ImGui.Button($"套用此日###week-apply-{day}"))
            {
                ApplyDay(day, true);
            }
            ImGui.SameLine();
        }
        if (ImGui.Button($"編輯###week-edit-{day}"))
        {
            ifData.SetCycle(day);
            status = $"已切到第{day + 1}日。請打開「生產計畫」修改；要在重算時保留，請先鎖定此日。";
        }
        if (!shown.Days[day].Schedule.Rest)
        {
            if (ImGui.Checkbox($"鎖定###week-lock-{day}", ref locked[day]) && locked[day] && plan != null && !plan.Stale)
            {
                ApplyDay(day, true);
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("鎖定後，重新計算不會改這一天。");
            }
        }
        if (busy)
        {
            ImGui.EndDisabled();
        }
    }

    private static void DrawLines(string[] lines)
    {
        var any = false;
        foreach (var line in lines)
        {
            if (line.Length == 0)
            {
                continue;
            }
            ImGui.Text(line);
            any = true;
        }
        if (!any)
        {
            ImGui.Text("—");
        }
    }

    private void EnsureRest()
    {
        if (restReady)
        {
            return;
        }
        var recorded = uiDataSource.DataSource.CurrentRestCycles;
        var found = new List<int>();
        for (var i = 0; i < Constants.MaxCycles; i++)
        {
            if (recorded[i])
            {
                found.Add(i);
            }
        }
        if (found.Count >= 2)
        {
            rest1 = found[0];
            rest2 = found[1];
        }
        else
        {
            rest1 = configuration.DefaultRestCycle1;
            rest2 = configuration.DefaultRestCycle2;
            if (found.Count == 1)
            {
                rest1 = found[0];
            }
            if (rest1 == rest2)
            {
                rest2 = (rest1 + 1) % Constants.MaxCycles;
            }
        }
        restLocked = rest1 != 0 && rest2 != 0;
        restReady = true;
    }

    private void StartSearch(bool clearLocks)
    {
        var today = SeasonUtils.GetCycle();
        if (clearLocks)
        {
            for (var d = today; d < Constants.MaxCycles; d++)
            {
                locked[d] = false;
            }
            status = "已取消還沒過的鎖定，正在重算整週。";
        }
        else
        {
            status = "計算中，請稍候……";
        }

        if (TryPreferFirstRestDay(today))
        {
            status = "尚未鎖定其他休息日，已改第1日為休息。後面幾天是預測。";
        }
        WriteFutureRest(today);

        var recorded = uiDataSource.DataSource.CurrentRestCycles;
        var rest = new bool[Constants.MaxCycles];
        var isFixed = new bool[Constants.MaxCycles];
        var schedules = new DaySchedule[Constants.MaxCycles];
        var produced = uiDataSource.DataSource.CurrentProducedItems;
        for (var d = 0; d < Constants.MaxCycles; d++)
        {
            if (d < today)
            {
                rest[d] = recorded[d];
                isFixed[d] = true;
            }
            else
            {
                rest[d] = d == rest1 || d == rest2;
                isFixed[d] = locked[d];
            }
            schedules[d] = rest[d]
                ? new DaySchedule { Rest = true }
                : DaySchedule.FromProduced(produced, d, false);
        }

        running = new WeekOptimizer(
            configuration,
            uiDataSource.DataSource.CurrentDynamicData,
            uiDataSource.WhenOverrides,
            ifData.Strictness,
            ifData.MultiCycleLimit,
            rest,
            isFixed,
            schedules,
            today);
    }

    private bool TryPreferFirstRestDay(int today)
    {
        if (restLocked || today != 0 || rest1 == 0 || rest2 == 0 || !PatternsWide(0))
        {
            return false;
        }
        var keep = Math.Max(rest1, rest2);
        rest1 = 0;
        rest2 = keep == 0 ? 4 : keep;
        restLocked = true;
        return true;
    }

    private void RestChanged(int today)
    {
        restLocked = true;
        if (running != null)
        {
            running = null;
            status = "休息日已改，請重新計算。";
        }
        WriteFutureRest(today);
        MarkStale();
    }

    private void WriteFutureRest(int today)
    {
        var recorded = uiDataSource.DataSource.CurrentRestCycles;
        var changed = false;
        for (var d = today; d < Constants.MaxCycles; d++)
        {
            var want = d == rest1 || d == rest2;
            if (recorded[d] == want)
            {
                continue;
            }
            recorded[d] = want;
            changed = true;
        }
        if (!changed)
        {
            return;
        }
        suppress = true;
        try
        {
            uiDataSource.DataChanged(today);
        }
        finally
        {
            suppress = false;
        }
    }

    private int CountPlannedRests(int today)
    {
        var recorded = uiDataSource.DataSource.CurrentRestCycles;
        var count = 0;
        for (var d = 0; d < Constants.MaxCycles; d++)
        {
            var isRest = d < today ? recorded[d] : d == rest1 || d == rest2;
            if (isRest)
            {
                count++;
            }
        }
        return count;
    }

    private bool PatternsWide(int cycle)
    {
        if (cycle < 0)
        {
            cycle = 0;
        }
        if (cycle >= Constants.MaxCycles)
        {
            cycle = Constants.MaxCycles - 1;
        }
        var cache = uiDataSource.CurrentItemCache;
        var valid = 0;
        var ambiguous = 0;
        for (uint i = 0; i < Constants.MaxItems; i++)
        {
            var staticData = ItemStaticData.Get(i);
            if (!staticData.IsValid())
            {
                continue;
            }
            valid++;
            if (cache[staticData].FindPatterns(cycle).Count != 1)
            {
                ambiguous++;
            }
        }
        return valid > 0 && ambiguous * 2 >= valid;
    }

    private WeekPlan BuildPreview()
    {
        var preview = new WeekPlan { IsPreview = true };
        var cache = uiDataSource.CurrentItemCache;
        var produced = uiDataSource.DataSource.CurrentProducedItems;
        var recorded = uiDataSource.DataSource.CurrentRestCycles;
        var today = SeasonUtils.GetCycle();
        var groove = new Groove();
        var total = 0.0;
        for (var d = 0; d < Constants.MaxCycles; d++)
        {
            var rest = d < today ? recorded[d] : d == rest1 || d == rest2;
            var schedule = rest ? new DaySchedule { Rest = true } : DaySchedule.FromProduced(produced, d, false);
            DayScheduleFormatter.Describe(preview.Days[d], schedule, cache, d, groove);
            groove = new Groove(preview.Days[d].EndGroove);
            total += preview.Days[d].Value;
        }
        preview.TotalValue = total;
        return preview;
    }

    private void ApplyAll()
    {
        if (plan == null)
        {
            return;
        }
        var today = SeasonUtils.GetCycle();
        suppress = true;
        try
        {
            for (var d = today; d < Constants.MaxCycles; d++)
            {
                WriteDay(d);
            }
            uiDataSource.DataChanged(today);
        }
        finally
        {
            suppress = false;
        }
        status = "已把還沒過的日子寫進生產計畫。請照著填進遊戲內的工坊生產計畫。";
    }

    private void ApplyDay(int day, bool notify)
    {
        if (plan == null || day < SeasonUtils.GetCycle())
        {
            return;
        }
        suppress = true;
        try
        {
            WriteDay(day);
            uiDataSource.DataChanged(day);
        }
        finally
        {
            suppress = false;
        }
        if (notify)
        {
            status = $"已把第{day + 1}日寫進生產計畫。";
        }
    }

    private void WriteDay(int day)
    {
        if (plan == null)
        {
            return;
        }
        var produced = uiDataSource.DataSource.CurrentProducedItems;
        var schedule = plan.Days[day].Schedule;
        for (var w = 0; w < Constants.MaxWorkshops; w++)
        {
            for (var s = 0; s < Constants.MaxSteps; s++)
            {
                produced[day, w, s] = schedule.Rest ? -1 : schedule.Items[w, s];
            }
        }
        uiDataSource.DataSource.CurrentRestCycles[day] = schedule.Rest;
    }

    private void MarkStale()
    {
        if (plan != null)
        {
            plan.Stale = true;
        }
    }
}
