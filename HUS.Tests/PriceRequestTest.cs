namespace HUS.Tests;

public class PriceRequestTest
{
    private PriceRequest _priceRequest = new PriceRequest();
    
    
    [Theory]
    [InlineData("1/5/2024 15:00", 746)]
    [InlineData("1/10/2024 15:00", 923)]
    [InlineData("2/5/2024 15:00", 415)]
    [InlineData("2/10/2024 15:00", 506)]
    [InlineData("3/5/2024 15:00", 444)]
    [InlineData("3/10/2024 15:00", 107)]
    [InlineData("4/5/2024 15:00", 176)]
    [InlineData("4/10/2024 15:00", 2)]
    [InlineData("5/5/2024 15:00", 0)]
    [InlineData("5/10/2024 15:00", 152)]
    public void GetElectricityPriceTest(DateTime date, double expected)
    {
        double result = _priceRequest.GetElectricityPrice(date);
        // Assert
        Assert.Equal(expected, (int)result);
    }
}