# DeliveryPlatform – Restoran Sipariş ve Canlı Kurye Takip Sistemi

Getir / Yemeksepeti benzeri, siparişlerin **RabbitMQ** kuyruğu üzerinden asenkron işlendiği ve kurye konumunun **SignalR** ile harita üzerinde canlı takip edildiği katmanlı bir .NET projesi.

## Özellikler

- Sipariş oluşturma isteği doğrudan veritabanına yazılmaz; JSON'a çevrilip RabbitMQ `order_queue` kuyruğuna gönderilir
- Ayrı bir **Worker Service** kuyruğu dinler, siparişi veritabanına işler ve mesajı onaylar (`ack`)
- SignalR `CourierHub` ile kurye konumunun anlık yayınlanması
- Leaflet + OpenStreetMap haritasında kuryenin canlı hareketi
- Kurye hareketini test etmek için simülasyon butonu
- Sipariş durumları: `Pending`, `Preparing`, `OnTheWay`, `Delivered`
- Generic Repository deseni ve servis katmanı

## Teknolojiler

- .NET 10 / ASP.NET Core MVC
- .NET Worker Service (BackgroundService)
- RabbitMQ (RabbitMQ.Client)
- ASP.NET Core SignalR
- Entity Framework Core 10 (SQLite)
- Leaflet.js, OpenStreetMap, Bootstrap

## Mimari

```
DeliveryPlatform/
├── DeliveryPlatform.Core/        # Entity'ler (Order, Restaurant, Courier), OrderStatus enum, IRepository
├── DeliveryPlatform.DataAccess/  # DeliveryDbContext, Generic Repository, migration'lar
├── DeliveryPlatform.Business/    # OrderService, RabbitMQPublisher
├── DeliveryPlatform.Web/         # MVC arayüz, OrderController (API), CourierHub (SignalR)
└── DeliveryPlatform.Worker/      # Kuyruğu dinleyip siparişleri kaydeden arka plan servisi
```

Akış:

```
Tarayıcı ──POST /api/Order──▶ Web ──publish──▶ RabbitMQ (order_queue) ──consume──▶ Worker ──▶ SQLite
Kurye (simülasyon) ──SendCourierLocation──▶ CourierHub ──ReceiveLocation──▶ Müşteri haritası
```

## Uç Noktalar

| Adres | Açıklama |
|-------|----------|
| `/` | Sipariş verme sayfası |
| `POST /api/Order` | Siparişi RabbitMQ kuyruğuna gönderir |
| `/Home/TrackCourier?orderId=1` | Canlı kurye takip haritası |
| `/courierHub` | SignalR hub'ı (`SendCourierLocation`, `ReceiveLocation`) |

## Gereksinimler

- .NET 10 SDK
- `localhost` üzerinde çalışan RabbitMQ (varsayılan port 5672). Docker ile:

```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

## Kurulum ve Çalıştırma

```bash
git clone https://github.com/smlmrt/DeliveryPlatform.git
cd DeliveryPlatform
dotnet restore
dotnet ef database update --project DeliveryPlatform.DataAccess --startup-project DeliveryPlatform.Web
```

Web uygulamasını ve Worker'ı iki ayrı terminalde çalıştırın:

```bash
cd DeliveryPlatform.Web && dotnet run
```

```bash
cd DeliveryPlatform.Worker && dotnet run
```

Web uygulaması `http://localhost:5047` adresinde açılır. Worker, `../DeliveryPlatform.Web/delivery.db` yolundaki veritabanını ortak kullandığı için kendi klasöründen çalıştırılmalıdır.
