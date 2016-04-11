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


namespace loadTestingPhysicalCreator{
    class Box : Physical {
        public Box() {
            this.username = "";
            this.org = "";
            this.cat = "";
            this.med = "";
            this.inactiveStorage = "";
            this.activeStorage = "";
            this.barcode = "";
            this.description = "";
            this.fileDate = DateTime.MinValue;
            this.memo1 = "";
            this.rfid = "";

            this.storName = "";
            this.storType = "";
        }
        public Box(List<List<string>> data, List<int> indexer, int count, int id, DateTime start, int range, Random gen) {
            this.username = data[0][count % indexer[0]].Trim();
            this.org = data[1][count % indexer[1]].Trim();
            this.cat = data[2][count % indexer[2]].Trim();
            this.med = data[3][count % indexer[3]].Trim();

            this.inactiveStorage = data[4][count % indexer[4]].Trim();
            this.activeStorage = data[5][count % indexer[5]].Trim();
            
            this.description = data[6][count % indexer[6]].Trim();
            this.memo1 = data[7][count % indexer[7]].Trim();

            this.barcode = "B-" + count.ToString("D10");
            this.fileDate = start.AddDays(gen.Next(range));
            this.rfid = id.ToString("X24");

            if (count % 4 == 0) {
                this.storType = "Active Storage";
                this.storName = this.activeStorage;
            }
            else {
                this.storType = "Inactive Storage";
                this.storName = this.inactiveStorage;
            }
        }
    }
}