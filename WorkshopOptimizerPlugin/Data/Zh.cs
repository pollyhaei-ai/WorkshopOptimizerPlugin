namespace WorkshopOptimizerPlugin.Data;

/// <summary>
/// Traditional Chinese (Taiwan client) labels.
/// Item and material names come from the Item sheet.
/// Workshop UI terms come from Addon.csv / MJIText.csv of the Taiwan client.
/// </summary>
internal static class Zh
{
    public static string Supply(Supply value) => value switch
    {
        Data.Supply.Nonexistent => "嚴重供少於求",
        Data.Supply.Insufficient => "供少於求",
        Data.Supply.Sufficient => "供求正常",
        Data.Supply.Surplus => "供過於求",
        Data.Supply.Overflowing => "嚴重供過於求",
        _ => "未知",
    };

    public static string Demand(Demand value) => value switch
    {
        Data.Demand.Skyrocketing => "需求大幅增加",
        Data.Demand.Increasing => "需求增加",
        Data.Demand.None => "無變化",
        Data.Demand.Decreasing => "需求降低",
        Data.Demand.Plummeting => "需求大幅降低",
        Data.Demand.Any => "任意",
        _ => "未知",
    };

    public static string Popularity(Popularity value) => value switch
    {
        Data.Popularity.VeryHigh => "人氣火爆",
        Data.Popularity.High => "略受矚目",
        Data.Popularity.Average => "普普通通",
        Data.Popularity.Low => "無人問津",
        _ => "未知",
    };

    public static string When(When value) => value switch
    {
        Data.When.Never => "不使用",
        Data.When.Weak => "僅弱週期",
        Data.When.Strong => "僅強週期",
        Data.When.Either => "弱或強",
        Data.When.Always => "總是",
        Data.When.Required => "必須",
        _ => "未知",
    };

    public static string Source(MaterialSource value) => value switch
    {
        MaterialSource.Gatherable => "開拓素材",
        MaterialSource.Granary => "囤貨倉庫",
        MaterialSource.Crop => "開拓作物",
        MaterialSource.RareCrop => "稀少作物",
        MaterialSource.Dropping => "畜牧素材",
        MaterialSource.RareDropping => "稀少畜牧素材",
        _ => "未知",
    };

    // Official MJICraftworksObjectTheme names. Not drawn as their own column;
    // kept so category labels stay the Taiwan client strings if a caller needs them.
    public static string Category(Categories value) => value switch
    {
        Categories.PreservedFood => "耐儲存食品",
        Categories.Attire => "防具",
        Categories.Foodstuffs => "菜餚",
        Categories.Confections => "點心",
        Categories.Sundries => "日用品",
        Categories.Furnishings => "傢俱",
        Categories.Arms => "武器",
        Categories.Concoctions => "藥品",
        Categories.Ingredients => "調味料",
        Categories.Accessories => "飾品",
        Categories.Metalworks => "金屬製品",
        Categories.Woodworks => "木工製品",
        Categories.Textiles => "紡織製品",
        Categories.CreatureCreations => "動物製品",
        Categories.MarineMerchandise => "海產製品",
        Categories.UnburiedTreasures => "窯爐製品",
        _ => "未知",
    };
}
