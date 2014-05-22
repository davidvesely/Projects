// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    public enum Category
    {
        Adventure,
        Love,
        History,
        Technology
    }

    [ServiceContract]
    public class BookService
    {
        List<Book> books = new List<Book>();

        public BookService()
        {
            this.books.Add(new Book() { Name = "Harry Potter", Id = 9999, Category = Category.Adventure, IsAvailable = true });
        }

        [WebGet(UriTemplate = "")]
        public List<Book> Get()
        {
            return this.books;
        }

        [WebGet(UriTemplate = "{category}?id={id}&name={name}&isavailable  ={available}")]
        public List<Book> GetCategory(Category category, int id, string name, bool available)
        {
            return new List<Book>(this.books.Where<Book>(p =>
            {
                return p.Category == category && p.Id == id && p.Name == name && p.IsAvailable == available;
            }));
        }

        [WebInvoke(UriTemplate = "", Method = "POST")]
        public Book Add(Book book)
        {
            this.books.Add(book);
            return book;
        }

        [WebInvoke(UriTemplate = "{i}", Method = "DELETE")]
        public Book Remove(int i)
        {
            Book toRemove = this.books[i];
            this.books.RemoveAt(i);
            return toRemove;
        }

        [WebInvoke(UriTemplate = "{i}", Method = "PUT")]
        public Book Modify(int i, Book book)
        {
            this.books[i] = book;
            return book;
        }
    }

    public class Book
    {
        public string Name
        {
            get;
            set;
        }

        public Category Category
        {
            get;
            set;
        }

        public int Id
        {
            get;
            set;
        }

        public bool IsAvailable
        {
            get;
            set;
        }
    }
}