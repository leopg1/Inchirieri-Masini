using System.Collections.ObjectModel;
using System.Globalization;
using LibrarieModele;

namespace NivelStocareDate;

// LAB 3 - clasa de stocare in memorie, extinsa cu CRUD complet pentru ambele entitati (LAB 6+)
public class AdministrareMasiniMemorie
{
    // LAB 10 - ObservableCollection pentru a notifica automat UI-ul (DataGrid, ListBox)
    // la operatiile Add / Remove. Fara aceasta, controlul WPF nu vede modificarile.
    private readonly ObservableCollection<Masina> masini = new();
    private readonly ObservableCollection<Rezervare> rezervari = new();

    public ObservableCollection<Masina> Masini => masini;
    public ObservableCollection<Rezervare> Rezervari => rezervari;

    // -------- CRUD MASINA --------

    public void AdaugaMasina(Masina masina)
    {
        if (masina.IdMasina <= 0)
        {
            masina.IdMasina = GenereazaIdMasina();
        }

        masina.DataAdaugare = DateTime.Now;
        masina.DataActualizare = DateTime.Now;
        masini.Add(masina);
    }

    public bool ModificaMasina(Masina masinaModificata)
    {
        Masina? existenta = GetMasina(masinaModificata.IdMasina);
        if (existenta is null)
        {
            return false;
        }

        existenta.Marca = masinaModificata.Marca;
        existenta.Model = masinaModificata.Model;
        existenta.PretPeZi = masinaModificata.PretPeZi;
        existenta.Disponibila = masinaModificata.Disponibila;
        existenta.Culoare = masinaModificata.Culoare;
        existenta.Combustibil = masinaModificata.Combustibil;
        existenta.Optiuni = masinaModificata.Optiuni;
        existenta.DataActualizare = DateTime.Now;
        return true;
    }

    public bool StergeMasina(int idMasina)
    {
        Masina? masina = GetMasina(idMasina);
        if (masina is null)
        {
            return false;
        }

        // sterg si rezervarile asociate ca sa nu raman rezervari orfane
        var rezervariAsociate = rezervari.Where(r => r.IdMasina == idMasina).ToList();
        foreach (var rezervare in rezervariAsociate)
        {
            rezervari.Remove(rezervare);
        }

        return masini.Remove(masina);
    }

    public Masina? GetMasina(int idMasina)
    {
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
        return masini
            .Where(m => m.Marca.Equals(marca, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    // LAB 11 - cautare folosita de WPF (filtrare partiala in timp real)
    public List<Masina> CautaMasini(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return masini.ToList();
        }

        string normalizat = text.Trim();
        return masini
            .Where(m =>
                m.Marca.Contains(normalizat, StringComparison.OrdinalIgnoreCase) ||
                m.Model.Contains(normalizat, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<Masina> GetMasini() => masini.ToList();

    public List<Masina> GetMasiniDisponibile() => masini.Where(m => m.Disponibila).ToList();

    public bool ModificaPretMasina(int idMasina, decimal pretNou)
    {
        Masina? masina = GetMasina(idMasina);
        if (masina is null)
        {
            return false;
        }

        masina.PretPeZi = pretNou;
        masina.DataActualizare = DateTime.Now;
        return true;
    }

    // -------- CRUD REZERVARE --------

    public bool AdaugaRezervare(Rezervare rezervare)
    {
        Masina? masina = GetMasina(rezervare.IdMasina);
        if (masina is null || !masina.Disponibila)
        {
            return false;
        }

        if (rezervare.IdRezervare <= 0)
        {
            rezervare.IdRezervare = GenereazaIdRezervare();
        }

        rezervare.PretPeZi = masina.PretPeZi;
        rezervare.DataActualizare = DateTime.Now;
        rezervari.Add(rezervare);
        masina.Disponibila = false;
        masina.DataActualizare = DateTime.Now;
        return true;
    }

    public bool ModificaRezervare(Rezervare rezervareModificata)
    {
        Rezervare? existenta = rezervari.FirstOrDefault(r => r.IdRezervare == rezervareModificata.IdRezervare);
        if (existenta is null)
        {
            return false;
        }

        existenta.NumeClient = rezervareModificata.NumeClient;
        existenta.NumarTelefon = rezervareModificata.NumarTelefon;
        existenta.Perioada = rezervareModificata.Perioada;
        existenta.DataActualizare = DateTime.Now;
        return true;
    }

    public bool StergeRezervare(int idRezervare)
    {
        Rezervare? rezervare = rezervari.FirstOrDefault(r => r.IdRezervare == idRezervare);
        if (rezervare is null)
        {
            return false;
        }

        // la anulare, masina redevine disponibila
        Masina? masina = GetMasina(rezervare.IdMasina);
        if (masina is not null)
        {
            masina.Disponibila = true;
            masina.DataActualizare = DateTime.Now;
        }

        return rezervari.Remove(rezervare);
    }

    public List<Rezervare> GetRezervari() => rezervari.ToList();

    // -------- ID generator --------

    private int GenereazaIdMasina() => masini.Count == 0 ? 1 : masini.Max(m => m.IdMasina) + 1;

    private int GenereazaIdRezervare() => rezervari.Count == 0 ? 1 : rezervari.Max(r => r.IdRezervare) + 1;

    // -------- Incarcare CSV (LAB 3) --------

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
                Optiuni = ParseOptiuni(campuri[7]),
                DataAdaugare = DateTime.Now,
                DataActualizare = DateTime.Now
            };

            masini.Add(masina);
            adaugate++;
        }

        return adaugate;
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
