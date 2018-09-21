using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComicBooks.Data;
using ComicBooks.Models;

namespace ComicBooks
{
   internal class DatabaseInitializer:DropCreateDatabaseIfModelChanges<Context>
    {
        protected override void Seed(Context context)
        {
            //var type
            Series series1 = new Series()
            {
                Title = "The amazing spideman"
            };

            Series series2 = new Series()
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

            var role1 = new Role()
            {
                Name = "script"
            };
            var role2 = new Role()
            {
                Name = "Pencils"
            };

            var comicBook1 = new ComicBook()
            {
                Series = series1,
                IssueNumber = 1,
                PublishedOn = DateTime.Today
            };

            comicBook1.AddArtist(artist1, role1);
            comicBook1.AddArtist(artist2, role2);


            var comicBook2 = new ComicBook()
            {
                Series = series2,
                IssueNumber = 2,
                PublishedOn = DateTime.Today
            };

            comicBook2.AddArtist(artist1, role1);
            comicBook2.AddArtist(artist2, role2);
            var comicBook3 = new ComicBook()
            {
                Series = series2,
                IssueNumber = 1,
                PublishedOn = DateTime.Today
            };

            comicBook3.AddArtist(artist1, role1);
            comicBook3.AddArtist(artist3, role2);

            context.ComicBooks.Add(comicBook1);
            context.ComicBooks.Add(comicBook2);
            context.ComicBooks.Add(comicBook3);

            context.SaveChanges();
        }
    }
}
