using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.PersisterInterface;
using BusinessObjects;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Security.Claims;

namespace DataAccessLayer
{
    public class UserPersister : IUserPersister
    {

        AppDBContext db = new AppDBContext();
        UserManager<User> userMan;

        public UserPersister()
        {
            userMan = db.GetUserManager();
        }

        public void AddUser(User user,string password)
        {
            ValidateUser(user);
            var result = userMan.Create(user, password);
            if(!result.Succeeded)
            {
                throw new ArgumentException("Error creating user. " + string.Join(";", result.Errors));
            }
        }

        public void UpdateUser(User user)
        {
            ValidateUser(user);
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
        }

        public User LoadUser(string username)
        {
            return db.GetUserManager().FindById(username);
        }

        public IEnumerable<User> LoadUsers()
        {
            return db.Users.ToList();
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public User AuthenticateUser(string username, string password,out ClaimsIdentity identity)
        {
            var user= userMan.Find(username, password);
            if(user != null)
            {
                identity = userMan.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            }
            else
            {
                identity = null;
            }
            return user;
        }

        private void ValidateUser(User user)
        {

        }



        public void SetUserRole(string username, string role)
        {
            var user = userMan.FindByName(username);
            userMan.AddToRole(user.Id, role);
        }


        public void SetUserPassword(string username, string password)
        {
            var userId = userMan.FindByName(username).Id;
            userMan.RemovePassword(userId);
            userMan.AddPassword(userId, password);
        }
    }
}
