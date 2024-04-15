using Avalonia.Controls;
using DefaultNamespace;
using HUS.Data;

namespace HUS;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        OptimizerManager optimizerManager = new OptimizerManager();
    }
}