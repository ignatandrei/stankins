using System;
using System.Threading.Tasks;

namespace StankinsInterfaces
{
    /// <summary>
    /// Defines members to be implemented by every job.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// It executes all objects (receivers, transformers, senders) from current job.
        /// </summary>
        /// <returns></returns>
        Task Execute();

        /// <summary>
        /// Serializes into a JSON string the definition of current job.
        /// </summary>
        /// <returns></returns>
        string SerializeMe();

        /// <summary>
        /// Deserializes from a JSON string the definition of current job.
        /// </summary>
        /// <returns></returns>
        void UnSerialize(string serializeData);
    }
    public class RuntimeParameter
    {
        
        public string[] NameObjectsToApplyTo { get; set; }
        public string VariableName { get; set; }
    }

    /// <summary>
    /// Simplest job which include one or more receivers, zero or more transformers and one or more senders.
    /// Execution plan: first it execute all receivers, then all transformers (if exist) and then all senders.
    /// </summary>
    public interface ISimpleJob    :IJob   
    {
        /// <summary>
        /// List of receivers.
        /// </summary>
        OrderedList<IReceive> Receivers { get; }
        /// <summary>
        /// List of transformers.
        /// </summary>
        OrderedList<IFilterTransformer> FiltersAndTransformers { get;  }        
        /// <summary>
        /// List of senders.
        /// </summary>
        OrderedList<ISend> Senders { get;  }

        RuntimeParameter[] RuntimeParameters { get; }
    }

}
