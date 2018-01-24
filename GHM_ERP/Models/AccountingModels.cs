using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GHM_ERP.Models
{
    public class CodeEditItem
    {
        [Required]
        public int AccountId { get; set; }
        public bool IsChanged { get; set; }
        public string NewCode { get; set; }

        public CodeEditItem()
        {
            IsChanged = false;
        }
    }

    public class AccountingCodeEditModel
    {
        public List<CodeEditItem> EditItems { get; set; }

        public AccountingCodeEditModel()
        {
            EditItems = new List<CodeEditItem>();
        }
    }
}