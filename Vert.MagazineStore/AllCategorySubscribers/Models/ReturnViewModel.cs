

namespace AllCategorySubscribers.Models
{

        public class Data
        {
            public string TotalTime { get; set; }
            public bool AnswerCorrect { get; set; }
            public object ShouldBe { get; set; }
        }

        public class ReturnViewModel
        {
            public Data Data { get; set; }
            public bool Success { get; set; }
            public string Token { get; set; }
        }     

}
