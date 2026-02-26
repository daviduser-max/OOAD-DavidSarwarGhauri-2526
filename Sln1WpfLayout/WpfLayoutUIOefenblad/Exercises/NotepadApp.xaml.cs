using System.Windows;
using System.Windows.Controls;
using WpfLayoutUIOefenblad.Helpers;

namespace WpfLayoutUIOefenblad.Exercises;

[NavPage(title: "Notepad App", description: "Menu en Statusbar in een \ngeïntegreerde oefening", order: 11)]
public partial class NotepadApp : Page
{
    public NotepadApp()
    {
        InitializeComponent();
    }

    private void About_Click(object sender, RoutedEventArgs e) { 
    AboutWindow window = new AboutWindow();
        window.ShowDialog();
    }
}
