using System.Globalization;
using LibrarieModele;

namespace NivelStocareDate;

// LAB 3 - Implementarea 2: clasa separata pentru stocare date in memorie
public class AdministrareMasiniMemorie
{
    // LAB 3 - Implementarea 3: colectii generice List<T>
    private readonly List<Masina> masini = [];
    private readonly List<Rezervare> rezervari = [];

    public void AdaugaMasina(Masina masina)
    {
        masini.Add(masina);
    }

    // LAB 3 - Implementarea 4: preluare date initiale din fisier CSV local
    public int IncarcaMasiniDinCsv(string caleFisier)
    {
        if (!File.Exists(caleFisier))
        {
            return 0;
        }

        string[] linii = File.ReadAllLines(caleFisier);
        if (linii.Length <= 1)
        {
            return 0;
        }

        int adaugate = 0;

        foreach (string linie in linii.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(linie))
            {
                continue;
            }

            string[] campuri = linie.Split(';');
            if (campuri.Length < 8)
            {
                continue;
            }

            if (!int.TryParse(campuri[0], out int idMasina))
            {
                continue;
            }

            if (masini.Any(m => m.IdMasina == idMasina))
            {
                continue;
            }

            if (!decimal.TryParse(
                    campuri[3],
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal pretPeZi))
            {
                continue;
            }

            if (!bool.TryParse(campuri[4], out bool disponibila))
            {
                disponibila = true;
            }

            Masina masina = new()
            {
                IdMasina = idMasina,
                Marca = campuri[1].Trim(),
                Model = campuri[2].Trim(),
                PretPeZi = pretPeZi,
                Disponibila = disponibila,
                Culoare = ParseEnum(campuri[5], CuloareMasina.Gri),
                Combustibil = ParseEnum(campuri[6], TipCombustibil.Benzina),
                Optiuni = ParseOptiuni(campuri[7])
            };

            masini.Add(masina);
            adaugate++;
        }

        return adaugate;
    }

    public List<Masina> GetMasini()
    {
        return masini.ToList();
    }

    public Masina? GetMasina(int idMasina)
    {
        // LAB 4 - Implementarea 12: LINQ FirstOrDefault
        return masini.FirstOrDefault(m => m.IdMasina == idMasina);
    }

    public Masina? CautaMasina(string marca, string model)
    {
        return masini.FirstOrDefault(m =>
            m.Marca.Equals(marca, StringComparison.OrdinalIgnoreCase) &&
            m.Model.Equals(model, StringComparison.OrdinalIgnoreCase));
    }

    public List<Masina> CautaMasiniDupaMarca(string marca)
    {
        // LAB 4 - Implementarea 13: LINQ Where + ToList
        return masini
            .Where(m => m.Marca.Equals(marca, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public bool ModificaPretMasina(int idMasina, decimal pretNou)
    {
        Masina? masina = GetMasina(idMasina);
        if (masina is null)
        {
            return false;
        }

        masina.PretPeZi = pretNou;
        return true;
    }

    public List<Masina> GetMasiniDisponibile()
    {
        return masini.Where(m => m.Disponibila).ToList();
    }

    public bool AdaugaRezervare(Rezervare rezervare)
    {
        Masina? masina = GetMasina(rezervare.IdMasina);
        if (masina is null || !masina.Disponibila)
        {
            return false;
        }

        rezervari.Add(rezervare);
        masina.Disponibila = false;
        return true;
    }

    public List<Rezervare> GetRezervari()
    {
        return rezervari.ToList();
    }

    private static TEnum ParseEnum<TEnum>(string text, TEnum valoareImplicita) where TEnum : struct, Enum
    {
        if (Enum.TryParse<TEnum>(text.Trim(), true, out TEnum rezultat) &&
            Enum.IsDefined(typeof(TEnum), rezultat))
        {
            return rezultat;
        }

        return valoareImplicita;
    }

    private static OptiuniMasina ParseOptiuni(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return OptiuniMasina.Nimic;
        }

        OptiuniMasina rezultat = OptiuniMasina.Nimic;
        string[] valori = text.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (string valoare in valori)
        {
            if (Enum.TryParse<OptiuniMasina>(valoare, true, out OptiuniMasina optiune))
            {
                rezultat |= optiune;
            }
        }

        return rezultat;
    }
}
