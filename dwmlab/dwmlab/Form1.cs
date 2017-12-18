using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pacman
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            g.FillRectangle(new SolidBrush(Color.Wheat), ClientRectangle);
            g.DrawLine(Pens.Green, 0, 0, Width - 1, Height - 1);
            g.DrawString("I am on the title bar!", new Font("Tahoma", 10, FontStyle.Bold), Brushes.Gray, 0, 4);
            //g.FillEllipse(Brushes.Black, this.Width - 40, this.Height - 40, 80, 80);
        }
        private const int WVR_ALIGNTOP = 0x0010;
        private const int WVR_ALIGNLEFT = 0x0020;
        private const int WVR_ALIGNBOTTOM = 0x0040;
        private const int WVR_ALIGNRIGHT = 0x0080;
        private const int WVR_HREDRAW = 0x0100;
        private const int WVR_VREDRAW = 0x0200;
        private const int WVR_REDRAW = (WVR_HREDRAW | WVR_VREDRAW);
        private const int WVR_VALIDRECTS = 0x400;
        private static IntPtr MSG_HANDLED = new IntPtr(0);


        protected override void WndProc(ref Message m)
        {

            switch (m.Msg)
            {
                case (WindowsMessages.CREATE):
                    RECT rcClient = new RECT();
                    NativeMethods.GetWindowRect(this.Handle, ref rcClient);
                    // force a calc size message
                    NativeMethods.SetWindowPos(this.Handle, IntPtr.Zero,
                                 rcClient.Left, rcClient.Top,
                                 rcClient.Right - rcClient.Left, rcClient.Bottom - rcClient.Top,
                                 SWP.FRAMECHANGED);
                    m.Result = MSG_HANDLED;
                    base.WndProc(ref m);
                    break;
                /*
            case (WindowsMessages.DWMCOMPOSITIONCHANGED):
            case (WindowsMessages.ACTIVATE):
                MARGINS margins;

                margins.cxLeftWidth = 10;
                margins.cxRightWidth = 10; 
                margins.cyBottomHeight = 0;
                margins.cyTopHeight = 30;

                NativeMethods.DwmExtendFrameIntoClientArea(this.Handle, ref margins);
                m.Result = MSG_HANDLED;
                base.WndProc(ref m);
                break;*/

                case (WindowsMessages.PAINT):

                    base.WndProc(ref m);
                    break;
                case (WindowsMessages.NCCALCSIZE):
                    if (m.WParam != IntPtr.Zero && m.Result == IntPtr.Zero)
                    {
                        NCCALCSIZE_PARAMS nc = (NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(NCCALCSIZE_PARAMS));
                        nc.rect0.Top -= 31;
                       // nc.rect1 = nc.rect0;
                        Marshal.StructureToPtr(nc, m.LParam, false);
                        m.Result = MSG_HANDLED;// (IntPtr)WVR_VALIDRECTS;
                    }
                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }


  










        }
    }
}
