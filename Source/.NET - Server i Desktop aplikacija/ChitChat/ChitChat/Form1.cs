using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChitChat
{
    public partial class Form1 : Form
    {
        private User user;
        private EventHandler disconnectHandler;

        public Form1()
        {
            InitializeComponent();
        }

        void im_LoginOK(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                textBox1.Text = "";
                maskedTextBox1.Text = "";
                errorProvider1.SetError(this.maskedTextBox1, "");
                errorProvider1.SetError(this.textBox1, "");
                Form form3 = new Main(user,this);
                user.Disconnected -= disconnectHandler;
                form3.Show();
                this.Hide();
            }));
        }

        void im_LoginFailed(object sender, ErrorEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                errorProvider1.SetError(this.maskedTextBox1, "");
                errorProvider1.SetError(this.textBox1, "");

                if (e.Error.ToString() == "WrongPassword")
                    errorProvider1.SetError(this.maskedTextBox1, "Wrong password!");
                else if(e.Error.ToString() == "NoExists") {
                    errorProvider1.SetError(this.textBox1, "User doesn't exist!");
                }
            }));
        }

        private void im_Disconnected(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                MessageBox.Show("Unable to connect to the server!");
            }));
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form form2 = new Registration(this);
            form2.Show();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text.Trim();
            maskedTextBox1.Text.Trim();
            if (textBox1.Text == "")
            {
                errorProvider1.SetError(textBox1, "Enter username!");
            }
            else if (maskedTextBox1.Text == "")
            {
                errorProvider1.SetError(maskedTextBox1, "Enter password!");
            }
            else
            {
                user = new User();
                user.LoginOK += new EventHandler(im_LoginOK);
                user.LoginFailed += new ErrorEventHandler(im_LoginFailed);
                disconnectHandler = new EventHandler(im_Disconnected);
                user.Disconnected += disconnectHandler;
                user.Login(textBox1.Text, maskedTextBox1.Text);
            }

        }

        private void Form1_Enter(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
    }
}
