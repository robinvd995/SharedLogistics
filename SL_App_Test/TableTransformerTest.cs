using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SL_App.Util;

namespace SL_App_Test
{
    [TestClass]
    public class TableTransformerTest
    {
        [TestMethod]
        public void JsonReaderTest()
        {
            string json = "{\"Columns\": { \"INBOUNDID\": \"Job Number\",\"RELATIONCODE\": \"Relation Code\" } }";
            TableTransformData data = JsonConvert.DeserializeObject<TableTransformData>(json);

            TestHelper.AssertEqualsi(data.Columns.Values.Count, 2);

            var e = data.Columns.GetEnumerator();
            e.MoveNext();
            TestHelper.AssertEqualsString("INBOUNDID", e.Current.Key);
            TestHelper.AssertEqualsString("Job Number", e.Current.Value);
            e.MoveNext();
            TestHelper.AssertEqualsString("RELATIONCODE", e.Current.Key);
            TestHelper.AssertEqualsString("Relation Code", e.Current.Value);
        }
    }

    
}
