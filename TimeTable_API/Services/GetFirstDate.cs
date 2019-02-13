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
    public class GetFirstDate : IGetFirstDate
    {
        public async Task<DateTime> GetDate(string group)
        {
            HttpClient client = new HttpClient();
            UriBuilder uriBuilder = new UriBuilder("https://mai.ru/education/schedule/detail.php");  // url с расписанием
            uriBuilder.Query = $"group={group}&week={1}"; // добавляем query
            HttpResponseMessage response = await client.GetAsync(uriBuilder.Uri); // Получаем ответ
            response.EnsureSuccessStatusCode();
            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(await response.Content.ReadAsStreamAsync()); // загружаем полученную страницу
            var root = htmlDocument.DocumentNode;
            var days = root.QuerySelectorAll(".sc-table-day"); // получаем ячейки с днями
            var date = DateTime.ParseExact(days.First().QuerySelector(".sc-day-header").InnerText.Substring(0, 5), "dd.MM", System.Globalization.CultureInfo.InvariantCulture); // получаем дату
            date = date.AddDays(-((int)(date.DayOfWeek)) + 1);
            return date;
        }
    }
}