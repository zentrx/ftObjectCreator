using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loadTestingPhysicalCreator {
    class Document : Physical {
        #region Document Specific Values
        public string multiCom;
        public string singleCom;
        public bool? boolean;
        public string name;
        public string phone;
        public string multiML;
        public string singleML;
        #endregion Document Specific Values

        public Document()
        {
            this.username = "";
            this.org = "";
            this.cat = "";
            this.med = "";

            this.inactiveStorage = "";
            this.activeStorage = "";

            this.rfid = "";
            this.barcode = "";

            this.description = ""; 
            this.fileDate = DateTime.MinValue;
            this.memo1 = "";

            this.multiCom = "";
            this.singleCom = "";
            this.boolean = null;
            this.name = "";
            this.phone = "";
            this.multiML = "";
            this.singleML = "";

            this.storName = "";
            this.storType = "";
        }
        public Document(List<List<string>> data, List<int> indexer, int count, int id, DateTime start, int range, Random gen) {
            this.username = data[0][count % indexer[0]].Trim();
            this.org = data[1][count % indexer[1]].Trim();
            this.cat = data[2][count % indexer[2]].Trim();
            this.med = data[3][count % indexer[3]].Trim();

            this.inactiveStorage = data[4][count % indexer[4]].Trim();
            this.activeStorage = data[5][count % indexer[5]].Trim();
            
            this.description = data[6][count % indexer[6]].Trim();
            this.memo1 = data[7][count % indexer[7]].Trim();

            this.singleCom = data[8][count % indexer[8]].Trim(); //check number
            this.name = data[9][count % indexer[9]].Trim(); //check number
            this.singleML = data[10][count % indexer[10]].Trim(); //might need to get this from DB

            this.multiCom = ""; //generated based on single com
            this.boolean = count % 2 == 0; //alternate bool value
            this.phone = String.Concat("(", (gen.Next() % 1000).ToString(), ")", (gen.Next() % 1000).ToString(), "-", (gen.Next() % 10000).ToString()); //(###)###-####
            this.multiML = ""; // generated based on single ml

            this.barcode = "D-" + count.ToString("D10");
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
