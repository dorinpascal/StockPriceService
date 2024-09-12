namespace Stock_Price_Service.Helpers;

public static class GenerateStockPrice
{
    private static readonly Random random = new();

    public static double GenerateRandom()
    {
        return Math.Round(random.NextDouble() * (1500 - 100) + 100, 2);
    }
}
