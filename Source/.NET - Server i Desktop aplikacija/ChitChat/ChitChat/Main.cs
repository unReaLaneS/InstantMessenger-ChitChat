using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChitChat
{
    public partial class Main : Form
    {
        private User user;
        private Form form;
        private BindingSource bs;
        private string sendTo = "";


        private AvailEventHandler availHandler;
        private ReceivedEventHandler receivedHandler;
        private EventHandler disconnectHandler;
        private EventHandler updateHandler;

        public Main(User user, Form form)
        {
            InitializeComponent();

            this.user = user;
            this.form = form;
            label1.Text = user.UserName;
            foreach (Control item in splitContainer1.Panel2.Controls)
            {
                item.Hide();
            }

            Thread.Sleep(300);

            bs = new BindingSource();
            bs.DataSource = user.Loadfriends;
            listBox1.DataSource = bs;
            bs.ResetBindings(false);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            availHandler = new AvailEventHandler(im_UserAvailable);
            receivedHandler = new ReceivedEventHandler(im_MessageReceived);
            user.UserAvailable += availHandler;
            user.MessageReceived += receivedHandler;
            disconnectHandler = new EventHandler(im_Disconnected);
            user.Disconnected += disconnectHandler;
            updateHandler = new EventHandler(im_UpdateForm);
            user.UpdateForm += updateHandler;

        }

        private void im_Disconnected(object sender, EventArgs e)
        {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    MessageBox.Show("Connection with server lost!");
                    form.Show();
                    this.Close();
                }));

        }

        void im_UserAvailable(object sender, AvailEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                if (e.UserName == sendTo)
                {
                    if (e.IsAvailable == true)
                    {
                        label4.Text = "Online";
                    }
                }
            }));
        }

        void im_MessageReceived(object sender, ReceivedEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                if (e.From == sendTo)
                {
                    textBox3.Select(textBox3.TextLength, 0);
                    textBox3.SelectionColor = Color.Green;
                    textBox3.AppendText(String.Format("[{0}] ", e.From));
                    textBox3.Select(textBox3.TextLength, 0);
                    textBox3.SelectionColor = Color.Black;
                    textBox3.AppendText(String.Format("{0}\r\n", e.Message));
                    textBox2.Text = "";
                }
            }));
        }

        void im_UpdateForm(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                bs.DataSource = user.Loadfriends;
                bs.ResetBindings(false);
            }));
        }


        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            user.Disconnected -= disconnectHandler;
            user.Disconnect();
            form.Show();
            this.Close();
        }

        private void onlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Online";
        }

        private void awayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Away";
        }

        private void busyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Busy";
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox3.Clear();
            if (listBox1.SelectedValue != null)
            {
                if (listBox1.SelectedValue.ToString().Contains("(Pending)"))
                {
                    label3.Text = listBox1.SelectedValue.ToString();
                    label4.Text = "Offline";

                    foreach (Control item in splitContainer1.Panel2.Controls)
                    {
                        item.Hide();
                    }

                    panel4.Show();
                    label5.Show();
                    button2.Show();
                    button3.Show();

                    panel4.BringToFront();
                }
                else
                {

                    label3.Text = listBox1.SelectedValue.ToString();
                    label4.Text = "Offline";

                    foreach (Control item in splitContainer1.Panel2.Controls)
                    {
                        item.Show();
                    }

                    panel4.SendToBack();

                    sendTo = listBox1.SelectedValue.ToString();
                    user.IsAvailable(sendTo);
                    Thread.Sleep(100);
                }
            }
            else if (user.Loadfriends.Count() > 1)
            {
                listBox1.SetSelected(0, true);

                foreach (Control item in splitContainer1.Panel2.Controls)
                {
                    item.Hide();
                }
            }

        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            user.Disconnected -= disconnectHandler;
            user.Disconnect();
            form.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

            if (user.AllUsers.Contains(textBox1.Text))
            {
                if (!user.Loadfriends.Contains(textBox1.Text))
                {
                    user.Loadfriends.Add(textBox1.Text);
                    bs.ResetBindings(false);
                    user.friendAdded();
                    textBox1.Clear();
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            user.Friend = listBox1.SelectedValue.ToString().Replace("(Pending)", "");
            user.friendAccepted();
            int index = user.Loadfriends.IndexOf(listBox1.SelectedValue.ToString());
            user.Loadfriends[index] = user.Loadfriends[index].Replace("(Pending)", "");
            label3.Text = user.Friend;
            label4.Text = "Offline";

            bs.DataSource = user.Loadfriends;
            bs.ResetBindings(false);

            foreach (Control item in splitContainer1.Panel2.Controls)
            {
                item.Show();
            }

            panel4.Hide();
            label5.Hide();
            button2.Hide();
            button3.Hide();

            user.IsAvailable(sendTo);
            Thread.Sleep(100);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            user.Friend = listBox1.SelectedValue.ToString().Replace("(Pending)", "");
            Thread.Sleep(100);
            user.friendDeleted();
            user.Loadfriends.Remove(listBox1.SelectedValue.ToString());

            bs.DataSource = user.Loadfriends;
            bs.ResetBindings(false);

            foreach (Control item in splitContainer1.Panel2.Controls)
            {
                item.Hide();
            }

            panel4.Hide();
            label5.Hide();
            button2.Hide();
            button3.Hide();

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void listBox1_DataSourceChanged(object sender, EventArgs e)
        {

            bs.DataSource = user.Loadfriends;
            bs.ResetBindings(false);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            user.SendMessage(sendTo, textBox2.Text);
            textBox3.Select(textBox3.TextLength, 0);
            textBox3.SelectionColor = Color.Blue;
            textBox3.AppendText(String.Format("[{0}] ", user.UserName));
            textBox3.Select(textBox3.TextLength, 0);
            textBox3.SelectionColor = Color.Black;
            textBox3.AppendText(String.Format("{0}\r\n", textBox2.Text));

            textBox2.Text = "";
        }

        private void panel3_VisibleChanged(object sender, EventArgs e)
        {
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            List<string> completeUsers = new List<string>();

            foreach (var item in user.AllUsers)
	        {
		        if(user.Loadfriends.FirstOrDefault(s => s.Contains(item)) == null)
                {
                    completeUsers.Add(item);
                }
	        }

            var autoComplete = new AutoCompleteStringCollection();
            autoComplete.AddRange(completeUsers.ToArray());
            textBox1.AutoCompleteCustomSource = autoComplete;
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listBox1.SelectedValue.ToString().Contains("(Pending)"))
                {
                    return;
                }
                if (user.Loadfriends.Count() != 0)
                {
                    //select the item under the mouse pointer
                    listBox1.SelectedIndex = listBox1.IndexFromPoint(e.Location);
                    if (listBox1.SelectedIndex != -1)
                    {
                        contextMenuStrip1.Show();
                    }
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            user.Friend = listBox1.SelectedValue.ToString().Replace("(Pending)", "");
            Thread.Sleep(100);
            user.friendDeleted();
            user.Loadfriends.Remove(user.Friend);

            bs.DataSource = user.Loadfriends;
            bs.ResetBindings(false);

            foreach (Control item in splitContainer1.Panel2.Controls)
            {
                item.Hide();
            }

            panel4.Hide();
            label5.Hide();
            button2.Hide();
            button3.Hide();
        }



    }
}
