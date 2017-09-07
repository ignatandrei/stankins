using Microsoft.CodeAnalysis.MSBuild;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverSolution
{
    public class ReceiverFromSolution : ISend
    {
        public IRow[] valuesToBeSent { get; set ; }
        public string Name { get ; set ; }
        public string SolutionFileName { get; set; }
        public ReceiverFromSolution()
        {

        }
        public async Task Send()
        {
            var rr = new Dictionary<Guid, RowReadRelation>();
            var dictAssemblyNames = new Dictionary<string, List<RowReadRelation>>();
            var msWorkspace = MSBuildWorkspace.Create();

            //var sol = msWorkspace.OpenSolutionAsync(@"D:\TFS\stankins\stankins.sln").Result;
            var sol = msWorkspace.OpenSolutionAsync(@"D:\TFS\stankins\stankins.sln").Result;
            var rrhSol = new RowReadRelation();
            rrhSol.Values.Add("Name", Path.GetFileNameWithoutExtension(sol.FilePath));
            rrhSol.Values.Add("FilePath", sol.FilePath);
            rrhSol.Values.Add("Type", sol.GetType().Name);
            rr.Add(sol.Id.Id, rrhSol);


            var projectGraph = sol.GetProjectDependencyGraph();
            var q = projectGraph.GetTopologicallySortedProjects();

            foreach (var projectId in q.ToArray())
            {
                var project = sol.GetProject(projectId);
                var rrProject = new RowReadRelation();
                rrProject.Values.Add("ID", projectId.Id);
                rrProject.Values.Add("Name", project.Name);
                rrProject.Values.Add("FilePath", project.FilePath);
                rrProject.Values.Add("Type", project.GetType().Name);

                rrProject.Add("Solution", rrhSol);
                rr.Add(projectId.Id, rrProject);
                var refProjects = projectGraph.GetProjectsThatThisProjectDirectlyDependsOn(projectId);
                var listRefProjects = new List<IRowReceiveRelation>();
                rrProject.Relations.Add("referencedIn", listRefProjects);
                foreach (var item in refProjects)
                {
                    var refItem = rr[item.Id];
                    listRefProjects.Add(refItem);

                }

                var c = project.GetCompilationAsync().Result;
                var listAssemblyReferenced = new List<IRowReceiveRelation>();
                rrProject.Relations.Add("assemblyReferenced", listAssemblyReferenced);
                var refAssembly = c.ReferencedAssemblyNames.ToArray();
                foreach (var item in refAssembly)
                {
                    if (!dictAssemblyNames.ContainsKey(item.Name))
                    {
                        dictAssemblyNames.Add(item.Name, new List<RowReadRelation>());

                    }

                    var list = dictAssemblyNames[item.Name];
                    var rrAssFound = list.FirstOrDefault(it => it.Values["Version"].ToString() == item.Version.ToString());
                    if (rrAssFound == null)
                    {
                        rrAssFound = new RowReadRelation();
                        rrAssFound.Values.Add("Name", item.Name);
                        rrAssFound.Values.Add("DisplayNameToken", item.GetDisplayName(true));
                        rrAssFound.Values.Add("DisplayName", item.GetDisplayName(false));
                        rrAssFound.Values.Add("Version", item.Version.ToString());
                        list.Add(rrAssFound);
                    }
                    listAssemblyReferenced.Add(rrAssFound);
                }
                //var x = project.MetadataReferences.ToArray();
                //var y = x.Length;
                valuesToBeSent = new IRow[] { rrhSol };
            }
        }
    }
}
