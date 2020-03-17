using MyBooks.Models;
using System.Threading.Tasks;

namespace MyBooks.Core.Interfaces
{
    public interface ICategory
    {
        /*string Name
        {
            get;
            set;
        }*/
        public Task<BookCategoryModel> Get(int id);
        public Task<BookCategoryModel> GetByName(string name);
        public Task<BookCategoryModel> Add(BookCategoryModel category);
        public Task<BookCategoryModel> Edit(BookCategoryModel category);
        public Task<bool> Delete(int id);
    }
}
