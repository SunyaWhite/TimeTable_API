using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TimeTable_API.Models;
using TimeTable_API.Services;
using TimeTable_API.Filters;

namespace TimeTable_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TimeTableController : ControllerBase
    {

        private LessonContext _Context { get; set; }
        private IMemoryCache _Cache { get; set; }

        public TimeTableController( LessonContext context, IMemoryCache cache, IGetFirstDate getFirstDate)
        {
            _Context = context;
            _Cache = cache;
            string stringDate;
            if (!_Cache.TryGetValue("FirstDate", out stringDate))
                _Cache.Set("FirstDate", getFirstDate.GetDate("М4О-209Б-17").Result.ToString("d", System.Globalization.CultureInfo.CreateSpecificCulture("es-ES")));
        }

        // Возвращает все данные, которые есть в локальной БД
        [HttpGet]
        public IActionResult GetAllLessons() => Ok(_Context.Lessons.OrderBy(l => l.StartTime));

        // Возвращает массив из Lesson, у которых свойство WeekNum равно заданному значению
        [HttpGet("{weekNum}")]
        public IActionResult GetAllLessonsByWeekNum([FromServices] ITimeTableParser parser, int weekNum)
        {
            if (weekNum < 1 || weekNum > 22)
                return BadRequest();
            if (_Context.Lessons.FirstOrDefault(l => l.WeekNum == weekNum && !l.UserAdded) == null)
            {
                _Context.Lessons.AddRange(parser.GetData("М4О-209Б-17", weekNum).Result);
                _Context.SaveChanges();
            }
            return Ok(_Context.Lessons.Where(l => l.WeekNum == weekNum).OrderBy(l => l.StartTime));
        }

        // Добавляет в базу данных новый элемент Lesson
        [HttpPost]
        [ServiceFilter(typeof(DataAddCheckAttribute))] // Передача сервисов (DbContext) в фильтр. Вся проверка реализована в классе DataCheckAttribute в папке Filters
        public IActionResult AddNewLesson([FromBody] Lesson lesson)
        {
            var firstDate = DateTime.ParseExact(_Cache.Get<string>("FirstDate"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture); // Измение номера недели
            lesson.WeekNum = lesson.StartTime.Subtract(firstDate).Days / 7 + 1; // Измение номера недели
            lesson.UserAdded = true; // Указываем, что данный параметр добавлен пользователем
            _Context.Lessons.Add(lesson);
            _Context.SaveChanges();
            return Ok();
        }

        // Вносит изменение в существующий элемент Lesson
        [HttpPut]
        [ServiceFilter(typeof(DataEditCheckAttribute))] // Передача сервисов (DbContext) в фильтр. Вся проверка реализована в классе DataCheckAttribute в папке Filters
        public IActionResult EditLesson([FromBody] Lesson lesson)
        {
            var firstDate = DateTime.ParseExact(_Cache.Get<string>("FirstDate"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture); // Измение номера недели
            lesson.WeekNum = lesson.StartTime.Subtract(firstDate).Days / 7 + 1; // Измение номера недели
            lesson.UserAdded = true; // Указываем, что данный параметр добавлен пользователем
            _Context.Lessons.Update(lesson);
            _Context.SaveChanges();
            return Ok();
        }

        // Удаляет элемент Lesson по заданному id
        [HttpDelete("{id}")]
        public IActionResult DeleteLesson( Guid id)
        {
            if (id == null || !_Context.Lessons.Any(l => l.id == id))
                return BadRequest();
            var lesson = _Context.Lessons.Find(id);
            if (lesson.Homework != null) // Передача свойства Homework  1-ого элемента Lesson  2-ому элементу Lesson при удалении 1-ого элемента 
            {
                var newLesson = _Context.Lessons.FirstOrDefault(l => l.Name == lesson.Name && l.StartTime > lesson.StartTime && l.Homework == null);
                if(newLesson != null)
                    newLesson.Homework = lesson.Homework;
            }
            _Context.Lessons.Remove(lesson);
            _Context.SaveChanges();
            return Ok();
        }
    }
}