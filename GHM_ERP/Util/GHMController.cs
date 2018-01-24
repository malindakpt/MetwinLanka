using BusinessLogic;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
namespace GHM_ERP.Util
{
    public abstract class GHMController : Controller
    {
        protected virtual ActionResult ErrorResult( string errorMessage, string errorHeading = null)
        {
            ViewBag.Message = errorMessage;
            Response.StatusCode = 404;
            return View("~/Views/Shared/Error.cshtml");

        }

        protected User GetCurrentLoggedUser()
        {
            if(this.User.Identity.IsAuthenticated)
            {
                using (var manager= new UsersManager())
                {
                    return manager.LoadUser(this.User.Identity.GetUserId());
                }
            }
            else
            {
                return null;
            }
        }

        protected void SetInfoAlert(string alert)
        {
            if(alert != null)
            {
                TempData["InfoMsg"] = alert;
            }
            
        }

        protected void SetErrorAlert(string alert)
        {
            if (alert != null)
            {
                TempData["ErrorMsg"] = alert;
            }
        }

        protected void SetWarningAlert(string alert)
        {
            if (alert != null)
            {
                TempData["WarningMsg"] = alert;
            }
        }

        protected string FormatAsInvoiceNo(int invNo)
        {
            var tempInv = new Invoice { Id = invNo };
            return tempInv.FormattedInvoiceNo;
        }

    }
}