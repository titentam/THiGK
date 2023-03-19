using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THiGK
{
    internal class DBHelper
    {
        private static DBHelper _Instance;
        private SqlConnection _connectionString;
        public static DBHelper Instance
        {
            get
            {
                if (_Instance == null)
                {
                    string s = "Data Source=msi\\sqlexpress01;Initial Catalog=NetGk;Integrated Security=True";
                    _Instance = new DBHelper(s);
                }
                return _Instance;
            }
            private set { }
        }
        private DBHelper(string s)
        {
            _connectionString = new SqlConnection(s);
        }
        public DataTable GetAllRecord(SqlCommand cmd)
        {
            cmd.Connection = _connectionString;
            SqlDataAdapter adapter = new SqlDataAdapter();

            adapter.SelectCommand = cmd;

            DataTable table = new DataTable();
            table.Columns.Add(ADD_STT());
            adapter.Fill(table);

            return table;
        }

        private DataColumn ADD_STT()
        {
            // Add a new column for STT
            DataColumn sttColumn = new DataColumn("STT", typeof(int));
            sttColumn.AutoIncrement = true;
            sttColumn.AutoIncrementSeed = 1;
            sttColumn.ReadOnly = true;
            return sttColumn;
        }

        public void ExcueteUpdateDB(SqlCommand cmd)
        {
            _connectionString.Open();
            cmd.Connection = _connectionString;

            cmd.ExecuteNonQuery();

            _connectionString.Close();
        }

        public List<string> GetList(string query)
        {
            _connectionString.Open();
            List<string> list = new List<string>();
            
            SqlCommand cmd = new SqlCommand(query, _connectionString);
            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    list.Add(reader.GetString(0));
                }
            }
            _connectionString.Close();

            return list;

        }
        public int? ExcuteSacarla(string query)
        {
            _connectionString.Open();
           

            SqlCommand cmd = new SqlCommand(query, _connectionString);
            var obj = cmd.ExecuteScalar();
            _connectionString.Close();

            if(obj!=null)
            return (int)obj;
            
            return null;

        }
    }
}
