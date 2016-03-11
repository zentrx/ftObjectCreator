using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loadTestingPhysicalCreator {
    class Builder {
        public static void Main(String[] args) {
            BoxCreator b = new BoxCreator();
            b.GenerateBoxes();
        }
    }
}
