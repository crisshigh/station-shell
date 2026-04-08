using System.Windows;

namespace StationShell.Desktop.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

       private void BtnConnexion_Click(object sender, RoutedEventArgs e)
        {
            string email = TxtEmail.Text.Trim();
            string motDePasse = TxtPassword.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(motDePasse))
                {
                    TxtErreur.Text = "Veuillez remplir tous les champs.";
                    TxtErreur.Visibility = Visibility.Visible;
            return;
                }

            TxtErreur.Visibility = Visibility.Collapsed;

            if (email == "admin@shell.com" && motDePasse == "admin123")
                 {
                    var dashboard = new AdminDashboard();
                    dashboard.Show();
            this.Close();
                }
            else
                {
                    TxtErreur.Text = "Email ou mot de passe incorrect.";
                    TxtErreur.Visibility = Visibility.Visible;
                }
        } 
    }
}