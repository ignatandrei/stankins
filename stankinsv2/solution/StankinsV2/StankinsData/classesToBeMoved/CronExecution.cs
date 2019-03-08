using Cronos;
using Stankins.Interpreter;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StankinsDataWeb.classesToBeMoved
{
    public class CronExecution
    {
        [DebuggerDisplay("{CRON} {LastRunTime} {NextRunTime}")]
        public class CronExecutionFile
        {
            public string CRON { get; set; }
            public string Name { get; set; }
            public string Content{get;set;}
            private RecipeFromString Recipe;
            private readonly string fileName;

            internal DateTime? LastRunTime { get; set; }
            internal DateTime? NextRunTime { get; set; }
            public CronExecutionFile()
            {

            }
            public CronExecutionFile(string fileName) : this(Path.GetFileName(fileName), File.ReadAllText(fileName))
            {
                this.fileName = fileName;
            }
            private bool Deleted=false;
            public bool reload()
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return false;
                }
                Deleted=(!File.Exists(fileName));
                if(Deleted)
                {
                    return false;
                }
                //TODO: create a backup if not work
                loadFromContent(File.ReadAllText(fileName));
                return true;
            }
            private void loadFromContent(string contents)
            {
                string[] lines = contents.Split('\n')
               .Select(it => it.Replace("\r", ""))
               .ToArray();
                CRON = lines[0];
                Content=string.Join(Environment.NewLine, lines.Skip(1));
                Recipe = new RecipeFromString(Content);

            }
            public string WholeContent()
            {
                return CRON + Environment.NewLine + Content;
            }
            public CronExecutionFile(string name, string contents)
            {
                Name = name;
                loadFromContent(contents);

            }
            //TODO : error transmitting
            public async Task<bool> execute()
            {
                if(Deleted)
                    return false;
                try
                {
                    await Recipe.TransformData(null);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error executing{ Name}=> {ex.Message}");
                    return false;
                }

            }

            private void MakeNextExecute()
            {
                LastRunTime = NextRunTime;
                //todo: cache this
                var expression = CronExpression.Parse(CRON, CronFormat.IncludeSeconds);
                NextRunTime = expression.GetNextOccurrence(DateTime.UtcNow);
            }
            public bool ShouldRun(DateTime currentTime)
            {
                if(Deleted)
                    return false;
                try{
                if (NextRunTime == null && LastRunTime == null)
                {
                    MakeNextExecute();
                    return true;//execute once
                }
                if (NextRunTime == null)
                {
                    return false;
                }

                if (NextRunTime < currentTime)
                {
                    MakeNextExecute();
                    return true;
                }
                return false;
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"cannot determine when to run - {ex.Message}");
                    return false;
                }
            }
        }
    }
}
