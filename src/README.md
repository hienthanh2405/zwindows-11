# zWindows 11 (zWindowsCore) - Hướng Dẫn Sử Dụng & Tài Liệu Tính Năng

**zWindows 11** (`zwindowscore`) là bộ công cụ tối ưu hóa không gian làm việc và quản lý cửa sổ nâng cao (Advanced Window Manager & Tiling Engine) dành cho Windows 10 và Windows 11. 

Chương trình mang đến trải nghiệm sắp xếp cửa sổ thông minh tương tự FancyZones / Tiling Window Manager, tích hợp thanh Tab quản lý cửa sổ (Window Tab Bars), nhóm ứng dụng đa nhiệm (Foreground Groups), hỗ trợ đa màn hình & Virtual Desktop mượt mà cùng các tiện ích tùy biến Taskbar.

---

## 📑 Mục lục
1. [Tính Năng Nổi Bật](#-tính-năng-nổi-bật)
2. [Bảng Phím Tắt Toàn Cục (Global Hotkeys)](#-bảng-phím-tắt-toàn-cục-global-hotkeys)
3. [Hướng Dẫn Thao Tác Chuột & Cửa Sổ](#-hướng-dẫn-thao-tác-chuột--cửa-sổ)
4. [Các Bộ Bố Cục Có Sẵn (Layout Presets)](#-các-bộ-bố-cục-có-sẵn-layout-presets)
5. [Thanh Quản Lý Tab Cửa Sổ (Window Tab Bars)](#-thanh-quản-lý-tab-cửa-sổ-window-tab-bars)
6. [Nhóm Ứng Dụng Đa Nhiệm (Foreground Groups)](#-nhóm-ứng-dụng-đa-nhiệm-foreground-groups)
7. [Tiện Ích Taskbar](#-tiện-ích-taskbar)
8. [Cấu Hình & Tùy Biến (layouts.json)](#-cấu-hình--tùy-biến-layoutsjson)
9. [Cách Chạy & Build Dự Án](#-cách-chạy--build-dự-án)

---

## 🚀 Tính Năng Nổi Bật

- **Smart Snap & Tiling Zones**: Chia màn hình thành nhiều vùng làm việc linh hoạt theo tỷ lệ phần trăm và căn lề từng pixel.
- **Hold Shift to Snap**: Kéo cửa sổ bình thường sẽ không bị vướng; chỉ khi giữ phím `Shift`, khung viền xanh bán trong suốt mới xuất hiện để hít cửa sổ vào vùng mong muốn.
- **Phím tắt toàn diện (`Win + Alt + ...`)**: Đưa cửa sổ đang làm việc vào bất kỳ vùng nào chỉ với một tổ hợp phím tức thì.
- **Window Tab Bars**: Các cửa sổ cùng một vùng hoặc trên cùng một màn hình được gắn các tab trực quan phía trên, giúp chuyển đổi nhanh mà không cần Alt+Tab.
- **Hỗ trợ Virtual Desktops & Multi-Monitor**: Tự động nhận diện khi chuyển Desktop ảo hoặc xoay màn hình (ngang / dọc). Mỗi Desktop có thể cấu hình layout riêng biệt.
- **Taskbar Enhancements**: Hỗ trợ căn giữa icon Taskbar (Center Taskbar), làm mờ/trong suốt thanh Taskbar (Transparent Taskbar), hoặc ẩn hoàn toàn Taskbar để tối đa diện tích màn hình.

---

## ⌨️ Bảng Phím Tắt Toàn Cục (Global Hotkeys)

Tất cả các phím tắt chính sử dụng tiền tố `Win + Alt` để tránh xung đột với các phím tắt mặc định của Windows (`Win + ...` hoặc `Alt + ...`):

| Phím tắt | Tên chức năng | Mô tả chi tiết |
| :--- | :--- | :--- |
| <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>1</kbd> | **Snap vào Vùng 1** | Đưa cửa sổ đang active vào Layout Zone số 1 (thường là cột bên trái). |
| <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>2</kbd> | **Snap vào Vùng 2** | Đưa cửa sổ đang active vào Layout Zone số 2 (thường là cột bên phải). |
| <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>3</kbd> | **Snap vào Vùng 3** | Đưa cửa sổ đang active vào Layout Zone số 3 (ví dụ: Fullscreen hoặc cột 3). |
| <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>4</kbd> .. <kbd>9</kbd> | **Snap vào Vùng 4–9** | Đưa cửa sổ active vào các vùng tương ứng (với bố cục Grid 4 hoặc bố cục nhiều ô). |
| <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>→</kbd> | **Vùng tiếp theo (Next Zone)** | Di chuyển xoay vòng cửa sổ hiện tại sang vùng kế tiếp theo chiều kim đồng hồ / thứ tự. |
| <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>←</kbd> | **Vùng trước đó (Prev Zone)** | Di chuyển xoay vòng cửa sổ hiện tại về vùng trước đó. |
| <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>P</kbd> | **Đổi Bố Cục (Cycle Preset)** | Chuyển nhanh giữa 5 bộ bố cục mặc định (`2_col` → `3_col` → `ultrawide_focus` → `grid_4` → `1_large_2_small`). |
| <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>L</kbd> | **Hiện / Ẩn Số Vùng (Toggle Layouts)** | Bật / tắt hiển thị khung viền màu kèm số thứ tự từng vùng trên tất cả màn hình. |
| <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>S</kbd> | **Bật / Tắt Giữ Shift để Snap** | Chuyển đổi giữa chế độ *Phải giữ Shift mới Snap* (mặc định) và *Snap tự do bất cứ khi nào kéo*. |

> [!TIP]
> Mỗi khi nhấn phím tắt, màn hình sẽ hiển thị một thông báo nổi (On-Screen Notification) nhỏ gọn trong 1-1.5 giây để xác nhận thao tác vừa thực hiện.

---

## 🖱️ Hướng Dẫn Thao Tác Chuột & Cửa Sổ

### 1. Kéo & Hít Cửa Sổ (Smart Window Drag & Snap)
- **Khi bật "Hold Shift to Snap" (Mặc định)**:
  1. Nhấp giữ thanh tiêu đề (Title Bar) của cửa sổ và bắt đầu kéo.
  2. Giữ phím <kbd>Shift</kbd> trên bàn phím: Khung định vị màu xanh lam (`#2192FF`) bán trong suốt sẽ xuất hiện tại vùng màn hình gần con trỏ chuột nhất.
  3. Thả chuột: Cửa sổ sẽ tự động hít khớp chính xác vào vùng đã chọn.
  4. Nếu nhả phím <kbd>Shift</kbd> trước khi thả chuột, cửa sổ sẽ giữ nguyên vị trí kéo tự do như thông thường.
- **Nếu cửa sổ đang Maximized (Toàn màn hình)**:
  - Khi bắt đầu kéo, cửa sổ sẽ tự động được thu về kích thước bình thường để bạn dễ dàng di chuyển và snap vào vị trí mới.

### 2. Thao Tác Với Thanh Tab (Window Tab Buttons)
- **Click chuột trái vào Tab**: Kích hoạt và đưa cửa sổ đó lên trên cùng (Foreground).
- **Double-Click vào Tab**: Hít lại (Re-snap) cửa sổ vào đúng vùng Layout được chỉ định.
- **Kéo thả Tab theo phương ngang**: Đổi vị trí thứ tự các tab trong cùng một thanh tab bar.
- **Màu sắc Tab**: Tab của ứng dụng đang kích hoạt sẽ sáng màu theo màu icon của chính ứng dụng đó; tab của cửa sổ thuộc Desktop ảo khác sẽ tự động đổi màu sẫm.

---

## 📐 Các Bộ Bố Cục Có Sẵn (Layout Presets)

Bạn có thể đổi bố cục thông qua Menu chuột phải khay hệ thống (`Layout Presets`) hoặc bấm phím tắt <kbd>Win</kbd> + <kbd>Alt</kbd> + <kbd>P</kbd>:

### 1. 2 Columns + Fullscreen (3 Layouts) — *Mặc định khuyên dùng*
- **Vùng 1**: Nửa trái màn hình (50% Width, 100% Height).
- **Vùng 2**: Nửa phải màn hình (50% Width, 100% Height).
- **Vùng 3**: Toàn màn hình (100% Width, 100% Height).
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
- Chia màn hình thành 3 cột đều nhau, lý tưởng khi làm việc trên màn hình độ phân giải 2K, 4K hoặc Ultrawide:
```
+--------------+----------------+--------------+
|    Zone 1    |     Zone 2     |    Zone 3    |
| (33% x 100%) |  (34% x 100%)  | (33% x 100%) |
+--------------+----------------+--------------+
```

### 3. Ultrawide Focus (25 / 50 / 25)
- Một cửa sổ chính lớn ở giữa (50% chiều ngang) và hai cột phụ trợ hai bên (25% mỗi bên):
```
+---------+--------------------------+---------+
|  Zone 1 |          Zone 2          |  Zone 3 |
|  (25%)  |        Focus (50%)       |  (25%)  |
+---------+--------------------------+---------+
```

### 4. Grid 4 Zones (2 x 2)
- Chia màn hình thành 4 góc phần tư đều nhau:
```
+--------------------+--------------------+
|    Zone 1 (50x50)  |    Zone 2 (50x50)  |
+--------------------+--------------------+
|    Zone 3 (50x50)  |    Zone 4 (50x50)  |
+--------------------+--------------------+
```

### 5. 1 Large Left + 2 Small Right
- Cột bên trái lớn (50% full chiều cao), bên phải chia thành 2 ô xếp chồng (mỗi ô 50% width, 50% height):
```
+--------------------+--------------------+
|                    |    Zone 2 (50x50)  |
|       Zone 1       +--------------------+
|      (50x100)      |    Zone 3 (50x50)  |
+--------------------+--------------------+
```

---

## 🗂️ Nhóm Ứng Dụng Đa Nhiệm (Foreground Groups)

Tính năng **Foreground Groups** cho phép gom nhiều cửa sổ thành một bộ và kích hoạt chúng cùng lúc:
1. Mở cửa sổ **Settings** (Nhấp đúp vào icon tròn màu đỏ hoặc icon khay hệ thống).
2. Chọn một nhóm (ví dụ: `Group 1`).
3. Bấm **Detect Window**, sau đó nhấp chuột vào cửa sổ mong muốn trên màn hình rồi bấm **Add Window**.
4. Khi cần làm việc với bộ cửa sổ này, chỉ cần chọn nhóm tương ứng trong danh sách, toàn bộ các cửa sổ trong nhóm sẽ được đưa lên màn hình đồng thời.

---

## 🛠️ Tiện Ích Taskbar

Nhấp chuột phải vào icon ứng dụng (hoặc nút tròn Controller) để truy cập menu tiện ích:
- **Center Taskbar Icons**: Tự động căn giữa các biểu tượng ứng dụng trên thanh Taskbar theo thời gian thực.
- **Transparent Taskbar**: Áp dụng hiệu ứng trong suốt cho thanh Taskbar.
- **Hide / Show Taskbar**: Ẩn hoàn toàn Taskbar để có thêm không gian hiển thị cho các màn hình nhỏ hoặc khi làm việc tập trung.
- **Toggle Tab Bar Visible**: Ẩn hoặc hiện các thanh Window Tab Bar trên đỉnh các vùng layout.
- **Tab Bar Always On Top**: Giữ thanh Tab luôn nổi lên trên cùng.

---

## ⚙️ Cấu Hình & Tùy Biến (layouts.json)

Tất cả cài đặt và tọa độ layout được lưu tự động trong file `layouts.json` nằm cùng thư mục chạy ứng dụng.

Một số thuộc tính quan trọng trong `layouts.json`:
- `"HoldShiftKeyToSnap"`: `true` (mặc định) nếu muốn chỉ snap khi giữ Shift; `false` nếu muốn snap mọi lúc.
- `"EnableHotkeys"`: `true` bật các phím tắt toàn cục `Win + Alt + ...`.
- `"WindowDragOverlayColor"`: Mã màu HEX của khung viền khi kéo cửa sổ (mặc định `#2192FF`).
- `"TabBarColor"`: Màu nền của thanh tab bar.
- `"TabButtonWidth"`, `"TabButtonHeight"`: Chiều rộng và chiều cao của từng tab nút bấm.
- `"HideTaskbars"`: `true` nếu muốn ẩn taskbar khi khởi động.
- `"MonitorsLayouts"`: Chứa danh sách các vùng layout (tọa độ `X`, `Y`, `Width`, `Height` tính theo % và `Offset` lề theo pixel `[Left, Top, Right, Bottom]`).

> [!NOTE]
> Bạn có thể mở trực tiếp cửa sổ **Settings**, sửa JSON trong khung nhập liệu và bấm **Apply Settings** để cập nhật ngay lập tức mà không cần khởi động lại ứng dụng.

---

## 📦 Cách Chạy & Build Dự Án

### 1. Chạy từ bản phát hành (Publish Folder)
Ứng dụng đã được biên dịch sẵn tại thư mục `publish/`:
- **Bản Self-Contained (Khuyên dùng)**: 
  - Đường dẫn: `publish\self-contained\zwindowscore.exe`
  - Chạy độc lập, không yêu cầu cài đặt thêm .NET Runtime trên máy.
- **Bản Framework-Dependent**: 
  - Đường dẫn: `publish\framework-dependent\zwindowscore.exe`
  - Nhẹ hơn, yêu cầu máy đã cài sẵn .NET 6.0 Desktop Runtime.

> [!IMPORTANT]
> **Quyền Administrator**: Khuyến khích chạy `zwindowscore.exe` bằng quyền Administrator (Run as Administrator) để phần mềm có thể quản lý, nhận diện sự kiện và snap các cửa sổ của các ứng dụng cũng đang chạy quyền Admin (như Task Manager, CMD Admin, IDE,...).

### 2. Biên dịch lại từ mã nguồn
Chạy lệnh dotnet CLI trong thư mục `src\zwindowscore`:
```powershell
# Build bản Self-Contained x64
dotnet publish "src\zwindowscore\zwindowscore.csproj" -c Release -r win-x64 --self-contained true -o "publish\self-contained"

# Build bản Framework-Dependent x64
dotnet publish "src\zwindowscore\zwindowscore.csproj" -c Release -r win-x64 --self-contained false -o "publish\framework-dependent"
```
