using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loadTestingPhysicalCreator {
    class Folder : Physical {
        #region Folder Specific Values
        public string parentBC;
        public string multiCom;
        public string singleCom;
        public bool? boolean;
        public string name;
        public string phone;
        public int volume;
        public string multiML;
        public string singleML;

        public int global = 0;
        private int client = 0;
        #endregion Folder Specific Values

        public Folder() {
            this.username = "";
            this.org = "";
            this.cat = "";
            this.med = "";

            this.inactiveStorage = "";
            this.activeStorage = "";

            this.rfid = "";
            this.barcode = "";
            this.parentBC = "";

            this.description = "";
            this.fileDate = DateTime.MinValue;
            this.memo1 = "";

            this.volume = 0;
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

        public Folder(int init, int global, int vol, List<List<string>> data, List<int> indexer, DateTime start, int range, Random gen, int parentId) {
            this.username = data[0][global % indexer[0]].Trim();
            this.org = data[1][init % indexer[1]].Trim();
            this.cat = data[2][init % indexer[2]].Trim();
            this.med = data[3][init % indexer[3]].Trim();

            this.inactiveStorage = data[4][global % indexer[4]].Trim();
            this.activeStorage = data[5][global % indexer[5]].Trim();

            this.description = data[6][global % indexer[6]].Trim();
            this.memo1 = data[7][global % indexer[7]].Trim();
            this.fileDate = start.AddDays(gen.Next(range));

            this.volume = vol;
            this.singleCom = data[8][global % indexer[8]].Trim(); //check number
            this.multiCom = String.Concat(data[8][global % indexer[8]].Trim(), "^", data[8][(global + gen.Next() % 8365) % indexer[8]].Trim());
            this.client = gen.Next() % 10000;
            this.singleML = String.Concat(client, "//", ((int)(client / 2000) * 50 + gen.Next() % 50));
            this.client = gen.Next() % 10000;
            this.multiML = String.Concat(client, "//", ((int)(client / 2000) * 50 + gen.Next() % 50), "^", this.singleML);

            this.name = data[9][global % indexer[9]].Trim(); //check number
            this.boolean = global % 2 == 0; //alternate bool value
            this.phone = String.Concat("(", (gen.Next() % 1000).ToString(), ")", (gen.Next() % 1000).ToString(), "-", (gen.Next() % 10000).ToString()); //(###)###-####

            this.barcode = "F-" + global.ToString("D10");
            this.parentBC = "B-" + parentId.ToString("D10");
            this.rfid = (global + 2000001).ToString("X24");

            this.global = global;
            if (global % 4 == 0) {
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
