

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Cryptography;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Logging;
using ServiceStack.Redis;
using ServiceStack.Text;


public class Common
{
    public static IAppSettings Settings { get; set; }
    public static ILog log = LogManager.GetLogger(typeof(Common));
    public static ConcurrentDictionary<string, string> InMemoryOnlineUsers = new ConcurrentDictionary<string, string>();
    public static ConcurrentDictionary<string, string> InMemoryAPPIDs = new ConcurrentDictionary<string, string>();

    //make a log
    public static void LogDTO<T>(ServiceStack.Web.IRequest request, T resObject)
    {
        //logging
            var reqUrl = request.GetAbsolutePath();
            var reqHeaders = "";
            //var reqHeaders = base.Request.Headers.ToCsv();
            var reqBody = request.GetRawBody();
            var reqMethod = request.GetHttpMethodOverride();
            var resBody = resObject.ToJson();
            log.Warn($"URL:{reqUrl} Headers:{reqHeaders} Body:{reqBody} Method:{reqMethod} Response:{resBody}\n");
    }
    public static DateTime GetDatetimeFromUnixTime(long TimestampInSeconds)
    {
        return DateTime.Parse("1970-1-1").AddSeconds(TimestampInSeconds).ToLocalTime();
    }
    public static long TimestampNowInMilliseconds()
    {
        return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds * 1000;
    }
    public static long TimestampNow()
    {
        return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }
    public static long TimestampFromDate(DateTime dt)
    {
        return (long)dt.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }
    public static string GenerateCode(int length, Random rand)
    {
        string result = "";
        string[] opchars = {"a","b","c","d","e","f","g","h","i","j","k","m","n","p","q","r","s","t","u","v","w","x","y","z",
        "A","B","C","D","E","F","G","H","J","K","L","M","N","P","Q","R","S","T","U","V","W","X","Y","Z","2","3","4","5","6","7","8","9"};
        for (int i = 0; i < length; i++)
        {
            result += opchars[rand.Next(opchars.Length)];
        }
        return result;
    }
    public static async Task<SessionCheck> ValidateUser(
        string token, string userid, bool needvalidation = false, string sessioncachesource = "memory", 
        AmazonDynamoDBClient awsDb = null, RedisManagerPool redisManager = null, string AccountServerURL=""
    )
    {
        //create dynamodb context
        var context = new DynamoDBContext(awsDb);
        //checking to see if we need to validate users
        if (!needvalidation)
            return new SessionCheck() { success = true };
        if (userid.IsNullOrEmpty()||token.IsNullOrEmpty())
            return new SessionCheck() { success = false };
        //check local cache first
        switch (sessioncachesource.ToLower())
        {
            //Local validation
            case "redis":
                //using redis
                using (var client = redisManager.GetClient())
                {
                    var tt = client.Get<string>(userid);
                    if (tt == token)
                    {
                        //找到
                        Console.WriteLine("found user in redis cache");
                        return new SessionCheck() { success = true };
                    }
                }
                break;
            case "memory":
                //using in Memory
                if (InMemoryOnlineUsers.ContainsKey(userid))
                {
                    var tokenValidation = InMemoryOnlineUsers[userid];
                    if (tokenValidation == token)
                    {
                        Console.WriteLine("found user in redis cache");
                        return new SessionCheck() { success = true };
                    }
                }
                break;
            case "dynamodb":
                //using dynamodb
                var query = await context.QueryAsync<UserSession>(userid).GetRemainingAsync();
                if (query.FindAll(p => p.token == token).Count > 0)
                {
                    //找到
                    Console.WriteLine("found user in ddb cache");
                    return new SessionCheck() { success = true };
                }
                break;
            default:
                Console.WriteLine($"Server error, SessionCacheSource is unknown");
                return new SessionCheck() { success = false };
        }

        //local validation failed, now try remote validation
        if (AccountServerURL.IsNullOrEmpty())
        {
            Console.WriteLine("AccountServerURL is null or empty.");
            return new SessionCheck() { success = false };
        }
            
        HttpWebRequest request = WebRequest.CreateHttp($"{AccountServerURL}/sessions/check");
        request.Method = "POST";
        request.Accept = "application/json";
        request.ContentType = "application/json";
        request.Headers["X-API-ID"] = "077b005e-d7cf-42d4-9761-212683b339ba";
        request.Headers["X-API-SIG"] = "9703bb28-eba6-4d99-84fb-ce27acac101d";
        var reqstream = await request.GetRequestStreamAsync();
        //convert to byte array
        byte[] data = UTF8Encoding.UTF8.GetBytes("{\"user\":\"" + userid + "\",\"token\":\"" + token + "\"}");
        //sending
        reqstream.Write(data, 0, data.Length);
        reqstream.Close();
        try
        {
            var res = await request.GetResponseAsync();
            var checkresult = JsonSerializer.DeserializeResponse<SessionCheck>(res);
            //add to local cache
            if (sessioncachesource == "redis")
            {
                using (var client = redisManager.GetClient())
                {
                    client.Set(userid, token);
                    Console.WriteLine("Add usersession in redis cache");
                }
            }
            else if (sessioncachesource == "memory")
            {
                InMemoryOnlineUsers.TryAdd(userid, token);
                Console.WriteLine("Add usersession in memory");
            }
            else if (sessioncachesource == "dynamodb")
            {
                //add to dynamodb cache
                await context.SaveAsync(new UserSession() { userid = userid, token = token });
                Console.WriteLine("Add usersession in dynamodb");
            }
            else
            {
                Console.WriteLine("Unknown SessionCacheSource when populate local usersession cache");
                return new SessionCheck() { success = false };
            }
            return checkresult;
        }
        catch
        {
            return new SessionCheck() { success = false };
        }
    }
    public static async Task InvalidateOnlineUser(
        string userid,
        string sessioncachesource = "memory",
        AmazonDynamoDBClient awsDb = null,
        RedisManagerPool redisManager = null
    )
    {
        if (String.IsNullOrEmpty(userid))
        {
            log.Error("Userid is null or empty when trying to invalidate user token");
            throw new ArgumentException("Userid is null or empty when trying to invalidate user token");
        }
        if (sessioncachesource == "dynamodb" && awsDb == null)
        {
            log.Error("awsDb is null and sessioncachesource is dynamodb");
            throw new ArgumentException("awsDb is null and sessioncachesource is dynamodb");
        }
        if (sessioncachesource == "redis" && redisManager == null)
        {
            log.Error("redismanager is null and sessioncachesource is redis");
            throw new ArgumentException("redismanager is null and sessioncachesource is redis");
        }
        switch (sessioncachesource.ToLower())
        {
            case "dynamodb":
                //create dynamodb context
                var context = new DynamoDBContext(awsDb);
                var batch = context.CreateBatchWrite<UserSession>();
                batch.AddDeleteKey(userid);
                try
                {
                    await batch.ExecuteAsync();
                }
                catch (Exception e)
                {
                    log.Error(e);
                    throw e;
                }
                break;
            case "redis":
                using (var client = redisManager.GetClient())
                {
                    try
                    {
                        client.Remove(userid);
                    }
                    catch (Exception e)
                    {
                        log.Error(e);
                        throw e;
                    }
                }
                break;
            case "memory":
                string outstr = "";
                InMemoryOnlineUsers.TryRemove(userid, out outstr);
                break;
            default:
                break;
        }

    }
    public class APPID
    {
        public string appId { get; set; }
    }
    public static async Task<APPID> ValidateAppKeyAsync(string appid, string appsecret, string AccountServerURL)
    {
        if (String.IsNullOrEmpty(AccountServerURL))
        {
            throw new Exception("AccountServerUrlBase is missing in appsettings.[txt|json]");
        }

        //try local cache first

        if (InMemoryAPPIDs.ContainsKey(appid))
        {
            string outstr = "";
            InMemoryAPPIDs.TryGetValue(appid, out outstr);
            return new APPID()
            {
                appId = outstr
            };
        }

        HttpWebRequest request = WebRequest.CreateHttp($"{AccountServerURL}/appkey");
        request.Method = "GET";
        request.Accept = "application/json";
        request.ContentType = "application/json";
        request.Headers["X-API-ID"] = appid;
        request.Headers["X-API-SIG"] = appsecret;
        try
        {
            var res = await request.GetResponseAsync();
            APPID appidObj = ServiceStack.Text.JsonSerializer.DeserializeResponse<APPID>(res);
            InMemoryAPPIDs.TryAdd(appid, appidObj.appId);
            return appidObj;
        }
        catch (Exception e)
        {
            log.Error(e);
            throw new ArgumentException(e.Message);
        }
    }
    public static string getHashSha256(string text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        SHA256 hashstring = System.Security.Cryptography.SHA256.Create();

        byte[] hash = hashstring.ComputeHash(bytes);
        string hashString = string.Empty;
        foreach (byte x in hash)
        {
            hashString += String.Format("{0:x2}", x);
        }
        return hashString;
    }
}

public class SessionCheck
{
    public bool success { get; set; }
}

[DynamoDBTable("UserSession")]
public class UserSession
{
    [DynamoDBHashKey]
    public string userid { get; set; }
    [DynamoDBRangeKeyAttribute]
    public string token { get; set; }
}
