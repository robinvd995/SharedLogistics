using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace SL_App.SQL
{
    internal struct ColumnTypeWrapper
    {
        internal ColumnTypeWrapper(string type, Func<object, ISqlValue> dlgt)
        {
            TypeName = type;
            Delegate = dlgt;
        }

        internal string TypeName { get; }
        internal Func<object, ISqlValue> Delegate { get; }
    }

    public class SqlResultBuilder
    {
        private int _columnCount;
        private string[] _columnNames;
        private ColumnTypeWrapper[] _columnTypes;

        private List<ISqlValue[]> _rows;
        private int _rowIndex = 0;

        private ISqlValue[] _curRow;

        private SqlResultBuilder(int colCount)
        {
            _columnCount = colCount;
            _columnNames = new string[_columnCount];
            _columnTypes = new ColumnTypeWrapper[_columnCount];
            _rows = new List<ISqlValue[]>();
        }

        public void SetColumnName(int index, string name)
        {
            _columnNames[index] = name;
        }

        public void SetColumnType(int index, string type)
        {
            var dic = GetTypeDictionary();
            bool success = dic.TryGetValue(type, out Func<object, ISqlValue> func);
            if (success && index < _columnCount)
            {
                _columnTypes[index] = new ColumnTypeWrapper(type, func);
            }
        }

        public void PushRow()
        {
            _curRow = new ISqlValue[_columnCount];
            _rowIndex = 0;
        }

        public void AddValue(object value)
        {
            if (_rowIndex < _columnCount)
            {
                var dlgt = _columnTypes[_rowIndex].Delegate;
                ISqlValue sqlvalue = dlgt.Invoke(value);
                _curRow[_rowIndex] = sqlvalue;
                _rowIndex++;
            }
        }

        public void PopRow()
        {
            ISqlValue[] newValues = new ISqlValue[_columnCount];
            Array.Copy(_curRow, newValues, _columnCount);

            _rows.Add(newValues);
            _curRow = null;
        }

        public ISqlResultSet Build()
        {
            SqlResult result = new SqlResult(_columnCount, _rows.Count);

            string[] newColumnNames = new string[_columnCount];
            Array.Copy(_columnNames, newColumnNames, _columnCount);
            result.SetColumnNames(newColumnNames);

            int i = 0;
            foreach(ISqlValue[] row in _rows)
            {
                result.AddRow(i, row);
                i++;
            }

            return result;
        }

        public static SqlResultBuilder NewInstance(int columnCount)
        {
            SqlResultBuilder builder = new SqlResultBuilder(columnCount);

            return builder;
        }

        public Dictionary<string, Func<object, ISqlValue>> GetTypeDictionary()
        {
            var typedic = new Dictionary<string, Func<object, ISqlValue>>
            {
                { "bit", CreateSqlValueBit },
                { "int", CreateSqlValueInt32 },
                { "nvarchar", CreateSqlValueString },
                { "date", CreateSqlValueDateTime },
                { "time", CreateSqlValueDateTime },
                { "decimal", CreateSqlValueDecimal }
            };
            return typedic;
        }

        private ISqlValue CreateSqlValueString(object value)
        {
            if(value == null || value is DBNull)
            {
                return new SqlValueString(null);
            }
            else
            {
                return new SqlValueString((string)value);
            }
        }

        private ISqlValue CreateSqlValueDateTime(object value)
        {
            if (value == null || value is DBNull)
            {
                return new SqlValueString(null);
            }
            else
            {
                DateTime dateTime = (DateTime)value;
                return new SqlValueString(dateTime.ToString());
            }
        }

        private ISqlValue CreateSqlValueInt32(object value)
        {
            if(value == null || value is DBNull)
            {
                return new SqlValueInt(null);
            }
            else
            {
                return new SqlValueInt((int)value);
            }
        }

        private ISqlValue CreateSqlValueBit(object value)
        {
            if (value == null || value is DBNull)
            {
                return new SqlValueBool(null);
            }
            else
            {
                if (value is int)
                {
                    Console.WriteLine("isint");
                    return new SqlValueBool((int)value == 1);
                }
                if (value is bool)
                {
                    return new SqlValueBool((bool)value);
                }
                return new SqlValueBool(null);
            }
        }

        private ISqlValue CreateSqlValueDecimal(object value)
        {
            if (value == null || value is DBNull)
            {
                return new SqlValueDecimal(null);
            }
            else
            {
                return new SqlValueDecimal((Decimal)value);
            }
        }

    }
}
