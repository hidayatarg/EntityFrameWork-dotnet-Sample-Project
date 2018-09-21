using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ComicBooks.Models
{
    
    public class Series
    {
        // initailize the constructor
        public Series()
        {
            ComicBooks=new List<ComicBook>();
        }
        
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }
        public string Description { get; set; }

        // a Series can be associated to many commic book
        public ICollection<ComicBook> ComicBooks { get; set; }
    }
}
