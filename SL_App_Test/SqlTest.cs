using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SL_App.SQL;

namespace SL_App_Test
{
    [TestClass]
    public class SqlTest
    {
        [TestMethod]
        public void SqlBuilderTest()
        {
            SqlResultBuilder builder = SqlResultBuilder.NewInstance(5);
            builder.SetColumnName(0, "Col-0");
            builder.SetColumnName(1, "Col-1");
            builder.SetColumnName(2, "Col-2");
            builder.SetColumnName(3, "Col-3");
            builder.SetColumnName(4, "Col-4");

            builder.PushRow();
            builder.AddValue("R0-C0");
            builder.AddValue("R0-C1");
            builder.AddValue("R0-C2");
            builder.AddValue("R0-C3");
            builder.AddValue("R0-C4");
            builder.PopRow();

            builder.PushRow();
            builder.AddValue("R1-C0");
            builder.AddValue("R1-C1");
            builder.AddValue("R1-C2");
            builder.AddValue("R1-C3");
            builder.AddValue("R1-C4");
            builder.PopRow();

            builder.PushRow();
            builder.AddValue("R2-C0");
            builder.AddValue("R2-C1");
            builder.AddValue("R2-C2");
            builder.AddValue("R2-C3");
            builder.AddValue("R2-C4");
            builder.PopRow();

            ISqlResultSet result = builder.Build();

            Assert.IsTrue(result.GetColumnCount() == 5, "Column count is not equal to 5!");
            Assert.IsTrue(result.GetRowCount() == 3, "Row count is not equal to 5!");

            for (int i = 0; i < result.GetColumnCount(); i++)
            {
                string expected0 = "Col-" + i;
                string actual0 = result.GetColumnName(i);
                Assert.IsTrue(expected0.Equals(actual0), "Expected value{0}, current value {1}", expected0, actual0);

                for (int j = 0; j < result.GetRowCount(); j++)
                {
                    string actual1 = result.GetValue(i, j).AsString();
                    string expected1 = "R" + j + "-C" + i;
                    Assert.IsTrue(expected1.Equals(actual1), "Expected value{0}, current value {1}", expected1, actual1);
                }
            }

        }
    }
}
