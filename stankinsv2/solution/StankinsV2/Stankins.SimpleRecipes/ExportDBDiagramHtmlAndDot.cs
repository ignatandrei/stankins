using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Stankins.Interfaces;
using Stankins.Razor;
using Stankins.SqlServer;
using StankinsCommon;
using StankinsObjects;

namespace Stankins.SimpleRecipes
{
    public class ExportDBDiagramHtmlAndDot :
        BaseObjectInSerial<ReceiveMetadataFromDatabaseSql, SenderDBDiagramToDot, SenderDBDiagramHTMLDocument, TransformerConcatenateOutputString, SenderOutputToFolder>
    , IReceive, ISender
    {
        public ExportDBDiagramHtmlAndDot(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ExportDBDiagramHtmlAndDot);
        }

        public ExportDBDiagramHtmlAndDot(string connectionString, string fileName) : this(new CtorDictionary()
        {
            {nameof(fileName),Path.GetFileName(fileName)},
            {nameof(connectionString),connectionString },
            //{"connectionType",typeof(SqlConnection).FullName },
            {"newTotalNameOutput",fileName },
            {"folderToSave", Path.GetDirectoryName(fileName)},
            {"addKey",false }

        })
        {

        }
    }
}
