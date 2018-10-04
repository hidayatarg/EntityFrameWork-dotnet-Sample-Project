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

### Sorting Query
```sh
var comicBookI = context.ComicBooks
	.Include(cb=>cb.Series)
	.OrderByDescending(cb=>cb.IssueNumber)
	.ToList();
	Console.WriteLine(comicBookI.Count);

	foreach (var comicbooki in comicBookI)
	{
		Console.WriteLine(comicbooki.DisplayText);
	}

```
Order by will be used
```sh
var comicBookI = context.ComicBooks
		.Include(cb=>cb.Series)
		.OrderByDescending(cb=>cb.IssueNumber)
		.OrderBy(cb=>cb.PublishedOn)
		.ToList();
	Console.WriteLine(comicBookI.Count);
	// display the names
	foreach (var comicbooki in comicBookI)
	{
		Console.WriteLine(comicbooki.DisplayText);
	}
```
If you want to sort by more

```sh
var comicBookI = context.ComicBooks
		.Include(cb=>cb.Series)
		.OrderByDescending(cb=>cb.IssueNumber)
		.ThenBy(cb=>cb.PublishedOn)
		.ToList();
	Console.WriteLine(comicBookI.Count);
	// display the names
	foreach (var comicbooki in comicBookI)
	{
		Console.WriteLine(comicbooki.DisplayText);
	}
```
You see the result query that is executed from the Output

## Loading the Related Entities
### Methods for Loading Related Entities
-Eager Loading: you can write a single query that not only retrieves the data for the main entity, but also the data for the related entities.The include method is used to tell EF which related entities to load.
-Lazy Loading: the related entities are not loaded until their navigation properties are accessed. The process of lazily loading related entities is automatically handled by EF.
-Explicit Loading : an alternative to lazy loading you can also explicitly load related entities using the load method.
with lazy loading we will add the virual keyword with the Navigation properties
with eager loading EF was executing a single query to retrive the related data, but with
Lazy Loading multiple queries are executed in order tor retrive the data.

```sh
public class ComicBook
{
	public ComicBook()
	{
		Artists= new List<ComicBookArtist>();
	}
	public int  Id { get; set; }
	//Series enity is principal
	//Comic book is dependent upon a series
	//many to one relationship
	public int SeriesId { get; set; }
		
	public int  IssueNumber { get; set; }
	public string  Description { get; set; }
	public DateTime  PublishedOn { get; set; }
	public decimal  AverageRating { get; set; }

	// navigation
	public virtual Series Series { get; set; }
	// many to many Relationship
	public virtual ICollection<ComicBookArtist> Artists { get; set; }

	// Display Text
	// getter propety ignored by Ef
	public string DisplayText
	{
		get { return $"{Series?.Title} #{IssueNumber}"; }
	}
}
```

and 

```sh
namespace ComicBooks.Models
{
	public class ComicBookArtist
	{
		public int Id { get; set; }
		// Foreign Key Property
		public int ComicBookId { get; set; }
		public int ArtistId { get; set; }
		public int RoleId { get; set; }

		// Navigation Properties
		public virtual ComicBook ComicBook { get; set; }
		public virtual Artist Artist { get; set; }
		public virtual Role Role { get; set; }
	}
}
```

## Detail Queries
> Find
to retrive a single entity we use dbset find method
```sh
var comicBookId = 1;
var comicBook = context.ComicBooks.Find(comicBookId);
```
or we can use the where operator
SingleorDefault find the entity 
FirstorDefault find the first matching entity  if more than one is found.
```sh
var comicBook = context.ComicBooks
	.where(cb=>cb.Id==comicBookId)
	.SingleOrDefault();
```
-Single
-First
we suggest using dbset queries over find method
```sh
 var comicBooks = context.ComicBooks
			  .Include(cb => cb.Series)
			  .Include(cb => cb.Artists.Select(a => a.Artist))
			  .Include(cb => cb.Artists.Select(a => a.Role))
			  .ToList();
```





### DAL (Data Access Layer)
under this layer we have Repository, Context, Database initializer

### Entities 
a layer contains entities.


## Repository 
All of the code that interact with the Entity framework context class into its own class that is named Repository class.
We are using the repository class instead of using the context class directly.

  -Retrieve
  -Create
  -Update
  -Delete

In Repository.cs, GetContext method to get the method 
```sh
static Context GetContext()
{
	var context= new Context();
	
	//Print a log to the output window
	context.Database.Log = (messag) => Debug.WriteLine(messag);
	return context;
}
```

##### Adding Method for this project
```sh
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
```
##### Add method structure (Generally used in EF)
```sh
public static void Add(ComicBook comicBook)
{
	using (Context context = new Context())
	{
		context.ComicBooks.Add(comicBook);      
		context.SaveChanges();
	}
}
```

##### Update method (1) for this project
Here we equalize the entities to the update values entered by the user.
```sh
public static void Update(ComicBook comicBook)
{
	using (Context context = GetContext())
	{
		ComicBook comicBookToUpdate = context.ComicBooks.Find(comicBook.Id);
		if (comicBookToUpdate != null)
		{
			comicBookToUpdate.SeriesId = comicBook.SeriesId;
			comicBookToUpdate.IssueNumber = comicBook.IssueNumber;
			comicBookToUpdate.Description = comicBook.Description;
			comicBookToUpdate.PublishedOn = comicBook.PublishedOn;
			comicBookToUpdate.AverageRating = comicBook.AverageRating;
		}

		context.SaveChanges();
	}
}
```
##### Update method (2) for this project (EF)
Here we attach the comic book to the entity. and then we confirm that is enity was updated.
```sh
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
```


> Note: There might be cases that some entities are not updated so then the upper code will be like
> 
```sh    
		context.ComicBooks.Attach(comicBook);
		var comicBookEntry = context.Entry(comicBook);
		comicBookEntry.State = EntityState.Modified;
		comicBookEntry.Property("IssueNumber").IsModified = False;

		context.SaveChanges(); 
```
   

##### Delete method (1) for this project
The input parameter to this method should be an interger of Comic Book Id. the find method will find the comic book with the related input comicBookId. Then remove that comic book.
```sh
public static void Delete(int comicBookId)
{
	using (Context context = GetContext())
	{
		ComicBook comicBook = context.ComicBooks.Find(comicBookId);
		context.ComicBooks.Remove(comicBook);
		context.SaveChanges();
	}
}
```

##### Delete method (2) for this project (EF)
We are not attching the entity, bwhen calling the context entry method.
If the passed in entity is not in the context EF will attach it and set its state to unchanged
```sh
public static void Delete(int comicBookId)
{
	using (Context context = GetContext())
	{
		ComicBook comicBook= new ComicBook(){Id=comicBookId};
		context.Entry(comicBook).State = EntityState.Deleted;
		context.SaveChanges();
	}
}
```

There are other ORM 

	Nhibernate 
	Dapper

https://docs.microsoft.com/en-us/ef/core/

http://www.entityframeworktutorial.net/

https://blogs.msdn.microsoft.com/dotnet/tag/entity-framework/
