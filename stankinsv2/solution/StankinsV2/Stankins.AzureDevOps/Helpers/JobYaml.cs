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
        public string condition { get; set; }
        public List<string> DependsOn { get; set; }
        public KeyValuePair<string, string> pool { get; set; }
        public List<Step> Steps { get; set; }
        public string Name { get; set; }
    }

}
