using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicBooks.Models
{
    public class ComicBook
    {
        public int  Id { get; set; }
        //Series enity is principal
        //Comic book is dependent upon a series
        //many to one relationship
        public Series  Series { get; set; }
        public int  IssueNumber { get; set; }
        public string  Description { get; set; }
        public DateTime  PublishedOn { get; set; }
        public decimal  AverageRating { get; set; }
    }
}
