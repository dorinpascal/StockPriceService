namespace Stock_Price_Service.Helpers;

public static class DoubleConversionHelper
{
    public static string ConvertDoubleToString(double value)
    {
        return value.ToString();
    }

    public static double? ConvertStringToDouble(string? value)
    {
        if (double.TryParse(value, out double result))
        {
            return result;
        }
        throw new FormatException($"The value '{value}' is not a valid double.");
    }
}
