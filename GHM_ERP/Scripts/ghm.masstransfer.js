"use strict";

function onAmountInputChange(inputElem)
{
    if ($(inputElem).val()) {
        $(inputElem).closest("td").find(".amountClearBtn").removeClass("hidden");
    }

    validateAmountInputs();
}

//State can be null,"error" or "success"
function setInputState(input,state)
{
    $(input).closest("td").removeClass("has-success").removeClass("has-error");
    if(state)
    {
        $(input).closest("td").addClass("has-" + state);
    }
}

//rowItem is any element within tr, state is "success","danger", null
function setRowState(rowItem,state)
{
    $(rowItem).closest("tr").removeClass("danger").removeClass("success");
    if(state)
    {
        $(rowItem).closest("tr").addClass(state);
    }
}

function validateAmountInputs()
{
    var allRows = $(".accountRow").toArray();

    var hasError = false;

    var totalDebit = 0;
    var totalCredit = 0;

    allRows.forEach(function (elem) {

        //-- credit input
        var elemCredit = $(elem).find(".creditInput");
        var strCredit = elemCredit.val().trim();
        var creditAmount = Number(strCredit);
        //empty input
        if(!strCredit)
        {
            creditAmount = 0;
            setInputState(elemCredit, null);
        }
        //invalid input
        else if(isNaN(creditAmount) || creditAmount < 0)
        {
            creditAmount = 0;
            hasError = true;
            setInputState(elemCredit, "error");
        }
        //allowed input
        else
        {
            setInputState(elemCredit, "success");
        }

        //--- debit input
        var elemDebit = $(elem).find(".debitInput");
        var strDebit = elemDebit.val().trim();
        var debitAmount = Number(strDebit);
        //empty input
        if (!strDebit) {
            debitAmount = 0;
            setInputState(elemDebit, null);
        }
            //invalid input
        else if (isNaN(debitAmount) || debitAmount < 0) {
            debitAmount = 0;
            hasError = true;
            setInputState(elemDebit, "error");
        }
            //allowed input
        else {
            setInputState(elemDebit, "success");
        }

        //if both credit and debit exist
        if(creditAmount > 0 && debitAmount > 0)
        {
            setRowState(elemCredit, "danger");
        }
        else
        {
            setRowState(elemCredit, null);
        }

        totalDebit += debitAmount;
        totalCredit += creditAmount;

    });

    $("#creditTotal").text(formatPrice(totalCredit));
    $("#debitTotal").text(formatPrice(totalDebit));

    //totals match
    if (Math.round(totalDebit * 100) == Math.round(totalCredit * 100))
    {
        $("#totalMismatchPanel").addClass("hidden");
    }
    else
    {
        $("#totalMismatchPanel").removeClass("hidden");
        hasError = true;
    }

    //totalZeroPanel
    if (hasError || totalDebit > 0)
    {
        $("#totalZeroPanel").addClass("hidden");
    }
    else
    {
        $("#totalZeroPanel").removeClass("hidden");
        hasError = true;
    }
    $("#totalTransfer").text(formatPrice(totalCredit));

    return !hasError;
}

function submitClicked()
{
    if (!$("#Description").val().trim())
    {
        alert("Description is required");
        return false;
    }

    if(validateAmountInputs())
    {
        return true;
    }
    else
    {
        alert("Fields are not populated correctly");
        return false;
    }
}

function pageControlInitialize() {

    $(".amountClearBtn").click(function (elem) {
        $(this).closest("td").find("input").val("");
        $(this).addClass("hidden");
        validateAmountInputs();
        return false;
    });

    $(".amountInput").change(function (elem) {
        //show clear button
        onAmountInputChange(this);
    });

    $(".amountInput").each(function (i, elem) { onAmountInputChange(elem); });

    validateAmountInputs();

    $("#btnSubmit").click(submitClicked);
}

$(pageControlInitialize);