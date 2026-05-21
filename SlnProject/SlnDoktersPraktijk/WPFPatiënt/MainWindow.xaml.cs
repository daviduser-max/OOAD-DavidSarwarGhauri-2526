using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DokterspraktijkLib;

namespace WPFPatiënt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string wachtwoord = pwdWachtwoord.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(wachtwoord))
            {
                lblFoutmelding.Text = "Gelieve alle velden in te vullen.";
                return;
            }

            Patient ingelogdePatient = PatientService.ValideerInloggen(email, wachtwoord);
            if (ingelogdePatient != null)
            {
                lblFoutmelding.Text = "";
                MessageBox.Show($"Welkom terug, {ingelogdePatient.Voornaam} {ingelogdePatient.Achternaam}!", "Inloggen Geslgaagd", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                lblFoutmelding.Text = "Ongelidge e-mailadres of wachtwoord";
            }

        }
     }

    }