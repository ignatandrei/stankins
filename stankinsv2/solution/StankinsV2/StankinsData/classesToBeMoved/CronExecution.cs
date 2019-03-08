using Stankins.Interpreter;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace StankinsDataWeb.classesToBeMoved
{
    public class CronExecution
    {
        [DebuggerDisplay("{CRON} {LastRunTime} {NextRunTime}")]
        public class CronExecutionFileWithCRON
        {
            public string CRON { get; set; }
            public string Name{get;set;}
            private readonly RecipeFromString Recipe;

            public DateTime? LastRunTime { get; set; }
            public DateTime? NextRunTime { get; set; }


            public CronExecutionFileWithCRON(string name,string contents)
            {
                this.Name= name;
                string[] lines = contents.Split('\n')
                .Select(it => it.Replace("\r", ""))
                .ToArray();
                CRON = lines[0];

                Recipe = new RecipeFromString(string.Join(Environment.NewLine, lines.Skip(1)));
            }
            //TODO : error transmitting
            public async Task<bool> execute()
            {
                try{
                    await Recipe.TransformData(null);
                    return true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"error executing{ Name}=> {ex.Message}");
                    return false;
                }

            }
            public bool ShouldRun(DateTime currentTime)
            {
                if (NextRunTime == null && LastRunTime == null)
                {
                    

                    return true;//execute once
                }
                if (NextRunTime == null)
                {
                    return false;
                }

                if (NextRunTime < currentTime)
                {
                   

                    return true;
                }
                return false;
            }
        }
    }
}
