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
        public Box(List<List<string>> data, List<int> indexer, int rfid, DateTime start, int range, Random gen) {
            this.username = data[0][rfid % indexer[0]].Trim();
            this.org = data[1][rfid % indexer[1]].Trim();
            this.cat = data[2][rfid % indexer[2]].Trim();
            this.med = data[3][rfid % indexer[3]].Trim();
            
            this.inactiveStorage = data[4][rfid % indexer[4]].Trim();
            this.activeStorage = data[5][rfid % indexer[5]].Trim();
            this.description = data[6][rfid % indexer[6]].Trim();
            this.memo1 = data[7][rfid % indexer[7]].Trim();
            
            this.barcode = "B-" + rfid.ToString("D10");
            this.fileDate = start.AddDays(gen.Next(range));
            this.rfid = rfid.ToString("X24");

            if (rfid % 4 == 0) {
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