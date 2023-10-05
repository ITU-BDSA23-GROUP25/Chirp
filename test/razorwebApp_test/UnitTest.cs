namespace razor_test;

using Xunit;

using Chirp.Razor;

public class UnitTest
{
   private readonly CheepService _cheepservice;

    public UnitTest()
    {
        _cheepservice = new CheepService();
    }

    [Theory]
    [InlineData("1690891760", "08/01/23 14.09.20")]
    [InlineData("1690978778", "08/02/23 14.19.38")]
    [InlineData("1690979858", "08/02/23 14.37.38")]
    [InlineData("1690981487", "08/02/23 15.04.47")]

    public void Convert_Time_FromUnix_ToDate_(string value, string expected)
    {
        //Arrange

        // ACt

        var result = _cheepservice.UnixTimeStampToDateTimeString(Convert.ToDouble(value));

        //Arssert

        Assert.Equal(result, expected);
    }
}