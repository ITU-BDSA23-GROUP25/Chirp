using SimpleDB;

public class IntegrationTest1{

[Fact]
public void IsDataInDbAsExpectedTest()
{
    //Arrange
        Class1 x = new("test_db.csv");
        Cheep c = new()
        {
            Author = Class1.getUsername(),
            Message = "Hello World!",
            Timestamp = Class1.getUNIXTime()
        };

        //Act
        string actual = c.ToString();
        x.Store(c);

        var Inumerable = x.Read();
        string? expected = Inumerable.Last().ToString();
        Console.WriteLine($"actual: {actual}");
        Console.WriteLine($"actual: {expected}");
        //Assert

        Assert.Equal(expected, actual);
    }
}