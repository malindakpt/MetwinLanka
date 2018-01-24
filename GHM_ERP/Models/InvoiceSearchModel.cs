using BusinessObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GHM_ERP.Models
{
    public class InvoiceSearchModel
    {
        public bool IsSearchRequested { get; set; }

        public bool IsSearchByInvNo { get; set; }

        [Display(Name="Invoice Number")]
        public string InvNo { get; set; }

        [Display(Name = "Client")]
        public int? ClientId { get; set; }

        public string Description { get; set; }

        [Display(Name = "Gate Pass")]
        public string GatePass { get; set; }

        [Display(Name = "Sales Rep")]
        public int? SalesRep { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? DateStart { get; set; }

        [Display(Name = "End Date")]
        public DateTime? DateEnd { get; set; }

        public InvoiceSearchModel()
        {
            IsSearchRequested = false;
        }


        public InvoiceSearchOptions ToInvoiceSearchOptions(Invoice.TansactionTypes transType,out int filterCount)
        {

            filterCount = 0;
            if(!IsSearchRequested || IsSearchByInvNo)
            {
                return null;
            }
            else
            {
                InvoiceSearchOptions options = new InvoiceSearchOptions
                {
                    TransType = transType
                };

                if(Description != null)
                {
                    options.Description = Description;
                    filterCount++;
                }
                if(GatePass != null)
                {
                    options.GatePass = GatePass;
                    filterCount++;
                }


                return options;
            }
        }
    }
}