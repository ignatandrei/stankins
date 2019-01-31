using System.Collections.Generic;
using System.Data;

namespace Stankins.Interfaces
{
    public interface ISender : IBaseObject
    {

    }

    public interface ISenderToOutput : ISender
    {
        string InputTemplate { get; set; }
        DataTableString OutputString { get; set; }
        DataTableString OutputByte { get; set; }

    }

    public class DataTableString : DataTable
    {
        public DataTableString():base()
        {
            this.Columns.Add(new DataColumn("ID", typeof(int)) { AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 });
            this.Columns.Add("Name", typeof(string));
            this.Columns.Add("Contents", typeof(string));


        }
    }
    public class DataTableByte : DataTable
    {
        public DataTableByte() : base()
        {
            this.Columns.Add("ID", typeof(long));
            this.Columns.Add("Name", typeof(string));
            this.Columns.Add("Contents", typeof(byte));


        }
    }
}
