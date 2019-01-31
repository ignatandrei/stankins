using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Stankins.Interfaces;
using Stankins.Razor;
using StankinsCommon;
using StankinsObjects;

namespace Stankins.SqlServer
{
    public class ExportDBDiagramHtmlAndDot :
        BaseObjectInSerial<ReceiveMetadataFromDatabaseSql, SenderDBDiagramToDot, SenderDBDiagramHTMLDocument, TransformerConcatenateOutputString, SenderOutputToFolder>
    ,IReceive,ISender
    {
        public ExportDBDiagramHtmlAndDot(CtorDictionary dataNeeded) : base(dataNeeded)
        {

        }

        public ExportDBDiagramHtmlAndDot(string connectionString, string fileName) : this(new CtorDictionary()
        {
            {nameof(fileName),Path.GetFileName(fileName)},
            {nameof(connectionString),connectionString },
            {"connectionType",typeof(SqlConnection).FullName },
            {"newTotalNameOutput",fileName },
            {"folderToSave", Path.GetDirectoryName(fileName)},
            {"addKey",false }
 
        })
        {

        }
    }

    
}
