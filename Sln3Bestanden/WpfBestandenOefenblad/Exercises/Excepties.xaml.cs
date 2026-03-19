using System;
using System.IO;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using WpfBestandenOefenblad.Helpers;

namespace WpfBestandenOefenblad.Exercises;

[NavPage(Title = "Excepties", Description = "Kies een speciale map, zie bestanden in een ListBox en info in een TextBlock", Order = 5, IsVisible = true)]
public partial class Excepties : Page
{
    public Excepties()
    {
        InitializeComponent();
        cmbFolders.SelectedIndex = 0;
    }

    private void CmbFolders_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // clear all
        lstBestanden.Items.Clear();
        txtBestandInfo.Text = "";

        // grab selected item and path
        ComboBoxItem item = cmbFolders.SelectedItem as ComboBoxItem;
        if (item == null || item.Tag == null) return;
        string tag = item.Tag as string;
        if (!Enum.TryParse<Environment.SpecialFolder>(tag, out Environment.SpecialFolder folder)) return;
        string path = Environment.GetFolderPath(folder);

        // read files in folder
        string[] bestanden;

        try
        {
            bestanden = Directory.GetFiles(path);
        }

        catch (UnauthorizedAccessException ex)
        {
            txtBestandInfo.Text = $"Geen toegang tot de map{ex.Message}";
            return;
        }

        catch (DirectoryNotFoundException ex)
        {
            txtBestandInfo.Text = $"Map niet gevonden:{ex.Message}";
            return;
        }

        catch(IOException ex)
        {
            txtBestandInfo.Text = $"Fout bij het lezen van de map:{ex.Message}";
            return;
        }

        foreach (string bestand in bestanden)
        {
            string naam = Path.GetFileName(bestand);
            lstBestanden.Items.Add(new ListBoxItem { Content = naam, Tag = bestand });
        }
    }

    private void LstBestanden_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // clear info
        txtBestandInfo.Text = "";

        // grab selected item and path
        ListBoxItem? selectedItem = lstBestanden.SelectedItem as ListBoxItem;
        if (selectedItem == null) return;

        // read file info
        string path = selectedItem.Tag.ToString();

        try
        {
            FileInfo info = new(path);
            txtBestandInfo.Text = $@"Bestand: {info.Name}
Extensie: {info.Extension}
Grootte (bytes): {info.Length}
Aangemaakt: {info.CreationTime:dd/MM/yyyy HH:mm:ss}
Gewijzigd: {info.LastWriteTime:dd/MM/yyyy HH:mm:ss}";

        }

        catch (UnauthorizedAccessException ex)
        {
            txtBestandInfo.Text = $"Geen toegang tot het bestand: {ex.Message}";
        }

        catch (FileNotFoundException ex)
        {
            txtBestandInfo.Text = $"Bestand niet gevonden: {ex.Message}";
        }

        catch(IOException ex)
        {
            txtBestandInfo.Text = $"Fout bij het lezen van het bestand:{ex.Message}";
        }
    }

    
}
