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
        SqlValue GetValue(int column, int row);
    }

    public class SqlResult : ISqlResultSet
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

        public void AddRow(int index, SqlValue[] row)
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
                SqlValue[] dummyValues = new SqlValue[_columnCount];
                for(int i = 0; i < _columnCount; i++)
                {
                    dummyValues[i] = new SqlValue("NULL");
                }

                return new SqlRow(dummyValues);
            }
        }

        public SqlValue GetValue(int column, int row)
        {
            if(column < _columnCount && row < _rowCount)
            {
                SqlRow sqlrow = GetRow(row);
                return sqlrow.Values[column];
            }
            else
            {
                return new SqlValue("NULL");
            }
        }
    }
}
