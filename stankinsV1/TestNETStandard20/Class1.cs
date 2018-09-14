using System;
using System.Data;

namespace TestNETStandard20
{
    public class Class1
    {
        public DataTable MyProperty { get; set; }
        public Class1()
        {
            MyProperty = new DataTable("ASda");
            MyProperty.Columns.Add("ASD", typeof(int));
        }
    }
}
