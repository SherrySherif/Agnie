using AllCategorySubscribers.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;



namespace AllCategorySubscribers.Services
{
   public class SubscriptionServices
    {
        static string url;
        static string token;       
        static List<string> categories;
        static List<Magazine> magazines;
        static List<Subscriber> subscribers;
        static IConfiguration config;
        static readonly HttpClient client = new HttpClient();

        public static async Task<Data> ProcessSubscriptionInfo(IConfiguration configuration)
        {           
            try
            {
                Data result = new Data();
                config = configuration;
                ConfigureHttpClient();
                token = GetToken();
                if (token.Length != 0)
                {
                    categories = await GetCategoriesAsync();                  
                    subscribers = await GetSubscribersAsync();
                    //Get Magazine List
                    url = config.GetSection("Parameters").GetSection("Magazines").Value + token + "/";
                    magazines=new List<Magazine>();
                    foreach (string category in categories)
                    { 
                        magazines.AddRange(await GetMagazinesAsync(url + category));
                    }
                    //Get subscribers with subscriptions for all categories
                   List<string> allCategorySubscribers= GetSubscriberList();
                    //Post the subscriber list to API,and receive result
                   result=  await PostAnswerAsync(allCategorySubscribers);                
                }
                return result;
            }
            catch (Exception)
            {
                throw ;
            }
            finally
            {
                client.Dispose();               
            }
           
        }
   

        /// <summary>
        /// Get token Value from API
        /// </summary>
        /// <returns>token</returns>
        public static string GetToken()
        {
            string tokenUrl =config.GetSection("Parameters").GetSection("Token").Value;
            var response = client.GetAsync(tokenUrl).GetAwaiter().GetResult();
            var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            token = JObject.Parse(result)["token"].ToString();
            return token;
        }

        /// <summary>
        /// Get all available categories  from API
        /// </summary>
        /// <returns>category list</returns>
        public static async Task<List<string>> GetCategoriesAsync()
        {          
            url =config.GetSection("Parameters").GetSection("Categories").Value + token;         
            string message =await  GetResponseResultAsync(url);
            Category category = JsonConvert.DeserializeObject<Category>(message);           
            return category.Data;
        }

        /// <summary>
        /// Get subscriber details from API
        /// </summary>
        /// <returns>Subscriber list</returns>
     public static async Task<List<Subscriber>> GetSubscribersAsync()
        {          
            url =config.GetSection("Parameters").GetSection("Subscribers").Value + token;
            string message = await GetResponseResultAsync(url);
            return JsonConvert.DeserializeObject<SubscriberList>(message).Subscribers;               
        }

        /// <summary>
        /// Get all magazine info from API
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Magazine list</returns>
      public  static async Task<List<Magazine>> GetMagazinesAsync(string url)
        {         
            string message = await GetResponseResultAsync(url);
            MagazineList list= JsonConvert.DeserializeObject<MagazineList>(message);
            return list.Magazines;
        }


        /// <summary>
        /// Common Method that calls the API
        /// </summary>
        /// <param name="url"></param>
        /// <returns>response content as a string</returns>
        static async Task<string> GetResponseResultAsync(string url)
        {
            string message=string.Empty;
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                message = await response.Content.ReadAsStringAsync();                
            }
            return message;
        }

      
        /// <summary>
        /// Get subscribers who have subscriptions in all categories
        /// </summary>
        /// <returns>subscriber list</returns>
        public  static List<string> GetSubscriberList()
        {
            List<string> subscriberIds = new List<string>();
            List<string> magazineIds;
            foreach (Subscriber subscriber in subscribers)
            {
                magazineIds = subscriber.MagazineIds;
                //Check if subscriber has subsciptions for all categories
                bool hasNotAllCategories = categories
                               .Except(magazines.Where(catList => magazineIds.Contains(catList.MagazineId.ToString()))                           
                              .Select(ctgrs => ctgrs.Category)
                              .Distinct())
                              .Any();
                //If subscriber has subscriptions for all categories,add her to the list
                if (hasNotAllCategories == false)
                {
                    subscriberIds.Add(subscriber.SubscriberId);
                }
            }
            return subscriberIds;
        }

        /// <summary>
        /// Post subscriber list to the API
        /// </summary>
        /// <param name="allCategorySubscribers"></param>
        /// <returns>API Response including submission success info,correct answer if the submitted data was wrong</returns>
        public static async Task<Data> PostAnswerAsync(List<string> allCategorySubscribers)
        {

            url = config.GetSection("Parameters").GetSection("Answer").Value + token;
            Answer answer = new Answer();
            answer.Subscribers = allCategorySubscribers;
            string requestBody = JsonConvert.SerializeObject(answer); 
            var response = await client.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"));
            var message = response.Content.ReadAsStringAsync().Result;
            ReturnViewModel result = JsonConvert.DeserializeObject<ReturnViewModel>(message);
            return result.Data;
        }

        /// <summary>
        /// Method to configure HttpClient Object
        /// </summary>
        public static void ConfigureHttpClient()
        {          
            client.BaseAddress = new Uri(config.GetSection("Parameters").GetSection("BaseUrl").Value); 
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }       

    }
}
