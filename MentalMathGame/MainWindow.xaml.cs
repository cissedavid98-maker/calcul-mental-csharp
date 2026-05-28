using System.Windows;
using MentalMathGame.Models;
using MentalMathGame.Models.Enums;
using MentalMathGame.Services;
using MentalMathGame.ViewModels;
using MentalMathGame.Views;

namespace MentalMathGame;

public partial class MainWindow : Window
{
    private readonly SaveService _saveService = new();
    private readonly NavigationService _navService = new();
    private PlayerProfile? _currentPlayer;

    public MainWindow()
    {
        InitializeComponent();
        _navService.Initialize(MainContent);
        ShowProfileSelection();
    }

    private void ShowProfileSelection()
    {
        var vm = new ProfileSelectionViewModel(_saveService, OnProfileSelected);
        var view = new ProfileSelectionView { DataContext = vm };
        _navService.NavigateTo(view);
    }

    private void OnProfileSelected(PlayerProfile profile)
    {
        _currentPlayer = profile;
        ShowMainMenu();
    }

    private void ShowMainMenu()
    {
        if (_currentPlayer == null) return;

        var vm = new MainMenuViewModel(_currentPlayer, OnModeSelected, ShowProfileSelection);
        var view = new MainMenuView { DataContext = vm };
        _navService.NavigateTo(view);
    }

    private void OnModeSelected(GameMode mode)
    {
        // Semaine 2 : navigation vers l'écran de jeu
        MessageBox.Show($"Mode {mode} — disponible à la semaine 2 !",
                        "Bientôt disponible",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
    }
}
