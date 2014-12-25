using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LocalisedResourceExtractionBenchmark.Setup
{
    class TestDataBulkReader : IDataReader
    {
        public enum Table
        {
            Dictionary,
            Source
        }

        private readonly Table _table;
        private readonly int _maxRows;
        private int _currentRow;
        private readonly IList<string> _columns;
        private readonly object[] _rowData;
        private readonly string[] _langs;
        private int _langNum;
        private readonly MD5 _md5;
        private readonly Random _random;
        private readonly StringBuilder _stringBuilder;

        public TestDataBulkReader(Table table, int maxRows)
        {
            _table = table;
            _maxRows = maxRows;
            _currentRow = 0;
            if (_table == Table.Dictionary)
            {
                _columns = new[] {"Id", "Lang", "Label"};
                _rowData = new object[3];
                _langs = new[] {"en", "fr", "de", "it", "es"};
                _langNum = _langs.Length;
                _md5 = MD5.Create();
            }
            else
            {
                _columns = new[] { "Id", "Code", "Parent", "Labels" };
                _rowData = new object[4];
                _random = new Random(832456); // for repeatable results
                _stringBuilder = new StringBuilder();
            }
        }

        private string GetLabel()
        {
            var dataString = Encoding.UTF8.GetBytes(_langs[_langNum] + _currentRow);
            var hashBytes = _md5.ComputeHash(dataString);
            return Convert.ToBase64String(hashBytes);
        }

        private string GetCode()
        {
            var id = _currentRow;
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            _stringBuilder.Length = 0;
            while (id > 0)
            {
                var index = (id - 1) % alphabet.Length;
                id = (id - 1) / alphabet.Length;
                _stringBuilder.Append(alphabet[index]);
            }
            return new string(_stringBuilder.ToString().Reverse().ToArray());
        }

        public bool Read()
        {
            if (_currentRow <= _maxRows)
            {
                if (_table == Table.Dictionary)
                {
                    _langNum++;
                    if (_langNum >= _langs.Length)
                    {
                        _currentRow++;
                        _rowData[0] = _currentRow;
                        _langNum = 0;
                    }
                    _rowData[1] = _langs[_langNum];
                    _rowData[2] = GetLabel();
                }
                else
                {
                    _currentRow++;
                    _rowData[0] = _currentRow;
                    _rowData[1] = GetCode();
                    var random = _random.Next(0, _currentRow);
                    _rowData[2] = random == 0 ? (object)DBNull.Value : random;
                    _rowData[3] = _currentRow;
                }
            }
            return _currentRow <= _maxRows;
        }

        public int FieldCount { get { return _rowData.Length; } }

        public int GetOrdinal(string name)
        {
            return _columns.IndexOf(name);
        }

        public object GetValue(int i)
        {
            return _rowData[i];
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        ~TestDataBulkReader()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_md5 != null) _md5.Dispose();
            }
        }

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        object IDataRecord.this[int i]
        {
            get { throw new NotImplementedException(); }
        }

        object IDataRecord.this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public int Depth { get {throw new NotImplementedException();} }
        public bool IsClosed { get{throw new NotImplementedException();} }
        public int RecordsAffected { get{throw new NotImplementedException();} }
    }
}
