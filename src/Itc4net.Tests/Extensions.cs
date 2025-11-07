namespace Itc4net.Tests;

internal static class Extensions
{
    public static void ShouldBe(this byte actual, int expected)
    {
        Shouldly.ShouldBeTestExtensions.ShouldBe<byte>(actual, (byte)expected);
    }
}
