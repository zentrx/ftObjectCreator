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
    class BoxCreator {
        public string location = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Remove(0, 6);
        public ConcurrentQueue<Box> qBoxesToAdd = new ConcurrentQueue<Box>();

        private List<List<string>> data = new List<List<string>>();
        private List<int> indexer = new List<int>();
        private string nl = Environment.NewLine;

        private int numberGenerated = 2000000;
        private int percent = 0;

        public static void Main(String[] args)
        {
            BoxCreator b = new BoxCreator();
            b.GenerateBoxes();
        }

        public void GenerateBoxes() {
            EstablishTable();
            EstablishData();

            Thread consumer = new Thread(new ThreadStart(ConsumeBox));
            Thread producer = new Thread(new ThreadStart(ProduceBox));

            try {
                producer.Priority = ThreadPriority.Lowest;
                producer.Start();

                List<Task> tasks = new List<Task>();
                for (int i = 0; i < 10; i++) { Task.Factory.StartNew(ConsumeBox); }

                consumer.Priority = ThreadPriority.Highest;
                consumer.Start();
            }
            catch (ThreadStateException e) { Console.WriteLine(e); }
            catch (ThreadInterruptedException e) { Console.WriteLine(e); }
        }

        public void EstablishTable()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                SqlConnectionStringBuilder connString = new SqlConnectionStringBuilder();
                connString.DataSource = "(local)\\FILETRAIL";
                connString.InitialCatalog = "LoadTest_Items";
                connString.IntegratedSecurity = true;
                conn.ConnectionString = connString.ConnectionString;
                conn.Open();

                StringBuilder sql = new StringBuilder();
                sql.Append(" if object_id ('dbo.boxes', 'U') is not null drop table dbo.boxes; \n ");
                sql.Append(" create table boxes( ");
                #region sql.Append(boxStructure.ToString());
                StringBuilder boxStructure = new StringBuilder();
                boxStructure.Append("usr nvarchar(50), ");
                boxStructure.Append("org nvarchar(255), ");
                boxStructure.Append("cat nvarchar(255), ");
                boxStructure.Append("med nvarchar(255), ");
                boxStructure.Append("inactiveStor nvarchar(255), ");
                boxStructure.Append("activeStor nvarchar(255), ");
                boxStructure.Append("barcode nvarchar(50), ");
                boxStructure.Append("rfid nvarchar(50), ");
                boxStructure.Append("fileDate datetime, ");
                boxStructure.Append("description nvarchar(max), ");
                boxStructure.Append("memo1 nvarchar(max), ");
                boxStructure.Append("storType nvarchar(255), ");
                boxStructure.Append("storName nvarchar(255), ");
                boxStructure.Append("primary key (rfid)");
                sql.Append(boxStructure.ToString());
                #endregion sql.Append(boxStructure.ToString());
                sql.Append(" ) ; \n ");

                SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
                cmd.ExecuteNonQuery();
            }
        }
        public void EstablishData()
        {
            StreamReader sr = new StreamReader(location + "\\Box_LoadTest_1.csv");
            string line;

            while (true)
            {
                line = sr.ReadLine();
                if (line == null) { break; }

                string[] entries = line.Split('|');
                int i = 0;
                foreach (string entry in entries)
                {
                    if (data.Count <= i) { data.Add(new List<string>()); }
                    if (!string.IsNullOrWhiteSpace(entry.Trim()))
                    {
                        data[i].Add(entry);
                    }
                    i++;
                }
            }

            foreach (List<string> column in data)
            {
                indexer.Add(column.Count());
            }
        }

        public string Structure() {
            StringBuilder boxStructure = new StringBuilder();
            boxStructure.Append("usr nvarchar(50), ");
            boxStructure.Append("org nvarchar(255), ");
            boxStructure.Append("cat nvarchar(255), ");
            boxStructure.Append("med nvarchar(255), ");
            boxStructure.Append("inactiveStor nvarchar(255), ");
            boxStructure.Append("activeStor nvarchar(255), ");
            boxStructure.Append("barcode nvarchar(50), ");
            boxStructure.Append("rfid nvarchar(50), ");
            boxStructure.Append("fileDate datetime, ");
            boxStructure.Append("description nvarchar(max), ");
            boxStructure.Append("memo1 nvarchar(max), ");
            boxStructure.Append("storType nvarchar(255), ");
            boxStructure.Append("storName nvarchar(255), ");
            boxStructure.Append("primary key (rfid)");

            return boxStructure.ToString();
        }

        public void ProduceBox()
        {
            Random gen = new Random();
            DateTime start = new DateTime(1990, 1, 1);

            int range = (DateTime.Today - start).Days;
            int rfid = 0;

            while (rfid < numberGenerated)
            {
                qBoxesToAdd.Enqueue(new Box(data, indexer, rfid + 1, start, range, gen));
                rfid++;
                if (rfid % 1000 == 0) Console.WriteLine(rfid + " entries produced");
            }
        }
        public void ConsumeBox()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                #region conn.Open();
                SqlConnectionStringBuilder connString = new SqlConnectionStringBuilder();
                connString.DataSource = "(local)\\FILETRAIL";
                connString.InitialCatalog = "LoadTest_Items";
                connString.IntegratedSecurity = true;
                conn.ConnectionString = connString.ConnectionString;
                conn.Open();
                #endregion conn.Open();

                StringBuilder sql = new StringBuilder();
                Box box = new Box();
                int waitCounts = 0;
                while (true)
                {
                    if (qBoxesToAdd.TryDequeue(out box))
                    {
                        try
                        {
                            sql = new StringBuilder();
                            sql.Append("insert into boxes (usr, org, cat, med, inactiveStor, activeStor, barcode, rfid, fileDate, description, memo1, storType, storName)").Append(nl);
                            sql.Append("Values ('").Append(box.username).Append("', '").Append(box.org).Append("', '").Append(box.cat).Append("', '").Append(box.med).Append("', '");
                            sql.Append(box.inactiveStorage).Append("', '").Append(box.activeStorage).Append("', '").Append(box.barcode).Append("', '").Append(box.rfid).Append("', '");
                            sql.Append(box.fileDate).Append("', '").Append(box.description).Append("', '").Append(box.memo1).Append("', '").Append(box.storType).Append("', '");
                            sql.Append(box.storName).Append("');").Append(nl);

                            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
                            cmd.ExecuteNonQuery();
                            percent++;
                            if (percent % 20000 == 0) Console.WriteLine(percent / 20000 + "% completed..." + nl);
                        }
                        catch (Exception e) { Console.WriteLine(e.ToString()); }
                    }
                    else
                    {
                        if (waitCounts <= 3)
                        {
                            Thread.Sleep(2000);
                            waitCounts += 1;
                        }
                        else break;
                    }
                }
            }
        }
    }
}
