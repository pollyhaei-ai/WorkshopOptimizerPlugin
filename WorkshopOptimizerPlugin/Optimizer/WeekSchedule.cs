using System.Collections.Generic;
using System.Linq;
using WorkshopOptimizerPlugin.Data;
using WorkshopOptimizerPlugin.Persistence;

namespace WorkshopOptimizerPlugin.Optimizer;

internal sealed class DaySchedule
{
    public bool Rest;
    public readonly int[,] Items = new int[Constants.MaxWorkshops, Constants.MaxSteps];

    public DaySchedule()
    {
        Clear();
    }

    public void Clear()
    {
        for (var w = 0; w < Constants.MaxWorkshops; w++)
        {
            for (var s = 0; s < Constants.MaxSteps; s++)
            {
                Items[w, s] = -1;
            }
        }
    }

    public bool IsEmpty()
    {
        if (Rest)
        {
            return true;
        }
        for (var w = 0; w < Constants.MaxWorkshops; w++)
        {
            if (Items[w, 0] >= 0)
            {
                return false;
            }
        }
        return true;
    }

    public DaySchedule Clone()
    {
        var copy = new DaySchedule { Rest = Rest };
        System.Array.Copy(Items, copy.Items, Items.Length);
        return copy;
    }

    public static DaySchedule FromProduced(ProducedItemsAdaptor produced, int cycle, bool rest)
    {
        var day = new DaySchedule { Rest = rest };
        if (rest)
        {
            return day;
        }
        for (var w = 0; w < Constants.MaxWorkshops; w++)
        {
            for (var s = 0; s < Constants.MaxSteps; s++)
            {
                day.Items[w, s] = produced[cycle, w, s];
            }
        }
        return day;
    }

    public static DaySchedule FromWorkshops(WorkshopsItemSets workshops)
    {
        var day = new DaySchedule();
        for (var w = 0; w < Constants.MaxWorkshops; w++)
        {
            var items = workshops.ItemSets[w].Items;
            for (var s = 0; s < Constants.MaxSteps; s++)
            {
                day.Items[w, s] = s < items.Length ? (int)items[s].Id : -1;
            }
        }
        return day;
    }
}

internal sealed class DayResult
{
    public DaySchedule Schedule = new();
    public int StartGroove;
    public int EndGroove;
    public double Value;
    public readonly string[] WorkshopItems = Blank();
    public readonly string[] WorkshopHours = Blank();
    public readonly string[] WorkshopPatterns = Blank();
    public readonly string[] WorkshopMarkets = Blank();

    private static string[] Blank()
    {
        var values = new string[Constants.MaxWorkshops];
        for (var i = 0; i < values.Length; i++)
        {
            values[i] = "";
        }
        return values;
    }
}

internal sealed class WeekPlan
{
    public readonly DayResult[] Days = new DayResult[Constants.MaxCycles];
    public double TotalValue;
    public bool IsPreview;
    public bool Stale;

    public WeekPlan()
    {
        for (var i = 0; i < Days.Length; i++)
        {
            Days[i] = new DayResult();
        }
    }
}

internal static class DayScheduleFormatter
{
    public static void Describe(DayResult result, DaySchedule schedule, ItemCache cache, int cycle, Groove start)
    {
        result.Schedule = schedule;
        result.StartGroove = start.Value;
        result.EndGroove = start.Value;
        result.Value = 0;
        for (var w = 0; w < Constants.MaxWorkshops; w++)
        {
            result.WorkshopItems[w] = "";
            result.WorkshopHours[w] = "";
            result.WorkshopPatterns[w] = "";
            result.WorkshopMarkets[w] = "";
        }

        if (schedule.Rest)
        {
            result.WorkshopItems[0] = "休息";
            return;
        }

        if (schedule.IsEmpty())
        {
            result.WorkshopItems[0] = "（沒有符合條件的產品）";
            return;
        }

        var workshops = ToWorkshops(cache, schedule, cycle, start);
        result.EndGroove = workshops.EndGroove.Value;
        result.Value = workshops.EffectiveValue;
        for (var w = 0; w < Constants.MaxWorkshops; w++)
        {
            var itemSet = workshops.ItemSets[w];
            if (itemSet.Items.Length == 0)
            {
                result.WorkshopItems[w] = "—";
                result.WorkshopHours[w] = "—";
                result.WorkshopPatterns[w] = "—";
                result.WorkshopMarkets[w] = "—";
                continue;
            }
            result.WorkshopItems[w] = string.Join("/", itemSet.Items.Select(item => item.Name));
            result.WorkshopHours[w] = string.Join("/", itemSet.Items.Select(item => item.Hours.ToString())) + $"（{itemSet.Hours}）";
            result.WorkshopPatterns[w] = string.Join("/", itemSet.Items.Select(item => item.PatternLabel(cycle)));
            result.WorkshopMarkets[w] = string.Join("/", itemSet.Items.Select(item => item.ForecastSupplyLabel(cycle) + "·" + item.ForecastDemandLabel(cycle)));
        }
    }

    public static WorkshopsItemSets ToWorkshops(ItemCache cache, DaySchedule schedule, int cycle, Groove groove)
    {
        var sets = new ItemSet[Constants.MaxWorkshops];
        for (var w = 0; w < Constants.MaxWorkshops; w++)
        {
            var items = new List<Item>();
            for (var s = 0; s < Constants.MaxSteps; s++)
            {
                var id = schedule.Items[w, s];
                if (id < 0)
                {
                    break;
                }
                var staticData = ItemStaticData.Get((uint)id);
                if (!staticData.IsValid())
                {
                    break;
                }
                items.Add(cache[staticData]);
            }
            sets[w] = new ItemSet(items.ToArray());
        }
        return new WorkshopsItemSets(sets, cycle, groove);
    }
}
