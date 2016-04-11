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
    class DocumentCreator : PhysicalCreator
    {
        public ConcurrentQueue<Document> qDocsToAdd = new ConcurrentQueue<Document>();

        public DocumentCreator() {
            this.fileName = "";
            this.table = "documents";
            this.numberGenerated = 30000000;
        }

        /// <summary>
        /// Builds structure for the document Database table
        /// </summary>
        /// <returns>Text that would sit between "CREATE TABLE DOCUMENT(" AND "); " </returns>
        public override string Structure() {
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
            structure.Append("rfid nvarchar(50), ");

            //generic
            structure.Append("fileDate datetime, ");
            structure.Append("description nvarchar(max), ");
            structure.Append("memo1 nvarchar(max), ");
            
            //document specific
            structure.Append("multiCom nvarchar(255), ");
            structure.Append("singleCom nvarchar(255), ");
            structure.Append("bool boolean, ");
            structure.Append("name nvarchar(max), ");
            structure.Append("phone nvarchar(50), ");
            structure.Append("multiML nvarchar(255), ");
            structure.Append("singleML nvarchar(255), ");
            
            //storage selector
            structure.Append("storType nvarchar(255), ");
            structure.Append("storName nvarchar(255), ");
            
            //primary keys
            structure.Append("primary key (org), ");
            structure.Append("primary key (cat), ");
            structure.Append("primary key (med), ");
            structure.Append("primary key (singleML)");

            return structure.ToString();
        }

        
        /// <summary>
        /// Producer -- Creates documents based on data to be enqueued
        /// </summary>
        public override void Produce() {
            Random gen = new Random();
            DateTime start = new DateTime(1990, 1, 1); 
            int range = (DateTime.Today - start).Days; 
            int count = 0; //unique count of progress inside Box creator. uniqueId is global.

            while (count < numberGenerated) {
                qDocsToAdd.Enqueue(new Document(data, indexer, count++, ++uniqueId, start, range, gen));
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
                Document doc = new Document();
                int waitCounts = 0;
                while (true) {
                    if (qDocsToAdd.TryDequeue(out doc)) {
                        try {
                            sql = new StringBuilder();
                            sql.Append("insert into documents (usr, org, cat, med, inactiveStor, activeStor, barcode, rfid, fileDate, description, memo1, storType, storName, parentId, singleCom, multiCom, bool, name, phone, singleML, multiML)").Append(Environment.NewLine);
                            sql.Append("Values ('").Append(doc.username).Append("', '").Append(doc.org).Append("', '").Append(doc.cat).Append("', '").Append(doc.med).Append("', '");
                            sql.Append(doc.inactiveStorage).Append("', '").Append(doc.activeStorage).Append("', '").Append(doc.barcode).Append("', '").Append(doc.rfid).Append("', '");
                            sql.Append(doc.fileDate).Append("', '").Append(doc.description).Append("', '").Append(doc.memo1).Append("', '").Append(doc.storType).Append("', '");
                            sql.Append(doc.storName).Append("', '").Append(doc.parentId).Append("', '").Append(doc.singleCom).Append("', '").Append(doc.multiCom).Append("', '");
                            sql.Append(doc.boolean.ToString()).Append("', '").Append(doc.name).Append("', '").Append(doc.phone).Append("', '").Append(doc.singleML).Append("', '");
                            sql.Append(doc.multiML).Append("');").Append(Environment.NewLine);

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
