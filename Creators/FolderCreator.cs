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
        public override string Structure()
        {
            StringBuilder fldrStruct = new StringBuilder();
            fldrStruct.Append("usr nvarchar(50), ");
            fldrStruct.Append("org nvarchar(255), ");
            fldrStruct.Append("cat nvarchar(255), ");
            fldrStruct.Append("med nvarchar(255), ");
            fldrStruct.Append("inactiveStor nvarchar(255), ");
            fldrStruct.Append("activeStor nvarchar(255), ");
            fldrStruct.Append("barcode nvarchar(50), ");
            fldrStruct.Append("rfid nvarchar(50), ");
            fldrStruct.Append("fileDate datetime, ");
            fldrStruct.Append("description nvarchar(max), ");
            fldrStruct.Append("memo1 nvarchar(max), ");
            fldrStruct.Append("storType nvarchar(255), ");
            fldrStruct.Append("storName nvarchar(255), ");
            fldrStruct.Append("primary key (rfid)");

            return fldrStruct.ToString();
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
        public override void Consume()
        {
            base.Consume();
        }


    }
}
