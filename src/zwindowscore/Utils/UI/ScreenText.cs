using System;
using System.Drawing;
using System.Windows.Forms;
using zwindowscore.Utils.UI;
using zwindowscore.Enum;

namespace zwindowscore.Utils
{
    public class ScreenText : IDisposable
    {
        private bool _visible;
        private Color _color;
        private Rectangle _location;

        private MoveableScreenElement _form;
        private Label _label;

        public ScreenText(Form desktopAnchor)
        {
            _label = new Label();
            _label.Text = "";
            _label.ForeColor = Color.White;
            try
            {
                _label.Font = new Font("Segoe UI Variable Display", 22, FontStyle.Bold);
            }
            catch
            {
                _label.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            }
            _label.AutoSize = false;
            _label.Dock = DockStyle.Fill;
            _label.TextAlign = ContentAlignment.MiddleCenter;
            _label.BackColor = Color.FromArgb(28, 28, 28); // Modern dark card background

            _form = new MoveableScreenElement(desktopAnchor);
            _form.TopMost = true;
            _form.Left = 0;
            _form.Top = 0;
            _form.Width = 1;
            _form.Height = 1;
            _form.Opacity = 0.95;
            _form.BackColor = Color.FromArgb(28, 28, 28);
            
            _form.AddControls(_label);
            _color = Color.FromArgb(28, 28, 28);

            _form.Resize += (s, e) => ApplyRoundedRegion();
        }

        private void ApplyRoundedRegion()
        {
            if (_form.Width > 0 && _form.Height > 0)
            {
                try
                {
                    int radius = 18;
                    IntPtr hRgn = Win32Helper.CreateRoundRectRgn(0, 0, _form.Width + 1, _form.Height + 1, radius, radius);
                    Win32Helper.SetWindowRgn(_form.Handle, hRgn, true);
                }
                catch { }
            }
        }

        public EventHandler Click
        {
            set
            {
                _form.MyClick += new MoveableScreenElement.MyClickDelegate(value);
            }
        }

        public EventHandler ClickUp
        {
            set
            {
                _form.MyClickUp += new MoveableScreenElement.MyClickDelegate(value);
            }
        }

        public EventHandler RightClick
        {
            set
            {
                _form.MyRightClick += new MoveableScreenElement.MyClickDelegate(value);
            }
        }

        public Form Form
        {
            get { return _form; }
        }

        public Label Label
        {
            get { return _label; }
        }

        public string Text
        {
            get { return _label.Text; }
            set { _label.Text = value; }
        }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                _label.Visible = value;

                if (value)
                    Win32Helper.ShowWindow(_form.Handle, CmdShow.SW_SHOWNA);
                else
                    _form.Hide();
            }
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                _form.BackColor = value;
                _label.BackColor = value;
            }
        }

        public double Opacity
        {
            get { return _form.Opacity; }
            set { _form.Opacity = value; }
        }

        public Rectangle Location
        {
            get { return _location; }
            set
            {
                _location = value;
                Layout();
            }
        }

        private void Layout()
        {
            Win32Helper.SetWindowPos(_form.Handle, Win32Helper.HWND_TOPMOST, 
                _location.X, _location.Y, 
                _location.Width, _location.Height, Enum.WindowPosFlags.Unknown);
            ApplyRoundedRegion();
        }

        #region IDisposable Members

        public void Dispose()
        {
            _form.Dispose();
        }

        #endregion
    }
}
