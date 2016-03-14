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
    class BoxCreator : PhysicalCreator {
        public ConcurrentQueue<Box> qBoxesToAdd = new ConcurrentQueue<Box>();
        public BoxCreator() {
            this.fileName = "\\Box_LoadTest_1.csv";
            this.table = "boxes";
            this.numberGenerated = 2000000;
        }

        /// <summary>
        /// Builds structure for the Box Database table
        /// </summary>
        /// <returns>Text that would sit between "CREATE TABLE BOX(" AND "); " </returns>
        public override string Structure() {
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

        /// <summary>
        /// Producer -- Creates boxes based on data to be enqueued
        /// </summary>
        public override void Produce() {
            Random gen = new Random();
            DateTime start = new DateTime(1990, 1, 1); 
            int range = (DateTime.Today - start).Days; 
            int count = 0; //unique count of progress inside Box creator. uniqueId is global.

            while (count < numberGenerated) {
                qBoxesToAdd.Enqueue(new Box(data, indexer, count++, ++uniqueId, start, range, gen));
                if (count % 1000 == 0) Console.WriteLine(count + " entries produced");
            }
        }
        /// <summary>
        /// Consumer -- Multiple instances import data from Produced queue to SQL
        /// </summary>
        public override void Consume() {
            using (SqlConnection conn = new SqlConnection()) {
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
                while (true) {
                    if (qBoxesToAdd.TryDequeue(out box)) {
                        try {
                            sql = new StringBuilder();
                            sql.Append("insert into boxes (usr, org, cat, med, inactiveStor, activeStor, barcode, rfid, fileDate, description, memo1, storType, storName)").Append(Environment.NewLine);
                            sql.Append("Values ('").Append(box.username).Append("', '").Append(box.org).Append("', '").Append(box.cat).Append("', '").Append(box.med).Append("', '");
                            sql.Append(box.inactiveStorage).Append("', '").Append(box.activeStorage).Append("', '").Append(box.barcode).Append("', '").Append(box.rfid).Append("', '");
                            sql.Append(box.fileDate).Append("', '").Append(box.description).Append("', '").Append(box.memo1).Append("', '").Append(box.storType).Append("', '");
                            sql.Append(box.storName).Append("');").Append(Environment.NewLine);

                            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
                            cmd.ExecuteNonQuery();
                            percent++;
                            if (percent % 20000 == 0) Console.WriteLine(percent / 20000 + "% completed..." + Environment.NewLine);
                        }
                        catch (Exception e) { Console.WriteLine(e.ToString()); }
                    }
                    else {
                        if (waitCounts <= 3) {
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
