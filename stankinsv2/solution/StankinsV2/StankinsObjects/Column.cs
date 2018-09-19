using Stankins.Interfaces;
using System.Diagnostics;

namespace StankinsV2Objects
{
    [DebuggerDisplay("{Id} {Name} IDTable = {IDTable}")]
    public class Column : MetadataRow, IColumn
    {
        public int IDTable { get ; set ; }

    }
}
