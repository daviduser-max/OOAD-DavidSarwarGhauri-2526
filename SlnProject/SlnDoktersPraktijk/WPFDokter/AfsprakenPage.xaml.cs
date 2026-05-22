using System.Windows.Controls;
using DokterspraktijkLib;

namespace WPFDokter
{
    public partial class AfsprakenPage : Page
    {
        private Dokter ingelogdeDokter;

        public AfsprakenPage(Dokter dokter)
        {
            InitializeComponent();
            ingelogdeDokter = dokter;
        }
    }
}
