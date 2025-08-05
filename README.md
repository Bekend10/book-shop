# 📚 Book Shop - E-commerce Platform

Hệ thống quản lý cửa hàng sách trực tuyến được xây dựng với .NET 8 và kiến trúc microservices.

## 🚀 Tính năng chính

### 📖 Quản lý sách
- Thêm, sửa, xóa thông tin sách
- Quản lý tác giả và thể loại
- Tìm kiếm và lọc sách
- Đánh giá và nhận xét sách

### 👥 Quản lý người dùng
- Đăng ký, đăng nhập tài khoản
- Xác thực JWT
- Đăng nhập bằng Google
- Quản lý vai trò (Role-based)
- Quản lý địa chỉ giao hàng

### 🛒 Giỏ hàng và Đặt hàng
- Thêm sách vào giỏ hàng
- Quản lý giỏ hàng
- Đặt hàng và theo dõi trạng thái
- Lịch sử đơn hàng

### 💳 Thanh toán
- Tích hợp nhiều phương thức thanh toán
- Xử lý thanh toán an toàn
- Theo dõi trạng thái thanh toán

### 📊 Báo cáo và Thống kê
- Báo cáo doanh thu
- Xuất báo cáo Excel
- Thống kê bán hàng

### ✉️ Dịch vụ Email
- Gửi email xác nhận
- Email thông báo đơn hàng
- Template email tùy chỉnh

## 🏗️ Kiến trúc hệ thống

Dự án sử dụng kiến trúc microservices với 3 service chính:

### 1. **Book Shop Service** (Main API)
- **Port**: 5000 (Development)
- **Chức năng**: API chính cho quản lý sách, người dùng, đơn hàng, thanh toán
- **Database**: SQL Server với Entity Framework Core
- **Authentication**: JWT Bearer Token

### 2. **Cloudinary Service**
- **Port**: 5001 (Development)  
- **Chức năng**: Quản lý hình ảnh và file upload
- **Cloud Storage**: Cloudinary

### 3. **Payment Service**
- **Port**: 5002 (Development)
- **Chức năng**: Xử lý thanh toán và tích hợp payment gateway

## 🛠️ Công nghệ sử dụng

### Backend
- **.NET 8** - Framework chính
- **ASP.NET Core Web API** - Web API
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **JWT** - Authentication
- **Swagger** - API Documentation

### Thư viện chính
- **BCrypt.Net** - Mã hóa mật khẩu
- **MailKit** - Gửi email
- **EPPlus** - Xuất Excel
- **Google.Apis.Auth** - Google Authentication
- **CloudinaryDotNet** - Cloud storage
- **Newtonsoft.Json** - JSON processing

### Tools & Services
- **Cloudinary** - Image hosting
- **Google OAuth** - Authentication
- **SMTP** - Email service

## 📁 Cấu trúc dự án

```
book-shop/
├── book-shop/                  # Main API Service
│   ├── Controllers/           # API Controllers
│   ├── Data/                 # Database Context
│   ├── Dto/                  # Data Transfer Objects
│   ├── Models/               # Entity Models
│   ├── Services/             # Business Logic
│   ├── Repositories/         # Data Access Layer
│   ├── Middlewares/          # Custom Middlewares
│   ├── EmailService/         # Email Services
│   ├── EmailTemplates/       # Email Templates
│   ├── Migrations/           # EF Migrations
│   └── ...
├── cloudinary-service/        # Image Upload Service
└── payment-service/          # Payment Processing Service
```

## ⚙️ Cài đặt và Chạy dự án

### Yêu cầu hệ thống
- .NET 8 SDK
- SQL Server
- Visual Studio 2022 hoặc VS Code

### 1. Clone repository
```bash
git clone https://github.com/Bekend10/book-shop.git
cd book-shop
```

### 2. Cấu hình Database
1. Cập nhật connection string trong `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=BookShopDB;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

2. Chạy migration:
```bash
cd book-shop
dotnet ef database update
```

### 3. Cấu hình dịch vụ bên ngoài

#### Email Service (MailKit)
```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password"
  }
}
```

#### Google OAuth
```json
{
  "GoogleAuth": {
    "ClientId": "your-google-client-id",
    "ClientSecret": "your-google-client-secret"
  }
}
```

#### Cloudinary
```json
{
  "Cloudinary": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  }
}
```

### 4. Chạy các service

#### Chạy tất cả service cùng lúc:
```bash
dotnet build
dotnet run --project book-shop
dotnet run --project cloudinary-service
dotnet run --project payment-service
```

#### Hoặc sử dụng Visual Studio:
- Mở file `book-shop.sln`
- Set multiple startup projects
- Chạy solution

## 📚 API Documentation

Sau khi chạy dự án, truy cập Swagger UI:

- **Main API**: `https://localhost:5001/swagger`
- **Cloudinary Service**: `https://localhost:5011/swagger`  
- **Payment Service**: `https://localhost:5021/swagger`

### API Endpoints chính

#### Authentication
- `POST /api/accounts/register` - Đăng ký tài khoản
- `POST /api/accounts/login` - Đăng nhập
- `POST /api/accounts/google-login` - Đăng nhập Google

#### Books Management
- `GET /api/books` - Lấy danh sách sách
- `POST /api/books` - Thêm sách mới
- `PUT /api/books/{id}` - Cập nhật sách
- `DELETE /api/books/{id}` - Xóa sách

#### Cart & Orders
- `GET /api/carts` - Lấy giỏ hàng
- `POST /api/carts` - Thêm vào giỏ hàng
- `POST /api/orders` - Tạo đơn hàng
- `GET /api/orders` - Lấy danh sách đơn hàng

## 🔐 Authentication & Authorization

Hệ thống sử dụng JWT Bearer Token với các role sau:
- **Admin** - Quản trị viên
- **User** - Người dùng thông thường

### Cách sử dụng API có xác thực:
```bash
curl -H "Authorization: Bearer {your-jwt-token}" \
     https://localhost:5001/api/books
```

## 📊 Database Schema

Hệ thống sử dụng các bảng chính:
- **Accounts** - Thông tin tài khoản
- **Books** - Thông tin sách
- **Authors** - Tác giả
- **Categories** - Thể loại
- **Orders** - Đơn hàng
- **OrderDetails** - Chi tiết đơn hàng
- **Carts** - Giỏ hàng
- **BookReviews** - Đánh giá sách
- **Addresses** - Địa chỉ giao hàng

## 🧪 Testing

Sử dụng các file `.http` trong mỗi project để test API:
- `book-shop/book-shop.http`
- `cloudinary-service/cloudinary-service.http`
- `payment-service/payment-service.http`

## 🚀 Deployment

### Development
```bash
dotnet run --environment Development
```

### Production
```bash
dotnet publish -c Release
# Deploy to your hosting service
```

## 📝 Logging

Hệ thống có middleware logging để ghi lại các request:
- **RequestLoggingMiddleware** - Log tất cả HTTP requests
- **JWTMiddleware** - Xử lý JWT authentication

## 🤝 Đóng góp

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.

## 📞 Liên hệ

- **Developer**: Nguyên Tiến Dũng
- **Email**: [bekendwork@gmail.com]
- **GitHub**: [@Bekend10](https://github.com/Bekend10)

## 🎯 Roadmap

- [ ] Tích hợp thanh toán VNPay
- [ ] Hệ thống thông báo real-time
- [ ] Mobile app với React Native
- [ ] AI recommendation system
- [ ] Multi-language support
- [ ] Advanced analytics dashboard

---

⭐️ **Nếu project này hữu ích, hãy cho một star nhé!** ⭐️
