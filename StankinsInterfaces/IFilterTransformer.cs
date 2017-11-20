using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StankinsInterfaces
{
    /// <summary>
    /// Interface which defines the methods which must be implemented by transformer objects.
    /// </summary>
    public interface IFilterTransformer : IBaseObjects
    {      
        /// <summary>
        /// Executes the current transformer.
        /// </summary>
        /// <returns></returns>
        Task Run();
    }
}
