
function setRMList(elem)
{
    var rmtypetext = $(elem).attr("data-rmTypes");
    var rmtypes = rmtypetext.split(",").map(function (i) { return parseInt(i); });
    var rms = getAcceptableandRejectedBalances(rmtypes);
    var rmselect = elem;
    rmselect.select2('destroy');
    rmselect.html("<option value=''>No source set</option>");

    
    var addrmOptions = function (elem, rms) {
        $.each(rms, function (i, rm) {
            var option = $("<option>").val(rm.itemid)
                    .text(rm.itemDate + " - " + rm.rawMaterialCode + "(" + rm.itemDesc + ") (remaining: " + rm.balance + ")");
            elem.append(option);
        });
    };

    var optgroup = $("<optgroup>").attr("label", "Related");
    addrmOptions(optgroup, rms.accepted);
    rmselect.append(optgroup);

    optgroup = $("<optgroup>").attr("label", "Unrelated");
    addrmOptions(optgroup, rms.rejected);
    rmselect.append(optgroup);

    rmselect.select2({ width: 'resolve' });
}

$(function ()
{
    $(".comborm").each(function (index, elem) {
        setRMList($(elem));
        // var profileId = $(elem).attr("data-productProfile");
    });
});

function skipRow(item) {
    var row = $(item).closest("tr");
    row.find("input,select").attr("disabled", "disabled");
    row.addClass("danger").removeClass("success");


    var inpQty = row.find('.qty');
    inpQty.attr("data-originalQty", inpQty.val());
    inpQty.val("0");

    updatePrice(item);
    updateTotal();
    row.find(".skipButton").addClass("hidden");
    row.find(".readdButton").removeClass("hidden");
}

function readdRow(item) {
    var row = $(item).closest("tr");
    row.addClass("success").removeClass("danger");

    var inpQty = row.find('.qty');
    if (inpQty.attr("data-originalQty") !== undefined)
    {
        inpQty.val(inpQty.attr("data-originalQty"));
    }
    row.find("input,select").removeAttr("disabled");
    updatePrice(item);
    updateTotal();

    row.find(".readdButton").addClass("hidden");
    row.find(".skipButton").removeClass("hidden");
}

function validateOrderToSaleInvoice()
{
    //item count validation
    if ($(".itemrow").length == 0) {
        alert("No items added to invoice");
        return false;
    }

    //rm balance validations
    var rmAreaBalances = {};
    _.each(rawMaterialList, function (rm) { rmAreaBalances[rm.itemid] = rm.balance * rm.width; });

    var itemRows = $(".itemrow");
    var success = true;
    var validItemCount = 0;
    itemRows.each(function (index, row) {

        //skip after one error
        if (!success) {
            return;
        }

        //skip skipped items
        if ($(row).find("[disabled]").length > 0)
        {
            return;
        }

        var length = parseFloat($(row).find(".length").val());
        var qty = parseFloat($(row).find(".qty").val());
        var width = parseFloat($(row).find(".productId").attr("data-width"));

        var productName = $(row).find(".comboproduct option:selected").text();

        //skip items with qty 0
        if (qty < 1)
        {
            return;
        }

        var area = length * qty * width;
        if (isNaN(area) || area <= 0) {
            alert("Invalid inputs for length and quantity");
            success = false;
            return;
        }

        var rmid = $(row).find(".comborm option:selected").val();
        if (!rmid) {
            alert("Raw Material source must be selected");
            success = false;
            return;
        }
        rmid = parseInt(rmid);

        if (rmid in rmAreaBalances) {
            rmAreaBalances[rmid] -= area;

            //not enough raw materials
            if (rmAreaBalances[rmid] < 0) {
                alert("Not enough raw materials for product " + (index + 1) + " - " + productName);
                success = false;
                return;
            }
        }
        validItemCount+= 1;

    });

    if (success && validItemCount == 0)
    {
        alert("No items selected for sale");
        success = false;
    }

    return success;
}





//// Overrides
getItemCount = function () {
    var tablebody = $("#itemsbody");
    var allPriceinputs = tablebody.find(".itemrow .price");
    var disabledPriceInputs = tablebody.find(".itemrow .price[disabled]");
    var count = allPriceinputs.toArray().length - disabledPriceInputs.toArray().length;
    return count;

}