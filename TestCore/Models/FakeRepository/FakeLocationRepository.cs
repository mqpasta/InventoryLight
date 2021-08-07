using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TestCore.Models;
using TestCore.Models.IRepository;

namespace TestCore.Models.FakeRepository
{
    public class FakeLocationRepository : ILocationRepository
    {
        public static List<Location> _locations = new List<Location>();

        public void Add(Location l)
        {
            l.LocationId = new Random().Next(100, 200);
            _locations.Add(l);
        }

        public void Edit(Location l)
        {
            var found = _locations.FirstOrDefault(x => x.LocationId == l.LocationId);
            if(found != null)
            {
                found.LocationCode = l.LocationCode;
                found.LocationName = l.LocationName;
            }
        }

        public Location Find(long id)
        {
            return _locations.FirstOrDefault(x => x.LocationId == id);
        }

        public List<Location> GetLocations()
        {
            return _locations;
        }

        public void Remove(long id)
        {
            _locations.Remove(_locations.Find(x => x.LocationId == id));
        }
    }
}
