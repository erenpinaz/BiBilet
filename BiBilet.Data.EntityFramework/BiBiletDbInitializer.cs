using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BiBilet.Domain.Entities.Application;
using BiBilet.Domain.Entities.Identity;

namespace BiBilet.Data.EntityFramework
{
    internal class BiBiletDbInitializer : DropCreateDatabaseIfModelChanges<BiBiletContext>
    {
        /// <summary>
        /// Seeds the database with data
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(BiBiletContext context)
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = "testioruserfindel",
                Name = "Testior Userfindel",
                Email = "testior@userfindel.com",
                PasswordHash = "AOgkKfvdMQGT6Hf7iG5Hl/rz6dPh2r09CxOq2+rtcwsUZbtf0a9CpzR1b14QsqzoCg==",
                SecurityStamp = "cebd32cf-0315-4e28-a8f4-30129bdfc952",
                Organizers = new List<Organizer>
                {
                    new Organizer
                    {
                        OrganizerId = Guid.NewGuid(),
                        Name = "Testior Userfindel",
                        Description = "Testior Userfindel kullanıcısının organizatör hesabı.",
                        Image = "/assets/images/placeholder.png",
                        Website = "http://www.testioruserfindel.com",
                        Slug = "testior-userfindel",
                        IsDefault = true
                    }
                }
            };

            context.Users.Add(user);
            context.SaveChanges();

            var categories = new List<Category>
            {
                new Category {CategoryId = Guid.NewGuid(), Name = "İmza Günü", Slug = "imza-gunu"},
                new Category {CategoryId = Guid.NewGuid(), Name = "Gezi veya Kamp", Slug = "gezi-kamp"},
                new Category {CategoryId = Guid.NewGuid(), Name = "Çalıştay", Slug = "calistay"},
                new Category {CategoryId = Guid.NewGuid(), Name = "Konser", Slug = "konser"},
                new Category {CategoryId = Guid.NewGuid(), Name = "Konferans", Slug = "konferans"},
                new Category {CategoryId = Guid.NewGuid(), Name = "Kongre", Slug = "kongre"},
                new Category {CategoryId = Guid.NewGuid(), Name = "Gala", Slug = "gala"},
                new Category {CategoryId = Guid.NewGuid(), Name = "Festival veya Sergi", Slug = "festival-sergi"},
                new Category {CategoryId = Guid.NewGuid(), Name = "Oyun veya Yarışma", Slug = "oyun-yarisma"},
                new Category {CategoryId = Guid.NewGuid(), Name = "Tanışma", Slug = "tanisma"},
                new Category {CategoryId = Guid.NewGuid(), Name = "Eğitim", Slug = "egitim"},
                new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = "Parti veya Sosyal Buluşma",
                    Slug = "parti-sosya-bulusma"
                },
                new Category {CategoryId = Guid.NewGuid(), Name = "Yarış", Slug = "yaris"},
                new Category {CategoryId = Guid.NewGuid(), Name = "Ralli", Slug = "ralli"},
                new Category {CategoryId = Guid.NewGuid(), Name = "Sahne Sanatları", Slug = "sahne-sanatlari"},
                new Category {CategoryId = Guid.NewGuid(), Name = "Seminer veya Söyleşi", Slug = "seminer-soylesi"},
                new Category {CategoryId = Guid.NewGuid(), Name = "Webiner", Slug = "webiner"},
                new Category {CategoryId = Guid.NewGuid(), Name = "Tur", Slug = "tur"},
                new Category {CategoryId = Guid.NewGuid(), Name = "Turnuva", Slug = "turnuva"},
                new Category {CategoryId = Guid.NewGuid(), Name = "Diğer", Slug = "diger"}
            };

            categories.ForEach(c => context.Categories.Add(c));
            context.SaveChanges();

            var topics = new List<Topic>
            {
                new Topic {TopicId = Guid.NewGuid(), Name = "Bilim & Teknoloji"},
                new Topic {TopicId = Guid.NewGuid(), Name = "Araba, Tekne & Uçak"},
                new Topic {TopicId = Guid.NewGuid(), Name = "İşletme & Profesyonel"},
                new Topic {TopicId = Guid.NewGuid(), Name = "Hayır"},
                new Topic {TopicId = Guid.NewGuid(), Name = "Topluluk & Kültür"},
                new Topic {TopicId = Guid.NewGuid(), Name = "Aile & Eğitim"},
                new Topic {TopicId = Guid.NewGuid(), Name = "Moda & Güzellik"},
                new Topic {TopicId = Guid.NewGuid(), Name = "Film, Medya ve Eğlence"},
                new Topic {TopicId = Guid.NewGuid(), Name = "Yeme İçme"},
                new Topic {TopicId = Guid.NewGuid(), Name = "Devlet & Politika"},
                new Topic {TopicId = Guid.NewGuid(), Name = "Sağlık"},
                new Topic {TopicId = Guid.NewGuid(), Name = "Hobiler ve İlgi Alanları"},
                new Topic {TopicId = Guid.NewGuid(), Name = "Ev & Yaşam"},
                new Topic {TopicId = Guid.NewGuid(), Name = "Müzik"},
                new Topic {TopicId = Guid.NewGuid(), Name = "Performans & Görsel Sanatlar"},
                new Topic {TopicId = Guid.NewGuid(), Name = "Din & Maneviyat"},
                new Topic {TopicId = Guid.NewGuid(), Name = "Mevsimlik & Tatil"},
                new Topic {TopicId = Guid.NewGuid(), Name = "Spor & Fitness"},
                new Topic {TopicId = Guid.NewGuid(), Name = "Diğer"}
            };

            topics.ForEach(t => context.Topics.Add(t));
            context.SaveChanges();

            var subtopics = new List<SubTopic>
            {
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Mobil", Topic = topics[0]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Biyoteknoloji", Topic = topics[0]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Yüksek Teknoloji", Topic = topics[0]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "İlaç", Topic = topics[0]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[0]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Robotik", Topic = topics[0]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Sosyal Medya", Topic = topics[0]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Bilim", Topic = topics[0]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Uçak", Topic = topics[1]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Araba", Topic = topics[1]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Tekne", Topic = topics[1]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Morotsiklet/ATV", Topic = topics[1]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[1]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Kariyer", Topic = topics[2]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Tasarım", Topic = topics[2]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Eğitmenler", Topic = topics[2]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Çevre & Süreklilik", Topic = topics[2]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Finans", Topic = topics[2]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Medya", Topic = topics[2]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Sivil Toplum, STK'lar", Topic = topics[2]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[2]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Emlak", Topic = topics[2]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Satış & Pazarlama", Topic = topics[2]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Sermaye & Küçük İşletme", Topic = topics[2]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Hayvan Refahı", Topic = topics[3]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "After Yardımı", Topic = topics[3]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Eğitim", Topic = topics[3]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Çevre", Topic = topics[3]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Sağlık", Topic = topics[3]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "İnsan Hakları", Topic = topics[3]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Uluslararası Yardım", Topic = topics[3]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[3]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Yoksulluk", Topic = topics[3]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Şehir/Kasaba", Topic = topics[4]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Ülke", Topic = topics[4]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Miras", Topic = topics[4]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "LGBT", Topic = topics[4]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Dil", Topic = topics[4]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Ortaçağ", Topic = topics[4]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Vatandaşlık", Topic = topics[4]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[4]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Rönesans", Topic = topics[4]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Devlet", Topic = topics[4]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Mezunlar Derneği", Topic = topics[5]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Bebek", Topic = topics[5]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Çocuk & Genç", Topic = topics[5]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Eğitim", Topic = topics[5]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[5]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Ebeveynlik", Topic = topics[5]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Kavuşma", Topic = topics[5]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Okul-Aile Birliği", Topic = topics[5]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Aksesuarlar", Topic = topics[6]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Güzellik", Topic = topics[6]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Gelinlik", Topic = topics[6]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Moda", Topic = topics[6]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[6]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Yetişkin", Topic = topics[7]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Anime", Topic = topics[7]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Komedi", Topic = topics[7]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Film", Topic = topics[7]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Oyun", Topic = topics[7]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[7]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "TV", Topic = topics[7]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Alkollü İçecekler", Topic = topics[8]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Gıda", Topic = topics[8]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[8]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Şarap", Topic = topics[8]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Yerel Yönetim", Topic = topics[9]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Partiler", Topic = topics[9]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Tarafsız", Topic = topics[9]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[9]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Devlet Yönetimi", Topic = topics[9]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Tıbbi", Topic = topics[10]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Akıl Sağlığı", Topic = topics[10]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[10]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Kişisel Sağlık", Topic = topics[10]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "SPA", Topic = topics[10]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Yoga", Topic = topics[10]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Yetişkin", Topic = topics[11]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Anime/Çizgi Roman", Topic = topics[11]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Kitaplar", Topic = topics[11]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "DIY", Topic = topics[11]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Çizim & Boyama", Topic = topics[11]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Oyun", Topic = topics[11]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Örgü", Topic = topics[11]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[11]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Fotoğrafçılık", Topic = topics[11]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Buluşma", Topic = topics[12]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Ev & Bahçe", Topic = topics[12]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[12]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Evcil Hayvanlar", Topic = topics[12]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Alternatif", Topic = topics[13]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Blues & Jazz", Topic = topics[13]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Klasik", Topic = topics[13]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Country", Topic = topics[13]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Kültürel", Topic = topics[13]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Elektronik", Topic = topics[13]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Folk", Topic = topics[13]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Hip Hop / Rap", Topic = topics[13]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Indie", Topic = topics[13]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Latin", Topic = topics[13]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Metal", Topic = topics[13]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Opera", Topic = topics[13]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[13]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Pop", Topic = topics[13]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "R&B", Topic = topics[13]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Reggae", Topic = topics[13]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Dini", Topic = topics[13]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Rock", Topic = topics[13]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Bale", Topic = topics[14]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Komedi", Topic = topics[14]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "El Sanatı", Topic = topics[14]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Dans", Topic = topics[14]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Güzel Sanatlar", Topic = topics[14]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Edebi Sanatlar", Topic = topics[14]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Müzikal", Topic = topics[14]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Opera", Topic = topics[14]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Orkestra", Topic = topics[14]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[14]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Tiyatro", Topic = topics[14]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Budizm", Topic = topics[15]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Hristiyanlık", Topic = topics[15]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "İslam", Topic = topics[15]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Musevilik", Topic = topics[15]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Mormonluk", Topic = topics[15]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Mistikçilik", Topic = topics[15]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Yeni Çağ", Topic = topics[15]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Yılbaşı", Topic = topics[16]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[16]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Belirli Günler", Topic = topics[16]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Basketbol", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Futbol", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Beyzbol", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Bisiklet", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Egzersiz", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Dövüş & Dövüş Sporları ", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Golf", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Hokey", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Motorsporları", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Dağ Bisikletçiliği", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Engelli", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Rugby", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Koşma", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Kar Sporları", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Yüzme & Su Sporları", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Tenis", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Voleybol", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Yoga", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[17]},
                new SubTopic {SubTopicId = Guid.NewGuid(), Name = "Diğer", Topic = topics[18]}
            };

            subtopics.ForEach(s => context.SubTopics.Add(s));
            context.SaveChanges();

            var venues = new List<Venue>
            {
                new Venue
                {
                    VenueId = Guid.NewGuid(),
                    Name = "Orta Doğu Teknik Üniversitesi Kampüsü",
                    Address = "Üniversiteler Mahallesi, Dumlupınar Bulvarı No: 1 06800, Çankaya",
                    City = "Ankara",
                    Country = "Türkiye"
                }
            };

            venues.ForEach(v => context.Venues.Add(v));
            context.SaveChanges();

            var events = new List<Event>
            {
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = "Android Geliştirici Günleri",
                    Published = true,
                    Description = "Android Geliştirici Günleri Açıklaması",
                    Image = "/assets/images/event-placeholder.png",
                    Slug = "android-gelistici-gunleri",
                    Category = categories[0],
                    Topic = topics[0],
                    SubTopic = subtopics[0],
                    Organizer = user.Organizers.ElementAt(0),
                    Venue = venues[0],
                    StartDate = DateTime.Parse("2016-05-15 9:00"),
                    EndDate = DateTime.Parse("2016-05-17 15:00")
                },
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = "IOS Kod Kampı",
                    Published = true,
                    Description = "IOS Kod Kampı Açıklaması",
                    Image = "/assets/images/event-placeholder.png",
                    Slug = "ios-kod-kampi",
                    Category = categories[0],
                    Topic = topics[0],
                    SubTopic = subtopics[0],
                    Organizer = user.Organizers.ElementAt(0),
                    Venue = venues[0],
                    StartDate = DateTime.Parse("2016-06-1 9:00"),
                    EndDate = DateTime.Parse("2016-06-2 15:00"),
                    Tickets = new List<Ticket>
                    {
                        new Ticket
                        {
                            TicketId = Guid.NewGuid(),
                            Title = "Free",
                            Quantity = 100,
                            Price = 0,
                            Type = TicketType.Free
                        },
                        new Ticket
                        {
                            TicketId = Guid.NewGuid(),
                            Title = "VIP",
                            Quantity = 10,
                            Price = 15.0M,
                            Type = TicketType.Paid
                        }
                    }
                },
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = "Windows Phone Etkinliği",
                    Published = true,
                    Description = "Windows Phone Etkinliği Açıklaması",
                    Image = "/assets/images/event-placeholder.png",
                    Slug = "windows-phone-etkinligi",
                    Category = categories[0],
                    Topic = topics[0],
                    SubTopic = subtopics[0],
                    Organizer = user.Organizers.ElementAt(0),
                    Venue = venues[0],
                    StartDate = DateTime.Parse("2016-06-5 9:00"),
                    EndDate = DateTime.Parse("2016-06-8 15:00")
                },
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = "Mobil Uygulamaların Geleceği",
                    Published = true,
                    Description = "Mobil Uygulamaların Geleceği Açıklaması",
                    Image = "/assets/images/event-placeholder.png",
                    Slug = "mobil-uygulamalarin-gelecegi",
                    Category = categories[0],
                    Topic = topics[0],
                    SubTopic = subtopics[0],
                    Organizer = user.Organizers.ElementAt(0),
                    Venue = venues[0],
                    StartDate = DateTime.Parse("2016-06-5 9:00"),
                    EndDate = DateTime.Parse("2016-06-6 15:00")
                },
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = "Akıllı Telefon Çılgınlığı",
                    Published = true,
                    Description = "Akıllı Telefon Çılgınlığı Açıklaması",
                    Image = "/assets/images/event-placeholder.png",
                    Slug = "akilli-telefon-cilginligi",
                    Category = categories[0],
                    Topic = topics[0],
                    SubTopic = subtopics[0],
                    Organizer = user.Organizers.ElementAt(0),
                    Venue = venues[0],
                    StartDate = DateTime.Parse("2016-06-10 9:00"),
                    EndDate = DateTime.Parse("2016-06-12 15:00")
                },
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = "Vosvos Buluşması",
                    Published = true,
                    Description = "Vosvos Buluşması Açıklaması",
                    Image = "/assets/images/event-placeholder.png",
                    Slug = "vosvos-bulusmasi",
                    Category = categories[0],
                    Topic = topics[0],
                    SubTopic = subtopics[0],
                    Organizer = user.Organizers.ElementAt(0),
                    Venue = venues[0],
                    StartDate = DateTime.Parse("2016-06-14 9:00"),
                    EndDate = DateTime.Parse("2016-06-14 15:00")
                },
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = "Skoda Tanıtım Festivali",
                    Published = true,
                    Description = "Skoda Tanıtım Festivali Açıklaması",
                    Image = "/assets/images/event-placeholder.png",
                    Slug = "skoda-tanitim-festivali",
                    Category = categories[0],
                    Topic = topics[0],
                    SubTopic = subtopics[0],
                    Organizer = user.Organizers.ElementAt(0),
                    Venue = venues[0],
                    StartDate = DateTime.Parse("2016-07-15 9:00"),
                    EndDate = DateTime.Parse("2016-07-15 15:00")
                },
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = "Yoga Hakkında Herşey",
                    Published = true,
                    Description = "Yoga Hakkında Herşey Açıklaması",
                    Image = "/assets/images/event-placeholder.png",
                    Slug = "yoga-hakkinda-hersey",
                    Category = categories[0],
                    Topic = topics[0],
                    SubTopic = subtopics[0],
                    Organizer = user.Organizers.ElementAt(0),
                    Venue = venues[0],
                    StartDate = DateTime.Parse("2016-08-20 9:00"),
                    EndDate = DateTime.Parse("2016-08-20 15:00")
                },
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = "Ankara Genel Sağlık Eğitimi",
                    Published = true,
                    Description = "Skoda Tanıtım Festivali Açıklaması",
                    Image = "/assets/images/event-placeholder.png",
                    Slug = "ankara-saglik-egitimi",
                    Category = categories[0],
                    Topic = topics[0],
                    SubTopic = subtopics[0],
                    Organizer = user.Organizers.ElementAt(0),
                    Venue = venues[0],
                    StartDate = DateTime.Parse("2016-08-22"),
                    EndDate = DateTime.Parse("2016-08-25 15:00")
                },
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = "Heisenberg Tavla Turnuvası",
                    Published = true,
                    Description = "Eskişehir Tavla Turnuvası Açıklaması",
                    Image = "/assets/images/event-placeholder.png",
                    Slug = "heiseenberg-tavla-turnuvasi",
                    Category = categories[0],
                    Topic = topics[0],
                    SubTopic = subtopics[0],
                    Organizer = user.Organizers.ElementAt(0),
                    Venue = venues[0],
                    StartDate = DateTime.Parse("2016-08-1 9:00"),
                    EndDate = DateTime.Parse("2016-08-2 15:00")
                },
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = "Özel Bing Koleji Satranç Turnuvası",
                    Published = true,
                    Description = "Özel Bing Koleji Satranç Turnuvası Açıklaması",
                    Image = "/assets/images/event-placeholder.png",
                    Slug = "bing-koleji-satranc-turnuvasi",
                    Category = categories[0],
                    Topic = topics[0],
                    SubTopic = subtopics[0],
                    Organizer = user.Organizers.ElementAt(0),
                    Venue = venues[0],
                    StartDate = DateTime.Parse("2016-3-5 9:00"),
                    EndDate = DateTime.Parse("2016-3-5 15:00")
                },
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = "World of Warcraft Arena Sezon 3",
                    Published = true,
                    Description = "World of Warcraft Arena Sezon 3 Açıklaması",
                    Image = "/assets/images/event-placeholder.png",
                    Slug = "world-warcraft-sezon-5",
                    Category = categories[0],
                    Topic = topics[0],
                    SubTopic = subtopics[0],
                    Organizer = user.Organizers.ElementAt(0),
                    Venue = venues[0],
                    StartDate = DateTime.Parse("2016-3-8 9:00"),
                    EndDate = DateTime.Parse("2016-3-8 15:00")
                }
            };

            events.ForEach(e => context.Events.Add(e));
            context.SaveChanges();

            base.Seed(context);
        }
    }
}