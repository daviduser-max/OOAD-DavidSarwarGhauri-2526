using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using DokterspraktijkLib;

namespace WPFPatiënt
{
    /// <summary>
    /// Logica voor ProfielPage van de patiënt.
    /// </summary>
    public partial class ProfielPage : Page
    {
        private Patient patient;
        private byte[] geselecteerdeFotoData;

        public ProfielPage(Patient ingelogdePatient)
        {
            InitializeComponent();
            patient = ingelogdePatient;
            
            LaadPatientGegevens();
        }

        private void LaadPatientGegevens()
        {
            lblFoutmelding.Text = "";
            
            txtVoornaam.Text = patient.Voornaam;
            txtAchternaam.Text = patient.Achternaam;
            dpGeboortedatum.SelectedDate = patient.Geboortedatum;
            
            if (patient.Geslacht == 1)
            {
                radMan.IsChecked = true;
            }
            else
            {
                radVrouw.IsChecked = true;
            }

            txtEmail.Text = patient.Email;
            txtGsm.Text = patient.Gsm;
            cmbNotificaties.SelectedIndex = (int)patient.Notificatie;

            geselecteerdeFotoData = patient.Profielfotodata;
            UpdateFotoWeergave();
        }

        private void UpdateFotoWeergave()
        {
            if (geselecteerdeFotoData != null && geselecteerdeFotoData.Length > 0)
            {
                try
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.StreamSource = new MemoryStream(geselecteerdeFotoData);
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.EndInit();
                    imgProfielfoto.Source = img;
                }
                catch
                {
                    imgProfielfoto.Source = null;
                }
            }
            else
            {
                imgProfielfoto.Source = null;
            }
        }

        private void BtnUploadFoto_Click(object sender, RoutedEventArgs e)
        {
            lblFoutmelding.Text = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Afbeeldingen (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    byte[] fotoBytes = File.ReadAllBytes(openFileDialog.FileName);
                    
                    // Controleer foto grootte (limiet tot 2MB voor DB prestaties)
                    if (fotoBytes.Length > 2 * 1024 * 1024)
                    {
                        lblFoutmelding.Text = "Afbeelding is te groot. Selecteer een foto kleiner dan 2MB.";
                        return;
                    }

                    geselecteerdeFotoData = fotoBytes;
                    UpdateFotoWeergave();
                }
                catch (Exception ex)
                {
                    lblFoutmelding.Text = "Fout bij inladen foto: " + ex.Message;
                }
            }
        }

        private void BtnVerwijderFoto_Click(object sender, RoutedEventArgs e)
        {
            geselecteerdeFotoData = null;
            imgProfielfoto.Source = null;
        }

        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            lblFoutmelding.Text = "";

            string voornaam = txtVoornaam.Text.Trim();
            string achternaam = txtAchternaam.Text.Trim();
            string email = txtEmail.Text.Trim();
            string gsm = txtGsm.Text.Trim();
            string nieuwWachtwoord = txtWachtwoord.Text;

            // Formulier validatie
            if (string.IsNullOrEmpty(voornaam) || string.IsNullOrEmpty(achternaam) || string.IsNullOrEmpty(email))
            {
                lblFoutmelding.Text = "Voornaam, Achternaam en E-mailadres zijn verplichte velden.";
                return;
            }

            if (!dpGeboortedatum.SelectedDate.HasValue)
            {
                lblFoutmelding.Text = "Gelieve een geldige geboortedatum te selecteren.";
                return;
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                lblFoutmelding.Text = "Gelieve een geldig e-mailadres in te vullen.";
                return;
            }

            // Gsm validatie: mag enkel cijfers bevatten
            if (!string.IsNullOrEmpty(gsm))
            {
                foreach (char c in gsm)
                {
                    if (c < '0' || c > '9')
                    {
                        lblFoutmelding.Text = "Het GSM-nummer mag enkel cijfers bevatten.";
                        return;
                    }
                }
            }

            try
            {
                // Update object waarden
                patient.Voornaam = voornaam;
                patient.Achternaam = achternaam;
                patient.Geboortedatum = dpGeboortedatum.SelectedDate.Value;
                patient.Geslacht = radMan.IsChecked == true ? 1 : 2;
                patient.Email = email;
                patient.Gsm = gsm;
                patient.Notificatie = (NotificatieType)cmbNotificaties.SelectedIndex;
                patient.Profielfotodata = geselecteerdeFotoData;

                // Enkel updaten indien er een nieuw wachtwoord is ingevuld
                if (!string.IsNullOrWhiteSpace(nieuwWachtwoord))
                {
                    if (nieuwWachtwoord.Length < 6)
                    {
                        lblFoutmelding.Text = "Het nieuwe wachtwoord moet minstens 6 karakters lang zijn.";
                        return;
                    }
                    patient.Passwoord = nieuwWachtwoord;
                }

                // Voer de update uit in de database via de domeinklasse
                bool gelukt = patient.Update();

                if (gelukt)
                {
                    // Update header in MainWindow direct
                    MainWindow main = (MainWindow)Window.GetWindow(this);
                    if (main != null)
                    {
                        main.UpdateProfielHeader();
                    }

                    // Toon succesmelding in het TextBlock (in groen via Foreground)
                    lblFoutmelding.Foreground = System.Windows.Media.Brushes.Green;
                    lblFoutmelding.Text = "Uw gegevens zijn succesvol bijgewerkt!";
                    
                    // Reset tekstkleur na 3 seconden is niet nodig, maar we herstellen het rood bij fouten
                    txtWachtwoord.Text = "";
                }
                else
                {
                    lblFoutmelding.Foreground = System.Windows.Media.Brushes.Red;
                    lblFoutmelding.Text = "De gegevens konden niet worden bijgewerkt in de database.";
                }
            }
            catch (Exception ex)
            {
                lblFoutmelding.Foreground = System.Windows.Media.Brushes.Red;
                lblFoutmelding.Text = "Fout bij opslaan: " + ex.Message;
            }
        }
    }
}
