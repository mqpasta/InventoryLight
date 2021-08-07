using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TestCore.Models;
using TestCore.Models.IRepository;

namespace TestCore.Models.FakeRepository
{
    public class FakeProductRepository : IProductRepository
    {
        public static List<Product> _products = new List<Product>();

        public FakeProductRepository()
        {
            //var p1 = new Product()
            //{
            //    ProductId = 1,
            //    ProductName = "Product 1",
            //    ProductCode = 101,
            //    PurchasePrice = 10.1M,
            //    SalePrice = 12.3M
            //};
            //var p2 = new Product()
            //{
            //    ProductId = 2,
            //    ProductName = "Product 2",
            //    ProductCode = 103,
            //    PurchasePrice = 1.1M,
            //    SalePrice = 12.0M
            //};

            //_products = new List<Product>
            //{
            //    p1,
            //    p2
            //};

        }

        public void Add(Product p)
        {
            Product newProduct = new Product()
            {
                ProductId = new Random().Next(100, 1000),
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                PurchasePrice = p.PurchasePrice,
                SalePrice = p.SalePrice
            };
            
            _products.Add(newProduct);
        }

        public void Edit(Product p)
        {
            Product found = _products.Find(x => x.ProductId == p.ProductId);
            if (found != null)
            {
                found.ProductCode = p.ProductCode;
                found.ProductName = p.ProductName;
                found.PurchasePrice = p.PurchasePrice;
                found.SalePrice = p.SalePrice;
            }

        }

        public Product Find(long id)
        {
            return _products.Find(x => x.ProductId == id);
        }

        public IEnumerable GetProducts()
        {
            return _products;
        }

        public void Remove(long id)
        {
            _products.Remove(_products.Find(x => x.ProductId == id));
        }
    }
}
