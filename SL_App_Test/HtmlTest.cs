using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SL_App.HTML;

namespace SL_App_Test
{
    [TestClass]
    public class HTMLBuilderTest
    {
        [TestMethod]
        public void HTMLTableParserTest()
        {
            HTMLValueMapper mapper = new HTMLValueMapper();
            mapper.ValueMap["MainTable"] = new DummySchema();
            mapper.ValueMap["SecondTable"] = new DummySchema();
            HTMLParser parser = new HTMLParser("<html><body>@table ( MainTable ) ;@table(SecondTable);</body></html>", mapper);
            string s = parser.Parse();
            Console.WriteLine(s);
        }

        public class DummySchema : IHTMLTableSchema
        {
            private string[] _columnNames;
            private string[,] _values;

            public DummySchema()
            {
                _columnNames = new string[]
                {
                    "Col-1","Col-2","Col-3","Col-4","Col-5","Col-6"
                };

                _values = new string[,]
                {
                    { "R1C1", "R1C2", "R1C3", "R1C4", "R1C5", "R1C6" },
                    { "R2C1", "R2C2", "R2C3", "R2C4", "R2C5", "R2C6" },
                    { "R3C1", "R3C2", "R3C3", "R3C4", "R3C5", "R3C6" },
                    { "R4C1", "R4C2", "R4C3", "R4C4", "R4C5", "R4C6" },
                };

            }

            public int GetColumnCount()
            {
                return 6;
            }

            public string GetColumnName(int index)
            {
                return _columnNames[index];
            }

            public int GetRowCount()
            {
                return 4;
            }

            public string GetValueAsString(int column, int row)
            {
                return _values[row, column];
            }
        }

        [TestMethod]
        public void BuilderTest()
        {
            HTMLBuilder builder = new HTMLBuilder();

            /*builder.PushElement("div");
            builder.AddAttribute("margin", "4 4 4 4");
            builder.SetValue("Hello World!");
            builder.PopElement();*/
            builder.PushElement("html");

                builder.PushElement("head");

                    builder.PushElement("style");
                    builder.SetValue("table, th, td { border: 1px solid black; border-collapse: collapse;} th, td { padding: 10px; }");
                    builder.PopElement();

                builder.PopElement();

            builder.PushElement("body");

                builder.PushElement("table");

                    builder.PushElement("tr");

                    builder.PushElement("th");
                    builder.SetValue("Header1");
                    builder.PopElement();

                    builder.PushElement("th");
                    builder.SetValue("Header2");
                    builder.PopElement();

                    builder.PushElement("th");
                    builder.SetValue("Header3");
                    builder.PopElement();

                    builder.PopElement();

                    builder.PushElement("tr");

                    builder.PushElement("td");
                    builder.SetValue("Data1");
                    builder.PopElement();

                    builder.PushElement("td");
                    builder.SetValue("Data2");
                    builder.PopElement();

                    builder.PushElement("td");
                    builder.SetValue("Data3");
                    builder.PopElement();

                    builder.PopElement();

            builder.PopElement();

            builder.PopElement();

            builder.PopElement();


            Console.WriteLine(builder.BuildHTML());
        }
    }
}
