using MyBooks.Models;
using System.Threading.Tasks;

namespace MyBooks.Core.Interfaces
{
    public interface IAuthor
    {
        public Task<AuthorModel> Get(int id);
        public Task<AuthorModel> GetByName(string name);
        public Task<AuthorModel> Add(AuthorModel author);
        public Task<AuthorModel> Edit(AuthorModel author);
        public Task<bool> Delete(int id);
    }
}
