using Api.Tracking.Helper;
using Api.Tracking.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Api.Tracking
{
    public class TrackDelegationHandler : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var bodyJson = await request.Content.ReadAsStringAsync();
            var response = await base.SendAsync(request, cancellationToken);

            var attributes = request.GetActionDescriptor().GetCustomAttributes<TrackEndpointActionAttribute>();
            if (attributes.Any(attribute => attribute is TrackEndpointActionAttribute) || (int)response.StatusCode >= 300)
            {
                var dependecyContainer = request.GetDependencyScope();

                var trackingServices = dependecyContainer.GetServices(typeof(ITrackingService)).OfType<ITrackingService>();
                if (trackingServices != null && trackingServices.Count() > 0)
                {
                    var trackHelper = dependecyContainer.GetService(typeof(ITrackHelper)) as ITrackHelper;

                    if (trackHelper == null) trackHelper = new TrackHelper();
                    var trackEntity = trackHelper.GetLogEndpoint(request, response, bodyJson);

                    var tasks = new List<Task>();
                    foreach(var trackService in trackingServices)
                    {
                        tasks.Add(trackService.Track(trackEntity));
                    }
                    await Task.WhenAll(tasks);
                }
            }

            return response;
        }
    }
}
