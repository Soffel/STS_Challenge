namespace STS_Challenge;

public class DateFinder : IDateFinder
{
    public IEnumerable<(DateTime, DateUsage)> FindDates(IEnumerable<string> input)
    {
        return new []{(DateTime.Now, DateUsage.BDay)};
    }
}