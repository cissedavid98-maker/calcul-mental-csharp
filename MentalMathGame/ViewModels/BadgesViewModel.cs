using MentalMathGame.Models;
using MentalMathGame.Services;

namespace MentalMathGame.ViewModels;

public class BadgesViewModel : BaseViewModel
{
    private readonly PlayerProfile _player;
    private readonly Action        _onBack;

    public IReadOnlyList<Badge> Badges       => _player.Badges;
    public int                  UnlockedCount => _player.Badges.Count(b => b.IsUnlocked);
    public int                  TotalCount    => _player.Badges.Count;
    public string               ProgressText  => $"{UnlockedCount} / {TotalCount} badges débloqués";

    public RelayCommand BackCommand { get; }

    public BadgesViewModel(PlayerProfile player, Action onBack)
    {
        _player = player;
        _onBack = onBack;

        BadgeService.EnsureInitialized(_player);

        BackCommand = new RelayCommand(_onBack);
    }
}
