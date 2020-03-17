using Microsoft.EntityFrameworkCore;
using MyBooks.Core.Interfaces;
using MyBooks.Data;
using MyBooks.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MyBooks.Controllers
{
    public class Category : ICategory
    {
        private readonly MyBooksContext _context;

        public Category(MyBooksContext context)
        {
            _context = context;
        }

        public async Task<BookCategoryModel> Add(BookCategoryModel category)
        {
            category.Name = category.Name.ToUpper();
            BookCategoryModel categorySameName = await GetByName(category.Name);
            if (categorySameName != null)
                return categorySameName;

            _context.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> Delete(int id)
        {
            BookCategoryModel item = await Get(id);
            if(item == null) return true;

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

        public async Task<BookCategoryModel> Edit(BookCategoryModel category)
        {
            try
            {
                category.Name = category.Name.ToUpper();
                _context.Update(category);
                await _context.SaveChangesAsync();
                return category;
            }
            catch (DbUpdateConcurrencyException)
            {
                return await Get(category.Id);
            }
        }

        public async Task<BookCategoryModel> Get(int id)
        {
            return await _context.Category.FindAsync(id);
        }

        public async Task<BookCategoryModel> GetByName(string name)
        {
            return await _context.Category
                .Where(c => c.Name== name.ToUpper())
                .FirstOrDefaultAsync();
        }
    }
}
