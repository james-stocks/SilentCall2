using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{
    public class LogMessage
    {
        public int age = 0;
        public String message;
        public bool isBad = false;

        public LogMessage()
        {
            age = 0;
            message = "";
            isBad = false;
        }

        public LogMessage(String aMessage)
        {
            age = 0;
            message = aMessage;
            isBad = false;
        }

        public LogMessage(String aMessage, bool bad)
        {
            age = 0;
            message = aMessage;
            isBad = bad;
        }
    }
}
