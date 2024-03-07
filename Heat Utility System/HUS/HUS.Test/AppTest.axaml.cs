using Avalonia;
using Avalonia.Headless;
using HUS;

namespace HUS.Test;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
