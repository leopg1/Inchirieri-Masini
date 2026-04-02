namespace LibrarieModele;

// LAB 2 - Implementarea 6: clasa simpla pentru modelarea unei rezervari
public class Rezervare
{
    public int IdRezervare { get; set; }
    public int IdMasina { get; set; }
    public string NumeClient { get; set; } = string.Empty;
    public PerioadaInchiriere Perioada { get; set; }
    public decimal PretPeZi { get; set; }

    public decimal CostTotal => PretPeZi * Perioada.NumarZile;

    public string Info()
    {
        return $"Rezervare:{IdRezervare} MasinaId:{IdMasina} Client:{NumeClient} " +
               $"Perioada:{Perioada} CostTotal:{CostTotal:F2}";
    }
}
