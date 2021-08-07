using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestCore.Models.IRepository
{
    public interface ITransferRepository
    {
        void Add(SaleMovement transer);
        void Edit(SaleMovement transfer);
        void Remove(long id);
        SaleMovement Find(long id);
        IEnumerable GetTransfers();
    }
}
