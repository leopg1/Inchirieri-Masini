using LibrarieModele;
using NivelStocareDate;

namespace InchirieriMasiniConsole;

internal class Program
{
    private const string NumeFisierMasiniCsv = "masini_default.csv";

    // LAB 3 - Implementarea 1: datele sunt administrate prin clasa de stocare in memorie
    private static readonly AdministrareMasiniMemorie admin = new();
    private static int urmatorIdMasina = 1;
    private static int urmatorIdRezervare = 1;

    private static void Main()
    {
        Console.Title = "Aplicatie PIU - Inchirieri Masini";
        IncarcaDateInitialeDinCsv();
        RuleazaMeniu();
    }

    private static void RuleazaMeniu()
    {
        // LAB 1 - Implementarea 1: structura repetitiva pentru meniu
        bool ruleaza = true;
        while (ruleaza)
        {
            AfiseazaMeniu();
            Console.Write("Optiune: ");
            string? optiune = Console.ReadLine()?.Trim().ToUpperInvariant();

            switch (optiune)
            {
                case "C":
                    AdaugaMasina();
                    break;
                case "A":
                    AfiseazaMasini(admin.GetMasini());
                    break;
                case "F":
                    CautaMasinaDupaMarcaModel();
                    break;
                case "N":
                    CautaMasiniDupaMarca();
                    break;
                case "M":
                    ModificaPretMasina();
                    break;
                case "R":
                    AdaugaRezervare();
                    break;
                case "D":
                    AfiseazaMasini(admin.GetMasiniDisponibile());
                    break;
                case "V":
                    DemoConversiiStringInt();
                    break;
                case "X":
                    ruleaza = false;
                    break;
                default:
                    Console.WriteLine("Optiune invalida.");
                    break;
            }

            Console.WriteLine();
        }
    }

    private static void AfiseazaMeniu()
    {
        Console.WriteLine("=== MENIU INCHIRIERI MASINI ===");
        Console.WriteLine("C - Citire masina");
        Console.WriteLine("A - Afisare masini");
        Console.WriteLine("F - Cauta masina dupa marca + model");
        Console.WriteLine("N - Cauta masini dupa marca");
        Console.WriteLine("M - Modifica pret masina");
        Console.WriteLine("R - Rezerva masina");
        Console.WriteLine("D - Afiseaza masini disponibile");
        Console.WriteLine("V - Demo conversii string -> int");
        Console.WriteLine("X - Iesire");
    }

    private static void AdaugaMasina()
    {
        // LAB 2 - Implementarea 1: creare obiect pe baza datelor citite de la tastatura
        Masina masina = new()
        {
            IdMasina = urmatorIdMasina++,
            Marca = CitesteTextObligatoriu("Marca: "),
            Model = CitesteTextObligatoriu("Model: "),
            PretPeZi = CitesteDecimalPozitiv("Pret pe zi: "),
            // LAB 4 - Implementarea 1: enum-uri cu validare la input
            Culoare = CitesteEnumCuValidare<CuloareMasina>("Culoare masina"),
            Combustibil = CitesteEnumCuValidare<TipCombustibil>("Tip combustibil"),
            // LAB 4 - Implementarea 2: enum cu [Flags] pentru optiuni multiple
            Optiuni = CitesteOptiuniMasina()
        };

        admin.AdaugaMasina(masina);
        Console.WriteLine("Masina a fost adaugata.");
    }

    private static void CautaMasinaDupaMarcaModel()
    {
        string marca = CitesteTextObligatoriu("Marca cautata: ");
        string model = CitesteTextObligatoriu("Model cautat: ");
        // LAB 3 + LAB 4 - Implementarea 2: cautare prin metoda separata (LINQ in storage)
        Masina? masina = admin.CautaMasina(marca, model);

        if (masina is null)
        {
            Console.WriteLine("Nu s-a gasit masina.");
            return;
        }

        Console.WriteLine(masina.Info());
    }

    private static void CautaMasiniDupaMarca()
    {
        string marca = CitesteTextObligatoriu("Marca cautata: ");
        // LAB 3 + LAB 4 - Implementarea 3: cautare multipla si afisare lista
        List<Masina> rezultat = admin.CautaMasiniDupaMarca(marca);
        AfiseazaMasini(rezultat);
    }

    private static void ModificaPretMasina()
    {
        int idMasina = CitesteIntPozitiv("Id masina: ");
        decimal pretNou = CitesteDecimalPozitiv("Pret nou pe zi: ");

        bool modificat = admin.ModificaPretMasina(idMasina, pretNou);
        Console.WriteLine(modificat ? "Pretul a fost modificat." : "Masina nu exista.");
    }

    private static void AdaugaRezervare()
    {
        int idMasina = CitesteIntPozitiv("Id masina pentru rezervare: ");
        Masina? masina = admin.GetMasina(idMasina);
        if (masina is null)
        {
            Console.WriteLine("Masina nu exista.");
            return;
        }

        if (!masina.Disponibila)
        {
            Console.WriteLine("Masina este deja rezervata.");
            return;
        }

        string numeClient = CitesteTextObligatoriu("Nume client: ");
        DateOnly dataStart = CitesteData("Data start (yyyy-MM-dd): ");
        DateOnly dataSfarsit = CitesteData("Data sfarsit (yyyy-MM-dd): ");
        if (dataSfarsit < dataStart)
        {
            Console.WriteLine("Data de sfarsit trebuie sa fie dupa data de start.");
            return;
        }

        Rezervare rezervare = new()
        {
            IdRezervare = urmatorIdRezervare++,
            IdMasina = idMasina,
            NumeClient = numeClient,
            PretPeZi = masina.PretPeZi,
            // LAB 4 - Implementarea 4: folosire struct pentru perioada
            Perioada = new PerioadaInchiriere
            {
                DataStart = dataStart,
                DataSfarsit = dataSfarsit
            }
        };

        bool rezultat = admin.AdaugaRezervare(rezervare);
        Console.WriteLine(rezultat ? rezervare.Info() : "Nu s-a putut face rezervarea.");
    }

    private static void AfiseazaMasini(List<Masina> masini)
    {
        if (masini.Count == 0)
        {
            Console.WriteLine("Nu exista masini.");
            return;
        }

        // LAB 1 - Implementarea 7: afisare tabelara in consola
        string header = string.Format(
            "{0,-4} | {1,-12} | {2,-12} | {3,9} | {4,-10} | {5,-10} | {6,-11} | {7,-28}",
            "ID",
            "MARCA",
            "MODEL",
            "PRET/ZI",
            "DISPONIBILA",
            "CULOARE",
            "COMBUSTIBIL",
            "OPTIUNI");
        Console.WriteLine(header);
        Console.WriteLine(new string('-', header.Length));

        foreach (Masina masina in masini)
        {
            Console.WriteLine(string.Format(
                "{0,-4} | {1,-12} | {2,-12} | {3,9:F2} | {4,-10} | {5,-10} | {6,-11} | {7,-28}",
                masina.IdMasina,
                TaieText(masina.Marca, 12),
                TaieText(masina.Model, 12),
                masina.PretPeZi,
                masina.Disponibila ? "Da" : "Nu",
                TaieText(masina.Culoare.ToString(), 10),
                TaieText(masina.Combustibil.ToString(), 11),
                TaieText(masina.Optiuni.ToString(), 28)));
        }
    }

    private static string CitesteTextObligatoriu(string mesaj)
    {
        // LAB 1 - Implementarea 2: validare input text obligatoriu
        while (true)
        {
            Console.Write(mesaj);
            string? text = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(text))
            {
                return text.Trim();
            }

            Console.WriteLine("Valoare invalida.");
        }
    }

    private static int CitesteIntPozitiv(string mesaj)
    {
        // LAB 1 - Implementarea 3: validare cu TryParse
        while (true)
        {
            Console.Write(mesaj);
            string? text = Console.ReadLine();
            if (int.TryParse(text, out int valoare) && valoare > 0)
            {
                return valoare;
            }

            Console.WriteLine("Introdu un numar intreg pozitiv.");
        }
    }

    private static decimal CitesteDecimalPozitiv(string mesaj)
    {
        // LAB 1 - Implementarea 4: validare numerica pentru valori pozitive
        while (true)
        {
            Console.Write(mesaj);
            string? text = Console.ReadLine();
            if (decimal.TryParse(text, out decimal valoare) && valoare > 0)
            {
                return valoare;
            }

            Console.WriteLine("Introdu un numar pozitiv.");
        }
    }

    private static DateOnly CitesteData(string mesaj)
    {
        while (true)
        {
            Console.Write(mesaj);
            string? text = Console.ReadLine();
            if (DateOnly.TryParse(text, out DateOnly data))
            {
                return data;
            }

            Console.WriteLine("Data invalida.");
        }
    }

    private static TEnum CitesteEnumCuValidare<TEnum>(string titlu) where TEnum : struct, Enum
    {
        while (true)
        {
            Console.WriteLine($"Alege {titlu}:");
            foreach (TEnum valoare in Enum.GetValues<TEnum>())
            {
                Console.WriteLine($"{Convert.ToInt32(valoare)} - {valoare}");
            }

            Console.Write("Optiune: ");
            try
            {
                // LAB 4 - Implementarea 5: convert + validare pe enum
                int optiune = Convert.ToInt32(Console.ReadLine());
                if (Enum.IsDefined(typeof(TEnum), optiune))
                {
                    return (TEnum)Enum.ToObject(typeof(TEnum), optiune);
                }
            }
            catch (FormatException)
            {
                // LAB 4 - Implementarea 6: tratarea exceptiei de format
                Console.WriteLine("Eroare: trebuie sa introduci un numar valid.");
            }
            catch (OverflowException)
            {
                // LAB 4 - Implementarea 7: tratarea exceptiei de overflow
                Console.WriteLine("Eroare: numarul introdus este prea mare.");
            }

            Console.WriteLine("Optiune inexistenta.");
        }
    }

    private static OptiuniMasina CitesteOptiuniMasina()
    {
        Console.WriteLine("Alege optiunile masinii separate prin virgula (ex: 1,2,4).");
        foreach (OptiuniMasina optiune in Enum.GetValues<OptiuniMasina>())
        {
            if (optiune == OptiuniMasina.Nimic)
            {
                continue;
            }

            Console.WriteLine($"{(int)optiune} - {optiune}");
        }

        Console.Write("Optiuni: ");
        string? text = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(text))
        {
            return OptiuniMasina.Nimic;
        }

        OptiuniMasina rezultat = OptiuniMasina.Nimic;
        // LAB 2 - Implementarea 2: prelucrare siruri cu Split
        string[] parti = text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (string parte in parti)
        {
            if (int.TryParse(parte, out int valoareOptiune) &&
                Enum.IsDefined(typeof(OptiuniMasina), valoareOptiune))
            {
                rezultat |= (OptiuniMasina)valoareOptiune;
            }
        }

        return rezultat;
    }

    private static void DemoConversiiStringInt()
    {
        // LAB 1 - Implementarea 5: comparatie intre Convert, Parse, TryParse
        Console.Write("Introdu un text numeric: ");
        string text = Console.ReadLine() ?? string.Empty;

        try
        {
            int v1 = Convert.ToInt32(text);
            Console.WriteLine($"Convert.ToInt32 => {v1}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Convert.ToInt32 a esuat: {ex.Message}");
        }

        try
        {
            int v2 = int.Parse(text);
            Console.WriteLine($"int.Parse => {v2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"int.Parse a esuat: {ex.Message}");
        }

        bool ok = int.TryParse(text, out int v3);
        Console.WriteLine(ok
            ? $"int.TryParse => {v3}"
            : "int.TryParse => false (fara exceptie)");

        // LAB 1 - Implementarea 6: valori minime si maxime pentru tipuri uzuale
        Console.WriteLine("Min/Max tipuri numerice:");
        Console.WriteLine($"byte: {byte.MinValue} .. {byte.MaxValue}");
        Console.WriteLine($"short: {short.MinValue} .. {short.MaxValue}");
        Console.WriteLine($"int: {int.MinValue} .. {int.MaxValue}");
        Console.WriteLine($"long: {long.MinValue} .. {long.MaxValue}");
        Console.WriteLine($"float: {float.MinValue} .. {float.MaxValue}");
        Console.WriteLine($"double: {double.MinValue} .. {double.MaxValue}");
    }

    private static void IncarcaDateInitialeDinCsv()
    {
        string caleCsv = Path.Combine(AppContext.BaseDirectory, NumeFisierMasiniCsv);
        int adaugate = admin.IncarcaMasiniDinCsv(caleCsv);

        int maxId = admin.GetMasini().Select(m => m.IdMasina).DefaultIfEmpty(0).Max();
        urmatorIdMasina = maxId + 1;

        Console.WriteLine($"Date initiale incarcate din CSV: {adaugate} masini.");
    }

    private static string TaieText(string text, int lungimeMaxima)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        return text.Length <= lungimeMaxima
            ? text
            : text[..(lungimeMaxima - 3)] + "...";
    }
}
