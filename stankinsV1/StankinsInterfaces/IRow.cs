using System.Collections.Generic;

namespace StankinsInterfaces
{
    /// <summary>
    /// It includes members needed to store a row of values.
    /// </summary>
    public interface IRow
    {
        /// <summary>
        /// Values are stored using a dictionary of column - value pairs.
        /// </summary>
        Dictionary<string, object> Values { get; set; }
    }
}