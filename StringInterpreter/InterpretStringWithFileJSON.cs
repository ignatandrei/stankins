using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        static void GetAllAssemblies(Assembly start)
        {
            
            AllAssemblies.Add(start.FullName, start);
            foreach(var item in start.GetReferencedAssemblies())
            {
                try
                {
                    if (AllAssemblies.ContainsKey(item.FullName))
                        continue;

                    var ass = Assembly.Load(item);
                    GetAllAssemblies(ass);
                    
                }
                catch (Exception)
                {
                    //TODO:log
                    
                }
            }
           
        }
        static Dictionary<string,Assembly> AllAssemblies;
        void InterpretStatic(List<ValuesToTranslate> values)
        {
            var length = (values?.Count ?? 0);
            if (length == 0)
                return;
            //TODO : make thread safe
            if(AllAssemblies== null)
            {
                AllAssemblies = new Dictionary<string, Assembly>();
                GetAllAssemblies(Assembly.GetEntryAssembly());
            }
            var types = AllAssemblies.Select(it=>it.Value).SelectMany(it => it.ExportedTypes);
            foreach(var item in values)
            {
                //Assembly.GetEntryAssembly().GetReferencedAssemblies().First().
                var lastDot = item.ValueToTranslate.LastIndexOf(".");
                var nameType = item.ValueToTranslate.Substring(0, lastDot );
                var nameMethod = item.ValueToTranslate.Substring(lastDot+1);
                var type = types.FirstOrDefault(it => it.FullName == nameType);
                if(type == null)
                {
                    type = types.FirstOrDefault(it => it.Name == nameType);
                }
                if (type == null)
                {
                    throw new ArgumentException($"Can not find {nameType}");
                }
                MethodInfo mi;
                if (item.ValueToTranslate.Last() == ')')//function
                {
                    mi = type.GetRuntimeMethod(nameMethod.Replace("()", ""),new Type[0]);

                }
                else//assuming property ., not field
                {
                    mi = type.GetRuntimeProperty(nameMethod).GetGetMethod();
                }
                var res = mi.Invoke(null, null);
                item.ValueTranslated = res?.ToString();
            }
        }

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
        public bool TwoSlashes = true;
        public string InterpretText(string text)
        {
            var env = new List<ValuesToTranslate>();
            var appSettings = new List<ValuesToTranslate>();
            var expressions = new List<ValuesToTranslate>();
            var staticClass = new List<ValuesToTranslate>();
            var options = RegexOptions.Multiline | RegexOptions.Singleline;
            //# separator
            //string regex = @"^.+?\#(?<myExpression>.+?)\#.+?$";
            //string regex = @"\#(?=<myExpression>.*)\#";
            //var matches = Regex.Matches(text, regex, options);
            //if (matches?.Count == 0)
            //    return text;
            //while (matches.Success)
            //{
            //    matchObj = regexObj.Match(subjectString, matchObj.Index + 1);
            //}

            //foreach (Match match in matches)
            var str = @".*?\#(?<myExpression>.+?)\#.*?";            

            Regex regexObj = new Regex(str);
            Match matchObj = regexObj.Match(text);
            while (matchObj.Success)
            {
                //Console.WriteLine(matchObj.Groups["myExpression"]);

                var toInterpret = matchObj.Groups["myExpression"].Value;

                matchObj = regexObj.Match(text, matchObj.Groups["myExpression"].Index + 1+ toInterpret.Length);
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
                    case "static":
                        staticClass.Add(p);
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
            if (staticClass.Count > 0)
            {
                InterpretStatic(staticClass);
            }
            Func<string, string> replace = (it) =>
             {
                 if (!TwoSlashes)
                     return it;
                 return it.Replace(@"\", @"\\");
             };
            //TODO: use Regex.Replace instead of this...
            var sb = new StringBuilder(text);
            foreach (var item in env)
            {
                sb.Replace($"#env:{item.ValueToTranslate}#", replace(item.ValueTranslated));
            }
            foreach (var item in appSettings)
            {
                sb.Replace($"#file:{item.ValueToTranslate}#", replace(item.ValueTranslated));
            }
            
            foreach (var item in staticClass)
            {
                sb.Replace($"#static:{item.ValueToTranslate}#", replace(item.ValueTranslated));
            }
            foreach (var item in expressions)
            {
                sb.Replace($"#{item.ValueToTranslate}#", replace(item.ValueTranslated));
            }
            return sb.ToString();
            

        }
    }
}
