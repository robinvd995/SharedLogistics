using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_App.SQL
{
    public struct SqlRow
    {
        public SqlRow(ISqlValue[] values)
        {
            Values = values;
        }

        public ISqlValue[] Values { get; }
    }
}
