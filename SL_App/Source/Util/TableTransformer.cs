using Microsoft.Office.Interop.Outlook;
using Newtonsoft.Json;
using SL_App.HTML;
using SL_App.SQL;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_App.Util
{
    public class TableTransformer
    {
        private TableTransformData _data;

        private TableTransformer(TableTransformData data)
        {
            _data = data;
        }

        public ISqlResultSet TransformSqlResultSet(ISqlResultSet resultSet)
        {
            List<int> colIndices = new List<int>();
            List<string> colIds = new List<string>();
            List<string> colNames = new List<string>();

            int i = 0;
            var e = _data.Columns.GetEnumerator();
            while (e.MoveNext())
            {
                string colId = e.Current.Key;
                int colIndex = resultSet.ColumnIndexOf(colId);
                if(colIndex != -1)
                {
                    colIndices.Add(colIndex);
                    colIds.Add(colId);
                    colNames.Add(e.Current.Value);
                    i++;
                }
            }

            SqlResult transformedResults = new SqlResult(colIndices.Count, resultSet.GetRowCount());
            transformedResults.SetColumnNames(colNames.ToArray());

            for(int j = 0; j < resultSet.GetRowCount(); j++)
            {
                ISqlValue[] row = new ISqlValue[colIndices.Count];
                i = 0;
                foreach(int index in colIndices)
                {
                    row[i] = resultSet.GetValue(index, j);
                    i++;
                }
                
                transformedResults.AddRow(j, row);
            }

            return transformedResults;
        }

        public static TableTransformer FromJson(string json)
        {
            TableTransformData data = JsonConvert.DeserializeObject<TableTransformData>(json);
            return new TableTransformer(data);
        }
    }

    public class TableTransformData
    {
        public Dictionary<string, string> Columns { get; set; }
    }
}
