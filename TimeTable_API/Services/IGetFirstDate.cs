using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTable_API.Services
{
    public interface IGetFirstDate
    {
        Task<DateTime> GetDate(string group);
    }
}
