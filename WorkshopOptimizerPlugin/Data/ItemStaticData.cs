using System.Collections.Immutable;
using System.Diagnostics;

namespace WorkshopOptimizerPlugin.Data;

internal class ItemStaticData
{
    public readonly uint Id;
    public readonly string Name;
    public readonly int MinLevel;
    public readonly int Hours;
    public readonly int Value;
    public readonly When When;
    public readonly Categories[] Categories;
    public readonly Materials[] Materials;

    public bool IsValid()
    {
        return Value != 0;
    }

    public static ItemStaticData Get(uint item_index)
    {
        return ItemsData[item_index];
    }

    public static ItemStaticData Get(int item_index)
    {
        return ItemsData[item_index];
    }

    public static ImmutableArray<ItemStaticData> GetAllItems()
    {
        return ItemsData.ToImmutableArray();
    }

    private ItemStaticData(uint id, string name, int minLevel, int hours, int base_value, When when, Categories[] categories, Materials[] materials)
    {
        Id = id;
        Name = name;
        MinLevel = minLevel;
        Hours = hours;
        Value = (int)(base_value * Constants.WorkshopLevelMultiplier);
        When = when;
        Categories = categories;
        Materials = materials;
    }

    static ItemStaticData()
    {
        Debug.Assert(ItemsData.Length == Constants.MaxItems);
    }

    private static readonly ItemStaticData[] ItemsData = {
        new ItemStaticData( 0, "未知",                0, 0,   0, When.Never,  System.Array.Empty<Categories>(),                                                       System.Array.Empty<Materials>()),
        new ItemStaticData( 1, "海島治療劑",                 1, 4,  28, When.Always, new Categories[]{Data.Categories.Concoctions},                                          new Materials[]{Material.PalmLeaf[2], Material.Islewort[2]}),
        new ItemStaticData( 2, "開拓工坊火藥",               1, 4,  28, When.Always, new Categories[]{Data.Categories.Concoctions, Data.Categories.UnburiedTreasures},       new Materials[]{Material.Sand[2], Material.Limestone[1], Material.Islewort[1]}),
        new ItemStaticData( 3, "海島座椅",           1, 6,  42, When.Always, new Categories[]{Data.Categories.Furnishings, Data.Categories.Woodworks},               new Materials[]{Material.Log[4], Material.Vine[2]}),
        new ItemStaticData( 4, "開拓工坊烤扇貝",           1, 4,  28, When.Always, new Categories[]{Data.Categories.Foodstuffs, Data.Categories.MarineMerchandise},        new Materials[]{Material.Clam[2], Material.Laver[2]}),
        new ItemStaticData( 5, "海島木製項鍊",               1, 4,  28, When.Always, new Categories[]{Data.Categories.Accessories, Data.Categories.Woodworks},               new Materials[]{Material.Branch[3], Material.Vine[1]}),
        new ItemStaticData( 6, "海島珊瑚戒指",             1, 6,  42, When.Always, new Categories[]{Data.Categories.Accessories, Data.Categories.MarineMerchandise},       new Materials[]{Material.Coral[3], Material.Vine[3]}),
        new ItemStaticData( 7, "海島開面盔",                 1, 6,  42, When.Always, new Categories[]{Data.Categories.Attire, Data.Categories.Metalworks},                   new Materials[]{Material.CopperOre[3], Material.Sand[3]}),
        new ItemStaticData( 8, "海島石刃棒",             1, 6,  42, When.Always, new Categories[]{Data.Categories.Arms, Data.Categories.Woodworks},                      new Materials[]{Material.PalmLog[3], Material.Stone[3]}),
        new ItemStaticData( 9, "開拓工坊酸泡菜",             1, 4,  40, When.Always, new Categories[]{Data.Categories.PreservedFood},                                        new Materials[]{Material.Cabbage[1], Material.RockSalt[3]}),
        new ItemStaticData(10, "開拓工坊烤南瓜",          1, 4,  40, When.Always, new Categories[]{Data.Categories.Foodstuffs},                                           new Materials[]{Material.Pumpkin[1], Material.Sap[3]}),
        new ItemStaticData(11, "海島束腰衣",                  1, 6,  72, When.Either, new Categories[]{Data.Categories.Attire, Data.Categories.Textiles},                     new Materials[]{Material.Fleece[2], Material.Vine[4]}),
        new ItemStaticData(12, "海島廚刀",         1, 4,  44, When.Always, new Categories[]{Data.Categories.Sundries, Data.Categories.CreatureCreations},          new Materials[]{Material.Claw[1], Material.PalmLog[3]}),
        new ItemStaticData(13, "海島木刷",                  1, 4,  44, When.Always, new Categories[]{Data.Categories.Sundries, Data.Categories.Woodworks},                  new Materials[]{Material.Fur[1], Material.PalmLog[3]}),
        new ItemStaticData(14, "開拓工坊水煮蛋",             1, 4,  44, When.Always, new Categories[]{Data.Categories.Foodstuffs, Data.Categories.CreatureCreations},        new Materials[]{Material.Egg[1], Material.Laver[3]}),
        new ItemStaticData(15, "海島骨拳",                   1, 6,  72, When.Either, new Categories[]{Data.Categories.Arms, Data.Categories.CreatureCreations},              new Materials[]{Material.Carapace[2], Material.Stone[4]}),
        new ItemStaticData(16, "海島獸牙耳墜",               1, 4,  44, When.Either, new Categories[]{Data.Categories.Accessories, Data.Categories.CreatureCreations},       new Materials[]{Material.Fang[1], Material.Vine[3]}),
        new ItemStaticData(17, "開拓工坊奶油",                 1, 4,  44, When.Either, new Categories[]{Data.Categories.Ingredients, Data.Categories.CreatureCreations},       new Materials[]{Material.Milk[1], Material.RockSalt[3]}),
        new ItemStaticData(18, "海島磚塊櫃台",          5, 6,  48, When.Always, new Categories[]{Data.Categories.Furnishings, Data.Categories.UnburiedTreasures},       new Materials[]{Material.Clay[2], Material.Limestone[2], Material.PalmLog[2]}),
        new ItemStaticData(19, "綿羊銅像",           5, 8,  64, When.Either, new Categories[]{Data.Categories.Furnishings, Data.Categories.Metalworks},              new Materials[]{Material.Tinsand[3], Material.CopperOre[3], Material.Log[2]}),
        new ItemStaticData(20, "開拓工坊植物成長劑",         5, 8, 136, When.Either, new Categories[]{Data.Categories.Concoctions},                                          new Materials[]{Material.Alyssum[2], Material.Islewort[3], Material.Branch[3]}),
        new ItemStaticData(21, "海島石榴石刺劍",          5, 8, 136, When.Either, new Categories[]{Data.Categories.Arms, Data.Categories.UnburiedTreasures},              new Materials[]{Material.RawGarnet[2], Material.CopperOre[3], Material.Tinsand[3]}),
        new ItemStaticData(22, "海島雲杉圓盾",    5, 8, 136, When.Either, new Categories[]{Data.Categories.Attire, Data.Categories.Woodworks},                    new Materials[]{Material.SpruceLog[2], Material.CopperOre[3], Material.Stone[3]}),
        new ItemStaticData(23, "開拓工坊鯊油",              5, 8, 136, When.Either, new Categories[]{Data.Categories.Sundries, Data.Categories.MarineMerchandise},          new Materials[]{Material.Hammerhead[2], Material.Laver[3], Material.Sap[3]}),
        new ItemStaticData(24, "海島白銀耳夾",       5, 8, 136, When.Either, new Categories[]{Data.Categories.Accessories, Data.Categories.Metalworks},              new Materials[]{Material.SilverOre[2], Material.Tinsand[3], Material.Coral[3]}),
        new ItemStaticData(25, "開拓工坊甜新薯",           5, 6,  72, When.Either, new Categories[]{Data.Categories.Confections},                                          new Materials[]{Material.Popoto[1], Material.Milk[1], Material.Sap[3]}),
        new ItemStaticData(26, "開拓工坊高山蘿蔔沙拉",          5, 4,  48, When.Always, new Categories[]{Data.Categories.Foodstuffs},                                           new Materials[]{Material.Parsnip[2], Material.Islewort[1], Material.Sap[1]}),
        new ItemStaticData(27, "開拓工坊焦糖",               6, 6,  81, When.Either, new Categories[]{Data.Categories.Confections},                                          new Materials[]{Material.Sugarcane[4], Material.Milk[2]}),
        new ItemStaticData(28, "海島緞帶",                 6, 6,  54, When.Either, new Categories[]{Data.Categories.Accessories, Data.Categories.Textiles},                new Materials[]{Material.CottonBoll[2], Material.CopperOre[2], Material.Vine[2]}),
        new ItemStaticData(29, "海島繩索",                   6, 4,  36, When.Either, new Categories[]{Data.Categories.Sundries, Data.Categories.Textiles},                   new Materials[]{Material.Hemp[2], Material.Islewort[1], Material.Vine[1]}),
        new ItemStaticData(30, "海島騎士帽",         6, 6,  81, When.Either, new Categories[]{Data.Categories.Attire, Data.Categories.Textiles},                     new Materials[]{Material.Feather[2], Material.CottonBoll[2], Material.Hemp[2]}),
        new ItemStaticData(31, "海島角笛",                   6, 6,  81, When.Either, new Categories[]{Data.Categories.Sundries, Data.Categories.CreatureCreations},          new Materials[]{Material.Horn[2], Material.Clay[2], Material.Hemp[2]}),
        new ItemStaticData(32, "海島鹽漬鱈魚",               7, 6,  54, When.Either, new Categories[]{Data.Categories.PreservedFood, Data.Categories.MarineMerchandise},     new Materials[]{Material.Islefish[4], Material.RockSalt[2]}),
        new ItemStaticData(33, "海島墨魚汁",              7, 4,  36, When.Always, new Categories[]{Data.Categories.Ingredients, Data.Categories.MarineMerchandise},       new Materials[]{Material.Squid[2], Material.RockSalt[2] }),
        new ItemStaticData(34, "開拓工坊仙藥",      7, 6,  54, When.Either, new Categories[]{Data.Categories.Concoctions, Data.Categories.MarineMerchandise},       new Materials[]{Material.Jellyfish[2], Material.PalmLeaf[2], Material.Laver[2]}),
        new ItemStaticData(35, "海島莓果醬",          7, 6,  78, When.Either, new Categories[]{Data.Categories.Ingredients},                                          new Materials[]{Material.Isleberry[3], Material.Sugarcane[2], Material.Sap[1]}),
        new ItemStaticData(36, "海島番茄醬",          7, 4,  52, When.Either, new Categories[]{Data.Categories.Ingredients},                                          new Materials[]{Material.Tomato[2], Material.Islewort[1], Material.Sap[1]}),
        new ItemStaticData(37, "開拓工坊洋蔥湯",             7, 6,  78, When.Either, new Categories[]{Data.Categories.Foodstuffs},                                           new Materials[]{Material.Onion[3], Material.RockSalt[2], Material.Islewort[1]}),
        new ItemStaticData(38, "開拓工坊魚派",           7, 6,  78, When.Either, new Categories[]{Data.Categories.Confections, Data.Categories.MarineMerchandise},       new Materials[]{Material.Wheat[3], Material.Islefish[2], Material.Sugarcane[1]}),
        new ItemStaticData(39, "開拓工坊玉米片",            7, 4,  52, When.Either, new Categories[]{Data.Categories.PreservedFood},                                        new Materials[]{Material.Corn[2], Material.Sugarcane[2]}),
        new ItemStaticData(40, "開拓工坊醃小蘿蔔",         7, 8, 104, When.Either, new Categories[]{Data.Categories.PreservedFood},                                        new Materials[]{Material.Radish[4], Material.Apple[2], Material.Sugarcane[2]}),
        new ItemStaticData(41, "海島鐵斧",               8, 8,  72, When.Either, new Categories[]{Data.Categories.Arms, Data.Categories.Metalworks},                     new Materials[]{Material.IronOre[3], Material.Log[3], Material.Sand[2]}),
        new ItemStaticData(42, "海島石英戒指",            8, 8,  72, When.Either, new Categories[]{Data.Categories.Accessories, Data.Categories.UnburiedTreasures},       new Materials[]{Material.Quartz[3], Material.IronOre[3], Material.Stone[2]}),
        new ItemStaticData(43, "開拓工坊瓷罐",         8, 8,  72, When.Either, new Categories[]{Data.Categories.Sundries, Data.Categories.UnburiedTreasures},          new Materials[]{Material.Leucogranite[3], Material.Quartz[3], Material.Clay[2]}),
        new ItemStaticData(44, "開拓工坊果蔬汁",        8, 6,  78, When.Either, new Categories[]{Data.Categories.Concoctions},                                          new Materials[]{Material.Cabbage[3], Material.Islewort[2], Material.Laver[1]}),
        new ItemStaticData(45, "開拓工坊南瓜布丁",        8, 6 , 78, When.Either, new Categories[]{Data.Categories.Confections},                                          new Materials[]{Material.Pumpkin[3], Material.Egg[1], Material.Milk[1]}),
        new ItemStaticData(46, "海島綿羊地毯",         8, 6,  90, When.Either, new Categories[]{Data.Categories.Furnishings, Data.Categories.CreatureCreations},       new Materials[]{Material.Fleece[3], Material.CottonBoll[2], Material.Hemp[1]}),
        new ItemStaticData(47, "海島園藝鐮",          9, 6,  90, When.Either, new Categories[]{Data.Categories.Sundries, Data.Categories.Metalworks},                 new Materials[]{Material.Claw[3], Material.IronOre[2], Material.PalmLog[1]}),
        new ItemStaticData(48, "海島床",                    9, 8, 120, When.Either, new Categories[]{Data.Categories.Furnishings, Data.Categories.Textiles},                new Materials[]{Material.Fur[4], Material.CottonBoll[2], Material.Log[2]}),
        new ItemStaticData(49, "海島鱗指套",          9, 8, 120, When.Either, new Categories[]{Data.Categories.Attire, Data.Categories.CreatureCreations},            new Materials[]{Material.Carapace[4], Material.IronOre[2], Material.CottonBoll[2]}),
        new ItemStaticData(50, "海島手杖",                  9, 8, 120, When.Either, new Categories[]{Data.Categories.Arms, Data.Categories.Woodworks},                      new Materials[]{Material.Fang[4], Material.Quartz[2], Material.Log[2]}),
        new ItemStaticData(51, "開拓工坊珊瑚劍",           10, 8,  72, When.Either, new Categories[]{Data.Categories.Arms, Data.Categories.MarineMerchandise},              new Materials[]{Material.Coral[3], Material.Resin[3], Material.PalmLog[2]}),
        new ItemStaticData(52, "開拓工坊椰汁",         10, 4,  36, When.Always, new Categories[]{Data.Categories.Confections, Data.Categories.Concoctions},             new Materials[]{Material.Coconut[2], Material.Sugarcane[2]}),
        new ItemStaticData(53, "開拓工坊蜂蜜",                 10, 4,  36, When.Always, new Categories[]{Data.Categories.Confections, Data.Categories.Ingredients},             new Materials[]{Material.BeehiveChip[2], Material.Sap[2]}),
        new ItemStaticData(54, "海島藍蛋白石",         10, 8,  80, When.Either, new Categories[]{Data.Categories.UnburiedTreasures},                                    new Materials[]{Material.WoodOpal[4], Material.Sand[4]}),
        new ItemStaticData(55, "開拓工坊乾燥花",          10, 6,  54, When.Either, new Categories[]{Data.Categories.Sundries, Data.Categories.Furnishings},                new Materials[]{Material.MulticoloredIslebloooms[1], Material.Coconut[2], Material.Sap[1]}),
        new ItemStaticData(56, "開拓工坊辣彩粉",     11, 4,  52, When.Either, new Categories[]{Data.Categories.Ingredients, Data.Categories.Concoctions},             new Materials[]{Material.Paprika[2], Material.Islewort[2]}),
        new ItemStaticData(57, "開拓工坊韭蔥洋蔥湯",           11, 6,  90, When.Either, new Categories[]{Data.Categories.Concoctions, Data.Categories.CreatureCreations},       new Materials[]{Material.Leek[3], Material.Milk[1], Material.Laver[3]}),
        new ItemStaticData(58, "開拓工坊無人麵包",                11, 4,  52, When.Either, new Categories[]{Data.Categories.Foodstuffs, Data.Categories.Concoctions},              new Materials[]{Material.Wheat[2], Material.Islefish[1], Material.RockSalt[1]}),
        new ItemStaticData(59, "開拓工坊新薯沙拉",          11, 4,  52, When.Either, new Categories[]{Data.Categories.Foodstuffs},                                           new Materials[]{Material.Popoto[2], Material.Apple[1], Material.Islewort[1]}),
        new ItemStaticData(60, "開拓工坊調味汁",              11, 4,  52, When.Either, new Categories[]{Data.Categories.Ingredients},                                          new Materials[]{Material.Onion[2], Material.Sap[1], Material.Laver[1]}),
        new ItemStaticData(61, "海島暖爐",                 12, 6,  54, When.Either, new Categories[]{Data.Categories.Furnishings, Data.Categories.Metalworks},              new Materials[]{Material.Coal[2], Material.IronOre[3], Material.Leucogranite[1]}),
        new ItemStaticData(62, "海島提燈",               12, 8,  80, When.Either, new Categories[]{Data.Categories.Sundries},                                             new Materials[]{Material.Glimshroom[3], Material.Quartz[3], Material.CopperOre[2]}),
        new ItemStaticData(63, "開拓工坊泡鹼",                12, 4,  36, When.Always, new Categories[]{Data.Categories.Sundries, Data.Categories.Concoctions},                new Materials[]{Material.EffervescentWater[2], Material.RockSalt[2]}),
        new ItemStaticData(64, "開拓工坊五海雜燴湯",         12, 8, 136, When.Either, new Categories[]{Data.Categories.Foodstuffs, Data.Categories.MarineMerchandise},        new Materials[]{Material.CaveShrimp[2], Material.Clam[2], Material.Squid[2], Material.Tomato[2] }),
        new ItemStaticData(65, "開拓工坊化石標本",        13, 6,  54, When.Either, new Categories[]{Data.Categories.CreatureCreations, Data.Categories.UnburiedTreasures}, new Materials[]{Material.Shale[3], Material.PalmLog[2], Material.CottonBoll[1]}),
        new ItemStaticData(66, "海島浴缸",               13, 8,  72, When.Either, new Categories[]{Data.Categories.Furnishings, Data.Categories.UnburiedTreasures},       new Materials[]{Material.Marble[4], Material.Leucogranite[2], Material.Clay[2]}),
        new ItemStaticData(67, "海島眼鏡",            13, 6,  54, When.Either, new Categories[]{Data.Categories.Attire, Data.Categories.Sundries},                     new Materials[]{Material.MythrilOre[3], Material.Quartz[2], Material.CopperOre[1]}),
        new ItemStaticData(68, "開拓工坊隔熱玻璃",         13, 8,  80, When.Either, new Categories[]{Data.Categories.UnburiedTreasures},                                    new Materials[]{Material.Spectrine[4], Material.Sand[4]}),
        new ItemStaticData(69, "開拓工坊煎紅花菜豆",     14, 4,  52, When.Either, new Categories[]{Data.Categories.Foodstuffs},                                           new Materials[]{Material.RunnerBeans[2], Material.RockSalt[2]}),
        new ItemStaticData(70, "開拓工坊紅菜湯",             14, 6,  78, When.Either, new Categories[]{Data.Categories.Foodstuffs},                                           new Materials[]{Material.Beet[3], Material.Popoto[1], Material.Milk[1]}),
        new ItemStaticData(71, "海島僧厥鑲茄子",          14, 6,  90, When.Either, new Categories[]{Data.Categories.Foodstuffs},                                           new Materials[]{Material.Eggplant[2], Material.Onion[2], Material.Tomato[2]}),
        new ItemStaticData(72, "開拓工坊醃櫛瓜",      14, 8, 104, When.Either, new Categories[]{Data.Categories.PreservedFood},                                        new Materials[]{Material.Zucchini[4], Material.Laver[2], Material.Sugarcane[2]}),
        new ItemStaticData(73, "海島黃銅餐盤",    16, 4,  36, When.Either, new Categories[]{Data.Categories.Sundries, Data.Categories.Metalworks},                 new Materials[]{Material.YellowCopperOre[2], Material.CopperOre[2]}),
        new ItemStaticData(74, "海島砂輪機",        16, 6,  60, When.Either, new Categories[]{Data.Categories.Sundries},                                             new Materials[]{Material.HawksEyeSand[2], Material.MythrilOre[2], Material.Sand[2]}),
        new ItemStaticData(75, "海島翠銀環刃",       17, 6,  54, When.Either, new Categories[]{Data.Categories.Arms, Data.Categories.Metalworks},                     new Materials[]{Material.DuriumSand[2], Material.IronOre[2], Material.Quartz[2]}),
        new ItemStaticData(76, "開拓工坊金髮飾",          17, 8,  72, When.Either, new Categories[]{Data.Categories.Accessories, Data.Categories.Metalworks},              new Materials[]{Material.GoldOre[4], Material.YellowCopperOre[2], Material.Quartz[2]}),
        new ItemStaticData(77, "開拓者水晶像", 17, 8,  80, When.Either, new Categories[]{Data.Categories.Furnishings},                                          new Materials[]{Material.CrystalFormation[2], Material.Spectrine[2], Material.MythrilOre[2], Material.Marble[2]}),
        new ItemStaticData(78, "海島水果撈",           18, 4,  52, When.Either, new Categories[]{Data.Categories.Confections},                                          new Materials[]{Material.Watermelon[1], Material.Isleberry[1], Material.Apple[1], Material.Coconut[1]}),
        new ItemStaticData(79, "海島甜新薯塔",      18, 8, 120, When.Either, new Categories[]{Data.Categories.Foodstuffs, Data.Categories.Confections},              new Materials[]{Material.SweetPopoto[3], Material.Wheat[1], Material.Egg[1], Material.Sugarcane[3]}),
        new ItemStaticData(80, "海島風味拌細麵",           18, 6,  75, When.Either, new Categories[]{Data.Categories.Foodstuffs},                                           new Materials[]{Material.Broccoli[2], Material.Wheat[1], Material.RockSalt[3]}),
        new ItemStaticData(81, "海島水牛豆沙拉",    18, 4,  52, When.Either, new Categories[]{Data.Categories.Foodstuffs, Data.Categories.CreatureCreations},        new Materials[]{Material.BuffaloBeans[2], Material.Milk[2]}),
    };
}
