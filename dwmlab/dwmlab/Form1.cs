using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pacman
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            g.FillRectangle(new SolidBrush(Color.Wheat), ClientRectangle);
            g.DrawLine(Pens.Green, 0, 0, 100, 100);
            g.DrawString("I am on the title bar!", new Font("Tahoma", 10, FontStyle.Bold), Brushes.Gray, 0, 4);
            g.FillEllipse(Brushes.Black, this.Width - 40, this.Height - 40, 80, 80);
        }
    }
}
