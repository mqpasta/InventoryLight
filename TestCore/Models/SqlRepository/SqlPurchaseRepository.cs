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
    public class SqlPurchaseRepository : IPurchaseRepository
    {
        string query = "Insert Into StockMovement(Date, ProductId," +
                                       "ToLocationId, Qty, PurchasePrice, SalePrice, StockMovementType," +
                                       "PurchaseOrderId) " +
                               "Values('{0}',{1},{2},{3},{4},{5},{6},{7});";

        string qryStock = "IF NOT EXISTS(select 1 from StockStatus " +
            "where ProductId = {0} and LocationId={1}) " +
            "INSERT StockStatus(ProductId, LocationId) VALUES({0}, {1}); " +
            "UPDATE StockStatus SET PurchaseQty += {2} WHERE ProductId = {0} AND LocationId = {1}";
        string qryCurrAvgPrice = "SELECT AvgPrice FROM Product where ProductId = {0}";
        string qryCurrStck = "SELECT sum(ISNULL(purchaseqty,0)) FROM StockStatus WHERE ProductId = {0}";
        string qryUpdateAvgPrice = "Update Product SET AvgPrice = {0} WHERE ProductId = {1}";

        public void Add(PurchaseMovement purchase)
        {
            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                try
                {
                    InsertPurchase(purchase, con, trans);
                    trans.Commit();
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    throw new Exception("Unable to commmit the transaction.", e);
                }
                finally
                {
                    con.Close();
                }

            }
        }

        private void InsertPurchase(PurchaseMovement purchase, SqlConnection con, SqlTransaction trans)
        {
            decimal curAvgPrice;
            int currStock;

            GetCurrentStockAndAvgPrice(purchase, con, trans, out curAvgPrice, out currStock);

            int newQty = currStock + purchase.Quantity;
            decimal newVal = (curAvgPrice * currStock) + (purchase.Quantity * purchase.PurchasePrice);
            decimal newAvgPrice = 0.0M;
            if (newQty > 0)
                newAvgPrice = newVal / newQty;


            object orderId = "null";
            if (purchase.PurchaseOrderId != null)
                orderId = purchase.PurchaseOrderId.Value;


            // Insert row in StockMovement
            DBHelper.Execute(con, string.Format(query, purchase.Date.ToString("s"),
                                                       purchase.ProductId,
                                                      purchase.ToLocationId,
                                                      purchase.Quantity,
                                                      purchase.PurchasePrice,
                                                      0, // SalePrice
                                                      Convert.ToInt32(StockMovementType.Purchase),
                                                      orderId),
                                                      trans);
            // Update StockStatus
            DBHelper.Execute(con, string.Format(qryStock, purchase.ProductId,
                                                        purchase.ToLocationId,
                                                        purchase.Quantity),
                                                        trans);

            DBHelper.Execute(con, string.Format(qryUpdateAvgPrice, newAvgPrice,
                                        purchase.ProductId),
                                        trans);
        }

        private void GetCurrentStockAndAvgPrice(PurchaseMovement purchase, SqlConnection con,
                                    SqlTransaction trans,
                                    out decimal curAvgPrice, out int currStock)
        {
            // calculate new average price
            curAvgPrice = Convert.ToDecimal(DBHelper.ExecuteScalar(con,
                                    string.Format(qryCurrAvgPrice, purchase.ProductId),
                                    trans));
            var dbCurrStock = DBHelper.ExecuteScalar(con,
                            string.Format(qryCurrStck, purchase.ProductId),
                            trans);
            if (DBNull.Value.Equals(dbCurrStock))
                currStock = 0;
            else
                currStock = Convert.ToInt32(dbCurrStock);
        }

        public void Edit(PurchaseMovement purchase)
        {
            PurchaseMovement old = this.Find(purchase.StockMovementId.Value);

            // if no change then do not update
            if (PurchaseMovement.Equals(purchase, old))
                return;

            //string query = "Update StockMovement " +
            //                " SET ToLocationId = {0} ," +
            //                " ProductId = {1}, " +
            //                " Qty = {2}, " +
            //                " PurchasePrice = {3}, " +
            //                " Date = '{4}' " +
            //                " WHERE StockMovementId = {5};";

            ////string qryOldNullify = "Update StockStatus SET PurchaseQty -= {0} " +
            ////                        "WHERE ProductId = {1} AND LocationId = {2}";

            //string qryStock = "IF NOT EXISTS(select 1 from StockStatus " +
            //    "where ProductId = {0} and LocationId={1}) " +
            //    "INSERT StockStatus(ProductId, LocationId) VALUES({0}, {1}); " +
            //    "UPDATE StockStatus SET PurchaseQty += {2} WHERE ProductId = {0} AND LocationId = {1}";

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                try
                {
                    // Remove previous transaction and nullify all impacts
                    RemovePurchase(old, con, trans);
                    // Insert new entry
                    InsertPurchase(purchase, con, trans);

                    trans.Commit();
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    throw new Exception("Unable to commit.", e);
                }
                con.Close();
            }
        }

        private decimal GetUpdatedAvgPrice(PurchaseMovement purchase, SqlConnection con, SqlTransaction trans)
        {
            // Calculate Avg Price without this transaction
            // S' = curent stock , Avg' = current Avg , v/q = value/qty of this trans
            // oldAvg = ((S' * Avg') - v) / (S' - q)
            decimal curAvgPrice; // Avg'
            int currStock; // S'
            GetCurrentStockAndAvgPrice(purchase, con, trans, out curAvgPrice, out currStock);
            decimal v = purchase.Quantity * purchase.PurchasePrice;
            int q = purchase.Quantity;
            int newStock = currStock - q;
            decimal newAvgPrice = 0.0M;
            if (newStock > 0)
                newAvgPrice = ((currStock * curAvgPrice) - v) / (currStock - q);

            return newAvgPrice;
        }

        public PurchaseMovement Find(long id)
        {
            string query = "SELECT * from StockMovement where StockMovementId = {0};";

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DataSet ds = DBHelper.LoadData(con, string.Format(query, id));
                con.Close();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow r = ds.Tables[0].Rows[0];
                    PurchaseMovement movement = LoadRow(r);

                    return movement;
                }
            }

            return null;
        }

        public IEnumerable GetPurchases(long purchaseOrderId)
        {
            string qryReceviedLiens = "SELECT * FROM StockMovement WHERE PurchaseOrderId = {0}";
            List<PurchaseMovement> movements = new List<PurchaseMovement>();

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DataSet ds = DBHelper.LoadData(con, string.Format(qryReceviedLiens, purchaseOrderId));
                con.Close();

                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        movements.Add(LoadRow(r));
                    }
                }
            }
            return movements;
        }

        public IEnumerable GetPurchases()
        {
            List<PurchaseMovement> purchaseMovements = new List<PurchaseMovement>();

            string query = "select * from StockMovement where StockMovementType = {0} and PurchaseOrderId IS NULL";
            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                DataSet ds = DBHelper.LoadData(con,
                                                string.Format(query, Convert.ToInt32(
                                                    StockMovementType.Purchase)));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        purchaseMovements.Add(LoadRow(r));
                    }
                }
            }

            return purchaseMovements;
        }

        private PurchaseMovement LoadRow(DataRow r)
        {
            long? nullPurchaseOrderId = null;

            return new PurchaseMovement()
            {
                StockMovementId = Convert.ToInt64(r["StockMovementId"]),
                Date = Convert.ToDateTime(r["Date"]),
                ProductId = Convert.ToInt64(r["ProductId"]),
                ToLocationId = Convert.ToInt64(r["ToLocationId"]),
                PurchasePrice = Convert.ToDecimal(r["PurchasePrice"]),
                Quantity = Convert.ToInt32(r["Qty"]),
                PurchaseOrderId = Convert.IsDBNull(r["PurchaseOrderId"]) ?
                                        nullPurchaseOrderId : Convert.ToInt64(r["PurchaseOrderId"]),
                MovementType = StockMovementType.Purchase
            };
        }

        public void Remove(long id)
        {
            PurchaseMovement old = this.Find(id);
            if (old == null)
                return;

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                try
                {
                    RemovePurchase(old, con, trans);
                    trans.Commit();
                }
                catch
                {

                }
                finally
                {
                    con.Close();
                }


            }
        }

        private void RemovePurchase(PurchaseMovement toRemove, SqlConnection con, SqlTransaction trans)
        {
            string query = "DELETE FROM StockMovement where StockMovementId = {0};";
            string qryOldNullify = "Update StockStatus SET PurchaseQty -= {0} " +
                                    "WHERE ProductId = {1} AND LocationId = {2}";
            decimal newAvgPrice = GetUpdatedAvgPrice(toRemove, con, trans);

            DBHelper.Execute(con, string.Format(query,
                                            toRemove.StockMovementId),
                                            trans);

            // Nullify the previous impact (reduce old quantity)
            DBHelper.Execute(con, string.Format(qryOldNullify,
                                                toRemove.Quantity,
                                                toRemove.ProductId,
                                                toRemove.ToLocationId), trans);

            // nullify the impact on Avg. Price
            DBHelper.Execute(con, string.Format(qryUpdateAvgPrice,
                                                    newAvgPrice,
                                                    toRemove.ProductId), trans);
        }
    }
}
