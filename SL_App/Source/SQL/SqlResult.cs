using SL_App.HTML;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_App.SQL
{
    public interface ISqlResultSet
    {
        int GetColumnCount();
        string GetColumnName(int index);
        int GetRowCount();
        SqlRow GetRow(int index);
        ISqlValue GetValue(int column, int row);
        int ColumnIndexOf(string columnName);
    }

    public class SqlResult : ISqlResultSet, IHTMLTableSchema
    {
        private SqlRow[] _rows;
        private string[] _columnNames;
        private int _columnCount;
        private int _rowCount;

        public SqlResult(int columns, int rows)
        {
            _columnCount = columns;
            _columnNames = new string[_columnCount];

            _rowCount = rows;
            _rows = new SqlRow[_rowCount];
        }

        public void SetColumnNames(string[] columnNames)
        {
            _columnNames = columnNames;
        }

        public void AddRow(int index, ISqlValue[] row)
        {
            _rows[index] = new SqlRow(row);
        }

        public int GetColumnCount()
        {
            return _columnCount;
        }

        public string GetColumnName(int index)
        {
            if(index < _columnCount)
            {
                return _columnNames[index];
            }
            else
            {
                return "COLUMN INDEX OUT OF RANGE!";
            }
        }

        public int GetRowCount()
        {
            return _rowCount;
        }

        public SqlRow GetRow(int index)
        {
            if(index < _rowCount)
            {
                return _rows[index];
            }
            else
            {
                ISqlValue[] dummyValues = new ISqlValue[_columnCount];
                for(int i = 0; i < _columnCount; i++)
                {
                    dummyValues[i] = new SqlValueString("NULL");
                }

                return new SqlRow(dummyValues);
            }
        }

        public ISqlValue GetValue(int column, int row)
        {
            if(column < _columnCount && row < _rowCount)
            {
                SqlRow sqlrow = GetRow(row);
                return sqlrow.Values[column];
            }
            else
            {
                return new SqlValueString("NULL");
            }
        }

        public int ColumnIndexOf(string columnName)
        {
            if (columnName == null || columnName.Length == 0)
                return -1;

            for(int i = 0; i < _columnCount; i++)
            {
                if (_columnNames[i].Equals(columnName))
                    return i;
            }

            return -1;
        }

        public string GetValueAsString(int column, int row)
        {
            return GetValue(column, row).AsString();
        }
    }
}
