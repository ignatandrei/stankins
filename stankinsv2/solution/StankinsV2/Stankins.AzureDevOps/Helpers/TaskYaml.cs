using System.Collections.Generic;

namespace Stankins.AzureDevOps
{
    public class TaskYaml : Step
    {
        public TaskYaml(string value)
        {
            Name = "task";
            Value = value;
            Inputs = new List<KeyValuePair<string, string>>();
        }
        public List<KeyValuePair<string, string>> Inputs { get; set; }
    }

}
