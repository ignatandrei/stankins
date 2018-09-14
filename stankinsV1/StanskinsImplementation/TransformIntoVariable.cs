using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StanskinsImplementation
{
    public class TransformIntoVariable : IFilterTransformToString
    {
        public TransformIntoVariable(string variableName, GroupingFunctions group,string columnName)
        {
            VariableName = variableName;
            Group = group;
            ColumnName = columnName;
            Name = $"transform {Group} {ColumnName} into {variableName}";
        }
        public IRow[] valuesToBeSent { get;set; }
        public string Result { get; set; }
        public string Name { get; set; }
        public string VariableName { get; }
        public GroupingFunctions Group { get; }
        public string ColumnName { get; }

        public async Task Run()
        {
            var arr = valuesToBeSent
                .Where(it => it.Values.ContainsKey(ColumnName))
                .Select(it => it.Values[ColumnName]);
            object val;
            switch(Group)
            {
                case GroupingFunctions.Max:
                    val = arr.Max();
                    break;
                case GroupingFunctions.Min:
                    val = arr.Max();
                    break;
                default:
                    throw new ArgumentException("do not understand group " + Group);
            }
            Result= val.ToString();
        }
    }
}
