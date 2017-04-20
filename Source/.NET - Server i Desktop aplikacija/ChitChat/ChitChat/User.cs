using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChitChat
{
    public class User
    {
        private Thread tcpThread;      // Receiver
        private bool _conn = false;    // Is connected/connecting?
        private bool _logged = false;  // Is logged in?
        private string _user;          // Username
        private string _pass;          // Password
        private DateTime dateOfBirth;
        private bool reg;              // Register mode
        private List<string> loadfriends;
        private List<string> allUsers;
        private string friend;


        public string Server { get { return "localhost"; } }  // Address of server. In this case - local IP address.
        public int Port { get { return 2000; } }

        public bool IsLoggedIn { get { return _logged; } }
        public string UserName { get { return _user; } }
        public string Password { get { return _pass; } }
        public DateTime DateOfBirth{ get { return dateOfBirth; }}

        public List<string> Loadfriends
        {
            get { return loadfriends; }
            set { loadfriends = value; }
        }

        public List<string> AllUsers
        {
            get { return allUsers; }
            set { allUsers = value; }
        }

        public string Friend
        {
            get { return friend; }
            set { friend = value; }
        }

        // Start connection thread and login or register.
        void connect(string user, string password, bool register, DateTime date = new DateTime())
        {
            if (!_conn)
            {
                _conn = true;
                _user = user;
                _pass = password;
                dateOfBirth = date;
                reg = register;
                tcpThread = new Thread(new ThreadStart(SetupConn));
                tcpThread.Start();
            }
        }
        public void Login(string user, string password)
        {
            connect(user, password, false);
        }
        public void Register(string user, string password, DateTime date)
        {
            connect(user, password, true, date);
        }
        public void Disconnect()
        {
            if (_conn)
                CloseConn();
        }

        public void IsAvailable(string user)
        {
            if (_conn)
            {
                bw.Write(IM_IsAvailable);
                bw.Write(user);
                bw.Flush();
            }
        }

        public void friendAdded() 
        {
            Trace.WriteLine("We entered in the friendAdded!");
            bw.Write(IM_FriendAdded);
            bw.Flush();
        }

        public void friendAccepted() 
        {
            bw.Write(IM_FriendAccepted);
            bw.Flush();
        }

        public void friendDeleted()
        {
            bw.Write(IM_FriendDeleted);
            bw.Flush();
        }

        public void updateFriends(string friendName) 
        {
            bw.Write(IM_UpdateFriends);
            bw.Write(friendName);
            bw.Flush();
        }

        public void SendMessage(string to, string msg)
        {
            if (_conn)
            {
                bw.Write(IM_Send);
                bw.Write(to);
                bw.Write(msg);
                bw.Flush();
            }
        }

        // Events
        public event EventHandler LoginOK;
        public event EventHandler RegisterOK;
        public event ErrorEventHandler LoginFailed;
        public event ErrorEventHandler RegisterFailed;
        public event EventHandler Disconnected;
        public event AvailEventHandler UserAvailable;
        public event ReceivedEventHandler MessageReceived;
        public event EventHandler UpdateForm;

        virtual protected void OnLoginOK()
        {
            if (LoginOK != null)
                LoginOK(this, EventArgs.Empty);
        }
        virtual protected void OnRegisterOK()
        {
            if (RegisterOK != null)
                RegisterOK(this, EventArgs.Empty);
        }
        virtual protected void OnLoginFailed(ErrorEventArgs e)
        {
            if (LoginFailed != null)
                LoginFailed(this, e);
        }
        virtual protected void OnRegisterFailed(ErrorEventArgs e)
        {
            if (RegisterFailed != null)
                RegisterFailed(this, e);
        }
        virtual protected void OnDisconnected()
        {
            if (Disconnected != null)
                Disconnected(this, EventArgs.Empty);
        }
        virtual protected void OnUserAvail(AvailEventArgs e)
        {
            if (UserAvailable != null)
                UserAvailable(this, e);
        }
        virtual protected void OnMessageReceived(ReceivedEventArgs e)
        {
            if (MessageReceived != null)
                MessageReceived(this, e);
        }

        virtual protected void onUpdateForm() 
        {
            if (UpdateForm != null)
                UpdateForm(this,EventArgs.Empty);
        }

        private TcpClient client;
        private NetworkStream netStream;
        private BinaryReader br;
        private BinaryWriter bw;
        private BinaryFormatter bin;

        void SetupConn()
        {
            try
            {
                client = new TcpClient(Server, Port);
                netStream = client.GetStream();

                br = new BinaryReader(netStream, Encoding.UTF8);
                bw = new BinaryWriter(netStream, Encoding.UTF8);



                // Receive "hello"
                int hello = br.ReadInt32();
                if (hello == IM_Hello)
                {
                    // Hello OK, so answer.
                    bw.Write(IM_Hello);

                    bw.Write(reg ? IM_Register : IM_Login);  // Login or register
                    bw.Write(UserName);
                    bw.Write(Password);
                    bw.Write(Convert.ToString(dateOfBirth));
                    bw.Flush();

                    byte ans = br.ReadByte();  // Read answer.
                    if (ans == IM_OK)  // Login/register OK
                    {

                        if (reg)
                            OnRegisterOK();  // Register is OK.
                        OnLoginOK();  // Login is OK (when registered, automatically logged in)
                        Receiver(); // Time for listening for incoming messages.
                    }
                    else
                    {
                        ErrorEventArgs err = new ErrorEventArgs((Error)ans);
                        if (reg)
                            OnRegisterFailed(err);
                        else
                            OnLoginFailed(err);
                    }
                }
                if (_conn)
                {
                    CloseConn();
                }
            }
            catch (Exception) 
            {
               /* try
                {*/
                    OnDisconnected();
                /*}
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }*/

                _conn = false;
            }
        }

        void CloseConn() // Close connection.
        {
                br.Close();
                bw.Close();
                netStream.Close();
                client.Close();
                _conn = false;
        }

        void Receiver()  // Receive all incoming packets.
        {
            _logged = true;

            try
            {
                bin = new BinaryFormatter();

                loadfriends = (List<string>)bin.Deserialize(netStream);

                allUsers = (List<string>)bin.Deserialize(netStream);

                friend = "";
                while (client.Connected)  // While we are connected.
                { 

                    byte type = br.ReadByte();  // Get incoming packet type.

                    if (type == IM_IsAvailable)
                    {
                        string user = br.ReadString();
                        bool isAvail = br.ReadBoolean();
                        OnUserAvail(new AvailEventArgs(user, isAvail));
                    }
                    else if (type == IM_Received)
                    {
                        string from = br.ReadString();
                        string msg = br.ReadString();
                        OnMessageReceived(new ReceivedEventArgs(from, msg));
                    }

                    else if (type == IM_FriendAdded)
                    {
                        Trace.WriteLine("Entered into FriendAdded!");
                        bw.Write(Loadfriends.Last());
                        bw.Flush();
                        updateFriends(Loadfriends.Last());
                        Trace.WriteLine("Exited from FriendAdded!");
                    }

                    else if (type == IM_FriendAccepted)
                    {
                        Trace.WriteLine("Entered into FriendAccepted!");
                        bw.Write(friend);
                        bw.Flush();
                        updateFriends(friend);
                        Trace.WriteLine("Exited from FriendAccepted!");
                    }

                    else if (type == IM_FriendDeleted)
                    {
                        Trace.WriteLine("Entered into FriendDeleted!");
                        bw.Write(friend);
                        bw.Flush();
                        updateFriends(friend);
                        Trace.WriteLine("Exited from FriendDeleted!");
                    }
                    else if (type == IM_UpdateFriends) 
                    {
                        Trace.WriteLine("Entered into UpdateFriends!");

                        loadfriends = (List<string>)bin.Deserialize(netStream);

                        onUpdateForm();
                        Trace.WriteLine("Exited from UpdateFriends!");
                    }
                }
            }
            catch (Exception e) {

                Trace.WriteLine(e.Message);

                throw new IOException();
            }

            _logged = false;
        }


        // Packet types
        public const int IM_Hello = 2012;      // Hello
        public const byte IM_OK = 0;           // OK
        public const byte IM_Login = 1;        // Login
        public const byte IM_Register = 2;     // Register
        public const byte IM_TooUsername = 3;  // Too long username
        public const byte IM_TooPassword = 4;  // Too long password
        public const byte IM_Exists = 5;       // Already exists
        public const byte IM_NoExists = 6;     // Doesn't exist
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
