using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SL_App.HTML
{
    public abstract class HTMLControl : Enumeration
    {
        public static HTMLControl Table = new HTMLControlTable(0, "table");

        public HTMLControl(int id, string name) :
            base(id, name)
        {

        }

        public abstract string BuildControl(string[] arguments, HTMLValueMapper mapper);
    }

    public class HTMLControlTable : HTMLControl
    {
        public HTMLControlTable(int id, string name) :
            base(id, name)
        {

        }

        public override string BuildControl(string[] arguments, HTMLValueMapper mapper)
        {
            IHTMLTableSchema schema = mapper.GetValue<IHTMLTableSchema>(arguments[0]);

            HTMLBuilder builder = new HTMLBuilder();

            builder.PushElement("table");
            builder.PushElement("tr");

            for (int i = 0; i < schema.GetColumnCount(); i++)
            {
                string columnName = schema.GetColumnName(i);
                builder.PushElement("th");
                builder.SetValue(columnName);
                builder.PopElement();
            }

            builder.PopElement();

            for (int j = 0; j < schema.GetRowCount(); j++)
            {
                builder.PushElement("tr");

                for (int i = 0; i < schema.GetColumnCount(); i++)
                {
                    string value = schema.GetValueAsString(i, j);
                    builder.PushElement("td");
                    builder.SetValue(value);
                    builder.PopElement();
                }

                builder.PopElement();
            }

            builder.PopElement();

            return builder.BuildHTML();
        }
    }

    public interface IHTMLTableSchema
    {
        int GetColumnCount();
        int GetRowCount();
        string GetColumnName(int index);
        string GetValueAsString(int column, int row);
    }
}
