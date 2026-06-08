# Diagramme de Cas d'Utilisation — Jeu de Calcul Mental

```mermaid
flowchart TB
    %% ─── ACTEURS ───
    Joueur([" Joueur"])
    Systeme([" Système"])

    %% ─── PACKAGE : Gestion du Profil ───
    subgraph Profil[" Gestion du Profil"]
        UC1("Créer un profil")
        UC2("Sélectionner un profil")
        UC3("Consulter ses badges")
        UC4("Consulter ses statistiques")
    end

    %% ─── PACKAGE : Jouer une Partie ───
    subgraph Jeu[" Jouer une Partie"]
        UC5("Choisir le mode de jeu")
        UC6("Choisir la difficulté")
        UC7("Jouer en mode Chrono")
        UC8("Jouer en mode Série")
        UC9("Jouer en mode Survie")
        UC10("Relever le Défi du jour")
        UC11("Répondre à une question")
        UC12("Recevoir un feedback")
    end 

    %% ─── PACKAGE : Classement ───
    subgraph Classement[" Classement"]
        UC13("Consulter le classement")
        UC14("Filtrer par mode / difficulté")
    end

    %% ─── PACKAGE : Système (automatique) ───
    subgraph Auto[" Traitements Automatiques"]
        UC15("Générer les questions")
        UC16("Calculer le score et le streak")
        UC17("Attribuer les badges")
        UC18("Sauvegarder les données")
        UC19("Générer le défi quotidien")
    end

    %% ─── RELATIONS : Joueur ───
    Joueur --> UC1
    Joueur --> UC2
    Joueur --> UC3
    Joueur --> UC4
    Joueur --> UC5
    Joueur --> UC13

    UC5 --> UC6
    UC6 --> UC7
    UC6 --> UC8
    UC6 --> UC9
    UC6 --> UC10

    UC7 --> UC11
    UC8 --> UC11
    UC9 --> UC11
    UC10 --> UC11
    UC11 --> UC12

    UC13 --> UC14

    %% ─── RELATIONS : Système ───
    Systeme --> UC15
    Systeme --> UC16
    Systeme --> UC17
    Systeme --> UC18
    Systeme --> UC19

    UC11 -.->|déclenche| UC15
    UC11 -.->|déclenche| UC16
    UC12 -.->|peut déclencher| UC17
    UC7 & UC8 & UC9 & UC10 -.->|fin de partie| UC18
    UC19 -.->|utilisé par| UC10
```

---

## Tableau des cas d'utilisation

### Acteurs

| Acteur | Rôle |
|--------|------|
| **Joueur** | Utilisateur principal — interagit avec l'interface |
| **Système** | Traite automatiquement les données en arrière-plan |

---

### Cas d'utilisation détaillés

#### Gestion du Profil

| ID | Cas d'utilisation | Acteur | Description |
|----|-------------------|--------|-------------|
| UC1 | Créer un profil | Joueur | Saisir un pseudo, initialiser les stats et badges |
| UC2 | Sélectionner un profil | Joueur | Choisir un profil existant au démarrage |
| UC3 | Consulter ses badges | Joueur | Voir les badges obtenus et ceux à débloquer |
| UC4 | Consulter ses statistiques | Joueur | Voir précision, meilleurs scores, opérations fortes/faibles |

#### Jouer une Partie

| ID | Cas d'utilisation | Acteur | Description |
|----|-------------------|--------|-------------|
| UC5 | Choisir le mode de jeu | Joueur | Sélectionner Chrono, Série, Survie ou Défi du jour |
| UC6 | Choisir la difficulté | Joueur | Sélectionner Facile, Moyen ou Difficile |
| UC7 | Jouer en mode Chrono | Joueur | Répondre au maximum en 60 secondes |
| UC8 | Jouer en mode Série | Joueur | Répondre à 10, 20 ou 30 questions sans limite de temps |
| UC9 | Jouer en mode Survie | Joueur | Jouer avec 3 vies, chaque erreur en retire une |
| UC10 | Relever le Défi du jour | Joueur | Jouer le défi unique du jour, jouable une seule fois |
| UC11 | Répondre à une question | Joueur | Saisir la réponse à une opération affichée |
| UC12 | Recevoir un feedback | Joueur | Voir si la réponse est correcte et la correction si besoin |

#### Classement

| ID | Cas d'utilisation | Acteur | Description |
|----|-------------------|--------|-------------|
| UC13 | Consulter le classement | Joueur | Voir le top 10 des meilleurs scores |
| UC14 | Filtrer par mode / difficulté | Joueur | Affiner le classement selon le mode et le niveau |

#### Traitements Automatiques (Système)

| ID | Cas d'utilisation | Acteur | Description |
|----|-------------------|--------|-------------|
| UC15 | Générer les questions | Système | Créer des opérations aléatoires selon le niveau choisi |
| UC16 | Calculer le score et le streak | Système | Mettre à jour le score en temps réel avec bonus de série |
| UC17 | Attribuer les badges | Système | Vérifier et débloquer les badges après chaque partie |
| UC18 | Sauvegarder les données | Système | Écrire profil, session et scores en JSON à la fin de chaque partie |
| UC19 | Générer le défi quotidien | Système | Produire un défi identique pour tous via un seed basé sur la date |

---

## Règles métier importantes

- **Divisions** : le résultat est toujours un entier (ex : 8 ÷ 4, jamais 7 ÷ 3).
- **Soustractions Facile** : le résultat est toujours ≥ 0 (le plus grand nombre est toujours en premier).
- **Défi du jour** : généré à partir de la date comme seed aléatoire — même questions pour tous les joueurs ce jour-là, jouable une seule fois par profil.
- **Streak** : le bonus de série s'accumule à chaque bonne réponse consécutive et se remet à zéro à la première erreur.
