using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace TestCore.Models.IRepository
{
    public interface IStockStatusRepository
    {
        List<StockStatus> GetAllStatus(bool isSummarize = false);
        List<StockStatus> GetForLocation(long locationId, bool isSummarize = false);
        List<StockStatus> GetForProduct(long productId, bool isSummarize = false);
        List<StockStatus> GetFor(long locationId, long productId, bool isSummarize = false);
        List<StockStatus> GetWearhouseStock();

        DataTable Search(DateTime? startDate, DateTime? endDate, long? locationId,
                            long? productId, StockMovementType? type);

        int GetBalanceQty(long locationId, long productId, DateTime startDate);
    }

}
