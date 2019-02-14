using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TimeTable_API.Models;


namespace TimeTable_API.Filters
{
    // Фильтр, который будет проверять входящие значения Lesson
    public class DataEditCheckAttribute : Attribute, IActionFilter
    {
        private readonly LessonContext _DbContext;

        public DataEditCheckAttribute(LessonContext Context)
        {
            _DbContext = Context;
        }

        // Фильтр, который выполняется после выполнения метода. В данном случае он будет пустым
        public void OnActionExecuted(ActionExecutedContext context)
        { }

        // Фильтр, который выполняется до выполнения метода
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var lesson = context.ActionArguments["lesson"] as Lesson; // Получаем данные из тела запроса
            if (lesson == null || lesson.StartTime > lesson.EndTime || _DbContext.Lessons.Any(l => !l.Check(lesson) && l.id != lesson.id)) // Проверка
                context.Result = new BadRequestResult();
        }
    }
}