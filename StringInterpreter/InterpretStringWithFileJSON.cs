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
    /// <summary>
    /// TODO example with GUID: SendTo#static:Guid.NewGuid():N#.csv
    /// TODO now and utc now
    /// test , code coverage
    /// </summary>
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
                
                var beginFunction = item.ValueToTranslate.LastIndexOf(".");
                var indexBegin = item.ValueToTranslate.LastIndexOf("(");
                if(indexBegin>-1 && indexBegin < beginFunction)
                {
                    beginFunction = item.ValueToTranslate.LastIndexOf(".",beginFunction-1);

                }
                var nameType = item.ValueToTranslate.Substring(0, beginFunction );
                
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
                string argMethod = null;
                var nameMethod = item.ValueToTranslate.Substring(beginFunction + 1);
                if (item.ValueToTranslate.Last() == ')')//function
                {
                    //find if have 1 argument
                    //TODO: resolve with argument not string
                    //TODO: resolve with multiple parameters
                    var len = item.ValueToTranslate.Length;
                    
                    if (indexBegin==len-2)
                    {
                        mi = type.GetRuntimeMethod(nameMethod.Replace("()", ""), new Type[0]);
                    }
                    else
                    {
                        argMethod = item.ValueToTranslate.Substring(indexBegin+1).Replace(")","");
                        nameMethod = nameMethod.Substring(0, nameMethod.Length - argMethod.Length - 2);
                        mi = type.GetRuntimeMethod(nameMethod,new Type[1] { typeof(string) }).GetRuntimeBaseDefinition();
                        
                    }
                }
                else//assuming property ., not field
                {
                    mi = type.GetRuntimeProperty(nameMethod).GetGetMethod();
                }
                object res;
                if (argMethod == null)
                {
                    res = mi.Invoke(null, null);
                }
                else
                {
                    res = mi.Invoke(null, new string[1]{ argMethod });
                }
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
                string val = values[i].ValueToTranslate;
                if (!keys.ContainsKey(val))
                {
                    throw new ArgumentException($"Environment does not contain {val}");
                }
                values[i].ValueTranslated = keys[val];
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
                string val = values[i].ValueToTranslate;

                try
                {
                    values[i].ValueTranslated = config[val];
                }
                catch(Exception ex)
                {
                    throw new ArgumentException($"config {filePath} does not contain {val}",ex);
                }
            }
            
                
            
        }
        public bool TwoSlashes = true;
        public string InterpretText(string text)
        {
            var data = InterpretText(text, '#');
            return InterpretText(data, '@');//Old separator @ it's already used to map SQL sp' parameters thus @param1=column1...
        }
        string InterpretText(string text, char special)
        {
            var env = new List<ValuesToTranslate>();
            var appSettings = new List<ValuesToTranslate>();
            var expressions = new List<ValuesToTranslate>();
            var staticClass = new List<ValuesToTranslate>();            
            
            var str = @".*?\"+ special+ @"(?<myExpression>.+?)\"+ special+".*?";            

            Regex regexObj = new Regex(str);
            Match matchObj = regexObj.Match(text);
            while (matchObj.Success)
            {
                //Console.WriteLine(matchObj.Groups["myExpression"]);

                var toInterpret = matchObj.Groups["myExpression"].Value;

                matchObj = regexObj.Match(text, matchObj.Groups["myExpression"].Index + 1+ toInterpret.Length);
                //two special means special char
                if (toInterpret[0]==special)
                    continue;

                var kv = toInterpret.Split(':');
                if(kv?.Length == 1 && (Regex.Match(toInterpret, @"[a-zA-Z0-9_]*\=[a-zA-Z0-9_]*\;").Success || Regex.Match(toInterpret, @"[a-zA-Z0-9_]*\=.[a-zA-Z0-9_]*").Success))
                {
                    continue;
                }
                if(kv?.Length > 2)
                {
                    int i = 1;
                    string s = "";
                    while (i < kv.Length)
                    {
                        s += kv[i++]+ ":";
                    }
                    kv[1] = s.Substring(0,s.Length-1);
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
                    case "guid":
                        expressions.Add(new ValuesToTranslate()
                        {
                            ValueToTranslate = toInterpret,
                            ValueTranslated = (kv.Length == 2) ? Guid.NewGuid().ToString(kv[1]):Guid.NewGuid().ToString()
                        });
                        break;

                    case "utcnow":
                        expressions.Add(new ValuesToTranslate()
                        {
                            ValueToTranslate = toInterpret,
                            ValueTranslated = (kv.Length == 2) ? DateTime.Now.ToString(kv[1]) : DateTime.Now.ToString()
                        });
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
                sb.Replace($"{special}env:{item.ValueToTranslate}{special}", replace(item.ValueTranslated));
            }
            foreach (var item in appSettings)
            {
                sb.Replace($"{special}file:{item.ValueToTranslate}{special}", replace(item.ValueTranslated));
            }
            
            foreach (var item in staticClass)
            {
                sb.Replace($"{special}static:{item.ValueToTranslate}{special}", replace(item.ValueTranslated));
            }
            foreach (var item in expressions)
            {
                sb.Replace($"{special}{item.ValueToTranslate}{special}", replace(item.ValueTranslated));
            }
            return sb.ToString();
            

        }
    }
}
