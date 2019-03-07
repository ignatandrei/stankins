using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace Stankins.AzureDevOps
{
    public class YamlDevOpsVisitor : IYamlVisitor
    {
        public void LoadFromString(string data)
        {
            var st = new StringReader(data);
            var yaml = new YamlStream();
            yaml.Load(st);
            var rt = yaml.Documents[0].RootNode;
            rt.Accept(this);
        }
        public YamlDevOpsVisitor()
        {
            jobs = new List<JobYaml>();
        }
        public List<JobYaml> jobs { get; set; }
        public void Visit(YamlStream stream)
        {
            Console.WriteLine("stream");
        }

        public void Visit(YamlDocument document)
        {
            Console.WriteLine("document");
        }

        public void Visit(YamlScalarNode scalar)
        {
            
        }

        public void Visit(YamlSequenceNode sequence)
        {
           
            foreach (var item in sequence.Children)
            {
                item.Accept(this);
            }
        }
        private JobYaml LastJob()
        {
            return this.jobs.LastOrDefault();
        }
        public void Visit(YamlMappingNode mapping)
        {

            foreach (var item in mapping.Children)
            {
                if (item.Key.NodeType == YamlNodeType.Scalar)
                {
                    var sc = item.Key as YamlScalarNode;
                    switch (sc.Value)
                    {
                        case "variables":
                            {
                                //TODO: variables
                                var job = LastJob();
                                if (job == null)
                                {
                                    //TODO: is the yaml name
                                    continue;
                                }
                                else
                                {
                                }
                            }
                            continue;
                        case "inputs":
                            {
                                var seq = item.Value as YamlMappingNode;
                                var t = LastJob().Steps.Last() as TaskYaml;
                                foreach (var inp in seq.Children)
                                {
                                    t.Inputs.Add(new KeyValuePair<string, string>(inp.Key.ToString(), inp.Value.ToString()));
                                }
                            }
                            continue;
                        case "displayName":
                        case "name":
                            {
                                var c = item.Value as YamlScalarNode;
                                var job = LastJob();
                                if (job == null)
                                {
                                    //TODO: is the yaml name
                                    continue;
                                }
                                else
                                {
                                    var step = job.Steps.LastOrDefault();
                                    if (step != null)
                                    {
                                        step.DisplayName = c.Value;
                                    }
                                    else
                                    {
                                        Console.WriteLine("NOT FOUND STEP FOR " + c.Value);
                                    }
                                }
                            }
                            continue;
                        case "jobs":
                            item.Value.Accept(this);
                            continue;
                        case "job":
                            var j = new JobYaml();
                            var n = item.Value as YamlScalarNode;
                            j.Name = n.Value;
                            this.jobs.Add(j);
                            item.Value.Accept(this);
                            continue;
                        case "condition":
                            var s = item.Value as YamlScalarNode;
                            LastJob().condition = s.Value;
                            continue;
                        case "pool":
                            {
                                var name = item.Value as YamlMappingNode;
                                var vmimage = name.Children.First();
                                var l = LastJob();
                                if (l == null)
                                {
                                    this.jobs.Add(new JobYaml());
                                }
                                LastJob().pool = new KeyValuePair<string, string>(vmimage.Key.ToString(), vmimage.Value.ToString());
                            }

                            continue;
                        case "steps":
                            var steps = item.Value as YamlSequenceNode;
                            steps.Accept(this);
                            continue;
                        case "checkout":
                            {
                                var step = item.Value as YamlScalarNode;
                                LastJob().Steps.Add(new Checkout(step.Value));
                            }
                            continue;
                        case "powershell":
                            {
                                var step = item.Value as YamlScalarNode;
                                LastJob().Steps.Add(new Powershell(step.Value));
                            }
                            continue;
                        case "bash":
                            {
                                var step = item.Value as YamlScalarNode;
                                LastJob().Steps.Add(new Bash(step.Value));
                            }
                            continue;
                        case "script":
                            {
                                var step = item.Value as YamlScalarNode;
                                LastJob().Steps.Add(new Script(step.Value));
                            }
                            continue;
                        case "task":
                            {
                                var step = item.Value as YamlScalarNode;
                                var t = new TaskYaml(step.Value);
                                LastJob().Steps.Add(t);
                                

                            }
                            continue;
                        case "dependsOn":
                            {
                                var l = LastJob();
                                var d = item.Value as YamlScalarNode;
                                if (d != null)
                                {
                                    l.DependsOn.Add(d.Value);
                                    continue;
                                }
                                var seq = item.Value as YamlSequenceNode;
                                foreach (var child in seq.Children)
                                {
                                    d = child as YamlScalarNode;
                                    l.DependsOn.Add(d.Value);
                                }
                            }
                            continue;
                        //foreach(var step in steps.Children)
                        //{
                        //    switch (step.NodeType)
                        //    {
                        //        case YamlNodeType.Mapping:

                        //            continue;

                        //        default:
                        //            Console.WriteLine("can not interpret step child " + step.NodeType);
                        //            continue;
                        //    }
                        //}
                        //continue;
                        default:
                            Console.WriteLine($" scalar not handled {sc.Value}");
                            continue;

                    }
                }
                Console.WriteLine("mapping");

            }
        }
    }

}
