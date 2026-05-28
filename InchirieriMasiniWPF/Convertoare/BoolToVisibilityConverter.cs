using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InchirieriMasiniWPF.Convertoare;

// Converteste un bool in Visibility. Daca parameter = "invert", logica se inverseaza.
// Folosit pentru empty states (afiseaza ilustratia cand colectia e goala).
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool valoare = value is bool b && b;
        bool inverteaza = parameter is string s && s.Equals("invert", StringComparison.OrdinalIgnoreCase);
        if (inverteaza)
        {
            valoare = !valoare;
        }

        return valoare ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;
}
