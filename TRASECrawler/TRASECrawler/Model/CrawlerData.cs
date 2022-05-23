using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRASECrawler.Model
{
    public class CrawlerData
    {
        public Address Address { get; set; }
        public Category Category { get; set; }
        public City City { get; set; }
        public string Title { get; set; }
        public List<string> ImageURLs { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
