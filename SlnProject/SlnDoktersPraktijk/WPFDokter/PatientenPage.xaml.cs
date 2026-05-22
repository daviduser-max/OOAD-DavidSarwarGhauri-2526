using System;
using System.Data.SqlClient;
using System.Windows.Controls;
using DokterspraktijkLib;

namespace WPFDokter
{
    public partial class PatientenPage : Page
    {
        public PatientenPage()
        {
            InitializeComponent();
            LaadAantalPatienten();
        }

        private void LaadAantalPatienten()
        {
            try
            {
                int aantal = Patient.GetPatients().Count;
                lblFoutmelding.Text = "Verbinding OK. Aantal patiënten in databank: " + aantal + ".";
                lblFoutmelding.Foreground = System.Windows.Media.Brushes.Green;
            }
            catch (SqlException ex)
            {
                lblFoutmelding.Text = "Databasefout: " + ex.Message;
            }
            catch (Exception ex)
            {
                lblFoutmelding.Text = "Fout: " + ex.Message;
            }
        }
    }
}
