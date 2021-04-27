
using AllCategorySubscribers.Models;
using AllCategorySubscribers.Services;
using Microsoft.Extensions.Configuration;
using System;
using static System.Console;


namespace AllCategorySubscribers
{
    class Program
    {      
       public  static void Main(string[] args)
        {
            try
            {
                //Build Config file
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .Build();
                //Process subscription info
                Data result = SubscriptionServices.ProcessSubscriptionInfo(configuration).GetAwaiter().GetResult();                
                DisplayResult(result);
            }
            catch (Exception ex)
            {
                WriteLine("\n********************Exception**************************************\n");
                WriteLine(" TIME:" + DateTime.Now);
                WriteLine(" ERROR:" + ex.Message);
                WriteLine("\n**********************Exception************************************\n");               
            }


            static void DisplayResult(Data result)
            {
                WriteLine("***********************Result**************************************\n");
                WriteLine(" Total time : " + result.TotalTime);
                string correctAnswer = result.AnswerCorrect == true ? "Yes" : "No";
                WriteLine(" Is the answer correct?  " + correctAnswer);
              
                if (result.ShouldBe != null)
                {                  
                    WriteLine(" Sorry! Expected Subscribers were :");                  
                    WriteLine(result.ShouldBe.ToString());
                }
                else
                {
                    WriteLine(" That went fine! Submitted subscriber list was accurate.");
                }
                WriteLine("\n********************Result****************************************\n");
            }

        }

    }
}
