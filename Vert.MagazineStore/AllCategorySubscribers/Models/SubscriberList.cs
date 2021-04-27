using Newtonsoft.Json;
using System.Collections.Generic;


namespace AllCategorySubscribers.Models
{
    public class SubscriberList
    {
        [JsonProperty("data")]
        public List<Subscriber> Subscribers { get; set; }
     
    }
}
