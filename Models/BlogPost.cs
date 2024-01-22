using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Models
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }
        public string url { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Content { get; set; }
    }
}
