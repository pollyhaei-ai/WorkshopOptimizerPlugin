namespace WorkshopOptimizerPlugin.Data;

internal class GranaryItemData
{
    public readonly uint Id;
    public readonly string Component;
    public readonly string Mission;

    public static readonly GranaryItemData[] Items = new GranaryItemData[]
    {
        new GranaryItemData(20 /* 開拓工坊植物成長劑 */, "無人島香薺", "草原"),
        new GranaryItemData(21 /* 海島石榴石刺劍 */, "無人島石榴石原石", "溪流"),
        new GranaryItemData(22 /* 海島雲杉圓盾 */, "無人島雲杉原木", "森林"),
        new GranaryItemData(23 /* 開拓工坊鯊油 */, "無人島錘頭鯊", "沙灘"),
        new GranaryItemData(24 /* 海島白銀耳夾 */, "無人島銀礦", "山"),
        new GranaryItemData(64 /* 開拓工坊五海雜燴湯 */, "無人島長臂蝦", "洞窟"),
    };

    private GranaryItemData(uint id, string component, string mission)
    {
        Id = id;
        Component = component;
        Mission = mission;
    }
}
