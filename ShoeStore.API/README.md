# ShoeStore - Web Bán Giày

Một ứng dụng web bán giày hoàn chỉnh với API backend và frontend MVC.

## 🚀 Tính năng

### Backend API
- ✅ Quản lý sản phẩm (CRUD)
- ✅ Quản lý danh mục và thương hiệu
- ✅ Quản lý giỏ hàng
- ✅ Quản lý đơn hàng
- ✅ Xác thực người dùng
- ✅ AutoMapper cho mapping DTOs
- ✅ Entity Framework Core với SQL Server

### Frontend MVC
- ✅ Trang chủ với sản phẩm nổi bật
- ✅ Danh sách sản phẩm với filter và pagination
- ✅ Chi tiết sản phẩm
- ✅ Giỏ hàng với session storage
- ✅ Trang giới thiệu và liên hệ
- ✅ Responsive design với Bootstrap 5
- ✅ Font Awesome icons
- ✅ JavaScript tương tác

## 🛠️ Công nghệ sử dụng

### Backend
- ASP.NET Core 8.0
- Entity Framework Core
- SQL Server
- AutoMapper
- Swagger/OpenAPI

### Frontend
- ASP.NET Core MVC
- Bootstrap 5
- jQuery
- Font Awesome
- HTML5/CSS3/JavaScript

## 📁 Cấu trúc Project

```
ShoeStore/
├── ShoeStore.API/                 # Backend API
│   ├── Controllers/               # API Controllers
│   ├── Models/                    # Entity Models
│   ├── DTOs/                      # Data Transfer Objects
│   ├── Services/                  # Business Logic
│   ├── Repositories/              # Data Access
│   ├── Mappers/                   # AutoMapper Profiles
│   └── Data/                      # DbContext & Migrations
│
└── ShoeStore.Client/              # Frontend MVC
    ├── Controllers/               # MVC Controllers
    ├── Models/                    # View Models
    ├── Views/                     # Razor Views
    ├── Services/                  # API Client Services
    └── wwwroot/                   # Static Files
        ├── css/
        ├── js/
        └── images/
```

## 🚀 Cách chạy project

### 1. Cài đặt dependencies

```bash
# Restore NuGet packages
dotnet restore

# Install Entity Framework tools (nếu chưa có)
dotnet tool install --global dotnet-ef
```

### 2. Cấu hình database

1. Cập nhật connection string trong `ShoeStore.API/appsettings.json`
2. Chạy migrations:

```bash
cd ShoeStore.API
dotnet ef database update
```

### 3. Chạy ứng dụng

```bash
# Terminal 1: Chạy API
cd ShoeStore.API
dotnet run

# Terminal 2: Chạy Client
cd ShoeStore.Client
dotnet run
```

- API sẽ chạy tại: `https://localhost:7001`
- Client sẽ chạy tại: `https://localhost:5001`
- Swagger UI: `https://localhost:7001/swagger`

## 📋 API Endpoints

### Products
- `GET /api/Products` - Lấy tất cả sản phẩm
- `GET /api/Products/active` - Lấy sản phẩm đang hoạt động
- `GET /api/Products/{id}` - Lấy chi tiết sản phẩm
- `GET /api/Products/category/{id}` - Lấy sản phẩm theo danh mục
- `GET /api/Products/brand/{id}` - Lấy sản phẩm theo thương hiệu
- `POST /api/Products` - Tạo sản phẩm mới
- `PUT /api/Products/{id}` - Cập nhật sản phẩm
- `DELETE /api/Products/{id}` - Xóa sản phẩm

### Categories
- `GET /api/Categories` - Lấy tất cả danh mục
- `GET /api/Categories/active` - Lấy danh mục đang hoạt động
- `GET /api/Categories/{id}` - Lấy chi tiết danh mục

### Brands
- `GET /api/Brands` - Lấy tất cả thương hiệu
- `GET /api/Brands/active` - Lấy thương hiệu đang hoạt động
- `GET /api/Brands/{id}` - Lấy chi tiết thương hiệu

### Cart
- `GET /api/Cart/{customerId}` - Lấy giỏ hàng
- `POST /api/Cart/{customerId}/items` - Thêm sản phẩm vào giỏ
- `PUT /api/Cart/items/{itemId}` - Cập nhật số lượng
- `DELETE /api/Cart/items/{itemId}` - Xóa sản phẩm khỏi giỏ

### Orders
- `GET /api/Orders` - Lấy tất cả đơn hàng
- `GET /api/Orders/{id}` - Lấy chi tiết đơn hàng
- `POST /api/Orders` - Tạo đơn hàng mới
- `PUT /api/Orders/{id}/status` - Cập nhật trạng thái đơn hàng

## 🎨 Giao diện

### Trang chủ
- Hero section với call-to-action
- Danh mục sản phẩm
- Sản phẩm nổi bật
- Thương hiệu nổi bật
- Thông tin dịch vụ

### Danh sách sản phẩm
- Grid layout responsive
- Filter theo danh mục, thương hiệu
- Tìm kiếm theo tên
- Sắp xếp theo giá, tên, ngày tạo
- Pagination

### Chi tiết sản phẩm
- Hình ảnh sản phẩm
- Thông tin chi tiết
- Chọn kích thước và số lượng
- Thêm vào giỏ hàng
- Sản phẩm liên quan

### Giỏ hàng
- Danh sách sản phẩm đã chọn
- Cập nhật số lượng
- Xóa sản phẩm
- Tính tổng tiền
- Thanh toán

## 🔧 Cấu hình

### API Base URL
Cập nhật `ApiBaseUrl` trong `ShoeStore.Client/appsettings.json`:

```json
{
  "ApiBaseUrl": "https://localhost:7001/api/"
}
```

### Database Connection
Cập nhật connection string trong `ShoeStore.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ShoeStoreDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

## 📝 Ghi chú

- Project sử dụng session storage cho giỏ hàng
- Hình ảnh sản phẩm được lưu dưới dạng URL
- API có đầy đủ validation và error handling
- Frontend responsive và tương thích mobile
- Có thể mở rộng thêm tính năng authentication/authorization

## 🤝 Đóng góp

1. Fork project
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request

## 📄 License

Project này được phát hành dưới MIT License.

## 👨‍💻 Tác giả

Được tạo bởi [Your Name] - 2025 