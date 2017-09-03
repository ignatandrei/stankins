using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace StringInterpreter
{
    class ValuesToTranslate
    {
        public string ValueToTranslate { get; set; }
        public string ValueTranslated { get; set; }
    }
    public class Interpret
    {
        void InterpretStringWithEnvironment( List<ValuesToTranslate> values)
        {
            var length = (values?.Count ?? 0);
            if (length == 0)
                return;
            var dict = Environment.GetEnvironmentVariables();
            var keys = new Dictionary<string, string>();
            foreach(var item in dict)
            {
                DictionaryEntry de;
                try
                {
                    de = (DictionaryEntry)item;
                }
                catch
                {
                    //do nothing if it is not DictionaryEntry
                    continue;
                }
                keys[de.Key.ToString()] = de.Value?.ToString();
                
            }
            for (int i = 0; i < length; i++)
            {
                values[i].ValueTranslated = keys[values[i].ValueToTranslate];
            }


        }
        void InterpretStringWithFileJSON(string filePath, List<ValuesToTranslate>  values)
        {
            var length = (values?.Count??0);
            if (length ==0)
                return;

            var builder = new ConfigurationBuilder()
                        .AddJsonFile(filePath);
            var config = builder.Build();
            
            for (int i = 0; i < length; i++)
            {

                values[i].ValueTranslated = config[values[i].ValueToTranslate];
            }
            
                
            
        }
        
        public string InterpretText(string text)
        {
            var env = new List<ValuesToTranslate>();
            var appSettings = new List<ValuesToTranslate>();
            var expressions = new List<ValuesToTranslate>();
            var options = RegexOptions.Multiline ;
            //# separator
            string regex = @"^.+?\#(?<myExpression>.+?)\#.+?$";
            var matches = Regex.Matches(text+Environment.NewLine, regex, options);
            if (matches?.Count == 0)
                return text;
            
            foreach (Match match in matches)
            {
                var toInterpret = match.Groups["myExpression"].Value;                
                //two # means special char
                if (toInterpret.StartsWith("#"))
                    continue;
                var kv = toInterpret.Split(':');
                if(kv?.Length !=2)
                {
                    throw new ArgumentException("interpret " + toInterpret + " has not 2 items separated by :");
                }
                var p= new ValuesToTranslate() { ValueToTranslate= kv[1] };
                switch (kv[0].ToLowerInvariant())
                {
                    case "file":
                        appSettings.Add(p);
                        break;
                    case "env":
                        env.Add(p);
                        break;
                    case "now":
                        expressions.Add(new ValuesToTranslate()
                        {
                            ValueToTranslate = toInterpret,
                            ValueTranslated = DateTime.Now.ToString(kv[1])
                        });
                        break;
                    default:
                        throw new ArgumentException("do not interpret " +kv[0]);
                }
            }
            if (appSettings.Count > 0)
            {
                var file = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                InterpretStringWithFileJSON(file, appSettings);

            }
            if (env.Count > 0)
            {
                InterpretStringWithEnvironment(env);
            }
            //TODO: use Regex.Replace instead of this...
            var sb = new StringBuilder(text);
            foreach (var item in env)
            {
                sb.Replace($"#env:{item.ValueToTranslate}#", item.ValueTranslated);
            }
            foreach (var item in appSettings)
            {
                sb.Replace($"#file:{item.ValueToTranslate}#", item.ValueTranslated);
            }
            foreach (var item in expressions)
            {
                sb.Replace($"#{item.ValueToTranslate}#", item.ValueTranslated);
            }

            return sb.ToString();
            

        }
    }
}
