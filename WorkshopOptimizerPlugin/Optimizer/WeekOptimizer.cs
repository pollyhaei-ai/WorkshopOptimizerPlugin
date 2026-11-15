using System;
using System.Collections.Generic;
using System.Diagnostics;
using WorkshopOptimizerPlugin.Data;
using WorkshopOptimizerPlugin.Persistence;

namespace WorkshopOptimizerPlugin.Optimizer;

internal sealed class WeekOptimizer
{
    public const int BeamWidth = 4;
    public const int PlansPerState = 3;

    private readonly Configuration configuration;
    private readonly DynamicDataAdaptor dynamicData;
    private readonly WhenOverrides whenOverrides;
    private readonly Strictness strictness;
    private readonly int multiCycleLimit;
    private readonly bool[] rest;
    private readonly bool[] fixedDay;
    private readonly DaySchedule[] fixedSchedules;
    private readonly List<SearchState> expansions = new();

    private List<SearchState> beam;
    private Optimizer? current;
    private int day;
    private int stateIndex;
    private bool finished;
    private WeekPlan? result;
    private double innerProgress;

    public double Progress { get; private set; }
    public int CurrentDay { get; private set; }

    public WeekOptimizer(
        Configuration configuration,
        DynamicDataAdaptor dynamicData,
        WhenOverrides whenOverrides,
        Strictness strictness,
        int multiCycleLimit,
        bool[] rest,
        bool[] fixedDay,
        DaySchedule[] fixedSchedules,
        int today)
    {
        this.configuration = configuration;
        this.dynamicData = dynamicData;
        this.whenOverrides = whenOverrides;
        this.strictness = strictness;
        this.multiCycleLimit = multiCycleLimit;
        this.rest = (bool[])rest.Clone();
        this.fixedDay = (bool[])fixedDay.Clone();
        this.fixedSchedules = new DaySchedule[Constants.MaxCycles];
        for (var i = 0; i < Constants.MaxCycles; i++)
        {
            this.fixedSchedules[i] = fixedSchedules[i].Clone();
            if (i < today)
            {
                this.fixedDay[i] = true;
            }
        }

        var initial = new SearchState();
        for (var d = 0; d < Constants.MaxCycles; d++)
        {
            if (!this.fixedDay[d] || this.rest[d])
            {
                continue;
            }
            initial.Production.AddSchedule(d, this.fixedSchedules[d]);
            Reserve(initial.Reserved, this.fixedSchedules[d]);
        }
        beam = new List<SearchState> { initial };
    }

    public WeekPlan? Advance(TimeSpan budget)
    {
        if (finished)
        {
            return result;
        }

        var watch = Stopwatch.StartNew();
        var steps = 0;
        while (!finished && watch.Elapsed < budget && steps < 10000)
        {
            StepOnce();
            steps++;
        }
        return finished ? result : null;
    }

    private void StepOnce()
    {
        if (day >= Constants.MaxCycles)
        {
            Finish();
            return;
        }

        CurrentDay = day;
        if (rest[day] || fixedDay[day])
        {
            for (var i = 0; i < beam.Count; i++)
            {
                ApplyKnownDay(beam[i], day);
            }
            day++;
            stateIndex = 0;
            current = null;
            innerProgress = 1;
            UpdateProgress();
            return;
        }

        if (current == null)
        {
            var state = beam[stateIndex];
            var cache = new ItemCache(dynamicData, state.Production, whenOverrides);
            var options = new OptimizerOptions(configuration, strictness, multiCycleLimit, rest, state.Reserved);
            current = new Optimizer(cache, day, state.Groove, options);
        }

        if (!current.IsCombinationsDone)
        {
            innerProgress = current.StepCombinations() * 0.5;
            UpdateProgress();
            return;
        }

        innerProgress = current.StepWorkshops(80);
        UpdateProgress();
        if (!current.IsWorkshopsDone)
        {
            return;
        }

            ExpandCurrentState();
            current = null;
            innerProgress = 1;
            stateIndex++;
            if (stateIndex >= beam.Count)
            {
                beam = new List<SearchState>(expansions);
                beam.Sort((a, b) => b.Value.CompareTo(a.Value));
                if (beam.Count > BeamWidth)
                {
                    beam.RemoveRange(BeamWidth, beam.Count - BeamWidth);
                }
                expansions.Clear();
                day++;
                stateIndex = 0;
                UpdateProgress();
            }
    }

    private void ExpandCurrentState()
    {
        var state = beam[stateIndex];
        var results = current!.WorkshopResults;
        var take = Math.Min(PlansPerState, results.Count);
        if (take == 0)
        {
            var empty = state.Clone();
            empty.Days[day] = new DaySchedule();
            expansions.Add(empty);
            return;
        }

        for (var i = 0; i < take; i++)
        {
            var chosen = results[i];
            var next = state.Clone();
            var schedule = DaySchedule.FromWorkshops(chosen);
            next.Days[day] = schedule;
            next.Value += chosen.EffectiveValue;
            next.Groove = chosen.EndGroove;
            next.Production.AddSchedule(day, schedule);
            Reserve(next.Reserved, schedule);
            expansions.Add(next);
        }
    }

    private void ApplyKnownDay(SearchState state, int dayIndex)
    {
        if (rest[dayIndex])
        {
            state.Days[dayIndex] = new DaySchedule { Rest = true };
            return;
        }

        var schedule = fixedSchedules[dayIndex].Clone();
        schedule.Rest = false;
        state.Days[dayIndex] = schedule;
        var cache = new ItemCache(dynamicData, state.Production, whenOverrides);
        var workshops = DayScheduleFormatter.ToWorkshops(cache, schedule, dayIndex, state.Groove);
        state.Value += workshops.EffectiveValue;
        state.Groove = workshops.EndGroove;
    }

    private void Finish()
    {
        finished = true;
        Progress = 1;
        if (beam.Count == 0)
        {
            result = new WeekPlan();
            return;
        }

        beam.Sort((a, b) => b.Value.CompareTo(a.Value));
        var best = beam[0];
        var cache = new ItemCache(dynamicData, best.Production, whenOverrides);
        var plan = new WeekPlan();
        var groove = new Groove();
        var total = 0.0;
        for (var d = 0; d < Constants.MaxCycles; d++)
        {
            var schedule = best.Days[d] ?? new DaySchedule { Rest = rest[d] };
            DayScheduleFormatter.Describe(plan.Days[d], schedule, cache, d, groove);
            groove = new Groove(plan.Days[d].EndGroove);
            total += plan.Days[d].Value;
        }
        plan.TotalValue = total;
        result = plan;
    }

    private void UpdateProgress()
    {
        var stateCount = Math.Max(1, beam.Count);
        var fraction = (stateIndex + Math.Clamp(innerProgress, 0, 1)) / stateCount;
        Progress = Math.Clamp((day + fraction) / Constants.MaxCycles, 0, finished ? 1 : 0.99);
    }

    private static void Reserve(Dictionary<uint, int> reserved, DaySchedule schedule)
    {
        if (schedule.Rest)
        {
            return;
        }
        for (var w = 0; w < Constants.MaxWorkshops; w++)
        {
            for (var s = 0; s < Constants.MaxSteps; s++)
            {
                var id = schedule.Items[w, s];
                if (id < 0 || id >= Constants.MaxItems)
                {
                    continue;
                }
                var staticData = ItemStaticData.Get((uint)id);
                if (!staticData.IsValid())
                {
                    continue;
                }
                var quantity = ItemSet.ItemsPerStep(s);
                foreach (var material in staticData.Materials)
                {
                    reserved.TryGetValue(material.Material.Id, out var used);
                    reserved[material.Material.Id] = used + (material.Count * quantity);
                }
            }
        }
    }

    private sealed class SearchState
    {
        public PlannedProduction Production = new();
        public Dictionary<uint, int> Reserved = new();
        public Groove Groove = new();
        public double Value;
        public readonly DaySchedule?[] Days = new DaySchedule?[Constants.MaxCycles];

        public SearchState Clone()
        {
            var copy = new SearchState
            {
                Production = Production.Clone(),
                Reserved = new Dictionary<uint, int>(Reserved),
                Groove = Groove,
                Value = Value,
            };
            for (var i = 0; i < Days.Length; i++)
            {
                copy.Days[i] = Days[i]?.Clone();
            }
            return copy;
        }
    }
}
