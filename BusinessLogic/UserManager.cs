using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.PersisterInterface;
using DataAccessLayer;
using BusinessObjects;
using System.Security.Claims;

namespace BusinessLogic
{
    public class UsersManager : IDisposable
    {
        private IUserPersister db;

        public UsersManager(IUserPersister persister = null)
        {
            db = persister ?? new UserPersister();
        }

        public void CreateUser(User user,string password)
        {
            db.AddUser(user, password);
        }

        public User Authenticate(string username,string password,out ClaimsIdentity identity)
        {
            return db.AuthenticateUser(username, password,out identity);
        }

        public object CreateIdentity(User user)
        {
            return 0;
        }

        public IEnumerable<User> LoadAllUsers()
        {
            return db.LoadUsers();
        }

        public User LoadUser(string username)
        {
            return db.LoadUser(username);
        }

        public void SetUserRole(string username,string role)
        {
            db.SetUserRole(username, role);
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public void SetUserPassword(string username,string password)
        {
            if(string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be empty");
            }
            db.SetUserPassword(username, password);
        }
    }
}
