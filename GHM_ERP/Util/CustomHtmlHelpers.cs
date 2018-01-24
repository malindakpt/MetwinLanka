using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace GHM_ERP.Util
{
    public static class CustomHtmlHelpers
    {

        public static HtmlString ActivenessDisplay(this HtmlHelper helper ,bool isActive)
        {
            if(isActive)
            {
                return new HtmlString("<span class='label label-success'>Active</span>");
            }
            else
            {
                return new HtmlString("<span class='label label-default'>Inactive</span>");
            }
        }

        public static HtmlString ActivenessDisplayFor<TModel>(this HtmlHelper<TModel> helper, Func<TModel, bool> accessor)
        {
            var model = helper.ViewData.Model;
            if( model == null)
            {
                return new HtmlString("");
            }
            else
            {
                return ActivenessDisplay(helper, accessor(model));
            }
        }


        public static HtmlString HtmlEscapeNewLine(this HtmlHelper helper,string text)
        {
            var htmlencoded = HttpUtility.HtmlEncode(text);
            var replaced = htmlencoded.Replace("\n", "<br/>");
            return new HtmlString(replaced);            
        }

        public static string FormatAsDate(this HtmlHelper helper, DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }


        public static string FormatAsDateTime(this HtmlHelper helper, DateTime date)
        {
            return date.ToString("yyyy-MM-dd hh:mm tt");
        }

        public static IHtmlString FormatAsMoney(this HtmlHelper helper, decimal amount)
        {
            return helper.Raw(amount.ToString("#,##0.00"));
        }

        public static IHtmlString FormatAsQty(this HtmlHelper helper, decimal qty)
        {
            return FormatAsMoney(helper,qty);
        }

        public static string FormatAsInvoiceNo(this HtmlHelper helper, int invNo)
        {
            var tempInv =  new Invoice{ DisplayInvoiceNo = invNo};
            return tempInv.FormattedInvoiceNo;
        }

        public static IHtmlString AlertPanel(this HtmlHelper helper )
        {
            return helper.Partial("_InfoPanel");
        }

        public static HtmlString InvoiceStatusDisplay(this HtmlHelper helper, Invoice.InvoiceStatus status)
        {
            switch (status)
            {
                case Invoice.InvoiceStatus.Approved:
                    return new HtmlString("<span class='label label-success'>Active</span>");

                case Invoice.InvoiceStatus.Cancelled:
                    return new HtmlString("<span class='label label-danger'>Cancelled</span>");

                case Invoice.InvoiceStatus.Order:
                    return new HtmlString("<span class='label label-info'>Order</span>");

                case Invoice.InvoiceStatus.Pending:
                case Invoice.InvoiceStatus.Rejected:
                case Invoice.InvoiceStatus.Draft:
                default:
                    return new HtmlString("<span class='label label-default'>Other</span>");
            }
        }
    }
}