using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;

namespace Stankins.AnalyzeSolution
{
    public class ReceiverFromSolution : BaseObject, IReceive
    {
        private readonly string fileNameSln;

        public ReceiverFromSolution(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.fileNameSln = GetMyDataOrThrow<string>(nameof(fileNameSln));
            this.Name = nameof(ReceiverFromSolution);
        }
        public ReceiverFromSolution(string fileNameSln) : this(new CtorDictionary()
        {
            {nameof(fileNameSln),fileNameSln }
            
        })
        {
           
        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (!File.Exists(fileNameSln))
                throw new FileNotFoundException(fileNameSln);

            if (receiveData == null)
            {
                receiveData=new DataToSentTable();
            }
            var manager = new AnalyzerManager(fileNameSln);
            var sol = manager.GetWorkspace().CurrentSolution;
            
            var dtSolution = new DataTable();
            dtSolution.TableName = "solutions";
           
            dtSolution.Columns.Add("Name", typeof(string));
            dtSolution.Columns.Add("FilePath", typeof(string));
            dtSolution.Columns.Add("Type", typeof(string));

            dtSolution.Rows.Add(Path.GetFileNameWithoutExtension(fileNameSln), fileNameSln);

            var dtProjects = new DataTable();
            dtProjects.TableName = "projects";
            dtProjects.Columns.Add("ID", typeof(string));
            dtProjects.Columns.Add("SolutionFilePath", typeof(string));
            dtProjects.Columns.Add("Name", typeof(string));
            dtProjects.Columns.Add("FilePath", typeof(string));
            dtProjects.Columns.Add("Type", typeof(string));


            var dtRelationProject=new DataTable();
            dtRelationProject.TableName = "projectReferences";
            dtRelationProject.Columns.Add("PrjId", typeof(string));
            dtRelationProject.Columns.Add("RefPrjId", typeof(string));

            var dtAssemblies= new DataTable();
            dtAssemblies.TableName = "assemblies";
            dtAssemblies.Columns.Add("Name", typeof(string));
            dtAssemblies.Columns.Add("DisplayNameToken", typeof(string));
            dtAssemblies.Columns.Add("DisplayName", typeof(string));
            dtAssemblies.Columns.Add("Version", typeof(string));
            
            var dtRelProjectAssemblies=new DataTable();
            dtRelProjectAssemblies.TableName = "projectAssemblies";
            dtRelProjectAssemblies.Columns.Add("PrjId", typeof(string));
            dtRelProjectAssemblies.Columns.Add("DisplayName", typeof(string));

            var existsAssemblies = new List<string>();
            FastAddTables(receiveData, dtSolution, dtProjects, dtRelationProject, dtAssemblies, dtRelProjectAssemblies);
            foreach (var prj in sol.Projects)
            {
                var id = prj.Id.Id.ToString("N");
                dtProjects.Rows.Add(
                    id,
                    fileNameSln,
                    prj.Name,
                    prj.FilePath,
                    prj.GetType().Name
                );
                foreach (var prjProjectReference in prj.ProjectReferences)
                {
                    
                    dtRelationProject.Rows.Add(id, prjProjectReference.ProjectId.Id.ToString("N"));

                }
                
                var c = await prj.GetCompilationAsync();
                foreach (var referencedAssemblyName in c.ReferencedAssemblyNames)
                {
                    var name = referencedAssemblyName.GetDisplayName(true);
                    if (!existsAssemblies.Contains(name))
                    {

                        existsAssemblies.Add(name);
                        dtAssemblies.Rows.Add(referencedAssemblyName.Name,
                            referencedAssemblyName.GetDisplayName(false),
                            name,
                            referencedAssemblyName.Version.ToString());
                    }

                    dtRelProjectAssemblies.Rows.Add(id, name);
                }
                //TODO: project.MetadataReferences

            }

            return receiveData;
        }

        public override async Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
