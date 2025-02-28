# 📌 Gestionnaire de Mots de Passe - Blazor

Un gestionnaire de mots de passe sécurisé développé avec **Blazor WebAssembly (InteractiveServer)** et **ASP.NET Core Web API**. 

### 🛠 Installation
1. **Cloner le repo**
```bash
 git clone https://github.com/christophersemard/PasswordManager
 cd password-manager
```
2. **Lancer l’API Backend**
```bash
 cd Api
 dotnet run
```
3. **Lancer l'application Blazor**
```bash
 cd ../Web
 dotnet run
```
4. 🌐 **Accéder à l’application**
Ouvre ton navigateur sur [http://localhost:5215](http://localhost:5215) (ou autre port selon la config).

## 📜 Fonctionnalités Implémentées
| 🛠 Fonctionnalité  | Statut |
|-------------------|--------|
| **Authentification** (Inscription, Connexion) | ✅ |
| **Gestion des mots de passe** (Ajout, Modification, Suppression) | ✅ |
| **Catégorisation** des mots de passe | ✅ (Filtre en haut à gauche du tableau des mdp, catégorie à choisir dans le formulaire) |
| **Recherche rapide** | ✅ (En haut à droite du tableau des mdp)|
| **Chiffrement des mots de passe** (Chiffrement/Déchiffrement AES seulement en local grâce au mot de passe utilisé pour générer la clé de chiffrement, mots de passes stockés chiffrés mais non déchiffrables sans la clé de déchiffrement) | |
| **Mode hors ligne** | ❌ *(Non implémenté)* |
| **Sécurité avancée** (Mot de passe principal, verrouillage après tentatives échouées) | ✅ *(Mot de passe classique, pas de vérouillage)* |
| **Générateur de mots de passe sécurisé** | ✅ *(Bouton de génération dans le formulaire d'ajout/edit* |
| **Sauvegarde sur base de données SQLite** | ✅ |
| **Interface utilisateur responsive et ergonomique** | ✅ |
| **Tests unitaires** (xUnit, bUnit) | ⚠️ *(Tests partiels dans le projet Tests)* |
| **Application mobile Blazor Hybrid** | ❌ *(Non implémenté, bonus)* |

## 📌 Technologies utilisées
- **Frontend :** Blazor WebAssembly (InteractiveServer) + MudBlazor
- **Backend :** ASP.NET Core Web API + Entity Framework Core
- **Base de données :** SQLite
- **Interop JavaScript :** Utilisé pour le copier-coller
