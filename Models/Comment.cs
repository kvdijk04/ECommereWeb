using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineMarket.Models
{
    public partial class Comment
    {
        public int CommentId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? ProductId { get; set; }
        public string CommentDescription { get; set; }
        public int? Rating { get; set; }
        public DateTime? CommentedOn { get; set; }
        
        public virtual Product Product { get; set; }
        

    }
}
