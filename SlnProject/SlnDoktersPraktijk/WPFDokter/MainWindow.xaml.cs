using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DokterspraktijkLib;

namespace WPFDokter
{
    public partial class MainWindow : Window
    {
        // Ingelogde dokter (null = niet ingelogd)
        public Dokter IngelogdeDokter { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            mainFrame.Navigate(new StartPage());
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new StartPage());
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new LoginPage());
        }

        private void BtnAfspraken_Click(object sender, RoutedEventArgs e)
        {
            if (IngelogdeDokter != null)
            {
                mainFrame.Navigate(new AfsprakenPage(IngelogdeDokter));
            }
        }

        private void BtnPatienten_Click(object sender, RoutedEventArgs e)
        {
            if (IngelogdeDokter != null)
            {
                mainFrame.Navigate(new PatientenPage());
            }
        }

        private void BtnUitloggen_Click(object sender, RoutedEventArgs e)
        {
            IngelogdeDokter = null;
            ZetMenuVoorUitgelogd();
            mainFrame.Navigate(new StartPage());
        }

        /// <summary>
        /// Na succesvol inloggen: menu en profiel bijwerken, patiëntenpagina tonen.
        /// </summary>
        public void NaInloggen(Dokter dokter)
        {
            IngelogdeDokter = dokter;
            ZetMenuVoorIngelogd();
            ToonProfiel(dokter);
            mainFrame.Navigate(new PatientenPage());
        }

        private void ZetMenuVoorIngelogd()
        {
            btnStart.Visibility = Visibility.Collapsed;
            btnLogin.Visibility = Visibility.Collapsed;
            btnAfspraken.Visibility = Visibility.Visible;
            btnPatienten.Visibility = Visibility.Visible;
            btnUitloggen.Visibility = Visibility.Visible;
            pnlHeader.Visibility = Visibility.Visible;
        }

        private void ZetMenuVoorUitgelogd()
        {
            btnStart.Visibility = Visibility.Visible;
            btnLogin.Visibility = Visibility.Visible;
            btnAfspraken.Visibility = Visibility.Collapsed;
            btnPatienten.Visibility = Visibility.Collapsed;
            btnUitloggen.Visibility = Visibility.Collapsed;
            pnlHeader.Visibility = Visibility.Collapsed;
            imgProfielfoto.Source = null;
            txtDokterNaam.Text = "";
        }

        private void ToonProfiel(Dokter dokter)
        {
            txtDokterNaam.Text = dokter.VolledigeNaam;

            if (dokter.Profielfotodata != null)
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = new MemoryStream(dokter.Profielfotodata);
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();
                imgProfielfoto.Source = img;
            }
            else
            {
                imgProfielfoto.Source = null;
            }
        }
    }
}
