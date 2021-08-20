using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TestCore.Models.IRepository;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace TestCore.Models.SqlRepository
{
    public class SqlStockStatusRepository : IStockStatusRepository
    {
        public List<StockStatus> GetAllStatus(bool isSummarize = false)
        {
            List<StockStatus> stockStatuses = new List<StockStatus>();

            string query = "Select * from StockStatusView";



            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DataSet ds = DBHelper.LoadData(con, query);

                if (ds.Tables.Count > 0)
                {
                    FillItems(stockStatuses, ds);
                }
                con.Close();
            }

            return stockStatuses;
        }

        private static void FillItems(List<StockStatus> stockStatuses, DataSet ds)
        {
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                stockStatuses.Add(new StockStatus()
                {
                    ProductName = Convert.ToString(r["ProductName"]),
                    LocationName = Convert.ToString(r["LocationName"]),
                    InQuantity = Convert.ToInt32(r["InQuantity"]),
                    OutQuantity = Convert.ToInt32(r["OutQuantity"]),
                    BalanceQuantity = Convert.ToInt32(r["BalanceQuantity"]),
                    AvgPrice = Convert.ToDecimal(r["AvgPrice"])
                });
            }
        }

        public List<StockStatus> GetFor(long locationId, long productId, bool isSummarize = false)
        {
            List<StockStatus> stockStatuses = new List<StockStatus>();
            string query = "Select * from StockStatusView " +
                            "WHERE LocationId = {0} AND ProductId = {1}";

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DataSet ds = DBHelper.LoadData(con,
                                string.Format(query, locationId, productId));

                if (ds.Tables.Count > 0)
                {
                    FillItems(stockStatuses, ds);
                }
                con.Close();
            }

            return stockStatuses;
        }

        public List<StockStatus> GetForLocation(long locationId, bool isSummarize = false)
        {
            List<StockStatus> stockStatuses = new List<StockStatus>();
            string query = "Select * from StockStatusView " +
                            "WHERE LocationId = {0}";

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DataSet ds = DBHelper.LoadData(con, string.Format(query, locationId));

                if (ds.Tables.Count > 0)
                {
                    FillItems(stockStatuses, ds);
                }
                con.Close();
            }

            return stockStatuses;
        }

        public List<StockStatus> GetForProduct(long productId, bool isSummarize = false)
        {
            List<StockStatus> stockStatuses = new List<StockStatus>();
            string query = "Select * from StockStatusView " +
                            "WHERE ProductId = {0}";

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DataSet ds = DBHelper.LoadData(con, string.Format(query, productId));

                if (ds.Tables.Count > 0)
                {
                    FillItems(stockStatuses, ds);
                }
                con.Close();
            }

            return stockStatuses;
        }

        public List<StockStatus> GetWearhouseStock()
        {
            List<StockStatus> statuses = new List<StockStatus>();
            string query = "SELECT * from ViewWearhouseStock";

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DataSet ds = DBHelper.LoadData(con, query);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        statuses.Add(new StockStatus()
                        {
                            ProductName = Convert.ToString(r["ProductName"]),
                            BalanceQuantity = Convert.ToInt32(r["BalanceQuantity"])
                        });
                    }
                }
            }

            return statuses;
        }

        public DataTable Search(DateTime? startDate, DateTime? endDate, long? locationId,
                                    long? productId, StockMovementType? type)
        {
            string qrySearch = "with CTE_Stock AS(" +
                "Select Date, FromLocationId, ToLocationId, ProductId, Qty as InQty, 0 AS OutQty, 'Opening' AS Detail, StockMovementType " +
                " ,PurchaseOrderId from StockMovement WHERE StockMovementType = 2 AND ({0} " +
                " IS NULL OR ToLocationId = {0}) AND ({1} IS NULL OR ProductId = {1}) " +
                " UNION ALL " +
                " Select Date, FromLocationId, ToLocationId, ProductId, Qty as InQty, 0 AS OutQty, 'In' AS Detail, StockMovementType " +
                " ,PurchaseOrderId from StockMovement WHERE StockMovementType IN (0,1) AND ({0} IS NULL OR ToLocationId = {0}) " +
                " AND({1} IS NULL OR ProductId = {1}) " +
                " UNION ALL " +
                " Select Date, FromLocationId, ToLocationId, ProductId, 0 as InQty, Qty AS OutQty, 'Out' AS Detail, StockMovementType " +
                " ,PurchaseOrderId from StockMovement " +
                " WHERE StockMovementType IN (0,1) AND ({0} IS NULL OR FromLocationId = {0}) " +
                " AND ({1} IS NULL OR ProductId = {1})) " +
                " Select S.* , L1.LocationName as FromLocationName, L2.LocationName as ToLocationName, P.ProductName from CTE_Stock S " +
                " Left Join Location L1 ON S.FromLocationId = L1.LocationId " +
                " Left Join Location L2 ON S.ToLocationId = L2.LocationId " +
                " Inner Join Product P ON S.ProductId = P.ProductId " +
                " WHERE({2} IS NULL OR S.StockMovementType IN ({3})) " +
                " AND ({4} IS NULL OR S.Date >= {4} ) " +
                " AND ({5} IS NULL OR S.Date <= {5} )";

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                string stockmvtVal = DBHelper.NullOrValue<StockMovementType>(type);

                DataSet ds = DBHelper.LoadData(con, string.Format(qrySearch,
                                                        DBHelper.NullOrValue<long>(locationId),
                                                        DBHelper.NullOrValue<long>(productId),
                                                        (stockmvtVal == "NULL") ? "NULL" : "NOT NULL",
                                                        stockmvtVal,
                                                        DBHelper.NullOrValue<DateTime>(startDate),
                                                        DBHelper.NullOrValue<DateTime>(endDate)
                                                        ));

                return ds.Tables[0];
            }

        }
    }
}
