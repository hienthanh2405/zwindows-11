# zWindows 11 (zWindowsCore) - User Guide & Feature Documentation

**zWindows 11** (`zwindowscore`) is an advanced workspace optimization and window management engine (Tiling Window Manager & FancyZones alternative) built with .NET for Windows 10 and Windows 11.

It delivers an elegant, high-performance window tiling experience styled with **Windows 11 Fluent Design**: rounded corner snap overlays, pill flyout notifications, sleek window tab bars, quick preset switchers, multi-tasking application groups (Foreground Groups), and taskbar customization tools.

---

## 📑 Table of Contents
1. [Key Features](#-key-features)
2. [Windows 11 Fluent Design System](#-windows-11-fluent-design-system)
3. [Global Hotkeys](#-global-hotkeys)
4. [Mouse & Window Interactions](#-mouse--window-interactions)
5. [Built-in Layout Presets](#-built-in-layout-presets)
6. [Settings & Quick Presets Toolbar](#-settings--quick-presets-toolbar)
7. [Window Tab Bars](#-window-tab-bars)
8. [Foreground Groups](#-foreground-groups)
9. [Taskbar Utilities](#-taskbar-utilities)
10. [Configuration & Customization (layouts.json)](#-configuration--customization-layoutsjson)
11. [Build & Run](#-build--run)

---

## 🚀 Key Features

- **Smart Snap & Tiling Zones**: Divide your screen into flexible work zones based on percentages with pixel-level margin offsets.
- **Hold Shift to Snap**: Normal window dragging behaves as usual; only when holding the <kbd>Shift</kbd> key will a semi-transparent blue bounding box appear to snap the window into the target zone.
- **Comprehensive Global Hotkeys (`Win + Alt + ...`)**: Instantly snap active windows to any zone or cycle through zones with keyboard shortcuts.
- **Window Tab Bars**: Windows in the same zone or monitor are organized with intuitive top tab buttons, allowing quick switching without Alt+Tab.
- **Virtual Desktops & Multi-Monitor Support**: Automatically detects Virtual Desktop switching and monitor orientation changes (landscape / portrait). Each desktop can maintain its own independent layout configuration.
- **Taskbar Enhancements**: Centering taskbar icons (Center Taskbar), enabling transparent taskbars (Transparent Taskbar), or completely hiding the taskbar for maximum screen real estate.

---

## 🎨 Windows 11 Fluent Design System

The UI has been crafted to blend seamlessly into modern Windows 11:

- **Rounded Corner Snap Overlays**: The snap indicator (`LayoutOverlay`) features smooth rounded corners (16px radius) and a FancyZones-style 4px margin gap from display boundaries, paired with a subtle luminous inner border.
- **Capsule Pill Flyout HUD**: Notifications (`Noti`) appear as a centered floating capsule pill near the top edge of the active display, styled with Dark Acrylic (`#1C1C1C`), a fine border, a glowing blue accent dot (`#60CDFF`), and a smooth fade-out timer.
- **Fluent Window Tab Buttons**: Tabs feature Segoe UI typography, rounded corners (6px), smooth hover brightening, and an accent indicator strip when active.
- **Immersive Dark Mode**: The **Settings** and **Layout Editor** windows automatically enable Windows 11 system-level dark mode (`DWMWA_USE_IMMERSIVE_DARK_MODE`) on titlebars, backgrounds, and controls.

---

## ⌨️ Global Hotkeys

All primary shortcuts use the <kbd>Win</kbd> + <kbd>Alt</kbd> prefix to avoid conflicting with default Windows shortcuts (`Win + ...` or `Alt + ...`):

| Shortcut | Action | Description |
| :--- | :--- | :--- |
| <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>1</kbd> | **Snap to Zone 1** | Snaps the active window into Layout Zone 1 (typically the left column). |
| <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>2</kbd> | **Snap to Zone 2** | Snaps the active window into Layout Zone 2 (typically the right column). |
| <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>3</kbd> | **Snap to Zone 3** | Snaps the active window into Layout Zone 3 (e.g., Fullscreen or 3rd column). |
| <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>4</kbd> .. <kbd>9</kbd> | **Snap to Zones 4–9** | Snaps the active window into the respective zones (for Grid 4 or custom layouts). |
| <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>→</kbd> | **Next Zone** | Cycles the active window forward to the next zone in sequence. |
| <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>←</kbd> | **Previous Zone** | Cycles the active window backward to the previous zone. |
| <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>P</kbd> | **Cycle Layout Preset** | Quickly cycles through the 5 built-in presets (`2_col` → `3_col` → `ultrawide_focus` → `grid_4` → `1_large_2_small`). |
| <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>L</kbd> | **Toggle Layout Overlay** | Toggles the visual layout overlay showing zone boundaries and numbers across all monitors. |
| <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>S</kbd> | **Toggle Hold Shift to Snap** | Toggles between *Hold Shift to Snap* (default) and *Always Snap while Dragging*. |

> [!TIP]
> Each time a shortcut is triggered, the centered Pill Flyout HUD pops up for 1–1.5 seconds confirming the executed action.

---

## 🖱️ Mouse & Window Interactions

### 1. Smart Window Drag & Snap
- **When "Hold Shift to Snap" is ON (Default)**:
  1. Click and hold a window's title bar to start dragging.
  2. Hold the <kbd>Shift</kbd> key: A rounded, glowing blue overlay (`#2192FF`) will appear over the closest layout zone under your cursor.
  3. Release the mouse button: The window automatically snaps cleanly into the designated zone.
  4. If you release the <kbd>Shift</kbd> key before releasing the mouse button, the window remains in its freely dragged position.
- **Handling Maximized Windows**:
  - Dragging a maximized window automatically restores it to normal size, allowing smooth re-positioning and snapping.

### 2. Window Tab Buttons
- **Left-Click Tab**: Activates and brings that window to the foreground.
- **Double-Click Tab**: Re-snaps the window into its designated layout zone.
- **Horizontal Drag & Drop Tab**: Reorders tabs within the same tab bar.
- **Hover & Active States**: Hovering highlights the tab background; the active tab displays a glowing bottom accent strip.

---

## 📐 Built-in Layout Presets

Presets can be selected from the system tray context menu (`Layout Presets`), via <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>P</kbd>, or directly from the **Quick Presets Bar** in the Settings window:

### 1. 2 Columns + Fullscreen (3 Layouts) — *Default & Recommended*
- **Zone 1**: Left half of the screen (50% Width, 100% Height).
- **Zone 2**: Right half of the screen (50% Width, 100% Height).
- **Zone 3**: Fullscreen (100% Width, 100% Height).
```
+--------------------+--------------------+
|                    |                    |
|       Zone 1       |       Zone 2       |
|      (50% x 100%)  |      (50% x 100%)  |
|                    |                    |
+--------------------+--------------------+
|      Zone 3: Fullscreen (100% x 100%)    |
+-----------------------------------------+
```

### 2. 3 Columns (33 / 34 / 33)
- Divides the screen into 3 equal vertical columns, ideal for 2K, 4K, or Ultrawide monitors:
```
+--------------+----------------+--------------+
|    Zone 1    |     Zone 2     |    Zone 3    |
| (33% x 100%) |  (34% x 100%)  | (33% x 100%) |
+--------------+----------------+--------------+
```

### 3. Ultrawide Focus (25 / 50 / 25)
- A dominant center focus zone (50% width) flanked by two secondary columns (25% each side):
```
+---------+--------------------------+---------+
|  Zone 1 |          Zone 2          |  Zone 3 |
|  (25%)  |        Focus (50%)       |  (25%)  |
+---------+--------------------------+---------+
```

### 4. Grid 4 Zones (2 x 2)
- Divides the monitor into four equal quadrants:
```
+--------------------+--------------------+
|    Zone 1 (50x50)  |    Zone 2 (50x50)  |
+--------------------+--------------------+
|    Zone 3 (50x50)  |    Zone 4 (50x50)  |
+--------------------+--------------------+
```

### 5. 1 Large Left + 2 Small Right
- Left column takes 50% width at full height; right side splits into two stacked tiles (50% width, 50% height each):
```
+--------------------+--------------------+
|                    |    Zone 2 (50x50)  |
|       Zone 1       +--------------------+
|      (50x100)      |    Zone 3 (50x50)  |
+--------------------+--------------------+
```

---

## ⚙️ Settings & Quick Presets Toolbar

Open the **Settings** dialog by double-clicking the system tray icon or through the context menu:

- **⚡ Quick Presets Bar**: Located right above the configuration panel, offering 1-click preset switching:
  - `[ ⊞ 2 Cols + Full ]`
  - `[ ▤ 3 Columns ]`
  - `[ ◫ Ultrawide Focus ]`
  - `[ ▦ Grid 4 ]`
  - `[ ◨ 1 Large + 2 Small ]`
- **Instant Toggles**:
  - `[✓] Hold Shift to Snap` checkbox for on-the-fly toggling.
  - `[✓] Global Hotkeys` checkbox for enabling or disabling shortcuts instantly.
- **Dark Mode JSON Editor**: Styled with Cascadia Code / Consolas for precise manual tuning.

---

## 🗂️ Foreground Groups

**Foreground Groups** allow you to group multiple windows and bring them all to the foreground together with a single click:
1. Open the **Settings** window.
2. Select a group (e.g., `Group 1`).
3. Click **Detect Window**, click on the desired window on your screen, then click **Add Window**.
4. Whenever you need this workflow cluster, select the group name from the list to raise all grouped windows simultaneously.

---

## 🛠️ Taskbar Utilities

Right-click the application icon or the floating controller circle to access utility options:
- **Center Taskbar Icons**: Automatically centers taskbar icons in real time.
- **Transparent Taskbar**: Applies transparent styling to the Windows taskbar.
- **Hide / Show Taskbar**: Completely hides the taskbar to maximize display real estate.
- **Toggle Tab Bar Visible**: Shows or hides the Window Tab Bars on top of layout zones.
- **Tab Bar Always On Top**: Pins the Window Tab Bars on top of all other windows.

---

## 📝 Configuration & Customization (layouts.json)

All settings and zone coordinates are automatically persisted in `layouts.json` located in the application working directory.

Key settings in `layouts.json`:
- `"HoldShiftKeyToSnap"`: `true` (default) requires holding Shift while dragging to snap; `false` allows snapping anytime.
- `"EnableHotkeys"`: `true` enables global `Win + Alt + ...` keyboard shortcuts.
- `"WindowDragOverlayColor"`: HEX color of the drag highlight overlay (default `#2192FF`).
- `"TabBarColor"`: Background color of the window tab bars.
- `"TabButtonWidth"`, `"TabButtonHeight"`: Width and height of individual tab buttons.
- `"HideTaskbars"`: `true` automatically hides the taskbar on application start.
- `"MonitorsLayouts"`: Contains configured zones (`X`, `Y`, `Width`, `Height` in percentages, plus pixel margin `Offset` `[Left, Top, Right, Bottom]`).

---

## 📦 Build & Run

### 1. Running from Published Binaries
The compiled binaries are available in the `publish/` directory:
- **Self-Contained (Recommended)**:
  - Path: `publish\self-contained\zwindowscore.exe`
  - Fully standalone; does not require .NET Runtime installed on the system.
- **Framework-Dependent**:
  - Path: `publish\framework-dependent\zwindowscore.exe`
  - Smaller file size; requires .NET 6.0 Desktop Runtime.

> [!IMPORTANT]
> **Administrator Privileges**: It is recommended to run `zwindowscore.exe` as Administrator so that it can manage, detect, and snap windows belonging to elevated applications (such as Task Manager, Administrator Terminals, IDEs, etc.).

### 2. Building from Source
Run the following .NET CLI commands inside the project repository:
```powershell
# Build Self-Contained x64
dotnet publish "src\zwindowscore\zwindowscore.csproj" -c Release -r win-x64 --self-contained true -o "publish\self-contained"

# Build Framework-Dependent x64
dotnet publish "src\zwindowscore\zwindowscore.csproj" -c Release -r win-x64 --self-contained false -o "publish\framework-dependent"
```
