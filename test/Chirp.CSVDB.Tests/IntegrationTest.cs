using SimpleDB;

public class IntegrationTest1{


public void IsDataInDbAsExpectedTest()
{
    //Arrange
        Class1 x = Class1.Instance;
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

        Assert.Equal(actual, expected);
    }
}