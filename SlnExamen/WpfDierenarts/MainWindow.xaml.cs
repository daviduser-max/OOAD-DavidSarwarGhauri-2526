using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CLDierenarts;

namespace WpfDierenarts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dier? selectedDier = null;
        private bool isInitializing = true;

        public MainWindow()
        {
            InitializeComponent();
            LoadFilterData();
            LoadRegistrationFormDefaults();
            isInitializing = false;
            LoadAnimals();
        }

        private void LoadFilterData()
        {
            // Owner Filter
            cmbFilterOwner.Items.Clear();
            cmbFilterOwner.Items.Add(new ComboBoxItem { Content = "(Alle)", Tag = null });
            List<Eigenaar> owners = Eigenaar.GetAll();
            foreach (Eigenaar owner in owners)
            {
                cmbFilterOwner.Items.Add(new ComboBoxItem { Content = owner.VolledigeNaam, Tag = owner.Id });
            }
            cmbFilterOwner.SelectedIndex = 0;

            // Urgency Filter
            cmbFilterUrgency.Items.Clear();
            cmbFilterUrgency.Items.Add(new ComboBoxItem { Content = "(Alle)", Tag = null });
            foreach (Urgentie urgency in Enum.GetValues(typeof(Urgentie)))
            {
                cmbFilterUrgency.Items.Add(new ComboBoxItem { Content = urgency.ToString(), Tag = urgency });
            }
            cmbFilterUrgency.SelectedIndex = 0;
        }

        private void LoadRegistrationFormDefaults()
        {
            // Owners ComboBox
            cmbRegOwner.Items.Clear();
            List<Eigenaar> owners = Eigenaar.GetAll();
            foreach (Eigenaar owner in owners)
            {
                cmbRegOwner.Items.Add(new ComboBoxItem { Content = owner.VolledigeNaam, Tag = owner.Id });
            }
            if (cmbRegOwner.Items.Count > 0)
            {
                cmbRegOwner.SelectedIndex = 0;
            }

            // Urgency ComboBox
            cmbRegUrgency.Items.Clear();
            foreach (Urgentie urgency in Enum.GetValues(typeof(Urgentie)))
            {
                cmbRegUrgency.Items.Add(new ComboBoxItem { Content = urgency.ToString(), Tag = urgency });
            }
            // Select "Normaal" as default if it exists
            cmbRegUrgency.SelectedIndex = 1; 

            // Type ComboBox
            cmbRegType.Items.Clear();
            cmbRegType.Items.Add(new ComboBoxItem { Content = "Hond" });
            cmbRegType.Items.Add(new ComboBoxItem { Content = "Kat" });
            cmbRegType.SelectedIndex = 0;

            // Date picker default value
            dpRegBirthdate.SelectedDate = DateTime.Today.AddYears(-2);
        }

        private void LoadAnimals(int? selectAnimalId = null)
        {
            if (isInitializing) return;

            // Clear overview list box
            lbxAnimals.Items.Clear();

            // Clear active selection in details if not refreshing same animal
            if (selectAnimalId == null)
            {
                ClearDetails();
            }

            // Retrieve filters
            string? filterOwnerId = null;
            if (cmbFilterOwner.SelectedItem is ComboBoxItem ownerItem && ownerItem.Tag != null)
            {
                filterOwnerId = ownerItem.Tag.ToString();
            }

            Urgentie? filterUrgency = null;
            if (cmbFilterUrgency.SelectedItem is ComboBoxItem urgencyItem && urgencyItem.Tag != null)
            {
                filterUrgency = (Urgentie)urgencyItem.Tag;
            }

            bool filterAdmitted = chkFilterAdmitted.IsChecked == true;

            // Fetch and apply filtering
            List<Dier> animals = Dier.GetAll();
            foreach (Dier animal in animals)
            {
                // Apply filters in code-behind (as per course practices)
                if (filterOwnerId != null && animal.EigenaarId != filterOwnerId) continue;
                if (filterUrgency != null && animal.Urgentie != filterUrgency.Value) continue;
                if (filterAdmitted && !animal.IsOpgenomen) continue;

                // Add to list box
                ListBoxItem item = new ListBoxItem
                {
                    Content = animal.ToString(),
                    Tag = animal.Id
                };

                lbxAnimals.Items.Add(item);

                // Select this item if requested
                if (selectAnimalId.HasValue && animal.Id == selectAnimalId.Value)
                {
                    item.IsSelected = true;
                }
            }
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            LoadAnimals();
        }

        private void LbxAnimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxAnimals.SelectedItem is ListBoxItem selectedItem)
            {
                int animalId = Convert.ToInt32(selectedItem.Tag);
                selectedDier = Dier.GetById(animalId);
                DisplayDetails();
            }
            else
            {
                ClearDetails();
            }
        }

        private void DisplayDetails()
        {
            if (selectedDier == null)
            {
                ClearDetails();
                return;
            }

            txtNoSelection.Visibility = Visibility.Collapsed;
            gridDetails.Visibility = Visibility.Visible;

            // Set Text fields
            txtDetailId.Text = selectedDier.Id.ToString();
            txtDetailName.Text = selectedDier.Naam;
            txtDetailType.Text = selectedDier is Kat ? "Kat" : "Hond";
            txtDetailUrgency.Text = selectedDier.Urgentie.ToString();
            txtDetailBirthdate.Text = selectedDier.Geboortedatum.ToString("dd-MM-yyyy");
            txtDetailWeight.Text = $"{selectedDier.Gewicht:F1} kg";
            txtDetailOwner.Text = selectedDier.Eigenaar != null 
                ? $"{selectedDier.Eigenaar.VolledigeNaam} ({selectedDier.EigenaarId})" 
                : $"Unknown ({selectedDier.EigenaarId})";

            if (selectedDier.IsOpgenomen)
            {
                txtDetailAdmitted.Text = $"Ja (sinds {selectedDier.DatumOpgenomen?.ToString("dd-MM-yyyy HH:mm")})";
                btnAdmit.IsEnabled = false;
            }
            else
            {
                txtDetailAdmitted.Text = "Nee";
                btnAdmit.IsEnabled = true;
            }

            // Set Dynamic elements
            if (selectedDier is Kat kat)
            {
                spDetailBreed.Visibility = Visibility.Collapsed;
                spDetailVaccinated.Visibility = Visibility.Visible;
                txtDetailVaccinated.Text = kat.IsGevaccineerd ? "Ja" : "Nee";
                
                try
                {
                    imgAvatar.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/cat_avatar.png"));
                }
                catch (Exception)
                {
                    imgAvatar.Source = null;
                }
            }
            else if (selectedDier is Hond hond)
            {
                spDetailVaccinated.Visibility = Visibility.Collapsed;
                spDetailBreed.Visibility = Visibility.Visible;
                txtDetailBreed.Text = hond.Ras ?? "-";
                
                try
                {
                    imgAvatar.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/dog_avatar.png"));
                }
                catch (Exception)
                {
                    imgAvatar.Source = null;
                }
            }
        }

        private void ClearDetails()
        {
            selectedDier = null;
            gridDetails.Visibility = Visibility.Collapsed;
            txtNoSelection.Visibility = Visibility.Visible;
            btnAdmit.IsEnabled = false;

            txtDetailId.Text = "";
            txtDetailName.Text = "";
            txtDetailType.Text = "";
            txtDetailUrgency.Text = "";
            txtDetailBirthdate.Text = "";
            txtDetailWeight.Text = "";
            txtDetailOwner.Text = "";
            txtDetailAdmitted.Text = "";
            txtDetailBreed.Text = "";
            txtDetailVaccinated.Text = "";
            imgAvatar.Source = null;
        }

        private void BtnAdmit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDier != null && !selectedDier.IsOpgenomen)
            {
                selectedDier.Opnemen();
                
                // Keep selected animal highlighted and refresh its data
                int currentId = selectedDier.Id;
                LoadAnimals(currentId);
                
                MessageBox.Show($"{selectedDier.Naam} is succesvol opgenomen in de kliniek.", "Opname Succesvol", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CmbRegType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRegType.SelectedItem is ComboBoxItem selectedItem)
            {
                string type = selectedItem.Content.ToString() ?? "";
                if (type == "Hond")
                {
                    spRegBreedField.Visibility = Visibility.Visible;
                    spRegVaccinatedField.Visibility = Visibility.Collapsed;
                }
                else if (type == "Kat")
                {
                    spRegBreedField.Visibility = Visibility.Collapsed;
                    spRegVaccinatedField.Visibility = Visibility.Visible;
                }
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            txtRegError.Text = "";

            // Gather inputs
            string name = txtRegName.Text.Trim();
            
            string? ownerId = null;
            if (cmbRegOwner.SelectedItem is ComboBoxItem ownerItem)
            {
                ownerId = ownerItem.Tag?.ToString();
            }

            DateTime? birthdate = dpRegBirthdate.SelectedDate;
            string weightInput = txtRegWeight.Text.Trim();
            
            Urgentie? urgency = null;
            if (cmbRegUrgency.SelectedItem is ComboBoxItem urgencyItem)
            {
                urgency = (Urgentie)urgencyItem.Tag;
            }

            string type = "";
            if (cmbRegType.SelectedItem is ComboBoxItem typeItem)
            {
                type = typeItem.Content.ToString() ?? "";
            }

            // Input validation
            DierValidator validator = new DierValidator { MinRasLengte = 3 };

            if (!validator.IsGeldigNaam(name))
            {
                txtRegError.Text = "Ongeldige naam: Naam mag enkel letters, spaties en koppeltekens bevatten.";
                return;
            }

            if (string.IsNullOrEmpty(ownerId))
            {
                txtRegError.Text = "Selecteer een eigenaar.";
                return;
            }

            if (!birthdate.HasValue)
            {
                txtRegError.Text = "Geboortedatum is verplicht.";
                return;
            }

            if (birthdate.Value > DateTime.Today)
            {
                txtRegError.Text = "Geboortedatum mag niet in de toekomst liggen.";
                return;
            }

            if (!double.TryParse(weightInput, out double weight) || weight <= 0)
            {
                txtRegError.Text = "Voer een geldig positief gewicht in kg in (bijv. 4.5).";
                return;
            }

            if (!urgency.HasValue)
            {
                txtRegError.Text = "Selecteer de urgentie.";
                return;
            }

            Dier newDier;

            if (type == "Hond")
            {
                string breed = txtRegBreed.Text.Trim();
                if (!validator.IsGeldigRas(breed))
                {
                    txtRegError.Text = "Ongeldig ras: Ras moet minstens 3 tekens lang zijn.";
                    return;
                }

                newDier = new Hond
                {
                    Naam = name,
                    EigenaarId = ownerId,
                    Geboortedatum = birthdate.Value,
                    Gewicht = weight,
                    Urgentie = urgency.Value,
                    IsOpgenomen = false,
                    DatumOpgenomen = null,
                    Ras = breed
                };
            }
            else if (type == "Kat")
            {
                bool isVaccinated = chkRegVaccinated.IsChecked == true;

                newDier = new Kat
                {
                    Naam = name,
                    EigenaarId = ownerId,
                    Geboortedatum = birthdate.Value,
                    Gewicht = weight,
                    Urgentie = urgency.Value,
                    IsOpgenomen = false,
                    DatumOpgenomen = null,
                    IsGevaccineerd = isVaccinated
                };
            }
            else
            {
                txtRegError.Text = "Selecteer een diertype.";
                return;
            }

            // Save to database
            try
            {
                int newId = newDier.InsertToDb();
                
                // Reset form fields
                txtRegName.Text = "";
                txtRegWeight.Text = "";
                txtRegBreed.Text = "";
                chkRegVaccinated.IsChecked = false;
                dpRegBirthdate.SelectedDate = DateTime.Today.AddYears(-2);
                cmbRegOwner.SelectedIndex = 0;
                cmbRegUrgency.SelectedIndex = 1;
                cmbRegType.SelectedIndex = 0;

                // Reload overview and select the newly added animal
                LoadAnimals(newId);

                MessageBox.Show($"{newDier.Naam} is succesvol geregistreerd met ID: {newId}.", "Registratie voltooid", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                txtRegError.Text = $"Databasefout: {ex.Message}";
            }
        }
    }
}