using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using LibrarieModele;
using NivelStocareDate;

namespace InchirieriMasiniWPF;

// LAB 10 - MainWindow implementeaza INotifyPropertyChanged pentru a expune
// MasinaCurenta si RezervareCurenta catre controalele legate prin Binding.
// LAB 11 - validarea (IDataErrorInfo) se face in clasele Masina si Rezervare.
public partial class MainWindow : Window, INotifyPropertyChanged
{
    private readonly AdministrareMasiniMemorie administrare = new();

    private Masina masinaCurenta = new();
    public Masina MasinaCurenta
    {
        get => masinaCurenta;
        set { masinaCurenta = value; OnPropertyChanged(); }
    }

    private Rezervare rezervareCurenta = new();
    public Rezervare RezervareCurenta
    {
        get => rezervareCurenta;
        set { rezervareCurenta = value; OnPropertyChanged(); }
    }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        IncarcaDateInitiale();
        PopuleazaControale();
        AfiseazaPanel(panelMasini);
    }

    // -------- initializare --------

    private void IncarcaDateInitiale()
    {
        string caleCsv = Path.Combine(AppContext.BaseDirectory, "masini_default.csv");
        administrare.IncarcaMasiniDinCsv(caleCsv);
    }

    private void PopuleazaControale()
    {
        // LAB 9 - umplere ComboBox cu valorile enum
        cmbCuloare.ItemsSource = Enum.GetValues<CuloareMasina>();

        // DataGrid-uri + ListBox legate de ObservableCollection-urile din stocare
        dgMasini.ItemsSource = administrare.Masini;
        dgRezervari.ItemsSource = administrare.Rezervari;
        lstMasiniDisponibile.ItemsSource = administrare.Masini;
        dgCautare.ItemsSource = administrare.GetMasini();

        // datepicker default: rezervare azi -> azi + 3 zile
        dtpDataStart.SelectedDate = DateTime.Today;
        dtpDataSfarsit.SelectedDate = DateTime.Today.AddDays(3);
        SincronizeazaOptiuniDinMasina();
    }

    // -------- navigare intre panouri (LAB 8 - meniu vertical) --------

    private void MeniuMasini_Click(object sender, RoutedEventArgs e) => AfiseazaPanel(panelMasini);
    private void MeniuRezervari_Click(object sender, RoutedEventArgs e)
    {
        // la fiecare deschidere afisez doar masinile disponibile in ListBox
        lstMasiniDisponibile.ItemsSource = administrare.GetMasiniDisponibile();
        AfiseazaPanel(panelRezervari);
    }
    private void MeniuCautare_Click(object sender, RoutedEventArgs e)
    {
        ActualizeazaCautare(txtCautare.Text);
        AfiseazaPanel(panelCautare);
    }

    private void AfiseazaPanel(UIElement panel)
    {
        panelMasini.Visibility    = panel == panelMasini    ? Visibility.Visible : Visibility.Collapsed;
        panelRezervari.Visibility = panel == panelRezervari ? Visibility.Visible : Visibility.Collapsed;
        panelCautare.Visibility   = panel == panelCautare   ? Visibility.Visible : Visibility.Collapsed;

        // evidentiaza butonul activ din sidebar (Tag="True" declanseaza style trigger)
        btnMeniuMasini.Tag    = panel == panelMasini    ? "True" : "False";
        btnMeniuRezervari.Tag = panel == panelRezervari ? "True" : "False";
        btnMeniuCautare.Tag   = panel == panelCautare   ? "True" : "False";

        ActualizeazaStats();
    }

    private void ActualizeazaStats()
    {
        int disponibile = administrare.GetMasiniDisponibile().Count;
        int total = administrare.GetMasini().Count;
        int rezervate = total - disponibile;
        decimal venit = administrare.GetRezervari().Sum(r => r.CostTotal);

        tbStatDisponibile.Text  = disponibile.ToString();
        tbStatRezervate.Text    = rezervate.ToString();
        tbStatsDisponibile.Text = disponibile.ToString();
        tbStatsVenit.Text       = $"{venit:N0} lei";
    }

    // -------- CRUD MASINA --------

    private void dgMasini_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (dgMasini.SelectedItem is not Masina masinaSelectata)
        {
            return;
        }

        // lucrez pe o copie (draft) ca modificarile din formular sa nu se aplice
        // direct in stocare; se aplica abia la apasarea butonului "Modifica".
        MasinaCurenta = ClonaMasina(masinaSelectata);
        SincronizeazaOptiuniDinMasina();
    }

    private void AdaugaMasina_Click(object sender, RoutedEventArgs e)
    {
        if (!ValideazaMasina())
        {
            return;
        }

        Masina masinaNoua = ClonaMasina(MasinaCurenta);
        masinaNoua.IdMasina = 0; // forteaza stocarea sa genereze un Id nou
        administrare.AdaugaMasina(masinaNoua);
        lstMasiniDisponibile.ItemsSource = administrare.GetMasiniDisponibile();
        ResetMasina();
    }

    private void ModificaMasina_Click(object sender, RoutedEventArgs e)
    {
        if (MasinaCurenta.IdMasina <= 0)
        {
            MessageBox.Show("Selecteaza intai o masina din tabel.", "Modifica masina", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (!ValideazaMasina())
        {
            return;
        }

        if (administrare.ModificaMasina(MasinaCurenta))
        {
            // refresh DataGrid (proprietatile sunt INPC, dar Optiuni e camp simplu)
            dgMasini.Items.Refresh();
            lstMasiniDisponibile.ItemsSource = administrare.GetMasiniDisponibile();
            ActualizeazaStats();
        }
    }

    private void StergeMasina_Click(object sender, RoutedEventArgs e)
    {
        if (MasinaCurenta.IdMasina <= 0)
        {
            MessageBox.Show("Selecteaza intai o masina din tabel.", "Sterge masina", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var raspuns = MessageBox.Show(
            $"Sigur stergi masina {MasinaCurenta.DenumireCompleta}?\nSe vor sterge si rezervarile asociate.",
            "Confirmare", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (raspuns != MessageBoxResult.Yes)
        {
            return;
        }

        administrare.StergeMasina(MasinaCurenta.IdMasina);
        lstMasiniDisponibile.ItemsSource = administrare.GetMasiniDisponibile();
        ResetMasina();
    }

    private void ReseteazaMasina_Click(object sender, RoutedEventArgs e) => ResetMasina();

    private void ResetMasina()
    {
        MasinaCurenta = new Masina();
        dgMasini.SelectedItem = null;
        SincronizeazaOptiuniDinMasina();
        ActualizeazaStats();
    }

    private bool ValideazaMasina()
    {
        // forteaza re-evaluarea ValidationRules pentru cazul "nu s-a tastat nimic"
        txtMarca.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
        txtModel.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
        txtPret.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

        if (!MasinaCurenta.EsteValid)
        {
            MessageBox.Show("Completeaza corect campurile (marca, model, pret).", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    // -------- CheckBox-uri pentru OptiuniMasina (enum [Flags]) --------

    private bool ignoraEvenimenteOptiuni;

    private void Optiuni_Schimbat(object sender, RoutedEventArgs e)
    {
        if (ignoraEvenimenteOptiuni || MasinaCurenta is null)
        {
            return;
        }

        OptiuniMasina rezultat = OptiuniMasina.Nimic;
        if (cbAer.IsChecked == true)     rezultat |= OptiuniMasina.AerConditionat;
        if (cbNav.IsChecked == true)     rezultat |= OptiuniMasina.Navigatie;
        if (cbAuto.IsChecked == true)    rezultat |= OptiuniMasina.CutieAutomata;
        if (cbSenzori.IsChecked == true) rezultat |= OptiuniMasina.SenzoriParcare;

        MasinaCurenta.Optiuni = rezultat;
    }

    private void SincronizeazaOptiuniDinMasina()
    {
        ignoraEvenimenteOptiuni = true;
        var optiuni = MasinaCurenta?.Optiuni ?? OptiuniMasina.Nimic;
        cbAer.IsChecked     = optiuni.HasFlag(OptiuniMasina.AerConditionat);
        cbNav.IsChecked     = optiuni.HasFlag(OptiuniMasina.Navigatie);
        cbAuto.IsChecked    = optiuni.HasFlag(OptiuniMasina.CutieAutomata);
        cbSenzori.IsChecked = optiuni.HasFlag(OptiuniMasina.SenzoriParcare);
        ignoraEvenimenteOptiuni = false;
    }

    private static Masina ClonaMasina(Masina sursa) => new()
    {
        IdMasina = sursa.IdMasina,
        Marca = sursa.Marca,
        Model = sursa.Model,
        PretPeZi = sursa.PretPeZi,
        Disponibila = sursa.Disponibila,
        Culoare = sursa.Culoare,
        Combustibil = sursa.Combustibil,
        Optiuni = sursa.Optiuni,
        DataAdaugare = sursa.DataAdaugare,
        DataActualizare = sursa.DataActualizare
    };

    // -------- CRUD REZERVARE --------

    private void lstMasiniDisponibile_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lstMasiniDisponibile.SelectedItem is not Masina masinaAleasa)
        {
            tbInfoMasinaAleasa.Text = string.Empty;
            return;
        }

        RezervareCurenta.IdMasina = masinaAleasa.IdMasina;
        RezervareCurenta.PretPeZi = masinaAleasa.PretPeZi;
        tbInfoMasinaAleasa.Text = $"{masinaAleasa.DenumireCompleta} — {masinaAleasa.PretPeZi:N2} lei/zi";
        ActualizeazaPerioadaSiCost();
    }

    private void DataRezervare_Schimbata(object sender, SelectionChangedEventArgs e)
    {
        ActualizeazaPerioadaSiCost();
    }

    private void ActualizeazaPerioadaSiCost()
    {
        DateOnly start = DateOnly.FromDateTime(dtpDataStart.SelectedDate ?? DateTime.Today);
        DateOnly sfarsit = DateOnly.FromDateTime(dtpDataSfarsit.SelectedDate ?? DateTime.Today);

        RezervareCurenta.Perioada = new PerioadaInchiriere
        {
            DataStart = start,
            DataSfarsit = sfarsit
        };

        tbCostTotal.Text = RezervareCurenta.IdMasina > 0
            ? $"{RezervareCurenta.CostTotal:N2} lei  ·  {RezervareCurenta.Perioada.NumarZile} zile"
            : "0,00 lei";
    }

    private void AdaugaRezervare_Click(object sender, RoutedEventArgs e)
    {
        if (!ValideazaRezervare(verificaMasinaAleasa: true))
        {
            return;
        }

        Rezervare rezervareNoua = ClonaRezervare(RezervareCurenta);
        rezervareNoua.IdRezervare = 0;

        if (!administrare.AdaugaRezervare(rezervareNoua))
        {
            MessageBox.Show("Masina nu mai este disponibila pentru rezervare.", "Rezervare", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        lstMasiniDisponibile.ItemsSource = administrare.GetMasiniDisponibile();
        dgMasini.Items.Refresh();
        ResetRezervare();
    }

    private void ModificaRezervare_Click(object sender, RoutedEventArgs e)
    {
        if (RezervareCurenta.IdRezervare <= 0)
        {
            MessageBox.Show("Selecteaza intai o rezervare din tabel.", "Modifica rezervare", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (!ValideazaRezervare(verificaMasinaAleasa: false))
        {
            return;
        }

        if (administrare.ModificaRezervare(RezervareCurenta))
        {
            dgRezervari.Items.Refresh();
        }
    }

    private void StergeRezervare_Click(object sender, RoutedEventArgs e)
    {
        if (RezervareCurenta.IdRezervare <= 0)
        {
            MessageBox.Show("Selecteaza intai o rezervare din tabel.", "Anuleaza rezervare", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var raspuns = MessageBox.Show(
            $"Sigur anulezi rezervarea pentru {RezervareCurenta.NumeClient}?",
            "Confirmare", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (raspuns != MessageBoxResult.Yes)
        {
            return;
        }

        administrare.StergeRezervare(RezervareCurenta.IdRezervare);
        lstMasiniDisponibile.ItemsSource = administrare.GetMasiniDisponibile();
        dgMasini.Items.Refresh();
        ResetRezervare();
    }

    private void ReseteazaRezervare_Click(object sender, RoutedEventArgs e) => ResetRezervare();

    private void ResetRezervare()
    {
        RezervareCurenta = new Rezervare();
        dgRezervari.SelectedItem = null;
        lstMasiniDisponibile.SelectedItem = null;
        tbInfoMasinaAleasa.Text = string.Empty;
        tbCostTotal.Text = "0,00 lei";
        dtpDataStart.SelectedDate = DateTime.Today;
        dtpDataSfarsit.SelectedDate = DateTime.Today.AddDays(3);
        ActualizeazaStats();
    }

    private bool ValideazaRezervare(bool verificaMasinaAleasa)
    {
        txtNumeClient.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
        txtTelefon.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

        if (verificaMasinaAleasa && RezervareCurenta.IdMasina <= 0)
        {
            MessageBox.Show("Alege o masina din lista de masini disponibile.", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (!RezervareCurenta.EsteValid)
        {
            MessageBox.Show("Completeaza corect numele clientului si numarul de telefon.", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (RezervareCurenta.Perioada.NumarZile <= 0)
        {
            MessageBox.Show("Data de sfarsit trebuie sa fie dupa data de start.", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (RezervareCurenta.Perioada.NumarZile > ConstanteAplicatie.PerioadaMaximaZile)
        {
            MessageBox.Show($"Perioada nu poate depasi {ConstanteAplicatie.PerioadaMaximaZile} zile.", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private void dgRezervari_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (dgRezervari.SelectedItem is not Rezervare rezervareSelectata)
        {
            return;
        }

        RezervareCurenta = ClonaRezervare(rezervareSelectata);
        dtpDataStart.SelectedDate = RezervareCurenta.Perioada.DataStart.ToDateTime(TimeOnly.MinValue);
        dtpDataSfarsit.SelectedDate = RezervareCurenta.Perioada.DataSfarsit.ToDateTime(TimeOnly.MinValue);

        Masina? masina = administrare.GetMasina(rezervareSelectata.IdMasina);
        tbInfoMasinaAleasa.Text = masina is null ? string.Empty : $"{masina.DenumireCompleta} — {masina.PretPeZi:N2} lei/zi";
        tbCostTotal.Text = $"Cost total: {RezervareCurenta.CostTotal:N2} lei  ({RezervareCurenta.Perioada.NumarZile} zile)";
    }

    private static Rezervare ClonaRezervare(Rezervare sursa) => new()
    {
        IdRezervare = sursa.IdRezervare,
        IdMasina = sursa.IdMasina,
        NumeClient = sursa.NumeClient,
        NumarTelefon = sursa.NumarTelefon,
        Perioada = sursa.Perioada,
        PretPeZi = sursa.PretPeZi,
        DataActualizare = sursa.DataActualizare
    };

    // -------- CAUTARE (LAB 11 - filtrare in timp real) --------

    private void txtCautare_TextChanged(object sender, TextChangedEventArgs e)
    {
        ActualizeazaCautare(txtCautare.Text);
    }

    private void ReseteazaCautare_Click(object sender, RoutedEventArgs e)
    {
        txtCautare.Clear();
    }

    private void ActualizeazaCautare(string text)
    {
        var rezultate = administrare.CautaMasini(text);
        dgCautare.ItemsSource = rezultate;
        tbInfoCautare.Text = rezultate.Count == 0
            ? "Nu a fost gasita nici o masina cu acest termen."
            : $"Rezultate gasite: {rezultate.Count}";
    }

    // -------- INotifyPropertyChanged --------

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
