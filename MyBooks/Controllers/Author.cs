using Microsoft.EntityFrameworkCore;
using MyBooks.Core.Interfaces;
using MyBooks.Data;
using MyBooks.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MyBooks.Controllers
{
    public class Author : IAuthor
    {
        private readonly MyBooksContext _context;

        public Author(MyBooksContext context)
        {
            _context = context;
        }

        public async Task<AuthorModel> Add(AuthorModel author)
        {
            AuthorModel authorSameName = await GetByName(author.Name);
            if (authorSameName != null)
                return authorSameName;

            author.Name = author.Name.ToUpper();

            _context.Add(author);
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task<bool> Delete(int id)
        {
            AuthorModel item = await Get(id);
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

        public async Task<AuthorModel> Edit(AuthorModel author)
        {
            try
            {
                author.Name = author.Name.ToUpper();
                _context.Update(author);
                await _context.SaveChangesAsync();
                return author;
            }
            catch (DbUpdateConcurrencyException)
            {
                return await Get(author.Id);
            }
        }

        public async Task<AuthorModel> Get(int id)
        {
            return await _context.Author.FindAsync(id);
        }

        public async Task<AuthorModel> GetByName(string name)
        {
            return await _context.Author
                .Where(c => c.Name == name.ToUpper())
                .FirstOrDefaultAsync();
        }
    }
}
