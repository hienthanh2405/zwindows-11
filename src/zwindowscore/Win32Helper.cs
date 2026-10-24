using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WindowsDesktop;
using zwindowscore.Enum;
using zwindowscore.Options;
using zwindowscore.Struct;
using zwindowscore.Utils;

namespace zwindowscore
{
    public static partial class Win32Helper
    {
        public static string GetDesktopName(Guid? desktopId, int index = 0)
        {
            if (desktopId == null) return null;
            string desktopName = null;
            try
            {
                desktopName = (string)Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VirtualDesktops\\Desktops\\{" + desktopId.ToString() + "}", "Name", null);
            }
            catch { }
            if (string.IsNullOrEmpty(desktopName))
            {
                return $"Unnamed-{index}";
            }
            return desktopName;
        }

        public static string GetDesktopName(IntPtr hwnd)
        {
            var vd = WindowsDesktop.VirtualDesktop.FromHwnd(hwnd);
            if (vd != null)
            {
                return GetDesktopName(vd.Id);
            }

            return null;
        }

        public static VirtualDesktop GetDesktop(IntPtr hwnd)
        {
            return WindowsDesktop.VirtualDesktop.FromHwnd(hwnd);
        }

        public static int GetDesktopIndex(VirtualDesktop vd)
        {
            var desktops = WindowsDesktop.VirtualDesktop.GetDesktops();
            return Array.IndexOf(desktops, vd);
        }

        public static void SetDesktop(IntPtr hwnd)
        {
            var vd = WindowsDesktop.VirtualDesktop.FromHwnd(hwnd);
            if (vd != null)
            {
                if (!currentDesktopId.HasValue || vd.Id != currentDesktopId.Value)
                {
                    currentDesktopId = vd.Id;
                    Global.CurrentDesktopName = GetDesktopName(vd.Id);
                }
            }
        }

        public static void SetDesktopAndMonitor(IntPtr hwnd)
        {
            SetDesktop(hwnd);

            var monitorInfo = Monitor.GetMonitorFromWindow(hwnd, Monitor.MONITOR_DEFAULTTONEAREST);
            if (monitorInfo != null)
            {
                if (currentMonitor == null || currentMonitor.DeviceName != monitorInfo.DeviceName)
                {
                    currentMonitor = monitorInfo;
                    Global.CurrentMonitor = Global.UpdateMonitorInfo(monitorInfo);
                }
            }
        }

        public static void BringWindowToFront(IntPtr hwnd)
        {
            SetForegroundWindow(hwnd);
        }

        public static void BringWindowAndItsGroupToFront(IntPtr hwnd)
        {
            var gs = Global.ForegroundGroups
                .Where(p => p.Windows.Any(w => w.Hwnd == hwnd))
                .ToList();
            if (gs != null && gs.Count > 0)
            {
                var index = 0;
                var g = gs[index];
                foreach (var w in g.Windows)
                {
                    if (IsIconic(w.Hwnd) || IsZoomed(w.Hwnd))
                    {
                        ShowWindow(w.Hwnd, CmdShow.SW_RESTORE);
                    }
                    SetWindowPos(w.Hwnd, HWND_TOPMOST, 0, 0, 0, 0,
                        WindowPosFlags.IgnoreResizeAndMove);
                    SetWindowPos(w.Hwnd, HWND_NOTOPMOST, 0, 0, 0, 0,
                        WindowPosFlags.IgnoreResizeAndMove);
                }
            }
        }

        /// <summary>
        /// Create TabBar (if not exits)
        /// Create TabButton (if not exits). Add/Move TabButton into TabBar
        /// TabBar Id = vdn + layout x + layout y;
        /// </summary>
        public static string CreateOrUpdateTabButton(IntPtr hwnd, MonitorDevice monitor,
            MonitorLayout layout)
        {
            if (!layout.TabsBar.Enable) return null;
            var title = Win32Helper.GetWindowTitle(hwnd);
            if (string.IsNullOrEmpty(title)) return null;

            var tbkey = (monitor.X + layout.Left).ToString()
                + ":" + (monitor.Y + layout.Top).ToString()
                + ":" + layout.Width.ToString();

            if (!Global.TabButtonsBars.ContainsKey(tbkey))
            {
                var l = monitor.X + layout.Left + layout.TabsBar.LeftOffset;
                var y = monitor.Y + layout.Top;
                Global.TabButtonsBars[tbkey] = new ScreenTabButtonsBar(null, layout.TabsBar)
                {
                    Key = tbkey,
                    MaxWidth = layout.TabsBar?.MaxWidth > 0 ? layout.TabsBar.MaxWidth : layout.CWidth,
                    X = l,
                    Y = y,
                    Location = new System.Drawing.Rectangle
                    {
                        X = l,
                        Y = y
                    },
                    Visible = true
                };
                if (Global.Settings.NoTabBarDesktopNames != null
                    && Global.Settings.NoTabBarDesktopNames.Contains(Global.CurrentDesktopName))
                {
                    Global.TabButtonsBars[tbkey].Bar.Hide();
                }
                Global.TabButtonsBars[tbkey].Opacity = Global.Settings.TabButtonsOpacity;
            }
            var bar = Global.TabButtonsBars[tbkey];

            WindowTabButton tab = Global.FindTabButtonWithHwnd(hwnd);
            // windows already has a tab button
            if (tab != null)
            {
                // tab button already on the bar of layout
                if (tab.Parent == bar)
                {
                    return null;
                }
                else
                {
                    // move tab button to another bar of another layout
                    if (tab.Parent != null)
                    {
                        tab.Parent.RemoveTabWindowButton(hwnd);
                    }
                    bar.AddTabWindowButton(tab);
                }
            }
            else
            {
                // new tab button
                tab = bar.AddTabWindowButton(hwnd, title);
                if (tab == null)
                {
                    if (Global.TabButtonsBars[tbkey].WindowButtons.Count == 0)
                    {
                        Global.TabButtonsBars[tbkey].Dispose();
                        Global.TabButtonsBars.Remove(tbkey);
                    }
                    return null;
                }
                Global.TabButtonWindows[hwnd] = tab;
            }
            tab.Layout = layout;
            return tbkey;
        }

        public static bool SnapWindowToLayout(IntPtr hwnd, MonitorDevice monitor, MonitorLayout layout, 
            bool isAuto = false)
        {
            if (!IsAltTabWindow(hwnd))
            {
                return false;
            }

            PauseWinEventHook = true;
            try
            {
                if (isAuto && Global.Settings.NoFullSnapWhenAutoSnapProcesses != null
                    && Global.Settings.NoFullSnapWhenAutoSnapProcesses.Count > 0)
                {
                    var processName = Win32Helper.GetProcessFileName(hwnd);
                    if (string.IsNullOrEmpty(processName))
                    {
                        return false;
                    }
                    foreach (var name in Global.Settings.NoFullSnapWhenAutoSnapProcesses)
                    {
                        if (processName.Contains(name, StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                    }
                }
                
                if (monitor == null)
                {
                    SetDesktopAndMonitor(hwnd);
                    monitor = Global.CurrentMonitor;
                }

                // Only restore if actually minimized or maximized, avoiding redundant window animation/repaint
                if (IsIconic(hwnd) || IsZoomed(hwnd))
                {
                    ShowWindow(hwnd, CmdShow.SW_RESTORE);
                }

                var border_thickness = GetBorderSize(hwnd);
                if (border_thickness < 0 || border_thickness > 30)
                {
                    border_thickness = 7;
                }

                var mdx = 0;
                var mdy = 0;

                int targetX = monitor.X + mdx + layout.Left - border_thickness + layout.LeftOffset;
                int targetY = monitor.Y + mdy + layout.Top + layout.TopOffset;
                int targetWidth = -mdx * 1 + layout.CWidth + border_thickness * 2 - (layout.RightOffset + layout.LeftOffset);
                int targetHeight = -mdy * 1 + layout.CHeight + border_thickness - (layout.BottomOffset + layout.TopOffset);

                if (targetWidth < 100) targetWidth = 100;
                if (targetHeight < 100) targetHeight = 100;

                SetWindowPos(hwnd, IntPtr.Zero,
                    targetX,
                    targetY,
                    targetWidth,
                    targetHeight,
                    WindowPosFlags.IgnoreZOrder | WindowPosFlags.FrameChanged | WindowPosFlags.ShowWindow);

                return true;
            }
            finally
            {
                PauseWinEventHook = false;
            }
        }

        public static int GetBorderSize(IntPtr hwnd)
        {
            Rect rcClient = new Rect();
            Rect rcWind = new Rect();
            GetClientRect(hwnd, ref rcClient);
            GetWindowRect(hwnd, ref rcWind);
            return ((rcWind.Right - rcWind.Left) - rcClient.Right) / 2;
        }

        public static String GetWindowTitle(IntPtr hwnd)
        {
            StringBuilder title = new StringBuilder(1024);
            GetWindowText(hwnd, title, title.Capacity);
            return title.ToString();
        }

        public static void RefreshTabBars()
        {
            Utils.Timer.StartTimer();
            foreach (var b in Global.TabButtonsBars.Values)
            {
                b.Dispose();
            }
            Global.TabButtonsBars.Clear();
            Global.TabButtonWindows.Clear();

            var windows = Global.GetDesktopWindows();
            foreach (var window in windows)
            {
                Rect rcClient = new Rect();
                GetClientRect(window.Handle, ref rcClient);
                if (rcClient.Left == 0 && rcClient.Right == 0
                    && rcClient.Top == 0 && rcClient.Bottom == 0)
                {
                    continue;
                }
                Rect rcWind = new Rect();
                GetWindowRect(window.Handle, ref rcWind);

                if (rcWind.Left <= -10000 || rcWind.Top <= -10000)
                {
                    continue;
                }

                var x = rcWind.Left;
                var y = rcWind.Top;
                var w = rcWind.Right - rcWind.Left;
                var h = rcWind.Bottom - rcWind.Top;
                var delta = Global.Settings.DeltaForAutoDetectWindowLayout;

                var availableMonitorIds = Global.MonitorDevices.Select(p => p.Id).ToList();
                var availableMonitors = Global.Settings.MonitorsLayouts.Values
                    .Where(p => availableMonitorIds.Contains(p.Id))
                    .ToList();

                foreach (var monitor in availableMonitors)
                {
                    var mx = x - monitor.X;
                    var my = y - monitor.Y;

                    var layout = monitor.Layouts.Where(p =>
                        p.TabsBar.Enable
                        && (mx >= p.Left - delta && mx <= p.Left + delta)
                        && (my >= p.Top - delta && my <= p.Top + delta)
                        && (w >= p.CWidth - delta && w <= p.CWidth + delta)
                        && (h >= p.CHeight - delta && h <= p.CHeight + delta)
                    ).FirstOrDefault();
                    if (layout != null)
                    {
                        SnapWindowToLayout(window.Handle, monitor, layout);
                        CreateOrUpdateTabButton(window.Handle, monitor, layout);
                    }
                }
            }

            Utils.Timer.StopTimer("GetProccesses");
        }

        public static void SaveDesktop()
        {
            Utils.Timer.StartTimer();
            var windows = Global.GetDesktopWindows();
            var vdn = Global.CurrentDesktopName;
            var ds = new DesktopState { DesktopName = vdn, WindowStates = new List<WindowState>() };

            foreach (var window in windows)
            {
                Rect rcClient = new Rect();
                GetClientRect(window.Handle, ref rcClient);
                if (rcClient.Left == 0 && rcClient.Right == 0
                    && rcClient.Top == 0 && rcClient.Bottom == 0)
                {
                    continue;
                }
                Rect rcWind = new Rect();
                GetWindowRect(window.Handle, ref rcWind);

                if (rcWind.Left <= -10000 || rcWind.Top <= -10000)
                {
                    continue;
                }

                var borderSize = ((rcWind.Right - rcWind.Left) - rcClient.Right) / 2;
                var x = rcWind.Left;
                var y = rcWind.Top;
                var w = rcClient.Right + borderSize * 2;
                var h = rcClient.Bottom + borderSize;

                var ws = new WindowState
                {
                    Hwnd = window.Handle,
                    X = x,
                    Y = y,
                    Width = w,
                    Height = h
                };
                ds.WindowStates.Add(ws);
            }
            Global.DesktopStates[vdn] = ds;
            File.WriteAllText(Global.GetDesktopsStatesFilePath(),
                JsonConvert.SerializeObject(Global.DesktopStates, Formatting.Indented));
            Utils.Timer.StopTimer("SaveWindowsStatesOfCurrentDesktop");
        }

        public static void LoadDesktop(string vdn = null)
        {
            if (vdn == null)
            {
                vdn = Global.CurrentDesktopName;
            }
            if (Global.DesktopStates.Keys.Count == 0)
            {
                Global.DesktopStates = Global.LoadDesktopsStatesFromFile();
            }
            if (Global.DesktopStates.ContainsKey(vdn))
            {
                var ds = Global.DesktopStates[vdn];
                foreach (var ws in ds.WindowStates)
                {
                    if (IsIconic(ws.Hwnd))
                    {
                        ShowWindow(ws.Hwnd, CmdShow.SW_RESTORE);
                    }
                    SetWindowPos(ws.Hwnd, IntPtr.Zero,
                        ws.X,
                        ws.Y,
                        ws.Width,
                        ws.Height,
                        WindowPosFlags.ShowWindow);
                }
            }
            RefreshTabBars();
        }

        public static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                Win32Helper.EnumWindowProc childProc = new Win32Helper.EnumWindowProc(EnumWindow);
                Win32Helper.EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }

        public static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if (list == null)
            {
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            }
            list.Add(handle);
            return true;
        }

        public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            try
            {
                if (IntPtr.Size == 4)
                    return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
                else
                    return GetClassLongPtr64(hWnd, nIndex);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        public static Process GetModernAppProcessPath(IntPtr hwnd)
        {
            uint pid = 0;
            Win32Helper.GetWindowThreadProcessId(hwnd, out pid);
            var children = Win32Helper.GetChildWindows(hwnd);
            foreach (var childHwnd in children)
            {
                uint childPid = 0;
                Win32Helper.GetWindowThreadProcessId(childHwnd, out childPid);
                if (childPid != pid)
                {
                    return Process.GetProcessById((int)childPid);
                }
            }
            return null;
        }

        public static string GetProcessFileName(IntPtr hwnd)
        {
            var p = GetProcess(hwnd);
            if (p != null)
            {
                return p.ProcessName + "|" + p.MainModule.FileName;
            }
            return null;
        }

        public static Process GetProcess(IntPtr hwnd)
        {
            try
            {
                uint pid;
                GetWindowThreadProcessId(hwnd, out pid);
                Process p = Process.GetProcessById((int)pid);
                if (p.MainModule.FileName.IndexOf("ApplicationFrameHost.exe") != -1)
                {
                    p = GetModernAppProcessPath(hwnd);
                }
                return p;
            }
            catch
            {

            }
            return null;
        }

        public static bool IsMatch(IntPtr hwnd, WindowFilterRule rule)
        {
            var p = GetProcess(hwnd);
            if (rule.ExactProcessName != null && rule.ExactProcessName == p.ProcessName)
            {
                return true;
            }
            if (rule.ExactExeFileName != null)
            {
                var exefilename = p.MainModule.FileName.Substring(p.MainModule.FileName.LastIndexOf('/'));
                if (rule.ExactExeFileName == exefilename)
                {
                    return true;
                }
            }
            if (rule.ExactTitle != null && p.MainWindowTitle == rule.ExactTitle)
            {
                return true;
            }
            if (rule.RegexTitle != null)
            {
                if (Regex.IsMatch(p.MainWindowTitle, rule.RegexTitle))
                {
                    return true;
                }
            }
            return false;
        }

        public static void EnableCloseButton(IntPtr hwnd, bool enabled)
        {
            IntPtr hMenu;
            hMenu = GetSystemMenu(hwnd, false);
            if (hMenu != IntPtr.Zero)
            {
                int n = GetMenuItemCount(hMenu);
                if (n > 0)
                {
                    if (enabled)
                    {
                        EnableClose(hwnd);
                    }
                    else
                    {
                        DisableClose(hwnd);
                    }
                    SendMessage(hwnd, WM_NCACTIVATE, (IntPtr)1, (IntPtr)0);
                    DrawMenuBar(hwnd);
                    Application.DoEvents();
                }
            }
        }

        private static void DisableClose(IntPtr hwnd)
        {
            IntPtr hMenu;
            hMenu = GetSystemMenu(hwnd, false);
            if (hMenu != IntPtr.Zero)
            {
                MenuItemInfo mif = new MenuItemInfo();
                mif.cbSize = (uint)Marshal.SizeOf(typeof(MenuItemInfo));
                mif.fMask = MIIM_ID | MIIM_STATE;
                mif.fType = 0;
                mif.dwTypeData = null;
                bool a = GetMenuItemInfo(hMenu, (int)SC_CLOSE, false, ref mif);
                mif.fState = MFS_GRAYED;
                SetMenuItemInfo(hMenu, (int)SC_CLOSE, false, ref mif);
                SendMessage(hwnd, WM_NCACTIVATE, (IntPtr)1, (IntPtr)0);

                mif.wID = -10;
                mif.fState = MFS_GRAYED;
                SetMenuItemInfo(hMenu, (int)SC_CLOSE, false, ref mif);
            }
        }

        private static void EnableClose(IntPtr hwnd)
        {
            IntPtr hMenu;
            hMenu = GetSystemMenu(hwnd, false);
            if (hMenu != IntPtr.Zero)
            {
                MenuItemInfo mif = new MenuItemInfo();
                mif.cbSize = (uint)Marshal.SizeOf(typeof(MenuItemInfo));
                mif.fMask = MIIM_ID | MIIM_STATE;
                mif.fType = 0;
                mif.dwTypeData = null;
                bool a = GetMenuItemInfo(hMenu, -10, false, ref mif);
                mif.wID = (int)SC_CLOSE;
                SetMenuItemInfo(hMenu, -10, false, ref mif);
                SendMessage(hwnd, WM_NCACTIVATE, (IntPtr)1, (IntPtr)0);

                mif.fState = MFS_ENABLED;
                SetMenuItemInfo(hMenu, (int)SC_CLOSE, false, ref mif);
                SendMessage(hwnd, WM_NCACTIVATE, (IntPtr)1, (IntPtr)0);
            }
        }

        public static IntPtr GetRealParent(IntPtr hWnd)
        {
            IntPtr hParent;

            hParent = GetAncestor(hWnd, GetAncestorFlags.GetParent);
            if (hParent.ToInt64() == 0 || hParent == GetDesktopWindow())
            {
                hParent = GetParent(hWnd);
                if (hParent.ToInt64() == 0 || hParent == GetDesktopWindow())
                {
                    hParent = hWnd;
                }

            }

            return hParent;
        }

        public static void CloseWindow(IntPtr hwnd)
        {
            SendMessage(hwnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        public static bool IsTopLevelWindows(IntPtr hWnd)
        {
            IntPtr hParent;

            hParent = GetAncestor(hWnd, GetAncestorFlags.GetParent);
            if (hParent == GetDesktopWindow())
            {
                return true;
            }
            else if (hParent.ToInt64() == 0)
            {
                hParent = GetParent(hWnd);
                if (hParent == GetDesktopWindow())
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsWindowTopMost(IntPtr hWnd)
        {
            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            return (exStyle & WS_EX_TOPMOST) == WS_EX_TOPMOST;
        }

        public static bool IsAltTabWindow(IntPtr hWnd, bool ignored_cloaked_shell_windows = false)
        {
            if (GetAncestor(hWnd, GetAncestorFlags.GetRootOwner) != hWnd)
            {
                return false;
            }

            if(!ignored_cloaked_shell_windows)
            { 
                DwmGetWindowAttribute(hWnd, DwmWindowAttribute.Cloaked, out uint cloaked, sizeof(uint));
                if (cloaked == DWM_CLOAKED_SHELL)
                {
                    return false;
                }
            }

            int style = GetWindowLong(hWnd, GWL_EXSTYLE);
            if ((style & WS_EX_TOOLWINDOW) != 0)
            {
                return false;
            }

            return true;
        }

        private static IntPtr _lastSnappedHwnd = IntPtr.Zero;
        private static long _lastSnappedTime = 0;

        public static void SnapWindow(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero || !IsTopLevelWindows(hwnd) || !IsAltTabWindow(hwnd))
            {
                return;
            }

            long now = Environment.TickCount64;
            if (_lastSnappedHwnd == hwnd && (now - _lastSnappedTime) < 300)
            {
                return;
            }
            _lastSnappedHwnd = hwnd;
            _lastSnappedTime = now;

            PauseWinEventHook = true;
            try
            {
                SetDesktopAndMonitor(hwnd);
                var monitor = Global.CurrentMonitor;
                if (monitor == null && Global.MonitorDevices.Count > 0)
                {
                    var first = Global.MonitorDevices.FirstOrDefault();
                    if (first != null && Global.Settings.MonitorsLayouts.ContainsKey(first.Id))
                    {
                        monitor = Global.Settings.MonitorsLayouts[first.Id];
                    }
                }

                if (monitor == null || monitor.Layouts == null || monitor.Layouts.Count == 0)
                {
                    return;
                }

                Point lpPoint;
                GetCursorPos(out lpPoint);

                // Find layout by mouse position, trying current monitor first, then any monitor
                var layout = Global.GetMonitorLayout(null, lpPoint, monitor.Id, Global.CurrentDesktopName)
                             ?? Global.GetMonitorLayout(null, lpPoint, null, Global.CurrentDesktopName);

                // If cursor is slightly outside (e.g. caption bar at top of screen), find the nearest layout on this monitor
                if (layout == null)
                {
                    layout = monitor.Layouts
                        .Where(l => string.IsNullOrEmpty(Global.CurrentDesktopName) || l.DesktopName == Global.CurrentDesktopName || l.DesktopName == "default")
                        .OrderBy(l => MyMath.GetDistance(lpPoint.X, lpPoint.Y, monitor.X + l.Left + l.CWidth / 2, monitor.Y + l.Top + l.CHeight / 2))
                        .FirstOrDefault() ?? monitor.Layouts[0];
                }

                if (layout != null)
                {
                    SnapWindowToLayout(hwnd, monitor, layout);
                    if (Global.Main != null)
                    {
                        MethodInvoker action = delegate
                        {
                            CreateOrUpdateTabButton(hwnd, monitor, layout);
                        };
                        Global.Main.BeginInvoke(action);
                    }
                }
            }
            finally
            {
                PauseWinEventHook = false;
            }
        }
    }
}
