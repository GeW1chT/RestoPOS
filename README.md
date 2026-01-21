# RestoPOS - Restaurant Point of Sale System

Modern bir restoran POS (Point of Sale) sistemi. C# .NET 8 ve PostgreSQL ile geliştirilmiştir.

## 🚀 Özellikler

- **Masa Yönetimi**: 20 masa desteği, masa durumu takibi (Boş, Dolu, Ödeme Bekleniyor)
- **Sipariş Yönetimi**: Sipariş oluşturma, ürün ekleme/çıkarma, sipariş güncelleme
- **Ürün Yönetimi**: Kategorili ürün listesi, stok takibi, KDV oranları
- **Mutfak Ekranı**: Bekleyen siparişlerin gerçek zamanlı görüntülenmesi
- **Ödeme İşlemleri**: Nakit ve kredi kartı desteği, para üstü hesaplama
- **Raporlama**: Günlük satış raporu, ürün satış analizi, kategori bazlı raporlar
- **Kullanıcı Yönetimi**: Çoklu kullanıcı desteği, rol bazlı erişim (Yönetici, Garson, Kasiyer, Mutfak)

## 🛠️ Teknolojiler

- **.NET 8** - Windows Forms
- **PostgreSQL** - Veritabanı
- **Entity Framework Core 8** - ORM
- **BCrypt.Net** - Şifre hashleme

## 📋 Gereksinimler

- .NET 8 SDK
- PostgreSQL 14+

## ⚙️ Kurulum

1. PostgreSQL'de `restpos` veritabanı oluşturun
2. Bağlantı bilgilerini `RestoPOS.Data/RestoPosContext.cs` içinde güncelleyin:
   ```
   Host=localhost;Port=5432;Database=restpos;Username=postgres;Password=YOUR_PASSWORD
   ```
3. Migration'ı uygulayın:
   ```bash
   dotnet ef database update --project RestoPOS.Data --startup-project RestoPOS.Presentation
   ```
4. Uygulamayı çalıştırın:
   ```bash
   dotnet run --project RestoPOS.Presentation
   ```

## 🔑 Varsayılan Giriş Bilgileri

| Kullanıcı Adı | Şifre | Rol |
|---------------|-------|-----|
| admin | admin123 | Yönetici |
| garson1 | admin123 | Garson |
| kasa1 | admin123 | Kasiyer |
| mutfak1 | admin123 | Mutfak |

## 📁 Proje Yapısı

```
RestoPOS/
├── RestoPOS.Common/        # Ortak enum ve yardımcı sınıflar
├── RestoPOS.Data/          # Entity Framework, Entities, Context
├── RestoPOS.Business/      # İş mantığı servisleri
└── RestoPOS.Presentation/  # Windows Forms UI
```

## 📸 Ekran Görüntüleri

- Masa Düzeni Ekranı
- Sipariş Formu
- Mutfak Ekranı
- Ödeme Ekranı
- Yönetim Paneli

## 📄 Lisans

MIT License
