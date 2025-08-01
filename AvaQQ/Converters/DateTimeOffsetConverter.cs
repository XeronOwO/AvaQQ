using Avalonia.Data.Converters;
using System.Globalization;

namespace AvaQQ.Converters;

public class DateTimeOffsetConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is not DateTimeOffset dateTimeOffset)
			throw new ArgumentException("Value must be of type DateTimeOffset", nameof(value));
		if (targetType != typeof(string))
			throw new ArgumentException("Target type must be string", nameof(targetType));

		return dateTimeOffset.ToString("HH:mm:ss", culture);
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
