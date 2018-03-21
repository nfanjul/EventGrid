using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Subscriber.Models
{
    public class Subscription
    {
        public string Name { get; set; }

        public string PrefixFilter { get; set; }

        public string SuffixFilter { get; set; }
    }
}