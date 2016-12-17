using System;

namespace Api.Tracking.Domain
{
    public class EndpointDescription
    {
        public string OriginIP { get; set; }
        public string UrlRequest { get; set; }
        public string Controller { get; set; }
        public string Method { get; set; }
        public string QueryParameters { get; set; }
        public string BodyMessage { get; set; }
        public string Result { get; set; }
        public int? HttpResponseStatus { get; set; }
        public string ErrorMessages { get; set; }
        public DateTime Date { get; set; }
    }
}
