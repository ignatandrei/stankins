using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StanskinsImplementation
{
    class TaskExecutedList : List<TaskExecuted>
    {
        public Task[] AllTasksWithErrors()
        {
            return this.Select(item => item.t).ToArray();
        }
        public Task[] AllTasksWithLogging()
        {
            return this.Select(item => item.Execute()).ToArray();
        }
    }
    class TaskExecuted
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
                this.Exception = ex;
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
