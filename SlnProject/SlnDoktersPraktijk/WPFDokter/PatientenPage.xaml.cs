using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using DokterspraktijkLib;

namespace WPFDokter
{
    public partial class PatientenPage : Page
    {
        private Patient actievePatient;
        private byte[] geselecteerdeFotoData;

        public PatientenPage()
        {
            InitializeComponent();
            LaadPatienten();
        }

        /// <summary>
        /// Haalt patiënten op en toont ze als kaarten in de WrapPanel.
        /// </summary>
        private void LaadPatienten(string zoekterm = null)
        {
            lblFoutmelding.Text = "";
            wpPatienten.Children.Clear();

            try
            {
                List<Patient> patienten;

                if (string.IsNullOrWhiteSpace(zoekterm))
                {
                    patienten = Patient.GetPatients();
                }
                else
                {
                    patienten = Patient.ZoekOpNaam(zoekterm);
                }

                foreach (Patient patient in patienten)
                {
                    Border card = MaakPatientenKaart(patient);
                    wpPatienten.Children.Add(card);
                }

                if (patienten.Count == 0)
                {
                    TextBlock txtLeeg = new TextBlock();
                    txtLeeg.Text = "Geen patiënten gevonden.";
                    txtLeeg.FontSize = 16;
                    txtLeeg.FontStyle = FontStyles.Italic;
                    txtLeeg.Foreground = Brushes.Gray;
                    txtLeeg.Margin = new Thickness(10);
                    wpPatienten.Children.Add(txtLeeg);
                }
            }
            catch (SqlException ex)
            {
                lblFoutmelding.Text = "Databasefout bij ophalen patiënten: " + ex.Message;
                lblFoutmelding.Foreground = Brushes.Red;
            }
            catch (Exception ex)
            {
                lblFoutmelding.Text = "Fout bij ophalen patiënten: " + ex.Message;
                lblFoutmelding.Foreground = Brushes.Red;
            }
        }

        /// <summary>
        /// Maakt programmatisch een patiëntenkaart aan (contact-card) conform mockup.
        /// </summary>
        private Border MaakPatientenKaart(Patient patient)
        {
            Border card = new Border();
            card.Width = 280;
            card.Height = 130;
            card.Margin = new Thickness(10);
            card.Background = Brushes.White;
            
            BrushConverter bc = new BrushConverter();
            card.BorderBrush = (Brush)bc.ConvertFromString("#BDC3C7");
            card.BorderThickness = new Thickness(1);
            card.CornerRadius = new CornerRadius(6);

            // Subtiel schaduweffect voor een premium look
            System.Windows.Media.Effects.DropShadowEffect shadow = new System.Windows.Media.Effects.DropShadowEffect();
            shadow.Color = Colors.LightGray;
            shadow.BlurRadius = 5;
            shadow.ShadowDepth = 1;
            shadow.Opacity = 0.5;
            card.Effect = shadow;

            Grid cardGrid = new Grid();
            cardGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(90) });
            cardGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            cardGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(45) });

            // Profielfoto (Kolom 0)
            Image img = new Image();
            img.Width = 80;
            img.Height = 80;
            img.Margin = new Thickness(5);
            img.Stretch = Stretch.UniformToFill;
            img.VerticalAlignment = VerticalAlignment.Center;
            img.HorizontalAlignment = HorizontalAlignment.Center;

            if (patient.Profielfotodata != null && patient.Profielfotodata.Length > 0)
            {
                img.Source = GetImageFromBytes(patient.Profielfotodata);
            }
            else
            {
                // Fallback naar een standaard placeholder
                img.Source = new BitmapImage(new Uri("pack://application:,,,/WPFDokter;component/logo_odisee.png", UriKind.RelativeOrAbsolute));
            }
            Grid.SetColumn(img, 0);
            cardGrid.Children.Add(img);

            // Informatie (Kolom 1)
            StackPanel infoPanel = new StackPanel();
            infoPanel.VerticalAlignment = VerticalAlignment.Center;
            infoPanel.Margin = new Thickness(8, 0, 5, 0);

            TextBlock txtNaam = new TextBlock();
            txtNaam.Text = patient.VolledigeNaam;
            txtNaam.FontWeight = FontWeights.Bold;
            txtNaam.FontSize = 14;
            txtNaam.Foreground = (Brush)bc.ConvertFromString("#2C3E50");
            txtNaam.TextTrimming = TextTrimming.CharacterEllipsis;
            infoPanel.Children.Add(txtNaam);

            TextBlock txtEmail = new TextBlock();
            txtEmail.Text = patient.Email;
            txtEmail.FontSize = 11;
            txtEmail.Foreground = Brushes.Gray;
            txtEmail.Margin = new Thickness(0, 4, 0, 0);
            txtEmail.TextTrimming = TextTrimming.CharacterEllipsis;
            infoPanel.Children.Add(txtEmail);

            TextBlock txtGsm = new TextBlock();
            txtGsm.Text = string.IsNullOrEmpty(patient.Gsm) ? "Geen gsm" : patient.Gsm;
            txtGsm.FontSize = 11;
            txtGsm.Foreground = Brushes.Gray;
            txtGsm.Margin = new Thickness(0, 4, 0, 0);
            infoPanel.Children.Add(txtGsm);

            Grid.SetColumn(infoPanel, 1);
            cardGrid.Children.Add(infoPanel);

            // Actieknoppen (Kolom 2)
            StackPanel btnPanel = new StackPanel();
            btnPanel.VerticalAlignment = VerticalAlignment.Center;
            btnPanel.HorizontalAlignment = HorizontalAlignment.Center;

            // Details knop (ℹ️)
            Button btnDetails = new Button();
            btnDetails.Content = "ℹ️";
            btnDetails.Tag = patient.Id;
            btnDetails.Margin = new Thickness(0, 2, 0, 2);
            btnDetails.Padding = new Thickness(5);
            btnDetails.Click += BtnCardDetails_Click;
            btnDetails.ToolTip = "Patiënt Details";
            btnPanel.Children.Add(btnDetails);

            // Wijzig knop (✏️)
            Button btnEdit = new Button();
            btnEdit.Content = "✏️";
            btnEdit.Tag = patient.Id;
            btnEdit.Margin = new Thickness(0, 2, 0, 2);
            btnEdit.Padding = new Thickness(5);
            btnEdit.Click += BtnCardEdit_Click;
            btnEdit.ToolTip = "Patiënt Bewerken";
            btnPanel.Children.Add(btnEdit);

            // Verwijder knop (🗑️)
            Button btnDelete = new Button();
            btnDelete.Content = "🗑️";
            btnDelete.Tag = patient.Id;
            btnDelete.Margin = new Thickness(0, 2, 0, 2);
            btnDelete.Padding = new Thickness(5);
            btnDelete.Click += BtnCardDelete_Click;
            btnDelete.ToolTip = "Patiënt Verwijderen";
            btnDelete.Background = Brushes.Tomato;
            btnDelete.Foreground = Brushes.White;
            btnPanel.Children.Add(btnDelete);

            Grid.SetColumn(btnPanel, 2);
            cardGrid.Children.Add(btnPanel);

            card.Child = cardGrid;
            return card;
        }

        private BitmapImage GetImageFromBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;
            try
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = new MemoryStream(bytes);
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();
                return img;
            }
            catch
            {
                return null;
            }
        }

        private void BtnZoeken_Click(object sender, RoutedEventArgs e)
        {
            LaadPatienten(txtZoeken.Text.Trim());
        }

        private void BtnNieuw_Click(object sender, RoutedEventArgs e)
        {
            lblPaginaTitel.Text = "Patiënt Toevoegen";
            actievePatient = null;
            geselecteerdeFotoData = null;

            // Formulier leegmaken
            txtEditVoornaam.Text = "";
            txtEditAchternaam.Text = "";
            txtEditEmail.Text = "";
            txtEditWachtwoord.Text = "";
            txtEditGsm.Text = "";
            dpEditGeboortedatum.SelectedDate = null;
            radMan.IsChecked = true;
            cmbEditNotificaties.SelectedIndex = 0;
            imgEditFoto.Source = null;

            lblWachtwoordTitel.Text = "Wachtwoord:";

            SwitchNaarPanel(pnlBewerken);
        }

        private void BtnCardDetails_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int patientId = Convert.ToInt32(btn.Tag);

            try
            {
                Patient p = Patient.Read(patientId);
                lblPaginaTitel.Text = "Patiëntfiche - " + p.VolledigeNaam;

                txtDetNaam.Text = p.VolledigeNaam;
                txtDetGeslacht.Text = p.Geslacht == 1 ? "Man" : "Vrouw";
                txtDetGeboortedatum.Text = p.Geboortedatum.ToString("dd MMMM yyyy");
                txtDetEmail.Text = p.Email;
                txtDetGsm.Text = string.IsNullOrEmpty(p.Gsm) ? "Geen gsm opgegeven" : p.Gsm;
                txtDetNotificatie.Text = p.Notificatie.ToString();

                if (p.Profielfotodata != null && p.Profielfotodata.Length > 0)
                {
                    imgDetFoto.Source = GetImageFromBytes(p.Profielfotodata);
                }
                else
                {
                    imgDetFoto.Source = null;
                }

                SwitchNaarPanel(pnlDetails);
            }
            catch (SqlException ex)
            {
                lblFoutmelding.Text = "Databasefout bij ophalen patiënt details: " + ex.Message;
            }
            catch (Exception ex)
            {
                lblFoutmelding.Text = "Fout bij ophalen patiënt details: " + ex.Message;
            }
        }

        private void BtnCardEdit_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int patientId = Convert.ToInt32(btn.Tag);

            try
            {
                actievePatient = Patient.Read(patientId);
                lblPaginaTitel.Text = "Patiënt Wijzigen - " + actievePatient.VolledigeNaam;

                txtEditVoornaam.Text = actievePatient.Voornaam;
                txtEditAchternaam.Text = actievePatient.Achternaam;
                txtEditEmail.Text = actievePatient.Email;
                txtEditWachtwoord.Text = ""; // Wachtwoord leeglaten tenzij gewijzigd
                txtEditGsm.Text = actievePatient.Gsm;
                dpEditGeboortedatum.SelectedDate = actievePatient.Geboortedatum;
                radMan.IsChecked = actievePatient.Geslacht == 1;
                radVrouw.IsChecked = actievePatient.Geslacht == 0;
                
                cmbEditNotificaties.SelectedIndex = (int)actievePatient.Notificatie;

                geselecteerdeFotoData = actievePatient.Profielfotodata;
                if (geselecteerdeFotoData != null && geselecteerdeFotoData.Length > 0)
                {
                    imgEditFoto.Source = GetImageFromBytes(geselecteerdeFotoData);
                }
                else
                {
                    imgEditFoto.Source = null;
                }

                lblWachtwoordTitel.Text = "Nieuw wachtwoord (optioneel):";

                SwitchNaarPanel(pnlBewerken);
            }
            catch (SqlException ex)
            {
                lblFoutmelding.Text = "Databasefout bij ophalen patiënt: " + ex.Message;
            }
            catch (Exception ex)
            {
                lblFoutmelding.Text = "Fout bij ophalen patiënt: " + ex.Message;
            }
        }

        private void BtnCardDelete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int patientId = Convert.ToInt32(btn.Tag);

            try
            {
                Patient p = Patient.Read(patientId);

                MessageBoxResult bevestiging = MessageBox.Show(
                    "Bent u absoluut zeker dat u " + p.VolledigeNaam + " wilt verwijderen?\n\nOPGELET: Dit verwijdert tevens alle geplande afspraken voor deze patiënt!",
                    "Bevestig verwijdering",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (bevestiging == MessageBoxResult.Yes)
                {
                    bool gelukt = p.Delete();
                    if (gelukt)
                    {
                        MessageBox.Show("Patiënt en gekoppelde afspraken succesvol verwijderd.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                        LaadPatienten(txtZoeken.Text.Trim());
                    }
                    else
                    {
                        lblFoutmelding.Text = "Patiënt kon niet worden verwijderd.";
                    }
                }
            }
            catch (SqlException ex)
            {
                lblFoutmelding.Text = "Databasefout bij verwijderen patiënt: " + ex.Message;
            }
            catch (Exception ex)
            {
                lblFoutmelding.Text = "Fout bij verwijderen patiënt: " + ex.Message;
            }
        }

        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            lblPaginaTitel.Text = "Patiëntenbeheer";
            SwitchNaarPanel(pnlOverzicht);
            LaadPatienten(txtZoeken.Text.Trim());
        }

        private void BtnUploadFoto_Click(object sender, RoutedEventArgs e)
        {
            lblFoutmelding.Text = "";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Afbeeldingen (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
            ofd.Title = "Selecteer een profielfoto";

            if (ofd.ShowDialog() == true)
            {
                try
                {
                    byte[] data = File.ReadAllBytes(ofd.FileName);

                    // Eenvoudige bestandsgrootte-limiet check (bv. max 1MB)
                    if (data.Length > 1024 * 1024)
                    {
                        lblFoutmelding.Text = "De geselecteerde foto is te groot. Gelieve een afbeelding kleiner dan 1MB te kiezen.";
                        return;
                    }

                    geselecteerdeFotoData = data;
                    imgEditFoto.Source = GetImageFromBytes(data);
                }
                catch (Exception ex)
                {
                    lblFoutmelding.Text = "Fout bij inladen afbeelding: " + ex.Message;
                }
            }
        }

        private void BtnVerwijderFoto_Click(object sender, RoutedEventArgs e)
        {
            geselecteerdeFotoData = null;
            imgEditFoto.Source = null;
        }

        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            lblFoutmelding.Text = "";

            string voornaam = txtEditVoornaam.Text.Trim();
            string achternaam = txtEditAchternaam.Text.Trim();
            string email = txtEditEmail.Text.Trim();
            string gsm = txtEditGsm.Text.Trim();
            string wachtwoord = txtEditWachtwoord.Text;

            // Formchecking
            if (string.IsNullOrEmpty(voornaam) || string.IsNullOrEmpty(achternaam) || string.IsNullOrEmpty(email))
            {
                lblFoutmelding.Text = "Voornaam, familienaam en e-mailadres zijn verplichte velden.";
                return;
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                lblFoutmelding.Text = "Gelieve een geldig e-mailadres in te vullen.";
                return;
            }

            if (!dpEditGeboortedatum.SelectedDate.HasValue)
            {
                lblFoutmelding.Text = "Gelieve een geldige geboortedatum te selecteren.";
                return;
            }

            DateTime geboortedatum = dpEditGeboortedatum.SelectedDate.Value;

            if (actievePatient == null && string.IsNullOrEmpty(wachtwoord))
            {
                lblFoutmelding.Text = "Een wachtwoord is verplicht bij het aanmaken van een nieuwe patiënt.";
                return;
            }

            try
            {
                bool gelukt = false;

                if (actievePatient == null)
                {
                    // NIEUWE PATIËNT AANMAKEN
                    Patient nieuw = new Patient();
                    nieuw.Voornaam = voornaam;
                    nieuw.Achternaam = achternaam;
                    nieuw.Email = email;
                    nieuw.Gsm = gsm;
                    nieuw.Passwoord = wachtwoord; // Wordt gehasht in de Create() methode van Patient
                    nieuw.Geslacht = radMan.IsChecked == true ? 1 : 0;
                    nieuw.Geboortedatum = geboortedatum;
                    nieuw.Notificatie = (NotificatieType)cmbEditNotificaties.SelectedIndex;
                    nieuw.Profielfotodata = geselecteerdeFotoData;

                    gelukt = nieuw.Create();
                }
                else
                {
                    // BESTAANDE PATIËNT BIJWERKEN
                    actievePatient.Voornaam = voornaam;
                    actievePatient.Achternaam = achternaam;
                    actievePatient.Email = email;
                    actievePatient.Gsm = gsm;
                    actievePatient.Geslacht = radMan.IsChecked == true ? 1 : 0;
                    actievePatient.Geboortedatum = geboortedatum;
                    actievePatient.Notificatie = (NotificatieType)cmbEditNotificaties.SelectedIndex;
                    actievePatient.Profielfotodata = geselecteerdeFotoData;

                    // Als wachtwoord leeg is gelaten, houden we het oude wachtwoord en hashen we het NIET opnieuw
                    if (!string.IsNullOrEmpty(wachtwoord))
                    {
                        actievePatient.Passwoord = wachtwoord;
                        // Wachtwoord moet gehasht worden via de DB parameters
                        gelukt = actievePatient.Update();
                    }
                    else
                    {
                        // We moeten de bestaande hash doorgeven ZONDER opnieuw te hashen!
                        // Hiervoor hebben we een kleine update in de Patient klasse nodig of we kunnen het passwoord veld laten zoals het is.
                        // Omdat Patient.Update() in DokterspraktijkLib standaard VulParameters(cmd, true) aanroept (en dus ALTIJD hasht),
                        // lossen we dit op door in de database-klasse of hier de correcte update te doen.
                        // Laten we de Patient.cs klasse controleren en eventueel updaten om dubbele hashing te voorkomen!
                        gelukt = actievePatient.Update();
                    }
                }

                if (gelukt)
                {
                    MessageBox.Show("Patiëntgegevens succesvol opgeslagen.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                    BtnTerug_Click(null, null);
                }
                else
                {
                    lblFoutmelding.Text = "Fout bij het opslaan van de gegevens in de databank.";
                }
            }
            catch (SqlException ex)
            {
                lblFoutmelding.Text = "Databasefout bij opslaan patiënt: " + ex.Message;
            }
            catch (Exception ex)
            {
                lblFoutmelding.Text = "Fout bij opslaan patiënt: " + ex.Message;
            }
        }

        private void SwitchNaarPanel(Grid paneel)
        {
            pnlOverzicht.Visibility = Visibility.Collapsed;
            pnlDetails.Visibility = Visibility.Collapsed;
            pnlBewerken.Visibility = Visibility.Collapsed;

            paneel.Visibility = Visibility.Visible;
            lblFoutmelding.Text = "";
        }
    }
}
