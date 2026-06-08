# Rapport d'avancement — Semaine du 2 au 7 juin 2026

**Projet :** Jeu de Calcul Mental (C# / WPF)  
**Auteur :** cissedavid98  
**Date :** 8 juin 2026

---

## 1. Résumé de la semaine

Cette semaine correspond à la finalisation de la **Semaine 1 (S1)** du plan de développement sur 4 semaines. L'objectif était d'établir les fondations complètes de l'application : architecture MVVM, modèles de données, services de persistance, navigation et interface de base. Les deux objectifs de S1 ont été entièrement atteints et poussés sur GitHub.

---

## 2. Langages et technologies utilisés

### 2.1 Langage de programmation — C#

C# (C Sharp) est un langage de programmation orienté objet créé par Microsoft. C'est le langage principal du projet.

**Pourquoi C# ?**
- Syntaxe claire et fortement typée : chaque variable a un type précis, ce qui évite les erreurs.
- Orienté objet : on modélise l'application avec des classes (`PlayerProfile`, `GameSession`, etc.).
- Intégration native avec .NET et WPF.
- Fonctionnalités modernes utilisées dans ce projet :
  - `Guid` — identifiant unique universel (pour les profils et les parties).
  - `INotifyPropertyChanged` — interface qui notifie l'interface graphique quand une donnée change.
  - `Dictionary<K, V>` — tableau associatif pour stocker les scores et statistiques par mode.
  - `List<T>` — liste générique pour les questions, badges et historique.
  - Propriétés automatiques (`{ get; set; }`) pour les modèles de données.

### 2.2 Framework — .NET 10

.NET est la plateforme d'exécution développée par Microsoft sur laquelle tourne l'application.

**Ce qu'il apporte au projet :**
- Compilation et exécution du code C#.
- Bibliothèques standard : `System.Text.Json` pour la sauvegarde, `System.Collections.Generic` pour les listes et dictionnaires.
- Gestion automatique de la mémoire (garbage collector).
- Version utilisée : **.NET 10** (dernière version stable au moment du développement).

### 2.3 Interface graphique — WPF (Windows Presentation Foundation)

WPF est le framework Microsoft pour créer des interfaces graphiques Windows. Les vues sont décrites en **XAML** (langage de balisage similaire au HTML).

**Ce qu'il apporte au projet :**
- Séparation claire entre l'interface (`.xaml`) et la logique (`.cs`) grâce au pattern MVVM.
- Système de **data binding** : l'interface se met à jour automatiquement quand les données changent, sans code supplémentaire.
- Styles et ressources réutilisables : le thème sombre est défini une seule fois dans `Styles.xaml` et appliqué à toute l'application.
- Contrôles riches : boutons, listes, zones de texte, animations.

**XAML** (eXtensible Application Markup Language) est le langage utilisé pour décrire les vues :
```xml
<!-- Exemple : un bouton lié à une commande du ViewModel -->
<Button Content="Jouer" Command="{Binding StartGameCommand}" />
```

### 2.4 Architecture logicielle — MVVM

MVVM (Model – View – ViewModel) est le patron de conception architectural choisi pour ce projet.

| Couche | Rôle | Exemple dans le projet |
|---|---|---|
| **Model** | Données pures, sans logique d'affichage | `PlayerProfile`, `GameSession`, `Question` |
| **View** | Interface graphique uniquement (XAML) | `ProfileSelectionView.xaml`, `MainMenuView.xaml` |
| **ViewModel** | Logique de présentation, liaison entre Model et View | `ProfileSelectionViewModel`, `MainMenuViewModel` |

**Avantage principal :** la Vue ne connaît pas le Model directement. Cela facilite les tests et les modifications de l'interface sans toucher à la logique métier.

### 2.5 Persistance des données — JSON

Les données (profils, scores, historique) sont sauvegardées localement dans des fichiers **JSON** (JavaScript Object Notation).

**Pourquoi JSON ?**
- Format texte lisible par un humain.
- Léger et rapide à lire/écrire.
- Natif en .NET via `System.Text.Json` (aucune dépendance externe nécessaire).

Exemple de profil sérialisé :
```json
{
  "id": "a1b2c3...",
  "username": "Alice",
  "statistics": {
    "totalGamesPlayed": 12,
    "totalPoints": 340
  }
}
```

### 2.6 Versionnement — Git et GitHub

- **Git** : système de contrôle de version local. Chaque modification importante est enregistrée dans un commit avec un message descriptif.
- **GitHub** : hébergement distant du dépôt, permettant la sauvegarde en ligne et le suivi de l'avancement du projet.
- Branche principale utilisée : `master`.

### 2.7 Documentation — Mermaid

Les diagrammes UML sont écrits en **Mermaid**, un langage de description de diagrammes en texte pur rendu visuellement par GitHub.

**Diagrammes réalisés :**
- Diagramme de classes (`classDiagram`) — structure des données et leurs relations.
- Diagramme de cas d'utilisation (`flowchart TB`) — interactions entre l'utilisateur et le système.

### 2.8 Environnement de développement

| Outil | Usage |
|---|---|
| Visual Studio / VS Code | Édition du code C# et XAML |
| .NET SDK 10 | Compilation et exécution |
| Git CLI | Commits et push vers GitHub |
| GitHub | Hébergement du dépôt |

---

## 3. Travaux réalisés cette semaine

### 2.1 Structure du projet et architecture MVVM

- Création de la solution WPF .NET 10 (`MentalMathGame.slnx`).
- Architecture **Model-View-ViewModel (MVVM)** mise en place dès le départ.
- Configuration du fichier `.gitignore` adapté à un projet .NET / WPF.
- Thème sombre complet défini dans `Resources/Styles.xaml` (206 lignes de styles XAML).

### 2.2 Modèles de données (`Models/`)

| Classe | Description |
|---|---|
| `PlayerProfile` | Profil complet du joueur : `Guid`, nom d'utilisateur, statistiques, badges, historique |
| `PlayerStatistics` | Métriques agrégées : parties jouées, points totaux, précision globale et par opération |
| `GameSession` | Une partie complète : mode, difficulté, score final, streak maximal, liste des questions |
| `Question` | Question posée avec réponse du joueur, résultat (correct/incorrect) et temps de réponse |
| `Score` | Entrée de classement indépendante du profil pour des requêtes rapides |
| `Badge` | Récompense débloquable avec date de déblocage |

**Énumérations :**
- `GameMode` — `Chrono`, `Serie`, `Survie`, `DefiDuJour`
- `Difficulty` — `Facile`, `Moyen`, `Difficile`
- `Operation` — `Addition`, `Soustraction`, `Multiplication`, `Division`

### 2.3 Services

| Service | Description |
|---|---|
| `SaveService` | Sérialisation/désérialisation JSON locale via `System.Text.Json` : sauvegarde des profils, chargement, classement (103 lignes) |
| `NavigationService` | Gestion de la navigation entre les vues depuis la `MainWindow` (19 lignes) |

### 2.4 ViewModels et Vues

| Fichier | Description |
|---|---|
| `BaseViewModel` | Implémentation de `INotifyPropertyChanged` et commandes `RelayCommand` (45 lignes) |
| `ProfileSelectionViewModel` | Liste des profils, création et sélection d'un profil (102 lignes) |
| `MainMenuViewModel` | Navigation vers les 4 modes de jeu (37 lignes) |
| `ProfileSelectionView.xaml` | Interface de sélection et création de profil (142 lignes XAML) |
| `MainMenuView.xaml` | Menu principal avec présentation visuelle des 4 modes (166 lignes XAML) |

### 2.5 Documentation UML

- **Diagramme de classes** (`docs/diagramme-classes.md`) — 12 classes et enums avec toutes leurs relations : composition, agrégation, dépendance et association.
- **Diagramme de cas d'utilisation** (`docs/diagramme-cas-utilisation.md`) — couvre les 4 modes de jeu, la gestion de profil, le classement et les statistiques.
- Correction du 1er juin : suppression des emojis dans les labels Mermaid pour améliorer la compatibilité d'affichage.

---

## 3. Historique des commits

| Date | Hash | Message | Statistiques |
|---|---|---|---|
| 28 mai 2026 | `53c8f7d` | Initialisation du projet — structure S1 | 29 fichiers ajoutés, 1 442 insertions |
| 1er juin 2026 | `9dc9be0` | docs: mise à jour du diagramme de cas d'utilisation | 1 fichier modifié, 6 changements |

---

## 4. Métriques du code

| Indicateur | Valeur |
|---|---|
| Fichiers source C# (`.cs`) | 15 fichiers |
| Fichiers XAML (`.xaml`) | 5 fichiers |
| Lignes de code total (hors `obj/`) | ~1 150 lignes |
| Documentation UML (Mermaid) | 2 diagrammes — 335 lignes |

---

## 5. Prochaines étapes — Semaine 2 (S2)

La Semaine 2 portera sur le **moteur de jeu** :

- [ ] Implémentation de `QuestionGenerator` — génération des questions selon la difficulté et l'opération.
- [ ] Implémentation de `GameEngine` — orchestration d'une partie (score, streak, timer, vies).
- [ ] Mode **Chrono** — parties chronométrées avec score basé sur la rapidité.
- [ ] Mode **Série** — enchaînement de questions avec gestion du streak.
- [ ] Système de score et streak (remis à zéro à la première réponse incorrecte).

---

## 6. Contraintes métier à respecter

- Les **divisions** sont toujours entières (pas de résultats décimaux).
- Les **soustractions** en mode Facile donnent toujours un résultat **≥ 0**.
- Le **Défi du jour** utilise un seed basé sur la date — jouable **une seule fois** par profil.
- Le **streak** est remis à zéro dès la première réponse incorrecte.
