using System.Windows;
using CLDierenarts;

namespace WpfDierenarts;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DatabaseInitializer.InitializeDatabase();
    }
}

