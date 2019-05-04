using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HttpServer
{
    public partial class Functions : Form
    {
        Point lastPoint;
        public TextBox inputBox;
        public Functions()
        {
            InitializeComponent();
        }
        public Functions(int width, int d, KeyEventHandler key_down)
        {
            InitializeComponent();

            Width = width + 2 * d;
            Height = 20 + 2 * d;

            inputBox = new TextBox();
            inputBox.Location = new Point(d, d);
            inputBox.Width = width;
            Controls.Add(inputBox);
            inputBox.KeyDown += key_down;
            inputBox.Focus();
        }

        private void Functions_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Left += e.X - lastPoint.X;
                Top += e.Y - lastPoint.Y;
            }
        }

        private void Functions_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }
    }
}
