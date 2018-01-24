using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GHM_ERP.Util
{
    public class GHMAuthorizeAttribute : AuthorizeAttribute
    {
        public GHMAuthorizeAttribute()
        {

        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Controller.TempData["ErrorMsg"] = "You don't have permissions for this action. Please login as an administrator.";
            }

            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}