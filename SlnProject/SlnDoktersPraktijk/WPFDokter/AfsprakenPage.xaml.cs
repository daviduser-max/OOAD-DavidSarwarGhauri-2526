using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using DokterspraktijkLib;

namespace WPFDokter
{
    public partial class AfsprakenPage : Page
    {
        private Dokter ingelogdeDokter;
        private List<Afspraak> actieveAfspraken;

        public AfsprakenPage(Dokter dokter)
        {
            InitializeComponent();
            ingelogdeDokter = dokter;
            actieveAfspraken = new List<Afspraak>();

            // Abonneer op event
            calDatum.SelectedDatesChanged += CalDatum_SelectedDatesChanged;

            // Standaard selecteren we vandaag
            calDatum.SelectedDate = DateTime.Today;
            LaadAfsprakenVanDatum(DateTime.Today);
        }

        private void CalDatum_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            lblFoutmelding.Text = "";
            if (calDatum.SelectedDate.HasValue)
            {
                DateTime geselecteerdeDatum = calDatum.SelectedDate.Value;
                LaadAfsprakenVanDatum(geselecteerdeDatum);
            }
        }

        private void LaadAfsprakenVanDatum(DateTime datum)
        {
            lblFoutmelding.Text = "";
            lstAfspraken.Items.Clear();
            actieveAfspraken.Clear();
            txtReden.Text = "Geen afspraak geselecteerd";
            txtReden.FontStyle = FontStyles.Italic;

            lblGeselecteerdeDatum.Text = "Afspraken voor " + datum.ToString("dddd dd MMMM yyyy");

            try
            {
                List<Afspraak> dbAfspraken = Afspraak.GetByDokterAndDate(ingelogdeDokter.Id, datum);

                foreach (Afspraak afspraak in dbAfspraken)
                {
                    actieveAfspraken.Add(afspraak);
                    
                    // Format van item in lijst: "UUR - Voornaam Achternaam"
                    string patientNaam = "";
                    if (afspraak.Patient != null)
                    {
                        patientNaam = afspraak.Patient.VolledigeNaam;
                    }
                    else
                    {
                        patientNaam = "Onbekende patiënt";
                    }

                    string uurString = afspraak.Moment.ToString("HH:mm");
                    lstAfspraken.Items.Add(uurString + " - " + patientNaam);
                }

                if (actieveAfspraken.Count == 0)
                {
                    lstAfspraken.Items.Add("Geen afspraken op deze dag.");
                }
            }
            catch (SqlException ex)
            {
                lblFoutmelding.Text = "Databasefout bij ophalen afspraken: " + ex.Message;
            }
            catch (Exception ex)
            {
                lblFoutmelding.Text = "Fout bij ophalen afspraken: " + ex.Message;
            }
        }

        private void LstAfspraken_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = lstAfspraken.SelectedIndex;

            // Controleer of er een geldige afspraak is geselecteerd en of de lijst niet leeg is
            if (index >= 0 && index < actieveAfspraken.Count)
            {
                Afspraak geselecteerd = actieveAfspraken[index];
                
                if (string.IsNullOrWhiteSpace(geselecteerd.Klacht))
                {
                    txtReden.Text = "Geen reden opgegeven.";
                }
                else
                {
                    txtReden.Text = geselecteerd.Klacht;
                }
                txtReden.FontStyle = FontStyles.Normal;
            }
            else
            {
                txtReden.Text = "Geen afspraak geselecteerd";
                txtReden.FontStyle = FontStyles.Italic;
            }
        }

        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            lblFoutmelding.Text = "";
            int index = lstAfspraken.SelectedIndex;

            if (index < 0 || index >= actieveAfspraken.Count)
            {
                lblFoutmelding.Text = "Gelieve eerst een geldige afspraak te selecteren.";
                return;
            }

            Afspraak teAnnuleren = actieveAfspraken[index];

            MessageBoxResult bevestiging = MessageBox.Show(
                "Bent u zeker dat u deze afspraak om " + teAnnuleren.Moment.ToString("HH:mm") + " wilt annuleren?",
                "Bevestig annulering",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (bevestiging == MessageBoxResult.Yes)
            {
                try
                {
                    bool gelukt = teAnnuleren.Delete();

                    if (gelukt)
                    {
                        MessageBox.Show("Afspraak succesvol geannuleerd.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        // Herlaad de afspraken voor de huidige geselecteerde datum
                        if (calDatum.SelectedDate.HasValue)
                        {
                            LaadAfsprakenVanDatum(calDatum.SelectedDate.Value);
                        }
                    }
                    else
                    {
                        lblFoutmelding.Text = "De afspraak kon niet worden verwijderd uit de databank.";
                    }
                }
                catch (SqlException ex)
                {
                    lblFoutmelding.Text = "Databasefout bij annuleren afspraak: " + ex.Message;
                }
                catch (Exception ex)
                {
                    lblFoutmelding.Text = "Fout bij annuleren afspraak: " + ex.Message;
                }
            }
        }
    }
}
