using MyBooks.Models;
using System.Threading.Tasks;

namespace MyBooks.Core.Interfaces
{
    public interface IUser
    {
        public Task<UserModel> Register(UserModel user, string password, string role);
        public Task<UserModel> Edit(UserModel user);
        public Task<UserModel> CheckUser(string username, string password);
        public Task Logout();
    }
}
