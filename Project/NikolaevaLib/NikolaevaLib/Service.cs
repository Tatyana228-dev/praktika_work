using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NikolaevaLib
{
    public class Service
    {
        // Метод для обновления скидок у партнёров на основе их общих продаж
        public void UpdateDiscounts(ApplicationContext _context)
        {
            var partners = _context.Partner
                .Include(p => p.Sale)
                .ToList();
            if (partners != null)
            {
                foreach (var partner in partners)
                {
                    var totalSales = partner.Sale?.Sum(s => s.Quantity) ?? 0;

                    double discountPercentage;

                    if (totalSales <= 10000)
                    {
                        discountPercentage = 0;
                    }
                    else if (totalSales <= 50000)
                    {
                        discountPercentage = 5;
                    }
                    else if (totalSales <= 300000)
                    {
                        discountPercentage = 10;
                    }
                    else
                    {
                        discountPercentage = 15;
                    }

                    var discount = _context.Discount.FirstOrDefault(d => d.Partnerid == partner.Id);
                    if (discount == null)
                    {
                        _context.Discount.Add(new Discount
                        {
                            Partnerid = partner.Id,
                            Percentage = discountPercentage
                        });
                    }
                    else
                    {
                        discount.Percentage = discountPercentage;
                    }
                }
                _context.SaveChanges();
            }

        }

        // Метод для получения партнёра по имени
        public Partner GetPartnerByProperties(ApplicationContext _context, string name)
        {
            return _context.Partner
                .Include(p => p.Sale)
                .FirstOrDefault(p =>
                    (p.Name == name)

                );
        }

        // Метод для сохранения изменений в контексте
        public void SaveChanges(ApplicationContext _context)
        {
            if (_context != null)
            {
                _context.SaveChanges();
            }
        }

        // Метод для загрузки списка партнёров с их скидками
        public List<PartnerView> LoadPartners(ApplicationContext _context)
        {
            return _context.Partner
                .Include(p => p.Discount)
                .Select(p => new PartnerView
                {
                    Type = p.Type,
                    Name = p.Name,
                    Director = p.Director,
                    Phone = p.Phone,
                    Rating = p.Rating,
                    Discount = p.Discount.First().Percentage
                })
                .ToList();
        }

        // Метод для добавления нового партнёра
        public void AddPartner(ApplicationContext _context, Partner partner)
        {
            if (partner != null)
            {
                _context.Partner.Add(partner);
                _context.SaveChanges();
            }
        }

        // Метод для обновления информации о партнёре
        public void UpdatePartner(ApplicationContext _context, Partner partner)
        {
            if (partner != null)
            {
                _context.Partner.Update(partner);
                _context.SaveChanges();
            }
        }

        // Метод для получения всех продаж конкретного партнёра
        public IEnumerable<Sale> GetSales(ApplicationContext _context, Partner partner)
        {
            if (partner == null)
                return Enumerable.Empty<Sale>();

            return _context.Sale
                            .Where(s => s.Partnerid == partner.Id)
                            .ToList();
        }

        // Метод для удаления партнёра и его продаж
        public void DeletePartner(ApplicationContext _context, Partner partner)
        {
            if (partner != null)
            {
                var sales = _context.Sale
                                    .Where(s => s.Partnerid == partner.Id)
                                    .ToList();
                if (sales != null)
                {
                    _context.Sale.RemoveRange(sales);
                }
                _context.Partner.Remove(partner);
                _context.SaveChanges();
            }
        }

        // Метод для получения всех партнёров
        public IEnumerable<Partner> GetPartners(ApplicationContext _context)
        {
            return _context.Partner.Include(p => p.Discount).ToList();
        }

        // Метод для добавления новой продажи
        public void AddSale(ApplicationContext _context, Sale sale)
        {
            if (sale != null)
            {
                _context.Sale.Add(sale);
                _context.SaveChanges();
            }
        }

        // Метод для обновления информации о продаже
        public void UpdateSale(ApplicationContext _context, Sale sale)
        {
            if (sale != null)
            {
                _context.Sale.Update(sale);
                _context.SaveChanges();
            }
        }

        // Метод для удаления продажи
        public void DeleteSale(ApplicationContext _context, Sale sale)
        {
            if (sale != null)
            {
                _context.Sale.Remove(sale);
                _context.SaveChanges();
            }
        }
    }
}
