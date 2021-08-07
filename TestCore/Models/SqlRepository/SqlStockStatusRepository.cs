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
                    BalanceQuantity = Convert.ToInt32(r["BalanceQuantity"])
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

        public List<StockStatus> GetForLocation(long locationId, bool isSummarize=false)
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

        public List<StockStatus> GetForProduct(long productId, bool isSummarize=false)
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
                if(ds.Tables.Count>0)
                {
                    foreach(DataRow r in ds.Tables[0].Rows)
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
    }
}
