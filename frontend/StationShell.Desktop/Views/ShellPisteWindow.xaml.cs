using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StationShell.Desktop.Views
{
    public partial class ShellPisteWindow : Window
    {
        private bool _isLoaded = false;
        private decimal _prixSuper = 920m;
        private decimal _prixGasoil = 680m;
        private decimal[] _indexDepart = { 0, 0, 0, 0, 0, 0 };

        public ShellPisteWindow()
        {
            InitializeComponent();
            _isLoaded = true;
            TxtDateJour.Text = DateTime.Now.ToString("dddd dd MMMM yyyy",
                new System.Globalization.CultureInfo("fr-FR"));
        }

        // ===== NAVIGATION =====
        private void MenuIndexes_Click(object sender, MouseButtonEventArgs e)
        {
            ShowPanel("indexes");
            TxtPageTitre.Text = "Indexes Pompes";
        }

        private void MenuCaisses_Click(object sender, MouseButtonEventArgs e)
        {
            ShowPanel("caisses");
            TxtPageTitre.Text = "Caisse du jour";
        }

        private void MenuCuves_Click(object sender, MouseButtonEventArgs e)
        {
            ShowPanel("cuves");
            TxtPageTitre.Text = "Cuves";
        }

        private void MenuVentes_Click(object sender, MouseButtonEventArgs e)
        {
            ShowPanel("ventes");
            TxtPageTitre.Text = "Ventes Piste";
        }

        private void ShowPanel(string panel)
        {
            PanelIndexes.Visibility = panel == "indexes" ? Visibility.Visible : Visibility.Collapsed;
            PanelCaisses.Visibility = panel == "caisses" ? Visibility.Visible : Visibility.Collapsed;
            PanelCuves.Visibility   = panel == "cuves"   ? Visibility.Visible : Visibility.Collapsed;
            PanelVentes.Visibility  = panel == "ventes"  ? Visibility.Visible : Visibility.Collapsed;

            MenuIndexesBorder.Background = panel == "indexes"
                ? System.Windows.Media.Brushes.Firebrick
                : System.Windows.Media.Brushes.Transparent;
            MenuCaissesBorder.Background = panel == "caisses"
                ? System.Windows.Media.Brushes.Firebrick
                : System.Windows.Media.Brushes.Transparent;
            MenuCuvesBorder.Background = panel == "cuves"
                ? System.Windows.Media.Brushes.Firebrick
                : System.Windows.Media.Brushes.Transparent;
            MenuVentesBorder.Background = panel == "ventes"
                ? System.Windows.Media.Brushes.Firebrick
                : System.Windows.Media.Brushes.Transparent;
        }

        // ===== CALCUL AUTOMATIQUE =====
       private void IndexArrivee_Changed(object sender, TextChangedEventArgs e)
        {
             if (!_isLoaded) return;
        // Pas encore initialisé
            if (TxtArr1 == null || TxtQte1 == null || TxtTotal1 == null ||
                TxtArr2 == null || TxtQte2 == null || TxtTotal2 == null ||
                TxtArr3 == null || TxtQte3 == null || TxtTotal3 == null ||
                TxtArr4 == null || TxtQte4 == null || TxtTotal4 == null ||
                TxtArr5 == null || TxtQte5 == null || TxtTotal5 == null ||
                TxtArr6 == null || TxtQte6 == null || TxtTotal6 == null ||
                TxtTotalQte == null || TxtTotalMontant == null) return;

        CalculerLigne(TxtArr1, TxtQte1, TxtTotal1, TxtPu1, _indexDepart[0]);
        CalculerLigne(TxtArr2, TxtQte2, TxtTotal2, TxtPu2, _indexDepart[1]);
        CalculerLigne(TxtArr3, TxtQte3, TxtTotal3, TxtPu3, _indexDepart[2]);
        CalculerLigne(TxtArr4, TxtQte4, TxtTotal4, TxtPu4, _indexDepart[3]);
        CalculerLigne(TxtArr5, TxtQte5, TxtTotal5, TxtPu5, _indexDepart[4]);
        CalculerLigne(TxtArr6, TxtQte6, TxtTotal6, TxtPu6, _indexDepart[5]);
        CalculerTotaux();
        }   

        private void CalculerLigne(TextBox txtArrivee, TextBox txtQte,
            TextBox txtTotal, TextBox txtPu, decimal indexDepart)
        {
            if (decimal.TryParse(txtArrivee.Text, out decimal arrivee) &&
                decimal.TryParse(txtPu.Text, out decimal pu))
            {
                decimal qte = arrivee - indexDepart;
                if (qte < 0) qte = 0;
                txtQte.Text = qte.ToString("F2");
                txtTotal.Text = (qte * pu).ToString("F0");
            }
            else
            {
                txtQte.Text = "";
                txtTotal.Text = "";
            }
        }

        private void CalculerTotaux()
        {
            decimal totalQte = 0;
            decimal totalMontant = 0;

            foreach (var (qte, total) in new[]
            {
                (TxtQte1, TxtTotal1), (TxtQte2, TxtTotal2),
                (TxtQte3, TxtTotal3), (TxtQte4, TxtTotal4),
                (TxtQte5, TxtTotal5), (TxtQte6, TxtTotal6)
            })
            {
                if (decimal.TryParse(qte.Text, out decimal q)) totalQte += q;
                if (decimal.TryParse(total.Text, out decimal t)) totalMontant += t;
            }

            TxtTotalQte.Text = totalQte.ToString("F2") + " L";
            TxtTotalMontant.Text = totalMontant.ToString("F0") + " FCFA";
        }

        // ===== CHANGEMENT CARBURANT =====
        private void Carburant_Changed(object sender, SelectionChangedEventArgs e)
            {
                if (!_isLoaded) return;
                if (TxtPu1 == null) return; // Pas encore initialisé

                if (sender is ComboBox cb && cb.SelectedItem is ComboBoxItem item)
                    {
                        string carburant = item.Content?.ToString() ?? "";
                        decimal prix = carburant == "Super" ? _prixSuper : _prixGasoil;

                        if (cb.Name == "CbCarb1") TxtPu1.Text = prix.ToString();
                        else if (cb.Name == "CbCarb2") TxtPu2.Text = prix.ToString();
                        else if (cb.Name == "CbCarb3") TxtPu3.Text = prix.ToString();
                        else if (cb.Name == "CbCarb4") TxtPu4.Text = prix.ToString();
                        else if (cb.Name == "CbCarb5") TxtPu5.Text = prix.ToString();
                        else if (cb.Name == "CbCarb6") TxtPu6.Text = prix.ToString();

                        IndexArrivee_Changed(sender, null);
                    }
                }

        // ===== SAUVEGARDER =====
        private void BtnSauvegarder_Click(object sender, MouseButtonEventArgs e)
        {
            // TODO : Sauvegarder en base de données via API Rust
            TxtStatut.Text = "✅ Indexes sauvegardés avec succès !";
            TxtStatut.Foreground = System.Windows.Media.Brushes.LightGreen;
            TxtStatut.Visibility = Visibility.Visible;
        }

        // ===== VALIDER =====
        private void BtnValider_Click(object sender, MouseButtonEventArgs e)
        {
            var result = MessageBox.Show(
                "Voulez-vous valider les indexes ?\nAprès validation, les indexes ne pourront plus être modifiés.",
                "Confirmation de validation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // TODO : Valider en base de données via API Rust
                TxtStatut.Text = "🔒 Indexes validés et figés !";
                TxtStatut.Foreground = System.Windows.Media.Brushes.Orange;
                TxtStatut.Visibility = Visibility.Visible;
            }
        }

        // ===== RETOUR =====
        private void BtnRetour_Click(object sender, MouseButtonEventArgs e)
        {
            var dashboard = new AdminDashboard();
            dashboard.Show();
            this.Close();
        }

        // ===== CAISSE =====

        private decimal _totalCharges = 0;
        private decimal _totalAvances = 0;
        private decimal[] _decharges = { 0, 0, 0 };

        private void BtnDecharge_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border btn && btn.Tag is string pompe)
            {
                string montantStr = Microsoft.VisualBasic.Interaction.InputBox(
                    $"Montant de la décharge pour {pompe} (FCFA) :", "Décharge", "0");

                if (decimal.TryParse(montantStr, out decimal montant) && montant > 0)
                {
                    int idx = pompe == "P1" ? 0 : pompe == "P2" ? 1 : 2;
                    _decharges[idx] += montant;
                    MettreAJourCaisse();
                }
            }
        }

        private void BtnAjouterCharge_Click(object sender, MouseButtonEventArgs e)
        {
            string libelle = TxtChargeLibelle.Text.Trim();
            if (string.IsNullOrEmpty(libelle) ||
                !decimal.TryParse(TxtChargeMontant.Text, out decimal montant) ||
                montant <= 0)
            {
                MessageBox.Show("Veuillez saisir un libellé et un montant valide.",
                    "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Ajouter à la liste visuelle
            var ligne = new Grid { Margin = new Thickness(0, 3, 0, 3) };
            ligne.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            ligne.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });

            var txtLib = new System.Windows.Controls.TextBlock
            {
                Text = libelle, Foreground = System.Windows.Media.Brushes.White, FontSize = 11
            };
            var txtMnt = new System.Windows.Controls.TextBlock
            {
                Text = montant.ToString("F0") + " FCFA",
                Foreground = System.Windows.Media.Brushes.OrangeRed,
                FontSize = 11, HorizontalAlignment = HorizontalAlignment.Right
            };

            Grid.SetColumn(txtLib, 0);
            Grid.SetColumn(txtMnt, 1);
            ligne.Children.Add(txtLib);
            ligne.Children.Add(txtMnt);
            ListeCharges.Children.Add(ligne);

            _totalCharges += montant;
            TxtTotalCharges.Text = _totalCharges.ToString("F0") + " FCFA";
            TxtChargeLibelle.Text = "";
            TxtChargeMontant.Text = "";
            MettreAJourCaisse();
        }

        private void BtnAjouterAvance_Click(object sender, MouseButtonEventArgs e)
        {
            string personnel = TxtAvancePersonnel.Text.Trim();
            if (string.IsNullOrEmpty(personnel) ||
                !decimal.TryParse(TxtAvanceMontant.Text, out decimal montant) ||
                montant <= 0)
            {
                MessageBox.Show("Veuillez saisir un nom et un montant valide.",
                    "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var ligne = new Grid { Margin = new Thickness(0, 3, 0, 3) };
            ligne.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            ligne.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });

            var txtLib = new System.Windows.Controls.TextBlock
            {
                Text = personnel, Foreground = System.Windows.Media.Brushes.White, FontSize = 11
            };
            var txtMnt = new System.Windows.Controls.TextBlock
            {
                Text = montant.ToString("F0") + " FCFA",
                Foreground = System.Windows.Media.Brushes.OrangeRed,
                FontSize = 11, HorizontalAlignment = HorizontalAlignment.Right
            };

            Grid.SetColumn(txtLib, 0);
            Grid.SetColumn(txtMnt, 1);
            ligne.Children.Add(txtLib);
            ligne.Children.Add(txtMnt);
            ListeAvances.Children.Add(ligne);

            _totalAvances += montant;
            TxtTotalAvances.Text = _totalAvances.ToString("F0") + " FCFA";
            TxtAvancePersonnel.Text = "";
            TxtAvanceMontant.Text = "";
            MettreAJourCaisse();
        }

        private void MettreAJourCaisse()
        {
            // Récupérer les totaux des indexes
            decimal totalSuper = 0, totalGasoil = 0, totalBrut = 0;
            decimal totalDecharges = _decharges[0] + _decharges[1] + _decharges[2];

            if (decimal.TryParse(TxtQte1.Text, out decimal q1)) totalSuper += q1;
            if (decimal.TryParse(TxtQte3.Text, out decimal q3)) totalSuper += q3;
            if (decimal.TryParse(TxtQte5.Text, out decimal q5)) totalSuper += q5;
            if (decimal.TryParse(TxtQte2.Text, out decimal q2)) totalGasoil += q2;
            if (decimal.TryParse(TxtQte4.Text, out decimal q4)) totalGasoil += q4;
            if (decimal.TryParse(TxtQte6.Text, out decimal q6)) totalGasoil += q6;

            if (decimal.TryParse(TxtTotal1.Text, out decimal t1)) totalBrut += t1;
            if (decimal.TryParse(TxtTotal2.Text, out decimal t2)) totalBrut += t2;
            if (decimal.TryParse(TxtTotal3.Text, out decimal t3)) totalBrut += t3;
            if (decimal.TryParse(TxtTotal4.Text, out decimal t4)) totalBrut += t4;
            if (decimal.TryParse(TxtTotal5.Text, out decimal t5)) totalBrut += t5;
            if (decimal.TryParse(TxtTotal6.Text, out decimal t6)) totalBrut += t6;

            // Caisses par pompe
            decimal brutP1 = (decimal.TryParse(TxtTotal1.Text, out decimal tp1) ? tp1 : 0)
                        + (decimal.TryParse(TxtTotal2.Text, out decimal tp2) ? tp2 : 0);
            decimal brutP2 = (decimal.TryParse(TxtTotal3.Text, out decimal tp3) ? tp3 : 0)
                        + (decimal.TryParse(TxtTotal4.Text, out decimal tp4) ? tp4 : 0);
            decimal brutP3 = (decimal.TryParse(TxtTotal5.Text, out decimal tp5) ? tp5 : 0)
                        + (decimal.TryParse(TxtTotal6.Text, out decimal tp6) ? tp6 : 0);

            // Mise à jour lignes pompistes
            TxtCaisseP1Super.Text = (decimal.TryParse(TxtQte1.Text, out decimal s1) ? s1 : 0).ToString("F2") + " L";
            TxtCaisseP1Gasoil.Text = (decimal.TryParse(TxtQte2.Text, out decimal g1) ? g1 : 0).ToString("F2") + " L";
            TxtCaisseP1Brut.Text = brutP1.ToString("F0") + " FCFA";
            TxtCaisseP1Decharge.Text = _decharges[0].ToString("F0") + " FCFA";
            TxtCaisseP1Net.Text = (brutP1 - _decharges[0]).ToString("F0") + " FCFA";

            TxtCaisseP2Super.Text = (decimal.TryParse(TxtQte3.Text, out decimal s2) ? s2 : 0).ToString("F2") + " L";
            TxtCaisseP2Gasoil.Text = (decimal.TryParse(TxtQte4.Text, out decimal g2) ? g2 : 0).ToString("F2") + " L";
            TxtCaisseP2Brut.Text = brutP2.ToString("F0") + " FCFA";
            TxtCaisseP2Decharge.Text = _decharges[1].ToString("F0") + " FCFA";
            TxtCaisseP2Net.Text = (brutP2 - _decharges[1]).ToString("F0") + " FCFA";

            TxtCaisseP3Super.Text = (decimal.TryParse(TxtQte5.Text, out decimal s3) ? s3 : 0).ToString("F2") + " L";
            TxtCaisseP3Gasoil.Text = (decimal.TryParse(TxtQte6.Text, out decimal g3) ? g3 : 0).ToString("F2") + " L";
            TxtCaisseP3Brut.Text = brutP3.ToString("F0") + " FCFA";
            TxtCaisseP3Decharge.Text = _decharges[2].ToString("F0") + " FCFA";
            TxtCaisseP3Net.Text = (brutP3 - _decharges[2]).ToString("F0") + " FCFA";

            // Récapitulatif global
            TxtRecapSuper.Text = totalSuper.ToString("F2") + " L";
            TxtRecapSuperMontant.Text = (totalSuper * _prixSuper).ToString("F0") + " FCFA";
            TxtRecapGasoil.Text = totalGasoil.ToString("F2") + " L";
            TxtRecapGasoilMontant.Text = (totalGasoil * _prixGasoil).ToString("F0") + " FCFA";
            TxtRecapBrut.Text = totalBrut.ToString("F0") + " FCFA";
            TxtRecapDecharges.Text = totalDecharges.ToString("F0") + " FCFA";
            TxtRecapCharges.Text = _totalCharges.ToString("F0") + " FCFA";
            TxtRecapAvances.Text = _totalAvances.ToString("F0") + " FCFA";

            decimal caisseNette = totalBrut + totalDecharges - _totalCharges - _totalAvances;
            TxtCaisseNette.Text = caisseNette.ToString("F0") + " FCFA";
        }

        private void BtnCloturerCaisse_Click(object sender, MouseButtonEventArgs e)
        {
            var result = MessageBox.Show(
                "Voulez-vous clôturer la caisse du jour ?\nCette action est irréversible.",
                "Clôture de caisse",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // TODO : Sauvegarder en base via API Rust
                MessageBox.Show("✅ Caisse clôturée avec succès !",
                    "Caisse clôturée", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}