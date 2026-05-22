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
            // Vooraf ingevuld om testen te vergemakkelijken (opdracht)
            pwdWachtwoord.Password = "t9ZmRrAbSfCvk";
        }

        private void BtnInloggen_Click(object sender, RoutedEventArgs e)
        {
            lblFoutmelding.Text = "";

            string email = txtEmail.Text.Trim();
            string wachtwoord = pwdWachtwoord.Password;

            // Formchecking
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(wachtwoord))
            {
                lblFoutmelding.Text = "Gelieve e-mail en wachtwoord in te vullen.";
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
                    lblFoutmelding.Text = "Ongeldig e-mailadres of wachtwoord.";
                }
            }
            catch (SqlException ex)
            {
                lblFoutmelding.Text = "Databasefout: " + ex.Message;
            }
            catch (Exception ex)
            {
                lblFoutmelding.Text = "Fout bij inloggen: " + ex.Message;
            }
        }
    }
}
