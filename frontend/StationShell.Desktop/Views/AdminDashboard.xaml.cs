using System.Windows;

namespace StationShell.Desktop.Views
{
    public partial class AdminDashboard : Window
    {
        public AdminDashboard()
        {
            InitializeComponent();
        }
        private void MenuPiste_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var piste = new ShellPisteWindow();
            piste.Show();
            this.Close();
        }
    } 
}