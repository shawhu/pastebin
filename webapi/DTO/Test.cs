using System.Collections.Generic;
using ServiceStack;
using ServiceStack.FluentValidation;

namespace web
{
    [Api("Test request")]
    [Route("/test/{Input}", "GET")]
    [Route("/test")]
    public class TestRequest : IReturn<TestResponse>
    {
        [ApiMember(Name = "Parameter name", Description = "Parameter Description",
        ParameterType = "body", DataType = "string", IsRequired = true)]
        public string Input { get; set; }
        public long Timestamp { get; set; }
    }
    public class TestResponse
    {
        public Dictionary<string, string> dictOutput { get; set; }
        public string Output { get; set; }
    }

    //Validator
    public class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(r => r.Input).NotEmpty();
        }
    }
}