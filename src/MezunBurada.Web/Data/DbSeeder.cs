using MezunBurada.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Data;

// Seeds real department content, one department at a time, as full content docs arrive.
// Idempotent per department (checked by slug) so re-running on startup is safe and future
// departments can be appended here without re-seeding existing ones.
public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        await SeedBilgisayarMuhendisligiAsync(db);
    }

    private static async Task SeedBilgisayarMuhendisligiAsync(ApplicationDbContext db)
    {
        if (await db.Departments.AnyAsync(d => d.Slug == "bilgisayar-muhendisligi"))
        {
            return;
        }

        var category = await db.Categories.FirstOrDefaultAsync(c => c.Slug == "bilgisayar-teknoloji");
        if (category is null)
        {
            category = new Category { Name = "Bilgisayar & Teknoloji", Slug = "bilgisayar-teknoloji", Icon = "💻" };
            db.Categories.Add(category);
        }

        // ---- 1. Temel Bölüm Bilgileri ----
        var department = new Department
        {
            Name = "Bilgisayar Mühendisliği",
            Slug = "bilgisayar-muhendisligi",
            ShortDescription = "Yazılım, algoritmalar, bilgisayar sistemleri ve yapay zeka gibi alanlarda uzmanlaşma imkanı sunan mühendislik bölümü.",
            LongDescription = "Bilgisayar Mühendisliği, donanım ve yazılımın kesişiminde yer alan; programlama, veri yapıları, algoritmalar, işletim sistemleri, ağlar, veritabanları ve yapay zeka gibi konuları kapsayan bir mühendislik dalıdır. Mezunlar hem yazılım geliştirme hem de sistem/altyapı tarafında çalışabilecek geniş bir teknik temele sahip olur.",
            DegreeType = DegreeType.Lisans,
            ExamType = ExamType.Sayisal,
            EducationDurationYears = 4,
            IsActive = true,
            MainTopics = "Programlama (C, C++, Java, Python), Veri Yapıları & Algoritmalar, İşletim Sistemleri, Veritabanı Sistemleri, Bilgisayar Ağları, Yazılım Mühendisliği, Yapay Zeka, Bilgisayar Mimarisi",
            SuitableFor = "Problem çözmeyi seven, mantıksal/analitik düşünen, teknolojiyle ilgilenen, sürekli öğrenmeye açık kişiler",
            ChallengingFor = "Soyut/matematiksel düşünmekte zorlananlar, uzun süreli tek başına odaklanma gerektiren işlerden sıkılanlar, hızlı değişime ayak uydurmakta zorlananlar",
            Category = category,
        };
        db.Departments.Add(department);

        // ---- 4. Alt Dallar (+ 3. Kariyer Alanları detayları — 7 of the 10 have full detail in the source doc) ----
        SubField MakeSubField(string name, string slug, string desc = "", string difficulty = "",
            string requiredSkills = "", string tech = "", string positions = "", string startingTopics = "") => new()
        {
            Name = name,
            Slug = slug,
            Description = desc,
            DifficultyLevel = difficulty,
            RequiredSkills = requiredSkills,
            RecommendedTechnologies = tech,
            ExamplePositions = positions,
            StartingTopics = startingTopics,
            Department = department,
        };

        var backend = MakeSubField("Backend", "backend-bilgisayar-muhendisligi",
            "Sunucu tarafı uygulamalar, API'ler, veritabanları ve iş mantığı üzerine çalışılan yazılım alanı.",
            "Orta", "Programlama, SQL, REST API, Git, Backend framework",
            "ASP.NET Core, Node.js, PostgreSQL, Docker",
            "Junior Backend Developer, Backend Developer, Software Developer",
            "HTTP temelleri, bir backend dili/framework'ü, SQL temelleri");

        var frontend = MakeSubField("Frontend", "frontend-bilgisayar-muhendisligi",
            "Kullanıcının doğrudan etkileşime girdiği arayüzleri geliştirme alanı.",
            "Başlangıç-Orta", "HTML/CSS, JavaScript, bir frontend framework (React/Vue)",
            "React, TypeScript, Tailwind CSS",
            "Junior Frontend Developer, UI Developer",
            "HTML/CSS temelleri, JavaScript temelleri, responsive tasarım");

        var yapayZeka = MakeSubField("Yapay Zeka", "yapay-zeka-bilgisayar-muhendisligi",
            "Veriden öğrenen modeller geliştirme, tahmin ve otomasyon sistemleri kurma alanı.",
            "İleri", "Python, İstatistik, Lineer Cebir, ML kütüphaneleri",
            "Python, scikit-learn, TensorFlow/PyTorch, Pandas",
            "Junior ML Engineer, Data Scientist, AI Engineer",
            "Python temelleri, istatistik, temel ML algoritmaları (KNN, Decision Tree)");

        var mobil = MakeSubField("Mobil Geliştirme", "mobil-gelistirme-bilgisayar-muhendisligi",
            "iOS/Android üzerinde çalışan uygulamalar geliştirme alanı.",
            "Orta", "Dart/Kotlin/Swift, State management, REST API tüketimi",
            "Flutter, React Native",
            "Junior Mobile Developer, Flutter Developer",
            "Flutter/Dart temelleri, UI bileşenleri, API entegrasyonu");

        var gomulu = MakeSubField("Gömülü Sistemler", "gomulu-sistemler-bilgisayar-muhendisligi",
            "Donanım ile yazılımı bir araya getiren, mikrodenetleyici tabanlı sistemler geliştirme alanı.",
            "İleri", "C/C++, Elektronik temelleri, Sensör entegrasyonu",
            "Arduino, Raspberry Pi, Embedded C",
            "Embedded Systems Engineer, IoT Developer",
            "C dili, temel elektronik, mikrodenetleyici programlama");

        var siberGuvenlik = MakeSubField("Siber Güvenlik", "siber-guvenlik-bilgisayar-muhendisligi",
            "Sistemleri, ağları ve verileri kötü niyetli saldırılara karşı koruma alanı.",
            "İleri", "Ağ bilgisi, İşletim sistemleri, Güvenlik protokolleri",
            "Linux, Wireshark, Burp Suite",
            "Junior Security Analyst, Penetration Tester",
            "Ağ temelleri, Linux, temel güvenlik kavramları");

        var devOps = MakeSubField("DevOps / Cloud", "devops-cloud-bilgisayar-muhendisligi",
            "Uygulamaların dağıtımı, ölçeklendirilmesi ve altyapı otomasyonu ile ilgilenen alan.",
            "Orta-İleri", "Linux, Docker, CI/CD, Cloud platformları",
            "Docker, Kubernetes, AWS/Azure, GitHub Actions",
            "Junior DevOps Engineer, Cloud Engineer",
            "Linux temelleri, Docker, temel CI/CD kavramları");

        // Listed as alt dallar in the source doc but no detailed "Kariyer Alanı" breakdown was given yet.
        var fullStack = MakeSubField("Full Stack", "full-stack-bilgisayar-muhendisligi");
        var veriBilimi = MakeSubField("Veri Bilimi", "veri-bilimi-bilgisayar-muhendisligi");
        var oyunGelistirme = MakeSubField("Oyun Geliştirme", "oyun-gelistirme-bilgisayar-muhendisligi");

        db.SubFields.AddRange(backend, frontend, yapayZeka, mobil, gomulu, siberGuvenlik, devOps,
            fullStack, veriBilimi, oyunGelistirme);

        // ---- 6. Skill Map ----
        Skill MakeSkill(string name, SkillCategory cat) => new() { Name = name, Category = cat };

        var skAlgoritma = MakeSkill("Algoritma", SkillCategory.Temel);
        var skProgramlama = MakeSkill("Programlama", SkillCategory.Temel);
        var skVeriYapilari = MakeSkill("Veri Yapıları", SkillCategory.Temel);
        var skProblemCozme = MakeSkill("Problem Çözme", SkillCategory.Temel);
        var skGit = MakeSkill("Git", SkillCategory.Teknik);
        var skSql = MakeSkill("SQL", SkillCategory.Teknik);
        var skApi = MakeSkill("API", SkillCategory.Teknik);
        var skLinux = MakeSkill("Linux", SkillCategory.Teknik);
        var skOop = MakeSkill("OOP", SkillCategory.Teknik);
        var skMl = MakeSkill("Machine Learning", SkillCategory.Uzmanlik);
        var skDl = MakeSkill("Deep Learning", SkillCategory.Uzmanlik);
        var skCloud = MakeSkill("Cloud", SkillCategory.Uzmanlik);
        var skCyberSec = MakeSkill("Cyber Security", SkillCategory.Uzmanlik);
        var skDistSystems = MakeSkill("Distributed Systems", SkillCategory.Uzmanlik);

        db.Skills.AddRange(skAlgoritma, skProgramlama, skVeriYapilari, skProblemCozme,
            skGit, skSql, skApi, skLinux, skOop, skMl, skDl, skCloud, skCyberSec, skDistSystems);

        foreach (var skill in new[]
                 {
                     skAlgoritma, skProgramlama, skVeriYapilari, skProblemCozme, skGit, skSql, skApi,
                     skLinux, skOop, skMl, skDl, skCloud, skCyberSec, skDistSystems,
                 })
        {
            db.DepartmentSkills.Add(new DepartmentSkill { Department = department, Skill = skill });
        }

        // ---- 5. Kariyer Hedefi Örneği: Backend Developer ----
        var backendDeveloper = new CareerPath
        {
            Name = "Backend Developer",
            Description = "Uygulamaların sunucu tarafı mantığını, veritabanı işlemlerini ve API'lerini geliştiren yazılımcı.",
            Difficulty = "Orta",
            SubField = backend,
        };
        db.CareerPaths.Add(backendDeveloper);

        foreach (var skill in new[] { skProgramlama, skSql, skApi, skGit })
        {
            db.CareerPathSkills.Add(new CareerPathSkill
            {
                CareerPath = backendDeveloper,
                Skill = skill,
                RequiredLevel = ProficiencyLevel.Beginner,
            });
        }

        // ---- 8. Bölüme Özel İlgi Testi ----
        // Some source options named two possible subfields at once (e.g. "Web sitesi → Frontend / Backend");
        // split into distinct single-target options since each answer maps to exactly one SubField here.
        InterestQuestion MakeInterestQuestion(string text, params (string Text, SubField MapsTo)[] options)
        {
            var q = new InterestQuestion { Text = text, Department = department };
            foreach (var (optText, mapsTo) in options)
            {
                q.Options.Add(new InterestQuestionOption { Text = optText, MapsToSubField = mapsTo });
            }
            return q;
        }

        db.InterestQuestions.AddRange(
            MakeInterestQuestion("Hangi proje seni daha çok heyecanlandırır?",
                ("Mobil uygulama", mobil),
                ("Web arayüzü (kullanıcının gördüğü kısım)", frontend),
                ("Web sunucusu (arka plan mantığı)", backend),
                ("Yapay zeka modeli", yapayZeka),
                ("Donanım sistemi", gomulu)),
            MakeInterestQuestion("Bir görevde en çok neyle uğraşmak istersin?",
                ("Kullanıcının gördüğü arayüz", frontend),
                ("Sunucu tarafı mantık", backend),
                ("Veriden anlam çıkarmak", veriBilimi),
                ("Donanım-yazılım entegrasyonu", gomulu)),
            MakeInterestQuestion("Takım içinde nasıl bir rol seni tanımlar?",
                ("Görsel/yaratıcı fikir üreten", frontend),
                ("Sistemi kuran, altyapıyı düşünen", devOps),
                ("Veriyle analiz yapan", veriBilimi),
                ("Güvenlik açıklarını arayan", siberGuvenlik)),
            MakeInterestQuestion("Hangi çalışma temposu sana daha uygun?",
                ("Hızlı görsel geri bildirim almak", frontend),
                ("Uzun vadeli, sistematik problemler çözmek", backend),
                ("Deneysel, tekrar tekrar test etmek", yapayZeka)),
            MakeInterestQuestion("Boş zamanında hangi konuyu araştırmayı seversin?",
                ("Yeni bir uygulama tasarımı", mobil),
                ("Bir sistemin nasıl çalıştığı", backend),
                ("Bir verinin içinde pattern bulmak", veriBilimi),
                ("Bir sistemin nasıl kırılabileceği", siberGuvenlik))
        );

        // ---- 9. Seviye Testi Örneği (Backend Alt Dalı) ----
        LevelQuestion MakeLevelQuestion(string text, QuestionDifficulty difficulty, Skill? skill, int points,
            params (string Text, bool IsCorrect)[] options)
        {
            var q = new LevelQuestion { Text = text, Difficulty = difficulty, Skill = skill, Points = points, SubField = backend };
            foreach (var (optText, isCorrect) in options)
            {
                q.Options.Add(new LevelQuestionOption { Text = optText, IsCorrect = isCorrect });
            }
            return q;
        }

        db.LevelQuestions.AddRange(
            MakeLevelQuestion("REST API ne işe yarar?", QuestionDifficulty.Easy, skApi, 2,
                ("Veri değişimi sağlar", true), ("Ekran tasarlar", false), ("Dosya sıkıştırır", false), ("Bilmiyorum", false)),
            MakeLevelQuestion("HTTP 404 ne anlama gelir?", QuestionDifficulty.Easy, skApi, 2,
                ("Sunucu hatası", false), ("Bulunamadı", true), ("Yetkisiz erişim", false), ("Bilmiyorum", false)),
            MakeLevelQuestion("SQL'de JOIN ne için kullanılır?", QuestionDifficulty.Medium, skSql, 3,
                ("Tablo birleştirme", true), ("Veri silme", false), ("Sıralama", false), ("Bilmiyorum", false)),
            MakeLevelQuestion("Rate limiting neden kullanılır?", QuestionDifficulty.Hard, skApi, 4,
                ("Hızlandırmak için", false), ("Kötüye kullanımı önlemek için", true), ("Estetik için", false), ("Bilmiyorum", false)),
            MakeLevelQuestion("Idempotency nedir?", QuestionDifficulty.Hard, null, 4,
                ("Aynı isteğin tekrar tekrar aynı sonucu vermesi", true), ("Şifreleme yöntemi", false),
                ("Veritabanı türü", false), ("Bilmiyorum", false))
        );

        // ---- Resources (used by the roadmap steps below) ----
        var resBtkAkademi = new Resource { Title = "BTK Akademi", Url = "https://www.btkakademi.gov.tr/", Type = ResourceType.Free };
        var resUdemyAspNet = new Resource { Title = "Udemy ASP.NET Core Kursu", Url = "https://www.udemy.com/", Type = ResourceType.Affiliate };
        var resSqlFree = new Resource { Title = "Ücretsiz SQL Kaynağı", Url = "https://www.w3schools.com/sql/", Type = ResourceType.Free };
        var resGitFree = new Resource { Title = "Ücretsiz Git Kaynağı", Url = "https://git-scm.com/doc", Type = ResourceType.Free };
        db.Resources.AddRange(resBtkAkademi, resUdemyAspNet, resSqlFree, resGitFree);

        // ---- 11. Örnek Projeler (Backend, Seviyeye Göre) ----
        var projGorevTakipApi = new Project
        {
            Name = "Basit bir Görev Takip API'si",
            Description = "Kullanıcının görev ekleyip listeleyebildiği basit bir CRUD API.",
            Difficulty = "Başlangıç",
            EstimatedDuration = "1-2 hafta",
            Technologies = "ASP.NET Core, SQLite",
            Steps = "1) Proje iskeletini kur, 2) Model oluştur, 3) Endpoint'leri yaz, 4) Test et, 5) GitHub'a yükle",
            CareerPath = backendDeveloper,
        };
        var projEticaretBackend = new Project
        {
            Name = "E-Ticaret Backend'i",
            Description = "Ürün, sepet, sipariş yönetimi olan bir API.",
            Difficulty = "Orta",
            EstimatedDuration = "4-6 hafta",
            Technologies = "ASP.NET Core, PostgreSQL, JWT Authentication",
            Steps = string.Empty,
            CareerPath = backendDeveloper,
        };
        var projMikroservis = new Project
        {
            Name = "Mikroservis Tabanlı Sipariş Sistemi",
            Description = "Birden fazla servise bölünmüş, mesaj kuyruğu ile haberleşen sistem.",
            Difficulty = "İleri",
            EstimatedDuration = "8+ hafta",
            Technologies = "Docker, RabbitMQ, ASP.NET Core, Redis",
            Steps = string.Empty,
            CareerPath = backendDeveloper,
        };
        db.Projects.AddRange(projGorevTakipApi, projEticaretBackend, projMikroservis);

        // ---- 10. Roadmap Örneği: Backend + Başlangıç Seviyesi ----
        var roadmap = new Roadmap
        {
            Title = "Backend Development — Başlangıç Yol Haritası",
            Level = ProficiencyLevel.Beginner,
            CareerPath = backendDeveloper,
        };
        db.Roadmaps.Add(roadmap);

        var step1 = new RoadmapStep
        {
            Order = 1,
            Title = "HTTP ve REST API Temelleri",
            Description = "Web'in nasıl çalıştığını, istek-yanıt döngüsünü öğren.",
            EstimatedDuration = "1 hafta",
            Skill = skApi,
            Resource = resBtkAkademi,
            Roadmap = roadmap,
        };
        db.RoadmapSteps.Add(step1);

        var step2 = new RoadmapStep
        {
            Order = 2,
            Title = "Bir Backend Framework Öğren",
            Description = "ASP.NET Core veya Node.js ile ilk API'ni yaz.",
            EstimatedDuration = "3 hafta",
            HowToApproach = "Önce dokümantasyonu oku (30 dk), sonra kursun ilk 3 bölümünü izle, ardından kendi başına aynısını tekrar yaz (kopyalamadan). Kendi başına basit bir CRUD API'si yazabildiğinde sıradaki adıma geç.",
            Skill = skProgramlama,
            Resource = resUdemyAspNet,
            SuggestedProject = projGorevTakipApi,
            Roadmap = roadmap,
            PrerequisiteStep = step1,
        };
        db.RoadmapSteps.Add(step2);

        var step3 = new RoadmapStep
        {
            Order = 3,
            Title = "Veritabanı Temelleri (SQL)",
            Description = "Tablo tasarımı, sorgu yazma, ilişkiler.",
            EstimatedDuration = "2 hafta",
            Skill = skSql,
            Resource = resSqlFree,
            Roadmap = roadmap,
        };
        db.RoadmapSteps.Add(step3);

        var step4 = new RoadmapStep
        {
            Order = 4,
            Title = "Git & GitHub",
            Description = "Versiyon kontrolü, branch, PR mantığı.",
            EstimatedDuration = "3 gün",
            Skill = skGit,
            Resource = resGitFree,
            Roadmap = roadmap,
        };
        db.RoadmapSteps.Add(step4);

        var step5 = new RoadmapStep
        {
            Order = 5,
            Title = "İlk Portföy Projesi",
            Description = "Öğrendiklerini birleştiren bir CRUD API yap.",
            EstimatedDuration = "2 hafta",
            SuggestedProject = projGorevTakipApi,
            Roadmap = roadmap,
            PrerequisiteStep = step4,
        };
        db.RoadmapSteps.Add(step5);

        // ---- 13. İş Pozisyonları (only the ones that belong to the Backend Developer path;
        // Data Analyst / ML Engineer / DevOps Engineer belong to career paths not yet detailed
        // in the source content, so they're skipped for this pass) ----
        db.JobRoles.AddRange(
            new JobRole
            {
                Title = "Junior Backend Developer",
                RequiredSkills = "temel CRUD, SQL",
                Level = "Başlangıç seviyesi",
                CareerPath = backendDeveloper,
            },
            new JobRole
            {
                Title = "Software Developer",
                RequiredSkills = "framework hakimiyeti, test yazma",
                Level = "Orta seviye",
                CareerPath = backendDeveloper,
            }
        );

        // ---- Piyasada Şu An Aranan (manually curated — placeholder note text/dates until
        // the admin panel CRUD is used to keep these current) ----
        var marketDemandNow = DateTime.UtcNow;
        db.MarketDemandSkills.AddRange(
            new MarketDemandSkill { SkillName = "ASP.NET Core", DemandNote = "İlanların %70'inde geçiyor", UpdatedAt = marketDemandNow, CareerPath = backendDeveloper },
            new MarketDemandSkill { SkillName = "SQL", DemandNote = "Neredeyse tüm backend ilanlarında aranıyor", UpdatedAt = marketDemandNow, CareerPath = backendDeveloper },
            new MarketDemandSkill { SkillName = "Docker", DemandNote = "Orta-üstü seviye ilanların %55'inde isteniyor", UpdatedAt = marketDemandNow, CareerPath = backendDeveloper },
            new MarketDemandSkill { SkillName = "REST API Tasarımı", DemandNote = "Junior ilanlarının çoğunda temel beklenti", UpdatedAt = marketDemandNow, CareerPath = backendDeveloper },
            new MarketDemandSkill { SkillName = "Git", DemandNote = "Pratik olarak tüm ilanlarda gerekli görülüyor", UpdatedAt = marketDemandNow, CareerPath = backendDeveloper }
        );

        // ---- 14. Sık Sorulan Sorular ----
        db.Faqs.AddRange(
            new Faq
            {
                Question = "Bilgisayar mühendisliği mezunu hangi alanlarda çalışabilir?",
                Answer = "Yazılım geliştirme, veri bilimi, siber güvenlik, gömülü sistemler, DevOps gibi geniş bir yelpazede çalışabilir.",
                Department = department,
            },
            new Faq
            {
                Question = "Bu bölümden mezun olduktan sonra yüksek lisans gerekli mi?",
                Answer = "Sektörde çalışmak için zorunlu değil, ancak akademik kariyer veya araştırma odaklı AI rolleri için faydalı olabilir.",
                Department = department,
            },
            new Faq
            {
                Question = "İş bulmak için hangi projeleri yapmalıyım?",
                Answer = "En az 2-3 tamamlanmış, GitHub'da paylaşılmış, gerçek bir problemi çözen proje önerilir.",
                Department = department,
            },
            new Faq
            {
                Question = "Hangi programlama dilini önce öğrenmeliyim?",
                Answer = "Backend için C#/Java/Python, frontend için JavaScript, mobil için Dart/Kotlin başlangıç noktası olabilir.",
                Department = department,
            },
            new Faq
            {
                Question = "Deneyimsiz olarak nasıl iş bulurum?",
                Answer = "Portföy projeleri, staj, açık kaynak katkıları ve networking en etkili yollardır.",
                Department = department,
            }
        );

        // ---- Sample approved reviews so /deneyimler isn't empty on day one ----
        db.Reviews.AddRange(
            new Review
            {
                Name = "Ece",
                Text = "Mezun olduktan sonra hangi alana yönelmem gerektiğini bilmiyordum. Test sonucunda backend tarafına daha yatkın olduğumu gördüm ve öğrenmem gereken adımlar gerçekten netleşti.",
                Level = "Başlangıç",
                Department = department,
                SubField = backend,
                Status = ReviewStatus.Approved,
            },
            new Review
            {
                Name = "Ahmet",
                Text = "En faydalı kısmı sadece bir alan önermesi değil, o alana ulaşmak için hangi becerileri geliştirmem gerektiğini göstermesiydi.",
                Level = "Orta",
                Department = department,
                SubField = backend,
                Status = ReviewStatus.Approved,
            },
            new Review
            {
                Name = "Zeynep",
                Text = "Kariyer seçeneklerini tek tek araştırmak yerine bana özel yolun düzenli şekilde gösterilmesi çok işime yaradı.",
                Level = "Başlangıç",
                Department = department,
                SubField = frontend,
                Status = ReviewStatus.Approved,
            }
        );

        await db.SaveChangesAsync();
    }
}
