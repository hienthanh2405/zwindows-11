using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using zwindowscore.Enum;

namespace zwindowscore.Utils.UI
{
    public class LayoutOverlay : Form
    {
        public LayoutOverlay()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.DoubleBuffered = true;

            Color baseColor = Global.GetColor(Global.Settings.WindowDragOverlayColor);
            BackColor = baseColor;
            ShowInTaskbar = false;
            MinimizeBox = false;
            MaximizeBox = false;

            Load += LayoutOverlay_Load;
            Resize += LayoutOverlay_Resize;
            Paint += LayoutOverlay_Paint;
        }

        private void LayoutOverlay_Load(object sender, EventArgs e)
        {
            // Set layered window with modern opacity (~55% opacity = 140/255)
            Win32Helper.SetLayeredWindowAttributes(this.Handle, 0, 140, Win32Helper.LWA_ALPHA);
            ApplyRoundedCorners();
        }

        private void LayoutOverlay_Resize(object sender, EventArgs e)
        {
            ApplyRoundedCorners();
            Invalidate();
        }

        private void ApplyRoundedCorners()
        {
            if (this.Width > 0 && this.Height > 0)
            {
                try
                {
                    IntPtr hRgn = Win32Helper.CreateRoundRectRgn(0, 0, this.Width + 1, this.Height + 1, 16, 16);
                    Win32Helper.SetWindowRgn(this.Handle, hRgn, true);
                }
                catch { }
            }
        }

        private void LayoutOverlay_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw modern crisp glowing inner border
            Color borderColor = Color.FromArgb(200, 240, 255);
            using (var pen = new Pen(borderColor, 2.5f))
            {
                using (var path = GetRoundedRectPath(new Rectangle(2, 2, this.Width - 4, this.Height - 4), 14))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var Params = base.CreateParams;
                Params.ExStyle |= 0x80;       // WS_EX_TOOLWINDOW
                Params.ExStyle |= 0x80000;    // WS_EX_LAYERED
                Params.ExStyle |= 0x20;       // WS_EX_TRANSPARENT (click-through)
                Params.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE (never steal focus)
                return Params;
            }
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(0, 0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LayoutOverlay";
            this.Padding = new System.Windows.Forms.Padding(15);
            this.ResumeLayout(false);
        }

        #region Static

        private static LayoutOverlay _current = null;
        private static int _lastX = -1, _lastY = -1, _lastW = -1, _lastH = -1;

        public static void ShowOverlay(IntPtr hwndOwner, int x, int y, int width, int height)
        {
            // Apply refined FancyZones-style inner margin gap (4px)
            const int gap = 4;
            int targetX = x + gap;
            int targetY = y + gap;
            int targetW = Math.Max(20, width - gap * 2);
            int targetH = Math.Max(20, height - gap * 2);

            if (_current == null || _current.IsDisposed)
            {
                _current = new LayoutOverlay();
                _current.Show();
            }

            if (!_current.Visible)
            {
                _current.Visible = true;
            }

            if (_lastX != targetX || _lastY != targetY || _lastW != targetW || _lastH != targetH)
            {
                _lastX = targetX;
                _lastY = targetY;
                _lastW = targetW;
                _lastH = targetH;

                Win32Helper.SetWindowPos(_current.Handle, Win32Helper.HWND_TOP, targetX, targetY, targetW, targetH,
                    WindowPosFlags.DoNotActivate | WindowPosFlags.ShowWindow);
            }
        }

        public static void ShowOverlay(int x, int y, int width, int height)
        {
            ShowOverlay(IntPtr.Zero, x, y, width, height);
        }

        public static void HideOverlay()
        {
            if (_current != null && !_current.IsDisposed && _current.Visible)
            {
                _current.Visible = false;
                _lastX = _lastY = _lastW = _lastH = -1;
            }
        }

        public static void ClearOverlay()
        {
            HideOverlay();
        }

        #endregion
    }
}
