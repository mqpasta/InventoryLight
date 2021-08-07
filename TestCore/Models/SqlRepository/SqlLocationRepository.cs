using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TestCore.Models.IRepository;
using System.Data;
using System.Data.SqlClient;

namespace TestCore.Models.SqlRepository
{
    public class SqlLocationRepository : ILocationRepository
    {
        public void Add(Location l)
        {
            using (SqlConnection _conn = new SqlConnection(DBHelper.ConnectionString))
            {
                _conn.Open();
                string query = string.Format("Insert Into Location(LocationCode, LocationName) " +
                    " values('{0}','{1}');", l.LocationCode, l.LocationName);

                DBHelper.Execute(_conn, query);
                _conn.Close();
            }
        }

        public void Edit(Location l)
        {
            string query = string.Format("UPDATE Location SET LocationCode = {0}, LocationName = '{1}' " +
                " WHERE LocationId = {2}", l.LocationCode, l.LocationName, l.LocationId);
            using (SqlConnection _con = new SqlConnection(DBHelper.ConnectionString))
            {
                _con.Open();
                DBHelper.Execute(_con, query);
                _con.Close();
            }
        }

        public Location Find(long id)
        {
            string query = string.Format("SELECT * from Location WHERE LocationId={0};", id);
            using (SqlConnection _con = new SqlConnection(DBHelper.ConnectionString))
            {
                _con.Open();
                DataSet ds = DBHelper.LoadData(_con, query);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Location found = new Location()
                    {
                        LocationId = Convert.ToInt64(ds.Tables[0].Rows[0]["LocationId"]),
                        LocationCode = Convert.ToInt16(ds.Tables[0].Rows[0]["LocationCode"]),
                        LocationName = Convert.ToString(ds.Tables[0].Rows[0]["LocationName"])
                    };
                    _con.Close();
                    return found;
                }
                _con.Close();
            }
           
            return null;
        }

        public List<Location> GetLocations()
        {
            List<Location> locations = new List<Location>();
            string query = string.Format("SELECT * from Location;");

            using (SqlConnection _con = new SqlConnection(DBHelper.ConnectionString))
            {
                _con.Open();
                DataSet ds = DBHelper.LoadData(_con, query);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Location aLocation = new Location()
                        {
                            LocationId = Convert.ToInt64(r["LocationId"]),
                            LocationCode = Convert.ToInt16(r["LocationCode"]),
                            LocationName = Convert.ToString(r["LocationName"])
                        };
                        locations.Add(aLocation);
                    }
                }
                _con.Close();
            }

            return locations;

        }

        public void Remove(long id)
        {
            string query = string.Format("DELETE FROM Location WHERE LocationId = {0}", id);
            using (SqlConnection _con = new SqlConnection(DBHelper.ConnectionString))
            {
                _con.Open();
                DBHelper.Execute(_con, query);
                _con.Close();
            }
        }
    }
}
