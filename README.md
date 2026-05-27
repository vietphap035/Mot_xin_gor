# DACS_4# Mot_xin_gor (API_CHAT & Mobile App)

**Mot_xin_gor** là một hệ sinh thái ứng dụng trò chuyện (Chat Application) hoàn chỉnh, bao gồm hệ thống **Backend Web API** kết hợp thời gian thực (Real-time) và ứng dụng **Client Multi-platform** (Android, iOS, Windows, macOS) được xây dựng trên các nền tảng công nghệ mới nhất của Microsoft (.NET 8 và .NET MAUI). Hệ thống hỗ trợ nhắn tin văn bản, gửi hình ảnh, tạo nhóm trò chuyện và tích hợp hạ tầng cuộc gọi video/audio thông qua WebRTC.

---

## Tính năng nổi bật

- **Xác thực & Bảo mật (Authentication):** Hệ thống Đăng nhập / Đăng ký người dùng bảo mật kết hợp lưu trữ cơ sở dữ liệu qua Entity Framework Core.
- **Nhắn tin thời gian thực (Real-time Messaging):** Giao tiếp hai chiều tức thời giữa các client bằng **SignalR Hub**, tự động đồng bộ hóa trạng thái tin nhắn và cuộc hội thoại.
- **Quản lý phòng & Nhóm (Room & Group Management):** Hỗ trợ tạo phòng chat riêng tư (1-1) hoặc tạo nhóm chat nhiều thành viên với giao diện trực quan.
- **Hỗ trợ đa phương tiện (Multimedia):** Cho phép gửi tin nhắn văn bản văn thuần túy và đính kèm hình ảnh (hỗ trợ upload và lưu trữ trực tiếp trên server API).
- **Cuộc gọi Video/Audio (WebRTC Video Call):** Tích hợp giải pháp giao tiếp ngang hàng **WebRTC** kết hợp SignalR đóng vai trò Signaling Server giúp thiết lập cuộc gọi video chất lượng cao giữa các thiết bị.
- **Giao diện đa nền tảng (Multi-platform UI):** Ứng dụng client viết bằng **.NET MAUI**, tự động tối ưu hóa giao diện thích ứng mượt mà trên cả Mobile (Android/iOS) và Desktop (Windows/macOS).

---

## Công nghệ sử dụng

### 1. Backend (API_CHAT)
- **Framework:** ASP.NET Core Web API (.NET 8).
- **Real-time:** Microsoft AspNetCore SignalR Client (v10.0).
- **ORM & Database:** Entity Framework Core với cơ chế Migrations tự động quản lý cấu trúc bảng dữ liệu (`ApplicationDbContext`).
- **Signaling:** WebRTC Signaling Server tích hợp trực tiếp qua `Callhub.cs`.

### 2. Client (Mot_xin_gor Mobile/Desktop App)
- **Framework:** .NET MAUI (Multi-platform App UI) hỗ trợ Single Codebase.
- **Hybrid Content:** Sử dụng Blazor Hybrid / WebView kết hợp mã HTML5/JavaScript để xử lý luồng WebRTC (`call.html`).
- **Data Binding:** Giao diện trực quan thông qua `ChatTemplateSelector` và `CheckToTextConverter` giúp tối ưu hóa hiệu năng hiển thị tin nhắn.

### 3. Shared Library (ShareModel)
Dự án Class Library dùng chung giúp chuẩn hóa các đối tượng dữ liệu truyền tải giữa Client và Server:
- **Entity Models:** `User.cs`, `Room.cs`, `Messages.cs`, `UserRoom.cs`
- **DTOs:** `LoginModel.cs`, `SendMessageDTO.cs`, `CreateRoomDto.cs`
- **Configuration:** `ApiConfig.cs`, `MessageType.cs`

---

## Cấu trúc thư mục Source Code

```text
vietphap035/mot_xin_gor/
├── API_CHAT/                  # --- BACKEND SERVER ---
│   ├── Controllers/           # Các RESTful API endpoints (AuthController, ChatController)
│   ├── Data/                  # Cấu hình Entity Framework (`ApplicationDbContext`)
│   ├── Hubs/                  # SignalR Hubs xử lý kết nối thời gian thực (`Callhub.cs`)
│   ├── Migrations/            # Lịch sử và các file khởi tạo cấu trúc Database
│   ├── wwwroot/               # Thư mục chứa tài nguyên tĩnh, hình ảnh upload và `webrtc.html`
│   └── Program.cs             # Điểm khởi chạy cấu hình Server, DI Container, và Middleware
│
├── Mot_xin_gor/               # --- MULTI-PLATFORM CLIENT ---
│   ├── Platforms/             # Cấu hình đặc thù cho từng OS (Android, iOS, Windows, MacCatalyst)
│   ├── Resources/             # Fonts, Images, Styles, AppIcon và giao diện WebRTC (`call.html`)
│   ├── LoginPage.xaml         # Giao diện và logic Đăng nhập ứng dụng
│   ├── HomePage.xaml          # Giao diện chính (Danh sách phòng chat, danh sách tin nhắn)
│   ├── CreateGroupPage.xaml   # Giao diện chọn thành viên và tạo nhóm chat mới
│   └── MauiProgram.cs         # Khởi tạo và cấu hình dependency ứng dụng Client
│
└── ShareModel/                # --- SHARED LIBS / MODELS ---
    └── [Models & DTOs]        # Định nghĩa cấu trúc dữ liệu dùng chung cho cả Client & Server
```
---

# Hướng dẫn Cài đặt & Chạy ứng dụng

## 1️. Yêu cầu chuẩn bị

### Hệ điều hành hỗ trợ

- Windows 11
- macOS

---

### IDE hỗ trợ

- Visual Studio 2022 (v17.8+)
- JetBrains Rider

---

### Workloads cần cài đặt

Đối với Visual Studio 2022, cần cài các workloads sau:

- .NET Desktop Development
- ASP.NET and web development
- .NET Multi-platform App UI development

> Workload MAUI là bắt buộc để biên dịch và chạy ứng dụng Client đa nền tảng.

---

### Database

Hệ thống hỗ trợ:

- SQL Server
- SQL Server LocalDB
- Hoặc bất kỳ Database nào được cấu hình trong `appsettings.json`

---

# Triển khai Backend Server (API_CHAT)

## Bước 1: Di chuyển vào thư mục Backend

```bash
cd API_CHAT
```

---

## Bước 2: Cấu hình Database

Mở file:

```plaintext
appsettings.json
```

Cập nhật chuỗi kết nối:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=YourDatabase;Trusted_Connection=True;TrustServerCertificate=True"
}
```

---

## Bước 3: Cập nhật Database

Chạy lệnh sau để tạo/cập nhật cấu trúc cơ sở dữ liệu:

```bash
dotnet ef database update
```

---

## Bước 4: Khởi chạy API Server

```bash
dotnet run
```

Sau khi chạy thành công, hệ thống sẽ hiển thị URL local API, ví dụ:

```plaintext
https://localhost:7191
```

hoặc

```plaintext
http://localhost:5123
```

> Lưu lại URL này để cấu hình cho ứng dụng Client.

---

# Cấu hình & Chạy ứng dụng Client (Mot_xin_gor)

## Bước 1: Cấu hình API Endpoint

Mở file:

```plaintext
ShareModel/ApiConfig.cs
```

Cập nhật địa chỉ API:

```csharp
public const string BaseUrl = "https://localhost:7191";
```

> Thay bằng URL Backend đang chạy trên máy của bạn.

---

## Bước 2: Mở Solution

Mở solution bằng:

- Visual Studio 2022
- Hoặc JetBrains Rider

---

## Bước 3: Chọn Startup Project

Đặt project:

```plaintext
Mot_xin_gor
```

làm Startup Project.

---

## 📲 Bước 4: Chọn nền tảng chạy

Bạn có thể lựa chọn các nền tảng sau:

| Platform | Mô tả |
|---|---|
| Windows Machine | Chạy trực tiếp trên Windows Desktop |
| Android Emulator | Chạy giả lập Android |
| iOS Simulator | Chạy giả lập iOS |
| Local Device | Chạy trên thiết bị thật |

---

## Android Emulator Requirements

Để chạy Android Emulator cần:

- Bật VT-X / AMD-V trong BIOS
- Bật Hyper-V (Windows)
- Cài Android SDK
- Cài Android Emulator

---

## iOS Requirements

Để build iOS cần:

- macOS
- Xcode
- Kết nối Mac Build Host với Visual Studio

---

## Bước 5: Build và chạy ứng dụng

Nhấn:

```plaintext
F5
```

hoặc nút:

```plaintext
Start
```

để tiến hành build và chạy ứng dụng.

---

# Quy trình chạy đầy đủ

```plaintext
1. Chạy Database
        ↓
2. Chạy API_CHAT Backend
        ↓
3. Copy URL API
        ↓
4. Cấu hình ApiConfig.cs
        ↓
5. Chạy ứng dụng Mot_xin_gor
```

---

# Một số lỗi thường gặp

## Không kết nối được API

Kiểm tra:

- API đã chạy chưa
- URL trong `ApiConfig.cs`
- Firewall / HTTPS Certificate

---

## Android Emulator không khởi động

Kiểm tra:

- VT-X / AMD-V
- Hyper-V
- Android SDK
- RAM khả dụng

---

## Lỗi HTTPS localhost

Chạy lệnh:

```bash
dotnet dev-certs https --trust
```

---

# Ghi chú

- Backend cần chạy trước khi Client kết nối.
- Khi chạy trên thiết bị thật, cần thay `localhost` bằng IP LAN của máy chạy API.
- Đảm bảo Client và Server cùng mạng nội bộ nếu test trên mobile device.
