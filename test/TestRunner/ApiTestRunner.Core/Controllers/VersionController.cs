﻿using ApiTestRunner.Core.Controllers.Base;
using Api.Versioned;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace ApiTestRunner.Core.Controllers
{
    [RoutePrefix(Constants.BaseRoute + "/" + Constants.Version)]
    public class VersionController : BaseController
    {
        [HttpGet]
        [ResponseType(typeof(bool))]
        [VersionedRoute(Constants.GetTestBoolean, VersionConstants.VersionDefault)]
        public async Task<IHttpActionResult> GetTestBoolean()
        {
            return Ok(false);
        }

        [HttpGet]
        [ResponseType(typeof(bool))]
        [VersionedRoute(Constants.GetTestBoolean, 2)]
        public async Task<IHttpActionResult> GetTestBoolean2()
        {
            return Ok(false);
        }
    }
}
