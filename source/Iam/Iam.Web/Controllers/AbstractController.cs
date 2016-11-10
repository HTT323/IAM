#region

using System.Web.Mvc;

#endregion

namespace Iam.Web.Controllers
{
    [Authorize]
    public abstract class AbstractController : Controller
    {
    }
}