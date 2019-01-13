using System.Collections.Generic;

namespace Stankins.AzureDevOps
{
    public class JobYaml
    {
        public JobYaml()
        {
            DependsOn = new List<string>();
            Steps = new List<Step>();
        }
        public string condition;
        public List<string> DependsOn;
        public KeyValuePair<string, string> pool;
        public List<Step> Steps;
        public string Name { get; set; }
    }

}
