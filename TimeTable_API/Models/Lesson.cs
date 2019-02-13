using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TimeTable_API.Models
{
    public class Lesson
    {
        [Key]
        public Guid id { get; set; } // id элемента Lesson
        [Required]
        public string Name { get; set; } // название предмета
        [Required]
        //[DisplayFormat(DataFormatString ="{dd/MM/yyyy HH:mm}", ApplyFormatInEditMode =true)] // Проверить как будет работать при выдаче результата
        public DateTime StartTime { get; set; } // время начала предмета
        [Required]
        public DateTime EndTime { get; set; } // время конца пары Добавиь аттрибут Remote
        [Required]
        public bool UserAdded { get; set; } // добавлено пользователем

        public int WeekNum { get; set; } // неделя, на которой будет предмет

        public string Location { get; set; } // место проведения пары

        public string Lecturer { get; set; } // преподаватель

        public string Homework { get; set; } // ДЗ

        // Проверяем, что данные элементы одинаковы
        //public bool Equals(Lesson l) => l.EndTime.Equals(this.EndTime) && l.StartTime.Equals(this.StartTime) && l.Name.Equals(this.Name);

        // Проверяем, что временные отрезки данных элементов Lesson не пересекаются
        public bool Check(Lesson l) => l.EndTime < this.StartTime ^ this.EndTime < l.StartTime; 

    }
}
