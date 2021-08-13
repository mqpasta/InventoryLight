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
    public class SqlProductRepository : IProductRepository
    {
        public void Add(Product p)
        {
            string query = "Insert Into Product(ProductCode, ProductName, PurchasePrice, SalePrice)" +
                " Values({0},'{1}',{2},{3});";
            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DBHelper.Execute(con, string.Format(query,
                                                    p.ProductCode,
                                                    p.ProductName,
                                                    p.PurchasePrice,
                                                    p.SalePrice));
                con.Close();
            }
        }

        public void Edit(Product p)
        {
            string query = "Update Product " +
                            "SET ProductCode = {0}, " +
                            "ProductName = '{1}', " +
                            "PurchasePrice = {2}, " +
                            "SalePrice = {3} " +
                            " WHERE ProductId = {4};";

            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DBHelper.Execute(con, string.Format(query,
                                                    p.ProductCode,
                                                    p.ProductName,
                                                    p.PurchasePrice,
                                                    p.SalePrice,
                                                    p.ProductId));
                con.Close();
            }
        }

        public Product Find(long id)
        {
            Product p;
            string query = "SELECT * FROM Product WHERE ProductId = {0}";
            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DataSet ds = DBHelper.LoadData(con, string.Format(query, id));

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow r = ds.Tables[0].Rows[0];
                    p = new Product()
                    {
                        ProductId = Convert.ToInt64(r["ProductId"]),
                        ProductCode = Convert.ToInt16(r["ProductCode"]),
                        ProductName = Convert.ToString(r["ProductName"]),
                        PurchasePrice = Convert.ToDecimal(r["PurchasePrice"]),
                        SalePrice = Convert.ToDecimal(r["SalePrice"])
                    };

                    return p;
                }

                con.Close();
            }

            return null;
        }

        public IEnumerable GetProducts()
        {
            List<Product> products = new List<Product>();
            string query = "SELECT * FROM Product";
            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DataSet ds = DBHelper.LoadData(con, query);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        products.Add(new Product()
                        {
                            ProductId = Convert.ToInt64(r["ProductId"]),
                            ProductCode = Convert.ToInt16(r["ProductCode"]),
                            ProductName = Convert.ToString(r["ProductName"]),
                            PurchasePrice = Convert.ToDecimal(r["PurchasePrice"]),
                            SalePrice = Convert.ToDecimal(r["SalePrice"])
                        });
                    }
                }
                con.Close();
            }

            return products;
        }

        public void Remove(long id)
        {
            string query = "DELETE FROM Product WHERE ProductId = {0}";
            using (SqlConnection con = new SqlConnection(DBHelper.ConnectionString))
            {
                con.Open();
                DBHelper.Execute(con, string.Format(query, id));
                con.Close();
            }
        }
    }
}
