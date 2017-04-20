using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace Server
{
    public class UserController
    {

        public UserController(Program p, TcpClient c)
        {
            prog = p;
            client = c;

            (new Thread(new ThreadStart(SetupConn))).Start();
        }

        private Program prog;
        private TcpClient client;
        private NetworkStream netStream;
        private BinaryReader br;
        private BinaryWriter bw;
        private UserService userService;
        private FriendshipService friendshipService;
        private User user;
        private List<string> friends;
        private List<string> allUsers;
        private BinaryFormatter bin;
        
        private void SetupConn()  // Setup connection and login or register.
        {
            try
            {
                Console.WriteLine("[{0}] New connection!", DateTime.Now);
                netStream = client.GetStream();
                userService = new UserService();
                friendshipService = new FriendshipService();
                Console.WriteLine("[{0}] Connection established!", DateTime.Now);

                br = new BinaryReader(netStream, Encoding.UTF8);
                bw = new BinaryWriter(netStream, Encoding.UTF8);

                bin = new BinaryFormatter();

                // Say "hello".
                bw.Write(IM_Hello);
                bw.Flush();
                int hello = br.ReadInt32();
                if (hello == IM_Hello)
                {
                    // Hello packet is OK. Time to wait for login or register.
                    Console.WriteLine("Communication established!");
                    byte logMode = br.ReadByte();
                    Console.WriteLine(logMode.ToString());

                    string userName = br.ReadString();
                    if (userName != "")
                    {
                        Console.WriteLine(userName);
                    }
                    else 
                    {
                        userName = br.ReadString();
                    }
                    string password = br.ReadString();
                    if (password != "")
                    {
                        Console.WriteLine(password);
                    }
                    else {
                        password = br.ReadString();
                    }

                    string dateOfBirth = br.ReadString();
                    if (dateOfBirth != "")
                    {
                        Console.WriteLine(dateOfBirth.ToString());
                    }
                    else 
                    {
                        dateOfBirth = br.ReadString();
                    }
                    

                    if (userName.Length < 10) // Isn't username too long?
                    {
                        if (password.Length < 20)  // Isn't password too long?
                        {
                            if (logMode == IM_Register)  // Register mode
                            {
                                User check;
                                try
                                {
                                   check = userService.FindUser(userName);
                                }
                                catch (Exception e) 
                                {
                                    Console.WriteLine(e.Message);
                                    bw.Write(IM_DBProblem);
                                    bw.Flush();
                                    throw e;
                                }
                                if (check == null)  // User already exists?
                                {
                                    user = new User(userName, password, Convert.ToDateTime(dateOfBirth), this);
                                    try
                                    {
                                        userService.AddUser(user);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                        bw.Write(IM_DBProblem);
                                        bw.Flush();
                                        throw e;
                                    }
                                    bw.Write(IM_OK);
                                    bw.Flush();
                                    Console.WriteLine("[{0}] ({1}) Registered new user", DateTime.Now, userName);
                                    Receiver();  // Listen to client in loop.
                                }
                                else
                                    bw.Write(IM_Exists);
                                    bw.Flush();
                            }
                            else if (logMode == IM_Login)  // Login mode
                            {
                                try
                                {
                                    user = userService.FindUser(userName);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    bw.Write(IM_DBProblem);
                                    bw.Flush();
                                    throw e;
                                }
                                if (user != null)  // User exists?
                                {
                                    if (password == user.Password)  // Is password OK?
                                    {
                                        // If user is logged in yet, disconnect him.
                                        if (user.LoggedIn)
                                            user.Connection.CloseConn();

                                        user.Connection = this;
                                        bw.Write(IM_OK);
                                        bw.Flush();
                                        Receiver();  // Listen to client in loop.
                                    }
                                    else
                                    {
                                        bw.Write(IM_WrongPass);
                                        bw.Flush();
                                    }
                                }
                                else
                                {
                                    bw.Write(IM_NoExists);
                                    bw.Flush();
                                }
                            }
                        }
                        else
                        {
                            bw.Write(IM_TooPassword);
                            bw.Flush();
                        }
                    }
                    else
                    {
                        bw.Write(IM_TooUsername);
                        bw.Flush();
                    }
                }
                CloseConn();
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        private void CloseConn() // Close connection.
        {
                int index = prog.UserControllers.IndexOf(this);
                prog.UserControllers.RemoveAt(index);
                br.Close();
                bw.Close();
                netStream.Close();
                client.Close();
                Console.WriteLine("[{0}] End of connection!", DateTime.Now);
                if (user != null)
                {
                    user.LoggedIn = false;
                }
        }
        void Receiver()  // Receive all incoming packets.
        {
            Console.WriteLine("[{0}] ({1}) User logged in", DateTime.Now, user.UserName);
            user.LoggedIn = true;

            try
            {
                friends = new List<string>();
                allUsers = new List<string>();

                foreach(User tmp in userService.FindUserFriendList(this.user.UserName))
                {
                    friends.Add(tmp.UserName);
                }

                bin.Serialize(netStream, friends);
                Console.WriteLine("Serialized {0} friends!", friends.Count);

                foreach (User tmp in userService.FindAllUsers())
                {
                    if(tmp.UserName != this.user.UserName)
                    allUsers.Add(tmp.UserName);
                }

                bin.Serialize(netStream, allUsers);
                Console.WriteLine("Serialized {0} users!", allUsers.Count);

                while (client.Client.Connected)  // While we are connected.
                {
                    bool temp = false;

                    if (client.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (client.Client.Receive(buff, SocketFlags.Peek) == 0)
                        {
                            // Client disconnected
                            temp = true;
                        }
                    }
                    if (temp == true)
                    {
                        break;
                    }

                    byte type = br.ReadByte();

                    if (type == IM_FriendAdded) 
                    {
                        Console.WriteLine("Entered in the FriendAdded!");
                        bw.Write(IM_FriendAdded);
                        bw.Flush();
                        string addedFriend = br.ReadString();
                        
                        friendshipService.AddFriendship(user.UserName, addedFriend);
                    }
                    else if (type == IM_FriendAccepted)
                    {
                        Console.WriteLine("Entered in the FriendAccepted!");
                        bw.Write(IM_FriendAccepted);
                        bw.Flush();
                        string acceptedFriend = br.ReadString();

                        friendshipService.UpdateFriendship(user.UserName, acceptedFriend);
                    }
                    else if (type == IM_FriendDeleted) 
                    {
                        Console.WriteLine("Entered in the FriendDeleted!");
                        bw.Write(IM_FriendDeleted);
                        bw.Flush();
              
                        string deletedFriend = br.ReadString();
                        Console.WriteLine("Deleted friend: " + deletedFriend);

                        friendshipService.DeleteFriendship(user.UserName, deletedFriend);
                    }

                    else if (type == IM_IsAvailable)
                    {
                        string who = br.ReadString();

                        bw.Write(IM_IsAvailable);
                        bw.Write(who);
                        bool notInside = true;
                        foreach (var usercontroller in prog.UserControllers)
                        {
                            if (usercontroller.user.UserName.Equals(who))
                            {
                                Console.WriteLine("{0} is online in the chat!",who);
                                if (usercontroller.user.LoggedIn)
                                    bw.Write(true);   // Available
                                else
                                    bw.Write(false);  // Unavailable
                                bw.Flush();

                                notInside = false;
                            }
                        }

                        if (notInside) 
                        {
                            Console.WriteLine("User is not online!");
                            bw.Write(false);
                            bw.Flush();
                        }

                    }
                    else if (type == IM_Send)
                    {
                        string to = br.ReadString();
                        string msg = br.ReadString();
                        UserController tempController = prog.UserControllers.Find(x => x.user.UserName.Equals(to));

                        if (tempController != null)
                        {
                            // Is recipient logged in?
                            if (tempController.user.LoggedIn)
                            {
                                // Write received packet to recipient
                                tempController.user.Connection.bw.Write(IM_Received);
                                tempController.user.Connection.bw.Write(user.UserName);  // From
                                tempController.user.Connection.bw.Write(msg);
                                tempController.user.Connection.bw.Flush();
                                Console.WriteLine("[{0}] ({1} -> {2}) Message sent!", DateTime.Now, user.UserName,  tempController.user.UserName);
                            }
                        }
                    }

                    else if (type == IM_UpdateFriends) 
                    {
                        Console.WriteLine("Entered in the UpdateFriends!");

                        string updateFriend = br.ReadString();
                        UserController tempUserController = null;

                        Console.WriteLine(updateFriend);

                        foreach (var usercontroller in prog.UserControllers)
                        {
                            Console.WriteLine(usercontroller.user.UserName);
                            if (usercontroller.user.UserName.Equals(updateFriend))
                            {
                                if (usercontroller.user.LoggedIn)
                                {
                                    tempUserController = usercontroller;
                                }
                            }
                        }

                        if (tempUserController == null) 
                        {
                            continue;
                        }

                        tempUserController.bw.Write(IM_UpdateFriends);
                        tempUserController.bw.Flush();

                        tempUserController.friends = new List<string>();


                        foreach (User tmp in userService.FindUserFriendList(tempUserController.user.UserName))
                        {
                            tempUserController.friends.Add(tmp.UserName);
                        }

                        bin.Serialize(tempUserController.netStream, tempUserController.friends);
                        Console.WriteLine("Serialized new friendlist with {0} friends!", tempUserController.friends.Count);

                    }
                }
            }
            catch (Exception e) 
            {
                Console.WriteLine("Error poruka:" + e.Message);
            }

            user.LoggedIn = false;
            Console.WriteLine("[{0}] ({1}) User logged out", DateTime.Now, user.UserName);
        }

        public const int IM_Hello = 2012;      // Hello
        public const byte IM_OK = 0;           // OK
        public const byte IM_Login = 1;        // Login
        public const byte IM_Register = 2;     // Register
        public const byte IM_TooUsername = 3;  // Too long username
        public const byte IM_TooPassword = 4;  // Too long password
        public const byte IM_Exists = 5;       // Already exists
        public const byte IM_NoExists = 6;     // Doesn't exists
        public const byte IM_WrongPass = 7;    // Wrong password
        public const byte IM_IsAvailable = 8;  // Is user available?
        public const byte IM_Send = 9;         // Send message
        public const byte IM_Received = 10;    // Message received
        public const byte IM_Drop = 11;     //Tcp connection closed
        public const byte IM_DBProblem = 12;    //Database problem
        public const byte IM_FriendAdded = 13;  //FriendAdded
        public const byte IM_FriendAccepted = 14;  //Accepted Friend
        public const byte IM_FriendDeleted = 15;    //Friend Deleted
        public const byte IM_UpdateFriends = 16; //Update Friends list

    }

    }
