# APLICATIE PIU - INCHIRIERI MASINI (GHID SIMPLU)

Acest proiect este facut pe structura din laborator: solutie cu 3 proiecte, meniu in consola, modele separate, stocare in memorie.

## 1) STRUCTURA PROIECTULUI

- `InchirieriMasiniConsole` - aici ruleaza meniul si citirea de la tastatura.
- `LibrarieModele` - aici sunt clasele si tipurile folosite in aplicatie (`Masina`, `Rezervare`, `enum`, `struct`, `const`).
- `NivelStocareDate` - aici este lista de masini/rezervari + functii de cautare/modificare.

## 2) CE TREBUIE SA STIU SA EXPLIC

### LAB 1 - BAZA C#
- Aplicatie consola + meniu repetitiv (`while` + `switch`).
- Citire de la tastatura cu validari.
- Conversii `Convert.ToInt32`, `int.Parse`, `int.TryParse`.
- Afisare `MinValue` si `MaxValue` pentru tipuri numerice.

### LAB 2 - CLASE, PROPRIETATI, SIRURI
- Clase scrise in fisiere separate.
- Proprietati auto-implemented (`get; set;`) si proprietate calculata (`DenumireCompleta`).
- Prelucrare siruri cu `Split` si `Join`.

### LAB 3 - COLECTII SI ORGANIZARE SOLUTIE
- Colectii generice `List<Masina>` si `List<Rezervare>`.
- Cautare dupa criterii (marca, marca+model).
- Separare pe proiecte: consola + modele + stocare.
- Metoda de modificare in memorie (`ModificaPretMasina`).
- Incarcare date initiale din fisier CSV local (`masini_default.csv`).

### LAB 4 - ENUM, STRUCT, EXCEPTII, LINQ
- `const` pentru separator (`SeparatorAfisare`).
- `enum` pentru culoare/combustibil.
- `enum [Flags]` pentru optiuni multiple la masina.
- `struct` pentru perioada inchiriere.
- `try/catch` la citirea optiunilor numerice.
- LINQ in metodele de cautare (`Where`, `FirstOrDefault`, `ToList`).

## 3) MENIU APLICATIE (DEMO RAPID)

- `C` - adauga masina
- `A` - afiseaza toate masinile
- `F` - cauta masina dupa marca + model
- `N` - cauta masini dupa marca
- `M` - modifica pret masina
- `R` - rezerva masina
- `D` - afiseaza masini disponibile
- `V` - demo conversii string -> int
- `X` - iesire

Nota: la pornire se incarca automat masini din `masini_default.csv`, iar afisarea la optiunea `A` este tabelara.

