using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Transformers
{
    public class FilterComparableFromSerializable : IFilter
    {
        public FilterComparableFromSerializable(FilterComparable fc, ISerializeData serializeData)
        {
            Fc = fc;
            SerializeData = serializeData;            
        }

        public FilterComparable Fc { get; set; }
        public ISerializeData SerializeData { get; set; }
        public string Key { get; set; }
        public IRow[] valuesRead { get => ((IFilter)Fc).valuesRead; set => ((IFilter)Fc).valuesRead = value; }
        public IRow[] valuesTransformed { get => ((IFilter)Fc).valuesTransformed; set => ((IFilter)Fc).valuesTransformed = value; }
        public string Name { get =>"Serializable "+ ((IFilter)Fc).Name; set => ((IFilter)Fc).Name = value.Replace("Serializable ",""); }

        public async Task Run()
        {
            var val = SerializeData.GetValue(Fc.FieldName);
            Fc.Value = val;
            await Fc.Run();
        }
    }
    public class FilterComparable : TypeConverter, IFilter

    {
        public string Name { get; set; }
        public IRow[] valuesRead { get; set ; }
        public IRow[] valuesTransformed { get ; set ; }
        public Type ComparableType { get; set; }
        public object Value { get; set; }
        public string FieldName { get; set; }
        public CompareValues HowToCompareValues { get; set; }

        public FilterComparable(Type comparableType, object value, string fieldName, CompareValues compareValues)
        {
            ComparableType = comparableType;
            Value = value;
            FieldName = fieldName;
            HowToCompareValues = compareValues;
            this.Name = $"{fieldName} {compareValues} {value ?? ""}";
        }
        

        public FilterComparable()
        {
        }

        public virtual async Task Run()
        {
            var returnValues = new List<IRow>();
            if(Value == null)
            {
                returnValues.AddRange(valuesRead);
                valuesTransformed = returnValues.ToArray();
                return;
            }
            var original = Convert.ChangeType(Value, ComparableType);
            IComparable v = (IComparable)original;
            foreach (var item in valuesRead)
            {
                if (!item.Values.ContainsKey(FieldName))
                {
                    //TODO:log
                    continue; 
                }
                var val = item.Values[FieldName];
                var valueLoop = Convert.ChangeType(val, ComparableType);
               
                var res = (v.CompareTo(valueLoop));
                bool add = false;
                switch (HowToCompareValues)
                {
                    case CompareValues.Equal:
                        add = (res == 0);
                        break;
                    case CompareValues.Less:
                        add = (res > 0);
                        break;
                    case CompareValues.LessOrEqual:
                        add = (res >= 0);
                        break;
                    case CompareValues.Greater:
                        add = (res < 0);
                        break;
                    case CompareValues.GreaterOrEqual:
                        add = (res <= 0);
                        break;
                    default:
                        throw new ArgumentException("This is not implemented :" + HowToCompareValues);
                }
                if (!add)
                    continue;
                returnValues.Add(item);
                

            }
            valuesTransformed = returnValues.ToArray();

            await Task.CompletedTask;
        }

        // Deserialization (ex. "int ColumnA >= 3000"). The separator is a single space/' '. 0 = type, 1 = fieldName, 2 = operator, 3 = value.
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if(value is string)
            {
                string[] tokens = ((string)value).Split(' ');
                Type filterDataType = Type.GetType(tokens[0]);
                string filterFieldName = tokens[1];
                string filterOperator = tokens[2];
                object filterValue = TypeDescriptor.GetConverter(filterDataType).ConvertFrom(context, culture, tokens[3]);

                FilterComparable filter = null;
                switch(filterOperator)
                {
                    case "=":
                        filter = new FilterComparableEqual(filterDataType, filterValue, filterFieldName);
                        break;
                    case "<":
                        filter = new FilterComparableLess(filterDataType, filterValue, filterFieldName);
                        break;
                    case "<=":
                        filter = new FilterComparableLessOrEqual(filterDataType, filterValue, filterFieldName);
                        break;
                    case ">":
                        filter = new FilterComparableGreat(filterDataType, filterValue, filterFieldName);
                        break;
                    case ">=":
                        filter = new FilterComparableGreaterOrEqual(filterDataType, filterValue, filterFieldName);
                        break;
                    default:
                        throw new ArgumentException("This is not implemented :" + filterOperator);
                }
                return filter;
            }
            return base.ConvertFrom(context, culture, value);
        }
        // Serialization
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if( destinationType == typeof(string) )
            {
                var filter = (FilterComparable)value;
                string filterOperator = string.Empty;
                switch (filter.HowToCompareValues)
                {
                    case CompareValues.Equal:
                        filterOperator = "=";
                        break;
                    case CompareValues.Less:
                        filterOperator = "<";
                        break;
                    case CompareValues.LessOrEqual:
                        filterOperator = "<=";
                        break;
                    case CompareValues.Greater:
                        filterOperator = ">";
                        break;
                    case CompareValues.GreaterOrEqual:
                        filterOperator = ">=";
                        break;
                    default:
                        throw new ArgumentException("This is not implemented :" + filter.HowToCompareValues);
                }
                return $"{filter.ComparableType} {filter.FieldName} {filterOperator} {filter.Value}";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
