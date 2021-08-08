using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TestCore.Models;
using TestCore.Models.IRepository;
using TestCore.Models.SqlRepository;
using System.Data;
using System.Data.SqlClient;

namespace TestCore.Models.SqlRepository
{
    public class SqlPurchaseOrderRepository : IPurchaseOrderRepository
    {

        #region QueryStrings
        readonly string qryInsert = "INSERT INTO PurchaseOrder(PODate,ProductId,ConvRate,RMBRate,Quantity,IsReceived) " +
            " Values('{0}',{1},{2},{3},{4},{5});";
        readonly string qryGetPurchases = "SELECT * FROM ViewPurchaseOrders";
        readonly string qryFindOrder = "SELECT * FROM ViewPurchaseOrders WHERE PurchaseOrderId={0}";
        readonly string qryUpdate = "UPDATE PurchaseOrder " +
                                    " SET PODate = '{0}', " +
                                    " ProductId = {1}, " +
                                    " ConvRate = {2}, " +
                                    " RMBRate = {3}, " +
                                    " Quantity = {4}, " +
                                    " IsReceived = {5}";
        readonly string qryCountReceviedLiens = "SELECT count(1) FROM StockMovement WHERE PurchaseOrderId = {0}";
        readonly string qryDeletePO = "DELETE FROM PurchaseOrder WHERE PurchaseOrderId = {0}";
        #endregion

        public void Add(PurchaseOrder purchaseOrder)
        {
            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DBHelper.Execute(con, string.Format(qryInsert,
                                    purchaseOrder.PODate,
                                    purchaseOrder.ProductId,
                                    purchaseOrder.ConvRate,
                                    purchaseOrder.RMBRate,
                                    purchaseOrder.Quantity,
                                    Convert.ToInt16(purchaseOrder.IsReceived)));
                con.Close();
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
                                    Convert.ToInt16(purchaseOrder.IsReceived)));
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
                    PurchaseOrder po = new PurchaseOrder()
                    {
                        PurchaseOrderId = Convert.ToInt64(r["PurchaseOrderId"]),
                        PODate = Convert.ToDateTime(r["PODate"]),
                        ProductId = Convert.ToInt64(r["ProductId"]),
                        ConvRate = Convert.ToDecimal(r["ConvRate"]),
                        RMBRate = Convert.ToDecimal(r["RMBRate"]),
                        Quantity = Convert.ToInt32(r["Quantity"]),
                        IsReceived = Convert.ToBoolean(r["IsReceived"]),
                        ReceivedQuantity = Convert.ToInt32(r["ReceivedQuantity"])
                    };

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
                        orders.Add(new PurchaseOrder()
                        {
                            PurchaseOrderId = Convert.ToInt64(r["PurchaseOrderId"]),
                            PODate = Convert.ToDateTime(r["PODate"]),
                            ProductId = Convert.ToInt64(r["ProductId"]),
                            ConvRate = Convert.ToDecimal(r["ConvRate"]),
                            RMBRate = Convert.ToDecimal(r["RMBRate"]),
                            Quantity = Convert.ToInt32(r["Quantity"]),
                            IsReceived = Convert.ToBoolean(r["IsReceived"]),
                            ReceivedQuantity = Convert.ToInt32(r["ReceivedQuantity"])
                        });
                    }
                }
                con.Close();
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

        public int TotalReceived(long id)
        {
            throw new NotImplementedException();
        }
    }
}
