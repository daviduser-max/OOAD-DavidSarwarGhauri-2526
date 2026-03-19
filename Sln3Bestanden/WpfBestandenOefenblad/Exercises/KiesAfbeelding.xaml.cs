using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using WpfBestandenOefenblad.Helpers;

namespace WpfBestandenOefenblad.Exercises;

[NavPage(Title = "Kies afbeelding", Description = "OpenFileDialog om een jpg/jpeg te kiezen en in een Image te tonen", Order = 3, IsVisible = true)]
public partial class KiesAfbeelding : Page
{
    public KiesAfbeelding()
    {
        InitializeComponent();
    }

    private void btnKies_Click(object sender, RoutedEventArgs e) { 
    
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        dialog.Filter = "Afbeelding|*.jpg;*.jpeg";

        if (dialog.ShowDialog() == true)
        {

            tblBestandnaam.Text = Path.GetFileName(dialog.FileName);


            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(dialog.FileName, UriKind.Absolute);
            bitmap.EndInit();
            imgAfbeelding.Source = bitmap;

        }

        else {
            tblBestandnaam.Text = "iezen afbeelding geanuleerd";
            imgAfbeelding.Source = null;
        }
    
    }
}
