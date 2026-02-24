# ?? Social Network API

Backend API cho ?ng d?ng m?ng xã h?i, xây d?ng trên **.NET 6 Web API** v?i **Entity Framework Core**, **JWT Authentication**, **Cloudinary** (l?u tr? ?nh/video) và **BCrypt** (mã hóa m?t kh?u).

---

## ?? M?c l?c

- [Công ngh? s? d?ng](#-công-ngh?-s?-d?ng)
- [Yêu c?u h? th?ng](#-yêu-c?u-h?-th?ng)
- [Cài ??t & Ch?y project](#-cài-??t--ch?y-project)
- [C?u trúc th? m?c](#-c?u-trúc-th?-m?c)
- [C?u hình](#-c?u-hình)
- [API Reference](#-api-reference)
  - [Authentication](#authentication)
  - [User](#user)
  - [Post](#post)
  - [Comment](#comment)
  - [Category](#category)
  - [Role](#role)
- [Database Schema](#-database-schema)
- [B?o m?t](#-b?o-m?t)

---

## ?? Công ngh? s? d?ng

| Công ngh? | Phiên b?n | M?c ?ích |
|---|---|---|
| .NET | 6.0 | Web API Framework |
| Entity Framework Core | 6.0.16 | ORM & Database Migration |
| SQL Server | — | C? s? d? li?u |
| JWT Bearer | 6.0.16 | Xác th?c ng??i dùng |
| BCrypt.Net-Next | 4.0.3 | Mã hóa m?t kh?u |
| CloudinaryDotNet | 1.26.2 | L?u tr? ?nh/video trên Cloud |
| AutoMapper | 12.0.1 | Mapping DTO ? Entity |
| Swagger | 6.5.0 | API Documentation |

---

## ?? Yêu c?u h? th?ng

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (ho?c SQL Server Express)
- [dotnet-ef tool](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) (cho migration)
- Tài kho?n [Cloudinary](https://cloudinary.com/) (mi?n phí)

---

## ?? Cài ??t & Ch?y project

### 1. Clone repository

```bash
git clone https://github.com/truong086/SocialNetwork.git
cd SocialNetwork/SocialNetwork
```

### 2. C?u hình `appsettings.json`

C?p nh?t connection string và các thông tin c?n thi?t:

```json
{
  "ConnectionStrings": {
    "MyDB": "Server=YOUR_SERVER;Database=socialnetwork;Integrated Security=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY_AT_LEAST_32_CHARS",
    "Issuer": "YourDomain.com"
  },
  "Cloud": {
    "Cloudinary_Name": "your_cloud_name",
    "Api_Key": "your_api_key",
    "Serec_Key": "your_secret_key"
  }
}
```

### 3. T?o database

```bash
dotnet tool install --global dotnet-ef --version 6.0.16
dotnet ef database update --context DBContext
```

### 4. Ch?y project

```bash
dotnet run
```

M? trình duy?t t?i: **https://localhost:7171/swagger**

---

## ?? C?u trúc th? m?c

```
SocialNetwork/
??? Clouds/                  # Upload & qu?n lý file trên Cloudinary
?   ??? uploadCloud.cs       # Service upload/xóa file
?   ??? CloudUploadResult.cs # K?t qu? upload (Link, PublicId)
?   ??? Cloud.cs             # Config model Cloudinary
?   ??? KiemTraDinhDangFile.cs # Ki?m tra lo?i file (Image/Video/Audio/Document)
??? Common/                  # Các class dùng chung
?   ??? BaseEntity.cs        # Base model (id, deleted, createdAt, updatedAt)
?   ??? EncryptionHelper.cs  # BCrypt hash & SHA256 (backward compatible)
?   ??? PayLoad.cs           # Response wrapper chu?n
?   ??? PageList.cs          # Phân trang
??? Controllers/             # API Controllers
?   ??? UserController.cs
?   ??? PostController.cs
?   ??? CommentController.cs
?   ??? CategoryController.cs
?   ??? RoleController.cs
??? Mapper/                  # AutoMapper profiles
?   ??? UserMapper.cs
?   ??? PostMapper.cs
?   ??? CategoryMapper.cs
?   ??? RoleMapper.cs
??? Migrations/              # EF Core migrations
??? Models/                  # Entity models
?   ??? User.cs
?   ??? Post.cs
?   ??? Post_Image.cs
?   ??? image_user.cs
?   ??? Comment.cs
?   ??? CommentRep.cs
?   ??? Like.cs
?   ??? Category.cs
?   ??? role.cs
?   ??? Tick.cs
?   ??? DBContext.cs
??? Service/                 # Business logic
?   ??? IUserService.cs / UserService.cs
?   ??? IPostService.cs / PostService.cs
?   ??? ICommentService.cs / CommentService.cs
?   ??? ICategoryService.cs / CategoryService.cs
?   ??? IRoleService.cs / RoleService.cs
?   ??? IUserNameLoginService.cs / UserNameLoginService.cs
??? ViewModel/               # DTOs & Enums
?   ??? UserDTO.cs
?   ??? LoginDTO.cs
?   ??? ForgotPasswordDTO.cs
?   ??? UpdateProfileDTO.cs
?   ??? PostDTO.cs
?   ??? PostGetData.cs
?   ??? PostSortBy.cs
?   ??? CommentDTO.cs
?   ??? CategoryDTO.cs
?   ??? roleDTO.cs
?   ??? ImageUserUpload.cs
?   ??? Jwt.cs
?   ??? Status.cs
??? Program.cs               # Entry point & DI configuration
??? appsettings.json         # C?u hình ?ng d?ng
```

---

## ? C?u hình

| Key | Mô t? |
|---|---|
| `ConnectionStrings:MyDB` | Connection string t?i SQL Server |
| `Jwt:Key` | Secret key cho JWT (t?i thi?u 32 ký t?) |
| `Jwt:Issuer` | Issuer/Audience c?a JWT token |
| `Cloud:Cloudinary_Name` | Tên Cloudinary cloud |
| `Cloud:Api_Key` | API Key Cloudinary |
| `Cloud:Serec_Key` | Secret Key Cloudinary |

---

## ?? API Reference

> **Base URL:** `https://localhost:7171/api`
>
> **Authentication:** Các API có ?? yêu c?u header `Authorization: Bearer <token>`
>
> **Response format:** T?t c? API tr? v? d?ng:
> ```json
> {
>   "success": true,
>   "errorCode": 200,
>   "error": "OK",
>   "content": { ... }
> }
> ```

---

### Authentication

#### ??ng ký tài kho?n

```
POST /api/User/Add          [AllowAnonymous]
Content-Type: multipart/form-data
```

| Tham s? | Ki?u | B?t bu?c | Mô t? |
|---|---|---|---|
| `username` | string | ? | Tên ??ng nh?p (duy nh?t) |
| `email` | string | ? | Email (duy nh?t) |
| `password` | string | ? | M?t kh?u (???c hash b?ng BCrypt) |
| `fullname` | string | ? | H? tên |
| `phone` | string | ? | S? ?i?n tho?i |
| `quocgia` | string | ? | Qu?c gia |
| `image` | file | ? | ?nh ??i di?n (upload lên Cloudinary) |
| `role_id` | int | ? | ID role (m?c ??nh: User) |
| `signature_name` | string | ? | Tên ch? ký |
| `signature_font` | string | ? | Phông ch? ký |
| `signature_size` | int | ? | C? ch? ký |

---

#### ??ng nh?p

```
POST /api/User/Login         [AllowAnonymous]
Content-Type: application/json
```

| Tham s? | Ki?u | B?t bu?c | Mô t? |
|---|---|---|---|
| `username` | string | ? | Tên ??ng nh?p ho?c email |
| `password` | string | ? | M?t kh?u |

**Response thành công:**
```json
{
  "content": {
    "id": 1,
    "username": "john",
    "email": "john@mail.com",
    "fullname": "John Doe",
    "image": "https://res.cloudinary.com/...",
    "role": "User",
    "quocgia": "Vietnam",
    "phone": "0123456789",
    "signature_name": "John",
    "signature_font": "Dancing Script",
    "signature_size": 24,
    "token": "eyJhbGciOiJIUzI1NiIs..."
  }
}
```

> **L?u ý:** H? tr? c? BCrypt (user m?i) và SHA256 (user c?). User c? ??ng nh?p thành công s? ???c t? ??ng migrate sang BCrypt.

---

#### ??i m?t kh?u

```
POST /api/User/ForgotPassword    [AllowAnonymous]
Content-Type: application/json
```

| Tham s? | Ki?u | B?t bu?c | Mô t? |
|---|---|---|---|
| `email` | string | ? | Email tài kho?n |
| `oldPassword` | string | ? | M?t kh?u c? |
| `newPassword` | string | ? | M?t kh?u m?i |
| `confirmPassword` | string | ? | Xác nh?n m?t kh?u m?i |

---

### User

#### Xem thông tin cá nhân ??

```
GET /api/User/GetProfile
```

**Response:**
```json
{
  "content": {
    "fullname": "John Doe",
    "email": "john@mail.com",
    "phone": "0123456789",
    "image": "https://res.cloudinary.com/...",
    "quocgia": "Vietnam",
    "signature_name": "John",
    "signature_font": "Dancing Script",
    "signature_size": 24
  }
}
```

---

#### C?p nh?t thông tin cá nhân ??

```
PUT /api/User/UpdateProfile
Content-Type: multipart/form-data
```

| Tham s? | Ki?u | B?t bu?c | Mô t? |
|---|---|---|---|
| `fullname` | string | ? | H? tên m?i |
| `image` | file | ? | ?nh ??i di?n m?i (?nh c? b? xóa trên Cloudinary) |
| `signature_name` | string | ? | Tên ch? ký m?i |
| `signature_font` | string | ? | Phông ch? ký m?i |
| `signature_size` | int | ? | C? ch? ký m?i |

> Ch? c?p nh?t tr??ng có giá tr?, b? tr?ng thì gi? nguyên.

---

#### Danh sách ng??i dùng ??

```
GET /api/User/FindAll?name=john&page=1&pageSize=20
```

| Tham s? | Ki?u | M?c ??nh | Mô t? |
|---|---|---|---|
| `name` | string | null | Tìm theo username ho?c fullname |
| `page` | int | 1 | Trang hi?n t?i |
| `pageSize` | int | 20 | S? l??ng m?i trang |

---

#### Upload ?nh vào kho cá nhân ??

```
POST /api/User/uploadImageUser
Content-Type: multipart/form-data
```

| Tham s? | Ki?u | B?t bu?c | Mô t? |
|---|---|---|---|
| `file` | List\<file\> | ? | Danh sách ?nh (upload lên Cloudinary) |

---

#### Xem kho ?nh cá nhân ??

```
GET /api/User/FindAlluploadImageUser
```

**Response:**
```json
{
  "content": {
    "data": [ { "id": 1, "image": "https://...", "isUsed": false, "cretoredat": "..." } ],
    "like": 5,
    "comment": 12,
    "imageTotal": 8,
    "postTotal": 3
  }
}
```

---

#### Xóa ?nh trong kho ??

```
DELETE /api/User/DeleteImageUser?imageId=1
```

| Tham s? | Ki?u | B?t bu?c | Mô t? |
|---|---|---|---|
| `imageId` | int | ? | ID ?nh c?n xóa |

> Soft delete ?nh + xóa file th?c trên Cloudinary. Các `Post_Image` liên quan c?ng b? soft delete.

---

### Post

#### Danh sách t?t c? bài vi?t ??

```
GET /api/Post/FindAll?name=hello&category=2&sortBy=0&page=1&pageSize=20
```

| Tham s? | Ki?u | M?c ??nh | Mô t? |
|---|---|---|---|
| `name` | string | null | Tìm theo title, tên ng??i ??ng, ho?c category |
| `category` | int | null | L?c theo category_id |
| `sortBy` | enum | 0 | Cách s?p x?p (xem b?ng bên d??i) |
| `page` | int | 1 | Trang hi?n t?i |
| `pageSize` | int | 20 | S? l??ng m?i trang |

**Giá tr? `sortBy`:**

| Giá tr? | Tên | Mô t? |
|---|---|---|
| `0` | Newest | Bài vi?t m?i nh?t ? c? nh?t **(m?c ??nh)** |
| `1` | Oldest | Bài vi?t c? nh?t ? m?i nh?t |
| `2` | MostLiked | Nhi?u like nh?t ? ít like |
| `3` | MostCommented | Nhi?u bình lu?n nh?t ? ít bình lu?n |
| `4` | Liked | Ch? hi?n th? bài vi?t ?ã like |

**Response:**
```json
{
  "content": {
    "data": [
      {
        "id": 1,
        "id_user": 1,
        "name": "John Doe",
        "avatar": "https://...",
        "title": "Hello World",
        "content": "N?i dung bài vi?t",
        "isBackground": false,
        "category_id": 2,
        "category_name": "Technology",
        "totalComment": 5,
        "likes": 12,
        "isUserLike": true,
        "time": "2025-02-23T10:00:00+00:00",
        "images": ["https://...", "https://..."],
        "comments": [
          { "id": 1, "id_post": 1, "user": "Jane", "text": "Great!", "image_user": "https://..." }
        ]
      }
    ],
    "page": 1,
    "pageSize": 20,
    "totalCounts": 50,
    "totalPages": 3,
    "sortBy": "Newest"
  }
}
```

---

#### Bài vi?t c?a tôi ??

```
GET /api/Post/FindAllPostByUser?name=&category=&sortBy=0&page=1&pageSize=20
```

> Tham s? gi?ng `FindAll`, nh?ng ch? tr? bài vi?t c?a user ?ang ??ng nh?p.

---

#### T?o bài vi?t ??

```
POST /api/Post/Add
Content-Type: application/json
```

```json
{
  "title": "Tiêu ??",
  "description": "N?i dung bài vi?t",
  "isBackground": false,
  "category_id": 1,
  "images": [
    { "id": 5, "image": "https://...", "publicId": "folder/abc123" }
  ]
}
```

> `images` là danh sách ?nh ?ã có trong kho cá nhân (`image_users`). Khi gán vào bài vi?t, ?nh s? ???c ?ánh d?u `isUsed = true`.

---

#### T?o bài vi?t + Upload ?nh m?i ??

```
POST /api/Post/AddEditImage
Content-Type: multipart/form-data
```

| Tham s? | Ki?u | B?t bu?c | Mô t? |
|---|---|---|---|
| `images` | file | ? | ?nh m?i (upload Cloudinary + t?o bài vi?t luôn) |
| `category_id` | int | ? | ID category |

---

#### Like / Unlike bài vi?t ??

```
POST /api/Post/AddLike?data=1
```

| Tham s? | Ki?u | B?t bu?c | Mô t? |
|---|---|---|---|
| `data` | int | ? | ID bài vi?t |

> Toggle: l?n ??u = like, l?n sau = unlike, l?n sau n?a = like l?i...

**Response:**
```json
{
  "content": {
    "postId": 1,
    "totalLike": 13
  }
}
```

---

#### Xóa bài vi?t ??

```
DELETE /api/Post/DeleteById?id=1
```

| Tham s? | Ki?u | B?t bu?c | Mô t? |
|---|---|---|---|
| `id` | int | ? | ID bài vi?t |

> Soft delete bài vi?t + cascade soft delete: `Post_Images` (xóa Cloudinary), `Comments`, `CommentReps`, `Likes`.
> Ch? ch? bài vi?t m?i có quy?n xóa.

---

#### Test upload Cloudinary ??

```
POST /api/Post/addImageTestCloud
Content-Type: multipart/form-data
```

| Tham s? | Ki?u | B?t bu?c | Mô t? |
|---|---|---|---|
| `data` | file | ? | File c?n test upload |

---

### Comment

#### Thêm bình lu?n ??

```
POST /api/Comment/AddComment
Content-Type: application/json
```

```json
{
  "description": "N?i dung bình lu?n",
  "post_id": 1
}
```

**Response:**
```json
{
  "content": {
    "id": 10,
    "id_post": 1,
    "user": "John Doe",
    "text": "N?i dung bình lu?n",
    "image_user": "https://..."
  }
}
```

> T? ??ng c?p nh?t `totalComment` c?a bài vi?t.

---

### Category

#### Danh sách category ??

```
GET /api/Category/FindAll?name=tech&page=1&pageSize=20
```

| Tham s? | Ki?u | M?c ??nh | Mô t? |
|---|---|---|---|
| `name` | string | null | Tìm theo tên category |
| `page` | int | 1 | Trang |
| `pageSize` | int | 20 | S? l??ng/trang |

---

#### T?o category ??

```
POST /api/Category/Add
Content-Type: application/json
```

```json
{
  "name": "Technology",
  "description": "Bài vi?t v? công ngh?"
}
```

---

### Role

#### Danh sách role ??

```
GET /api/Role/FindAll?name=admin&page=1&pageSize=20
```

---

#### T?o role ??

```
POST /api/Role/Add
Content-Type: application/json
```

```json
{
  "name": "Admin"
}
```

---

## ?? Database Schema

```
????????????     ????????????     ????????????????
?   role    ???????   User   ???????  image_user  ?
????????????     ????????????     ????????????????
                      ?                    ?
                 ???????????               ?
                 ?    ?    ?               ?
                 ?    ?    ?               ?
           ????????????????????????? ?????????????
           ? Post ??Like??Category ? ?Post_Image ?
           ????????????????????????? ?????????????
              ?
         ???????????
         ?         ?
    ?????????????????????????
    ? Comment ??CommentRep  ?
    ?????????????????????????
```

**Quan h? chính:**
- `User` ? nhi?u `Post`, `image_user`, `Like`, `Comment`
- `Post` ? nhi?u `Post_Image`, `Like`, `Comment`
- `Comment` ? nhi?u `CommentRep`
- `image_user` ? nhi?u `Post_Image`
- `Category` ? nhi?u `Post`
- `role` ? nhi?u `User`

**Soft Delete:** T?t c? entity k? th?a `BaseEntity` có tr??ng `deleted` — d? li?u không b? xóa th?c, ch? ?ánh d?u.

---

## ?? B?o m?t

| Tính n?ng | Chi ti?t |
|---|---|
| **Mã hóa m?t kh?u** | BCrypt (salt 12 rounds). H? tr? t? ??ng migrate t? SHA256 |
| **JWT Token** | H?t h?n sau 120 phút. HMAC-SHA256 |
| **Authorization** | T?t c? API yêu c?u JWT (tr? Login, Register, ForgotPassword) |
| **CORS** | Ch? cho phép `http://localhost:8080` (frontend) |
| **Cloud Storage** | ?nh/video l?u trên Cloudinary, không l?u server |
| **Soft Delete** | D? li?u không b? xóa v?t lý, có th? khôi ph?c |
| **File Validation** | Gi?i h?n 10MB, ki?m tra ??nh d?ng (Image/Video/Audio/Document) |

---

## ?? License

Project này ???c phát tri?n cho m?c ?ích h?c t?p và nghiên c?u.
