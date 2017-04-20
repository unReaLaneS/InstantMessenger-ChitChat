using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class FriendshipRepository
    {
        public Friendship GetFriendship(string username, string friendname)
        {
            try
            {
                using (var context = new UniContext())
                {
                    if (username == null || friendname == null) 
                    {
                        throw new ArgumentNullException();
                    }
                    var friendship = context.Friendships.Include("User").Include("Friend").SingleOrDefault(u => (u.User.UserName == friendname && u.Friend.UserName == username) || u.User.UserName == username && u.Friend.UserName == friendname);

                    return friendship;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public List<Friendship> GetFriendships(string username)
        {
            try
            {
                using (var context = new UniContext())
                {
                    if (username == null) 
                    {
                        throw new ArgumentNullException();
                    }
                    List<Friendship> friendships = context.Friendships.Include("User").Include("Friend").Where(u => (u.User.UserName == username || u.Friend.UserName == username)).ToList<Friendship>();

                    return friendships;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CreateFriendship(Friendship friendship) 
        {
            try
            {
            if (friendship == null) 
            {
                throw new ArgumentNullException();
            }
                using (var context = new UniContext())
                {
                    context.Friendships.Add(friendship);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateFriendship(Friendship friendship) 
        {
            try 
            {
                using (var context = new UniContext()) 
                {
                    if (friendship == null) 
                    {
                        throw new ArgumentNullException();
                    }
                    var result = context.Friendships.SingleOrDefault(u => u.Id == friendship.Id);
                    if (result != null)
                    {
                        result.Accepted = true;
                        context.SaveChanges();
                    }
                
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteFriendship(Friendship friendship)
        {
            try
            {
                using (var context = new UniContext())
                {
                    if (friendship == null) 
                    {
                        throw new ArgumentNullException();
                    }
                    Friendship tempFriendShip = (Friendship)context.Friendships.Where(f => f.Id == friendship.Id).First();
                    context.Friendships.Remove(tempFriendShip);
                    context.SaveChanges();

                }
            }
            catch (Exception e)
            {
                throw e;
            }


        }

    }
}
