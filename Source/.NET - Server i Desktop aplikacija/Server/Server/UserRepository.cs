using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class UserRepository
    {

        public void CreateUser(User user)
        {
            try
            {
                using (var context = new UniContext())
                {
                    if (user == null) 
                    {
                        throw new ArgumentNullException();
                    }
                    context.Users.Add(user);
                    context.SaveChanges();

                }
            }
            catch (Exception e) {
                throw e;
            }
        }

        public User GetUser(string username)
        {
            try
            {
                using (var context = new UniContext())
                {
                    if (username == null)
                    {
                        throw new ArgumentNullException();
                    }
                    var user = context.Users.Where(u => u.UserName == username).FirstOrDefault();

                    return user;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }


        //public Korisnik read(Korisnik entity) { }
        //public Korisnik update(Korisnik entity) { }
        //public void delete(Korisnik entity) { }


        public List<User> GetAllUsers()
        {
            try
            {
                using (UniContext context = new UniContext())
                {
                    List<User> users = context.Users.ToList<User>();

                    return users;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

}
