using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models
{
    public partial class Feedback
    {
        public int Id { get; set; }
        public string Nmrid { get; set; }
        public string CommentedBy { get; set; }

        public string Message { get; set; }

        public DateTime CommentDate{get;set;}
        public virtual Nmr Nmr { get; set; }
        [NotMapped]
        public  ICollection<Feedback> items { get; set; }

    }
}
