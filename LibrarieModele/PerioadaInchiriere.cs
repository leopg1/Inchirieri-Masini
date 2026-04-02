namespace LibrarieModele;

// LAB 4 - Implementarea 11: struct (tip valoare) pentru intervalul unei rezervari
public struct PerioadaInchiriere
{
    public DateOnly DataStart { get; set; }
    public DateOnly DataSfarsit { get; set; }

    public int NumarZile
    {
        get
        {
            int diferenta = DataSfarsit.DayNumber - DataStart.DayNumber + 1;
            return diferenta > 0 ? diferenta : 0;
        }
    }

    public override string ToString()
    {
        return $"{DataStart:dd.MM.yyyy} - {DataSfarsit:dd.MM.yyyy} ({NumarZile} zile)";
    }
}
