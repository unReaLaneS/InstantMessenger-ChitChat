package com.example.unreal.chitchat;

import android.content.Loader;
import android.util.Log;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.InetAddress;
import java.net.Socket;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.Scanner;

/**
 * Created by unReaL on 1/1/2017.
 */

public class Client {

    public static final String SERVER_IP = "192.168.0.105"; //your computer IP address
    public static final int SERVER_PORT = 2000;

    // message to send to the server
    //private String mServerMessage;
    // sends message received notifications
    //private OnMessageReceived mMessageListener = null;
    // while this is true, the server will continue running


    private boolean mRun = false;
    // used to send messages
    //private PrintWriter out;
    // used to read messages from the server
    //private BufferedReader in;
    private DataOutputStream out;
    private DataInputStream in;
    private boolean _conn = false;    // Is connected/connecting?
    private boolean _logged = false;  // Is logged in?
    private String _user;          // Username
    private String _pass;          // Password
    private Date dateOfBirth;
    private boolean reg;              // Register mode
    private ArrayList<String> loadfriends;
    private ArrayList<String> allUsers;
    private String friend;

    public LoginCheck mLoginListener = null;

    public boolean isLogged() {
        return _logged;
    }

    public void setLogged(boolean _logged) {
        this._logged = _logged;
    }
    public boolean isConn() {
        return _conn;
    }

    public void setConn(boolean _conn) {
        this._conn = _conn;
    }

    public String getUser() {
        return _user;
    }

    public void setUser(String _user) {
        this._user = _user;
    }

    public String getPass() {
        return _pass;
    }

    public void setPass(String _pass) {
        this._pass = _pass;
    }

    public Date getDateOfBirth() {
        return dateOfBirth;
    }

    public void setDateOfBirth(Date dateOfBirth) {
        this.dateOfBirth = dateOfBirth;
    }

    public boolean isReg() {
        return reg;
    }

    public void setReg(boolean reg) {
        this.reg = reg;
    }

    public ArrayList<String> getLoadfriends() {
        return loadfriends;
    }

    public void setLoadfriends(ArrayList<String> loadfriends) {
        this.loadfriends = loadfriends;
    }

    public ArrayList<String> getAllUsers() {
        return allUsers;
    }

    public void setAllUsers(ArrayList<String> allUsers) {
        this.allUsers = allUsers;
    }

    public String getFriend() {
        return friend;
    }

    public void setFriend(String friend) {
        this.friend = friend;
    }

    /**
     * Constructor of the class. OnMessagedReceived listens for the messages received from server
     */
    /*public Client(/*OnMessageReceived listener) {
        //mMessageListener = listener;
    }*/

    /**
     * Sends the message entered by client to the server
     *
     * @param message text entered by client
     */
    /*public void sendMessage(String message) {
        if (mBufferOut != null && !mBufferOut.checkError()) {
            mBufferOut.println(message);
            mBufferOut.flush();
        }
    }*/

    /**
     * Close the connection and release the members
     */
    public void stopClient() {
        try {

            mRun = false;

            if (out != null) {
                out.flush();
                out.close();
            }

            mLoginListener = null;
            in = null;
            out = null;
        }
        catch(Exception e)
        {
            Log.e("Error","Thrown when stopping client!");

        }
    }

    private void connect(String user, String password, boolean register, Date date)
    {
        if (!_conn)
        {
            _conn = true;
            _user = user;
            _pass = password;
            dateOfBirth = date;
            reg = register;
            run();
        }
    }
    public void Login(String user, String password)
    {
        connect(user, password, false, new Date());
    }
    public void Register(String user, String password, Date date)
    {
        connect(user, password, true, date);
    }

    int little2big(int i) {
        return (i&0xff)<<24 | (i&0xff00)<<8 | (i&0xff0000)>>8 | (i>>24)&0xff;
    }


    public void run() {

        try {

            //here you must put your computer's IP address.
            InetAddress serverAddr = InetAddress.getByName(SERVER_IP);

            Log.e("TCP Client", "C: Connecting...");

            //create a socket to make the connection with the server

            Socket socket = new Socket(serverAddr, SERVER_PORT);

            Log.e("Socket",socket.toString());

            try {

                //sends the message to the server
                //out = new PrintWriter(new BufferedWriter(new OutputStreamWriter(socket.getOutputStream())), true);
                out = new DataOutputStream(socket.getOutputStream());

                //receives the message which the server sends back
                //in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
                in = new DataInputStream(socket.getInputStream());

                int hello = in.readInt();

                hello = little2big(hello);

                if (hello == IM_Hello)
                {
                    // Hello OK, so answer.
                    out.writeInt(little2big(IM_Hello));

                    out.writeByte(reg ? IM_Register : IM_Login);  // Login or register
                    while(_user.isEmpty()) {}
                    out.writeUTF(_user);
                    while(_pass.isEmpty()) {}
                    out.writeUTF(_pass);
                    DateFormat format = new SimpleDateFormat("MM/dd/yyyy hh:mm a");
                    while(dateOfBirth == null) {}
                    out.writeUTF(format.format(dateOfBirth));
                    out.flush();

                    byte ans = in.readByte();  // Read answer.
                    if (ans == IM_OK)  // Login/register OK
                    {

                        if (reg)
                            Log.e("Register","OK");
                        byte[] test = new byte[209];
                        in.readFully(test);

                        byte[] ime1 = new byte[5];
                        in.readFully(ime1);
                        String name1 = new String(ime1, "UTF-8");
                        loadfriends = new ArrayList<String>();
                        loadfriends.add(name1);
                        Log.e("Primio",name1);
                        byte[] garbage = new byte[5];
                        in.readFully(garbage);
                        String gar1 = new String(garbage, "UTF-8");
                        byte[] ime2 = new byte[5];
                        in.readFully(ime2);
                        String name2 = new String(ime2, "UTF-8");
                        loadfriends.add(name2);
                        byte[] garbage1 = new byte[5];
                        in.readFully(garbage1);
                        String gar2 = new String(garbage1, "UTF-8");
                        byte[] ime3 = new byte[5];
                        in.readFully(ime3);
                        String name3 = new String(ime3, "UTF-8");
                        loadfriends.add(name3);
                        mLoginListener.OnLoginOk();
                            /*OnRegisterOK();  // Register is OK.
                        OnLoginOK();  // Login is OK (when registered, automatically logged in)*/
                        try {
                            Receiver(); // Time for listening for incoming messages.
                        }
                        catch(Exception e)
                        {
                            throw e;
                        }
                        /*Log.e("Logged in","Success");*/

                    }
                    else
                    {
                        Log.e("Login","Failed");
                        //ErrorEventArgs err = new ErrorEventArgs((Error)ans);
                        /*if (reg)
                            OnRegisterFailed(err);
                        else*/
                            mLoginListener.OnLoginFailed();
                    }
                }
                //Log.e("RESPONSE FROM SERVER", "S: Received Message: '" + mServerMessage + "'");

            } catch (Exception e) {

                Log.e("TCP", "S: Error", e);

            } finally {
                //the socket must be closed. It is not possible to reconnect to this socket
                // after it is closed, which means a new socket instance has to be created.
                socket.close();
            }

        } catch (Exception e) {

            Log.e("TCP", "C: Error", e);

        }

    }

    public void Receiver() throws Exception
    {
        try {
            mRun = true;




            while (mRun) {



            }
        }
        catch(Exception e)
        {
            Log.e("Receiver",e.toString());

            throw new Exception();
        }

    }


    //Declare the interface. The method messageReceived(String message) will must be implemented in the MyActivity
    //class at on asynckTask doInBackground
    public interface LoginCheck {
        public void OnLoginOk();
        public void OnLoginFailed();
    }
    public static final int IM_Hello = 2012;      // Hello
    public static final byte IM_OK = 0;           // OK
    public static final byte IM_Login = 1;        // Login
    public static final byte IM_Register = 2;     // Register
    public static final byte IM_TooUsername = 3;  // Too long username
    public static final byte IM_TooPassword = 4;  // Too long password
    public static final byte IM_Exists = 5;       // Already exists
    public static final byte IM_NoExists = 6;     // Doesn't exist
    public static final byte IM_WrongPass = 7;    // Wrong password
    public static final byte IM_IsAvailable = 8;  // Is user available?
    public static final byte IM_Send = 9;         // Send message
    public static final byte IM_Received = 10;    // Message received
    public static final byte IM_Drop = 11;     //Tcp connection closed
    public static final byte IM_DBProblem = 12;    //Database problem
    public static final byte IM_FriendAdded = 13;  //FriendAdded
    public static final byte IM_FriendAccepted = 14;  //Accepted Friend
    public static final byte IM_FriendDeleted = 15;    //Friend Deleted
    public static final byte IM_UpdateFriends = 16; //Update Friends list

}
