using XmlArchiveApp.ViewModels;

namespace XmlArchiveApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainViewModel();
    }
}
