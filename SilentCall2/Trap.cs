using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{
    //[Serializable]
    public class Trap
    {
        public int trapType = 0;
        public int x = 0;
        public int y = 0;
        public bool discovered = false;
        public bool disarmed = false;

        public Trap()
        {
        }

        public Trap(int aX, int aY, int type)
        {
            x = aX;
            y = aY;
            trapType = type;
        }

    }
}