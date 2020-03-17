using Microsoft.EntityFrameworkCore;
using MyBooks.Core.Interfaces;
using MyBooks.Data;
using MyBooks.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MyBooks.Controllers
{
    public class Publisher : IPublisher
    {
        private readonly MyBooksContext _context;

        public Publisher(MyBooksContext context)
        {
            _context = context;
        }

        public async Task<PublisherModel> Add(PublisherModel publisher)
        {
            PublisherModel publisherSameName = await GetByName(publisher.Name);
            if (publisherSameName != null)
                return publisherSameName;

            publisher.Name = publisher.Name.ToUpper();

            _context.Add(publisher);
            await _context.SaveChangesAsync();
            return publisher;
        }

        public async Task<bool> Delete(int id)
        {
            PublisherModel item = await Get(id);
            if (item == null) return true;

            try
            {
                _context.Remove(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                item = await Get(id);
                return (item == null);
            }
        }

        public async Task<PublisherModel> Edit(PublisherModel publisher)
        {
            try
            {
                publisher.Name = publisher.Name.ToUpper();
                _context.Update(publisher);
                await _context.SaveChangesAsync();
                return publisher;
            }
            catch (DbUpdateConcurrencyException)
            {
                return await Get(publisher.Id);
            }
        }

        public async Task<PublisherModel> Get(int id)
        {
            return await _context.Publisher.FindAsync(id);
        }

        public async Task<PublisherModel> GetByName(string name)
        {
            return await _context.Publisher
                .Where(c => c.Name == name.ToUpper())
                .FirstOrDefaultAsync();
        }
    }
}
