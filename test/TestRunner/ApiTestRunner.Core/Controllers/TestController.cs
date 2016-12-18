using Api.Core.Controllers.Base;
using Api.Core.Features.Versioned;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Api.Core.Controllers
{
    [RoutePrefix(Constants.BaseRoute + "/" + Constants.Test)]
    public class TestController : BaseController
    {
        [HttpGet]
        [ResponseType(typeof(bool))]
        [VersionedRoute(Constants.GetTestBoolean, Constants.VersionDefault)]
        public async Task<IHttpActionResult> GetTestBoolean()
        {
            return Ok(true);
        }
    }
}
