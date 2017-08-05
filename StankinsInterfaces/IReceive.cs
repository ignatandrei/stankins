using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StankinsInterfaces
{
    public interface IReceive
    {
        Task LoadData();

        IRowReceive[] valuesRead { get; }
    }
    public interface IReceive<T>:IReceive
        where T : IComparable<T>
    {
       

        T LastValue { get; set; }
    }
}