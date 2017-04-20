using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace Server
{
    [Table("AppUser")]
    public class User
    {
        private int id;
        private string userName;
        private string password;
        private DateTime dateOfBirth;
        private bool loggedIn;
        private UserController connection;

        public User() { }

        public User(string user, string pass,DateTime date)
        {
            this.userName = user;
            this.password = pass;
            this.dateOfBirth = date;
            this.loggedIn = true;
        }

        public User(string user, string pass, DateTime date, UserController conn)
        {
            this.userName = user;
            this.password = pass;
            this.dateOfBirth = date;
            this.loggedIn = true;
            this.connection = conn;
        }

        [Key]
        [Column("UserId")]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [StringLength(24)]
        [Index(IsUnique = true)]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        [NotMapped]
        public bool LoggedIn
        {
            get { return loggedIn; }
            set { loggedIn = value; }
        }

        [NotMapped]
        public UserController Connection
        {
            get { return connection; }
            set { connection = value; }
        }

        public DateTime DateOfBirth
        {
            get { return dateOfBirth; }
            set { dateOfBirth = value; }
        }
    }
}
