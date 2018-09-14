using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StankinsInterfaces
{
    /// <summary>
    /// Interface which defines the methods which must be implemented by receiver objects.
    /// </summary>
    public interface IReceive: IBaseObjects
    {
        /// <summary>
        /// Execute the receiver and fills valuesRead with data.
        /// </summary>
        /// <returns></returns>
        Task LoadData();

        /// <summary>
        /// Used to store received data.
        /// </summary>
        IRowReceive[] valuesRead { get; }

        /// <summary>
        /// Deletes data from valuesRead. 
        /// </summary>
        void ClearValues();
    }
    public interface IReceive<T>:IReceive
        where T : IComparable<T>
    {
       

        T LastValue { get; set; }
    }
}