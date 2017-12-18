using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Forms.VisualStyles;

namespace Pacman
{
    internal static class NativeMethods
    {
        public static IntPtr SendMessage(Control control, int msg, IntPtr wParam, IntPtr lParam)
        {
            MethodInfo WndProc = control.GetType().GetMethod(
                "WndProc",
                BindingFlags.NonPublic |
                BindingFlags.InvokeMethod |
                BindingFlags.FlattenHierarchy |
                BindingFlags.IgnoreCase |
                BindingFlags.Instance);

            object[] args = new object[] { new Message() {
                HWnd = control.Handle,
                LParam = lParam,
                WParam = wParam,
                Msg = (int)msg
                } };
            WndProc.Invoke(control, args);
            return ((Message)args[0]).Result;
        }

        public static void Suspend(Control c)
        {
            if (c != null && !c.IsDisposed)
            {
                SendMessage(c, WindowsMessages.SETREDRAW, IntPtr.Zero, IntPtr.Zero);
            }
        }
        public static void Resume(Control c)
        {
            if (c != null && !c.IsDisposed)
            {
                SendMessage(c, WindowsMessages.SETREDRAW, (IntPtr)(1), IntPtr.Zero);
                c.Invalidate(true);
            }
        }

        #region GDI
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);

        // The CreateDIBSection function creates a DIB that applications can write to directly.
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, uint iUsage, int ppvBits, IntPtr hSection, uint dwOffset);

        // This function transfers pixels from a specified source rectangle to a specified destination rectangle, altering the pixels according to the selected raster operation (ROP) code.
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        // This function creates a memory device context (DC) compatible with the specified device.
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        // This function selects an object into a specified device context. The new object replaces the previous object of the same type.
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        // The DeleteObject function deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources associated with the object. After the object is deleted, the specified handle is no longer valid.
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        // The DeleteDC function deletes the specified device context (DC).
        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hdc);

        // The SaveDC function saves the current state of the specified device context (DC) by copying data describing selected objects and graphic modes
        [DllImport("gdi32.dll")]
        public static extern int SaveDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        #endregion

        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        // This function releases a device context (DC), freeing it for use by other applications. The effect of ReleaseDC depends on the type of device context.
        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hdc, int state);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT ps);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT ps);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RedrawWindow(IntPtr hWnd, [In] ref RECT lprcUpdate, IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, ref RECT lpRect);

        #region DWM
        // Default window procedure for Desktop Window Manager (DWM) hit-testing within the non-client area.
        [DllImport("dwmapi.dll")]
        public static extern int DwmDefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, out IntPtr result);

        // Obtains a value that indicates whether Desktop Window Manager (DWM) composition is enabled.
        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);
        // Gets if computer is glass capable and enabled
        public static bool IsGlassEnabled
        {
            get
            {
                //Check what DWM says about composition
                int enabled = 0;
                int response = DwmIsCompositionEnabled(ref enabled);
                return enabled > 0;
            }
        }

        // Extends the window frame behind the client area.
        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        [DllImport("dwmapi.dll", EntryPoint = "#127")]
        private static extern void DwmGetColorizationParameters(ref DwmGetColorizationColors colors);
        public static Color GetWindowColorizationColor(bool opaque)
        {
            DwmGetColorizationColors c = new DwmGetColorizationColors();
            DwmGetColorizationParameters(ref c);
            return Color.FromArgb(
                (byte)(opaque ? 255 : c.ColorizationColor >> 24),
                (byte)(c.ColorizationColor >> 16),
                (byte)(c.ColorizationColor >> 8),
                (byte)c.ColorizationColor
                );
        }

        #endregion

        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        public static float ScalingFactor()
        {
            IntPtr desktop = Graphics.FromHwnd(IntPtr.Zero).GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);
            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;
            return ScreenScalingFactor; // 1.25 = 125%
        }

        // Draws text using the color and font defined by the visual style. Extends DrawThemeText by allowing additional text format options.
        [DllImport("UxTheme.dll", CharSet = CharSet.Unicode)]
        private static extern int DrawThemeTextEx(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags, ref RECT pRect, ref DTTOPTS pOptions);

        // Draws text using the color and font defined by the visual style.
        [DllImport("UxTheme.dll")]
        public static extern int DrawThemeText(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags1, int dwFlags2, ref RECT pRect);


        // Copies the source rectangle directly to the destination rectangle.
        public const int SRCCOPY = 0x00CC0020;
        // An uncompressed format.
        public const int BI_RGB = 0;
        // The BITMAPINFO structure contains an array of literal RGB values.
        public const int DIB_RGB_COLORS = 0;

        // Fills an area for glass rendering
        public static void FillForGlass(Graphics g, Rectangle r)
        {
            RECT rc = new RECT(r);
            IntPtr destdc = g.GetHdc();    //hwnd must be the handle of form, not control
            IntPtr Memdc = CreateCompatibleDC(destdc);
            IntPtr bitmap;
            IntPtr bitmapOld = IntPtr.Zero;

            BITMAPINFO dib = new BITMAPINFO();
            dib.bmiHeader.biHeight = r.Height; // -(rc.Bottom - rc.Top);
            dib.bmiHeader.biWidth = r.Width; // rc.Right - rc.Left;
            dib.bmiHeader.biPlanes = 1;
            dib.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            dib.bmiHeader.biBitCount = 32;
            dib.bmiHeader.biCompression = BI_RGB;
            if (!(SaveDC(Memdc) == 0))
            {
                bitmap = CreateDIBSection(Memdc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);
                if (!(bitmap == IntPtr.Zero))
                {
                    bitmapOld = SelectObject(Memdc, bitmap);
                    BitBlt(destdc, rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top, Memdc, 0, 0, SRCCOPY);

                }

                //Remember to clean up
                SelectObject(Memdc, bitmapOld);
                DeleteObject(bitmap);
                ReleaseDC(Memdc, -1);
                DeleteDC(Memdc);
            }
            g.ReleaseHdc();
        }

        private const int DTT_COMPOSITED = (int)(1UL << 13);
        private const int DTT_GLOWSIZE = (int)(1UL << 11);

        private const int uFormat = DrawText.SINGLELINE | DrawText.CENTER | DrawText.VCENTER | DrawText.NOPREFIX;

        // Draws theme text on glass.
        public static void DrawTextOnGlass(Graphics graphics, String text, Font font, Rectangle bounds, int glowSize)
        {
            if (IsGlassEnabled)
            {
                IntPtr destdc = IntPtr.Zero;
                try
                {
                    destdc = graphics.GetHdc();
                    IntPtr Memdc = CreateCompatibleDC(destdc);   // Set up a memory DC where we'll draw the text.
                    IntPtr bitmap;
                    IntPtr bitmapOld = IntPtr.Zero;
                    IntPtr logfnotOld;

                    //int uFormat = DT_SINGLELINE | DT_CENTER | DT_VCENTER | DT_NOPREFIX;   //text format

                    BITMAPINFO dib = new BITMAPINFO();
                    dib.bmiHeader.biHeight = -bounds.Height;         // negative because DrawThemeTextEx() uses a top-down DIB
                    dib.bmiHeader.biWidth = bounds.Width;
                    dib.bmiHeader.biPlanes = 1;
                    dib.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
                    dib.bmiHeader.biBitCount = 32;
                    dib.bmiHeader.biCompression = BI_RGB;
                    if (!(SaveDC(Memdc) == 0))
                    {
                        bitmap = CreateDIBSection(Memdc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);   // Create a 32-bit bmp for use in offscreen drawing when glass is on
                        if (!(bitmap == IntPtr.Zero))
                        {
                            bitmapOld = SelectObject(Memdc, bitmap);
                            IntPtr hFont = font.ToHfont();
                            logfnotOld = SelectObject(Memdc, hFont);
                            try
                            {

                                VisualStyleRenderer renderer = new VisualStyleRenderer(VisualStyleElement.Window.Caption.Active);

                                DTTOPTS dttOpts = new DTTOPTS();
                                dttOpts.dwSize = (uint)Marshal.SizeOf(typeof(DTTOPTS));
                                dttOpts.dwFlags = DTT_COMPOSITED | DTT_GLOWSIZE;
                                dttOpts.iGlowSize = glowSize;

                                RECT rc2 = new RECT(0, 0, bounds.Width, bounds.Height);
                                int dtter = DrawThemeTextEx(renderer.Handle, Memdc, 0, 0, text, -1, uFormat, ref rc2, ref dttOpts);
                                bool bbr = BitBlt(destdc, bounds.Left, bounds.Top, bounds.Width, bounds.Height, Memdc, 0, 0, SRCCOPY);
                                if (!bbr)
                                {
                                    //throw new Exception("???");
                                }
                            }
                            catch (Exception)
                            {
                                //Log.Print(e.ToString());
                                //throw new Exception("???");
                            }

                            //Remember to clean up
                            SelectObject(Memdc, bitmapOld);
                            SelectObject(Memdc, logfnotOld);
                            DeleteObject(bitmap);
                            DeleteObject(hFont);

                            ReleaseDC(Memdc, -1);
                            DeleteDC(Memdc);
                        }
                        else
                        {
                            //throw new Exception("???");
                        }
                    }
                    else
                    {
                        //throw new Exception("???");
                    }
                }
                finally
                {
                    if (destdc != IntPtr.Zero)
                        graphics.ReleaseHdc(destdc);
                }
            }
        }

    }
}
