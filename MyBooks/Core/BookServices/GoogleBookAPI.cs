using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using MyBooks.Core.Interfaces;
using MyBooks.Models;
using MyBooks.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MyBooks.Core.BookServices
{
    public class GoogleBookAPI : IBookService
    {
        private string apiKey;
        private readonly BooksService _booksService;
        private readonly IConfiguration _configuration;
        private int resultsPerPage;

        public GoogleBookAPI(IConfiguration configuration)
        {
            _configuration = configuration;
            apiKey = _configuration.GetValue<string>("GoogleBooksAPIKey");
            resultsPerPage = _configuration.GetValue<int>("ResultsPerPage");
            _booksService = new BooksService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = this.GetType().ToString()
            }); ;
        }

        BookModel IBookService.Get(string id)
        {
            VolumesResource.GetRequest query = _booksService.Volumes.Get(id);
            return StandarizeBookModel(query.Execute());
        }

        public PaginationModelView<BookModel> Search(string search, int page = 0)
        {
            if(string.IsNullOrEmpty(search))
                return new PaginationModelView<BookModel>
                {
                    totalItems = 0,
                    elementsPerPage = resultsPerPage,
                    currentPage = 0,
                    items = new List<BookModel>()
                };

            return List(search, page);
        }

        public PaginationModelView<BookModel> Search(BookModel book, int page)
        {
            //https://developers.google.com/books/docs/v1/using?csw=1
            string q = "";
            if (!string.IsNullOrEmpty(book.Title)) q += String.Format("+intitle:{0}", book.Title);
            if (book.Authors.Count > 0) q += String.Format("+inauthor:{0}", book.Authors.First());
            if (book.Publisher == null || string.IsNullOrEmpty(book.Publisher.Name)) q += String.Format("+inpublisher:{0}", book.Publisher.Name);
            if (book.Categories.Count > 0) q += String.Format("+subject:{0}", book.Categories.First());
            if (!string.IsNullOrEmpty(book.ISBN10)) q += String.Format("+isbn:{0}", book.ISBN10);
            else if (!string.IsNullOrEmpty(book.ISBN13)) q += String.Format("+isbn:{0}", book.ISBN13);

            return List(q.Trim('+'), page);
        }

        private PaginationModelView<BookModel> List(string search, int page=0)
        {
            VolumesResource.ListRequest listquery = _booksService.Volumes.List(search);
            listquery.MaxResults = resultsPerPage;
            listquery.StartIndex = resultsPerPage*page;
            listquery.LangRestrict = CultureInfo.CurrentCulture.ToString().Substring(0,2);
            try
            {
                var res = listquery.Execute();
                return new PaginationModelView<BookModel>
                {
                    totalItems = (int)res.TotalItems,
                    elementsPerPage = resultsPerPage,
                    currentPage = page+1,
                    items = res.Items.Select(b => StandarizeBookModel(b)).ToList()
                };
            }
            catch(Exception) {
                return new PaginationModelView<BookModel>
                {
                    totalItems = 0,
                    elementsPerPage = resultsPerPage,
                    currentPage = 0,
                    items = new List<BookModel>()
                };
            }
        }

        BookModel StandarizeBookModel(object o)
        {
            
            try
            {
                Volume v = (Volume)o;

                BookModel book = new BookModel
                {
                    IdGoogleService = v.Id,
                    Title = v.VolumeInfo.Title,
                    Subtitle = v.VolumeInfo.Subtitle,
                    Description = v.VolumeInfo.Description,
                    PageCount = v.VolumeInfo.PageCount,
                    Authors = (v.VolumeInfo.Authors == null) ? null : v.VolumeInfo.Authors.Select(a => new AuthorModel() { Name = a }).ToList(),
                    Publisher = new PublisherModel() { Name = v.VolumeInfo.Publisher },
                    SmallImage = (v.VolumeInfo.ImageLinks != null) ? v.VolumeInfo.ImageLinks.SmallThumbnail : "",
                    Image = (v.VolumeInfo.ImageLinks != null) ? v.VolumeInfo.ImageLinks.Thumbnail : "",
                    Categories = (v.VolumeInfo.Categories == null) ? null : v.VolumeInfo.Categories.Select(a => new BookCategoryModel() { Name = a }).ToList()
                };

                DateTime publishedDate;
                if (DateTime.TryParse(v.VolumeInfo.PublishedDate, out publishedDate)) book.PublishedDate = publishedDate;

                if (v.VolumeInfo.IndustryIdentifiers != null)
                {
                    foreach (Volume.VolumeInfoData.IndustryIdentifiersData ident in v.VolumeInfo.IndustryIdentifiers)
                    {
                        switch (ident.Type)
                        {
                            case "ISBN_10": book.ISBN10 = ident.Identifier; break;
                            case "ISBN_13": book.ISBN13 = ident.Identifier; break;
                        }
                    }
                }

                return book;
            }
            catch (Exception) {
                return null;
            }

        }
    }
}
