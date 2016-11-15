#region

using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

#endregion

namespace Demo.Nebula.Api
{
    [Route("Identity")]
    [Authorize]
    public class IdentityController : ApiController
    {
        public IHttpActionResult Get()
        {
            var user = User as ClaimsPrincipal;

            if (user == null)
                throw new InvalidOperationException();

            var claims = user.Claims.Select(s => new {type = s.Type, value = s.Value});

            return Json(claims);
        }
    }
}