using System.Collections.Generic;
using System.Diagnostics;

namespace WorkshopOptimizerPlugin.Data;

internal enum MaterialSource
{
    Gatherable,
    Granary,
    Crop,
    RareCrop,
    Dropping,
    RareDropping,
}

internal class Materials
{
    public readonly Material Material;
    public readonly int Count;

    public Materials(Material material, int count)
    {
        this.Material = material;
        Count = count;
    }
}

internal class Material
{
    private static readonly IDictionary<int, Material> MaterialList = new Dictionary<int, Material>();
    private static readonly IDictionary<uint, Material> UniqueMaterialIds = new Dictionary<uint, Material>();

    public static Material? GetMaterialByIndex(int index)
    {
        return MaterialList.TryGetValue(index, out var value)? value : null;
    }

    public static int MaxMaterials => MaterialList.Count;

    public static Material? GetMaterialById(uint id)
    {
        return UniqueMaterialIds.TryGetValue(id, out var value)? value : null;
    }


    public readonly uint Id;
    public readonly string Name;
    public readonly MaterialSource Source;

    public Materials this[int count] { get { return new Materials(this, count); } } 


    // Gatherable
    public static readonly Material PalmLeaf = new(37551, "無人島棕櫚葉", MaterialSource.Gatherable);
    public static readonly Material Branch = new(37553, "無人島小樹枝", MaterialSource.Gatherable);
    public static readonly Material Stone = new(37554, "無人島石材", MaterialSource.Gatherable);
    public static readonly Material Clam = new(37555, "無人島雙殼貝", MaterialSource.Gatherable);
    public static readonly Material Laver = new(37556, "無人島球藻", MaterialSource.Gatherable);
    public static readonly Material Coral = new(37557, "無人島珊瑚", MaterialSource.Gatherable);
    public static readonly Material Islewort = new(37558, "無人島草葉", MaterialSource.Gatherable);
    public static readonly Material Sand = new(37559, "無人島砂礫", MaterialSource.Gatherable);
    public static readonly Material Vine = new(37562, "無人島藤蔓", MaterialSource.Gatherable);
    public static readonly Material Sap = new(37563, "無人島樹液", MaterialSource.Gatherable);
    public static readonly Material Apple = new(37552, "無人島蘋果", MaterialSource.Gatherable);
    public static readonly Material Log = new(37560, "無人島原木", MaterialSource.Gatherable);
    public static readonly Material PalmLog = new(37561, "無人島棕櫚原木", MaterialSource.Gatherable);
    public static readonly Material CopperOre = new(37564, "無人島銅礦", MaterialSource.Gatherable);
    public static readonly Material Limestone = new(37565, "無人島石灰岩", MaterialSource.Gatherable);
    public static readonly Material RockSalt = new(37566, "無人島岩鹽", MaterialSource.Gatherable);
    public static readonly Material Clay = new(37570, "無人島黏土", MaterialSource.Gatherable);
    public static readonly Material Tinsand = new(37571, "無人島錫砂", MaterialSource.Gatherable);
    public static readonly Material Sugarcane = new(37567, "無人島甘蔗", MaterialSource.Gatherable);
    public static readonly Material CottonBoll = new(37568, "無人島棉花", MaterialSource.Gatherable);
    public static readonly Material Hemp = new(37569, "無人島麻", MaterialSource.Gatherable);
    public static readonly Material Islefish = new(37575, "無人島海水魚", MaterialSource.Gatherable);
    public static readonly Material Squid = new(37576, "無人島烏賊", MaterialSource.Gatherable);
    public static readonly Material Jellyfish = new(37577, "無人島水母", MaterialSource.Gatherable);
    public static readonly Material IronOre = new(37572, "無人島鐵礦", MaterialSource.Gatherable);
    public static readonly Material Quartz = new(37573, "無人島石英", MaterialSource.Gatherable);
    public static readonly Material Leucogranite = new(37574, "無人島花崗岩", MaterialSource.Gatherable);
    public static readonly Material MulticoloredIslebloooms = new(39228, "無人島色彩繽紛的花", MaterialSource.Gatherable);
    public static readonly Material Resin = new(39224, "無人島天然樹脂", MaterialSource.Gatherable);
    public static readonly Material Coconut = new(39225, "無人島椰子", MaterialSource.Gatherable);
    public static readonly Material BeehiveChip = new(39226, "無人島蜂巢", MaterialSource.Gatherable);
    public static readonly Material WoodOpal = new(39227, "無人島木化石", MaterialSource.Gatherable);
    public static readonly Material Coal = new(39887, "無人島石炭", MaterialSource.Gatherable);
    public static readonly Material Glimshroom = new(39889, "無人島燈火茸", MaterialSource.Gatherable);
    public static readonly Material EffervescentWater = new(39892, "無人島氣泡水", MaterialSource.Gatherable);
    public static readonly Material Shale = new(39888, "無人島堆積岩", MaterialSource.Gatherable);
    public static readonly Material Marble = new(39890, "無人島大理石", MaterialSource.Gatherable);
    public static readonly Material MythrilOre = new(39891, "無人島靈銀礦", MaterialSource.Gatherable);
    public static readonly Material Spectrine = new(39893, "無人島幻影石", MaterialSource.Gatherable);
    public static readonly Material DuriumSand = new(41630, "無人島翠銀沙", MaterialSource.Gatherable);
    public static readonly Material YellowCopperOre = new(41631, "無人島黃銅礦", MaterialSource.Gatherable);
    public static readonly Material GoldOre = new(41632, "無人島金礦", MaterialSource.Gatherable);
    public static readonly Material HawksEyeSand = new(41633, "無人島鷹眼砂", MaterialSource.Gatherable);
    public static readonly Material CrystalFormation = new(41634, "無人島水晶層", MaterialSource.Gatherable);

    // Granary
    public static readonly Material Alyssum = new(37578, "無人島香薺", MaterialSource.Granary);
    public static readonly Material RawGarnet = new(37579, "無人島石榴石原石", MaterialSource.Granary);
    public static readonly Material SpruceLog = new(37580, "無人島雲杉原木", MaterialSource.Granary);
    public static readonly Material Hammerhead = new(37581, "無人島錘頭鯊", MaterialSource.Granary);
    public static readonly Material SilverOre = new(37582, "無人島銀礦", MaterialSource.Granary);
    public static readonly Material CaveShrimp = new(39894, "無人島長臂蝦", MaterialSource.Granary);

    // Crops
    public static readonly Material Popoto = new(37593, "海島新薯", MaterialSource.Crop);
    public static readonly Material Cabbage = new(37594, "海島甘藍", MaterialSource.Crop);
    public static readonly Material Isleberry = new(37595, "海島莓果", MaterialSource.RareCrop);
    public static readonly Material Pumpkin = new(37596, "海島南瓜", MaterialSource.Crop);
    public static readonly Material Onion = new(37597, "海島洋蔥", MaterialSource.RareCrop);
    public static readonly Material Tomato = new(37598, "海島番茄", MaterialSource.RareCrop);
    public static readonly Material Wheat = new(37599, "海島小麥", MaterialSource.RareCrop);
    public static readonly Material Corn = new(37600, "海島玉米", MaterialSource.RareCrop);
    public static readonly Material Parsnip = new(37601, "海島高山蘿蔔", MaterialSource.Crop);
    public static readonly Material Radish = new(37602, "海島小蘿蔔", MaterialSource.RareCrop);


    // Rare Crop
    public static readonly Material Paprika = new(39231, "海島紅彩椒", MaterialSource.RareCrop);
    public static readonly Material Leek = new(39232, "海島韭蔥", MaterialSource.RareCrop);
    public static readonly Material RunnerBeans = new(39899, "海島花豆", MaterialSource.RareCrop);
    public static readonly Material Beet = new(39900, "海島紅菜", MaterialSource.RareCrop);
    public static readonly Material Eggplant = new(39901, "海島茄子", MaterialSource.RareCrop);
    public static readonly Material Zucchini = new(39902, "海島櫛瓜", MaterialSource.RareCrop);
    public static readonly Material Watermelon = new(41635, "海島西瓜", MaterialSource.RareCrop);
    public static readonly Material SweetPopoto = new(41636, "海島甜新薯", MaterialSource.RareCrop);
    public static readonly Material Broccoli = new(41637, "海島花椰菜", MaterialSource.RareCrop);
    public static readonly Material BuffaloBeans = new(41638, "海島水牛豆", MaterialSource.RareCrop);

    // Dropping
    public static readonly Material Fleece = new(37603, "牧場動物的絨毛", MaterialSource.Dropping);
    public static readonly Material Claw = new(37604, "牧場動物的尖爪", MaterialSource.Dropping);
    public static readonly Material Fur = new(37605, "牧場動物的獸毛", MaterialSource.Dropping);
    public static readonly Material Feather = new(37606, "牧場動物的羽毛", MaterialSource.Dropping);
    public static readonly Material Egg = new(37607, "牧場動物的鳥蛋", MaterialSource.Dropping);

    // Rare Dropping
    public static readonly Material Carapace = new(37608, "牧場動物的甲殼", MaterialSource.RareDropping);
    public static readonly Material Fang = new(37609, "牧場動物的尖牙", MaterialSource.RareDropping);
    public static readonly Material Horn = new(37610, "牧場動物的角片", MaterialSource.RareDropping);
    public static readonly Material Milk = new(37611, "牧場動物的乳汁", MaterialSource.RareDropping);

    private Material(uint id, string name, MaterialSource source)
    {
        Id = id;
        Name = name;
        Source = source;

        MaterialList[MaterialList.Count] = this;
        Debug.Assert(UniqueMaterialIds.TryAdd(id, this), $"Duplicate id {id}");
    }
}
