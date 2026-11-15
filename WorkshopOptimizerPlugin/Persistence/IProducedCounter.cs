namespace WorkshopOptimizerPlugin.Persistence;

internal interface IProducedCounter
{
    int ItemsProducedUntil(int cycle, uint itemId);
}
