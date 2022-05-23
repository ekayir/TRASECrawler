using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TRASECrawler.Model;
using TRASECrawler.Service;

namespace TRASECrawler
{
    class Program
    {
        static IDictionaryService _dictionaryService;
        static string _locationIQAPIKey = "TAKE_LOCATION_IQ_API_KEY";//https://locationiq.com/register
        static string _locationIQAPIURL = "https://eu1.locationiq.com/v1/search.php?key={0}&q={1}{2}&format=json";
        static string _baseURL = "https://www.tripadvisor.com.tr";
        static string _parametricLink = "https://www.tripadvisor.com.tr/Attractions-{0}-Activities-{1}-{2}.html";
        static string _flickrBaseURL = "https://www.flickr.com/search/?text={0}{1}";
        static void Initialize()
        {
            _dictionaryService = new DictionaryService();
        }
        static void Main(string[] args)
        {
            Initialize();

            StartCrawlerTask();
        }

        static async Task StartCrawlerTask()
        {
            try
            {
                var crawlerDatas = new List<CrawlerData>();

                var cities = _dictionaryService.GetCities();
                var categories = _dictionaryService.GetCategories();

                foreach (var city in cities)
                {
                    foreach (var category in categories)
                    {
                        var url = string.Format(_parametricLink, city.Code, category.Code, city.Name);

                        var htmlDocument = new HtmlDocument();
                        var html = GetHtmlContent(url).GetAwaiter().GetResult();

                        htmlDocument.LoadHtml(html);

                        var divs = htmlDocument.DocumentNode.Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("_25PvF8uO")).ToList();
                        //note that : in the time, tripadvisor change classes name.

                        foreach (var div in divs)
                        {
                            try
                            {
                                var comments = new List<Comment>();

                                var title = div.Descendants("h2").FirstOrDefault().InnerText;
                                var detailPageLink = div.Descendants("a").FirstOrDefault().GetAttributeValue("href", "");

                                var detailHtml = GetHtmlContent(_baseURL + detailPageLink).GetAwaiter().GetResult();
                                var detailHtmlDocument = new HtmlDocument();
                                detailHtmlDocument.LoadHtml(detailHtml);
                                var detailPageDiv = detailHtmlDocument.DocumentNode.Descendants("div").FirstOrDefault(x => x.GetAttributeValue("class", "").Equals("LjCWTZdN"));
                                var address = detailPageDiv != null && detailPageDiv.ChildNodes != null ? detailPageDiv.ChildNodes.LastOrDefault()?.InnerText : "-";
                                var commentDivs = detailHtmlDocument.DocumentNode.Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("Dq9MAugU T870kzTX LnVzGwUB")).ToList();
                                foreach (var comment in commentDivs)
                                {
                                    var commentDate = comment.Descendants("span").FirstOrDefault(x => x.GetAttributeValue("class", "").Equals("_34Xs-BQm"))?.InnerText;
                                    var commentText = comment.Descendants("q").FirstOrDefault(x => x.GetAttributeValue("class", "").Equals("IRsGHoPm"))?.InnerText;
                                    comments.Add(new Comment { DateText = commentDate, CommentText = commentText, Date = _dictionaryService.ConvertStringToDate(commentDate) });
                                }

                                crawlerDatas.Add(new CrawlerData
                                {
                                    Title = title,
                                    Address = GetAddress(title, address, city.Name).GetAwaiter().GetResult(),
                                    Comments = comments,
                                    City = city,
                                    Category = category,
                                    ImageURLs = GetImageURLs(title, city.Name).GetAwaiter().GetResult()
                                });
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }

                WriteDataToFile(JsonConvert.SerializeObject(crawlerDatas));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static void WriteDataToFile(string serializedJsonData)
        {
            string path = @"../../CrawledData/MarmaraRegionCrawledDataFromTripAdvisor.json";
            
            if (File.Exists(path) == false)
            {
                using (StreamWriter sw = File.CreateText(path)) { }
            }

            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(serializedJsonData);
            }
        }
        static async Task<string> GetHtmlContent(string url)
        {
            var httpClient = new HttpClient() { Timeout = TimeSpan.FromMinutes(2) };
            var html = await httpClient.GetStringAsync(url);

            return html;
        }

        static async Task<Address> GetAddress(string title, string fullAddress, string cityName)
        {
            try
            {
                string path = string.Format(_locationIQAPIURL, _locationIQAPIKey, cityName, title);

                var apiResult = GetHtmlContent(path).GetAwaiter().GetResult();
                var resultList = JsonConvert.DeserializeObject<List<LocationIQResult>>(apiResult);
                var firstRecord = resultList.FirstOrDefault(x => x.display_name.Contains(cityName));

                if (firstRecord == null) return new Address { FullAddress = fullAddress };

                return new Address { FullAddress = string.IsNullOrEmpty(fullAddress) || fullAddress == "-" ? firstRecord.display_name : fullAddress, Latitude = firstRecord.lat, Longitude = firstRecord.lon };
            }
            catch (Exception ex)
            {
                return new Address { FullAddress = fullAddress };
            }
        }
        static async Task<List<string>> GetImageURLs(string searchKey, string city)
        {
            var imageURLs = new List<string>();

            var htmlDocument = new HtmlDocument();
            var html = GetHtmlContent(string.Format(_flickrBaseURL, searchKey + " ", city)).GetAwaiter().GetResult();
            htmlDocument.LoadHtml(html);

            var divs = htmlDocument.DocumentNode.Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("view photo-list-photo-view requiredToShowOnServer awake")).ToList();

            foreach (var div in divs)
            {
                try
                {
                    var title = div.GetAttributeValue("style", "");
                    imageURLs.Add(PickURL(title));
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return imageURLs;
        }

        private static string PickURL(string value)
        {
            try
            {
                var startIndexOfImage = value.IndexOf("background-image: url");
                var firstParenthesis = value.IndexOf('(', startIndexOfImage);
                var lastParenthesis = value.IndexOf(')', startIndexOfImage);
                return value.Substring(firstParenthesis + 3, lastParenthesis - firstParenthesis - 3);
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
