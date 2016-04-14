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
    class FolderCreator : PhysicalCreator
    {
        public ConcurrentQueue<Folder> qFoldersToAdd = new ConcurrentQueue<Folder>();
        public FolderCreator() {
            this.fileName = "\\Folder_LoadTest_1.csv";
            this.table = "folders";
            this.numberGenerated = 5000000;
        }

        public override string Structure()
        {
            StringBuilder structure = new StringBuilder();
            //classification / user
            structure.Append("usr nvarchar(50), ");
            structure.Append("org nvarchar(255), ");
            structure.Append("cat nvarchar(255), ");
            structure.Append("med nvarchar(255), ");

            //storage locations
            structure.Append("inactiveStor nvarchar(255), ");
            structure.Append("activeStor nvarchar(255), ");

            //tracking info
            structure.Append("barcode nvarchar(50), ");
            structure.Append("parentBarcode nvarchar(50), ");
            structure.Append("rfid nvarchar(50), ");

            //generic
            structure.Append("fileDate datetime, ");
            structure.Append("description nvarchar(max), ");
            structure.Append("memo1 nvarchar(max), ");

            //folder specific
            structure.Append("volume int, ");
            structure.Append("bool bit, ");
            structure.Append("name nvarchar(max), ");
            structure.Append("phone nvarchar(50), ");
            structure.Append("multiCom nvarchar(255), ");
            structure.Append("singleCom nvarchar(255), ");
            structure.Append("multiML nvarchar(255), ");
            structure.Append("singleML nvarchar(255), ");

            //storage selector
            structure.Append("storType nvarchar(255), ");
            structure.Append("storName nvarchar(255), ");

            //primary keys
            structure.Append("primary key (org, cat, med, singleML, volume), ");

            return structure.ToString();
        }
        public override void Produce() {
            Random gen = new Random();
            DateTime start = new DateTime(1990, 1, 1);
            int range = (DateTime.Today - start).Days;
            
            int global = 0;
            int parentBarcode = 1;
            int init = 0;
            int vol = 0;
            int volMax = gen.Next() % 4 + 1;

            while (global < numberGenerated) {
                qFoldersToAdd.Enqueue(new Folder(init, global, vol, data, indexer, start, range, gen, parentBarcode));
                if (global % 100000 == 0) Console.WriteLine(global + " entries produced");

                global++; vol++;
                if (global % 39 == 0) { parentBarcode++; }
                if (vol % 5 == 0) { init++; volMax = gen.Next() % 4 + 1; }
            }

        }
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
                Folder fldr = new Folder();
                int waitCounts = 0;
                while (true) {
                    if (qFoldersToAdd.TryDequeue(out fldr)) {
                        try {
                            sql = new StringBuilder();
                            sql.Append("insert into folders (usr, org, cat, med, inactiveStor, activeStor, barcode, parentBarcode, rfid, fileDate, description, memo1, storType, ");
                            sql.Append("storName, volume, singleCom, multiCom, bool, name, phone, singleML, multiML) ").Append(Environment.NewLine);
                            sql.Append("Values ('").Append(fldr.username).Append("', '").Append(fldr.org).Append("', '").Append(fldr.cat).Append("', '").Append(fldr.med).Append("', '");
                            sql.Append(fldr.inactiveStorage).Append("', '").Append(fldr.activeStorage).Append("', '").Append(fldr.barcode).Append("', '").Append(fldr.parentBC).Append("', '");
                            sql.Append(fldr.rfid).Append("', '").Append(fldr.fileDate).Append("', '").Append(fldr.description).Append("', '").Append(fldr.memo1).Append("', '");
                            sql.Append(fldr.storType).Append("', '").Append(fldr.storName).Append("', '").Append(fldr.volume).Append("', '").Append(fldr.singleCom).Append("', '");
                            sql.Append(fldr.multiCom).Append("', '").Append(fldr.boolean.ToString()).Append("', '").Append(fldr.name).Append("', '").Append(fldr.phone).Append("', '");
                            sql.Append(fldr.singleML).Append("', '").Append(fldr.multiML).Append("');").Append(Environment.NewLine);

                            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
                            cmd.ExecuteNonQuery();
                            percent++;
                            if (percent % 100000 == 0) Percentage(percent, this.numberGenerated);
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
                conn.Close();
            }
        }


    }
}
