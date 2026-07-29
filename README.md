# VulnLab.NET

VulnLab.NET, .NET tabanli bir **uygulamali web guvenligi egitim platformudur**.
Amac, gercek dunyada karsilasilan OWASP kaynakli guvenlik aciklarini kendi bilgisayarinda
guvenli bir lab ortami icinde ogrenmektir.

Bu projede her lab iki akistan olusur:
- **Vulnerable:** Acigin nasil olustugunu ve nasil istismar edildigini gosterir.
- **Secure:** Ayni sorunun guvenli kodla nasil cozuldugunu gosterir.

---

## Neden VulnLab.NET?

- Teoriyi degil, **pratik odakli ogrenmeyi** hedefler
- Ayni senaryoda hem **saldiran** hem **savunan** perspektifi sunar
- Burp Suite ile kolay test edilebilecek akislar icerir
- OWASP Top 10 kapsamindaki kritik konulari adim adim isler

---

## Ogrenme Akisi

Her lab benzer bir egitim patikasi izler:

1. Vulnerable Application
2. Acigi kesfet
3. Neden olustugunu anla
4. Burp Suite ile dogrula
5. Guvenli kod yaz / incele
6. Tekrar test et

---

## Mevcut Lablar

- SQL Injection
- Cross Site Scripting (XSS)
- CSRF
- IDOR
- File Upload
- JWT
- SSRF
- XXE
- Race Condition
- Command Injection

---

## Teknoloji Yigini

- ASP.NET Core MVC (.NET 8)
- Razor Views
- Bootstrap
- SQLite (egitim senaryolari icin)

---

## Baslangic

### Gereksinimler

- .NET 8 SDK

### Kurulum ve Calistirma

```bash
dotnet restore
dotnet run
```

Uygulama ayaga kalkinca ana sayfadan lab secerek testlere baslayabilirsin.

---

## Proje Yapisi (Ozet)

```text
Controllers/
  - HomeController.cs
  - SqlInjectionLabController (dosya adi su an LabsController.cs)
  - XssLabController.cs
  - CsrfLabController.cs
  - IdorLabController.cs
  - FileUploadLabController.cs
  - JwtLabController.cs
  - SsrfLabController.cs
  - XxeLabController.cs
  - RaceConditionLabController.cs
  - CommandInjectionLabController.cs

Views/
  - Home/Index.cshtml
  - Labs/*.cshtml
  - Shared/_Layout.cshtml
```

---

## Guvenlik ve Sorumluluk Notu

Bu proje sadece **egitim amacli**dir. Vulnerable akislarda kasitli olarak guvenlik aciklari bulunur.
Bu teknikleri izinsiz sistemlerde kullanmak etik ve hukuki olarak yanlistir.

---

## SEO Anahtar Niyetleri

Bu proje ozellikle su arama niyetlerine deger uretmek uzere tasarlanmistir:
- siber guvenlik egitimi
- owasp nedir
- owasp egitimi

---

## Yol Haritasi

- Kalan lablarin kapsamini derinlestirme (senaryo zorluk seviyeleri)
- Her lab icin otomatik test senaryolari
- Lab raporu / write-up cikti formati
- Docker ile tek komutta ortam kurulumu

## 🌐 Canlı Yayın

VulnLab.NET artık yayında!

🔗 **Web Sitesi:** https://berrey.com.tr

Proje, GitHub Actions ile oluşturulan CI/CD süreci sayesinde otomatik olarak dağıtılmaktadır. `main` dalına yapılan her gönderim (push), uygulamanın AWS EC2 sunucusuna otomatik olarak yayınlanmasını sağlar.

### Kullanılan Altyapı

- ASP.NET Core (.NET 9)
- AWS EC2
- Nginx Reverse Proxy
- Let's Encrypt SSL
- GitHub Actions (CI/CD)
- Özel Alan Adı (berrey.com.tr)

> 🚧 Proje aktif olarak geliştirilmektedir. Yeni OWASP Top 10 senaryoları, güvenlik açıkları ve uygulamalı laboratuvarlar düzenli olarak eklenecektir.