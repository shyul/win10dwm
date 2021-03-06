﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            CaptionBut bt1 = new CaptionBut();
            TPanel tp1 = new TPanel();
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            Font = new Font("Segoe UI", 10, FontStyle.Bold);
            InitializeComponent();

            tp1.Location = new Point(0, TOPEXTENDWIDTH);
            tp1.Size = new Size(ClientRectangle.Size.Width, ClientRectangle.Size.Height - TOPEXTENDWIDTH);


            bt1.Location = new Point(2, 2);
            bt1.Size = new Size(20, 20);

            Controls.Add(bt1);
            Controls.Add(tp1);

            Console.WriteLine("SystemInformation.CaptionHeight = " + SystemInformation.CaptionHeight);
            Console.WriteLine("SystemInformation.BorderSize.Height = " + SystemInformation.BorderSize.Height);
            Console.WriteLine("SystemInformation.BorderSize.Width = " + SystemInformation.BorderSize.Width);
            Console.WriteLine("SystemInformation.Border3DSize.Height = " + SystemInformation.Border3DSize.Height);
            Console.WriteLine("SystemInformation.Border3DSize.Width = " + SystemInformation.Border3DSize.Width);
            Console.WriteLine("SystemInformation.FixedFrameBorderSize.Height = " + SystemInformation.FixedFrameBorderSize.Height);
            Console.WriteLine("SystemInformation.FixedFrameBorderSize.Width = " + SystemInformation.FixedFrameBorderSize.Width);
        }

        private static IntPtr MSG_HANDLED = new IntPtr(0);
        private MARGINS _tMargins = new MARGINS(0, 0, TOPEXTENDWIDTH, 0);
        private bool m_painting = false;
        private const int BLACK_BRUSH = 4;
        private static StringFormat TabTextFormat = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };

        private void DrawCaption(IntPtr HWnd)
        {
            PAINTSTRUCT ps = new PAINTSTRUCT();
            NativeMethods.BeginPaint(HWnd, ref ps);
            NativeMethods.FillRect(ps.hdc, ref ps.rcPaint, NativeMethods.GetStockObject(BLACK_BRUSH));

            Graphics g = Graphics.FromHdc(ps.hdc);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (WindowState == FormWindowState.Maximized)
            {
                g.DrawLine(Pens.Red, 0, 8, 100, 100);
                Rectangle rcx = new Rectangle(300, 8, 100, 20);
                g.FillRectangle(Brushes.DarkOrange, rcx);
                g.DrawString("U1234504", Font, Brushes.White, rcx, TabTextFormat);
            }
            else
            {
                g.DrawLine(Pens.Red, 0, 0, 100, 100);
                Rectangle rcx = new Rectangle(300, 1, 100, 20);
                g.FillRectangle(Brushes.DarkOrange, rcx);
                g.DrawString("U1234504", Font, Brushes.White, rcx, TabTextFormat);
            }

            NativeMethods.EndPaint(HWnd, ref ps);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (WindowsMessages.CREATE):
                    {
                        RECT rcClient = new RECT();
                        NativeMethods.GetWindowRect(this.Handle, ref rcClient);
                        NativeMethods.SetWindowPos(this.Handle, IntPtr.Zero,
                                     rcClient.Left, rcClient.Top,
                                     rcClient.Right - rcClient.Left, rcClient.Bottom - rcClient.Top,
                                     SWP.FRAMECHANGED);
                        m.Result = MSG_HANDLED;
                        base.WndProc(ref m);
                        break;
                    }
                case (WindowsMessages.DWMCOMPOSITIONCHANGED):
                case (WindowsMessages.ACTIVATE):
                    {
                        NativeMethods.DwmExtendFrameIntoClientArea(this.Handle, ref _tMargins);
                        m.Result = MSG_HANDLED;
                        base.WndProc(ref m);
                        break;
                    }
                case (WindowsMessages.PAINT):
                    {
                        if (!m_painting)
                        {
                            m_painting = true;
                            DrawCaption(m.HWnd);
                            m_painting = false;
                        }
                        base.WndProc(ref m);
                        break;
                    }
                case (WindowsMessages.NCCALCSIZE):
                    {
                        if (m.WParam != IntPtr.Zero && m.Result == IntPtr.Zero)
                        {
                            NCCALCSIZE_PARAMS nc = (NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(NCCALCSIZE_PARAMS));
                            nc.rect0.Top -= SystemInformation.CaptionHeight + 8;// SystemInformation.CaptionHeight + 1;
                            nc.rect1 = nc.rect0;
                            Marshal.StructureToPtr(nc, m.LParam, false);
                            m.Result = WVR.VALIDRECTS; //MSG_HANDLED;// (IntPtr)WVR_VALIDRECTS;
                        }
                        base.WndProc(ref m);
                        break;
                    }
                case (WindowsMessages.NCHITTEST):
                    {
                        if (m.Result == NCHITTEST.NOWHERE)
                        {
                            IntPtr res = IntPtr.Zero;
                            if (NativeMethods.DwmDefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam, ref res))
                            {
                                m.Result = res;
                            }
                            else
                            {
                                m.Result = HitTestNCA(Handle);
                                //Console.WriteLine(res.ToString());
                                //m.Result = res;
                            }
                        }
                        else
                            base.WndProc(ref m);
                        break;
                    }
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        // Hit test (HTTOPLEFT, ... HTBOTTOMRIGHT)
        private const int EDGEGRIPWIDTH = 8;
        private const int TOPEXTENDWIDTH = 31;

        private static IntPtr HitTestNCA(IntPtr hWnd)
        {
            // Get the point coordinates for the hit test.
            //POINT ptMouse = { GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam) };

            Point ptMouse = Control.MousePosition;

            // Get the window rectangle.
            RECT rcWindow = new RECT();
            NativeMethods.GetWindowRect(hWnd, ref rcWindow); // this.Handle


            // Get the frame rectangle, adjusted for the style without a caption.
            RECT rcFrame = new RECT();
            NativeMethods.AdjustWindowRectEx(ref rcFrame, WindowStyles.OVERLAPPEDWINDOW & ~WindowStyles.CAPTION, false, 0); // The last is supposed to be NULL

            // Determine if the hit test is for resizing. Default middle (1,1).
            int uRow = 1;
            int uCol = 1;
            //bool fOnResizeBorder = false;

            // Determine if the point is at the left or right of the window.
            if (ptMouse.X >= rcWindow.Left && ptMouse.X < rcWindow.Left + EDGEGRIPWIDTH)
            {
                uCol = 0; // left side
            }
            else if (ptMouse.X < rcWindow.Right && ptMouse.X >= rcWindow.Right - EDGEGRIPWIDTH)
            {
                uCol = 2; // right side
            }

            // Determine if the point is at the top or bottom of the window.
            if (ptMouse.Y >= rcWindow.Top && ptMouse.Y < rcWindow.Top + TOPEXTENDWIDTH)
            {
                if (ptMouse.Y < (rcWindow.Top - rcFrame.Top))
                {
                    uRow = 0;
                }
                else if(uCol == 1)
                {
                    return NCHITTEST.CAPTION;
                }
            }
            else if (ptMouse.Y < rcWindow.Bottom && ptMouse.Y >= rcWindow.Bottom - EDGEGRIPWIDTH)
            {
                uRow = 2;
            }

            IntPtr[,] hitTests =  { { NCHITTEST.TOPLEFT,    NCHITTEST.TOP,      NCHITTEST.TOPRIGHT },
                                    { NCHITTEST.LEFT,       NCHITTEST.NOWHERE,  NCHITTEST.RIGHT },
                                    { NCHITTEST.BOTTOMLEFT, NCHITTEST.BOTTOM,   NCHITTEST.BOTTOMRIGHT } };

            return hitTests[uRow, uCol];
        }
    }

    public class TPanel : Panel
    {
        public TPanel()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        
            //Location = new Point(0, 57);
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.FillRectangle(new SolidBrush(Color.Wheat), ClientRectangle);
            g.DrawLine(Pens.Blue, ClientRectangle.Left, ClientRectangle.Top, ClientRectangle.Right, ClientRectangle.Bottom);
            g.DrawString("I am on the title bar!", new Font("Tahoma", 10, FontStyle.Bold), Brushes.Gray, 0, 4);
        }
    }

    public class CaptionBut : UserControl
    {
        public CaptionBut()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.Clear(Color.Transparent);
            g.DrawLine(Pens.Green, 5, 20, 20, 0);
            //base.OnPaint(e);
        }

    }
}
