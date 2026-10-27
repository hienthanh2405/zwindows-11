using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using zwindowscore.Enum;

namespace zwindowscore.Utils.UI
{
    public class Noti : Form
    {
        private Label lblMessage;
        private System.Windows.Forms.Timer _autoOffTimer;
        private System.Windows.Forms.Timer _fadeTimer;
        private int _initialStyle;
        private byte _currentAlpha = 230;

        public Noti()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.DoubleBuffered = true;

            BackColor = Color.FromArgb(28, 28, 28);
            lblMessage.ForeColor = Color.FromArgb(245, 245, 245);
            lblMessage.BackColor = Color.Transparent;
            TopMost = true;
            ShowInTaskbar = false;
            MinimizeBox = false;
            MaximizeBox = false;

            Load += Noti_Load;
            Resize += Noti_Resize;
            Paint += Noti_Paint;
        }

        private void Noti_Load(object sender, EventArgs e)
        {
            _initialStyle = Win32Helper.GetWindowLong(this.Handle, -20);
            Win32Helper.SetWindowLong(this.Handle, -20, _initialStyle | 0x80000 | 0x20); // WS_EX_LAYERED | WS_EX_TRANSPARENT
            Win32Helper.SetLayeredWindowAttributes(this.Handle, 0, _currentAlpha, Win32Helper.LWA_ALPHA);
            ApplyRoundedRegion();
        }

        private void Noti_Resize(object sender, EventArgs e)
        {
            ApplyRoundedRegion();
            Invalidate();
        }

        private void ApplyRoundedRegion()
        {
            if (this.Width > 0 && this.Height > 0)
            {
                try
                {
                    int radius = Math.Min(this.Height, 24);
                    IntPtr hRgn = Win32Helper.CreateRoundRectRgn(0, 0, this.Width + 1, this.Height + 1, radius, radius);
                    Win32Helper.SetWindowRgn(this.Handle, hRgn, true);
                }
                catch { }
            }
        }

        private void Noti_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw modern subtle border
            using (var pen = new Pen(Color.FromArgb(65, 65, 65), 1.5f))
            {
                int r = Math.Min(this.Height - 2, 22);
                using (var path = GetRoundedRectPath(new Rectangle(1, 1, this.Width - 2, this.Height - 2), r))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }

            // Draw glowing accent indicator dot
            using (var brush = new SolidBrush(Color.FromArgb(96, 205, 255))) // Windows 11 Fluent Blue accent
            {
                e.Graphics.FillEllipse(brush, 14, (this.Height - 8) / 2, 8, 8);
            }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            // top left arc
            path.AddArc(arc, 180, 90);

            // top right arc
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        public string Message
        {
            set
            {
                lblMessage.Text = value;
                UpdateNotificationSize();
            }
            get
            {
                return lblMessage.Text;
            }
        }

        private void UpdateNotificationSize()
        {
            using (var g = this.CreateGraphics())
            {
                var size = g.MeasureString(lblMessage.Text, lblMessage.Font);
                int calculatedWidth = Math.Max(180, (int)size.Width + 60);
                int calculatedHeight = 42;
                this.Size = new Size(calculatedWidth, calculatedHeight);
            }
            ApplyRoundedRegion();
        }

        public void ShowFloatingForXMilliSeconds(int milliSeconds)
        {
            _currentAlpha = 230;
            if (this.IsHandleCreated)
            {
                Win32Helper.SetLayeredWindowAttributes(this.Handle, 0, _currentAlpha, Win32Helper.LWA_ALPHA);
            }

            if (_fadeTimer != null)
            {
                _fadeTimer.Stop();
                _fadeTimer.Dispose();
                _fadeTimer = null;
            }

            Show();

            if (_autoOffTimer == null)
            {
                _autoOffTimer = new System.Windows.Forms.Timer();
                _autoOffTimer.Tick += OnAutoOffTimerElapsed;
            }
            else
            {
                _autoOffTimer.Stop();
            }
            _autoOffTimer.Interval = milliSeconds;
            _autoOffTimer.Start();
        }

        private void OnAutoOffTimerElapsed(object sender, EventArgs e)
        {
            _autoOffTimer?.Stop();
            StartFadeOut();
        }

        private void StartFadeOut()
        {
            if (_fadeTimer != null) return;
            _fadeTimer = new System.Windows.Forms.Timer();
            _fadeTimer.Interval = 25;
            _fadeTimer.Tick += (s, ev) =>
            {
                if (_currentAlpha > 25)
                {
                    _currentAlpha -= 25;
                    Win32Helper.SetLayeredWindowAttributes(this.Handle, 0, _currentAlpha, Win32Helper.LWA_ALPHA);
                }
                else
                {
                    _fadeTimer?.Stop();
                    _fadeTimer?.Dispose();
                    _fadeTimer = null;
                    Dispose();
                }
            };
            _fadeTimer.Start();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var Params = base.CreateParams;
                Params.ExStyle |= 0x80;       // WS_EX_TOOLWINDOW
                Params.ExStyle |= 0x80000;    // WS_EX_LAYERED
                Params.ExStyle |= 0x20;       // WS_EX_TRANSPARENT
                Params.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
                return Params;
            }
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        private void InitializeComponent()
        {
            this.lblMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // lblMessage
            Font fontToUse;
            try
            {
                fontToUse = new Font("Segoe UI Variable Text", 10.5F, FontStyle.Bold);
            }
            catch
            {
                fontToUse = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            }

            this.lblMessage.Font = fontToUse;
            this.lblMessage.ForeColor = System.Drawing.Color.FromArgb(245, 245, 245);
            this.lblMessage.Location = new System.Drawing.Point(30, 0);
            this.lblMessage.Dock = DockStyle.Fill;
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.TextAlign = ContentAlignment.MiddleCenter;
            this.lblMessage.Padding = new Padding(24, 0, 16, 0);

            // Noti Form
            this.ClientSize = new System.Drawing.Size(220, 42);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Noti";
            this.ResumeLayout(false);
        }

        #region Static

        private static Dictionary<string, Noti> _notis = new Dictionary<string, Noti>();

        public static void ShowMessage(string message, int miliSeconds, int x = 0, int y = 0)
        {
            foreach (var md in Global.MonitorDevices)
            {
                if (!Global.Settings.MonitorsLayouts.ContainsKey(md.Id)) continue;
                var monitor = Global.Settings.MonitorsLayouts[md.Id];

                Noti current = null;
                bool isNew = false;
                if (_notis.ContainsKey(monitor.Id))
                {
                    current = _notis[monitor.Id];
                }

                if (current != null && !current.IsDisposed)
                {
                    isNew = false;
                }
                else
                {
                    isNew = true;
                }

                if (isNew)
                {
                    current = new Noti();
                    _notis[monitor.Id] = current;
                }

                current.Message = message;

                // Center horizontally at the top of the monitor (Windows 11 Flyout style)
                int targetX = (x != 0) ? monitor.X + x : monitor.X + (monitor.Width - current.Width) / 2;
                int targetY = (y != 0) ? monitor.Y + y : monitor.Y + 45;
                current.Location = new Point(targetX, targetY);

                current.ShowFloatingForXMilliSeconds(miliSeconds);
            }
        }

        #endregion
    }
}
