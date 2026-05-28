using System.Collections.ObjectModel;
using MentalMathGame.Models;
using MentalMathGame.Services;

namespace MentalMathGame.ViewModels;

public class ProfileSelectionViewModel : BaseViewModel
{
    private readonly SaveService _saveService;
    private readonly Action<PlayerProfile> _onProfileSelected;

    private string _newUsername = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _hasError;

    public ObservableCollection<PlayerProfile> Profiles { get; } = [];

    public string NewUsername
    {
        get => _newUsername;
        set
        {
            Set(ref _newUsername, value);
            HasError = false;
            ErrorMessage = string.Empty;
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => Set(ref _errorMessage, value);
    }

    public bool HasError
    {
        get => _hasError;
        set => Set(ref _hasError, value);
    }

    public RelayCommand CreateProfileCommand { get; }
    public RelayCommand<PlayerProfile> SelectProfileCommand { get; }
    public RelayCommand<PlayerProfile> DeleteProfileCommand { get; }

    public ProfileSelectionViewModel(SaveService saveService, Action<PlayerProfile> onProfileSelected)
    {
        _saveService = saveService;
        _onProfileSelected = onProfileSelected;

        CreateProfileCommand = new RelayCommand(CreateProfile);
        SelectProfileCommand = new RelayCommand<PlayerProfile>(p => { if (p != null) _onProfileSelected(p); });
        DeleteProfileCommand = new RelayCommand<PlayerProfile>(DeleteProfile);

        LoadProfiles();
    }

    private void LoadProfiles()
    {
        Profiles.Clear();
        foreach (var p in _saveService.LoadAllProfiles())
            Profiles.Add(p);
    }

    private void CreateProfile()
    {
        var name = NewUsername.Trim();

        if (string.IsNullOrEmpty(name))
        {
            ErrorMessage = "Le pseudo ne peut pas être vide.";
            HasError = true;
            return;
        }

        if (name.Length < 2 || name.Length > 20)
        {
            ErrorMessage = "Le pseudo doit contenir entre 2 et 20 caractères.";
            HasError = true;
            return;
        }

        if (Profiles.Any(p => p.Username.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            ErrorMessage = "Ce pseudo est déjà utilisé.";
            HasError = true;
            return;
        }

        var profile = new PlayerProfile { Username = name };
        _saveService.SaveProfile(profile);
        NewUsername = string.Empty;
        LoadProfiles();
        _onProfileSelected(profile);
    }

    private void DeleteProfile(PlayerProfile? profile)
    {
        if (profile == null) return;
        _saveService.DeleteProfile(profile.Id);
        Profiles.Remove(profile);
    }
}
