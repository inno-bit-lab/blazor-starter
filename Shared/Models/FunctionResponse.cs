using System;
using System.Net;

namespace BlazorApp.Shared.Models
{
    public class FunctionResponse<T>
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public T Content { get; set; }
        public FunctionResponseError Error { get; set; } 
        public bool IsSuccess => (int)HttpStatusCode < 400 && HttpStatusCode != HttpStatusCode.NoContent;

        public FunctionResponse()
        {

        }


        public FunctionResponse(HttpStatusCode httpStatusCode, T content)
        {
            HttpStatusCode = httpStatusCode;
            Content = content;
            Error = default;
        }
        
        public FunctionResponse(HttpStatusCode httpStatusCode, FunctionResponseError error)
        {
            HttpStatusCode = httpStatusCode;
            Content = default(T);
            Error = error;
        }
    }


    public class FunctionResponseError
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public FunctionResponseError(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
