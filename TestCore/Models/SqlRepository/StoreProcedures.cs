using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestCore.Models.SqlRepository
{
    public enum StorePorcedureNames
    {
        GetOpeningQty,
    }
    public class StoreProceduresParams
    {
        public static class GetOpeningQty
        {
            public const string LocationId = "@LocationId";
            public const string ProductId = "@ProductId";
            public const string StartDate = "@StartDate";

        }
    }
}
