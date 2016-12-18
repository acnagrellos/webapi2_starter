using System.Web.Http;

namespace Api.Core.Controllers.Base
{
    public class NotFoundController : BaseController
    {
        [HttpGet, HttpPost, HttpPut, HttpDelete, HttpHead, HttpOptions, AcceptVerbs("PATCH")]
        public IHttpActionResult ErrorNotFound()
        {
            return NotFound();
        }
    }
}
