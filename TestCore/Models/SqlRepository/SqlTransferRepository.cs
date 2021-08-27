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
    public class SqlTransferRepository : ITransferRepository
    {
        public void Add(SaleMovement transer)
        {
            string query = "Insert Into StockMovement(Date, ProductId, FromLocationId," +
                                        "ToLocationId, Qty, PurchasePrice, SalePrice, StockMovementType) " +
                                "Values('{0}',{1},{2},{3},{4},{5},{6},{7});";

            string qryStockUp = "IF NOT EXISTS(select 1 from StockStatus " +
                "where ProductId = {0} and LocationId={1}) " +
                "INSERT StockStatus(ProductId, LocationId) VALUES({0}, {1}); " +
                "UPDATE StockStatus SET PurchaseQty += {2} WHERE ProductId = {0} AND LocationId = {1}";

            string qryStockDown = "IF NOT EXISTS(select 1 from StockStatus " +
                "where ProductId = {0} and LocationId={1}) " +
                "INSERT StockStatus(ProductId, LocationId) VALUES({0}, {1}); " +
                "UPDATE StockStatus SET SaleQty += {2} WHERE ProductId = {0} AND LocationId = {1}";

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                try
                {

                    // Insert row in StockMovement
                    DBHelper.Execute(con, string.Format(query, transer.Date.ToString("s"),
                                                              transer.ProductId,
                                                              transer.FromLocationId,
                                                              transer.ToLocationId,
                                                              transer.Quantity,
                                                              0, // PurchasePrice
                                                              0, // SalePrice
                                                              Convert.ToInt32(StockMovementType.Transfer)),
                                                              trans);
                    // Update StockStatus from Target Location (add in purchase)
                    DBHelper.Execute(con, string.Format(qryStockUp, transer.ProductId,
                                                                transer.ToLocationId,
                                                                transer.Quantity),
                                                                trans);
                    // Update StockStatus from Source Location (add in Sales)
                    DBHelper.Execute(con, string.Format(qryStockDown, transer.ProductId,
                                            transer.FromLocationId,
                                            transer.Quantity),
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

        public void Edit(SaleMovement transfer)
        {
            SaleMovement old = this.Find(transfer.StockMovementId.Value);

            string query = "Update StockMovement " +
                            " SET ToLocationId = {0} ," +
                            " FromLocationId = {1}, " +
                            " ProductId = {2}, " +
                            " Qty = {3}, " +
                            " Date = '{4}' " +
                            " WHERE StockMovementId = {5};";

            // restore to the position before this transaction
            string qryOldNullifyTarget = "Update StockStatus SET PurchaseQty -= {0} " +
                                    "WHERE ProductId = {1} AND LocationId = {2}";

            string qryOldNullifySource = "Update StockStatus SET SaleQty -= {0} " +
                                    "WHERE ProductId = {1} AND LocationId = {2}";

            string qryStockUp = "IF NOT EXISTS(select 1 from StockStatus " +
                "where ProductId = {0} and LocationId={1}) " +
                "INSERT StockStatus(ProductId, LocationId) VALUES({0}, {1}); " +
                "UPDATE StockStatus SET PurchaseQty += {2} WHERE ProductId = {0} AND LocationId = {1}";

            string qryStockDown = "IF NOT EXISTS(select 1 from StockStatus " +
                "where ProductId = {0} and LocationId={1}) " +
                "INSERT StockStatus(ProductId, LocationId) VALUES({0}, {1}); " +
                "UPDATE StockStatus SET SaleQty += {2} WHERE ProductId = {0} AND LocationId = {1}";

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                try
                {
                    // Update the StockMovement Line
                    DBHelper.Execute(con, string.Format(query,
                                                    transfer.ToLocationId,
                                                    transfer.FromLocationId,
                                                    transfer.ProductId,
                                                    transfer.Quantity,
                                                    transfer.Date.ToString("s"),
                                                    transfer.StockMovementId), trans);
                    // Nullify Source: the previous impact (reduce old sale quantity)
                    DBHelper.Execute(con, string.Format(qryOldNullifySource,
                                                        old.Quantity,
                                                        old.ProductId,
                                                        old.FromLocationId), trans);
                    // Nullify Target: the previous impact (reduce old purchase quantity)
                    DBHelper.Execute(con, string.Format(qryOldNullifyTarget,
                                                        old.Quantity,
                                                        old.ProductId,
                                                        old.ToLocationId), trans);
                    // Update Stock status for new values for target
                    DBHelper.Execute(con, string.Format(qryStockUp,
                                                        transfer.ProductId,
                                                        transfer.ToLocationId,
                                                        transfer.Quantity), trans);
                    // Update Stock status for new values for sournce
                    DBHelper.Execute(con, string.Format(qryStockDown,
                                                        transfer.ProductId,
                                                        transfer.FromLocationId,
                                                        transfer.Quantity), trans);
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

        public SaleMovement Find(long id)
        {
            string query = "SELECT * FROM StockMovement where StockMovementId = {0}";

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DataSet ds = DBHelper.LoadData(con, string.Format(query, id));

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow r = ds.Tables[0].Rows[0];

                    SaleMovement movement = new SaleMovement()
                    {
                        Date = Convert.ToDateTime(r["Date"]),
                        StockMovementId = Convert.ToInt64(r["StockMovementId"]),
                        FromLocationId = Convert.ToInt64(r["FromLocationId"]),
                        ToLocationId = Convert.ToInt64(r["ToLocationId"]),
                        ProductId = Convert.ToInt64(r["ProductId"]),
                        Quantity = Convert.ToInt32(r["Qty"]),
                        MovementType = StockMovementType.Transfer
                    };
                    con.Close();
                    return movement;
                }
                con.Close();

                return null;
            }
        }

        public IEnumerable GetTransfers()
        {
            List<SaleMovement> movements = new List<SaleMovement>();
            string query = "SELECT * FROM StockMovement where StockMovementType = {0} ORDER BY DATE";

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DataSet ds = DBHelper.LoadData(con,
                                string.Format(query, Convert.ToInt32(StockMovementType.Transfer)));

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        movements.Add(new SaleMovement()
                        {
                            Date = Convert.ToDateTime(r["Date"]),
                            StockMovementId = Convert.ToInt64(r["StockMovementId"]),
                            FromLocationId = Convert.ToInt64(r["FromLocationId"]),
                            ToLocationId = Convert.ToInt64(r["ToLocationId"]),
                            ProductId = Convert.ToInt64(r["ProductId"]),
                            Quantity = Convert.ToInt32(r["Qty"]),
                            MovementType = StockMovementType.Transfer
                        });
                    }
                }
                con.Close();
            }
            return movements;
        }

        public void Remove(long id)
        {
            SaleMovement old = this.Find(id);

            if (old == null)
                return;

            string query = "DELETE FROM StockMovement where StockMovementId = {0};";

            // restore to the position before this transaction
            string qryOldNullifyTarget = "Update StockStatus SET PurchaseQty -= {0} " +
                                    "WHERE ProductId = {1} AND LocationId = {2}";

            string qryOldNullifySource = "Update StockStatus SET SaleQty -= {0} " +
                                    "WHERE ProductId = {1} AND LocationId = {2}";

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                try
                {
                    DBHelper.Execute(con, string.Format(query, id), trans);
                    // Nullify Source: the previous impact (reduce old sale quantity)
                    DBHelper.Execute(con, string.Format(qryOldNullifySource,
                                                        old.Quantity,
                                                        old.ProductId,
                                                        old.FromLocationId), trans);
                    // Nullify Target: the previous impact (reduce old purchase quantity)
                    DBHelper.Execute(con, string.Format(qryOldNullifyTarget,
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
