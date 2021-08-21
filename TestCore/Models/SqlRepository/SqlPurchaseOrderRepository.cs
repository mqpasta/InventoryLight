using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TestCore.Models;
using TestCore.Models.IRepository;
using TestCore.Models.SqlRepository;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace TestCore.Models.SqlRepository
{
    public class SqlPurchaseOrderRepository : IPurchaseOrderRepository
    {

        #region QueryStrings
        readonly string qryInsert = "INSERT INTO PurchaseOrder(PODate,ProductId,ConvRate,RMBRate,Quantity,IsReceived, CostPrice) " +
            " Values('{0}',{1},{2},{3},{4},{5},{6});";
        readonly string qryGetPurchases = "SELECT * FROM ViewPurchaseOrders";
        readonly string qryFindOrder = "SELECT * FROM ViewPurchaseOrders WHERE PurchaseOrderId={0}";
        readonly string qryUpdate = "UPDATE PurchaseOrder " +
                                    " SET PODate = '{0}', " +
                                    " ProductId = {1}, " +
                                    " ConvRate = {2}, " +
                                    " RMBRate = {3}, " +
                                    " Quantity = {4}, " +
                                    " IsReceived = {5}, " + 
                                    " CostPrice = {6} " +
                                    " WHERE PurchaseOrderId = {7}";
        readonly string qryCountReceviedLiens = "SELECT count(1) FROM StockMovement WHERE PurchaseOrderId = {0}";
        readonly string qryDeletePO = "DELETE FROM PurchaseOrder WHERE PurchaseOrderId = {0}";
        

        #endregion

        public void Add(PurchaseOrder purchaseOrder)
        {
            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                try
                {
                    DBHelper.Execute(con, string.Format(qryInsert,
                                    purchaseOrder.PODate,
                                    purchaseOrder.ProductId,
                                    purchaseOrder.ConvRate,
                                    purchaseOrder.RMBRate,
                                    purchaseOrder.Quantity,
                                    Convert.ToInt16(purchaseOrder.IsReceived),
                                    purchaseOrder.CostPrice), trans);

                    trans.Commit();
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    throw new Exception("Unable to commit the transaction", e);
                }
                finally
                {
                    con.Close();
                }
                
            }
        }

        public void Edit(PurchaseOrder purchaseOrder)
        {
            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DBHelper.Execute(con, string.Format(qryUpdate,
                                    purchaseOrder.PODate,
                                    purchaseOrder.ProductId,
                                    purchaseOrder.ConvRate,
                                    purchaseOrder.RMBRate,
                                    purchaseOrder.Quantity,
                                    Convert.ToInt16(purchaseOrder.IsReceived),
                                    purchaseOrder.CostPrice,
                                    purchaseOrder.PurchaseOrderId));
                con.Close();
            }
        }

        public PurchaseOrder Find(long id)
        {
            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DataSet ds = DBHelper.LoadData(con,
                                    string.Format(qryFindOrder, id));
                con.Close();
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow r = ds.Tables[0].Rows[0];
                    PurchaseOrder po = LoadRow(r);
                    return po;
                }
            }

            return null;
        }

        public List<PurchaseOrder> GetOrders()
        {
            List<PurchaseOrder> orders = new List<PurchaseOrder>();

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DataSet ds = DBHelper.LoadData(con, qryGetPurchases);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        orders.Add(LoadRow(r));
                    }
                }
                con.Close();
            }
            return orders;
        }

        private PurchaseOrder LoadRow(DataRow r)
        {
            return new PurchaseOrder()
            {
                PurchaseOrderId = Convert.ToInt64(r["PurchaseOrderId"]),
                PODate = Convert.ToDateTime(r["PODate"]),
                ProductId = Convert.ToInt64(r["ProductId"]),
                ConvRate = Convert.ToDecimal(r["ConvRate"]),
                RMBRate = Convert.ToDecimal(r["RMBRate"]),
                Quantity = Convert.ToInt32(r["Quantity"]),
                IsReceived = Convert.ToBoolean(r["IsReceived"]),
                ReceivedQuantity = Convert.ToInt32(r["ReceivedQuantity"]),
                CostPrice = Convert.ToDecimal(r["CostPrice"])
                
            };
        }

        public List<PurchaseOrder> Search(DateTime? startDate, DateTime? endDate,
                                    long? locationId, long? productId, bool? isReceived,
                                    bool? isBalanceQuantity)
        {

            string qrySearch = $"select VPO.*, SM.FromLocationId as LocationId " +
                " from ViewPurchaseOrders VPO " +
            "Left Join StockMovement SM ON VPO.PurchaseOrderId = SM.PurchaseOrderId " +
            " WHERE({0} IS NULL OR SM.ToLocationId = {0}) " +
            " AND({1} IS NULL OR VPO.ProductId = {1}) " +
            " AND({2} IS NULL OR VPO.PODate >= {2}) " +
            " AND({3} IS NULL OR VPO.PODate <= {3}) " +
            " AND({4} IS NULL OR VPO.IsReceived = {4}) " +
            " AND({5} IS NULL OR (VPO.Quantity - VPO.ReceivedQuantity)>0)";

            List<PurchaseOrder> orders = new List<PurchaseOrder>();
            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                DataSet ds = DBHelper.LoadData(con,
                                string.Format(qrySearch,
                                        DBHelper.NullOrValue<long>(locationId),
                                        DBHelper.NullOrValue<long>(productId),
                                        DBHelper.NullOrValue<DateTime>(startDate),
                                        DBHelper.NullOrValue<DateTime>(endDate),
                                        DBHelper.NullOrValue<bool>(isReceived),
                                        DBHelper.NullOrValue<bool>(isBalanceQuantity))
                                        );
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        orders.Add(LoadRow(r));
                    }
                }
            }

            return orders;
        }

        public void Remove(long id)
        {
            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                int count = Convert.ToInt32(DBHelper.ExecuteScalar(con, string.Format(qryCountReceviedLiens, id)));

                if (count > 0)
                    throw new Exception("Can't delete PO with goods received.");

                DBHelper.Execute(con, string.Format(qryDeletePO, id));
                con.Close();

            }
        }
    }
}
