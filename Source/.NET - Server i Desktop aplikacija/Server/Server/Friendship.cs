using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Friendship
    {
        private int id;
        private int userid;
        private int friendid;
        private bool accepted;

        public Friendship() 
        {
            
        }

        public Friendship(int p1, int p2)
        {
            this.userid = p1;
            this.friendid = p2;
            this.accepted = false;
        }

        [Key]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public bool Accepted
        {
            get { return accepted; }
            set { accepted = value; }
        }

        public int Userid
        {
            get { return userid; }
            set { userid = value; }
        }
        
        public int Friendid
        {
            get { return friendid; }
            set { friendid = value; }
        }

        [ForeignKey("Userid")]
        public virtual User User { get; set; }

        [ForeignKey("Friendid")]
        public virtual User Friend { get; set; }

    }
}
