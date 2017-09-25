using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace Transformers
{
    public class TransformRowRegex : ITransform
    {
        public TransformRowRegex(string regexWithGroups,string key)
        {
            RegexWithGroups = regexWithGroups;
            Key = key;
            Name = $"Regex for field {key} to add group names as fields";
        }
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get ; set ; }
        public string Name { get ; set ; }
        public string RegexWithGroups { get; set; }
        public string Key { get; set; }

        public async Task Run()
        {
            Regex regex = new Regex(RegexWithGroups);
            foreach (var item in valuesRead)
            {
                if (!item.Values.ContainsKey(Key))
                {
                    string message = $"values not contain {Key}";
                    //@class.Log(LogLevel.Information, 0, $"transformer row regex: {message}", null, null);                        
                    message += "";
                    continue;
                }
                var val = item.Values[Key];
                if (val == null)
                {
                    string message = $"val is null for {Key}";
                    //@class.Log(LogLevel.Information, 0, $"transformer row regex: {message}", null, null);                        
                    message += "";
                    continue;
                }
                var input = val.ToString();
                //string regex = @"^Date:\ (?<date>.{23}).*?$";

                //string input = @"Date: 2017/07/31 18:20:52.309: Log file for rdc-16 created.";
                var groups = regex.Match(input).Groups;
                
                foreach (string g in regex.GetGroupNames())
                {
                    
                    var value = groups[g].Value;
                    if(!string.IsNullOrWhiteSpace(value))
                        item.Values.Add(g, value);
                }

            }
            valuesTransformed = valuesRead;
        }
    }
}
