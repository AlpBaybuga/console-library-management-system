# Konsol Kütüphane Yönetim Sistemi

Kitap envanterinin, üye kayıtlarının ve ödünç verme/iade işlemlerinin tek bir yerden, tutarlı ve izlenebilir biçimde yönetilmesini sağlayan, .NET 8 üzerinde çalışan bir konsol uygulamasıdır. Kütüphanelerde bu bilgilerin genellikle ayrı defterlerde tutulmasından kaynaklanan "hangi kitap kimde, ne zaman iade edilecek, rafta ne var" belirsizliğini ortadan kaldırmayı hedefler.

## Kullanılan Teknoloji ve Ön Koşullar

- **.NET SDK 8.0** (proje `net8.0` hedefiyle derlenir)
- Konsol uygulaması — veritabanı, Web API, Entity Framework veya başka bir ORM kullanılmamıştır
- Tüm veriler yalnızca uygulama çalışırken bellekte tutulur; uygulama kapatıldığında veriler kaybolur (beklenen davranıştır)

## Derleme ve Çalıştırma

```bash
# Proje klasörüne girin
cd ConsoleLibraryManagementSystem

# Bağımlılıkları geri yükleyip derleyin
dotnet build

# Uygulamayı çalıştırın
dotnet run
```

## Desteklenen Özellikler

- Kitap ekleme, listeleme, güncelleme, silme
- Üye ekleme ve listeleme
- Kitap ödünç verme ve iade alma
- İş kuralları: ödünçteki bir kitap tekrar ödünç verilemez, bir üye aynı anda en fazla 3 kitap ödünç alabilir, iade süresi otomatik 14 gün olarak belirlenir, ödünçteki bir kitap silinemez
- Doğrulama kuralları: kitap/üye alanları için zorunluluk, uzunluk ve format kontrolleri (örn. geçerli e-posta formatı, ISBN tekilliği, yayın yılı aralığı)
- Kategoriye göre filtreleme, duruma göre (rafta/ödünçte) filtreleme, başlık veya yazara göre arama
- Gecikmiş ödünç kayıtlarının listelenmesi ve belirli bir üyenin ödünç aldığı kitapların listelenmesi
- İstatistikler: toplam kitap/üye sayısı, kategoriye göre dağılım, rafta/ödünçteki kitap sayısı, gecikmiş kayıt sayısı, en çok ödünç alınan kitap ve kategori
- Hatalı girişlerde uygulama çökmez; anlamlı bir hata mesajı gösterip menüye geri döner

## Menü Kullanımına Örnek Akış

```
=== Kütüphane Yönetim Sistemi ===
1. Kitap Ekle
2. Kitapları Listele
3. Kitap Güncelle
4. Kitap Sil
5. Üye Ekle
6. Üyeleri Listele
7. Kitap Ödünç Ver
8. Kitap İade Al
9. Filtrele ve Ara
10. İstatistikleri Görüntüle
0. Çıkış
Seçiminiz: 1
Başlık: 1984
Yazar: George Orwell
ISBN: 978-0451524935
Yayın Yılı: 1949
Kategori seçin: 0-Novel 1-Science 2-History 3-Children 4-Other
Kategori: 0
'1984' adlı kitap eklendi. Id: 3f2a1c9e-....

Devam etmek için Enter'a basın...
```

Kitap ve üye eklerken dönen `Id` (Guid) değerleri, o kitabı/üyeyi ödünç verme, iade alma, güncelleme veya silme işlemlerinde kullanılır.

## Bilinen Kısıtlar ve Varsayımlar

- Veriler yalnızca bellekte tutulur; kalıcı depolama (dosya, veritabanı) yoktur.
- Kitap/üye/ödünç kayıtları arasındaki ilişkiler Id (Guid) üzerinden kurulur.
- Konsol arayüzü basit metin girişine dayanır; girilen Id'lerin listeleme ekranından kopyalanarak kullanılması beklenir.
