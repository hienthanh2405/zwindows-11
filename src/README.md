# zWindows 11 (zWindowsCore) - User Guide & Feature Documentation

**zWindows 11** (`zwindowscore`) is an advanced workspace optimization and window management tool (Advanced Window Manager & Tiling Engine) designed for Windows 10 and Windows 11.

It provides a smart window arrangement experience similar to FancyZones / Tiling Window Managers, integrated window tab bars (Window Tab Bars), multi-tasking application groups (Foreground Groups), seamless multi-monitor & Virtual Desktop support, and taskbar customization utilities.

---

## 📑 Table of Contents
1. [Key Features](#-key-features)
2. [Global Hotkeys](#-global-hotkeys)
3. [Mouse & Window Interactions](#-mouse--window-interactions)
4. [Built-in Layout Presets](#-built-in-layout-presets)
5. [Window Tab Bars](#-window-tab-bars)
6. [Foreground Groups](#-foreground-groups)
7. [Taskbar Utilities](#-taskbar-utilities)
8. [Configuration & Customization (layouts.json)](#-configuration--customization-layoutsjson)
9. [Build & Run](#-build--run)

---

## 🚀 Key Features

- **Smart Snap & Tiling Zones**: Divide your screen into flexible work zones based on percentages with pixel-level margin offsets.
- **Hold Shift to Snap**: Normal window dragging behaves as usual; only when holding the <kbd>Shift</kbd> key will a semi-transparent blue bounding box appear to snap the window into the target zone.
- **Comprehensive Global Hotkeys (`Win + Alt + ...`)**: Instantly snap active windows to any zone or cycle through zones with keyboard shortcuts.
- **Window Tab Bars**: Windows in the same zone or monitor are organized with intuitive top tab buttons, allowing quick switching without Alt+Tab.
- **Virtual Desktops & Multi-Monitor Support**: Automatically detects Virtual Desktop switching and monitor orientation changes (landscape / portrait). Each desktop can maintain its own independent layout configuration.
- **Taskbar Enhancements**: Centering taskbar icons (Center Taskbar), enabling transparent taskbars (Transparent Taskbar), or completely hiding the taskbar for maximum screen real estate.

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
> Each time a shortcut is triggered, a clean On-Screen Notification banner pops up for 1–1.5 seconds confirming the executed action.

---

## 🖱️ Mouse & Window Interactions

### 1. Smart Window Drag & Snap
- **When "Hold Shift to Snap" is ON (Default)**:
  1. Click and hold a window's title bar to start dragging.
  2. Hold the <kbd>Shift</kbd> key: A semi-transparent blue overlay (`#2192FF`) will appear over the closest layout zone under your cursor.
  3. Release the mouse button: The window automatically snaps cleanly into the designated zone.
  4. If you release the <kbd>Shift</kbd> key before releasing the mouse button, the window remains in its freely dragged position.
- **Handling Maximized Windows**:
  - Dragging a maximized window automatically restores it to normal size, allowing smooth re-positioning and snapping.

### 2. Window Tab Buttons
- **Left-Click Tab**: Activates and brings that window to the foreground.
- **Double-Click Tab**: Re-snaps the window into its designated layout zone.
- **Horizontal Drag & Drop Tab**: Reorders tabs within the same tab bar.
- **Tab Colors**: Active application tabs highlight using their application icon's primary color; tabs belonging to other Virtual Desktops darken automatically.

---

## 📐 Built-in Layout Presets

Presets can be selected from the system tray context menu (`Layout Presets`) or via <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>P</kbd>:

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

## 🗂️ Foreground Groups

**Foreground Groups** allow you to group multiple windows and bring them all to the foreground together with a single click:
1. Open the **Settings** window (Double-click the red controller circle or the system tray icon).
2. Select a group (e.g., `Group 1`).
3. Click **Detect Window**, click on the desired window on your screen, then click **Add Window**.
4. Whenever you need this workflow cluster, select the group name from the list to raise all grouped windows simultaneously.

---

## 🛠️ Taskbar Utilities

Right-click the application icon or the red floating controller circle to access utility options:
- **Center Taskbar Icons**: Automatically centers taskbar icons in real time.
- **Transparent Taskbar**: Applies transparent styling to the Windows taskbar.
- **Hide / Show Taskbar**: Completely hides the taskbar to maximize display real estate.
- **Toggle Tab Bar Visible**: Shows or hides the Window Tab Bars on top of layout zones.
- **Tab Bar Always On Top**: Pins the Window Tab Bars on top of all other windows.

---

## ⚙️ Configuration & Customization (layouts.json)

All settings and zone coordinates are automatically persisted in `layouts.json` located in the application working directory.

Key settings in `layouts.json`:
- `"HoldShiftKeyToSnap"`: `true` (default) requires holding Shift while dragging to snap; `false` allows snapping anytime.
- `"EnableHotkeys"`: `true` enables global `Win + Alt + ...` keyboard shortcuts.
- `"WindowDragOverlayColor"`: HEX color of the drag highlight overlay (default `#2192FF`).
- `"TabBarColor"`: Background color of the window tab bars.
- `"TabButtonWidth"`, `"TabButtonHeight"`: Width and height of individual tab buttons.
- `"HideTaskbars"`: `true` automatically hides the taskbar on application start.
- `"MonitorsLayouts"`: Contains configured zones (`X`, `Y`, `Width`, `Height` in percentages, plus pixel margin `Offset` `[Left, Top, Right, Bottom]`).

> [!NOTE]
> You can open the **Settings** dialog, edit the JSON directly in the text editor, and click **Apply Settings** to update your setup immediately without restarting the application.

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
