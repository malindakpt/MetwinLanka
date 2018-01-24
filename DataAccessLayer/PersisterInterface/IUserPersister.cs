using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using System.Security.Claims;

namespace DataAccessLayer.PersisterInterface
{
    public interface IUserPersister : IDisposable
    {
        void AddUser(User user,string password);
        void UpdateUser(User user);
        User LoadUser(string username);
        IEnumerable<User> LoadUsers();
        User AuthenticateUser(string username, string password,out ClaimsIdentity identity);
        void SetUserRole(string username, string role);
        void SetUserPassword(string username, string password);
    }
}
