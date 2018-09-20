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
                //var type
                Series series1= new Series()
                {
                    Title = "The amazing spideman"
                };

                Series series2= new Series()
                {
                    Title = "The amazing spideman"
                };

                var artist1 = new Artist()
                {
                    Name = "Stev Lee"
                };
                var artist2 = new Artist()
                {
                    Name = "Leon Koweski"
                };
                var artist3 = new Artist()
                {
                    Name = "Jack Koweski"
                };

                var comicBook1 = new ComicBook()
                {
                    Series = series1,
                    IssueNumber = 1,
                    PublishedOn = DateTime.Today
                };
                comicBook1.Artists.Add(artist1);
                comicBook1.Artists.Add(artist2);


                var comicBook2= new ComicBook()
                {
                    Series = series2,
                    IssueNumber = 2,
                    PublishedOn = DateTime.Today
                };

                comicBook2.Artists.Add(artist1);
                comicBook2.Artists.Add(artist2);
                var comicBook3 = new ComicBook()
                {
                    Series = series2,
                    IssueNumber = 1,
                    PublishedOn = DateTime.Today
                };

                comicBook3.Artists.Add(artist1);
                comicBook3.Artists.Add(artist2);

                context.ComicBooks.Add(comicBook1);
                context.ComicBooks.Add(comicBook2);
                context.ComicBooks.Add(comicBook3);

                context.SaveChanges();

                var comicBooks = context.ComicBooks
                    .Include(cb=>cb.Series)
                    .Include(cb=>cb.Artists)
                    .ToList();
                foreach (var comicBook in comicBooks)
                {
                    var artistNames = comicBook.Artists.Select(a => a.Name).ToList();
                    var artistDisplayText = string.Join(", ", artistNames);
                    Console.WriteLine(comicBook.Series.Title);
                    Console.WriteLine(comicBook.DisplayText);
                    Console.WriteLine(artistDisplayText);
                }
            }
        }
    }
}
