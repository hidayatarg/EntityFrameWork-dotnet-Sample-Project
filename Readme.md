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

A foreign key property named SeriesId of type int
A navigation property named Series of type Series
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

A navigation collection property named ComicBooks of type ICollection<ComicBook>
A default constructor that initializes the ComicBooks property to an instance of List<ComicBook>

```sh
public class Series
{
    // initailize the constructor
    public Series()
    {
        ComicBooks=new List<ComicBook>();
    }      
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    // a Series can be associated to many commic book
    public ICollection<ComicBook> ComicBooks { get; set; }
}
```
## Many To Many Relationship
```sh
public class Artist
{
    public Artist()
    {
        ComicBooks= new List<ComicBook>();
    }
    public int Id { get; set; }
    public string Name { get; set; }
	// Comic Book is depenedent on artist
    public ICollection<ComicBook> ComicBooks { get; set; }
}
```
in Comic Book
```sh
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
```

A new table is maded by the many to many relationship , (Junction, linking, bridges)
Called ArtistComicBook

## Bridge Entity Class
Sometimes we add extra property to the classes so we add the bridge table explicitly.
First we add the role table
```sh
public class Role
{
    public int  Id { get; set; }
    public string  Name { get; set; }
}
```
and ComicBookArtist
```sh
public class ComicBookArtist
{
    public int Id { get; set; }
    // Foreign Key Property
    public int ComicBookId { get; set; }
    public int ArtistId { get; set; }
    public int RoleId { get; set; }

    // Navigation Properties
    public ComicBook ComicBook { get; set; }
    public Artist Artist { get; set; }
    public Role Role { get; set; }
}
```

Now we need to update the ComicBook and Artist class Properties to the ComicBookArtist class

>> we do this because we want to add the bridge table a role Id.

in the ComicBook class since we will add comic book and each comic book with an artist and apply a specific *(explicit rule for each artist)
```sh
 // method to add artist a specific Role 
public void AddArtist(Artist artist, Role role)
{
    Artists.Add(new ComicBookArtist()
    {
        Artist = artist,
        Role = role
    });
}
```

and in the program.cs
```sh
    comicBook3.AddArtist(artist1, role1);
    comicBook3.AddArtist(artist2, role2);
``` 

here we use Select to find the path to the child property
and we update the query
```sh
var comicBooks = context.ComicBooks
                .Include(cb=>cb.Series)
                .Include(cb=>cb.Artists.Select(a=>a.Artist))
                .Include(cb=>cb.Artists.Select(a=>a.Role))
                .ToList();
```
## Data Annotations to Refine the Generated Database
We use data annotations
> use StringLength in place of maxlenght becuase max length is not working with validations.

## [Table("Talent")]
Define the table name in the corresponding database

## Column name
 [Required, StringLength(100), Column("FullName")]
  public string Name { get; set; }
  
  [NotMapped]
  It is ignored by entityframework 
  EntityFramework will automatically ignore properties that doesn`t have setter