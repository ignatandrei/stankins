using StankinsInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;

namespace CommonDB
{
    /// <summary>
    /// see also https://github.com/mgravell/fast-member/blob/master/FastMember/ObjectReader.cs
    /// </summary>    
    public class RowDataReader : DbDataReader
    {
        
        int nrRecord;
        private IDictionary<string, int> _propertyNameToOrdinal = new Dictionary<string, int>();
        private IDictionary<int, string> _ordinalToPropertyName = new Dictionary<int, string>();
       
        public RowDataReader(IRow[] enumerableRows, params string[] properties)
        {
            
            EnumerableRows = enumerableRows;
            Properties = properties;
            nrRecord = -1;//limbo until read first time!
            Initialize();
        }
        

        private void Initialize()
        {
            int ordinal = 0;
            var properties = Properties;
            foreach (var property in properties)
            {
                string propertyName = property;
                _propertyNameToOrdinal.Add(propertyName, ordinal);
                _ordinalToPropertyName.Add(ordinal, propertyName);

                
                
                ordinal++;
            }
        }

        public override object this[int ordinal]
        {
            get
            {
                return GetValue(ordinal);
            }
        }

        public override object this[string name]
        {
            get
            {
                return GetValue(GetOrdinal(name));
            }
        }

        public override int Depth => 1;

        public override int FieldCount => _ordinalToPropertyName.Count;

        public override bool HasRows => true;

        public override bool IsClosed
        {
            get
            {
                return false;
            }
        }

        public override int RecordsAffected
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IRow[] EnumerableRows { get; }
        public string[] Properties { get; }

        public override bool GetBoolean(int ordinal)
        {
            return (bool)GetValue(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return (byte)GetValue(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            return (char)GetValue(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return (DateTime)GetValue(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return (decimal)GetValue(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            return (double)GetValue(ordinal);
        }
        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
        //public override IEnumerator GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        public override Type GetFieldType(int ordinal)
        {
            var value = GetValue(ordinal);
            if (value == null)
                return typeof(object);

            return value.GetType();
        }

        public override float GetFloat(int ordinal)
        {
            return (float)GetValue(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            return (Guid)GetValue(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return (short)GetValue(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            return (int)GetValue(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            return (long)GetValue(ordinal);
        }

        public override string GetName(int ordinal)
        {
            string name;
            if (_ordinalToPropertyName.TryGetValue(ordinal, out name))
                return name;

            return null;
        }

        public override int GetOrdinal(string name)
        {
            int ordinal;
            if (_propertyNameToOrdinal.TryGetValue(name, out ordinal))
                return ordinal;

            return -1;
        }

        public override string GetString(int ordinal)
        {
            return (string)GetValue(ordinal);
        }

        public override object GetValue(int ordinal)
        {

            return EnumerableRows[nrRecord].Values[GetName(ordinal)];
        }

        public override int GetValues(object[] values)
        {
            int max = Math.Min(values.Length, FieldCount);
            for (var i = 0; i < max; i++)
            {
                values[i] = IsDBNull(i) ? DBNull.Value : GetValue(i);
            }

            return max;
        }

        public override bool IsDBNull(int ordinal)
        {
            return GetValue(ordinal) == null;
        }

        public override bool NextResult()
        {
            return false;
        }

        public override bool Read()
        {
            nrRecord++;
            return (nrRecord < EnumerableRows.Length);
        }
    }
}
