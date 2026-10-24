using System;
using System.Drawing;
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

            BackColor = Global.GetColor(Global.Settings.WindowDragOverlayColor);
            ShowInTaskbar = false;
            MinimizeBox = false;
            MaximizeBox = false;

            Load += LayoutOverlay_Load;
        }

        private void LayoutOverlay_Load(object sender, EventArgs e)
        {
            Win32Helper.SetLayeredWindowAttributes(this.Handle, 0, 100, Win32Helper.LWA_ALPHA);
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
            if (_current == null || _current.IsDisposed)
            {
                _current = new LayoutOverlay();
                _current.Show();
            }

            if (!_current.Visible)
            {
                _current.Visible = true;
            }

            if (_lastX != x || _lastY != y || _lastW != width || _lastH != height)
            {
                _lastX = x;
                _lastY = y;
                _lastW = width;
                _lastH = height;

                // Move and resize in a single Win32 call without activating or stealing focus
                Win32Helper.SetWindowPos(_current.Handle, Win32Helper.HWND_TOP, x, y, width, height,
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
