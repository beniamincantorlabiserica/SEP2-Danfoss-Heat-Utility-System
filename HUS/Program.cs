using Avalonia;
using System;
using System.Collections.Generic;
using DefaultNamespace;
using HUS.Data;

namespace HUS;

class Program
{
    
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    
    
    public static void Main(string[] args)
    {
        
        ExcelLoader loader = new();
        AssetManager assets = new AssetManager();
        Optimizer optimizer = new Optimizer(loader.GetData(), assets.GetAssets());

        optimizer.PrintOriginAssets();
        optimizer.PrintOptimizedAssets();
        
        loader.WriteDataToConsole(loader.GetData());
        
        optimizer.StartThread();

       
        /*
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
        */
        
    }

/*
    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    
    /*little dev playground*/
}
