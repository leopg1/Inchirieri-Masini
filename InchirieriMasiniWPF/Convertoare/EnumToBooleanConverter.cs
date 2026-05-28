using System.Globalization;
using System.Windows.Data;

namespace InchirieriMasiniWPF.Convertoare;

// LAB 11 - converter pentru a lega un grup de RadioButton la o proprietate de tip enum.
// La afisare: enum -> bool (true daca enum-ul corespunde parametrului).
// La click:   bool -> enum (intoarce parametrul daca s-a bifat, altfel Binding.DoNothing).
public class EnumToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null && parameter != null && value.Equals(parameter);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool bifat = (value as bool?) ?? false;
        if (bifat && parameter is not null)
        {
            return parameter;
        }

        return Binding.DoNothing;
    }
}
