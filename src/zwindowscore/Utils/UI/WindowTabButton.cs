using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using zwindowscore;
using zwindowscore.Enum;
using zwindowscore.Options;
using zwindowscore.Utils;

namespace zwindowscore.Utils
{
    public class WindowTabButton
    {
        private Label _label;
        private Label _lblDesktopIndex;
        private ToolTip _tooltip;
        private PictureBox _icon;
        public IntPtr Hwnd { get; set; }
        public CustomPanel Panel { get; set; }
        public ScreenTabButtonsBar Parent { get; set; }
        public MonitorLayout Layout { get; set; }
        private Color _iconColor;
        private Color _currentColor;
        private Color _currentTextColor;
        private bool _isActive;
        private string _desktopName;
        public bool TitleEvent { get; set; } = false;
        public bool IsWindowTopMost
        {
            get
            {
                return Win32Helper.IsWindowTopMost(Hwnd);
            }
        }
        private int DragState = 0;

        public string Desktop
        {
            set
            {
                ChangeDesktop(value, Global.CurrentDesktopName);
                _tooltip.SetToolTip(_label, $"Desktop: {value}");
                _label.ResumeLayout();
            }
        }

        public WindowTabButton()
        {
            TitleEvent = Global.Settings != null ? Global.Settings.DefaultWindowsTabNotificationOn : false;

            var tmp = (int)Math.Round((Global.Settings.TabButtonHeight - Global.Settings.TabButtonIconSize) / 2f);
            var labelX = Global.Settings.TabButtonIconSize + 4;
            _label = new Label();
            _label.Text = "";
            _label.ForeColor = Color.White;
            _label.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            _label.AutoSize = false;
            _label.AutoEllipsis = true;
            _label.Location = new Point(labelX, 0);
            _label.Size = new System.Drawing.Size(Global.Settings.TabButtonWidth - labelX, Global.Settings.TabButtonHeight);
            _label.TextAlign = ContentAlignment.MiddleLeft;
            _label.UseCompatibleTextRendering = true;

            _tooltip = new ToolTip();
            _tooltip.SetToolTip(_label, "Click to activate");
            _tooltip.ShowAlways = true;

            _icon = new PictureBox();
            _icon.Width = Global.Settings.TabButtonIconSize;
            _icon.Height = Global.Settings.TabButtonIconSize;
            _icon.SizeMode = PictureBoxSizeMode.StretchImage;
            _icon.Location = new Point(2, tmp);
            _icon.Enabled = false;
            
            _lblDesktopIndex = new Label();
            _lblDesktopIndex.Text = "";
            _lblDesktopIndex.ForeColor = Color.Black;
            _lblDesktopIndex.BackColor = Color.Transparent;
            _lblDesktopIndex.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            _lblDesktopIndex.Location = new Point(0, (Global.Settings.TabButtonHeight - 9) / 2);
            _lblDesktopIndex.Size = new System.Drawing.Size(9, 18);
            _lblDesktopIndex.TextAlign = ContentAlignment.BottomLeft;
            _lblDesktopIndex.UseCompatibleTextRendering = true;
            _lblDesktopIndex.Parent = _icon;
            _lblDesktopIndex.Visible = false;
            
            Panel = new CustomPanel();
            Panel.Margin = new Padding(2, 1, 2, 1);
            Panel.Controls.Add(_label);
            Panel.Controls.Add(_icon);
            Panel.Size = new System.Drawing.Size(Global.Settings.TabButtonWidth, Global.Settings.TabButtonHeight);
            Panel.TabButton = this;
            Panel.Cursor = Cursors.Hand;

            // Hover effects
            Action onHoverEnter = () =>
            {
                if (!_isActive && Panel != null)
                {
                    Panel.BackColor = ControlPaint.Light(_currentColor, 0.25f);
                }
            };
            Action onHoverLeave = () =>
            {
                if (!_isActive && Panel != null)
                {
                    Panel.BackColor = _currentColor;
                }
            };

            Panel.MouseEnter += (s, e) => onHoverEnter();
            Panel.MouseLeave += (s, e) => onHoverLeave();
            _label.MouseEnter += (s, e) => onHoverEnter();
            _label.MouseLeave += (s, e) => onHoverLeave();

            _label.MouseClick += Panel_Click;
            Panel.MouseClick += Panel_Click;
            
            _label.DoubleClick += Panel_DoubleClick;
            Panel.DoubleClick += Panel_DoubleClick;

            Panel.MouseDown += Panel_MouseDown;
            Panel.MouseMove += Panel_MouseMove;
            Panel.MouseUp += Panel_MouseUp;

            _label.MouseDown += Panel_MouseDown;
            _label.MouseMove += Panel_MouseMove;
            _label.MouseUp += Panel_MouseUp;

            Panel.AllowDrop = true;
            Panel.DragOver += Panel_DragOver;
            _label.DragOver += Panel_DragOver;
        }

        private void Panel_MouseUp(object sender, MouseEventArgs e)
        {
            if (DragState != 0)
            { 
                DragState = 0;
                BackColor = _currentColor;
                TextColor = _currentTextColor;
            }
        }

        private void Panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (DragState == 1)
            {
                if (Math.Abs(Cursor.Position.X - Global.GlobalMouseDragStartX) >= 6)
                { 
                    DragState = 2;
                    BackColor = Global.CustomColors["DragWindowTab"];
                    TextColor = Color.Black;
                }
            }
            else if (DragState == 2)
            {
                var d = Math.Abs(Parent.Bar.Location.X - Cursor.Position.X);
                var index = (int)Math.Floor(d / Panel.Width + 0.0);
                var oldIndex = (int)Math.Floor(Panel.Location.X / Panel.Width + 0.0);

                if (oldIndex != index)
                {
                    Parent.WindowButtonsPanel.Controls.SetChildIndex(Panel, index);
                }
            }
        }

        private void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (DragState == 0)
            {
                DragState = 1;
                Global.GlobalMouseDragStartX = Cursor.Position.X;
            }
        }

        private void Panel_DoubleClick(object sender, EventArgs e)
        {
            Snap();
        }

        private void Panel_DragOver(object sender, DragEventArgs e)
        {
            Active();
        }

        private void Panel_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Active();
            }
        }

        public void Active()
        {
            if (!Global.IsHwndOnCurrentDesktop(Hwnd) && Global.Settings.LockTabButtonBetweenDesktops)
            {
                return;
            }

            var title = Win32Helper.GetWindowTitle(Hwnd);
            if (string.IsNullOrEmpty(title))
            {
                Parent.RemoveTabWindowButton(this);
                return;
            }
            Text = title;
            if (Win32Helper.IsIconic(Hwnd))
            {
                Win32Helper.ShowWindow(Hwnd, CmdShow.SW_RESTORE);
            }
            else
            {
                Win32Helper.ShowWindow(Hwnd, CmdShow.SW_RESTORE);
                Win32Helper.BringWindowToFront(Hwnd);
            }
            Parent.SelectedWindowTabButton = this;
        }

        public void Snap()
        {
            if (!Global.IsHwndOnCurrentDesktop(Hwnd) && Global.Settings.LockTabButtonBetweenDesktops)
            {
                return;
            }

            var monitor = Layout?._monitor ?? Global.CurrentMonitor;
            if (monitor == null && Parent != null)
            {
                Win32Helper.SetDesktopAndMonitor(Parent.Handle);
                monitor = Global.CurrentMonitor;
            }
            Win32Helper.SnapWindowToLayout(Hwnd, monitor, Layout);
            Win32Helper.BringWindowToFront(Hwnd);
        }

        public void Topmost(bool isActive)
        {
            if (isActive)
            {
                Win32Helper.SetWindowPos(Hwnd, Win32Helper.HWND_TOPMOST, 0, 0, 0, 0, WindowPosFlags.ShowOnly);
            }
            else
            {
                Win32Helper.SetWindowPos(Hwnd, Win32Helper.HWND_NOTOPMOST, 0, 0, 0, 0, WindowPosFlags.ShowOnly);
            }
        }

        public void DesktopChanged(string desktopName)
        {
            if (desktopName != _desktopName)
            {
                _currentColor = Color.FromArgb(32, 32, 32);
                _currentTextColor = Color.Gray;
            }
            else
            {
                _currentColor = _iconColor;
                _currentTextColor = Color.White;
            }
            BackColor = _currentColor;
            TextColor = _currentTextColor;
        }

        public void ChangeDesktop(string desktopName, string currentDesktopName)
        {
            if (desktopName != currentDesktopName)
            {
                _currentColor = Color.FromArgb(32, 32, 32);
                _currentTextColor = Color.Gray;
            }
            else
            {
                _currentColor = _iconColor;
                _currentTextColor = Color.White;
            }
            BackColor = _currentColor;
            TextColor = _currentTextColor;
            _desktopName = desktopName;
        }

        public string Text
        {
            get { return _label.Text; }
            set 
            { 
                _tooltip.ToolTipTitle = value;
                _label.Text = value; 
            }
        }

        private Color BackColor
        {
            set
            {
                Panel.BackColor = value;
            }
            get
            {
                return Panel.BackColor;
            }
        }

        public Color TextColor
        {
            set
            {
                _label.ForeColor = value;
            }
            get
            {
                return _label.ForeColor;
            }
        }

        public Icon Icon
        {
            set
            {
                if (value == null) return;
                _iconColor = MyMath.CalculateAverageColor(value.ToBitmap(), 2);
                _currentColor = _iconColor;
                BackColor = _currentColor;
                _icon.Image = value.ToBitmap();
            }
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                if (_isActive)
                {
                    BackColor = Global.GetColor(Global.Settings.ActiveWindowTabButtonColor);
                    _lblDesktopIndex.ForeColor = Color.White;
                }
                else
                {
                    BackColor = _currentColor;
                    _lblDesktopIndex.ForeColor = Color.Black;
                }
                Panel.Invalidate();
            }
        }

        public void ResetBackColor()
        {
            BackColor = _currentColor;
        }

        public class CustomPanel : Panel
        {
            public WindowTabButton TabButton { get; set; }

            public CustomPanel()
            {
                this.DoubleBuffered = true;
                this.Resize += (s, e) => ApplyRoundedRegion();
            }

            private void ApplyRoundedRegion()
            {
                if (this.Width > 0 && this.Height > 0)
                {
                    try
                    {
                        IntPtr hRgn = Win32Helper.CreateRoundRectRgn(0, 0, this.Width + 1, this.Height + 1, 6, 6);
                        Win32Helper.SetWindowRgn(this.Handle, hRgn, true);
                    }
                    catch { }
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                if (TabButton != null && TabButton.IsActive)
                {
                    // Draw bottom accent indicator strip
                    using (var brush = new SolidBrush(Color.FromArgb(96, 205, 255)))
                    {
                        e.Graphics.FillRectangle(brush, 4, this.Height - 3, this.Width - 8, 3);
                    }
                }
            }
        }
    }
}
