using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestCore.Models.IRepository
{
    public interface ILocationRepository
    {
        void Add(Location l);
        void Edit(Location l);
        void Remove(long id);
        Location Find(long id);
        List<Location> GetLocations();
    }
}
