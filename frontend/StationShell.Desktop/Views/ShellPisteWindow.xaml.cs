using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace StationShell.Desktop.Views
{
    public partial class ShellPisteWindow : Window
    {
        private bool _isLoaded = false;
        private decimal _prixSuper = 920m;
        private decimal _prixGasoil = 680m;
        private decimal[] _indexDepart = { 0, 0, 0, 0, 0, 0 };

        // Caisse panel
        private decimal _totalCharges = 0;
        private decimal _totalAvances = 0;
        private decimal[] _decharges = { 0, 0, 0 };

        // ===== FORMATAGE =====
        private static string Fmt(decimal v) =>
            v.ToString("N0", CultureInfo.CurrentCulture) + " FCFA";

        private static string FmtL(decimal v) =>
            v.ToString("N2", CultureInfo.CurrentCulture) + " L";

        private static bool ParseDec(string? text, out decimal value) =>
            decimal.TryParse(
                text ?? "",
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out value);

        public ShellPisteWindow()
        {
            InitializeComponent();
            TxtDateJour.Text = DateTime.Now.ToString("dddd dd MMMM yyyy",
                new System.Globalization.CultureInfo("fr-FR"));
            DpDebut.SelectedDate = DateTime.Today.AddDays(-1);
            DpFin.SelectedDate = DateTime.Today;
            TxtHeureDebut.Text = "08:00";
            TxtHeureFin.Text = "08:00";
            _isLoaded = true;
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
                ? Brushes.Firebrick : Brushes.Transparent;
            MenuCaissesBorder.Background = panel == "caisses"
                ? Brushes.Firebrick : Brushes.Transparent;
            MenuCuvesBorder.Background = panel == "cuves"
                ? Brushes.Firebrick : Brushes.Transparent;
            MenuVentesBorder.Background = panel == "ventes"
                ? Brushes.Firebrick : Brushes.Transparent;
        }

        // ===== AFFECTATION AGENTS → ÎLOTS =====
        private void IlotAgent_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded) return;
            MettreAJourPompistes();
            GenererResumeAgents();
        }

        private string GetAgentNom(ComboBox cb) =>
            cb.SelectedItem is ComboBoxItem item && item.Content?.ToString() != "-- Aucun --"
                ? item.Content?.ToString() ?? "" : "";

        private string GetIlotNom(ComboBox cb) =>
            cb.SelectedItem is ComboBoxItem item && item.Content?.ToString() != "-- Aucun --"
                ? item.Content?.ToString() ?? "" : "";

        private string GetAgentPourIlot(string ilotNom)
        {
            if (!string.IsNullOrEmpty(ilotNom))
            {
                if (GetIlotNom(CbIlotAgent1) == ilotNom) return GetAgentNom(CbAgent1);
                if (GetIlotNom(CbIlotAgent2) == ilotNom) return GetAgentNom(CbAgent2);
                if (GetIlotNom(CbIlotAgent3) == ilotNom) return GetAgentNom(CbAgent3);
            }
            return "";
        }

        private void MettreAJourPompistes()
        {
            TxtPompiste1.Text = GetAgentPourIlot(GetIlotNom(CbIlot1));
            TxtPompiste2.Text = GetAgentPourIlot(GetIlotNom(CbIlot2));
            TxtPompiste3.Text = GetAgentPourIlot(GetIlotNom(CbIlot3));
            TxtPompiste4.Text = GetAgentPourIlot(GetIlotNom(CbIlot4));
            TxtPompiste5.Text = GetAgentPourIlot(GetIlotNom(CbIlot5));
            TxtPompiste6.Text = GetAgentPourIlot(GetIlotNom(CbIlot6));
        }

        // ===== CALCUL AUTOMATIQUE =====
        private void IndexArrivee_Changed(object sender, TextChangedEventArgs? e)
        {
            if (!_isLoaded) return;
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
            GenererResumeAgents();
        }

        private void CalculerLigne(TextBox txtArrivee, TextBox txtQte,
            TextBox txtTotal, TextBox txtPu, decimal indexDepart)
        {
            if (ParseDec(txtArrivee.Text, out decimal arrivee) &&
                ParseDec(txtPu.Text, out decimal pu))
            {
                decimal qte = arrivee - indexDepart;
                if (qte < 0) qte = 0;
                txtQte.Text = qte.ToString("N2", CultureInfo.CurrentCulture);
                txtTotal.Text = (qte * pu).ToString("N0", CultureInfo.CurrentCulture);
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
                if (ParseDec(qte.Text, out decimal q)) totalQte += q;
                if (ParseDec(total.Text, out decimal t)) totalMontant += t;
            }

            TxtTotalQte.Text = FmtL(totalQte);
            TxtTotalMontant.Text = Fmt(totalMontant);
        }

        // ===== LIAISON ÎLOT → POMPE + POMPISTE =====
        private void Ilot_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded) return;
            if (sender is not ComboBox cbIlot) return;

            int idx = cbIlot.SelectedIndex;
            if (idx < 0) return;

            ComboBox? cbPompe = cbIlot.Name switch
            {
                "CbIlot1" => CbPompe1,
                "CbIlot2" => CbPompe2,
                "CbIlot3" => CbPompe3,
                "CbIlot4" => CbPompe4,
                "CbIlot5" => CbPompe5,
                "CbIlot6" => CbPompe6,
                _ => null
            };

            if (cbPompe != null && idx < cbPompe.Items.Count)
                cbPompe.SelectedIndex = idx;

            MettreAJourPompistes();
            GenererResumeAgents();
        }

        // ===== CHANGEMENT CARBURANT =====
        private void Carburant_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded) return;
            if (TxtPu1 == null) return;

            if (sender is ComboBox cb && cb.SelectedItem is ComboBoxItem item)
            {
                string carburant = item.Content?.ToString() ?? "";
                decimal prix = carburant == "Super" ? _prixSuper : _prixGasoil;

                if      (cb.Name == "CbCarb1") TxtPu1.Text = prix.ToString();
                else if (cb.Name == "CbCarb2") TxtPu2.Text = prix.ToString();
                else if (cb.Name == "CbCarb3") TxtPu3.Text = prix.ToString();
                else if (cb.Name == "CbCarb4") TxtPu4.Text = prix.ToString();
                else if (cb.Name == "CbCarb5") TxtPu5.Text = prix.ToString();
                else if (cb.Name == "CbCarb6") TxtPu6.Text = prix.ToString();

                IndexArrivee_Changed(sender, null);
            }
        }

        // ===== RÉSUMÉ INDEXES POMPES =====
        private void GenererResumeAgents()
        {
            if (!_isLoaded || PanelResumeAgents == null) return;
            PanelResumeAgents.Children.Clear();

            var agentSlots = new[]
            {
                (cbNom: CbAgent1, cbIlot: CbIlotAgent1),
                (cbNom: CbAgent2, cbIlot: CbIlotAgent2),
                (cbNom: CbAgent3, cbIlot: CbIlotAgent3),
            };

            var lignes = new[]
            {
                (cbIlot: CbIlot1, cbCarb: CbCarb1, txtQte: TxtQte1, txtTotal: TxtTotal1),
                (cbIlot: CbIlot2, cbCarb: CbCarb2, txtQte: TxtQte2, txtTotal: TxtTotal2),
                (cbIlot: CbIlot3, cbCarb: CbCarb3, txtQte: TxtQte3, txtTotal: TxtTotal3),
                (cbIlot: CbIlot4, cbCarb: CbCarb4, txtQte: TxtQte4, txtTotal: TxtTotal4),
                (cbIlot: CbIlot5, cbCarb: CbCarb5, txtQte: TxtQte5, txtTotal: TxtTotal5),
                (cbIlot: CbIlot6, cbCarb: CbCarb6, txtQte: TxtQte6, txtTotal: TxtTotal6),
            };

            bool hasContent = false;

            foreach (var slot in agentSlots)
            {
                string agentNom = GetAgentNom(slot.cbNom);
                if (string.IsNullOrEmpty(agentNom)) continue;

                string ilotAffecte = GetIlotNom(slot.cbIlot);

                decimal qteSuper = 0, montantSuper = 0;
                decimal qteGasoil = 0, montantGasoil = 0;

                foreach (var ligne in lignes)
                {
                    string ligneIlot = GetIlotNom(ligne.cbIlot);
                    if (string.IsNullOrEmpty(ilotAffecte) || ligneIlot != ilotAffecte) continue;

                    string carb = ligne.cbCarb.SelectedItem is ComboBoxItem ci
                        ? ci.Content?.ToString() ?? "" : "";
                    ParseDec(ligne.txtQte.Text, out decimal qte);
                    ParseDec(ligne.txtTotal.Text, out decimal montant);

                    if (carb == "Super") { qteSuper += qte; montantSuper += montant; }
                    else                 { qteGasoil += qte; montantGasoil += montant; }
                }

                PanelResumeAgents.Children.Add(
                    BuildAgentResumeSection(agentNom, ilotAffecte,
                        qteSuper, montantSuper, qteGasoil, montantGasoil));
                hasContent = true;
            }

            if (!hasContent)
            {
                PanelResumeAgents.Children.Add(new TextBlock
                {
                    Text = "Aucun agent sélectionné.",
                    Foreground = new SolidColorBrush(Color.FromRgb(0x44, 0x44, 0x66)),
                    FontSize = 12,
                    Margin = new Thickness(0, 8, 0, 8)
                });
            }
        }

        private Border BuildAgentResumeSection(string agentNom, string ilot,
            decimal qteSuper, decimal montantSuper, decimal qteGasoil, decimal montantGasoil)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0x0D, 0x1F, 0x3C)),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 8)
            };

            var sp = new StackPanel();

            string titre = string.IsNullOrEmpty(ilot)
                ? agentNom : $"{agentNom}   —   {ilot}";

            sp.Children.Add(new TextBlock
            {
                Text = titre,
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 10)
            });

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(160) });
            for (int i = 0; i < 3; i++)
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(28) });

            var verte = new SolidColorBrush(Color.FromRgb(0x00, 0xC8, 0x96));
            var jaune = new SolidColorBrush(Color.FromRgb(0xFF, 0xC2, 0x00));
            var gris  = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x88));

            void AddCell(int row, int col, string text, Brush fg, bool bold = false)
            {
                var tb = new TextBlock
                {
                    Text = text,
                    Foreground = fg,
                    FontSize = 12,
                    FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetRow(tb, row);
                Grid.SetColumn(tb, col);
                grid.Children.Add(tb);
            }

            AddCell(0, 0, "Super vendu",  Brushes.White);
            AddCell(0, 1, FmtL(qteSuper),       verte);
            AddCell(0, 2, Fmt(montantSuper),     Brushes.White);

            AddCell(1, 0, "Gasoil vendu", Brushes.White);
            AddCell(1, 1, FmtL(qteGasoil),      verte);
            AddCell(1, 2, Fmt(montantGasoil),    Brushes.White);

            decimal totalMontant = montantSuper + montantGasoil;
            AddCell(2, 0, "TOTAL", Brushes.White, true);
            AddCell(2, 1, "—",     gris);
            AddCell(2, 2, Fmt(totalMontant),     jaune, true);

            sp.Children.Add(grid);
            border.Child = sp;
            return border;
        }

        // ===== SAUVEGARDER =====
        private void BtnSauvegarder_Click(object sender, MouseButtonEventArgs e)
        {
            // TODO: Sauvegarder en base de données via API Rust
            TxtStatut.Text = "✅ Indexes sauvegardés avec succès !";
            TxtStatut.Foreground = Brushes.LightGreen;
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
                // TODO: Valider en base de données via API Rust
                TxtStatut.Text = "🔒 Indexes validés et figés !";
                TxtStatut.Foreground = Brushes.Orange;
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

        // ===== CAISSE (panel existant) =====

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
                !decimal.TryParse(TxtChargeMontant.Text, out decimal montant) || montant <= 0)
            {
                MessageBox.Show("Veuillez saisir un libellé et un montant valide.",
                    "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var ligne = new Grid { Margin = new Thickness(0, 3, 0, 3) };
            ligne.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            ligne.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });

            var txtLib = new TextBlock { Text = libelle, Foreground = Brushes.White, FontSize = 11 };
            var txtMnt = new TextBlock
            {
                Text = Fmt(montant),
                Foreground = Brushes.OrangeRed, FontSize = 11,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            Grid.SetColumn(txtLib, 0);
            Grid.SetColumn(txtMnt, 1);
            ligne.Children.Add(txtLib);
            ligne.Children.Add(txtMnt);
            ListeCharges.Children.Add(ligne);

            _totalCharges += montant;
            TxtTotalCharges.Text = Fmt(_totalCharges);
            TxtChargeLibelle.Text = "";
            TxtChargeMontant.Text = "";
            MettreAJourCaisse();
        }

        private void BtnAjouterAvance_Click(object sender, MouseButtonEventArgs e)
        {
            string personnel = TxtAvancePersonnel.Text.Trim();
            if (string.IsNullOrEmpty(personnel) ||
                !decimal.TryParse(TxtAvanceMontant.Text, out decimal montant) || montant <= 0)
            {
                MessageBox.Show("Veuillez saisir un nom et un montant valide.",
                    "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var ligne = new Grid { Margin = new Thickness(0, 3, 0, 3) };
            ligne.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            ligne.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });

            var txtLib = new TextBlock { Text = personnel, Foreground = Brushes.White, FontSize = 11 };
            var txtMnt = new TextBlock
            {
                Text = Fmt(montant),
                Foreground = Brushes.OrangeRed, FontSize = 11,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            Grid.SetColumn(txtLib, 0);
            Grid.SetColumn(txtMnt, 1);
            ligne.Children.Add(txtLib);
            ligne.Children.Add(txtMnt);
            ListeAvances.Children.Add(ligne);

            _totalAvances += montant;
            TxtTotalAvances.Text = Fmt(_totalAvances);
            TxtAvancePersonnel.Text = "";
            TxtAvanceMontant.Text = "";
            MettreAJourCaisse();
        }

        private void MettreAJourCaisse()
        {
            decimal totalSuper = 0, totalGasoil = 0, totalBrut = 0;
            decimal totalDecharges = _decharges[0] + _decharges[1] + _decharges[2];

            if (ParseDec(TxtQte1.Text, out decimal q1)) totalSuper += q1;
            if (ParseDec(TxtQte3.Text, out decimal q3)) totalSuper += q3;
            if (ParseDec(TxtQte5.Text, out decimal q5)) totalSuper += q5;
            if (ParseDec(TxtQte2.Text, out decimal q2)) totalGasoil += q2;
            if (ParseDec(TxtQte4.Text, out decimal q4)) totalGasoil += q4;
            if (ParseDec(TxtQte6.Text, out decimal q6)) totalGasoil += q6;

            if (ParseDec(TxtTotal1.Text, out decimal t1)) totalBrut += t1;
            if (ParseDec(TxtTotal2.Text, out decimal t2)) totalBrut += t2;
            if (ParseDec(TxtTotal3.Text, out decimal t3)) totalBrut += t3;
            if (ParseDec(TxtTotal4.Text, out decimal t4)) totalBrut += t4;
            if (ParseDec(TxtTotal5.Text, out decimal t5)) totalBrut += t5;
            if (ParseDec(TxtTotal6.Text, out decimal t6)) totalBrut += t6;

            decimal brutP1 = (ParseDec(TxtTotal1.Text, out decimal tp1) ? tp1 : 0)
                           + (ParseDec(TxtTotal2.Text, out decimal tp2) ? tp2 : 0);
            decimal brutP2 = (ParseDec(TxtTotal3.Text, out decimal tp3) ? tp3 : 0)
                           + (ParseDec(TxtTotal4.Text, out decimal tp4) ? tp4 : 0);
            decimal brutP3 = (ParseDec(TxtTotal5.Text, out decimal tp5) ? tp5 : 0)
                           + (ParseDec(TxtTotal6.Text, out decimal tp6) ? tp6 : 0);

            TxtCaisseP1Super.Text    = FmtL(ParseDec(TxtQte1.Text, out decimal s1) ? s1 : 0);
            TxtCaisseP1Gasoil.Text   = FmtL(ParseDec(TxtQte2.Text, out decimal g1) ? g1 : 0);
            TxtCaisseP1Brut.Text     = Fmt(brutP1);
            TxtCaisseP1Decharge.Text = Fmt(_decharges[0]);
            TxtCaisseP1Net.Text      = Fmt(brutP1 - _decharges[0]);

            TxtCaisseP2Super.Text    = FmtL(ParseDec(TxtQte3.Text, out decimal s2) ? s2 : 0);
            TxtCaisseP2Gasoil.Text   = FmtL(ParseDec(TxtQte4.Text, out decimal g2) ? g2 : 0);
            TxtCaisseP2Brut.Text     = Fmt(brutP2);
            TxtCaisseP2Decharge.Text = Fmt(_decharges[1]);
            TxtCaisseP2Net.Text      = Fmt(brutP2 - _decharges[1]);

            TxtCaisseP3Super.Text    = FmtL(ParseDec(TxtQte5.Text, out decimal s3) ? s3 : 0);
            TxtCaisseP3Gasoil.Text   = FmtL(ParseDec(TxtQte6.Text, out decimal g3) ? g3 : 0);
            TxtCaisseP3Brut.Text     = Fmt(brutP3);
            TxtCaisseP3Decharge.Text = Fmt(_decharges[2]);
            TxtCaisseP3Net.Text      = Fmt(brutP3 - _decharges[2]);

            TxtRecapSuper.Text         = FmtL(totalSuper);
            TxtRecapSuperMontant.Text  = Fmt(totalSuper * _prixSuper);
            TxtRecapGasoil.Text        = FmtL(totalGasoil);
            TxtRecapGasoilMontant.Text = Fmt(totalGasoil * _prixGasoil);
            TxtRecapBrut.Text          = Fmt(totalBrut);
            TxtRecapDecharges.Text     = Fmt(totalDecharges);
            TxtRecapCharges.Text       = Fmt(_totalCharges);
            TxtRecapAvances.Text       = Fmt(_totalAvances);

            decimal caisseNette = totalBrut + totalDecharges - _totalCharges - _totalAvances;
            TxtCaisseNette.Text = Fmt(caisseNette);
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
                // TODO: Sauvegarder en base via API Rust
                MessageBox.Show("✅ Caisse clôturée avec succès !",
                    "Caisse clôturée", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
