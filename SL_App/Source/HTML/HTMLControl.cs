using SL_App.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
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

        public abstract string BuildControl(string[] arguments, object context);

        public T GetProperty<T>(string propertyId, object context)
        {
            PropertyInfo pInfo = context.GetType().GetProperty(propertyId);
            if (pInfo == null)
                return default;

            object value = pInfo.GetValue(context);
            if(value is T t)
            {
                return t;
            }

            return default;
        }
    }

    public class HTMLControlTable : HTMLControl
    {
        public HTMLControlTable(int id, string name) :
            base(id, name)
        {

        }

        public override string BuildControl(string[] arguments, object context)
        {
            if (arguments == null || arguments.Length != 1)
                return "";

            IHTMLTableSchema schema = GetProperty<IHTMLTableSchema>(arguments[0], context);

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
