using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_App.SQL
{
    public interface ISqlValue
    {
        string AsString();
        bool? AsBool();
        int? AsInt();
        double? AsDouble();
        long? AsLong();
    }

    public class SqlValueString : ISqlValue
    {
        private string _value;

        public SqlValueString(string value)
        {
            _value = value;
        }

        public bool? AsBool()
        {
            return false;
        }

        public double? AsDouble()
        {
            return 0.0D;
        }

        public int? AsInt()
        {
            return 0;
        }

        public long? AsLong()
        {
            return 0;
        }

        public string AsString()
        {
            return _value;
        }
    }

    public class SqlValueBool : ISqlValue
    {
        private bool? _value;
        public SqlValueBool(bool? value)
        {
            _value = value;
        }

        public bool? AsBool()
        {
            return _value;
        }

        public double? AsDouble()
        {
            return !_value.HasValue ? null : (double?)(_value.Value ? 1.0D : 0.0D);
        }

        public int? AsInt()
        {
            return !_value.HasValue ? null : (int?)(_value.Value ? 1 : 0);
        }

        public long? AsLong()
        {
            return (long?)AsInt();
        }

        public string AsString()
        {
            return !_value.HasValue ? "NULL" : (_value.Value ? "TRUE" : "FALSE");
        }
    }

    public class SqlValueInt : ISqlValue
    {
        private int? _value;

        public SqlValueInt(int? value)
        {
            _value = value;
        }

        public bool? AsBool()
        {
            return !_value.HasValue ? null : (bool?)(_value.Value == 0 ? false : true);
        }

        public double? AsDouble()
        {
            return !_value.HasValue ? null : (double?)_value;
        }

        public int? AsInt()
        {
            return _value;
        }

        public long? AsLong()
        {
            return (long?)AsInt();
        }

        public string AsString()
        {
            return !_value.HasValue ? "NULL" : (_value.Value.ToString());
        }
    }

    public class SqlValueDecimal : ISqlValue
    {
        private decimal? _value;

        public SqlValueDecimal(decimal? value)
        {
            _value = value;
        }

        public bool? AsBool()
        {
            return !_value.HasValue ? null : (bool?)((int)_value.Value == 0 ? false : true);
        }

        public double? AsDouble()
        {
            return (double?)_value;
        }

        public int? AsInt()
        {
            return !_value.HasValue ? null : (int?)_value;
        }

        public long? AsLong()
        {
            return (long?)AsInt();
        }

        public string AsString()
        {
            return !_value.HasValue ? "NULL" : (_value.Value.ToString());
        }
    }

    public class SqlValueLong : ISqlValue
    {
        private long? _value;

        public SqlValueLong(long? value)
        {
            _value = value;
        }

        public bool? AsBool()
        {
            return !_value.HasValue ? null : (bool?)(_value.Value == 0 ? false : true);
        }

        public double? AsDouble()
        {
            return !_value.HasValue ? null : (double?)_value;
        }

        public int? AsInt()
        {
            return !_value.HasValue ? null : (int?)_value;
        }

        public long? AsLong()
        {
            return _value;
        }

        public string AsString()
        {
            return !_value.HasValue ? "NULL" : (_value.Value.ToString());
        }
    }

}
