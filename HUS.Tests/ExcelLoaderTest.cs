using Google.Protobuf.WellKnownTypes;
using HUS.Data;
using HUS.Model;

namespace HUS.Tests;

public class ExcelLoaderTest
{
    private readonly ExcelLoader _loader = new ExcelLoader();
    
    [Fact]
    public void LoaderGetData()
    {
        var dataList = _loader.GetData();
        Assert.NotEmpty(dataList);

        foreach (var data in dataList)
            Assert.NotNull(data);
        
        Assert.Equal(336, dataList.Count);
        
    }
    
}
