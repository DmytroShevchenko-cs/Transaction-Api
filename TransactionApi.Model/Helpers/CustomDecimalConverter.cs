using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace TransactionApi.Model.Helpers;

public class CustomDecimalConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        // Remove the dollar sign and any other currency symbol
        text = text.Replace("$", "").Trim();

        if (decimal.TryParse(text, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.InvariantCulture, out var decimalValue))
        {
            return decimalValue;
        }

        throw new TypeConverterException(this, memberMapData, text, row.Context, $"The conversion cannot be performed for value '{text}'.");
    }
}