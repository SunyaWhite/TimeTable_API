using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTable_API.Models;

namespace TimeTable_API.Services
{
    public interface ITimeTableParser
    {
        Task<List<Lesson>> GetData(string group, int weekNum);
    }
}
