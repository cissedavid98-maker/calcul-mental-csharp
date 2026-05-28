using System.Windows.Controls;

namespace MentalMathGame.Services;

public class NavigationService
{
    private ContentControl? _host;

    public void Initialize(ContentControl host)
    {
        _host = host;
    }

    public void NavigateTo(UserControl view)
    {
        if (_host != null)
            _host.Content = view;
    }
}
