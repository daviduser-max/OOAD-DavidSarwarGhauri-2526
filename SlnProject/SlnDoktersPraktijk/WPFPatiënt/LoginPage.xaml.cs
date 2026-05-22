using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using DokterspraktijkLib;

namespace WPFPatiënt
{
    /// <summary>
    /// Logica voor LoginPage van de patiënt.
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            lblFoutmelding.Text = "";

            string email = txtEmail.Text.Trim();
            string wachtwoord = pwdWachtwoord.Password;

            // Formulier controle
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(wachtwoord))
            {
                lblFoutmelding.Text = "Gelieve alle velden in te vullen.";
                return;
            }

            try
            {
                // Valideer inloggegevens via de domain class
                Patient ingelogdePatient = Patient.ValideerInloggen(email, wachtwoord);

                if (ingelogdePatient != null)
                {
                    // Haal MainWindow instantie op om navigatie af te handelen
                    MainWindow main = (MainWindow)Window.GetWindow(this);
                    main.NaInloggen(ingelogdePatient);
                }
                else
                {
                    lblFoutmelding.Text = "Ongeldig e-mailadres of wachtwoord.";
                }
            }
            catch (SqlException ex)
            {
                lblFoutmelding.Text = "Databasefout bij inloggen: " + ex.Message;
            }
            catch (Exception ex)
            {
                lblFoutmelding.Text = "Onverwachte fout: " + ex.Message;
            }
        }
    }
}
