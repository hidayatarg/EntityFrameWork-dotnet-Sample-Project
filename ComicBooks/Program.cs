using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComicBooks.Data;
using ComicBooks.Models;
using System.Data.Entity;

namespace ComicBooks
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context= new Context())
            {

                Series series= new Series()
                {
                    Title = "The amazing spideman"
                };

                context.ComicBooks.Add(new ComicBook()
                {
                    Series= series,
                    IssueNumber = 1,
                    PublishedOn = DateTime.Today
                });

                context.ComicBooks.Add(new ComicBook()
                {
                    Series = series,
                    IssueNumber = 2,
                    PublishedOn = DateTime.Today
                });

                context.SaveChanges();

                var comicBooks = context.ComicBooks
                    .Include(cb=>cb.Series)
                    .ToList();
                foreach (var comicBook in comicBooks)
                {
                    Console.WriteLine(comicBook.Series.Title);
                    Console.WriteLine(comicBook.DisplayText);
                }
            }
        }
    }
}
