using Stankins.AnalyzeSolution;
using Stankins.Interfaces;
using Stankins.Razor;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Stankins.SimpleRecipes
{
    public class ExportSolutionToHtmlAndDot :
        BaseObjectInSerial<ReceiverFromSolution, SenderSolutionToDot, SenderSolutionToHTMLDocument, TransformerConcatenateOutputString, SenderOutputToFolder>
    , IReceive, ISender
    {
        public ExportSolutionToHtmlAndDot(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ExportSolutionToHtmlAndDot);
        }

        public ExportSolutionToHtmlAndDot(string fileNameSln, string fileName) : this(new CtorDictionary()
        {
            {nameof(fileName),Path.GetFileName(fileName)},
            {nameof(fileNameSln),fileNameSln },
            {"newTotalNameOutput",fileName },
            {"folderToSave", Path.GetDirectoryName(fileName)},
            {"addKey",false }

        })
        {

        }
    }
}