using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loadTestingPhysicalCreator {
    class Folder : Physical {
        #region Folder Specific Values
        public string parentId;
        public string multiCom;
        public string singleCom;
        public bool? boolean;
        public string name;
        public string phone;
        public int volume;
        public string multiML;
        public string singleML;
        #endregion Folder Specific Values

        public Folder(int init, int global, int vol, List<List<string>> data, List<int> indexer, DateTime start, int range, Random gen, int parentId) {
            this.username = data[0][global % indexer[0]].Trim();
            this.org = data[1][init % indexer[1]].Trim();
            this.cat = data[2][init % indexer[2]].Trim();
            this.med = data[3][init % indexer[3]].Trim();

            this.volume = vol;

            this.inactiveStorage = data[4][global % indexer[4]].Trim();
            this.activeStorage = data[5][global % indexer[5]].Trim();

            this.description = data[6][global % indexer[6]].Trim();
            this.memo1 = data[7][global % indexer[7]].Trim();
            this.fileDate = start.AddDays(gen.Next(range));
            
            this.singleCom = data[8][global % indexer[8]].Trim(); //check number
            this.multiCom = String.Concat(data[8][global % indexer[8]].Trim(), "^", data[8][(global + gen.Next() % 8365) % indexer[8]].Trim());
            this.singleML = data[10][init % indexer[10]].Trim();
            this.multiML = String.Concat(data[10][init % indexer[10]].Trim(), "^", data[10][(init + gen.Next() % 8365) % indexer[10]].Trim());

            this.name = data[9][global % indexer[9]].Trim(); //check number
            this.boolean = global % 2 == 0; //alternate bool value
            this.phone = String.Concat("(", (gen.Next() % 1000).ToString(), ")", (gen.Next() % 1000).ToString(), "-", (gen.Next() % 10000).ToString()); //(###)###-####

            this.barcode = "F-" + global.ToString("D10");
            this.parentId = "B-" + parentId.ToString("D10");
            this.rfid = (global + 2000001).ToString("X24");
            
            
        }
    }
}
