namespace LibrarieModele;

// LAB 2 - Implementarea 3: model de clasa in fisier separat
public class Masina
{
    private const string ValoareNecunoscuta = "NECUNOSCUT";

    public int IdMasina { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public decimal PretPeZi { get; set; }
    public bool Disponibila { get; set; } = true;
    public CuloareMasina Culoare { get; set; }
    public TipCombustibil Combustibil { get; set; }
    public OptiuniMasina Optiuni { get; set; }

    // LAB 2 - Implementarea 4: proprietate computed
    public string DenumireCompleta => $"{Marca} {Model}".Trim();

    public string Info()
    {
        // LAB 2 - Implementarea 5: Join + Split pentru afisare optiuni
        string optiuniFormatate = string.Join(
            ConstanteAplicatie.SeparatorAfisare.ToString(),
            Optiuni.ToString().Split(", ").Select(optiune => optiune.Trim()));

        return $"Id:{IdMasina} Marca:{(string.IsNullOrWhiteSpace(Marca) ? ValoareNecunoscuta : Marca)} " +
               $"Model:{(string.IsNullOrWhiteSpace(Model) ? ValoareNecunoscuta : Model)} " +
               $"Pret/zi:{PretPeZi:F2} Disponibila:{Disponibila} Culoare:{Culoare} " +
               $"Combustibil:{Combustibil} Optiuni:{optiuniFormatate}";
    }
}
