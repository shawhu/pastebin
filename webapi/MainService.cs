using System;
using System.Collections.Generic;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Logging;

namespace web
{
    public class MainService:Service
    {
        public IAppSettings Settings { get; set; }
        

        public object Any(TestRequest req)
        {
            ILog log = LogManager.GetLogger(typeof(TestRequest));
            string str = "";
            Dictionary<string,string> dict = new Dictionary<string,string>();
            //testing Common
            DateTime dt = Common.GetDatetimeFromUnixTime(req.Timestamp);
            dict.Add("Datetime",$"{dt.ToString()}");

            var token = base.Request.GetHeader("Authorization");
            dict.Add("Token",$"{token}");

            var awsappid = Settings.GetString("awsappid");//case sensitive
            dict.Add("AwsAppID",$"{awsappid}");

            str+=$" Input:{req.Input}";
            dict.Add("Input",$"{req.Input}");

            var result = new TestResponse(){
                Output = "",
                dictOutput = dict
            };
            Common.LogDTO(base.Request,result);
            return result;
        }


        public object Delete(PasteBinClear req)
        {
            return PasteBinModel.Clear();
        }
        public object Post(PasteBin req)
        {
            var result = PasteBinModel.Push(req.pastedcode);
            return result;
        }
        public object Get(PasteBin req)
        {
            var result = PasteBinModel.GetAll();
            return result;
        }

    }
}