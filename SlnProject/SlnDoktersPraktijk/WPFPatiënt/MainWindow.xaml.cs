using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using DokterspraktijkLib;

namespace WPFPatiënt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Patient IngelogdePatient { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ZetVoorUitgelogd();
        }

        /// <summary>
        /// Deze methode wordt aangeroepen na een succesvolle login.
        /// Het bouwt het portaal op voor de patiënt.
        /// </summary>
        public void NaInloggen(Patient patient)
        {
            IngelogdePatient = patient;
            ZetVoorIngelogd();
            UpdateProfielHeader();
            
            // Navigeer standaard naar het dashboard
            lblPaginaTitel.Text = "Mijn Dashboard";
            mainFrame.Navigate(new DashboardPage(IngelogdePatient));
        }

        private void ZetVoorIngelogd()
        {
            pnlSidebar.Visibility = Visibility.Visible;
            pnlHeader.Visibility = Visibility.Visible;
            colSidebar.Width = new GridLength(220);
        }

        private void ZetVoorUitgelogd()
        {
            IngelogdePatient = null;
            pnlSidebar.Visibility = Visibility.Collapsed;
            pnlHeader.Visibility = Visibility.Collapsed;
            colSidebar.Width = new GridLength(0);
            imgBrushProfielfoto.ImageSource = null;
            txtPatientNaam.Text = "";
            
            // Navigeer terug naar login
            mainFrame.Navigate(new LoginPage());
        }

        /// <summary>
        /// Update de profielfoto en naam in de header.
        /// </summary>
        public void UpdateProfielHeader()
        {
            if (IngelogdePatient == null) return;

            txtPatientNaam.Text = IngelogdePatient.VolledigeNaam;

            if (IngelogdePatient.Profielfotodata != null && IngelogdePatient.Profielfotodata.Length > 0)
            {
                try
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.StreamSource = new MemoryStream(IngelogdePatient.Profielfotodata);
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.EndInit();
                    imgBrushProfielfoto.ImageSource = img;
                }
                catch
                {
                    imgBrushProfielfoto.ImageSource = null;
                }
            }
            else
            {
                imgBrushProfielfoto.ImageSource = null;
            }
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            if (IngelogdePatient != null)
            {
                lblPaginaTitel.Text = "Mijn Dashboard";
                mainFrame.Navigate(new DashboardPage(IngelogdePatient));
            }
        }

        private void BtnAfspraken_Click(object sender, RoutedEventArgs e)
        {
            if (IngelogdePatient != null)
            {
                lblPaginaTitel.Text = "Mijn Afspraken";
                mainFrame.Navigate(new AfsprakenPage(IngelogdePatient));
            }
        }

        private void BtnProfiel_Click(object sender, RoutedEventArgs e)
        {
            if (IngelogdePatient != null)
            {
                lblPaginaTitel.Text = "Mijn Profiel";
                mainFrame.Navigate(new ProfielPage(IngelogdePatient));
            }
        }

        private void BtnUitloggen_Click(object sender, RoutedEventArgs e)
        {
            ZetVoorUitgelogd();
        }
    }
}