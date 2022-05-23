using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRASECrawler.Model;

namespace TRASECrawler.Service
{
    public interface IDictionaryService
    {
        List<City> GetCities();
        List<Category> GetCategories();
        Nullable<DateTime> ConvertStringToDate(string textDate);
    }
}
