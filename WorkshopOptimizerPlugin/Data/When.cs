namespace WorkshopOptimizerPlugin.Data;


internal enum When
{
    Never = 0,
    Weak = 1,
    Strong = 2,
    Either = 3,
    Always = 4,
    Required = 5,
}

internal static class WhenUtils
{
    public static readonly string[] WhenAsStrings = new string[]
    {
        Zh.When(When.Never), Zh.When(When.Weak), Zh.When(When.Strong),
        Zh.When(When.Either), Zh.When(When.Always), Zh.When(When.Required),
    };
}
