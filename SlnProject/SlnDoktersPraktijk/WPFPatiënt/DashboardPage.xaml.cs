using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DokterspraktijkLib;

namespace WPFPatiënt
{
    /// <summary>
    /// Logica voor DashboardPage van de patiënt.
    /// </summary>
    public partial class DashboardPage : Page
    {
        private Patient patient;

        public DashboardPage(Patient ingelogdePatient)
        {
            InitializeComponent();
            patient = ingelogdePatient;
            
            LaadPatientGegevens();
            BerekenAfsprakenStatistiek();
        }

        private void LaadPatientGegevens()
        {
            txtWelkomNaam.Text = "Welkom terug, " + patient.Voornaam + "!";
            txtEmail.Text = patient.Email;
            txtGsm.Text = string.IsNullOrEmpty(patient.Gsm) ? "Niet opgegeven" : patient.Gsm;
            txtGeboortedatum.Text = patient.Geboortedatum.ToString("dd-MM-yyyy");

            // Laad de profielfoto
            if (patient.Profielfotodata != null && patient.Profielfotodata.Length > 0)
            {
                try
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.StreamSource = new MemoryStream(patient.Profielfotodata);
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.EndInit();
                    imgWelkomFoto.ImageSource = img;
                }
                catch
                {
                    imgWelkomFoto.ImageSource = null;
                }
            }
            else
            {
                imgWelkomFoto.ImageSource = null;
            }
        }

        private void BerekenAfsprakenStatistiek()
        {
            try
            {
                // Haal alle afspraken van de patiënt op
                List<Afspraak> alleAfspraken = Afspraak.GetByPatient(patient.Id);
                int toekomstigeCount = 0;
                DateTime nu = DateTime.Now;

                // Tel de toekomstige afspraken (GEEN LINQ!)
                foreach (Afspraak afspraak in alleAfspraken)
                {
                    if (afspraak.Moment >= nu)
                    {
                        toekomstigeCount++;
                    }
                }

                txtAantalAfspraken.Text = toekomstigeCount.ToString();
            }
            catch (Exception ex)
            {
                // Toon stilzwijgend een fout of laat op 0 staan bij DB-fout
                txtAantalAfspraken.Text = "?";
            }
        }

        private void BtnBekijkAfspraken_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = (MainWindow)Window.GetWindow(this);
            if (main != null)
            {
                main.lblPaginaTitel.Text = "Mijn Afspraken";
                main.mainFrame.Navigate(new AfsprakenPage(patient));
            }
        }

        private void BtnBekijkProfiel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = (MainWindow)Window.GetWindow(this);
            if (main != null)
            {
                main.lblPaginaTitel.Text = "Mijn Profiel";
                main.mainFrame.Navigate(new ProfielPage(patient));
            }
        }
    }
}