using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using LibrarieModele;

namespace InchirieriMasiniWPF.Convertoare;

// Mapeaza valoarea enum CuloareMasina la un Brush real pentru a colora
// previzualizarea masinii din UI (chip culoare + accent card).
public class CuloareMasinaToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CuloareMasina culoare)
        {
            return new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8));
        }

        return culoare switch
        {
            CuloareMasina.Rosu     => new SolidColorBrush(Color.FromRgb(0xDC, 0x26, 0x26)),
            CuloareMasina.Alb      => new SolidColorBrush(Color.FromRgb(0xF1, 0xF5, 0xF9)),
            CuloareMasina.Negru    => new SolidColorBrush(Color.FromRgb(0x0F, 0x17, 0x2A)),
            CuloareMasina.Gri      => new SolidColorBrush(Color.FromRgb(0x64, 0x74, 0x8B)),
            CuloareMasina.Albastru => new SolidColorBrush(Color.FromRgb(0x25, 0x63, 0xEB)),
            _ => new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8))
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;
}
