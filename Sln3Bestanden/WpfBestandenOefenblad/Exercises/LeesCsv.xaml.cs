using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using WpfBestandenOefenblad.Helpers;

namespace WpfBestandenOefenblad.Exercises;

[NavPage(Title = "Lees CSV", Description = "CSV inlezen (Product;Quantity;Price), regels tonen en totaal verkoopbedrag", Order = 2, IsVisible = true)]
public partial class LeesCsv : Page
{
    public LeesCsv()
    {
        InitializeComponent();
    }

    private void btnLaad_Click(object sender, RoutedEventArgs e) { 
    
    LstVerkoop.Items.Clear();
        tblTotaal.Text = "";

        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exercises", "Files", "verkoop.csv");

        if (!File.Exists(filePath)) {
            MessageBox.Show($"Bestand niet gevonden: {filePath}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        string[] regels = File.ReadAllLines(filePath);

        double totaal = 0;

        for (int i = 1; i < regels.Length; i++) {
            string[] velden = regels[i].Split(';');

            if (velden.Length < 3) continue;

            string product = velden[0];
            int aantal = int.Parse(velden[1]);
            double prijs = double.Parse(velden[2]);

            LstVerkoop.Items.Add($"{product} x{aantal} aan €{prijs:F2} = €{aantal * prijs:F2} ");

            totaal += aantal * prijs;

           
        
        
        
        }
        tblTotaal.Text = $"Totaal verkoopbedrag: €{totaal:F2}";



    }
}
