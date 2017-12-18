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

            Console.WriteLine(SystemInformation.CaptionHeight);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            g.FillRectangle(new SolidBrush(Color.Wheat), ClientRectangle);
            g.DrawLine(Pens.Green, ClientRectangle.Left, ClientRectangle.Top, ClientRectangle.Right, ClientRectangle.Bottom);
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
        private MARGINS _tMargins = new MARGINS(0, 0, 62, 0);
        private bool m_painting = false;
        private const int BLACK_BRUSH = 4;

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
                        PAINTSTRUCT ps = new PAINTSTRUCT();
                        if (!m_painting)
                        {
                            m_painting = true;
                            NativeMethods.BeginPaint(m.HWnd, ref ps);
                            NativeMethods.FillRect(ps.hdc, ref ps.rcPaint, NativeMethods.GetStockObject(BLACK_BRUSH));
                            NativeMethods.EndPaint(m.HWnd, ref ps);
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
                                m.Result = HitTest();
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

        private IntPtr HitTest()
        {
            RECT windowRect = new RECT();
            Point cursorPoint = new Point();
            RECT posRect;
            NativeMethods.GetCursorPos(ref cursorPoint);
            NativeMethods.GetWindowRect(this.Handle, ref windowRect);
            cursorPoint.X -= windowRect.Left;
            cursorPoint.Y -= windowRect.Top;
            int width = windowRect.Right - windowRect.Left;
            int height = windowRect.Bottom - windowRect.Top;

            posRect = new RECT(0, 0, FrameWidth, FrameHeight);
            if (PtInRect(ref posRect, cursorPoint))
                return HIT_CONSTANTS.HTTOPLEFT;

            posRect = new RECT(width - FrameWidth, 0, width, FrameHeight);
            if (PtInRect(ref posRect, cursorPoint))
                return HIT_CONSTANTS.HTTOPRIGHT;

            posRect = new RECT(FrameWidth, 0, width - (FrameWidth * 2) - _iFrameOffset, FrameHeight);
            if (PtInRect(ref posRect, cursorPoint))
                return HIT_CONSTANTS.HTTOP;

            posRect = new RECT(FrameWidth, FrameHeight, width - ((FrameWidth * 2) + _iFrameOffset), _tMargins.cyTopHeight);
            if (PtInRect(ref posRect, cursorPoint))
                return HIT_CONSTANTS.HTCAPTION;

            posRect = new RECT(0, FrameHeight, FrameWidth, height - FrameHeight);
            if (PtInRect(ref posRect, cursorPoint))
                return HIT_CONSTANTS.HTLEFT;

            posRect = new RECT(0, height - FrameHeight, FrameWidth, height);
            if (PtInRect(ref posRect, cursorPoint))
                return HIT_CONSTANTS.HTBOTTOMLEFT;

            posRect = new RECT(FrameWidth, height - FrameHeight, width - FrameWidth, height);
            if (PtInRect(ref posRect, cursorPoint))
                return HIT_CONSTANTS.HTBOTTOM;

            posRect = new RECT(width - FrameWidth, height - FrameHeight, width, height);
            if (PtInRect(ref posRect, cursorPoint))
                return HIT_CONSTANTS.HTBOTTOMRIGHT;

            posRect = new RECT(width - FrameWidth, FrameHeight, width, height - FrameHeight);
            if (PtInRect(ref posRect, cursorPoint))
                return HIT_CONSTANTS.HTRIGHT;

            return HIT_CONSTANTS.HTCLIENT;
        }
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
}
