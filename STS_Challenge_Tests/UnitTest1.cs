using System.Collections;
using STS_Challenge;

namespace STS_Challenge_Tests;

public class UnitTest1
{
    [Theory]
    [ClassData(typeof(TestCases))]
    public void Test1(IEnumerable<string> input, uint cOfDates, IEnumerable<(DateTime,DateUsage)> testData)
    {
        Assert.Equal(cOfDates, (uint)testData.Count());

        IDateFinder finder = new DateFinder();

        var result = finder.FindDates(input).ToList();

        Assert.NotNull(result);
        Assert.NotEmpty(result);

        Assert.Equal(cOfDates, (uint) result.Count);

        foreach (var data in testData)
        {
            Assert.Contains(data, result);
        }
    }
}



public class TestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new[]
            {
                "kein Datum :o)",
                "07.05.1999",
                DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                DateTime.Now.AddHours(3).ToString("yyyy-MM-ddTHH:mm:ss"),
                DateTime.Now.AddMinutes(-5).ToString("yyyy-MM-ddTHH:mm:ss"),
            },
            4,
            new[]
            {
                (new DateTime(1999,5,07), DateUsage.BDay),
                (DateTime.Now, DateUsage.ValidFrom),
                (DateTime.Now.AddHours(3),  DateUsage.ValidTo),
                (DateTime.Now.AddMinutes(-5), DateUsage.SaleDate)
            }
        };

        yield return new object[]
        {
            new[]
            {
                "2023-05-01 00:00:02-2023-06-01 03:00:00"
            },
            2,
            new[]
            {
                (new DateTime(2023,5,1,0,0,2), DateUsage.ValidFrom),
                (new DateTime(2023,6,1,3,0,0), DateUsage.ValidTo),
            }
        };
        yield return new object[]
        {
            new[]
            {
                "Gültig 01.05.2023 00:00 - 31.05.2023 23:59"
            },
            2,
            new[]
            {
                (new DateTime(2023,5,1,0,0,0), DateUsage.ValidFrom),
                (new DateTime(2023,5,31,23,59,0), DateUsage.ValidTo),
            }
        };
        yield return new object[]
        {
            new[]
            {
                "01.05.2023 00:00 - 01.06.2023 00:00"
            },
            2,
            new[]
            {
                (new DateTime(2023,5,1,0,0,0), DateUsage.ValidFrom),
                (new DateTime(2023,6,1,0,0,0), DateUsage.ValidTo),
            }
        };
        yield return new object[]
        {
            new[]
            {
                "1.Oktober 2023 00:00 Uhr bis 31.Oktober 23:59 Uhr"
            },
            2,
            new[]
            {
                (new DateTime(2023,10,1,0,0,0), DateUsage.ValidFrom),
                (new DateTime(2023,10,31,23,59,0), DateUsage.ValidTo),
            }
        };
        yield return new object[]
        {
            new[]
            {
                "2023",
                "01.05",
                "00:00",
                "01.06",
                "03:00"
            },
            2,
            new[]
            {
                (new DateTime(2023,5,1,0,0,0), DateUsage.ValidFrom),
                (new DateTime(2023,6,1,3,0,0), DateUsage.ValidTo),
            }
        };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}