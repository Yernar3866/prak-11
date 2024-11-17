using System;
using System.Collections.Generic;
using System.Linq;

#region Книга
class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public string ISBN { get; set; }
    public bool IsAvailable { get; set; } = true; // Книга доступна по умолчанию

    public Book(string title, string author, string genre, string isbn)
    {
        Title = title;
        Author = author;
        Genre = genre;
        ISBN = isbn;
    }
}
#endregion

#region Читатель
class Reader
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string TicketNumber { get; set; }

    public Reader(string firstName, string lastName, string ticketNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        TicketNumber = ticketNumber;
    }
}
#endregion

#region Каталог
interface ICatalog
{
    List<Book> SearchByTitle(string title);
    List<Book> SearchByAuthor(string author);
    List<Book> FilterByGenre(string genre);
}

class Catalog : ICatalog
{
    private List<Book> books;

    public Catalog()
    {
        books = new List<Book>();
    }

    public void AddBook(Book book)
    {
        books.Add(book);
    }

    public List<Book> SearchByTitle(string title) =>
        books.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();

    public List<Book> SearchByAuthor(string author) =>
        books.Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase)).ToList();

    public List<Book> FilterByGenre(string genre) =>
        books.Where(b => b.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase)).ToList();

    public List<Book> GetAllBooks() => books;
}
#endregion

#region Учетная система
interface IAccountingSystem
{
    void RecordIssuedBook(Book book, Reader reader);
    void RecordReturnedBook(Book book, Reader reader);
}

class AccountingSystem : IAccountingSystem
{
    private List<(Book book, Reader reader, DateTime dateIssued)> issuedBooks = new();

    public void RecordIssuedBook(Book book, Reader reader)
    {
        issuedBooks.Add((book, reader, DateTime.Now));
        Console.WriteLine($"Книга '{book.Title}' выдана читателю {reader.FirstName} {reader.LastName}.");
    }

    public void RecordReturnedBook(Book book, Reader reader)
    {
        var record = issuedBooks.FirstOrDefault(rb => rb.book == book && rb.reader == reader);
        if (record.book != null)
        {
            issuedBooks.Remove(record);
            Console.WriteLine($"Книга '{book.Title}' возвращена читателем {reader.FirstName} {reader.LastName}.");
        }
    }

    public void DisplayIssuedBooks()
    {
        Console.WriteLine("\nВыданные книги:");
        foreach (var record in issuedBooks)
        {
            Console.WriteLine($"- {record.book.Title}, читатель: {record.reader.FirstName} {record.reader.LastName}, дата: {record.dateIssued}");
        }
    }
}
#endregion

#region Библиотекарь
class Librarian
{
    private ICatalog catalog;
    private IAccountingSystem accountingSystem;

    public Librarian(ICatalog catalog, IAccountingSystem accountingSystem)
    {
        this.catalog = catalog;
        this.accountingSystem = accountingSystem;
    }

    public void IssueBook(string title, Reader reader)
    {
        var book = catalog.SearchByTitle(title).FirstOrDefault(b => b.IsAvailable);
        if (book != null)
        {
            book.IsAvailable = false;
            accountingSystem.RecordIssuedBook(book, reader);
        }
        else
        {
            Console.WriteLine($"Книга '{title}' недоступна.");
        }
    }

    public void ReturnBook(string title, Reader reader)
    {
        var book = catalog.GetAllBooks().FirstOrDefault(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (book != null && !book.IsAvailable)
        {
            book.IsAvailable = true;
            accountingSystem.RecordReturnedBook(book, reader);
        }
    }
}
#endregion

#region Программа
class Program
{
    static void Main(string[] args)
    {
        // Создание компонентов
        Catalog catalog = new();
        AccountingSystem accountingSystem = new();
        Librarian librarian = new(catalog, accountingSystem);

        // Добавление книг в каталог
        catalog.AddBook(new Book("Гарри Поттер", "Дж.К. Роулинг", "Фэнтези", "12345"));
        catalog.AddBook(new Book("Война и мир", "Лев Толстой", "Классика", "54321"));
        catalog.AddBook(new Book("Мастер и Маргарита", "Михаил Булгаков", "Классика", "67890"));

        // Создание читателя
        Reader reader = new("Ернар", "Алимов", "123");

        // Операции
        librarian.IssueBook("Гарри Поттер", reader);
        librarian.IssueBook("Война и мир", reader);

        accountingSystem.DisplayIssuedBooks();

        librarian.ReturnBook("Гарри Поттер", reader);

        accountingSystem.DisplayIssuedBooks();
    }
}
#endregion
