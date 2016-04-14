using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace loadTestingPhysicalCreator
{
    class PhysicalCreator { 
        #region csvData
        public string location = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Remove(0, 6);
        public string fileName = "";

        public List<List<string>> data = new List<List<string>>();
        public List<int> indexer = new List<int>();
        #endregion csvData 
        
        #region creationData
        public string table = "";
        public int numberGenerated = 0;
        public int percent = 0;
        public static int uniqueId = 0;
        #endregion creationData

        public void EstablishData() {
            //reads in data from a csv file at the location of the executable
            StreamReader sr = new StreamReader(this.location + this.fileName);
            string line;

            while (true) {
                line = sr.ReadLine();
                if (line == null) { break; }

                int i = 0;
                string[] entries = line.Split('|');
                foreach (string entry in entries) {
                    if (data.Count <= i) { data.Add(new List<string>()); }
                    if (!string.IsNullOrWhiteSpace(entry.Trim())) {
                        data[i].Add(entry);
                    }
                    i++;
                }
            }

            foreach (List<string> column in data) {
                indexer.Add(column.Count());
            }
        }
        public void EstablishTable() {
            //builds db table to insert into.
            using (SqlConnection conn = new SqlConnection()) {
                SqlConnectionStringBuilder connString = new SqlConnectionStringBuilder();
                connString.DataSource = "(local)\\FILETRAIL";
                connString.InitialCatalog = "LoadTest_Items";
                connString.IntegratedSecurity = true;
                conn.ConnectionString = connString.ConnectionString;
                conn.Open();

                StringBuilder sql = new StringBuilder();
                sql.Append(" if object_id ('dbo.").Append(this.table);
                sql.Append("', 'U') is not null drop table dbo.").Append(this.table).Append("; \n ");
                
                sql.Append(" create table ").Append(this.table).Append("( ");
                sql.Append(Structure());
                sql.Append(" ) ; \n ");

                SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
                cmd.ExecuteNonQuery();
            }
        }
        public virtual string Structure() {
            //Structure of the table goes here in inherited classes
            return String.Empty;
        }

        public virtual void Produce() { Console.WriteLine("wrong produce"); }
        public virtual void Consume() { }

        public void Generate()
        {
            EstablishTable();
            EstablishData();

            try {
                List<Task> tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => Produce()));
                for (int i = 0; i < numberGenerated / 100000; i++) {
                    tasks.Add(Task.Factory.StartNew(() => Consume()));
                }
                Task.WaitAll(tasks.ToArray());
            }
            catch (ThreadStateException e) { Console.WriteLine(e); }
            catch (ThreadInterruptedException e) { Console.WriteLine(e); }
        }
        public void Percentage(int current, int total) {
            Console.WriteLine(String.Concat((current / total).ToString("p1"), " completed...", Environment.NewLine));
        }
    }
    public delegate void AsyncGenerate();

}
