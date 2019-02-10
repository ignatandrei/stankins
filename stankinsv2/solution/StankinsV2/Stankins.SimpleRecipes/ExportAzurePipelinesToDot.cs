using Stankins.AzureDevOps;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Stankins.SimpleRecipes
{
    public class ExportAzurePipelinesToDot :BaseObjectInSerial<YamlReader, SenderYamlAzurePipelineToDot, TransformerConcatenateOutputString, SenderOutputToFolder>
    , IReceive, ISender
    {
        public ExportAzurePipelinesToDot(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ExportAzurePipelinesToDot);
        }

        public ExportAzurePipelinesToDot(string fileNameYaml, string fileName) : this(new CtorDictionary()
        {
            {nameof(fileName),Path.GetFileName(fileName)},
            {nameof(fileNameYaml),fileNameYaml },
            {"newTotalNameOutput",fileName },
            {"folderToSave", Path.GetDirectoryName(fileName)},
            {"addKey",false }

        })
        {

        }
    }
}
