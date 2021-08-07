using System;
using System.Collections;
using System.Collections.Generic;

namespace TestCore.Models.IRepository
{
    public interface IProductRepository
    {
        void Add(Product p);
        void Edit(Product p);
        void Remove(long id);
        Product Find(long id);
        IEnumerable GetProducts();
    }
}
