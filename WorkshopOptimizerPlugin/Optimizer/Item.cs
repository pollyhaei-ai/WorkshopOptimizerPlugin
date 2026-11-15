using System.Collections.Generic;
using WorkshopOptimizerPlugin.Data;
using WorkshopOptimizerPlugin.Persistence;

namespace WorkshopOptimizerPlugin.Optimizer;

internal class Item
{
    // Static Data
    public uint Id => staticData.Id;
    public string Name => staticData.Name;
    public int Hours => staticData.Hours;
    public int Value => staticData.Value;
    public When When => whenOverrides[staticData.Id];
    public Categories[] Categories => staticData.Categories;
    public Materials[] Materials => staticData.Materials;
    public int MinLevel => staticData.MinLevel;

    // Dynamic Data
    public Popularity Popularity => dynamicData.Popularity;
    public Popularity NextPopularity => dynamicData.NextPopularity;
    public Supply[] Supply => dynamicData.Supply;
    public Demand[] Demand => dynamicData.Demand;

    public Item(ItemStaticData staticData, ItemDynamicData dynamicData, IProducedCounter producedItems, WhenOverrides whenOverrides)
    {
        this.staticData = staticData;
        this.dynamicData = dynamicData;
        this.producedItems = producedItems;
        this.whenOverrides = whenOverrides;
    }

    public int AccumulatedProduced(int cycle)
    {
        return producedItems.ItemsProducedUntil(cycle, this.Id);
    }

    public List<SupplyDemandPattern> FindPatterns(int cycle)
    {
        if (cycle > 4)
        {
            cycle = 4;
        }

        var ret = new List<SupplyDemandPattern>();

        foreach (var pattern in SupplyDemandPattern.PatternsTable)
        {
            var mismatch = false;
            for (var i = 0; !mismatch && i < cycle; i++)
            {
                // An uncollected day must not reject every pattern. Later days are
                // forecast by averaging the patterns that still fit the days we have.
                if (!IsCollected(Supply[i], Demand[i]))
                {
                    continue;
                }
                mismatch = (pattern.SupplyPattern[i] != Supply[i]) ||
                           ((pattern.DemandPattern[i] != Data.Demand.Any) && (pattern.DemandPattern[i] != Demand[i]));
            }
            if (!mismatch)
            {
                ret.Add(pattern);
            }
        }
        return ret;
    }

    public string PatternLabel(int cycle)
    {
        var patterns = FindPatterns(cycle);
        if (patterns.Count == 0)
        {
            return "?";
        }
        if (patterns.Count != 1)
        {
            return "*";
        }
        var name = patterns[0].Name;
        return Supply[cycle] == Data.Supply.Unknown ? name + "*" : name;
    }

    public string ForecastSupplyLabel(int cycle)
    {
        if (Supply[cycle] != Data.Supply.Unknown)
        {
            return Zh.Supply(Supply[cycle]);
        }

        var produced = AccumulatedProduced(cycle);
        var patterns = FindPatterns(cycle);
        if (patterns.Count == 0)
        {
            return Zh.Supply(SupplyUtils.Advance(Data.Supply.Sufficient, produced)) + "*";
        }

        var have = false;
        var shared = Data.Supply.Unknown;
        foreach (var pattern in patterns)
        {
            var supply = SupplyUtils.Advance(pattern.SupplyPattern[cycle], produced);
            if (!have)
            {
                shared = supply;
                have = true;
            }
            else if (shared != supply)
            {
                return "*";
            }
        }
        return Zh.Supply(shared) + "*";
    }

    public string ForecastDemandLabel(int cycle)
    {
        if (Demand[cycle] != Data.Demand.Unknown)
        {
            return Zh.Demand(Demand[cycle]);
        }

        var produced = AccumulatedProduced(cycle);
        var patterns = FindPatterns(cycle);
        if (patterns.Count == 0)
        {
            return Zh.Demand(DemandUtils.Adjust(Data.Demand.None, produced).Item1) + "*";
        }

        var have = false;
        var shared = Data.Demand.Unknown;
        var sawAny = false;
        foreach (var pattern in patterns)
        {
            var raw = pattern.DemandPattern[cycle];
            if (raw == Data.Demand.Any)
            {
                sawAny = true;
                continue;
            }
            var demand = DemandUtils.Adjust(raw, produced).Item1;
            if (!have)
            {
                shared = demand;
                have = true;
            }
            else if (shared != demand)
            {
                return "*";
            }
        }
        if (!have)
        {
            return sawAny ? Zh.Demand(Data.Demand.Any) + "*" : "*";
        }
        if (sawAny)
        {
            return "*";
        }
        return Zh.Demand(shared) + "*";
    }

    private static bool IsCollected(Supply supply, Demand demand)
    {
        return supply != Data.Supply.Unknown && demand != Data.Demand.Unknown;
    }

    public (SupplyDemandPattern?, bool) FindPattern(int cycle)
    {
        var patterns = FindPatterns(cycle);
        if (patterns.Count != 1 || patterns[0].Cycle != cycle)
        {
            return (null, patterns.Count > 0);
        }

        return (patterns[0], true);
    }

    public double EffectiveValue(int cycle, int add = 0)
    {
        var produced = AccumulatedProduced(cycle) + add;

        var sup_mult = 0.0;
        var n = 0;
        if (Supply[cycle] != Data.Supply.Unknown)
        {
            sup_mult += SupplyUtils.Multiplier(Supply[cycle], produced);
            n++;
        }
        else
        {
            var patterns = FindPatterns(cycle);
            if (patterns.Count == 0)
            {
                sup_mult += SupplyUtils.Multiplier(Data.Supply.Sufficient, produced);
                n++;
            }
            else
            {

                foreach (var pattern in patterns)
                {
                    sup_mult += SupplyUtils.Multiplier(pattern.SupplyPattern[cycle], produced);
                    n++;
                }
            }
        }

        return ((double)Value) * PopularityUtils.Multiplier(Popularity) * (sup_mult / n);
    }

    private readonly ItemStaticData staticData;
    private readonly ItemDynamicData dynamicData;
    private readonly IProducedCounter producedItems;
    private readonly WhenOverrides whenOverrides;
}
