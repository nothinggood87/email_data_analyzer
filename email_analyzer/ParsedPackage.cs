using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace email_analyzer
{
    class ParsedPackage
    {
        public ParsedPackage(string from,string subject,string body, DateTimeOffset timeDelivered)
        {
            From = from;
            Subject = subject;
            Body = body;
            TimeDelivered = timeDelivered;
        }
        public string From { get; }
        public string Subject { get; }
        public string Body { get; }
        public DateTimeOffset TimeDelivered { get; }
    }
}
