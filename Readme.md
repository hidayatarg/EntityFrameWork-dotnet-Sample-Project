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
First we add the role table.
When defining a many-to-many relationship without defining an explicit bridge entity class, Entity Framework will automatically add an "implicit" bridge table to the database in order to store the relationship data.
Defining an explicit Many-to-Many bridge entity class allows you to include additional properties beyond the properties that are needed to define the relationship.
navigation properties allow you to define relationships between entities.
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

## Overriding the Context's OnModelCreating Method
we can customize EF's conventions and use EF's fluent API to refine our model.


> 123.45 Precision=5 and scale=2   Average rating
## Fluent API
```sh
protected override void OnModelCreating(DbModelBuilder modelBuilder)
{
            
    // remove pluralize conditions
    modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();

    /*  // For Average Rating 
    modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
    modelBuilder.Conventions.Add(new DecimalPropertyConvention(5,2));
    */ 
    //Bu using fluent API
    modelBuilder.Entity<ComicBook>()
        .Property(cb => cb.AverageRating)
        .HasPrecision(5, 2);
}
```

## Seed 
Seed method is called after the database is created
we created the DatabaseInitializer in the main tree of program
in context we changed the following

```sh
		 Database.SetInitializer(new DatabaseInitializer());
```

# List Queries
```sh
context.Database.Log = (message) => Debug.WriteLine(message);
var comicBooks = context.ComicBooks.ToList();
Console.WriteLine("the number of comicbooks{0}",comicBooks.Count);
```
From the output pannel see the query that is executed from the Output window.

## Linq query 
var comicBooksQuery = from cb in context.ComicBooks select cb;
var comicBooks = comicBooksQuery.ToList();
Console.WriteLine("the number of comicbooks{0}", comicBooks.Count);


the Type for comicBooks here is IQueryable need toList 

### Filtering Query
```sh
var comicBookI = context.ComicBooks.Where(cb => cb.IssueNumber == 1).ToList();
Console.WriteLine(comicBookI.Count);
// display the names
foreach (var comicbooki in comicBookI)
{
    Console.WriteLine(comicbooki.DisplayText);
}
```
Even we can add include with context to get the series title if we want


```sh
var comicBookI = context.ComicBooks
    .Include(cb=>cb.Series)
    .Where(cb => cb.IssueNumber == 1 && cb.Series.Title=="The amazing Spiderman").ToList();
Console.WriteLine(comicBookI.Count);
// display the names
foreach (var comicbooki in comicBookI)
{
    Console.WriteLine(comicbooki.DisplayText);
}
```

Contain
```sh
 //Filtering Query
var comicBookI = context.ComicBooks
    .Include(cb=>cb.Series)
    .Where(cb =>cb.Series.Title.Contains("The amazing spider")).ToList();
Console.WriteLine(comicBookI.Count);
// display the names
foreach (var comicbooki in comicBookI)
{
    Console.WriteLine(comicbooki.DisplayText);
}
```