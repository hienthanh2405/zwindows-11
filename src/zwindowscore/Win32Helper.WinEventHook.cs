using System;
using zwindowscore.Enum;
using zwindowscore.Struct;
using System.Windows.Forms;
using zwindowscore.Utils.UI;
using static zwindowscore.Monitor;
using zwindowscore.Options;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace zwindowscore
{
    public static partial class Win32Helper
    {
        public static bool PauseWinEventHook = false;
        static WinEventDelegate procDelegate = new WinEventDelegate(WinEventProc);
        static IntPtr foreground;
        static MonitorDeviceInfo currentMonitor;
        static Guid? currentDesktopId;

        static int DragWindowState = 0;
        static IntPtr DragWindowHwnd;
        static Rect DragWindowRect;
        static MonitorLayout DragWindowLayout;
        static bool _wasZoomedOnDragStart = false;
        static System.Windows.Forms.Timer _dragTimer = null;

        private static void StartDragMonitorTimer()
        {
            if (Global.Main == null) return;
            if (Global.Main.InvokeRequired)
            {
                Global.Main.BeginInvoke((MethodInvoker)StartDragMonitorTimer);
                return;
            }
            if (_dragTimer == null)
            {
                _dragTimer = new System.Windows.Forms.Timer();
                _dragTimer.Interval = 35;
                _dragTimer.Tick += (s, e) =>
                {
                    if (DragWindowState == 2 && DragWindowHwnd != IntPtr.Zero)
                    {
                        UpdateDragOverlay(DragWindowHwnd);
                    }
                    else if (DragWindowState == 0)
                    {
                        StopDragMonitorTimer();
                    }
                };
            }
            _dragTimer.Start();
        }

        private static void StopDragMonitorTimer()
        {
            if (Global.Main == null) return;
            if (Global.Main.InvokeRequired)
            {
                Global.Main.BeginInvoke((MethodInvoker)StopDragMonitorTimer);
                return;
            }
            _dragTimer?.Stop();
        }

        private static void UpdateDragOverlay(IntPtr hwnd)
        {
            if (DragWindowState != 2 || hwnd == IntPtr.Zero) return;

            bool isShift = Win32Helper.IsShiftKeyDown();
            if (Global.Settings.HoldShiftKeyToSnap && !isShift)
            {
                if (DragWindowLayout != null)
                {
                    DragWindowLayout = null;
                    if (Global.Main != null)
                    {
                        if (Global.Main.InvokeRequired)
                        {
                            Global.Main.BeginInvoke((MethodInvoker)delegate { LayoutOverlay.HideOverlay(); });
                        }
                        else
                        {
                            LayoutOverlay.HideOverlay();
                        }
                    }
                }
                return;
            }

            Point lpPoint;
            GetCursorPos(out lpPoint);
            var layout = Global.GetMonitorLayout(null, lpPoint, null, Global.CurrentDesktopName);
            if (layout != null)
            {
                if (DragWindowLayout != layout)
                {
                    DragWindowLayout = layout;
                    var mon = layout._monitor;
                    int x = mon.X + layout.Left;
                    int y = mon.Y + layout.Top;
                    int w = layout.CWidth;
                    int h = layout.CHeight;

                    if (Global.Main != null)
                    {
                        if (Global.Main.InvokeRequired)
                        {
                            Global.Main.BeginInvoke((MethodInvoker)delegate { LayoutOverlay.ShowOverlay(hwnd, x, y, w, h); });
                        }
                        else
                        {
                            LayoutOverlay.ShowOverlay(hwnd, x, y, w, h);
                        }
                    }
                }
            }
            else
            {
                if (DragWindowLayout != null)
                {
                    DragWindowLayout = null;
                    if (Global.Main != null)
                    {
                        if (Global.Main.InvokeRequired)
                        {
                            Global.Main.BeginInvoke((MethodInvoker)delegate { LayoutOverlay.HideOverlay(); });
                        }
                        else
                        {
                            LayoutOverlay.HideOverlay();
                        }
                    }
                }
            }
        }
        
        public static void StartEventHook()
        {
            IntPtr hhook = SetWinEventHook(EventType.EVENT_MIN, 
                    EventType.EVENT_MAX, 
                    IntPtr.Zero,
                    procDelegate, 0, 0, 
                    WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
        }

        private static bool IsWindowMaximized(IntPtr hwnd)
        {
            if (IsZoomed(hwnd)) return true;
            try
            {
                WindowPlacement wp = new WindowPlacement();
                wp.length = System.Runtime.InteropServices.Marshal.SizeOf(typeof(WindowPlacement));
                if (GetWindowPlacement(hwnd, ref wp))
                {
                    return wp.showCmd == CmdShow.SW_MAXIMIZE;
                }
            }
            catch { }
            return false;
        }

        private static void TriggerSnapOnMaximize(IntPtr hwnd)
        {
            if (DragWindowState == 0 && IsWindowMaximized(hwnd) && IsTopLevelWindows(hwnd) && IsAltTabWindow(hwnd))
            {
                Task.Run(async () =>
                {
                    await Task.Delay(30).ConfigureAwait(false);
                    Global.Main?.BeginInvoke((MethodInvoker)delegate
                    {
                        if (IsWindowMaximized(hwnd))
                        {
                            SnapWindow(hwnd);
                        }
                    });
                });
            }
        }

        static void WinEventProc(IntPtr hWinEventHook, EventType eventType,
            IntPtr hwnd, WinEventObjectId idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (PauseWinEventHook 
                || idObject == WinEventObjectId.OBJID_CURSOR 
                || idObject == WinEventObjectId.OBJID_CARET
                || hwnd == IntPtr.Zero
                || idObject != WinEventObjectId.OBJID_SELF 
                || idChild != 0)
            {
                return;
            }
            var tab = Global.FindTabButtonWithHwnd(hwnd);
            switch (eventType)
            {
                case EventType.EVENT_OBJECT_DESTROY:
                    if (tab != null)
                    {
                        MethodInvoker action = delegate {
                            if(tab.Parent != null)
                            { 
                                tab.Parent.RemoveTabWindowButton(tab);
                            }
                            Global.TabButtonWindows.Remove(hwnd);
                        };
                        Global.Main.BeginInvoke(action);
                    }
                    break;
                
                case EventType.EVENT_OBJECT_NAMECHANGE:
                    if (tab != null)
                    {
                        if (tab.TitleEvent)
                        {
                            tab.Text = GetWindowTitle(hwnd);
                        }
                    }
                    break;
                
                case EventType.EVENT_SYSTEM_FOREGROUND:
                    if(IsTopLevelWindows(hwnd))
                    {
                        foreground = hwnd;
                        if (Global.IsDetectWindow)
                        {
                            var title = GetWindowTitle(hwnd);
                            if(title != "ZWindows")
                            {
                                if (!string.IsNullOrEmpty(title))
                                {
                                    Global.CurrentForegroundWindow.Hwnd = hwnd;
                                    Global.CurrentForegroundWindow.Name = title;
                                    Global.CurrentForegroundWindowText.Text = title;
                                }
                                else
                                {
                                    Global.CurrentForegroundWindow.Hwnd = IntPtr.Zero;
                                    Global.CurrentForegroundWindow.Name = "";
                                    Global.CurrentForegroundWindowText.Text = "Could not detect window!";
                                }
                            }
                        }
                        else
                        {
                            if (tab != null && tab.Parent != null)
                            {
                                tab.Parent.SelectedWindowTabButton = tab;
                            }
                        }
                    }
                    break;

                case EventType.EVENT_OBJECT_STATECHANGE:
                    TriggerSnapOnMaximize(hwnd);
                    break;

                case EventType.EVENT_OBJECT_LOCATIONCHANGE:
                    if(DragWindowState == 0)
                    { 
                        TriggerSnapOnMaximize(hwnd);
                    }
                    else if(!Global.Settings.DisableDragWindowOverlay)
                    { 
                        if(DragWindowState == 0 || DragWindowHwnd != hwnd) break;

                        if(DragWindowState == 1)
                        {
                            var tmp = new Rect();
                            GetClientRect(hwnd, ref tmp);
                            if(_wasZoomedOnDragStart || tmp.ToString() == DragWindowRect.ToString())
                            {
                                DragWindowState = 2; // dragging
                                UpdateDragOverlay(hwnd);
                            }
                            else
                            {
                                DragWindowState = 3; // resizing
                                StopDragMonitorTimer();
                            }
                        }
                        else if(DragWindowState == 2)
                        {
                            UpdateDragOverlay(hwnd);
                        }
                    }
                    break;

                case EventType.EVENT_SYSTEM_MINIMIZESTART:
                    if (Global.IgnoredMinimizeEvents.Contains(hwnd))
                    {
                        Global.IgnoredMinimizeEvents.Remove(hwnd);
                        break;
                    }
                    if (Global.Settings.DisabledMinimize)
                    {
                        if (hwnd == foreground)
                        {
                            ShowWindow(hwnd, CmdShow.SW_RESTORE);
                        }
                    }
                    break;

                case EventType.EVENT_SYSTEM_MOVESIZESTART:
                    if(!Global.Settings.DisableDragWindowOverlay)
                    {
                        if(DragWindowState == 0)
                        {
                            DragWindowHwnd = hwnd;
                            DragWindowState = 1;
                            DragWindowLayout = null;
                            _wasZoomedOnDragStart = IsZoomed(hwnd);
                            DragWindowRect = new Rect();
                            GetClientRect(hwnd, ref DragWindowRect);
                            StartDragMonitorTimer();
                        }
                    }
                    break;

                case EventType.EVENT_SYSTEM_MOVESIZEEND:
                    StopDragMonitorTimer();
                    if(!Global.Settings.DisableDragWindowOverlay)
                    { 
                        if(DragWindowState == 2)
                        {
                            var targetLayout = DragWindowLayout;
                            var targetHwnd = hwnd;

                            // Hide overlay instantly
                            LayoutOverlay.ClearOverlay();

                            bool shouldSnap = false;
                            if (Global.Settings.HoldShiftKeyToSnap)
                            {
                                // In Shift-to-Snap mode: snapping is triggered if a layout zone was matched with Shift held
                                shouldSnap = (targetLayout != null);
                            }
                            else
                            {
                                // Default mode: snap if auto-snap is not disabled
                                shouldSnap = (targetLayout != null && !Global.Settings.DisableAutoSnapWindowAfterDrag);
                            }

                            if(DragWindowHwnd == hwnd && shouldSnap && targetLayout != null)
                            {
                                if (Global.Main != null)
                                {
                                    Global.Main.BeginInvoke((MethodInvoker)delegate
                                    {
                                        if (SnapWindowToLayout(targetHwnd,
                                            targetLayout._monitor, targetLayout, true))
                                        {
                                            CreateOrUpdateTabButton(targetHwnd,
                                                targetLayout._monitor, targetLayout);
                                        }
                                    });
                                }
                            }
                        }

                        DragWindowState = 0;
                        DragWindowHwnd = IntPtr.Zero;
                        DragWindowLayout = null;
                        _wasZoomedOnDragStart = false;
                    }
                    break;
            }
        }
    }
}
