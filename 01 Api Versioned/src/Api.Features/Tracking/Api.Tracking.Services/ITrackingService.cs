using Api.Tracking.Domain;
using System.Threading.Tasks;

namespace Api.Tracking.Services
{
    public interface ITrackingService
    {
        Task Track(EndpointDescription entity);
    }
}
