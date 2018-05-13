using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{
    public class Furniture
    {
        public int furnType;
        public String name;
        public bool burnable;
        public bool inscribed;
        public bool CanWalkOver = false;
        public bool CanSeeThrough = true;
        public String inscription;
        public int x;
        public int y;
        public byte visibility = 0;

        public Furniture()
        {
            furnType = (int)SC.FurnTypes.TABLE;
            name = "UNDEFINED FURNITURE";
            burnable = true;
            inscribed = false;
            inscription = "JIMMY WOZ ERE";
            x = 0;
            y = 0;
        }
    }
}
