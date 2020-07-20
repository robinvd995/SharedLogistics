using SL_App.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_App.HTML
{
    public class HTMLParser
    {
        private string _source;

        public HTMLParser(string source)
        {
            _source = source;
        }

        public object DataContext { get; set; }

        public string Parse()
        {
            //Scanning for nodes to parse
            StringBuilder builder = new StringBuilder();
            int lastIndex = 0;
            for(int i = 0; i < _source.Length; i++) 
            {
                char c = _source[i];

                if(c == '@')
                {
                    bool found = ScanForStatement(i, out string statement, out int newIndex);
                    if (found)
                    {
                        string subs = _source.Substring(lastIndex, i - lastIndex);
                        builder.Append(subs);
                        builder.Append(ParseStatement(statement.Substring(1)));
                        i = newIndex - 1;
                        lastIndex = newIndex;
                    }
                }
            }

            string finalsubs = _source.Substring(lastIndex, _source.Length - lastIndex);
            builder.Append(finalsubs);

            return builder.ToString();
        }

        private string ParseStatement(string statement)
        {
            string trimmedStatement = statement.Replace(" ", "");
            int identifierEnd = trimmedStatement.IndexOf('(');
            string identifier = trimmedStatement.Substring(0, identifierEnd);
            int argumentsEnd = trimmedStatement.IndexOf(')');
            string argumentLine = trimmedStatement.Substring(identifierEnd + 1, argumentsEnd - identifierEnd - 1);
            string[] arguments = argumentLine.Split(',');

            string parsed = "";
            try
            {
                Console.WriteLine(identifier);
                HTMLControl control = Enumeration.GetAll<HTMLControl>().Where(c => c.Name.Equals(identifier)).First();
                parsed = control.BuildControl(arguments, DataContext);
            }
            catch(Exception e) { Console.WriteLine(e.Message); }

            return parsed;
        }

        private bool ScanForStatement(int index, out string statement, out int newIndex)
        {
            for(int i = 0; i < 200; i++)
            {
                int realIndex = index + i;
                if(realIndex < _source.Length)
                {
                    char c = _source[realIndex];
                    if(c == ';')
                    {
                        statement = _source.Substring(index, i);
                        newIndex = realIndex + 1;
                        return true;
                    }
                }
                else
                {
                    newIndex = index;
                    statement = "ERROR";
                    return false;
                }
            }

            newIndex = index;
            statement = "ERROR";
            return false;
        }
    }

    public class HTMLValueMapper
    {
        public HTMLValueMapper()
        {
            ValueMap = new Dictionary<string, object>();
        }

        public Dictionary<string,object> ValueMap { get; private set; }

        public T GetValue<T>(string id)
        {
            bool success = ValueMap.TryGetValue(id, out object value);
            if (success)
            {
                if(value is T t)
                {
                    return t;
                }
            }

            return default;
        }
    }
}
