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
        public void Add(PurchaseMovement purchase)
        {
            string query = "Insert Into StockMovement(Date, ProductId," +
                                        "ToLocationId, Qty, PurchasePrice, SalePrice, StockMovementType) " +
                                "Values('{0}',{1},{2},{3},{4},{5},{6});";

            string qryStock = "IF NOT EXISTS(select 1 from StockStatus " +
                "where ProductId = {0} and LocationId={1}) " +
                "INSERT StockStatus(ProductId, LocationId) VALUES({0}, {1}); " +
                "UPDATE StockStatus SET PurchaseQty += {2} WHERE ProductId = {0} AND LocationId = {1}";

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                try
                {

                    // Insert row in StockMovement
                    DBHelper.Execute(con, string.Format(query, purchase.Date, purchase.ProductId,
                                                              purchase.ToLocationId,
                                                              purchase.Quantity,
                                                              purchase.PurchasePrice,
                                                              0, // SalePrice
                                                              Convert.ToInt32(StockMovementType.Purchase)),
                                                              trans);
                    // Update StockStatus
                    DBHelper.Execute(con, string.Format(qryStock, purchase.ProductId,
                                                                purchase.ToLocationId,
                                                                purchase.Quantity),
                                                                trans);
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

        public void Edit(PurchaseMovement purchase)
        {
            PurchaseMovement old = this.Find(purchase.StockMovementId.Value);

            string query = "Update StockMovement " +
                            " SET ToLocationId = {0} ," +
                            " ProductId = {1}, " +
                            " Qty = {2}, " +
                            " PurchasePrice = {3}, " +
                            " Date = '{4}' " +
                            " WHERE StockMovementId = {5};";

            string qryOldNullify = "Update StockStatus SET PurchaseQty -= {0} " +
                                    "WHERE ProductId = {1} AND LocationId = {2}";

            string qryStock = "IF NOT EXISTS(select 1 from StockStatus " +
                "where ProductId = {0} and LocationId={1}) " +
                "INSERT StockStatus(ProductId, LocationId) VALUES({0}, {1}); " +
                "UPDATE StockStatus SET PurchaseQty += {2} WHERE ProductId = {0} AND LocationId = {1}";

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                try
                {
                    // Update the StockMovement Line
                    DBHelper.Execute(con, string.Format(query,
                                                    purchase.ToLocationId,
                                                    purchase.ProductId,
                                                    purchase.Quantity,
                                                    purchase.PurchasePrice,
                                                    purchase.Date,
                                                    purchase.StockMovementId), trans);
                    // Nullify the previous impact (reduce old quantity)
                    DBHelper.Execute(con, string.Format(qryOldNullify,
                                                        old.Quantity,
                                                        old.ProductId,
                                                        old.ToLocationId), trans);
                    // Update Stock status for new values
                    DBHelper.Execute(con, string.Format(qryStock,
                                                        purchase.ProductId,
                                                        purchase.ToLocationId,
                                                        purchase.Quantity), trans);
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

        public PurchaseMovement Find(long id)
        {
            string query = "SELECT * from StockMovement where StockMovementId = {0};";

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DataSet ds = DBHelper.LoadData(con, string.Format(query, id));

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow r = ds.Tables[0].Rows[0];

                    PurchaseMovement movement = new PurchaseMovement()
                    {
                        StockMovementId = Convert.ToInt64(r["StockMovementId"]),
                        Date = Convert.ToDateTime(r["Date"]),
                        ProductId = Convert.ToInt64(r["ProductId"]),
                        ToLocationId = Convert.ToInt64(r["ToLocationId"]),
                        PurchasePrice = Convert.ToDecimal(r["PurchasePrice"]),
                        Quantity = Convert.ToInt32(r["Qty"]),
                        MovementType = StockMovementType.Purchase
                    };
                    con.Close();
                    return movement;
                }
                con.Close();
            }

            return null;
        }

        public IEnumerable GetPurchases()
        {
            List<PurchaseMovement> purchaseMovements = new List<PurchaseMovement>();

            string query = "select * from StockMovement where StockMovementType = {0}";
            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                DataSet ds = DBHelper.LoadData(con,
                                                string.Format(query, Convert.ToInt32(
                                                    StockMovementType.Purchase)));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        purchaseMovements.Add(new PurchaseMovement()
                        {
                            StockMovementId = Convert.ToInt64(r["StockMovementId"]),
                            Date = Convert.ToDateTime(r["Date"]),
                            ProductId = Convert.ToInt64(r["ProductId"]),
                            ToLocationId = Convert.ToInt64(r["ToLocationId"]),
                            PurchasePrice = Convert.ToDecimal(r["PurchasePrice"]),
                            Quantity = Convert.ToInt32(r["Qty"]),
                            MovementType = StockMovementType.Purchase
                        });
                    }
                }
            }

            return purchaseMovements;
        }

        public void Remove(long id)
        {
            PurchaseMovement old = this.Find(id);
            if (old == null)
                return;

            string query = "DELETE FROM StockMovement where StockMovementId = {0};";
            string qryOldNullify = "Update StockStatus SET PurchaseQty -= {0} " +
                                    "WHERE ProductId = {1} AND LocationId = {2}";

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                try
                {
                    DBHelper.Execute(con, string.Format(query, id), trans);
                    // Nullify the previous impact (reduce old quantity)
                    DBHelper.Execute(con, string.Format(qryOldNullify,
                                                        old.Quantity,
                                                        old.ProductId,
                                                        old.ToLocationId), trans);
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
    }
}
