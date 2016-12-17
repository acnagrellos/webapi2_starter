using Api.Tracking.Domain;
using System.Net.Http;

namespace Api.Tracking.Helper
{
    public interface ITrackHelper
    {
        EndpointDescription GetLogEndpoint(HttpRequestMessage request, HttpResponseMessage response, string bodyJson);
    }
}
