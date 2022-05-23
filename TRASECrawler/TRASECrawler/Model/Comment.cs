using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRASECrawler.Model
{
    public class Comment
    {
        public string CommentText { get; set; }
        public string DateText { get; set; }
        public Nullable<DateTime> Date { get; set; }
    }
}
