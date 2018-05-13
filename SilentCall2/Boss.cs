using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{
    //A class to represent records for the different bosses in the game
    public class Boss
    {
        //The level at which the boss will appear
        public int level = 0;
        //The name of the boss (for use in quest title etc)
        public String name;
        //If this boss has been beaten yet
        public bool beaten = false;

        public Boss()
        {
            name = "UNDEFINED BOSS";
        }

        public Boss(int aLevel, String aName, bool aBeaten)
        {
            level = aLevel;
            name = aName;
            beaten = aBeaten;
        }
    }
}
