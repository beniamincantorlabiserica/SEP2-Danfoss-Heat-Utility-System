using Avalonia.Controls;

namespace HUS;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ExcelLoader excelLoader = new ExcelLoader();
    }
}