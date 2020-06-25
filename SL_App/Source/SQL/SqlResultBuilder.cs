using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_App.SQL
{
    public class SqlResultBuilder
    {
        private int _columnCount;
        private string[] _columnNames;

        private List<SqlValue[]> _rows;
        private int _rowIndex = 0;

        private SqlValue[] _curRow;

        private SqlResultBuilder(int colCount)
        {
            _columnCount = colCount;
            _columnNames = new string[_columnCount];
            _rows = new List<SqlValue[]>();
        }

        public void SetColumnName(int index, string name)
        {
            _columnNames[index] = name;
        }

        public void PushRow()
        {
            _curRow = new SqlValue[_columnCount];
            _rowIndex = 0;
        }

        public void AddValue(string value)
        {
            if (_rowIndex < _columnCount)
            {
                _curRow[_rowIndex] = new SqlValue(value);
                _rowIndex++;
            }
        }

        public void PopRow()
        {
            SqlValue[] newValues = new SqlValue[_columnCount];
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
            foreach(SqlValue[] row in _rows)
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

    }
}
