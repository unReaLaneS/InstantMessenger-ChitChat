using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChitChat
{

    public enum Error : byte
    {
        Exists = User.IM_Exists,
        NoExists = User.IM_NoExists,
        WrongPassword = User.IM_WrongPass,
        DatabaseFailed = User.IM_DBProblem
    }

    public class ErrorEventArgs : EventArgs
    {
        Error err;

        public ErrorEventArgs(Error error)
        {
            this.err = error;
        }

        public Error Error
        {
            get { return err; }
        }
    }

    public class AvailEventArgs : EventArgs
    {
        string user;
        bool avail;

        public AvailEventArgs(string user, bool avail)
        {
            this.user = user;
            this.avail = avail;
        }

        public string UserName
        {
            get { return user; }
        }
        public bool IsAvailable
        {
            get { return avail; }
        }
    }
    public class ReceivedEventArgs : EventArgs
    {
        string user;
        string msg;

        public ReceivedEventArgs(string user, string msg)
        {
            this.user = user;
            this.msg = msg;
        }

        public string From
        {
            get { return user; }
        }
        public string Message
        {
            get { return msg; }
        }
    }

    public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);
    public delegate void AvailEventHandler(object sender, AvailEventArgs e);
    public delegate void ReceivedEventHandler(object sender, ReceivedEventArgs e);
}
