using System.IO;
using System.Text.Json;
using MentalMathGame.Models;
using MentalMathGame.Models.Enums;

namespace MentalMathGame.Services;

public class SaveService
{
    private static readonly string DataFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "MentalMathGame");

    private static readonly string ProfilesFolder = Path.Combine(DataFolder, "profiles");
    private static readonly string ScoresFile = Path.Combine(DataFolder, "scores.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public SaveService()
    {
        Directory.CreateDirectory(ProfilesFolder);
    }

    public void SaveProfile(PlayerProfile profile)
    {
        var path = GetProfilePath(profile.Id);
        var json = JsonSerializer.Serialize(profile, JsonOptions);
        File.WriteAllText(path, json);
    }

    public PlayerProfile? LoadProfile(Guid id)
    {
        var path = GetProfilePath(id);
        if (!File.Exists(path)) return null;
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<PlayerProfile>(json, JsonOptions);
    }

    public List<PlayerProfile> LoadAllProfiles()
    {
        var profiles = new List<PlayerProfile>();
        foreach (var file in Directory.GetFiles(ProfilesFolder, "*.json"))
        {
            try
            {
                var json = File.ReadAllText(file);
                var profile = JsonSerializer.Deserialize<PlayerProfile>(json, JsonOptions);
                if (profile != null) profiles.Add(profile);
            }
            catch { }
        }
        return profiles.OrderBy(p => p.CreatedAt).ToList();
    }

    public void DeleteProfile(Guid id)
    {
        var path = GetProfilePath(id);
        if (File.Exists(path)) File.Delete(path);
    }

    public void SaveScore(Score score)
    {
        var scores = LoadAllScores();
        scores.Add(score);
        var json = JsonSerializer.Serialize(scores, JsonOptions);
        File.WriteAllText(ScoresFile, json);
    }

    public List<Score> GetLeaderboard(GameMode mode, Difficulty difficulty, int top = 10)
    {
        return LoadAllScores()
            .Where(s => s.Mode == mode && s.Difficulty == difficulty)
            .OrderByDescending(s => s.Points)
            .Take(top)
            .ToList();
    }

    public List<Score> GetAllScoresForPlayer(Guid playerId)
    {
        return LoadAllScores()
            .Where(s => s.PlayerId == playerId)
            .OrderByDescending(s => s.Date)
            .ToList();
    }

    private List<Score> LoadAllScores()
    {
        if (!File.Exists(ScoresFile)) return [];
        try
        {
            var json = File.ReadAllText(ScoresFile);
            return JsonSerializer.Deserialize<List<Score>>(json, JsonOptions) ?? [];
        }
        catch { return []; }
    }

    private static string GetProfilePath(Guid id) =>
        Path.Combine(ProfilesFolder, $"{id}.json");
}
