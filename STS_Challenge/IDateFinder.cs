using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STS_Challenge
{
    public interface IDateFinder
    {
        IEnumerable<(DateTime, DateUsage)> FindDates(IEnumerable<string> input);
    }

    public enum DateUsage
    {
        ValidFrom,
        ValidTo,
        BDay,
        SaleDate, 
        DontKnow
    }
}
