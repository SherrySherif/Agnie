using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCategorySubscribers.Models
{
    public class Magazine
    {
        [JsonProperty ("id")]
        public int MagazineId { get; set; }        
        public string Name { get; set; }
        public string Category { get; set; }
    }
}
