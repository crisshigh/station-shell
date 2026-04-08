using System.Windows;

namespace StationShell.Desktop;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        DispatcherUnhandledException += (s, ex) =>
        {
            MessageBox.Show($"Erreur : {ex.Exception.Message}", "Erreur");
            ex.Handled = true;
        };

        var window = new Views.LoginWindow();
        window.Show();
    }
}