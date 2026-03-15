using NikolaevaLib;

namespace NikolaevaTests
{
    [TestClass]
    public class NikolaevaTests
    {
        private ApplicationContext _db;
        private Service _svc;
        private const string Prefix = "TMP_"; // Префикс для временных партнёров

        // Метод, который выполняется перед каждым тестом для инициализации контекста и сервиса
        [TestInitialize]
        public void Init()
        {
            _db = new ApplicationContext();
            _svc = new Service();
        }

        // Метод, который выполняется после каждого теста для очистки данных
        [TestCleanup]
        public void Cleanup()
        {
            var partners = _db.Partner.Where(p => p.Name.StartsWith(Prefix)).ToList();
            var partnerIds = partners.Select(p => p.Id).ToList();

            var sales = _db.Sale.Where(s => partnerIds.Contains(s.Partnerid ?? 0)).ToList();
            var discounts = _db.Discount.Where(d => partnerIds.Contains(d.Partnerid ?? 0)).ToList();

            _db.Sale.RemoveRange(sales);
            _db.Discount.RemoveRange(discounts);
            _db.Partner.RemoveRange(partners);
            _db.SaveChanges();
            _db.Dispose();
        }

        // Тест для добавления нового партнёра
        [TestMethod]
        public void AddPartner()
        {
            var p = new Partner { Name = Prefix + "Alpha", Type = "Retail", Director = "Alice", Email = "alpha@example.com", Phone = "111222333", Legaladdress = "City A", Rating = 4 };
            _svc.AddPartner(_db, p);

            var res = _db.Partner.FirstOrDefault(x => x.Name == p.Name);
            Assert.IsNotNull(res);
            Assert.AreEqual("Retail", res.Type);
        }

        // Тест для обновления данных о партнёре
        [TestMethod]
        public void UpdatePartner()
        {
            var p = new Partner { Name = Prefix + "Old", Type = "Wholesale", Director = "Bob", Email = "old@example.com", Phone = "444555666", Legaladdress = "City B", Rating = 3 };
            _db.Partner.Add(p);
            _db.SaveChanges();

            p.Name = Prefix + "New";
            p.Email = "new@example.com";
            _svc.UpdatePartner(_db, p);

            var res = _db.Partner.FirstOrDefault(x => x.Id == p.Id);
            Assert.IsNotNull(res);
            Assert.AreEqual(Prefix + "New", res.Name);
            Assert.AreEqual("new@example.com", res.Email);
        }

        // Тест для удаления партнёра
        [TestMethod]
        public void DeletePartner()
        {
            var p = new Partner { Name = Prefix + "ToRemove", Type = "Retail", Director = "Charlie", Email = "remove@example.com", Phone = "777888999", Legaladdress = "City C", Rating = 5 };
            _db.Partner.Add(p);
            _db.SaveChanges();

            _svc.DeletePartner(_db, p);
            var res = _db.Partner.FirstOrDefault(x => x.Name == p.Name);
            Assert.IsNull(res);
        }

        // Тест для обновления скидок партнёров
        [TestMethod]
        public void UpdateDiscounts()
        {
            var p = new Partner { Name = Prefix + "Beta", Type = "Retail", Director = "Dave", Email = "beta@example.com", Phone = "123321123", Legaladdress = "City D", Rating = 2 };
            _db.Partner.Add(p);
            _db.SaveChanges();

            _db.Sale.Add(new Sale { Partnerid = p.Id, Quantity = 50000, Date = DateTime.Now, ProductName = "Item X" });
            _db.SaveChanges();

            _svc.UpdateDiscounts(_db);
            var d = _db.Discount.FirstOrDefault(x => x.Partnerid == p.Id);
            Assert.IsNotNull(d);
            Assert.AreEqual(5, d.Percentage); // Проверка, что скидка обновилась на 5%
        }

        // Тест для получения партнёра по имени
        [TestMethod]
        public void GetPartner()
        {
            var p = new Partner { Name = Prefix + "Gamma", Type = "Wholesale", Director = "Eve", Email = "gamma@example.com", Phone = "987654321", Legaladdress = "City E", Rating = 1 };
            _db.Partner.Add(p);
            _db.SaveChanges();

            var res = _svc.GetPartnerByProperties(_db, p.Name);
            Assert.IsNotNull(res);
            Assert.AreEqual(p.Name, res.Name);
        }

        // Тест для загрузки списка партнёров с их скидками
        [TestMethod]
        public void LoadPartners()
        {
            var p = new Partner { Name = Prefix + "Delta", Type = "Retail", Director = "Frank", Email = "delta@example.com", Phone = "741852963", Legaladdress = "City F", Rating = 3 };
            _db.Partner.Add(p);
            _db.SaveChanges();

            _db.Sale.Add(new Sale { Partnerid = p.Id, Quantity = 45000, Date = DateTime.Now, ProductName = "Item Y" });
            _db.SaveChanges();

            _svc.UpdateDiscounts(_db);
            var res = _svc.LoadPartners(_db);
            Assert.IsTrue(res.Any(x => x.Name == p.Name)); // Проверка, что партнёр попал в список
        }

        // Тест для сохранения изменений в контексте
        [TestMethod]
        public void SaveChanges()
        {
            var p = new Partner { Name = Prefix + "Sigma", Type = "Retail", Director = "George", Email = "sigma@example.com", Phone = "555666777", Legaladdress = "City G", Rating = 5 };
            _db.Partner.Add(p);
            _svc.SaveChanges(_db);  // Этот метод просто сохраняет изменения, поэтому проверим, что данные были сохранены

            var res = _db.Partner.FirstOrDefault(x => x.Name == p.Name);
            Assert.IsNotNull(res);
            Assert.AreEqual("Retail", res.Type);
        }

        // Тест для получения всех продаж конкретного партнёра
        [TestMethod]
        public void GetSales()
        {
            var p = new Partner { Name = Prefix + "Epsilon", Type = "Wholesale", Director = "Hannah", Email = "epsilon@example.com", Phone = "333444555", Legaladdress = "City H", Rating = 4 };
            _db.Partner.Add(p);
            _db.SaveChanges();

            var sale = new Sale { Partnerid = p.Id, Quantity = 10000, Date = DateTime.Now, ProductName = "Item Z" };
            _db.Sale.Add(sale);
            _db.SaveChanges();

            var sales = _svc.GetSales(_db, p);
            Assert.IsTrue(sales.Any());
            Assert.AreEqual(10000, sales.First().Quantity);
        }

        // Тест для добавления новой продажи
        [TestMethod]
        public void AddSale()
        {
            var p = new Partner { Name = Prefix + "Zeta", Type = "Retail", Director = "Ivy", Email = "zeta@example.com", Phone = "123456789", Legaladdress = "City I", Rating = 2 };
            _db.Partner.Add(p);
            _db.SaveChanges();

            var sale = new Sale { Partnerid = p.Id, Quantity = 30000, Date = DateTime.Now, ProductName = "Item W" };
            _svc.AddSale(_db, sale);

            var res = _db.Sale.FirstOrDefault(x => x.Partnerid == p.Id && x.ProductName == "Item W");
            Assert.IsNotNull(res);
            Assert.AreEqual(30000, res.Quantity);
        }

        // Тест для обновления информации о продаже
        [TestMethod]
        public void UpdateSale()
        {
            var p = new Partner { Name = Prefix + "Eta", Type = "Wholesale", Director = "Jack", Email = "eta@example.com", Phone = "999888777", Legaladdress = "City J", Rating = 5 };
            _db.Partner.Add(p);
            _db.SaveChanges();

            var sale = new Sale { Partnerid = p.Id, Quantity = 15000, Date = DateTime.Now, ProductName = "Item V" };
            _db.Sale.Add(sale);
            _db.SaveChanges();

            sale.Quantity = 20000;
            _svc.UpdateSale(_db, sale);

            var res = _db.Sale.FirstOrDefault(x => x.Partnerid == p.Id && x.ProductName == "Item V");
            Assert.IsNotNull(res);
            Assert.AreEqual(20000, res.Quantity);
        }

        // Тест для удаления продажи
        [TestMethod]
        public void DeleteSale()
        {
            var p = new Partner { Name = Prefix + "Theta", Type = "Retail", Director = "Karen", Email = "theta@example.com", Phone = "222333444", Legaladdress = "City K", Rating = 4 };
            _db.Partner.Add(p);
            _db.SaveChanges();

            var sale = new Sale { Partnerid = p.Id, Quantity = 25000, Date = DateTime.Now, ProductName = "Item U" };
            _db.Sale.Add(sale);
            _db.SaveChanges();

            _svc.DeleteSale(_db, sale);

            var res = _db.Sale.FirstOrDefault(x => x.Partnerid == p.Id && x.ProductName == "Item U");
            Assert.IsNull(res);
        }
    }
}
