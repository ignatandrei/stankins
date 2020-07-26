using Stankins.Interfaces;
using System.Diagnostics;

namespace StankinsObjects 
{
    [DebuggerDisplay("{Name} {Id}")]
    public class Table: MetadataRow, ITable
    {

    }
}
