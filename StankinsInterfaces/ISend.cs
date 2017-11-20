using System.Threading.Tasks;

namespace StankinsInterfaces
{
    /// <summary>
    /// Interface which defines the methods which must be implemented by sender objects.
    /// </summary>
    public interface ISend : IBaseObjects
    {
        /// <summary>
        /// Used to store data to be sent.
        /// </summary>
        IRow[] valuesToBeSent{ set; }

        /// <summary>
        /// Read data from valuesToBeSent and it executes the sender object.
        /// </summary>
        /// <returns></returns>
        Task Send();
        //TODO: just export some fields, not all...
        //string[] FieldNames { set; }
    }
}