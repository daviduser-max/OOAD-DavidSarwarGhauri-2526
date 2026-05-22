using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using DokterspraktijkLib;

namespace WPFDokter
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            // Pre-filled to simplify testing (as per assignment)
            pwdWachtwoord.Password = "t9ZmRrAbSfCvk";
        }

        private void ZetFout(string bericht)
        {
            lblFoutmelding.Text = bericht;
            foutBorder.Visibility = string.IsNullOrEmpty(bericht)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void BtnInloggen_Click(object sender, RoutedEventArgs e)
        {
            ZetFout("");

            string email    = txtEmail.Text.Trim();
            string wachtwoord = pwdWachtwoord.Password;

            // Basic form validation
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(wachtwoord))
            {
                ZetFout("Gelieve e-mail en wachtwoord in te vullen.");
                return;
            }

            try
            {
                Dokter dokter = Dokter.ValideerInloggen(email, wachtwoord);

                if (dokter != null)
                {
                    MainWindow main = (MainWindow)Window.GetWindow(this);
                    main.NaInloggen(dokter);
                }
                else
                {
                    ZetFout("Ongeldig e-mailadres of wachtwoord.");
                }
            }
            catch (SqlException ex)
            {
                ZetFout("Databasefout: " + ex.Message);
            }
            catch (Exception ex)
            {
                ZetFout("Fout bij inloggen: " + ex.Message);
            }
        }
    }
}
