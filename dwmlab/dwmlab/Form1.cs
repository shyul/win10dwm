using System;
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
            InitializeComponent();

            tp1.Location = new Point(0, TOPEXTENDWIDTH);
            tp1.Size = new Size(ClientRectangle.Size.Width, ClientRectangle.Size.Height - TOPEXTENDWIDTH);

            button1.BackColor = Color.Transparent;

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
        /*
        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            g.FillRectangle(new SolidBrush(Color.Wheat), ClientRectangle);
            g.DrawLine(Pens.Green, ClientRectangle.Left, ClientRectangle.Top, ClientRectangle.Right, ClientRectangle.Bottom);
            g.DrawString("I am on the title bar!", new Font("Tahoma", 10, FontStyle.Bold), Brushes.Gray, 0, 4);
            //g.FillEllipse(Brushes.Black, this.Width - 40, this.Height - 40, 80, 80);
        }*/


        private const int WVR_ALIGNTOP = 0x0010;
        private const int WVR_ALIGNLEFT = 0x0020;
        private const int WVR_ALIGNBOTTOM = 0x0040;
        private const int WVR_ALIGNRIGHT = 0x0080;
        private const int WVR_HREDRAW = 0x0100;
        private const int WVR_VREDRAW = 0x0200;
        private const int WVR_REDRAW = (WVR_HREDRAW | WVR_VREDRAW);
        private const int WVR_VALIDRECTS = 0x400;
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
            //g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (WindowState == FormWindowState.Maximized)
            {
                g.DrawLine(Pens.Red, 0, 8, 100, 100);
                Rectangle rcx = new Rectangle(300, 8, 100, 20);
                g.FillRectangle(Brushes.DarkOrange, rcx);
                g.DrawString("U1234504", Font, Brushes.Black, rcx, TabTextFormat);
            }
            else
            {
                g.DrawLine(Pens.Red, 0, 0, 100, 100);
                Rectangle rcx = new Rectangle(300, 1, 100, 20);
                g.FillRectangle(Brushes.DarkOrange, rcx);
                g.DrawString("U1234504", Font, Brushes.Black, rcx, TabTextFormat);
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
                            m.Result = MSG_HANDLED;// (IntPtr)WVR_VALIDRECTS;
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
                                m.Result = res;
                            else
                                m.Result = HitTestNCA(Handle);
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
            bool fOnResizeBorder = false;

            // Determine if the point is at the top or bottom of the window.
            if (ptMouse.Y >= rcWindow.Top && ptMouse.Y < rcWindow.Top + TOPEXTENDWIDTH)
            {
                fOnResizeBorder = (ptMouse.Y < (rcWindow.Top - rcFrame.Top));
                uRow = 0;
            }
            else if (ptMouse.Y < rcWindow.Bottom && ptMouse.Y >= rcWindow.Bottom - EDGEGRIPWIDTH)
            {
                uRow = 2;
            }

            // Determine if the point is at the left or right of the window.
            if (ptMouse.X >= rcWindow.Left && ptMouse.X < rcWindow.Left + EDGEGRIPWIDTH)
            {
                uCol = 0; // left side
            }
            else if (ptMouse.X < rcWindow.Right && ptMouse.X >= rcWindow.Right - EDGEGRIPWIDTH)
            {
                uCol = 2; // right side
            }

            IntPtr[,] hitTests = { { NCHITTEST.TOPLEFT, fOnResizeBorder ? NCHITTEST.TOP : NCHITTEST.CAPTION, NCHITTEST.TOPRIGHT },
                    { NCHITTEST.LEFT, NCHITTEST.NOWHERE, NCHITTEST.RIGHT },
                    { NCHITTEST.BOTTOMLEFT, NCHITTEST.BOTTOM, NCHITTEST.BOTTOMRIGHT } };

            return hitTests[uRow, uCol];
        }

        /*
        private IntPtr HitTest()
        {
            RECT windowRect = new RECT();
            Point cursorPoint = Control.MousePosition;
            RECT posRect;

            //Point Pt = PointToClient(Control.MousePosition);

            NativeMethods.GetWindowRect(this.Handle, ref windowRect);
            cursorPoint.X -= windowRect.Left;
            cursorPoint.Y -= windowRect.Top;
            int width = windowRect.Right - windowRect.Left;
            int height = windowRect.Bottom - windowRect.Top;

            posRect = new RECT(0, 0, FrameWidth, FrameHeight);
            if (NativeMethods.PtInRect(ref posRect, cursorPoint))
                return NCHITTEST.TOPLEFT;

            posRect = new RECT(width - FrameWidth, 0, width, FrameHeight);
            if (NativeMethods.PtInRect(ref posRect, cursorPoint))
                return NCHITTEST.TOPRIGHT;

            posRect = new RECT(FrameWidth, 0, width - (FrameWidth * 2) - _iFrameOffset, FrameHeight);
            if (NativeMethods.PtInRect(ref posRect, cursorPoint))
                return NCHITTEST.TOP;

            posRect = new RECT(FrameWidth, FrameHeight, width - ((FrameWidth * 2) + _iFrameOffset), _tMargins.cyTopHeight);
            if (NativeMethods.PtInRect(ref posRect, cursorPoint))
                return NCHITTEST.CAPTION;

            posRect = new RECT(0, FrameHeight, FrameWidth, height - FrameHeight);
            if (NativeMethods.PtInRect(ref posRect, cursorPoint))
                return NCHITTEST.LEFT;

            posRect = new RECT(0, height - FrameHeight, FrameWidth, height);
            if (NativeMethods.PtInRect(ref posRect, cursorPoint))
                return NCHITTEST.BOTTOMLEFT;

            posRect = new RECT(FrameWidth, height - FrameHeight, width - FrameWidth, height);
            if (NativeMethods.PtInRect(ref posRect, cursorPoint))
                return NCHITTEST.BOTTOM;

            posRect = new RECT(width - FrameWidth, height - FrameHeight, width, height);
            if (NativeMethods.PtInRect(ref posRect, cursorPoint))
                return NCHITTEST.BOTTOMRIGHT;

            posRect = new RECT(width - FrameWidth, FrameHeight, width, height - FrameHeight);
            if (NativeMethods.PtInRect(ref posRect, cursorPoint))
                return NCHITTEST.RIGHT;

            return NCHITTEST.CLIENT;
        }*/
        /*
        private void PaintThis(IntPtr hdc, RECT rc)
        {
            RECT clientRect = new RECT();
            NativeMethods.GetClientRect(this.Handle, ref clientRect);
            clientRect.Left = _tClientRect.Left - _tMargins.cxLeftWidth;
            clientRect.Top = _tMargins.cyTopHeight;
            clientRect.Right -= _tMargins.cxRightWidth;
            clientRect.Bottom -= _tMargins.cyBottomHeight;

            int clr;
            IntPtr hb;
            using (ClippingRegion cp = new ClippingRegion(hdc, clientRect, rc))
            {
                FillRect(hdc, ref rc, GetStockObject(BLACK_BRUSH));
            }
            clr = ColorTranslator.ToWin32(this.BackColor);
            hb = CreateSolidBrush(clr);
            NativeMethods.FillRect(hdc, ref clientRect, hb);
            DeleteObject(hb);
        }*/
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
            g.FillRectangle(new SolidBrush(Color.Wheat), ClientRectangle);
            g.DrawLine(Pens.Red, ClientRectangle.Left, ClientRectangle.Top, ClientRectangle.Right, ClientRectangle.Bottom);
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
            g.Clear(Color.Transparent);
            g.DrawLine(Pens.Green, 5, 20, 20, 0);
            //base.OnPaint(e);
        }

    }
}
