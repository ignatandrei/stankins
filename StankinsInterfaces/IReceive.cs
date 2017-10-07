using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StankinsInterfaces
{
    public interface IReceive: IBaseObjects
    {
        Task LoadData();

        IRowReceive[] valuesRead { get; }

        void ClearValues();
    }
    public interface IReceive<T>:IReceive
        where T : IComparable<T>
    {
       

        T LastValue { get; set; }
    }
}