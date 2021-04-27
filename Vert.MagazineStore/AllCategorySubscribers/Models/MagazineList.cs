using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCategorySubscribers.Models
{
   public class MagazineList
    {
        [JsonProperty("data")]
        public List<Magazine> Magazines { get; set; }
    }
}
