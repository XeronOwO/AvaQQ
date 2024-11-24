using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using AvaQQ.SDK;
using System;
using System.Globalization;

namespace AvaQQ.Converters;

public class ColorAlphaChannelConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		var app = AppBase.Current;

		if (value is DynamicResourceExtension resource
			&& resource.ResourceKey is { } key
			&& app.TryFindResource(key, app.ActualThemeVariant, out var @object)
			&& @object is IImmutableSolidColorBrush brush
			&& parameter is string strParameter
			&& byte.TryParse(strParameter, out var alpha))
		{
			var color = brush.Color;
			return new SolidColorBrush(new Color(alpha, color.R, color.G, color.B));
		}

		return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
