using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRASECrawler.Model;

namespace TRASECrawler.Service
{
    public class DictionaryService : IDictionaryService
    {
        //this method returns the categories according to trip advisor infos
        public List<Category> GetCategories()
        {
            var categories = new List<Category>();

            categories.Add(new Category { Code = "c57-t70", Name = "Park" });
            categories.Add(new Category { Code = "c49-t161", Name = "Özel Müzeler" });
            categories.Add(new Category { Code = "c47-t26", Name = "Anıtlar ve Heykeller" });
            categories.Add(new Category { Code = "c26-t143", Name = "Avm" });
            categories.Add(new Category { Code = "c58-t116", Name = "Tiyatro" });
            categories.Add(new Category { Code = "c47-t10", Name = "Kutsal mekan" });
            categories.Add(new Category { Code = "c47-t17", Name = "Tarihi yer" });
            categories.Add(new Category { Code = "c57-t58", Name = "Bahçe" });
            categories.Add(new Category { Code = "c49-t30", Name = "Tarihi Müze" });
            categories.Add(new Category { Code = "c47-t163", Name = "Merkezi Nokta" });
            categories.Add(new Category { Code = "c57-t57", Name = "Orman" });
            categories.Add(new Category { Code = "c47-t175", Name = "Klise-Katedral" });
            categories.Add(new Category { Code = "c61-t52", Name = "Plajlar" });

            return categories;
        }

        //this method returns the cities in Marmara region
        public List<City> GetCities()
        {
            var cities = new List<City>();
            cities.Add(new City { Name = "Balıkesir", Code = "g297974" });
            cities.Add(new City { Name = "Bilecik", Code = "g3183767" });
            cities.Add(new City { Name = "Bursa", Code = "g297977" });
            cities.Add(new City { Name = "Çanakkale", Code = "g297979" });
            cities.Add(new City { Name = "Edirne", Code = "g652369" });
            cities.Add(new City { Name = "İstanbul", Code = "g293974" });
            cities.Add(new City { Name = "Kocaeli", Code = "g788070" });
            cities.Add(new City { Name = "Kırklareli", Code = "g1202846" });
            cities.Add(new City { Name = "Sakarya", Code = "g612462" });
            cities.Add(new City { Name = "Tekirdağ", Code = "g781293" });
            cities.Add(new City { Name = "Yalova", Code = "g298043" });

            return cities;
        }

        public Nullable<DateTime> ConvertStringToDate(string textDate)
        {
            try
            {
                int month = 0; int year = 0;
                if (textDate.Contains("Ocak")) month = 1;
                else if (textDate.Contains("Şubat")) month = 2;
                else if (textDate.Contains("Mart")) month = 3;
                else if (textDate.Contains("Nisan")) month = 4;
                else if (textDate.Contains("Mayıs")) month = 5;
                else if (textDate.Contains("Haziran")) month = 6;
                else if (textDate.Contains("Temmuz")) month = 7;
                else if (textDate.Contains("Ağustos")) month = 8;
                else if (textDate.Contains("Eylül")) month = 9;
                else if (textDate.Contains("Ekim")) month = 10;
                else if (textDate.Contains("Kasım")) month = 11;
                else if (textDate.Contains("Aralık")) month = 12;

                var dateParts = textDate.Split(' ');

                return new DateTime(Convert.ToInt32(dateParts[3]), month, 1);//no day info so return start of the month
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
