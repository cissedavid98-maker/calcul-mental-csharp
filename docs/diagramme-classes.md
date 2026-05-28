# Diagramme de Classes — Jeu de Calcul Mental

```mermaid
classDiagram
    direction TB

    %% ───────────── MODÈLES ─────────────

    class PlayerProfile {
        +Guid Id
        +string Username
        +DateTime CreatedAt
        +PlayerStatistics Statistics
        +List~Badge~ Badges
        +List~GameSession~ GameHistory
    }

    class PlayerStatistics {
        +int TotalGamesPlayed
        +int TotalPoints
        +double GlobalAccuracy
        +Dictionary~GameMode, int~ BestScorePerMode
        +Dictionary~Operation, double~ OperationAccuracy
    }

    class GameSession {
        +Guid Id
        +Guid PlayerId
        +GameMode Mode
        +Difficulty Difficulty
        +DateTime StartTime
        +DateTime EndTime
        +int FinalScore
        +int MaxStreak
        +bool IsCompleted
        +List~Question~ Questions
    }

    class Question {
        +int Operand1
        +int Operand2
        +Operation Operation
        +int CorrectAnswer
        +int? PlayerAnswer
        +bool IsCorrect
        +TimeSpan ResponseTime
    }

    class Score {
        +Guid PlayerId
        +string PlayerName
        +int Points
        +GameMode Mode
        +Difficulty Difficulty
        +DateTime Date
        +int MaxStreak
    }

    class Badge {
        +string Id
        +string Name
        +string Description
        +bool IsUnlocked
        +DateTime? UnlockedAt
    }

    %% ───────────── SERVICES ─────────────

    class QuestionGenerator {
        +Question GenerateQuestion(Difficulty, Operation)
        +Operation GenerateRandomOperation()
        -ValueTuple GetNumberRange(Difficulty)
    }

    class GameEngine {
        -GameSession _currentSession
        -int _currentStreak
        -int _score
        +void StartGame(GameMode, Difficulty, Guid)
        +AnswerResult SubmitAnswer(int answer)
        +GameSession EndGame()
    }

    class SaveService {
        -string _dataPath
        +void SaveProfile(PlayerProfile)
        +PlayerProfile LoadProfile(string username)
        +List~PlayerProfile~ LoadAllProfiles()
        +void SaveScore(Score)
        +List~Score~ GetLeaderboard(GameMode, Difficulty)
    }

    class BadgeService {
        +List~Badge~ CheckAndAwardBadges(PlayerProfile, GameSession)
        +List~Badge~ GetAllBadges()
    }

    class StatisticsService {
        +PlayerStatistics CalculateStatistics(PlayerProfile)
        +Operation GetStrongestOperation(PlayerProfile)
        +Operation GetWeakestOperation(PlayerProfile)
    }

    class DailyChallenge {
        +DateTime Date
        +int Seed
        +GameSession GenerateChallenge(DateTime)
        +bool IsPlayedToday(PlayerProfile)
    }

    %% ───────────── ÉNUMÉRATIONS ─────────────

    class GameMode {
        <<enumeration>>
        Chrono
        Serie
        Survie
        DéfiDuJour
    }

    class Difficulty {
        <<enumeration>>
        Facile
        Moyen
        Difficile
    }

    class Operation {
        <<enumeration>>
        Addition
        Soustraction
        Multiplication
        Division
    }

    %% ───────────── RELATIONS ─────────────

    PlayerProfile "1" *-- "1" PlayerStatistics : possède
    PlayerProfile "1" *-- "*" Badge : détient
    PlayerProfile "1" *-- "*" GameSession : historique

    GameSession "1" *-- "*" Question : contient
    GameSession --> GameMode : mode
    GameSession --> Difficulty : difficulté
    Question --> Operation : opération

    GameEngine ..> QuestionGenerator : utilise
    GameEngine ..> GameSession : crée
    GameEngine --> GameMode : utilise
    GameEngine --> Difficulty : utilise

    BadgeService ..> PlayerProfile : analyse
    BadgeService ..> GameSession : analyse
    BadgeService ..> Badge : attribue

    StatisticsService ..> PlayerProfile : analyse
    StatisticsService ..> PlayerStatistics : calcule

    SaveService ..> PlayerProfile : persiste
    SaveService ..> Score : persiste

    DailyChallenge ..> GameSession : configure

    Score --> GameMode : filtré par
    Score --> Difficulty : filtré par
```

## Légende

| Symbole | Signification |
|---------|---------------|
| `*--`   | Composition (l'enfant ne peut exister sans le parent) |
| `o--`   | Agrégation (relation forte mais indépendante) |
| `..>`   | Dépendance (utilisation temporaire) |
| `-->`   | Association (lien direct) |
| `<<enumeration>>` | Type énuméré |

## Description des classes principales

### Modèles
- **PlayerProfile** : profil complet du joueur, point central des données.
- **PlayerStatistics** : agrège toutes les métriques calculées d'un joueur.
- **GameSession** : une partie complète, avec toutes ses questions et son score final.
- **Question** : une question posée, avec la réponse du joueur et le temps de réponse.
- **Score** : entrée de classement, indépendante du profil pour les requêtes rapides.
- **Badge** : récompense débloquée selon les accomplissements.

### Services
- **QuestionGenerator** : génère des questions selon le niveau et l'opération, garantit les règles (division entière, soustraction ≥ 0).
- **GameEngine** : orchestre une partie en temps réel (score, streak, timer, vies).
- **SaveService** : sérialise/désérialise toutes les données en JSON local.
- **BadgeService** : vérifie les conditions de déblocage après chaque partie.
- **StatisticsService** : calcule les statistiques à partir de l'historique.
- **DailyChallenge** : génère un défi reproductible via un seed basé sur la date.
