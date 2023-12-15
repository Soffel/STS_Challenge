using System.Collections;
using System.Diagnostics;
using STS_Challenge;
using Xunit.Abstractions;

namespace STS_Challenge_Tests;

public class UnitTest1
{
    private readonly ITestOutputHelper _log;

    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        _log = testOutputHelper;
    }

    [Theory]
    [ClassData(typeof(TestCases))]
    public void DateFinderTest(IEnumerable<string> input, uint cOfDates, IEnumerable<(DateTime, DateUsage)> testData)
    {
        Assert.Equal(cOfDates, (uint)testData.Count());

        IDateFinder finder = new DateFinder();

        Stopwatch stopwatch = Stopwatch.StartNew();
        var result = finder.FindDates(input).ToList();
        stopwatch.Stop();
        _log.WriteLine("needs: " + stopwatch.ElapsedTicks);

        Assert.NotNull(result);
        Assert.NotEmpty(result);

        if(cOfDates !=  (uint)result.Count)
        {
            foreach(var date in result)
            {
                _log.WriteLine(date.ToString());
            }
            Assert.Fail("Falsche anzhal!");
        }

        foreach (var data in testData)
        {
            Assert.Contains(data, result);
        }
    }


    [Theory]
    [ClassData(typeof(TestCases))]
    public void SimpleDevTest(IEnumerable<string> input, uint cOfDates, IEnumerable<(DateTime, DateUsage)> testData)
    {     
        IDateFinder finder = new DateFinder();

        var result = finder.FindDates(input).ToList();

        var _ = result;

        _log.WriteLine(result.Count.ToString());
       
    }

    [Fact]
    public void FormatTest()
    {
        string[] dayFormats =
        {
            "dd.MM",           
            "d.MM",            
            "dd.MMM",          
            "d.MMM",           
            "dd.MMMM",         
            "d.MMMM",          
        };

        var date = new DateTime(2023, 10, 1);

        foreach (var dayFormat in dayFormats)
        {
            
            _log.WriteLine(dayFormat+ " >> " + date.ToString(dayFormat));
        }

    }
}



public class TestCases : IEnumerable<object[]>
{
    DateTime Now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new[]
            {
                "kein Datum :o)",
                "07.05.1999",
                Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                Now.AddHours(3).ToString("yyyy-MM-ddTHH:mm:ss"),
                Now.AddMinutes(-5).ToString("yyyy-MM-ddTHH:mm:ss"),
            },
            4,
            new[]
            {
                (new DateTime(1999,5,07), DateUsage.BDay),
                (Now, DateUsage.ValidFrom),
                (Now.AddHours(3),  DateUsage.ValidTo),
                (Now.AddMinutes(-5), DateUsage.SaleDate)
            }
        };

        yield return new object[]
        {
            new[]
            {
                "2023-05-01 00:00:02 - 2023-06-01 03:00:00"
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
        yield return new object[]
        {
            new[]
            {
                "1.Oktober 2023 00:00 Uhr bis 31.Oktober 23:59 Uhr",
                "30.Sep 2023"
            },
            3,
            new[]
            {
                (new DateTime(2023,10,1,0,0,0), DateUsage.ValidFrom),
                (new DateTime(2023,10,31,23,59,0), DateUsage.ValidTo),
                (new DateTime(2023,09,30,0,0,0), DateUsage.SaleDate),

            }
        };
        yield return new object[]
        {
            new[]
            {
                "25.04.1991",
                "1.Oktober 2023 00:00 Uhr bis 31.Oktober 23:59 Uhr",
                "30.Sep 2023"
            },
            4,
            new[]
            {
                (new DateTime(2023,10,1,0,0,0), DateUsage.ValidFrom),
                (new DateTime(2023,10,31,23,59,0), DateUsage.ValidTo),
                (new DateTime(2023,09,30,0,0,0), DateUsage.SaleDate),
                (new DateTime(1991,04,25,0,0,0), DateUsage.BDay),

            }
        };
        yield return new object[]
       {
            new[]
            {
                "25.04.1991",
                "1.Oct 2023 00:00 Uhr bis 31.Oktober 23:59 Uhr",
                "30.Sep 2023"
            },
            4,
            new[]
            {
                (new DateTime(2023,10,1,0,0,0), DateUsage.ValidFrom),
                (new DateTime(2023,10,31,23,59,0), DateUsage.ValidTo),
                (new DateTime(2023,09,30,0,0,0), DateUsage.SaleDate),
                (new DateTime(1991,04,25,0,0,0), DateUsage.BDay),

            }
       };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}