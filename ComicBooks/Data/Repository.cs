using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations.Model;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComicBooks.Models;

namespace ComicBooks.Data
{
   public static class Repository
    {

        static Context GetContext()
        {
            var context= new Context();
            //Print a log to the output window
            context.Database.Log = (messag) => Debug.WriteLine(messag);
            return context;
        }

        public static void Add(ComicBook comicBook)
        {
            using (Context context = GetContext())
            {
                context.ComicBooks.Add(comicBook);
                if (comicBook.Series != null && comicBook.Series.Id > 0)
                {
                    context.Entry(comicBook.Series).State = EntityState.Unchanged;
                }
                context.SaveChanges();
            }
        }

        public static void Update(ComicBook comicBook)
        {
            using (Context context = GetContext())
            {
                context.ComicBooks.Attach(comicBook);
                var comicBookEntry = context.Entry(comicBook);
                comicBookEntry.State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static void Delete(int comicBookId)
        {
            using (Context context = GetContext())
            {
               ComicBook comicBook= new ComicBook(){Id=comicBookId};
               
                context.Entry(comicBook).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }


    }
}
