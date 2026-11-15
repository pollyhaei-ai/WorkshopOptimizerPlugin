using WorkshopOptimizerPlugin.Data;
using WorkshopOptimizerPlugin.Persistence;

namespace WorkshopOptimizerPlugin.Optimizer;

internal sealed class PlannedProduction : IProducedCounter
{
    private readonly int[,] counts;

    public PlannedProduction()
    {
        counts = new int[Constants.MaxCycles, Constants.MaxItems];
    }

    private PlannedProduction(int[,] counts)
    {
        this.counts = counts;
    }

    public PlannedProduction Clone()
    {
        var copy = new int[Constants.MaxCycles, Constants.MaxItems];
        System.Array.Copy(counts, copy, counts.Length);
        return new PlannedProduction(copy);
    }

    public void AddSchedule(int cycle, DaySchedule schedule)
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
                counts[cycle, id] += ItemSet.ItemsPerStep(s);
            }
        }
    }

    public int ItemsProducedUntil(int cycle, uint itemId)
    {
        if (itemId >= Constants.MaxItems)
        {
            return 0;
        }
        var sum = 0;
        var last = cycle < Constants.MaxCycles ? cycle : Constants.MaxCycles;
        for (var c = 0; c < last; c++)
        {
            sum += counts[c, itemId];
        }
        return sum;
    }
}
