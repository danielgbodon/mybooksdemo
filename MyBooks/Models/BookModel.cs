using MyBooks.Core;
using MyBooks.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MyBooks.Models
{
    public class BookModel
    {
        public BookModel()
        {
            Ratings = (new Random()).Next(0, 10000);
            Stars = (new Random()).Next(0, 5) + (new Random()).NextDouble();

            this.Authors_Book = new HashSet<Author_BookModel>();
            this.Book_Categories = new HashSet<Book_CategoryModel>();
            this.Book_BookLists = new HashSet<Book_BookListModel>();
        }

        public int Id { get; set; }

        public string IdGoogleService { get; set; }

        [Display(Name = "Titulo")]
        public string Title { get; set; }

        [Display(Name = "Subtitulo")]
        public string Subtitle { get; set; }

        [Display(Name = "Sinopsis")]
        public string Description { get; set; }

        [Display(Name = "NumeroPaginas")]
        public int? PageCount { get; set; }

        [Display(Name = "ISBN")]
        public string ISBN10 { get; set; }

        [Display(Name = "ISBN")]
        public string ISBN13 { get; set; }

        [Display(Name = "FechaPublicacion")]
        public DateTime? PublishedDate { get; set; }

        public string SmallImage { get; set; }
        public string Image { get; set; }

        [Display(Name = "Idioma")]
        public string Language { get; set; }

        [Display(Name = "Editorial")]
        public PublisherModel Publisher { get; set; }

        public int PublisherId { get; set; }

        public virtual ICollection<Author_BookModel> Authors_Book { get; set; }
        [NotMapped]
        public virtual ICollection<AuthorModel> Authors { 
            get {
                if (Authors_Book == null) return new List<AuthorModel>();
                return Authors_Book.Select(a => a.Author).ToList();
            }
            set
            {
                if (value == null)
                    Authors_Book = null;
                else
                    Authors_Book = value.Select(a => new Author_BookModel() { Book = this, Author = a }).ToList();
            }
        }

        public virtual ICollection<Book_CategoryModel> Book_Categories { get; set; }
        [NotMapped]
        public virtual ICollection<BookCategoryModel> Categories
        {
            get
            {
                if (Book_Categories == null) return new List<BookCategoryModel>();
                return Book_Categories.Select(c => c.Category).ToList();
            }
            set
            {
                if (value == null)
                    Book_Categories = null;
                else
                    Book_Categories = value.Select(c => new Book_CategoryModel() { Book = this, Category = c }).ToList();
            }
        }

        public virtual ICollection<Book_BookListModel> Book_BookLists { get; set; }

        [NotMapped]
        public int UserBookListId { get; set; }

        [Display(Name = "Puntuacion")]
        [NotMapped]
        public double Stars { get; set; }

        [Display(Name = "Opiniones")]
        [NotMapped]
        public int Ratings { get; set; }

        public BookViewModel getViewModel()
        {
            BookViewModel b = new BookViewModel() { 
                Title = Title,
                Description = Description,
                PageCount = PageCount,
                ISBN10 = ISBN10,
                ISBN13 = ISBN13,
                Authors = Authors.Select(a => a.Name),
                Publisher = (Publisher == null) ? null : Publisher.Name,
                PublishedDate = PublishedDate,
                SmallImage = SmallImage,
                Image = Image,
                Categories = Categories.Select(c => c.Name),
                Stars = Stars,
                Ratings = Ratings,
                UserBookListId = UserBookListId
            };

            if (Id > 0)
                b.Id = string.Format("{0}{1}{2}", AppEnvironment.BOOK_PREFIX_BD, AppEnvironment.BOOK_SEPARATOR_ID, Id);
            else if (!string.IsNullOrEmpty(IdGoogleService))
                b.Id = string.Format("{0}{1}{2}", AppEnvironment.BOOK_PREFIX_GOOGLE, AppEnvironment.BOOK_SEPARATOR_ID, IdGoogleService);
            
            return b;
        }
    }
}
