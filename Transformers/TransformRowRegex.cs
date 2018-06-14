using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace Transformers
{
    public static class RegexExtensions
    {
        public static string Replace(this string input, Regex regex, string groupName, string replacement)
        {
            return regex.Replace(input, m =>
            {
                return ReplaceNamedGroup(input, groupName, replacement, m);
            });
        }

        private static string ReplaceNamedGroup(string input, string groupName, string replacement, Match m)
        {
            string capture = m.Value;
            capture = capture.Remove(m.Groups[groupName].Index - m.Index, m.Groups[groupName].Length);
            capture = capture.Insert(m.Groups[groupName].Index - m.Index, replacement);
            return capture;
        }
    }
    public class TransformRowRegexReplaceGuid : TransformRowRegex
    {

        public TransformRowRegexReplaceGuid(string regexWithGroups, string key):base(regexWithGroups,key)
        {
            RegexWithGroups = regexWithGroups;
            Key = key;
            Name = $"Regex for field {key} to replace group names as fields";
        }
        public bool ReplaceAllNextOccurences;
        Dictionary<string, string> namesAndReplace = new Dictionary<string, string>();

        public override async Task Run()
        {
        
            foreach (var data in FindValues())
            {
                var item = data.Item1;
                var groups = data.Item2;
                var value = item.Values[Key].ToString();
                foreach (string groupName in regex.GetGroupNames())
                {
                    if (int.TryParse(groupName, out var _))
                        continue;

                    if (groups[groupName].Captures.Count > 0)
                    {
                        if (!namesAndReplace.ContainsKey(groupName))
                        {
                            var newVal = Guid.NewGuid().ToString("N");
                            namesAndReplace[groups[groupName].Captures[0].Value] = newVal;
                            value = value.Replace(regex, groupName, newVal);  
                        }
                        
                    }
                   
                }
                if (ReplaceAllNextOccurences)
                {
                    foreach(var itemDict in namesAndReplace)
                    {
                        value = value.Replace(itemDict.Key, itemDict.Value);
                    }
                }
                item.Values[Key] = value;
            }

            valuesTransformed = valuesRead;
        }
    }
    public class TransformRowRegex : ITransform
    {
        public TransformRowRegex(string regexWithGroups,string key)
        {
            RegexWithGroups = regexWithGroups;
            Key = key;
            Name = $"Regex for field {key} to add group names as fields";
            regex = new Regex(RegexWithGroups);
        }
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get ; set ; }
        public string Name { get ; set ; }

        protected Regex regex;

        public string RegexWithGroups { get; set; }
        public string Key { get; set; }
        public async virtual Task Run()
        {
            foreach (var data in FindValues())
            {
                var item = data.Item1;
                var groups = data.Item2;

                foreach (string g in regex.GetGroupNames())
                {
                    var value = groups[g].Value;
                    if (!string.IsNullOrWhiteSpace(value))
                        item.Values.Add(g, value);
                }
            }

            valuesTransformed = valuesRead;
        }
        protected IEnumerable< ValueTuple<IRow, GroupCollection>> FindValues ()
        {
            
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
                var matching = regex.Match(input);
                var groups = regex.Match(input).Groups;
                var v = ValueTuple.Create(item, groups);
                yield return v;

            }
            yield break;
        }
    }
}
