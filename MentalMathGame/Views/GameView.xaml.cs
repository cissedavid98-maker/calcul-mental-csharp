using System.ComponentModel;
using System.Windows.Controls;
using MentalMathGame.ViewModels;

namespace MentalMathGame.Views;

public partial class GameView : UserControl
{
    public GameView()
    {
        InitializeComponent();
        DataContextChanged += (_, _) =>
        {
            if (DataContext is GameViewModel vm)
                vm.PropertyChanged += OnVmPropertyChanged;
        };
    }

    // Remet le focus sur le champ de réponse à chaque nouvelle question
    private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GameViewModel.CurrentQuestion))
            Dispatcher.BeginInvoke(() => AnswerBox.Focus());
    }
}
