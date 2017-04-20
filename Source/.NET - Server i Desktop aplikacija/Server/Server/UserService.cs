using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class UserService
    {

        private FriendshipRepository friendshipRepository = new FriendshipRepository();
        private UserRepository userRepository = new UserRepository();


        public List<User> FindUserFriendList(string username)
        {
            try
            {
                if (username == null) 
                {
                    throw new ArgumentNullException();
                }
                List<Friendship> friends = friendshipRepository.GetFriendships(username);
                List<User> userFriends = new List<User>();
                foreach (var friendship in friends)
                {
                    if (friendship.User.UserName == username)
                    {
                            userFriends.Add(friendship.Friend);

                    }
                    else if (friendship.Friend.UserName == username)
                    {
                        if (friendship.Accepted == false)
                        {
                            userFriends.Add(friendship.User);
                            userFriends.Last().UserName += "(Pending)";
                        }
                        else 
                        {
                            userFriends.Add(friendship.User);
                        }
                    }

                }

                return userFriends;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;    
            }

        }

        public User FindUser(string username)
        {
            try
            {
                if (username == null)
                {
                    throw new ArgumentNullException();
                }
                return userRepository.GetUser(username);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        public List<User> FindAllUsers() 
        {
            try 
            {
               return userRepository.GetAllUsers(); 
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        public void AddUser(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException();
                }
                userRepository.CreateUser(user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }


    }
}
