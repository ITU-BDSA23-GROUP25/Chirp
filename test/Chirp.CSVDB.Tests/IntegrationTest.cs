using SimpleDB;

public class IntegrationTest1{

[Fact]
public void IsDataInDbAsExpectedTest()
{
    //Arrange
        DB x = DB.Instance("../../../../Chirp.CSVDB.Tests/test_db.csv");
        Cheep c = new()
        {
            Author = DB.getUsername(),
            Message = "Hello World!",
            Timestamp = DB.getUNIXTime()
        };

        //Act
        string actual = c.ToString();
        x.Store(c);

        var Inumerable = x.Read();
        string? expected = Inumerable.Last().ToString();
        Console.WriteLine($"actual: {actual}");
        Console.WriteLine($"actual: {expected}");
        //Assert

        Assert.Equal(actual, expected);
    }
}