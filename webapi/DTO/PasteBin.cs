using System.Collections.Generic;
using System.IO;
using ServiceStack;
using ServiceStack.Web;

namespace web
{
    [Api("Paste and show pastebin items")]
    [Route("/pastebin", "POST")]
    [Route("/pastebin", "GET")]
    public class PasteBin:IReturn<PasteBinResponse>
    {
        public string pastedcode { get; set; }
    }
    
    [Api("Clear pastebin items")]
    [Route("/pastebin", "DELETE")]
    public class PasteBinClear:IReturn<PasteBinResponse>
    {
        
    }

    public class PasteBinResponse
    {
        public List<PasteBinModel> codesnippets;
    }

}