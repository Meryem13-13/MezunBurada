# MezunBurada — Sayfa ve Ürün Spesifikasyonu

> Bu doküman, Claude Code ile geliştirmeye başlarken referans olması için hazırlanmıştır. Faz 1 (MVP) öncelikli, Faz 2-3 ileride eklenecek sayfaları gösterir.

---

## Genel Bakış

- **Proje adı:** MezunBurada
- **Misyon:** Yeni mezunları bölümlerine ve seviyelerine göre test edip kişiselleştirilmiş bir kariyer yol haritasına yönlendiren, ücretsiz, sosyal fayda odaklı bir platform.
- **Önerilen stack:** ASP.NET Core (backend/API) + React veya Razor Pages (frontend, MVP için) + PostgreSQL/SQL Server. Mobil (Flutter) Faz 2-3'te.
- **Toplam sayfa sayısı:** Faz 1 (MVP): **13 sayfa** · Faz 2: **6 sayfa** · Faz 3: **5 sayfa** · Admin paneli: **6 sayfa** → **Toplam: 30 sayfa/ekran**

---

## FAZ 1 — MVP (İlk 90 Gün)

### 1. Ana Sayfa (`/`)
**Amaç:** Ziyaretçiyi teste yönlendirmek, güven oluşturmak.
**Bileşenler:**
- Üst menü: logo, arama çubuğu, giriş/kayıt butonu
- Hero bölümü: başlık + "Teste başla" CTA butonu
- Popüler bölümler grid'i (tıklanabilir kartlar → doğrudan o bölümün testine gider)
- Kullanıcı deneyimleri (yorum) bölümü — 3-4 öne çıkan yorum
- Alt bilgi (footer): Hakkımızda, İletişim, Gizlilik, SSS linkleri
**Veri ihtiyacı:** Öne çıkan bölümler listesi, öne çıkan yorumlar (admin tarafından işaretlenmiş)

### 2. Bölüm Seçimi — Test Adım 1 (`/test/bolum`)
**Amaç:** Kullanıcının mezun olduğu bölümü seçmesi.
**Bileşenler:** Kart grid (Bilgisayar Müh., Mimarlık, Endüstri Müh., İşletme, Elektrik-Elektronik — ilk sürümde 4-5 bölüm), her kartta ikon + isim
**Veri ihtiyacı:** Bölüm listesi tablosu

### 3. Alt Dallanma Testi — Test Adım 2 (`/test/{bolum}/ilgi-alani`)
**Amaç:** Seçilen bölüm içinde alt uzmanlık/ilgi alanını belirlemek (5-8 soru).
**Bileşenler:** Tek soru + çoktan seçmeli cevap ekranı, ilerleme çubuğu (1/6, 2/6...), "geri" butonu
**Veri ihtiyacı:** Bölüme özel soru bankası (ilgi tespiti tipi)

### 4. Seviye Tespit Testi — Test Adım 3 (`/test/{bolum}/{alt-dal}/seviye`)
**Amaç:** Kullanıcının teknik seviyesini ölçmek (10-15 soru, zorluk kademeli).
**Bileşenler:** Soru ekranı (Adım 2 ile aynı UI paterni), zamanlayıcı gerekmez, "testi bitir" butonu son soruda
**Veri ihtiyacı:** Bölüme/alt dala özel seviye testi soru bankası, puanlama mantığı

### 5. Sonuç / Yol Haritası Sayfası (`/yol-haritasi/{sonuc-id}`)
**Amaç:** Kişiselleştirilmiş yol haritasını göstermek — ürünün kalbi.
**Bileşenler:**
- Seviye özeti kartı ("Şu an: Başlangıç seviyesi, Backend odaklı")
- Adım adım checklist (öğrenilecek konular, sırayla)
- Her adımda: önerilen kaynak linki (affiliate), tahmini süre, "tamamladım" checkbox'ı
- Önerilen 1 proje fikri
- CV'ye eklenecek anahtar kelimeler kutusu
- "Sonucu kaydet" CTA'sı (kayıt olmaya teşvik)
**Veri ihtiyacı:** Yol haritası içerik tablosu (seviye × bölüm × alt dal kombinasyonuna göre), affiliate link tablosu

### 6. Giriş / Kayıt Sayfası (`/giris`, `/kayit`)
**Amaç:** Kullanıcı hesabı oluşturma, ilerleme takibi için.
**Bileşenler:** E-posta/şifre formu, Google ile giriş (opsiyonel), "neden kayıt olmalıyım" kısa açıklaması
**Veri ihtiyacı:** Kullanıcı tablosu (auth)

### 7. Kullanıcı Paneli / Dashboard (`/panel`)
**Amaç:** Kayıtlı kullanıcının kendi yol haritasını ve ilerlemesini görmesi.
**Bileşenler:** Aktif yol haritası özeti, ilerleme yüzdesi, tamamlanan/kalan adımlar listesi, "testi tekrar al" butonu
**Veri ihtiyacı:** Kullanıcı-yol haritası ilişki tablosu, ilerleme kayıtları

### 8. Bölüm Detay Sayfası (`/bolum/{bolum-adi}`)
**Amaç:** SEO ve organik trafik için — her bölümün kendi açılış sayfası (örn. "Bilgisayar Mühendisliği mezunu ne iş yapar").
**Bileşenler:** Bölüm hakkında kısa bilgi, o bölümdeki alt dallar, "teste başla" CTA'sı, o bölümden gelen yorumlar
**Veri ihtiyacı:** Bölüm açıklama içeriği (statik/CMS)

### 9. Deneyimler / Yorumlar Sayfası (`/deneyimler`)
**Amaç:** Tüm kullanıcı yorumlarının listelendiği sosyal kanıt sayfası.
**Bileşenler:** Filtre (bölüme göre), yorum kartları listesi (isim, bölüm, yorum metni, tarih), sayfalama
**Veri ihtiyacı:** Yorum tablosu (onaylanmış olanlar)

### 10. Deneyim Paylaşma Formu (`/deneyim-paylas`)
**Amaç:** Kullanıcının kendi deneyimini yazması.
**Bileşenler:** İsim, bölüm, yorum metni alanı, gönder butonu, "yorumun incelendikten sonra yayınlanır" notu
**Veri ihtiyacı:** Yorum tablosu (moderasyon bekleyen kayıt olarak eklenir)

### 11. Hakkımızda (`/hakkimizda`)
**Amaç:** Projenin hikayesini ve misyonunu anlatmak, güven inşa etmek. Sosyal fayda vurgusu burada güçlü olmalı.

### 12. SSS (`/sss`)
**Amaç:** Sık sorulan sorular — "ücretli mi?", "verilerim ne olacak?", "test ne kadar sürer?" gibi sorular.

### 13. Gizlilik Politikası & KVKK (`/gizlilik`)
**Amaç:** Yasal zorunluluk — kullanıcı verisi topladığın için KVKK uyumu şart.

---

## FAZ 2 — Büyütme (3-6. Ay)

### 14. Profil Sayfası (`/profil`)
Kullanıcının kendi bilgilerini düzenlediği, tamamladığı testleri gördüğü sayfa.

### 15. Arama Sonuçları Sayfası (`/arama?q=...`)
Üst menüdeki arama çubuğunun sonuç sayfası — bölüm, kurs veya içerik araması.

### 16. Kaynak/Kurs Detay Sayfası (`/kaynak/{id}`)
Yol haritasında önerilen bir kaynağa tıklandığında açılan ara sayfa (affiliate link öncesi, "bu kaynak neden önerildi" açıklaması + dış linke yönlendirme).

### 17. Üniversite/Kurumsal İşbirliği Sayfası (`/kurumlar-icin`)
B2B landing page — üniversite kariyer merkezlerine platformu tanıtan sayfa, iletişim formu.

### 18. İletişim Sayfası (`/iletisim`)
Genel iletişim formu.

### 19. Kullanım Şartları (`/kullanim-sartlari`)

---

## FAZ 3 — Olgunlaşma (6-12. Ay)

### 20. İşveren Girişi (`/isveren/giris`)
İşverenlerin platforma giriş yaptığı ayrı bir alan.

### 21. İşveren Paneli — Aday Havuzu (`/isveren/panel`)
Test edilmiş, seviyesi belli adayları filtreleyip görebildiği ücretli erişim ekranı (Faz 3 gelir modeli burada devreye girer).

### 22. İşveren Fiyatlandırma Sayfası (`/isveren/fiyatlandirma`)
Kurumsal üyelik paketlerini gösteren sayfa.

### 23. Premium Mentorluk Sayfası (`/mentorluk`)
Opsiyonel ücretli "uzmanla görüş" özelliğinin tanıtım ve rezervasyon sayfası.

### 24. Rozet/Başarı Sayfası (`/panel/basarilar`)
Kullanıcı motivasyonu için ilerleme rozetlerinin gösterildiği sayfa (gamification).

---

## Admin Paneli (Faz 1'den itibaren gerekli — ayrı bir alan)

### A1. Admin Giriş (`/admin/giris`)
### A2. Admin Dashboard (`/admin`)
Genel istatistikler: toplam kullanıcı, tamamlanan test sayısı, bölüme göre dağılım.

### A3. Soru Bankası Yönetimi (`/admin/sorular`)
Soru ekleme/düzenleme/silme — kod yazmadan içerik yönetimi burada kritik, senin daha önce yaptığın admin panel deneyimin doğrudan işine yarayacak.

### A4. Yol Haritası İçerik Yönetimi (`/admin/yol-haritalari`)
Her seviye/bölüm kombinasyonu için adım içeriklerini ve kaynak linklerini düzenleme.

### A5. Yorum Moderasyonu (`/admin/yorumlar`)
Bekleyen yorumları onaylama/reddetme.

### A6. Kullanıcı Yönetimi (`/admin/kullanicilar`)
Kayıtlı kullanıcı listesi, arama, temel yönetim.

---

## Veritabanı — Temel Tablolar (Taslak)

```
Users (id, email, password_hash, created_at)
Departments (id, name, slug, description)
SubFields (id, department_id, name)
Questions (id, sub_field_id, question_type[interest|level], text, options_json, difficulty)
TestResults (id, user_id, sub_field_id, level, completed_at)
RoadmapSteps (id, level, sub_field_id, order, title, description, resource_url, resource_type[free|affiliate])
UserProgress (id, user_id, roadmap_step_id, completed_at)
Reviews (id, user_id_or_name, department_id, text, status[pending|approved], created_at)
```

---

## Öncelik Sırası Önerisi (Claude Code ile geliştirme sırası)

1. Veritabanı şeması + backend API iskeleti (Users, Departments, Questions)
2. Test akışı (Sayfa 2-3-4) — ürünün çekirdeği
3. Sonuç/Yol haritası sayfası (Sayfa 5) — burada da değer teslim ediliyor
4. Ana sayfa (Sayfa 1) — akış çalıştıktan sonra vitrin
5. Giriş/Kayıt + Panel (Sayfa 6-7)
6. Admin paneli (A1-A4) — içerik eklemeyi kolaylaştırmak için
7. Deneyimler/Yorumlar (Sayfa 9-10) + Bölüm detay sayfaları (Sayfa 8) — SEO ve sosyal kanıt
8. Yasal sayfalar (11-13) — yayına almadan önce mutlaka tamamla
