using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DokterspraktijkLib;

namespace WPFPatiënt
{
    /// <summary>
    /// Logica voor AfsprakenPage van de patiënt.
    /// </summary>
    public partial class AfsprakenPage : Page
    {
        private Patient patient;
        private List<Afspraak> getoondeAfspraken;
        private List<Dokter> alleDokters;

        public AfsprakenPage(Patient ingelogdePatient)
        {
            InitializeComponent();
            patient = ingelogdePatient;
            getoondeAfspraken = new List<Afspraak>();
            alleDokters = new List<Dokter>();

            // Abonneer op events programmatisch om XAML compiler issues te vermijden
            calAfspraakDatum.SelectedDatesChanged += CalAfspraakDatum_SelectedDatesChanged;

            // Laad gegevens
            LaadMijnAfspraken();
            LaadDoktersInComboBox();

            // Standaard selecteer vandaag voor de nieuwe afspraak datum picker
            calAfspraakDatum.SelectedDate = DateTime.Today;
        }

        /// <summary>
        /// Haalt de afspraken van de patiënt op en toont ze in de ListBox met filters.
        /// </summary>
        private void LaadMijnAfspraken()
        {
            lblFoutmelding.Text = "";
            lstAfspraken.Items.Clear();
            getoondeAfspraken.Clear();
            ResetDetailsWeergave();

            try
            {
                List<Afspraak> dbAfspraken = Afspraak.GetByPatient(patient.Id);
                DateTime nu = DateTime.Now;

                foreach (Afspraak afspraak in dbAfspraken)
                {
                    // Filter toepassen (toekomstig vs alle)
                    if (radToekomstig.IsChecked == true && afspraak.Moment < nu)
                    {
                        continue; // Sla eerdere afspraken over als we enkel toekomstige tonen
                    }

                    getoondeAfspraken.Add(afspraak);

                    string dokterNaam = afspraak.Dokter != null ? afspraak.Dokter.VolledigeNaam : "Onbekende dokter";
                    string datumTijd = afspraak.Moment.ToString("dd-MM-yyyy HH:mm");
                    
                    lstAfspraken.Items.Add(datumTijd + " - " + dokterNaam);
                }

                if (getoondeAfspraken.Count == 0)
                {
                    lstAfspraken.Items.Add("Geen afspraken gevonden.");
                }
            }
            catch (Exception ex)
            {
                lblFoutmelding.Foreground = System.Windows.Media.Brushes.Red;
                lblFoutmelding.Text = "Fout bij inladen afspraken: " + ex.Message;
            }
        }

        private void ResetDetailsWeergave()
        {
            pnlDetailsInhoud.Visibility = Visibility.Collapsed;
            lblGeenSelectie.Visibility = Visibility.Visible;
            btnAnnuleren.Visibility = Visibility.Collapsed;
        }

        private void LstAfspraken_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblFoutmelding.Text = "";
            int index = lstAfspraken.SelectedIndex;

            if (index >= 0 && index < getoondeAfspraken.Count)
            {
                Afspraak afspraak = getoondeAfspraken[index];
                
                // Vul details in
                txtDetDokterNaam.Text = afspraak.Dokter != null ? afspraak.Dokter.VolledigeNaam : "Onbekende dokter";
                txtDetDokterEmail.Text = afspraak.Dokter != null ? afspraak.Dokter.Email : "-";
                txtDetMoment.Text = afspraak.Moment.ToString("dddd dd MMMM yyyy om HH:mm");
                txtDetKlacht.Text = string.IsNullOrEmpty(afspraak.Klacht) ? "Geen reden opgegeven." : afspraak.Klacht;

                // Laad dokter foto
                if (afspraak.Dokter != null && afspraak.Dokter.Profielfotodata != null && afspraak.Dokter.Profielfotodata.Length > 0)
                {
                    try
                    {
                        BitmapImage img = new BitmapImage();
                        img.BeginInit();
                        img.StreamSource = new MemoryStream(afspraak.Dokter.Profielfotodata);
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.EndInit();
                        imgDetDokterFoto.ImageSource = img;
                    }
                    catch
                    {
                        imgDetDokterFoto.ImageSource = null;
                    }
                }
                else
                {
                    imgDetDokterFoto.ImageSource = null;
                }

                // Toon panelen
                lblGeenSelectie.Visibility = Visibility.Collapsed;
                pnlDetailsInhoud.Visibility = Visibility.Visible;

                // Annuleren knop is enkel zichtbaar voor afspraken in de toekomst
                if (afspraak.Moment >= DateTime.Now)
                {
                    btnAnnuleren.Visibility = Visibility.Visible;
                }
                else
                {
                    btnAnnuleren.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                ResetDetailsWeergave();
            }
        }

        private void RadFilter_Changed(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                LaadMijnAfspraken();
            }
        }

        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            lblFoutmelding.Text = "";
            int index = lstAfspraken.SelectedIndex;

            if (index < 0 || index >= getoondeAfspraken.Count)
            {
                lblFoutmelding.Foreground = System.Windows.Media.Brushes.Red;
                lblFoutmelding.Text = "Selecteer eerst een geldige afspraak.";
                return;
            }

            Afspraak afspraak = getoondeAfspraken[index];

            MessageBoxResult result = MessageBox.Show(
                "Bent u zeker dat u uw afspraak met " + afspraak.Dokter.VolledigeNaam + " op " + afspraak.Moment.ToString("dd-MM-yyyy om HH:mm") + " wilt annuleren?",
                "Afspraak annuleren",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool gelukt = afspraak.Delete();

                    if (gelukt)
                    {
                        lblFoutmelding.Foreground = System.Windows.Media.Brushes.Green;
                        lblFoutmelding.Text = "De afspraak is succesvol geannuleerd.";
                        LaadMijnAfspraken();
                    }
                    else
                    {
                        lblFoutmelding.Foreground = System.Windows.Media.Brushes.Red;
                        lblFoutmelding.Text = "Kon de afspraak niet verwijderen uit de database.";
                    }
                }
                catch (Exception ex)
                {
                    lblFoutmelding.Foreground = System.Windows.Media.Brushes.Red;
                    lblFoutmelding.Text = "Fout bij annuleren: " + ex.Message;
                }
            }
        }

        private void BtnNieuweAfspraakPagina_Click(object sender, RoutedEventArgs e)
        {
            lblFoutmelding.Text = "";
            pnlOverzicht.Visibility = Visibility.Collapsed;
            pnlNieuw.Visibility = Visibility.Visible;
            
            // Trigger tijdslot berekening
            UpdateBeschikbareTijdslots();
        }

        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            lblFoutmelding.Text = "";
            pnlNieuw.Visibility = Visibility.Collapsed;
            pnlOverzicht.Visibility = Visibility.Visible;
            
            LaadMijnAfspraken();
        }

        /// <summary>
        /// Laadt alle dokters uit de database in de ComboBox.
        /// </summary>
        private void LaadDoktersInComboBox()
        {
            try
            {
                cmbDokters.Items.Clear();
                alleDokters = Dokter.GetDokters();

                foreach (Dokter dokter in alleDokters)
                {
                    cmbDokters.Items.Add(dokter.VolledigeNaam);
                }

                if (cmbDokters.Items.Count > 0)
                {
                    cmbDokters.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                lblFoutmelding.Foreground = System.Windows.Media.Brushes.Red;
                lblFoutmelding.Text = "Fout bij inladen dokters: " + ex.Message;
            }
        }

        private void CmbDokters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblFoutmelding.Text = "";
            int index = cmbDokters.SelectedIndex;

            if (index >= 0 && index < alleDokters.Count)
            {
                Dokter dokter = alleDokters[index];
                
                // Vul de kaart in
                txtKaartDokterNaam.Text = dokter.VolledigeNaam;
                txtKaartRiziv.Text = dokter.RizivNummer.ToString();
                txtKaartGeconventioneerd.Text = dokter.IsGeconventioneerd ? "Ja" : "Nee";
                txtKaartEmail.Text = dokter.Email;

                // Laad foto
                if (dokter.Profielfotodata != null && dokter.Profielfotodata.Length > 0)
                {
                    try
                    {
                        BitmapImage img = new BitmapImage();
                        img.BeginInit();
                        img.StreamSource = new MemoryStream(dokter.Profielfotodata);
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.EndInit();
                        imgKaartDokterFoto.ImageSource = img;
                    }
                    catch
                    {
                        imgKaartDokterFoto.ImageSource = null;
                    }
                }
                else
                {
                    imgKaartDokterFoto.ImageSource = null;
                }

                pnlDokterKaart.Visibility = Visibility.Visible;
                
                // Bereken tijdslots opnieuw
                UpdateBeschikbareTijdslots();
            }
            else
            {
                pnlDokterKaart.Visibility = Visibility.Collapsed;
            }
        }

        private void CalAfspraakDatum_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateBeschikbareTijdslots();
        }

        /// <summary>
        /// Berekent de beschikbare tijdslots voor de geselecteerde dokter op de geselecteerde datum.
        /// Reeds geplande afspraken worden eruit gefilterd.
        /// </summary>
        private void UpdateBeschikbareTijdslots()
        {
            cmbTijdslots.Items.Clear();

            int dokterIndex = cmbDokters.SelectedIndex;
            if (dokterIndex < 0 || dokterIndex >= alleDokters.Count) return;
            if (!calAfspraakDatum.SelectedDate.HasValue) return;

            Dokter geselecteerdeDokter = alleDokters[dokterIndex];
            DateTime geselecteerdeDatum = calAfspraakDatum.SelectedDate.Value;

            try
            {
                // Haal alle bestaande afspraken van deze dokter op deze datum op
                List<Afspraak> geplandeAfspraken = Afspraak.GetByDokterAndDate(geselecteerdeDokter.Id, geselecteerdeDatum);
                
                // Maak een lijst van mogelijke uren (van 08:00 tot 17:00, elk half uur)
                List<string> alleSlots = new List<string>();
                alleSlots.Add("08:00"); alleSlots.Add("08:30");
                alleSlots.Add("09:00"); alleSlots.Add("09:30");
                alleSlots.Add("10:00"); alleSlots.Add("10:30");
                alleSlots.Add("11:00"); alleSlots.Add("11:30");
                alleSlots.Add("12:00"); // Middagpauze uitsluiten of behouden? We behouden het
                alleSlots.Add("13:00"); alleSlots.Add("13:30");
                alleSlots.Add("14:00"); alleSlots.Add("14:30");
                alleSlots.Add("15:00"); alleSlots.Add("15:30");
                alleSlots.Add("16:00"); alleSlots.Add("16:30");
                alleSlots.Add("17:00");

                // Filter reeds geboekte slots eruit (GEEN LINQ!)
                foreach (string slot in alleSlots)
                {
                    bool isBezet = false;

                    foreach (Afspraak geplande in geplandeAfspraken)
                    {
                        if (geplande.Moment.ToString("HH:mm") == slot)
                        {
                            isBezet = true;
                            break;
                        }
                    }

                    // Mag ook niet in het verleden liggen als de gekozen datum vandaag is
                    if (!isBezet && geselecteerdeDatum.Date == DateTime.Today)
                    {
                        string[] uurMin = slot.Split(':');
                        int slotUur = Convert.ToInt32(uurMin[0]);
                        int slotMin = Convert.ToInt32(uurMin[1]);
                        
                        DateTime slotTijd = DateTime.Today.AddHours(slotUur).AddMinutes(slotMin);
                        if (slotTijd <= DateTime.Now)
                        {
                            isBezet = true; // Sla slots over die al voorbij zijn vandaag
                        }
                    }

                    if (!isBezet)
                    {
                        cmbTijdslots.Items.Add(slot);
                    }
                }

                if (cmbTijdslots.Items.Count > 0)
                {
                    cmbTijdslots.SelectedIndex = 0;
                }
                else
                {
                    cmbTijdslots.Items.Add("Geen slots vrij op deze datum");
                    cmbTijdslots.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                lblFoutmelding.Foreground = System.Windows.Media.Brushes.Red;
                lblFoutmelding.Text = "Fout bij berekenen tijdslots: " + ex.Message;
            }
        }

        private void BtnBevestigen_Click(object sender, RoutedEventArgs e)
        {
            lblFoutmelding.Text = "";

            int dokterIndex = cmbDokters.SelectedIndex;
            if (dokterIndex < 0 || dokterIndex >= alleDokters.Count)
            {
                lblFoutmelding.Foreground = System.Windows.Media.Brushes.Red;
                lblFoutmelding.Text = "Gelieve een dokter te selecteren.";
                return;
            }

            if (!calAfspraakDatum.SelectedDate.HasValue)
            {
                lblFoutmelding.Foreground = System.Windows.Media.Brushes.Red;
                lblFoutmelding.Text = "Gelieve een datum te selecteren.";
                return;
            }

            if (cmbTijdslots.SelectedItem == null || cmbTijdslots.SelectedItem.ToString().Contains("Geen slots vrij"))
            {
                lblFoutmelding.Foreground = System.Windows.Media.Brushes.Red;
                lblFoutmelding.Text = "Er is geen geldig tijdslot geselecteerd.";
                return;
            }

            string klacht = txtKlacht.Text.Trim();
            if (string.IsNullOrEmpty(klacht))
            {
                lblFoutmelding.Foreground = System.Windows.Media.Brushes.Red;
                lblFoutmelding.Text = "Gelieve een reden voor het consult op te geven.";
                return;
            }

            Dokter gekozenDokter = alleDokters[dokterIndex];
            DateTime gekozenDatum = calAfspraakDatum.SelectedDate.Value;
            string gekozenTijd = cmbTijdslots.SelectedItem.ToString();

            try
            {
                // Converteer tijd naar DateTime
                string[] uurMin = gekozenTijd.Split(':');
                int uur = Convert.ToInt32(uurMin[0]);
                int minuut = Convert.ToInt32(uurMin[1]);

                DateTime afspraakMoment = new DateTime(
                    gekozenDatum.Year,
                    gekozenDatum.Month,
                    gekozenDatum.Day,
                    uur,
                    minuut,
                    0
                );

                // Controleer nogmaals of de afspraak niet in het verleden ligt
                if (afspraakMoment <= DateTime.Now)
                {
                    lblFoutmelding.Foreground = System.Windows.Media.Brushes.Red;
                    lblFoutmelding.Text = "U kunt geen afspraak in het verleden boeken.";
                    return;
                }

                // Maak afspraak instantie aan
                Afspraak nieuweAfspraak = new Afspraak(
                    afspraakMoment,
                    klacht,
                    patient.Id,
                    gekozenDokter.Id
                );

                // Bewaar in de database via de domeinklasse CRUD method
                bool gelukt = nieuweAfspraak.Create();

                if (gelukt)
                {
                    MessageBox.Show("Uw afspraak met " + gekozenDokter.VolledigeNaam + " op " + afspraakMoment.ToString("dd-MM-yyyy om HH:mm") + " is succesvol geboekt!", "Afspraak bevestigd", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Reset velden en switch terug naar overzicht
                    txtKlacht.Text = "";
                    pnlNieuw.Visibility = Visibility.Collapsed;
                    pnlOverzicht.Visibility = Visibility.Visible;
                    
                    LaadMijnAfspraken();
                }
                else
                {
                    lblFoutmelding.Foreground = System.Windows.Media.Brushes.Red;
                    lblFoutmelding.Text = "De afspraak kon niet worden opgeslagen in de database.";
                }
            }
            catch (Exception ex)
            {
                lblFoutmelding.Foreground = System.Windows.Media.Brushes.Red;
                lblFoutmelding.Text = "Fout bij boeken afspraak: " + ex.Message;
            }
        }
    }
}
