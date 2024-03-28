using System;

namespace HUS;

public class Utils
{
    public const bool IsUnderDevelopment = true; // turn false for project build or to switch off console prints about the optimizer
    
    public static void Dev(string str)
    {
        if (IsUnderDevelopment)
            Console.WriteLine($"Developer log: ~ {str}");
    }
}