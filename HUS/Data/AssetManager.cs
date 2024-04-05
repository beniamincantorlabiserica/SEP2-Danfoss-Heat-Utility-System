using System.Collections.Generic;
using DefaultNamespace;

namespace HUS.Data;

public class AssetManager
{
    public List<HeatingAsset> Assets = new List<HeatingAsset>();
    
    /// <summary>
    /// Class constructor
    /// </summary>
    public AssetManager()
    {
        
    }
    
    /// <summary>
    /// Function that returns the available heating assets.
    /// </summary>
    /// <returns> A list of all the Heating Assets available</returns>
    public List<HeatingAsset> GetAssets()
    {
        LoadAssets();

        return Assets;
    }
    
    /// <summary>
    /// Loads hard-coded data for heating assets: Gas boiler, Oil boiler, Gas motor and Electric boiler in order
    /// </summary>
    private void LoadAssets()
    {
        Assets.Add(new HeatingAsset("GB", 5.0, 0.0,500,215,1.1));
        Assets.Add(new HeatingAsset("OB", 4.0, 0.0,700,265,1.2));
        Assets.Add(new HeatingAsset("GM", 3.6, 2.7,1100,645,1.9));
        Assets.Add(new HeatingAsset("EK", 8.0, -8.0,50,0,0));
        
    }

    /// <summary>
    /// Starts an individual asset
    /// </summary>
    /// <param name="asset"></param>
    public static void StartAsset(HeatingAsset asset)
    {
        asset.IsOperating = true;
    }
    
    /// <summary>
    /// Stops an individual asset
    /// </summary>
    /// <param name="asset"></param>
    public static void StopAsset(HeatingAsset asset)
    {
        asset.IsOperating = false;
    }
}