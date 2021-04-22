using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rightmove
{
    public class RoomInfo
    {
        public RoomInfo()
        {
            Address = Price = AddOn = LetAvailableDate = "";
            LetType = FurnishType = PropertyType = "";
            BedRoom = BathRoom = Size = MarkedBy = ""; 
        }

        public string Address { get; set;}
        public string Price { get; set; }
        public string AddOn { get; set; }

        public string LetAvailableDate { get; set; }
        public string LetType { get; set; }
        public string FurnishType { get; set; }

        public string PropertyType { get; set; }
        public string BedRoom { get; set; }
        public string BathRoom { get; set; }
        public string Size { get; set; }
        public string MarkedBy { get; set; }

        public string GetRoomInfo()
        {
            return Address + "\t" + Price + "\t" + AddOn + "\t"
                + LetAvailableDate + "\t" + LetType + "\t" + FurnishType + "\t"
                + PropertyType + "\t" + BedRoom + "\t" + BathRoom + "\t" + Size + "\t" + MarkedBy;
        }

    }
}
