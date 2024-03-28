using System.Collections.Generic;

namespace DefaultNamespace;

public class ReturnOptimizedData
{
    public DataPerHour Hour { get; set; }
    public List<HeatingAsset> AssetsUsed { get; set; }

    public ReturnOptimizedData(DataPerHour hour, List<HeatingAsset> assets)
    {
        Hour = hour;
        AssetsUsed = assets;
    }
}