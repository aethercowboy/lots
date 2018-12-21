using lots.Domain.Models;
using System;
using System.Collections.Generic;

namespace lots.BusinessLogic.Interfaces
{
    public interface IRatingService
    {
        IEnumerable<MeasuredItem> Rate(IEnumerable<MeasuredItem> items, int limit, Func<MeasuredItem[], int> selectFunc);
    }
}
