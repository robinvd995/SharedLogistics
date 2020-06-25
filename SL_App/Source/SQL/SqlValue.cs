using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_App.SQL
{
    public class SqlValue
    {
        private string _value;

        public SqlValue(string value)
        {
            _value = value;
        }

        public string AsString()
        {
            return _value;
        }
    }
}
