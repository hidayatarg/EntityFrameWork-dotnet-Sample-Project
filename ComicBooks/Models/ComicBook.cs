using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicBooks.Models
{
    public class ComicBook
    {
        public ComicBook()
        {
            Artists= new List<Artist>();
        }
        public int  Id { get; set; }
        //Series enity is principal
        //Comic book is dependent upon a series
        //many to one relationship
        public int SeriesId { get; set; }
        public Series  Series { get; set; }
        public int  IssueNumber { get; set; }
        public string  Description { get; set; }
        public DateTime  PublishedOn { get; set; }
        public decimal  AverageRating { get; set; }

        // many to many Relationship
        public ICollection<Artist> Artists { get; set; }

        //Display Text
        //getter propety ignored by Ef
        public string DisplayText
        {
            get { return $"{Series?.Title} #{IssueNumber}"; }
        }
    }
}
