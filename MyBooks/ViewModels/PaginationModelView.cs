using System.Collections.Generic;

namespace MyBooks.ViewModels
{
    public class PaginationModelView<T>
    {
        public int totalItems { get; set; }
        public int elementsPerPage { get; set; }
        public int currentPage { get; set; }
        public IList<T> items { get; set; }
    }
}
