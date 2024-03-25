using System.Collections.Generic;
using DefaultNamespace;

namespace HUS.Data;

public class AssetManager
{
    public List<HeatingAsset> Assets = new List<HeatingAsset>();

    public AssetManager()
    {
        
    }
    
    public List<HeatingAsset> GetAssets()
    {
        LoadAssets();

        return Assets;
    }

    private void LoadAssets()
    {
        /*Hard-coded data for heating assets, Gas boiler, Oil boiler, Gas motor and Electric boiler in order*/
        Assets.Add(new HeatingAsset("GB", 5.0, 0.0,500,215,1.1));
        Assets.Add(new HeatingAsset("OB", 4.0, 0.0,700,265,1.2));
        //Assets.Add(new HeatingAsset("GM", 3.6m, 2.7m,1100,645,1.9m));
        //Assets.Add(new HeatingAsset("EK", 8.0m, -8.0m,50,0,0m));
        
    }

    public static void StartAsset(HeatingAsset asset)
    {
        asset.IsOperating = true;
    }
    
    public static void StopAsset(HeatingAsset asset)
    {
        asset.IsOperating = false;
    }
}