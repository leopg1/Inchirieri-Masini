using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LibrarieModele;

// LAB 2 - clasa Masina, extinsa pentru WPF (LAB 6+):
// - INotifyPropertyChanged pentru data binding TwoWay
// - IDataErrorInfo pentru validare automata in WPF
// - DataAdaugare / DataActualizare pentru a marca momentul modificarii (LAB 9)
public class Masina : INotifyPropertyChanged, IDataErrorInfo
{
    private const string ValoareNecunoscuta = "NECUNOSCUT";

    private int idMasina;
    private string marca = string.Empty;
    private string model = string.Empty;
    private decimal pretPeZi;
    private bool disponibila = true;
    private CuloareMasina culoare = CuloareMasina.Gri;
    private TipCombustibil combustibil = TipCombustibil.Benzina;
    private OptiuniMasina optiuni = OptiuniMasina.Nimic;
    private DateTime dataAdaugare = DateTime.Now;
    private DateTime dataActualizare = DateTime.Now;

    public int IdMasina
    {
        get => idMasina;
        set { idMasina = value; OnPropertyChanged(); }
    }

    public string Marca
    {
        get => marca;
        set { marca = value; OnPropertyChanged(); OnPropertyChanged(nameof(DenumireCompleta)); OnPropertyChanged(nameof(EsteValid)); }
    }

    public string Model
    {
        get => model;
        set { model = value; OnPropertyChanged(); OnPropertyChanged(nameof(DenumireCompleta)); OnPropertyChanged(nameof(EsteValid)); }
    }

    public decimal PretPeZi
    {
        get => pretPeZi;
        set { pretPeZi = value; OnPropertyChanged(); OnPropertyChanged(nameof(EsteValid)); }
    }

    public bool Disponibila
    {
        get => disponibila;
        set { disponibila = value; OnPropertyChanged(); }
    }

    public CuloareMasina Culoare
    {
        get => culoare;
        set { culoare = value; OnPropertyChanged(); }
    }

    public TipCombustibil Combustibil
    {
        get => combustibil;
        set { combustibil = value; OnPropertyChanged(); }
    }

    public OptiuniMasina Optiuni
    {
        get => optiuni;
        set { optiuni = value; OnPropertyChanged(); }
    }

    public DateTime DataAdaugare
    {
        get => dataAdaugare;
        set { dataAdaugare = value; OnPropertyChanged(); }
    }

    public DateTime DataActualizare
    {
        get => dataActualizare;
        set { dataActualizare = value; OnPropertyChanged(); }
    }

    // LAB 2 - proprietate computed afisata in DataGrid
    public string DenumireCompleta => $"{Marca} {Model}".Trim();

    public string Info()
    {
        // LAB 2 - Join + Split pentru afisare optiuni
        string optiuniFormatate = string.Join(
            ConstanteAplicatie.SeparatorAfisare.ToString(),
            Optiuni.ToString().Split(", ").Select(optiune => optiune.Trim()));

        return $"Id:{IdMasina} Marca:{(string.IsNullOrWhiteSpace(Marca) ? ValoareNecunoscuta : Marca)} " +
               $"Model:{(string.IsNullOrWhiteSpace(Model) ? ValoareNecunoscuta : Model)} " +
               $"Pret/zi:{PretPeZi:F2} Disponibila:{Disponibila} Culoare:{Culoare} " +
               $"Combustibil:{Combustibil} Optiuni:{optiuniFormatate}";
    }

    // LAB 10 - INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // LAB 11 - IDataErrorInfo: validare automata pe proprietati legate prin Binding
    public string Error => string.Empty;

    public string this[string columnName]
    {
        get
        {
            switch (columnName)
            {
                case nameof(Marca):
                    if (string.IsNullOrWhiteSpace(Marca))
                        return "Marca este obligatorie.";
                    if (Marca.Length > ConstanteAplicatie.LungimeMaximaMarca)
                        return $"Marca nu poate depasi {ConstanteAplicatie.LungimeMaximaMarca} caractere.";
                    break;

                case nameof(Model):
                    if (string.IsNullOrWhiteSpace(Model))
                        return "Modelul este obligatoriu.";
                    if (Model.Length > ConstanteAplicatie.LungimeMaximaModel)
                        return $"Modelul nu poate depasi {ConstanteAplicatie.LungimeMaximaModel} caractere.";
                    break;

                case nameof(PretPeZi):
                    if (PretPeZi < ConstanteAplicatie.PretMinimPeZi || PretPeZi > ConstanteAplicatie.PretMaximPeZi)
                        return $"Pretul trebuie sa fie intre {ConstanteAplicatie.PretMinimPeZi} si {ConstanteAplicatie.PretMaximPeZi} lei.";
                    break;
            }

            return string.Empty;
        }
    }

    public bool EsteValid =>
        string.IsNullOrEmpty(this[nameof(Marca)]) &&
        string.IsNullOrEmpty(this[nameof(Model)]) &&
        string.IsNullOrEmpty(this[nameof(PretPeZi)]);
}
