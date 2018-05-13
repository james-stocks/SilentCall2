using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{
    public class Feat
    {
        public int ID;
        public String title;
        public String description;
        public String descriptionWhenHidden;
        public int rank;
        public int MAX_RANK;
        public int goal;
        public int progress;
        public int rewardGold;
        public int rewardXP;
        public bool hidden = true;

        public Feat()
        {

        }

    }
}
