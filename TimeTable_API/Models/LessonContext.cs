using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TimeTable_API.Models
{
    public class LessonContext : DbContext
    {

        public DbSet<Lesson> Lessons { get; set; }

        public LessonContext (DbContextOptions<LessonContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

    }
}
