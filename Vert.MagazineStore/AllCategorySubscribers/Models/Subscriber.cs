using Newtonsoft.Json;

using System.Collections.Generic;
using System.Text.Json;


namespace AllCategorySubscribers.Models
{
    public class Subscriber
    { 
        [JsonProperty ("id")]
       public string SubscriberId { get; set; }
        [JsonProperty("firstName")]
      public  string FirstName { get; set; }
        [JsonProperty ("lastName")]
      public string LastName { get; set; }
        [JsonProperty("magazineIds")]
      public  List <string> MagazineIds { get; set; } 
        
    }
}

