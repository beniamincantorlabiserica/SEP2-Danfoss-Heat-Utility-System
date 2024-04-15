using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HUS.Data;
using HUS.Model;

namespace HUS;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
    }
}