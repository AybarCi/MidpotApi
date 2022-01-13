using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingWeb.Model
{
    public class BannedModel
    {
        public long BaseUser { get; set; }
        public long TargetUser { get; set; }
    }
}
