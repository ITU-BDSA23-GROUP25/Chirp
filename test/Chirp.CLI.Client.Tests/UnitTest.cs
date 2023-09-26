namespace Chirp.CLI.Client.Tests;

using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using SimpleDB;
using UI;
using Xunit;

public class UnitTest
{

    private readonly UserInterface _userinterface;
    private readonly DB _DB;

    public UnitTest()
    {
        _userinterface = new UserInterface();
        _DB = DB.Instance;
    }


    #region chirp.cli_TestCode
    [Theory]
    [InlineData("1690891760", "08/01/23 14.09.20")]
    [InlineData("1690978778", "08/02/23 14.19.38")]
    [InlineData("1690979858", "08/02/23 14.37.38")]
    [InlineData("1690981487", "08/02/23 15.04.47")]

    public void Convert_Time_FromUnix_ToDate_(string value, string expected)
    {
        //Arrange

        // ACt

        var result = _userinterface.timeConverter(Convert.ToDouble(value));

        //Arssert

        Assert.Equal(result, expected);
    }
    #endregion



    [Theory]
    [InlineData("ropf","Hello, BDSA students!","1690891760", "ropf @ 08/01/23 14.09.20: Hello, BDSA students!")]
    [InlineData("rnie","Welcome to the course!","1690978778", "rnie @ 08/02/23 14.19.38: Welcome to the course!")]
    [InlineData("rnie","I hope you had a good summer.","1690979858", "rnie @ 08/02/23 14.37.38: I hope you had a good summer.")]
    [InlineData("ropf","Cheeping cheeps on Chirp :)","1690981487","ropf @ 08/02/23 15.04.47: Cheeping cheeps on Chirp :)")]
    public void Print_Cheeps(String A, String M, string T, String expected)
    {
        //Arrange

        Cheep c = new()
        {
            Author = A,
            Message = M,
            Timestamp = T,
        };

        //Act

        var result = _userinterface.convert_toString(c);

        //Assert
        Assert.Equal(result, expected);
    }
}
