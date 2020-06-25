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
        public void SqlBuilderTestString()
        {
            SqlResultBuilder builder = SqlResultBuilder.NewInstance(5);

            builder.SetColumnType(0, "nvarchar");
            builder.SetColumnType(1, "nvarchar");
            builder.SetColumnType(2, "nvarchar");
            builder.SetColumnType(3, "nvarchar");
            builder.SetColumnType(4, "nvarchar");

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

        [TestMethod]
        public void SqlBuilderTestInt()
        {
            SqlResultBuilder builder = SqlResultBuilder.NewInstance(3);

            builder.SetColumnType(0, "int");
            builder.SetColumnType(1, "int");
            builder.SetColumnType(2, "int");

            builder.SetColumnName(0, "Col-0");
            builder.SetColumnName(1, "Col-1");
            builder.SetColumnName(2, "Col-2");

            builder.PushRow();
            builder.AddValue(0);
            builder.AddValue(1);
            builder.AddValue(2);
            builder.PopRow();

            builder.PushRow();
            builder.AddValue(3);
            builder.AddValue(4);
            builder.AddValue(5);
            builder.PopRow();

            ISqlResultSet result = builder.Build();

            Assert.IsTrue(result.GetColumnCount() == 3, "Column count is not equal to 5!");
            Assert.IsTrue(result.GetRowCount() == 2, "Row count is not equal to 5!");

            Assert.IsTrue(result.GetValue(0, 0).AsInt() == 0);
            Assert.IsTrue(result.GetValue(1, 0).AsInt() == 1);
            Assert.IsTrue(result.GetValue(2, 0).AsInt() == 2);

            Assert.IsTrue(result.GetValue(0, 1).AsInt() == 3);
            Assert.IsTrue(result.GetValue(1, 1).AsInt() == 4);
            Assert.IsTrue(result.GetValue(2, 1).AsInt() == 5);
        }

        [TestMethod]
        public void SqlBuilderTestBit()
        {
            SqlResultBuilder builder = SqlResultBuilder.NewInstance(2);

            builder.SetColumnType(0, "bit");
            builder.SetColumnType(1, "bit");

            builder.SetColumnName(0, "Col-0");
            builder.SetColumnName(1, "Col-1");

            builder.PushRow();
            builder.AddValue(0);
            builder.AddValue(1);
            builder.PopRow();

            builder.PushRow();
            builder.AddValue(false);
            builder.AddValue(true);
            builder.PopRow();

            ISqlResultSet result = builder.Build();

            Assert.IsTrue(result.GetColumnCount() == 2, "Column count is not equal to 5!");
            Assert.IsTrue(result.GetRowCount() == 2, "Row count is not equal to 5!");

            Assert.IsTrue(result.GetValue(0, 0).AsBool() == false);
            Assert.IsTrue(result.GetValue(1, 0).AsBool() == true);

            Assert.IsTrue(result.GetValue(0, 1).AsBool() == false);
            Assert.IsTrue(result.GetValue(1, 1).AsBool() == true);
        }
    }
}
