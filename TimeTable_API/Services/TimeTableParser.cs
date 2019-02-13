using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using System.Net.Http;
using TimeTable_API.Models;

namespace TimeTable_API.Services
{

    // Парсер HTML-страницы с расписанием
    public class TimetableParser : ITimeTableParser
    {

        public async Task<List<Lesson>> GetData(string group, int weekNum) // Таск для 
        {
            var Result = new List<Lesson>();
            using (HttpClient client = new HttpClient())
            {
                UriBuilder uriBuilder = new UriBuilder("https://mai.ru/education/schedule/detail.php");  // url с расписанием
                uriBuilder.Query = $"group={group}&week={weekNum}"; // добавляем query
                HttpResponseMessage response = await client.GetAsync(uriBuilder.Uri); // Получаем ответ
                response.EnsureSuccessStatusCode();
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.Load(await response.Content.ReadAsStreamAsync()); // загружаем полученную страницу
                var root = htmlDocument.DocumentNode;
                var days = root.QuerySelectorAll(".sc-table-day");
                foreach (var day in days)
                {
                    var date = day.QuerySelector(".sc-day-header").InnerText.Substring(0, 5); // дата
                    var rows = day.QuerySelectorAll(".sc-table-row").ToList();
                    rows.RemoveAt(0); // Строку не удалять! Баг в Fizzler или HtmlAgiltyPack. Создает 2 первых элемента
                    foreach (var elem in rows)
                    {
                        var hours = elem.QuerySelector(".sc-item-time").InnerText.Split(" &ndash; "); // время пары !
                        var location = elem.QuerySelector(".sc-item-location").InnerText.TrimStart('&', 'n', 'b', 's', 'p', ';'); // место пары
                        var lecturerNode = elem.QuerySelector(".sc-lecturer"); // нода с преподом. Может быть null и приводить к ошибке
                        var lecturer = (lecturerNode == null) ? "" : lecturerNode.InnerText; // Преподаватель
                        Result.Add(new Lesson
                        {
                            StartTime = DateTime.ParseExact(date + "." + hours[0], "dd.MM.HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                            EndTime = DateTime.ParseExact(date + "." + hours[1], "dd.MM.HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                            UserAdded = false,
                            WeekNum = weekNum,
                            Location = location,
                            Lecturer = lecturer,
                            Name = elem.QuerySelector(".sc-title").InnerText
                        });
                    }
                }
            }
            return Result;
        }

    }
}
