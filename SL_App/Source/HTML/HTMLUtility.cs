using SL_App.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_App.HTML
{
    public class SimpleHTMLTable : IHTMLTableSchema
    {
        private int _columnCount;
        private int _rowCount;
        private string[] _columnNames;
        private string[,] _values;

        private SimpleHTMLTable(int colCount, int rowCount)
        {
            _columnCount = colCount;
            _rowCount = rowCount;
            _columnNames = new string[colCount];
            _values = new string[rowCount,colCount];
        }

        public int GetColumnCount()
        {
            return _columnCount;
        }

        public string GetColumnName(int index)
        {
            return _columnNames[index];
        }

        public int GetRowCount()
        {
            return _rowCount;
        }

        public string GetValueAsString(int column, int row)
        {
            return _values[row,column];
        }

        public static SimpleHTMLTable FromSqlResult(ISqlResultSet result)
        {
            return FromSqlResult(result, result.GetRowCount());
        }

        public static SimpleHTMLTable FromSqlResult(ISqlResultSet result, int rows)
        {
            SimpleHTMLTable table = new SimpleHTMLTable(result.GetColumnCount(), rows);
            for (int i = 0; i < table.GetColumnCount(); i++)
            {
                table._columnNames[i] = result.GetColumnName(i);
            }

            for (int i = 0; i < table.GetColumnCount(); i++)
            {
                for(int j = 0; j < table.GetRowCount(); j++)
                {
                    table._values[j,i] = result.GetValue(i, j).AsString();
                }
            }
            return table;
        }

        public static SimpleHTMLTable FromSqlResult(ISqlResultSet result, int startRow, int size)
        {
            SimpleHTMLTable table = new SimpleHTMLTable(result.GetColumnCount(), size);
            for (int i = 0; i < table.GetColumnCount(); i++)
            {
                table._columnNames[i] = result.GetColumnName(i);
            }

            for (int i = 0; i < table.GetColumnCount(); i++)
            {
                for (int j = 0; j < size; j++)
                {
                    table._values[j, i] = result.GetValue(i, startRow + j).AsString();
                }
            }
            return table;
        }
    }
}
