namespace LibrarieModele;

// LAB 4 - Implementarea 9: enum simplu pentru stare/alegere unica
public enum CuloareMasina
{
    Rosu = 1,
    Alb = 2,
    Negru = 3,
    Gri = 4,
    Albastru = 5
}

public enum TipCombustibil
{
    Benzina = 1,
    Motorina = 2,
    Hibrid = 3,
    Electric = 4
}

// LAB 4 - Implementarea 10: enum cu Flags pentru selectii multiple
[Flags]
public enum OptiuniMasina
{
    Nimic = 0,
    AerConditionat = 1,
    Navigatie = 2,
    CutieAutomata = 4,
    SenzoriParcare = 8
}
