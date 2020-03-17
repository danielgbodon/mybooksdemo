using MyBooks.Models;
using System.Threading.Tasks;

namespace MyBooks.Core.Interfaces
{
    public interface IPublisher
    {
        public Task<PublisherModel> Get(int id);
        public Task<PublisherModel> GetByName(string name);
        public Task<PublisherModel> Add(PublisherModel publisher);
        public Task<PublisherModel> Edit(PublisherModel publisher);
        public Task<bool> Delete(int id);
    }
}
