# Microservices Project

Bu proje, **Backend Developer Task (3. aşaması)** kapsamında geliştirilmiş bir mikroservis mimarisidir.  
Amaç, modern bir backend altyapısını **Docker üzerinde çalışan, güvenli, ölçeklenebilir ve yönetilebilir** bir şekilde tasarlamaktır.

---

## 🚀 Teknolojiler

- **.NET 8** (ASP.NET Core Web API)
- **PostgreSQL** (Her servis için ayrı veritabanı)
- **Redis** (Cache mekanizması)
- **MediatR + CQRS** (Komut/Sorgu ayrımı)
- **JWT Authentication + Role-based Authorization**
- **Ocelot API Gateway**
- **Entity Framework Core (Code-First)**
- **Swagger (OpenAPI)**
- **Docker & Docker Compose**
- **Rate Limiting**
- **Centralized Logging (LogService)**

---

## 📦 Servisler

| Servis         | Açıklama |
|----------------|----------|
| **AuthService** | Kullanıcı kaydı, login, JWT üretimi, Admin seeding |
| **ProductService** | Ürün & kategori CRUD, CQRS, Redis cache entegrasyonu |
| **LogService** | Mikroservislerin loglarını merkezi olarak toplar |
| **ApiGateway** | Ocelot tabanlı servis yönlendirme (Auth, Product, Log) |
| **Shared** | Ortak DTO ve yapıların bulunduğu katman |

---

## ⚙️ Kurulum

1. Repoyu klonlayın:
```
git clone https://github.com/FatihKlp/MicroservicesProject.git
cd MicroservicesProject
```
- .env dosyalarını servislerin içine ekleyin (örnekler repo içerisinde mevcut).

Docker Compose ile sistemi ayağa kaldırın:

```
docker compose up --build
```
Servisler çalışmaya başladıktan sonra şu adreslerden erişebilirsiniz:

- Api Gateway → http://localhost:8080

- AuthService Swagger → http://localhost:8081/swagger

- ProductService Swagger → http://localhost:8082/swagger

- LogService Swagger → http://localhost:8083/swagger

🔑 Test Kullanıcıları
- Admin kullanıcısı otomatik olarak seed edilmektedir.

.env içinde ayarlayabilirsiniz:

## .env
### Database Connection
- CONNECTION_STRING=
### Admin Seeding
- ADMIN_EMAIL=""
- ADMIN_PASSWORD=""
### JWT Configuration
- JWT_KEY=""
- JWT_ISSUER=""
- JWT_AUDIENCE=""
### Redis
- REDIS_CONNECTION=""
-----
Normal kullanıcılar için /auth/register endpoint’i kullanılabilir.

## 🛠 Özellikler
 - CQRS + MediatR ile Product & Category yönetimi

 - Redis Cache ile query sonuçlarının önbelleğe alınması

 - Cache Invalidation → POST/PUT/DELETE sonrası cache temizlenir

 - JWT Authentication ve Role-based Authorization

 - Admin Seeding (.env üzerinden email & şifre ayarlanır)

 - Rate Limiting (Ocelot + ASP.NET Core)

 - Centralized Logging (her işlem LogService’e kaydedilir)

 - Swagger UI (her serviste aktif)

 - Docker Compose (tüm sistem tek komutla ayağa kalkar)

## 📜 Örnek Akış
AuthService → /auth/register ile kullanıcı kaydı yapılır.

AuthService → /auth/login ile JWT token alınır.

ProductService → Token ile POST /api/products çağrılır.

LogService → Tüm başarılı ve hatalı istekler kaydedilir.

Redis → GET /api/products istekleri cache’den gelir, CRUD sonrası invalidation yapılır.

ApiGateway → Tüm istekler http://localhost:8080 üzerinden servis edilir.

📂 Proje Yapısı
vbnet
Kopyala
Düzenle
MicroservicesProject/
├── ApiGateway/
│   └── ocelot.json
├── AuthService/
│   └── Program.cs
├── ProductService/
│   ├── Controllers/
│   ├── CQRS/
│   ├── Program.cs
│   └── ...
├── LogService/
│   └── Program.cs
├── Shared/
│   └── DTOs/
├── docker-compose.yml
└── README.md

## 📝 Notlar
Her servis bağımsızdır → ayrı DB, ayrı migration.

API Gateway üzerinden birleşik erişim mümkündür.

Rate Limiting global tanımlanmıştır, isteğe göre controller bazlı da yapılabilir.

LogService tüm hataları & başarılı istekleri kayıt altına alır.