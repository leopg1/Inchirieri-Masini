using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LibrarieModele;

// LAB 2 - clasa Rezervare, extinsa pentru WPF (LAB 6+):
// - INotifyPropertyChanged pentru data binding
// - IDataErrorInfo pentru validarea numelui clientului si a numarului de telefon
// - NumarTelefon + DataActualizare (LAB 9)
public class Rezervare : INotifyPropertyChanged, IDataErrorInfo
{
    private int idRezervare;
    private int idMasina;
    private string numeClient = string.Empty;
    private string numarTelefon = string.Empty;
    private PerioadaInchiriere perioada;
    private decimal pretPeZi;
    private DateTime dataActualizare = DateTime.Now;

    public int IdRezervare
    {
        get => idRezervare;
        set { idRezervare = value; OnPropertyChanged(); }
    }

    public int IdMasina
    {
        get => idMasina;
        set { idMasina = value; OnPropertyChanged(); }
    }

    public string NumeClient
    {
        get => numeClient;
        set { numeClient = value; OnPropertyChanged(); OnPropertyChanged(nameof(EsteValid)); }
    }

    public string NumarTelefon
    {
        get => numarTelefon;
        set { numarTelefon = value; OnPropertyChanged(); OnPropertyChanged(nameof(EsteValid)); }
    }

    public PerioadaInchiriere Perioada
    {
        get => perioada;
        set { perioada = value; OnPropertyChanged(); OnPropertyChanged(nameof(CostTotal)); }
    }

    public decimal PretPeZi
    {
        get => pretPeZi;
        set { pretPeZi = value; OnPropertyChanged(); OnPropertyChanged(nameof(CostTotal)); }
    }

    public DateTime DataActualizare
    {
        get => dataActualizare;
        set { dataActualizare = value; OnPropertyChanged(); }
    }

    public decimal CostTotal => PretPeZi * Perioada.NumarZile;

    public string Info()
    {
        return $"Rezervare:{IdRezervare} MasinaId:{IdMasina} Client:{NumeClient} Telefon:{NumarTelefon} " +
               $"Perioada:{Perioada} CostTotal:{CostTotal:F2}";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public string Error => string.Empty;

    public string this[string columnName]
    {
        get
        {
            switch (columnName)
            {
                case nameof(NumeClient):
                    if (string.IsNullOrWhiteSpace(NumeClient))
                        return "Numele clientului este obligatoriu.";
                    if (NumeClient.Length > ConstanteAplicatie.LungimeMaximaNumeClient)
                        return $"Numele clientului nu poate depasi {ConstanteAplicatie.LungimeMaximaNumeClient} caractere.";
                    break;

                case nameof(NumarTelefon):
                    if (string.IsNullOrWhiteSpace(NumarTelefon))
                        return "Numarul de telefon este obligatoriu.";
                    if (NumarTelefon.Length != ConstanteAplicatie.LungimeTelefon)
                        return $"Telefonul trebuie sa aiba {ConstanteAplicatie.LungimeTelefon} cifre.";
                    if (!NumarTelefon.All(char.IsDigit))
                        return "Telefonul trebuie sa contina doar cifre.";
                    break;
            }

            return string.Empty;
        }
    }

    public bool EsteValid =>
        string.IsNullOrEmpty(this[nameof(NumeClient)]) &&
        string.IsNullOrEmpty(this[nameof(NumarTelefon)]);
}
