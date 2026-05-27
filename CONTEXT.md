# Gestion Station Shell — Contexte du projet

> **Usage :** Ce fichier est le point de vérité partagé entre Claude Code et Claude.ai.
> - **Claude Code** : lis ce fichier en début de session. Mets-le à jour en fin de session.
> - **Claude.ai** : l'utilisateur colle ce fichier pour synchroniser le contexte.

---

## Propriétaire du projet

**Jean Christophe Bougah (CRISS)**
QM – Station Shell Sangalkam, Sénégal
Environnement de développement : Windows, VS Code, PowerShell

---

## Stack technique

| Couche       | Technologie          | Rôle                              |
|--------------|----------------------|-----------------------------------|
| Backend      | Rust / Axum          | API REST                          |
| Frontend     | .NET 8 WPF (Desktop) | Interface utilisateur             |
| Base de données | PostgreSQL        | Stockage des données              |

---

## Modules métier

### Shell Piste (en cours)
Gestion des pompes à carburant : index, ventes, caisse journalière.

### Shell Shop (à venir)
Gestion de la boutique : inventaire, point de vente.

---

## Schéma de base de données

### Tables existantes (21 au total)

| Table                | Description                                      |
|----------------------|--------------------------------------------------|
| `ilots`              | 3 îlots de la station                            |
| `pompes`             | 3 pompes (liées aux îlots)                       |
| `prix_carburants`    | Super = 920 FCFA/L · Gasoil = 680 FCFA/L        |
| `affectations`       | Affectation des pompistes aux pompes             |
| `indexes_pompes`     | Index de départ / arrivée par pompe et par jour  |
| `decharges`          | Versements anticipés par pompiste                |
| `avances_salaire`    | Avances sur salaire (prélèvement global)         |
| *(+ 14 autres)*      | À documenter au fil du développement            |

---

## Règles métier — Shell Piste

### Carburants
- 2 types : **Super** (920 FCFA/L) et **Gasoil** (680 FCFA/L)
- 3 pompes · 2 carburants = **6 index à saisir par jour**

### Workflow des index
1. **Chef de Piste** saisit les index d'arrivée et de départ
2. **Manager** peut modifier après validation
3. **Admin** a contrôle total
4. L'index d'arrivée du jour suivant = index de départ du jour courant (report automatique)

### Caisse du Jour
- Détail par pompiste : ventes calculées depuis les index
- **Décharges** : versements anticipés par pompiste (déduits de sa part)
- **Prélèvements globaux** : charges et avances sur salaire (déduits du total journalier)
- Clôture journalière avec total net

### Prélèvements (2 types)
- **Charges** : dépenses opérationnelles déduites du global
- **Avances sur salaire** : déduites du global journalier

---

## Hiérarchie des rôles

```
Admin
 └── Manager
      └── Chef de Piste (1 seul)
           ├── Chefs de Piste Adjoints
           ├── Agents Commerciaux Piste (pompistes)
           ├── Laveurs
           ├── Graisseurs
           ├── Techniciens de Surface
           └── Clients
```

**Création des comptes :**
- Admin crée les comptes Manager
- Manager crée tous les autres comptes

---

## Interfaces WPF réalisées

### 1. LoginWindow (`Views/LoginWindow.xaml`)
- Formulaire email + mot de passe
- Validation côté client (champs vides → message d'erreur)
- Authentification locale provisoire : `admin@shell.com` / `admin123`
- Redirige vers `AdminDashboard` après connexion réussie
- **Point d'entrée de l'application** (défini dans `App.xaml.cs`)

### 2. AdminDashboard (`Views/AdminDashboard.xaml`)
- Tableau de bord de l'administrateur
- Menu de navigation : clic sur "Piste" → ouvre `ShellPisteWindow`
- Design dark theme cohérent

### 3. ShellPisteWindow (`Views/ShellPisteWindow.xaml` + `.xaml.cs`)
Interface principale de gestion de la piste — **refaite en profondeur** (sessions 2026-05-18 et 2026-05-27).
Menu latéral avec 4 panels : Indexes Pompes · Caisse du Jour · Cuves · Ventes Piste.

**Panel Indexes (3 sections) :**
- **Sect. 1 — Agents & Plage horaire** : 3 slots horizontaux (CbAgent1/2/3 + CbIlotAgent1/2/3) + DatePickers début/fin stylisés (texte visible sur fond sombre via `DatePickerTextBox` resource, début=jaune #FFC200, fin=rouge #DD1F26)
- **Sect. 2 — Indexes Pompes** : grille 9 colonnes (Pompiste, Îlot, Pompe, Carburant, Idx Départ, Idx Arrivée, Qté, P.U, Montant) · 6 lignes · TxtPompiste1–6 auto-remplis selon mapping îlot→agent · calcul auto Qté et Montant · ligne TOTAL GÉNÉRAL · tous les montants formatés avec séparateur milliers
- **Sect. 3 — Résumé Indexes Pompes** : `PanelResumeAgents` (StackPanel) généré dynamiquement — une section par agent avec total Super L + montant, total Gasoil L + montant, Total FCFA
- Boutons : Sauvegarder (#0F3460) + Valider (#DD1F26) en bas du panel
- *Anciennes Sections 3/4/5 (Paiements, Prélèvements, Résumé Caisse) supprimées du panel Indexes*

**Panel Caisse du Jour** : tableau par pompiste (Super L, Gasoil L, Ventes Brutes, Décharges, Net à remettre) · Prélèvements globaux (Charges + Avances sur salaire) · Récapitulatif global (Caisse Nette) · Bouton Clôturer

**Panel Cuves** : saisie quantité actuelle Super et Gasoil

**Panel Ventes Piste** : placeholder (aucune vente enregistrée)

**Code-behind :**
- Guard `_isLoaded` · `ShowPanel()` pour navigation entre les 4 panels
- Helpers formatage : `Fmt(decimal)` → "N0 FCFA" · `FmtL(decimal)` → "N2 L" · `ParseDec(string, out decimal)` → parse avec séparateur milliers (`NumberStyles.Number`)
- Calcul automatique : `IndexArrivee_Changed` → `CalculerLigne` → `CalculerTotaux` → `GenererResumeAgents`
- `IlotAgent_Changed` : déclenché par CbIlotAgent1/2/3 → `MettreAJourPompistes` + `GenererResumeAgents`
- `Ilot_Changed` : synchronise ComboBox Pompe + `MettreAJourPompistes` + `GenererResumeAgents`
- `Carburant_Changed` : met à jour le prix unitaire et recalcule
- `MettreAJourPompistes` : mappe îlot → agent (GetAgentPourIlot) et remplit TxtPompiste1–6
- `GenererResumeAgents` : vide PanelResumeAgents et reconstruit une section par agent (BuildAgentResumeSection)
- Caisse panel : `BtnDecharge_Click`, `BtnAjouterCharge_Click`, `BtnAjouterAvance_Click`, `MettreAJourCaisse`

---

## Flux de navigation

```
App démarrage
  └── LoginWindow
        └── [succès] → AdminDashboard
                          └── [clic Piste] → ShellPisteWindow
```

---

## Points techniques importants

- **Guard `_isLoaded`** sur les event handlers WPF → évite `NullReferenceException` à l'initialisation XAML
- **Authentification provisoire** : hardcodée dans `LoginWindow.xaml.cs` (email/mdp en dur), à remplacer par un appel API
- L'API Rust/Axum **n'est pas encore connectée** au frontend WPF (intégration à faire)
- Backend Rust/Axum initialisé : `backend/src/main.rs`, `backend/src/db/mod.rs`, dépendances dans `Cargo.toml`
- Développement en cours sur Windows avec PowerShell

---

## État du projet

> ⚠️ **Section mise à jour par Claude Code après chaque session**

### Fait ✅
- [x] Schéma BDD (21 tables)
- [x] LoginWindow (authentification locale provisoire)
- [x] AdminDashboard (tableau de bord admin avec navigation)
- [x] ShellPisteWindow entièrement refaite (menu 4 panels + 5 sections indexes, commitée)
- [x] Caisse du Jour (décharges, prélèvements, clôture)
- [x] Structure backend Rust/Axum (main.rs + db/mod.rs + Cargo.toml)
- [x] Correction bug double instance : suppression `StartupUri` dans `App.xaml` (fenêtre créée uniquement par `OnStartup`)
- [x] ShellPisteWindow refaite en profondeur (session 2026-05-27) : 3 slots agents horizontaux, colonne Pompiste auto, Résumé Agents dynamique, séparateur milliers sur tous les montants, DatePickers corrigés (texte visible + couleurs début/fin correctes)

### En cours 🔄
- [ ] Logique de persistance (boutons Sauvegarder / Valider / Clôturer → TODO en dur, API Rust non connectée)

### À faire 📋
- [ ] Remplacer l'authentification hardcodée par un appel à l'API Rust
- [ ] Connexion API Rust ↔ Frontend WPF (endpoints à implémenter)
- [ ] Gestion des rôles (Manager, Chef de Piste, etc.) dans le routing post-login
- [ ] Panel Cuves : brancher la mise à jour réelle des quantités
- [ ] Panel Ventes Piste : implémenter la liste des ventes
- [ ] Agents dynamiques dans CbAgent (depuis la BDD, pas statiques)
- [ ] Module Shell Shop (inventaire, POS)
- [ ] Tests et validation des règles métier bout en bout

---

## Historique des sessions

| Date       | Résumé                                                                 |
|------------|------------------------------------------------------------------------|
| 2026-04-08 | Schéma BDD étendu à 21 tables, ShellPisteWindow et Caisse du Jour créés, règles métier documentées |
| 2026-04-08 | Ajout LoginWindow (auth provisoire), AdminDashboard avec navigation, initialisation backend Rust/Axum |
| 2026-05-14 | Lecture du projet, lancement de l'app WPF (`dotnet run`), mise à jour de CONTEXT.md pour refléter l'état réel du code |
| 2026-05-18 | ShellPisteWindow entièrement refaite : menu latéral 4 panels (Indexes, Caisse, Cuves, Ventes), 5 sections dans le panel Indexes (Agent/Plage, Grille pompes, Paiements, Prélèvements, Résumé caisse), calculs automatiques cash/écart — commitée et pushée sur GitHub |
| 2026-05-26 | Correction bug double instance (`StartupUri` + `OnStartup` ouvraient chacun une LoginWindow) — suppression `StartupUri` dans `App.xaml`, correction pushée |
| 2026-05-27 | ShellPisteWindow : 3 slots agents horizontaux (CbAgent1/2/3 + CbIlotAgent1/2/3), colonne Pompiste auto dans la grille, Résumé Agents dynamique (PanelResumeAgents), séparateur milliers sur tous les montants (Fmt/FmtL/ParseDec), DatePickers corrigés (texte visible + couleurs début=jaune/fin=rouge correctes) |

---

*Dernière mise à jour : 2026-05-27 (Claude Code — session)*
