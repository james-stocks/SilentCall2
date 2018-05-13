using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{
    class TutorialMessage
    {
        public int ID = -1;
        public bool shown = false;
        public bool neverShowAgain = false;
        public String title;
        public String text;

        public TutorialMessage()
        {
            ID = -1;
            shown = false;
            neverShowAgain = false;
            title="UNINITIALISED!";
            text = "Oh God, How Did This Get Here? I'm Not Very Good With Computers :(";
        }

        public TutorialMessage(int aID, String aTitle, String aText)
        {
            ID = aID;
            shown = false;
            neverShowAgain = false;
            title = aTitle;
            text = aText;
        }
    }
}
