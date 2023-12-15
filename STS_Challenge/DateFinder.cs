using System;
using System.Globalization;
using System.Runtime.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace STS_Challenge;

public class DateFinder : IDateFinder
{
    public DateFinder() 
    { 
        year = 0;
        date = DateTime.MinValue;
        time = DateTime.MinValue;
    }

    private int year;
    private DateTime date;
    private DateTime time;

    readonly CultureInfo[] cultures =
    {
        null,
        CultureInfo.InvariantCulture,
        CultureInfo.CurrentCulture,
        CultureInfo.GetCultureInfo("de-DE"),
    };

    readonly string[] dateTimeFormats =
    {
        "dd.MM.yyyy HH:mm:ss",   
        "dd.MM.yyyy HH:mm",      
        "d.MM.yyyy HH:mm:ss",    
        "d.MM.yyyy HH:mm",       
        "dd.MM.yy HH:mm:ss",     
        "dd.MM.yy HH:mm",        
        "d.MM.yy HH:mm:ss",      
        "d.MM.yy HH:mm",         
        "dd.MMMM yyyy HH:mm:ss", 
        "dd.MMMM yyyy HH:mm",    
        "d.MMMM yyyy HH:mm:ss",  
        "d.MMMM yyyy HH:mm",     
        "dd.MMMM yy HH:mm:ss",   
        "dd.MMMM yy HH:mm",      
        "d.MMMM yy HH:mm:ss",    
        "d.MMMM yy HH:mm",       

        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-ddTHH:mm",


    };

    readonly string[] dateFormats =
    {
        "dd.MM.yyyy",            
        "d.MM.yyyy",             
        "dd.MM.yy",              
        "d.MM.yy",               
        "dd.MMMM yyyy",            
        "d.MMMM yyyy",           
        "dd.MMMM yy",                
        "d.MMMM yy",             

        "yyyy-MM-dd",
    };

    readonly string[] dayFormats =
    {
        "dd.MM",           
        "d.MM",            
        "dd.MMM",          
        "d.MMM",           
        "dd.MMMM",         
        "d.MMMM",          
    };

    readonly string[] timeFormats =
    {
        "HH:mm:ss",   
        "HH:mm",      
    };

    private void Reset()
    {
        date = DateTime.MinValue;
        time = DateTime.MinValue;
    }

    public IEnumerable<(DateTime, DateUsage)> FindDates(IEnumerable<string> inputs)
    {
        var listOfDates = new List<DateTime>();

        foreach (string input in inputs)
        {
            foreach(string split in input.Split(' '))
            {
                if(isDateTime(split, out DateTime dt))
                {                
                    listOfDates.Add(dt);
                    continue;          
                }

                if(IsDate(split, out DateTime c))
                {          
                    if(date != DateTime.MinValue && date.Year != DateTime.MinValue.Year)
                    {
                        listOfDates.Add(date);
                        Reset();
                    }

                    date = new DateTime(c.Year, c.Month, c.Day);

                    if (time != DateTime.MinValue && date.Year != DateTime.MinValue.Year)
                    {
                        time = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
                        listOfDates.Add(time);
                        Reset();
                    }
                    continue;
                }

                if (IsYear(split, out int y))
                {
                    if(year == 0)
                    { 
                        year = y; 

                        if(date != DateTime.MinValue && time != DateTime.MinValue)
                        {
                            date = new DateTime(year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
                            listOfDates.Add(date);
                            Reset();    
                        }
                        else
                        {
                            if (date != DateTime.MinValue)
                            {
                                date = new DateTime(year, date.Month, date.Day);
                            }

                            if (time != DateTime.MinValue)
                            {
                                time = new DateTime(year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
                            }
                        }    
                    }
                    continue;
                }
                   

                if (IsDayMonth(split, out DateTime d))
                {
                    if (date != DateTime.MinValue && date.Year != DateTime.MinValue.Year)
                    {
                        listOfDates.Add(date);
                        Reset();
                    }
                    date = d;

                    date = new DateTime(year != 0 ? year : DateTime.MinValue.Year, date.Month, date.Day);

                    if (time != DateTime.MinValue && date.Year != DateTime.MinValue.Year)
                    {
                        time = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
                        listOfDates.Add(time);
                        Reset();
                    }
                    
                    continue;
                }

                if(IsTime(split, out DateTime t))
                {
                    time = t;
                    if(date != DateTime.MinValue && date.Year != DateTime.MinValue.Year)
                    {
                        date = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
                        listOfDates.Add(date);
                        Reset();       
                    }
                    continue;
                }

                var o = split;
            }
        }

        if (date != DateTime.MinValue && date.Year != DateTime.MinValue.Year)
        {
            listOfDates.Add(date);
            Reset();
        }

        listOfDates.Sort();

        switch (listOfDates.Count)
        {
            case 1:
            {
                    yield return (listOfDates[0], DateUsage.ValidFrom);
                    break;
            }
            case 2:
            {
                    yield return (listOfDates[0], DateUsage.ValidFrom);
                    yield return (listOfDates[1], DateUsage.ValidTo);
                    break;
            }
            case 3:
            {
                    yield return (listOfDates[0], IsBirthOrSale(listOfDates[0]));
                    yield return (listOfDates[1], DateUsage.ValidFrom);
                    yield return (listOfDates[2], DateUsage.ValidTo);
                    
                    break;
            }
            case 4:
            {
                    yield return (listOfDates[0], DateUsage.BDay);
                    yield return (listOfDates[1], DateUsage.SaleDate);
                    yield return (listOfDates[2], DateUsage.ValidFrom);
                    yield return (listOfDates[3], DateUsage.ValidTo);
                    break;
            }
            default:
            {
                    if (listOfDates.Count > 5)
                    {
                        yield return (listOfDates[^1], DateUsage.ValidTo);
                        yield return (listOfDates[^2], DateUsage.ValidFrom);
                        
                        foreach (DateTime r in listOfDates.SkipLast(2))
                        {
                            yield return (r, DateUsage.DontKnow);
                        }

                        break;
                    }
                    else yield break; 
            }
        }
    }

    private static DateUsage IsBirthOrSale(DateTime date)
    {
        if ((DateTime.Now - date).TotalDays > 400) // evtl Vorverkauf 
            return DateUsage.BDay;

        return DateUsage.SaleDate;
    }

    private bool IsYear(string input, out int year)
    {
        year = 0;
        foreach(var cultur in cultures)
        {
            if (DateTime.TryParseExact(input, "yyyy", cultur, DateTimeStyles.None, out DateTime parsedYear))
            {

                if (parsedYear.Year > DateTime.Now.AddYears(-100).Year &&
                    parsedYear.Year < DateTime.Now.AddYears(2).Year)
                {
                    year = parsedYear.Year;
                    return true;
                }
            }
        }
       
        return false;    
    }

    private bool IsDayMonth(string input, out DateTime date)
    {
        date = DateTime.MinValue;

        foreach (var cultur in cultures)
            foreach (var format in dayFormats)
            {
                if (DateTime.TryParseExact(input, format, cultur, DateTimeStyles.None, out DateTime extractedDate))
                {
                   date = extractedDate;
                    return true;
                }
            }


        return false; 
    }

    private bool IsDate (string input, out DateTime date)
    {
        date = DateTime.MinValue;


        foreach (var cultur in cultures)
            foreach (var format in dateFormats)
            {
                if (DateTime.TryParseExact(input, format, cultur, DateTimeStyles.None, out DateTime extractedDate))
                {
                    date = extractedDate;
                    return true;
                }
            } 
       
        return false;
    }

    private bool isDateTime(string input, out DateTime dateTime)
    {
        dateTime = DateTime.MinValue;

        foreach(var cultur in cultures)
            foreach (var format in dateTimeFormats)
            {
                if (DateTime.TryParseExact(input, format, cultur, DateTimeStyles.None, out DateTime extractedDate))
                {
                    dateTime = extractedDate;
                    return true;
                }
            }
        
        return false;
    }

    private bool IsTime(string input, out DateTime time)
    {
        time = DateTime.MinValue;
        foreach (var cultur in cultures)
            foreach (var format in timeFormats)
            {
                if (DateTime.TryParseExact(input, format, cultur, DateTimeStyles.None, out DateTime extractedDate))
                {
                    time = extractedDate;
                    return true;
                }
            } 
       
        return false; 
    }
}