"use strict";

function switchtab(newtab) {
    var tabs = ["#cheque", "#cash", "#deposit"];
    tabs.forEach(function (elem) {
        $(elem).find("input,textarea,select,button").attr('disabled', 'disabled');
    });
    $(newtab).find("input,textarea,select,button").removeAttr('disabled');
    
}

function getDueInvoices(clientId)
{
    return _.filter(dueInvoices,_.matches({"ClientId":clientId}));
}

function clearDueInvoices()
{
    $("#invoiceTable").find(".invoiceRow").remove();
    $("#invoiceTable").find("#emptyMessageRow").removeClass('hidden');
}

function addDueInvoices(invoices) {
    $.each(invoices,
    function (i, inv) {
        var newRow = $("#invoiceTable").find("#templaterow").clone().removeAttr('id').removeClass('hidden');
        newRow.find(".invnocol").text(inv.FormattedInvNo);
        newRow.find(".datecol").text(inv.Date);
        newRow.find(".desccol").text(inv.Description);
        newRow.find(".totalcol").text(inv.TotalAmount);
        newRow.find(".duecol").text(inv.DueAmount);
        newRow.find(".duecol").attr("data-due", inv.DueAmount);
        newRow.find("input, select").removeAttr('disabled');
        newRow.addClass("invoiceRow");
        //set inputs
        var newRowId = generate_id();
        newRow.find("input").removeAttr("disabled");
        newRow.find(".invPayAmount, .hiddenid").each(function (i, elem) {
            var newName = $(elem).attr("name").replace("INDEXER", newRowId);
            $(elem).attr("name", newName);
        });
        newRow.find(".hiddenindexer").val(newRowId);
        newRow.find(".hiddenid").val(inv.Id);

        $("#invoiceTable").find("tbody").append(newRow);

    });

    if(_.any(invoices))
    {
        $("#invoiceTable").find("#emptyMessageRow").addClass('hidden');
    }
    else{
        $("#invoiceTable").find("#emptyMessageRow").removeClass('hidden');
    }
}

function clientChanged(clientSelect)
{
    //remove old ones
    clearDueInvoices();
    $("#prevDepositedAmount").text("Select a client").attr("data-amount", "0");
    //add new ones
    var clientId = $(clientSelect).find(":selected").val();
    if(clientId)
    {
        var dues = getDueInvoices(parseInt(clientId));
        addDueInvoices(dues);
        if (isSalesInvoice)
        {
            var clientDeposit = deposits[clientId];
            if (!clientDeposit)
                clientDeposit = 0;

            $("#prevDepositedAmount").text(formatPrice(clientDeposit)).attr("data-amount", clientDeposit);
        }

    }
    updateTotal();
}

function getPaymentTotal()
{
    var invAmountInps = $("#invoiceTable").find(".invoiceRow");
    var total = 0.00;

    invAmountInps.each(function (i, row) {
        var inp = $(row).find(" .invPayAmount");
        var val = Number($(inp).val());
        var due = Number($(row).find(".duecol").attr("data-due"));

        if($(inp).val().trim() == "" || val == 0 )
        {
            $(inp).parent().removeClass("has-error").removeClass("has-success");
        }
        else if (isNaN(val) || val < 0 || val > due + 0.00001)
        {
            $(inp).parent().addClass("has-error").removeClass("has-success");
        }
        else if (!isNaN(val) && val >= 0.00) {
            total += val;
            $(inp).parent().addClass("has-success").removeClass("has-error");
        }
    }
    );

    return total;
}

function updateTotal() {
    
    var total = getPaymentTotal();

    $("#totalTD").text(formatPrice(total));
    $("#paidToInvoices").text(formatPrice(total));

    var payAmount = parseFloat($("#Amount").val());

    if (!isNaN(payAmount) && payAmount >= 0)
    {
        $("#paymentAmount").text(formatPrice(payAmount));
        var deposit = payAmount - total;
        $("#depositAmount").text(formatPrice(deposit));
        if(deposit < -0.000001)
        {
            $("#depositAmount").addClass("text-danger");
        }
        else{
            $("#depositAmount").removeClass("text-danger");
        }
    }

}

function clearInvAmount(row)
{
    $(row).find(".invPayAmount").val("0.00");
    updateTotal();
}

function fillInvAmount(row) {
    clearInvAmount(row);

    var curPayments = getPaymentTotal();
    var payAmount = parseFloat($("#Amount").val());
    var balance = payAmount - curPayments;
    if(isNaN(balance))
    {
        balance = Infinity;
    }
    var due = parseFloat($(row).find('.duecol').text());

    var invAmount = _.min([due, balance]);

    var amountStr = formatPrice(invAmount);
    $(row).find(".invPayAmount").val(amountStr);
    updateTotal();
}


//validate invoice
function validateInvoice() {
    var amountText =$("#Amount").val();
    var payAmount = Number(amountText);
    if (isNaN(payAmount) || payAmount <= 0)
    {
        alert("Invalid Payment Amount - " + amountText);
        return false;
    }

    var hasError = false;
    var errText = "";

    var invAmountInps = $("#invoiceTable").find(".invoiceRow");
    invAmountInps.each(function (i, row)
    {
        var inp = $(row).find(".invPayAmount");
        var due = Number($(row).find(".duecol").attr("data-due"));
        var val = Number($(inp).val());
        var remainder = due - val;

        if (isNaN(val) || val < 0.00)
        {
            hasError = true;
            errText = "Invalid invoice payment - " + $(inp).val();
        }
        else if(remainder < -0.000001)
        {
            hasError = true;
            errText = "Invoice is paid more than the due amount - " + val + " for "+ due;
        }
    }
    );
    if (hasError)
    {
        alert(errText);
        return false;
    }
    var paymentTotal = getPaymentTotal();
    var balance = payAmount - paymentTotal;
    var isPayFromDeposit = false;
    if (isSalesInvoice && $("#dipositPaymentMethod").attr("disabled") === undefined)
    {
        isPayFromDeposit = true;
    }
    if (balance < -0.00001)
    {
        alert("Amount allocated to invoices is more than the payment amount");
        return false;
    }
    else if (!isPurchaseInvoice && balance > 0.00001)
    {
        if (isPayFromDeposit)
        {
            alert("Amount must be allocated completely when paying from a deposit");
            return false;
        }
        var confirmed = confirm("Your payment has a balance of " + formatPrice(balance) + ".\nDo you want to consider this as an advance?");
        return confirmed;
    }
    else if (isPurchaseInvoice && balance > 0.00001)
    {
        alert("Your payment has a balance of " + formatPrice(balance) +".\nYour amount must be completely allocated to purchase invoices");
        return false;
    }
    if (isPayFromDeposit)
    {
        var deposited = parseFloat($("#prevDepositedAmount").attr("data-amount"));

        if(paymentTotal > deposited )
        {
            alert("Deposited money is insufficient");
            return false;
        }

    }
    return true;
}


//------------ HTML UTIL ----------

function findParentTR(item) {
   return $(item).parentsUntil('tbody');
}