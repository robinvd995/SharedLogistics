using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_App.HTML
{
    public class HTMLElement
    {
        public HTMLElement()
        {
            Attributes = new Dictionary<string, string>();
            Children = new List<HTMLElement>();
            Value = "";
        }

        public Dictionary<string, string> Attributes;
        public string ElementTag { get; set; }
        public List<HTMLElement> Children { get; set; }
        public HTMLElement Parent { get; set; }
        public string Value { get; set; }
        public bool Single { get; set; }

        public void BuildElement(StringBuilder builder)
        {
            builder.Append(GetElementString());
            if (!Single)
            {
                builder.Append(Value);
                foreach(var child in Children)
                {
                    child.BuildElement(builder);
                }

                builder.Append("</" + ElementTag + ">");
            }
        }

        private string GetElementString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<");
            builder.Append(ElementTag);

            if(Attributes.Count > 0)
            {
                builder.Append(" ");
                foreach (var pair in Attributes)
                {
                    string attribute = pair.Key + "=" + "\"" + pair.Value + "\" ";
                    builder.Append(attribute);
                }
            }

            if (Single)
            {
                builder.Append("/");
            }

            builder.Append(">");

            return builder.ToString();
        }
    }
}
