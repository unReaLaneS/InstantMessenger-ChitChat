using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class FriendshipService
    {
        private FriendshipRepository friendshipRepository = new FriendshipRepository();
        private UserRepository userRepository = new UserRepository();

        public List<Friendship> FindFriendships(string username)
        {
            try
            {
                return friendshipRepository.GetFriendships(username);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        public void AddFriendship(string username, string friendname) 
        {
            try
            {
                User user = userRepository.GetUser(username);
                User friend = userRepository.GetUser(friendname);

                Friendship friendship = new Friendship(user.Id,friend.Id);

                friendshipRepository.CreateFriendship(friendship);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                throw e;
            }
        }

        public void UpdateFriendship(string username, string friendname)
        {
            try
            {
                Friendship friendship = friendshipRepository.GetFriendship(username, friendname);
                friendshipRepository.UpdateFriendship(friendship);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                throw e;
            }
        }

        public void DeleteFriendship(string username, string friendname)
        {
            try
            {
                Friendship friendship = friendshipRepository.GetFriendship(username, friendname);
                friendshipRepository.DeleteFriendship(friendship);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                throw e;
            }
        }

    }
}
