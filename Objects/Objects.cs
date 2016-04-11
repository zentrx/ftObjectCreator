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


namespace loadTestingPhysicalCreator {
    class Object {
        #region dbEntries
        public string username;
        public string org;
        public string cat;
        public string med;
        public string activeStorage;
        public string barcode;
        public DateTime fileDate;
        public string description;
        public string memo1;
        #endregion dbEntries
    }

    class Physical : Object {
        public string inactiveStorage;
        public string rfid;

        public string storType;
        public string storName;
    }

    
    class Electronic : Object {

    }
}
