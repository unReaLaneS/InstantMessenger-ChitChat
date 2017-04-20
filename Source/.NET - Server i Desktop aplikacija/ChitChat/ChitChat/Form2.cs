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
    public partial class Registration : Form
    {
        private User user;
        private Form1 form;

        public Registration(Form1 form1)
        {
            InitializeComponent();
            user = new User();
            form = form1;
            user.RegisterOK += new EventHandler(im_RegisterOK);
            user.RegisterFailed += new ErrorEventHandler(im_RegisterFailed);
            user.Disconnected += new EventHandler(im_Disconnected);
        }

        void im_RegisterOK(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                Form form3 = new Main(user, form);
                form3.Show();
                form.Hide();
                this.Close();
            }));
        }

        void im_RegisterFailed(object sender, ErrorEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                errorProvider1.SetError(this.textBox1, "");

                if (e.Error.ToString() == "Exists")
                    errorProvider1.SetError(this.textBox1, "Username already taken!");
            }));
        }

        private void im_Disconnected(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                MessageBox.Show("Unable to connect to the server!");
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text.Trim();
            maskedTextBox2.Text.Trim();
            maskedTextBox1.Text.Trim();
            if(textBox1.Text == "")
            {
                errorProvider1.SetError(textBox1,"Enter username!");
            }
            else if (maskedTextBox1.Text == "")
            {
                errorProvider1.SetError(maskedTextBox1, "Enter password!");
            }
            else if (maskedTextBox2.Text == "")
            {
                errorProvider1.SetError(maskedTextBox2, "Confirm password!");
            }
            else
            {
                MessageBox.Show(dateTimePicker1.Value.Date.ToString());
                user.Register(textBox1.Text, maskedTextBox1.Text, dateTimePicker1.Value.Date);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            form.Show();
            this.Close();
        }

        private void textBox1_Validated(object sender, EventArgs e)
        {
            textBox1.Text.Trim();

            if (textBox1.Text == "")
            {
                errorProvider1.SetError(textBox1, "Username missing!");
            }
            else {
                errorProvider1.SetError(textBox1, "");
            }

        }

        private void maskedTextBox1_Validated(object sender, EventArgs e)
        {

        }

        private void maskedTextBox2_Validated(object sender, EventArgs e)
        {
            if (maskedTextBox1.Text == maskedTextBox2.Text)
            {
                errorProvider1.SetError(maskedTextBox2, "");
            }
            else errorProvider1.SetError(maskedTextBox2, "Passwords don't match!");
        }


    }
}
