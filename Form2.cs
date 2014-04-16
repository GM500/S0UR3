using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace S0URC3
{
    public partial class Form2 : Form
    {
        public Form1 form1;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            form1.Create_Empty_Class(textBox1.Text);

        }

        private void textBox1_Leave(object sender, EventArgs e)
        {

        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Close();
                e.Handled = true;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
