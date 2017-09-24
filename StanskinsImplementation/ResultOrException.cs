using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace StanskinsImplementation
{
    //TODO: make nuget package
    public class TaskExecutedList : List<TaskExecuted>
    {
        //public Task[] AllTasksWithErrors()
        //{
        //    return this.Select(item => item.t).ToArray();
        //}
        public Task[] AllTasksWithLogging()
        {
            return this.Select(item => item.Execute()).ToArray();
        }
    }
    public class TaskExecuted
    {
        public Exception Exception { get; protected set; }
        public Task t;
        public TaskExecuted(Task t)
        {
            this.t = t;
        }

        
        public bool IsSuccess()
        {
            return Exception == null;
        }
        public virtual async Task Execute()
        {
            try
            {
                await t;
            }
            catch( Exception ex)
            {
                string message = ex.Message;
                //@class.Log(LogLevel.Error, 0,"error in execute"+ message, ex, null);
                Exception = ex;
                throw;
            }
        }
    }
    class TaskExecutedWithResult<T>: TaskExecuted
    {
        public T Result { get; protected set; }
        
        public TaskExecutedWithResult(Task<T> t):base(t)
        {
            
        }
        
        public override async Task Execute()
        {
            try
            {
                Task<T> initial = t as Task<T>;
                Result= await initial;

            }
            catch (Exception ex)
            {
                this.Exception = ex;
            }
        }
    }
}
