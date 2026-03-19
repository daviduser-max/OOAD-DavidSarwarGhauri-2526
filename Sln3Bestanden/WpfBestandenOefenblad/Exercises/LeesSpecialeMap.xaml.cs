using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WpfBestandenOefenblad.Helpers;

namespace WpfBestandenOefenblad.Exercises;

[NavPage(Title = "Lees speciale map", Description = "Drie knoppen: Desktop, Documenten, Afbeeldingen; bestanden met grootte en aanmaakdatum in TextBlock", Order = 6, IsVisible = true)]
public partial class LeesSpecialeMap : Page
{
    public LeesSpecialeMap()
    {
        InitializeComponent();
    }

    private void btnMap_Click(object sender, RoutedEventArgs e) {

        Button btn = sender as Button;
        string tag = btn.Tag.ToString();

        Environment.SpecialFolder folder;

        if (tag == "Desktop")
        {

            folder = Environment.SpecialFolder.Desktop;

        }

        else if (tag == "Documenten") {

            folder = Environment.SpecialFolder.MyDocuments;
        }

        else
        {
            folder =Environment.SpecialFolder.MyPictures;
        }

        string path = Environment.GetFolderPath(folder);
        string[] bestanden = Directory.GetFiles(path);

        txtBestanden.Text = "";
        foreach (string bestand in bestanden)
        {
            FileInfo info = new FileInfo(bestand);
            txtBestanden.Text += $"{info.Name} ({info.Length}bytes)\n";
        }



    }
}
