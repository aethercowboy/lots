using lots.Domain.Models;
using System;
using System.Collections.Generic;

namespace lots.BusinessLogic.Interfaces
{
    public interface IRatingService
    {
        IEnumerable<MeasuredItem> Rate(string path, int limit, Func<MeasuredItem, MeasuredItem, int> selectFunc);
    }
}
