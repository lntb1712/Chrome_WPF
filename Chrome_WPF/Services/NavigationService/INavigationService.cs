using System.Windows.Controls;

namespace Chrome_WPF.Services.NavigationService
{
    public interface INavigationService
    {
        void NavigateTo(UserControl userControl);
        void NavigateTo<T>() where T : UserControl;
    }
}
