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

### 3. ShellPisteWindow (`Views/ShellPisteWindow.xaml`)
- Grille de saisie des index : 3 pompes × 2 carburants = 6 lignes
- Calcul automatique des quantités et totaux
- Boutons : Enregistrer / Valider
- Thème sombre personnalisé (ComboBox, styles WPF)

### 4. Caisse du Jour *(intégrée dans ShellPisteWindow)*
- Tableau par pompiste avec décharges
- Section prélèvements globaux (charges + avances sur salaire)
- Récapitulatif journalier avec total net
- Bouton de clôture

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
- [x] ShellPisteWindow (saisie des index, calculs automatiques)
- [x] Caisse du Jour (décharges, prélèvements, clôture)
- [x] Structure backend Rust/Axum (main.rs + db/mod.rs + Cargo.toml)

### En cours 🔄
- [ ] Modifications récentes sur `ShellPisteWindow.xaml` (non commitées)

### À faire 📋
- [ ] Remplacer l'authentification hardcodée par un appel à l'API Rust
- [ ] Connexion API Rust ↔ Frontend WPF (endpoints à implémenter)
- [ ] Gestion des rôles (Manager, Chef de Piste, etc.) dans le routing post-login
- [ ] Module Shell Shop (inventaire, POS)
- [ ] Tests et validation des règles métier en bout en bout

---

## Historique des sessions

| Date       | Résumé                                                                 |
|------------|------------------------------------------------------------------------|
| 2026-04-08 | Schéma BDD étendu à 21 tables, ShellPisteWindow et Caisse du Jour créés, règles métier documentées |
| 2026-04-08 | Ajout LoginWindow (auth provisoire), AdminDashboard avec navigation, initialisation backend Rust/Axum |
| 2026-05-14 | Lecture du projet, lancement de l'app WPF (`dotnet run`), mise à jour de CONTEXT.md pour refléter l'état réel du code |

---

*Dernière mise à jour : 2026-05-14 (Claude Code — session)*
