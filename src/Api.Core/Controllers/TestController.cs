using Api.Core.Controllers.Base;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Api.Core.Controllers
{
    public class TestController : BaseController
    {
        [HttpGet]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> Get()
        {
            return Ok();
        }
    }
}
