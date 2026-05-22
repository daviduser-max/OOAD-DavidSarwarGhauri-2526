using DokterspraktijkLib;
using System.Windows.Controls;

namespace WPFPatiënt
{
    public partial class DashboardPage : Page
    {
        private Patient patient;

        public DashboardPage(Patient ingelogdePatient)
        {
            InitializeComponent();

            patient = ingelogdePatient;

            txtNaam.Text = patient.VolledigeNaam;
            txtEmail.Text = patient.Email;
            txtGsm.Text = patient.Gsm;
        }
    }
}