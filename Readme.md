# NuGET install EntityFramework

All communication from our application to the database flows through the context class.

# In this Project we will be dropping the Database each time the model is created.
DROP the DB and start


## Include 
Version 1: 
```sh
var comicBooks = context.ComicBooks
                    .Include("Series")
                    .ToList();
```
Version 2: using lambda expression
```sh
 var comicBooks = context.ComicBooks
                    .Include(cb=>cb.Series)
                    .ToList();
                foreach (var comicBook in comicBooks)
                {
                    Console.WriteLine(comicBook.Series.Title);
                }
```

## Defining a Foreign Key Property
In order to make the SeriesId not accept null,
There are two ways, the first method:

```sh
public class ComicBook
{
    public int  Id { get; set; }
    //Series enity is principal
    //Comic book is dependent upon a series
    //many to one relationship
    public int SeriesRefId { get; set; }
    [ForeignKey("SeriesRefId")]
    public Series  Series { get; set; }
    public int  IssueNumber { get; set; }
    public string  Description { get; set; }
    public DateTime  PublishedOn { get; set; }
    public decimal  AverageRating { get; set; }

    //Display Text
    //getter propety ignored by Ef
    public string DisplayText
    {
        get { return $"{Series?.Title} #{IssueNumber}"; }
    }
}
```

But this can be made cleaner and easier with entity framework
```sh
public class ComicBook
{
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

    //Display Text
    //getter propety ignored by Ef
    public string DisplayText
    {
        get { return $"{Series?.Title} #{IssueNumber}"; }
    }
}
```