"use strict";
/* PURCHASE TABLE */

function addRMRow() {
    var original = $('#itemtemplaterow');
    var tablebody = $("#itemsbody");
    disableSelectSearch(tablebody.find('select'));

    var id = generate_id();

    var template = original.clone(true);
    template.removeClass('hidden');
    template.removeAttr('id');
    template.addClass("itemrow");
    template.insertBefore($("#emptylistmessage"));
    template.attr("id", id);
    $("#emptylistmessage").attr('class', 'hidden');

    updateDetailCols($("#" + id));

    //add indexer
    template.find("input,select").each(function(i,e)
    {
        var name = $(e).attr("name");
        if(name)
        {
            var newName = name.replace("INDEXER", id);
            $(e).attr("name", newName);
        }
    })
    template.find(".hiddenindexer").removeAttr("disabled").val(id);
    
    searcheableSelect(tablebody.find('select'));
    updateTotal();
}

function updateDetailCols(item)
{
    var option = $(item).find("option:selected");
    var color = option.attr('data-color');
    var thickness = option.attr('data-thickness');
    var type = option.attr('data-type');
    var width = option.attr('data-width');

    var row = option.parentsUntil('tbody');
    var cols = row.find('td');

    var desc = "Color : " + htmlEncode(color) + "<br/>Type: " + htmlEncode(type)
            + "<br/>Width :" + htmlEncode(width) +"<br/>Thickness: " + htmlEncode(thickness);
    var popupbtn = cols.eq(1).find(".detailpopup");
    popupbtn.attr("title", option.text().trim());
    popupbtn.attr("data-content", desc);
    popupbtn.popover('destroy')
    popupbtn.popover(
        {
            html: true
        }
    );

}



/* SALES TABLE */


function getAcceptableandRejectedBalances(rmtypes) {
    //rawMaterialList
    var accepted = [];
    var rejected = [];

    $.each(rawMaterialList,
        function (i, bal) {
            if ($.inArray(bal.rawMaterialId,rmtypes) >= 0) {
                accepted.push(bal);
            }
            else {
                rejected.push(bal);
            }
        }
    );
    return { accepted: accepted, rejected: rejected };
}

function addProdRow() {
    var original = $('#itemtemplaterow');
    var tablebody = $("#itemsbody");
    disableSelectSearch(tablebody.find('select'));

    var id = generate_id();

    var template = original.clone(true);
    template.removeClass('hidden');
    template.addClass("itemrow");
    template.insertBefore($("#emptylistmessage"));
    template.attr("id", id);
    $("#emptylistmessage").attr('class', 'hidden');

    //add indexer
    template.find("input,select").each(function (i, e) {
        var name = $(e).attr("name");
        if (name) {
            var newName = name.replace("INDEXER", id);
            $(e).attr("name", newName);
        }
    })
    template.find(".hiddenindexer").removeAttr("disabled").val(id);


    searcheableSelect(tablebody.find('select'));

    updateProdDetailCols(template);
    updatePrice(template);
    updateTotal();
}

function updateProdDetailCols(item) {
    var row = $(item);
    if (row[0].tagName.toLowerCase() != "tr")
    {
        row = $(item).parentsUntil('tbody');
    }

    // ---------- Change column texts
    var option = row.find(".comboproduct option:selected");
    var color = option.attr('data-color');
    var thickness = option.attr('data-thickness');
    var type = option.attr('data-type');
    var width = option.attr('data-width');
    var rmtypetext = option.attr('data-rawmaterials');
    var unitprice = formatPrice(parseFloat(option.attr('data-unitprice')));

    //----------set unit price
    var inpPrice = row.find(".unitprice").val(unitprice);

    //var row = option.parentsUntil('tbody');

    var cols = row.find('td');
    
    //-- details
    var desc = "Color : " + color + "<br/>Type: " + type + "<br/>Width :" + width;
    if (cols.eq(1).find(".detailpopup").size())
    {
        var popupbtn = cols.eq(1).find(".detailpopup");
        popupbtn.attr("title", option.text().trim());
        popupbtn.attr("data-content", desc);
        popupbtn.popover('destroy')
        popupbtn.popover(
            {
                html: true
            }
        );
    }
    else
    {
        cols.eq(1).html(desc);
    }

    
    //------- RAW Materials options

    var rmtypes = rmtypetext.split(",").map(function (i) { return parseInt(i); });
    var rms = getAcceptableandRejectedBalances(rmtypes);
    var rmselect = row.find('.comborm');
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

    updatePrice(item);
}

function deleteProdRow(item) {
    var row = $(item).parentsUntil('tbody');
    row.find('select').select2('destroy');
    row.remove();

    if ($("#itemsbody").find('.itemrow').length === 0) {
        $("#emptylistmessage").removeClass('hidden');
    }
    updateTotal();

}

function updatePrice(rowitem){
    var row = $(rowitem);
    if (row[0].tagName.toLowerCase() != "tr") {
        var row = $(rowitem).parentsUntil('tbody');
    }

    var qty = parseFloat(row.find('.qty').val());
    var length = parseFloat(row.find('.length').val());
    var unitprice = row.find(".unitprice").val();
    
    if (!isNaN(qty))
    {
        var subtotal = formatPrice(qty * length * parseFloat(unitprice));
        var inpPrice = row.find(".price");
        inpPrice.val(subtotal);

        updateTotal();
    }

}

/* Common */
function deleteRow(item) {
    var row = $(item).parentsUntil('tbody');
    row.find('select').select2('destroy');
    row.remove();

    if ($("#itemsbody").find('.itemrow').length === 0) {
        $("#emptylistmessage").removeClass('hidden');
    }
    updateTotal();

}




/* Payments and totals */

function getItemTotal() {
    var tablebody = $("#itemsbody");
    var priceinputs = tablebody.find(".itemrow .price");
    var total = 0.0;
    priceinputs.each(function (i, e) {
        var text = $(e).val();
        if (!isNaN(parseFloat(text))) {
            total += parseFloat(text);
        }
    });
    return total;

}

function getItemCount()
{
    var tablebody = $("#itemsbody");
    var priceinputs = tablebody.find(".itemrow .price");
    var count = priceinputs.toArray().length
    return count;

}

function updateTotal() {
    var total = getItemTotal();
    var itemCount = getItemCount();

    $("#itemtotal").text(total.toFixed(2));
    $("#summarytotal").text(total.toFixed(2));
    $("#itemCount").text(String(itemCount));
  }



/***************************** VALIDATIONS ***************************************/

function validateSalesInvoice()
{
    //item count validation
    if($(".itemrow").length == 0)
    {
        alert("No items added to invoice");
        return false;
    }

    //rm balance validations
    var rmAreaBalances = {};
    _.each(rawMaterialList, function (rm) { rmAreaBalances[rm.itemid] = rm.balance * rm.width; });
    
    var itemRows = $(".itemrow");
    var success = true;
    itemRows.each(function (index, row) {

        //skip after one error
        if (!success)
        {
            return;
        }

        var length = parseFloat($(row).find(".length").val());
        var qty = parseFloat($(row).find(".qty").val());
        var width = parseFloat($(row).find(".comboproduct option:selected").attr("data-width"));

        var productName = $(row).find(".comboproduct option:selected").text();
        
        var area = length * qty * width;
        if(isNaN(area))
        {
            alert("Invalid inputs for length and quantity");
            success = false;
            return;
        }

        var rmid = $(row).find(".comborm option:selected").val();
        if(!rmid)
        {
            alert("Raw Material source must be selected");
            success = false;
            return;
        }
        rmid = parseInt(rmid);
        
        if(rmid in rmAreaBalances)
        {
            rmAreaBalances[rmid] -= area;

            //not enough raw materials
            if(rmAreaBalances[rmid] < 0)
            {
                alert("Not enough raw materials for product " + (index+1) + " - " + productName);
                success = false;
                return;
            }
        }

    });

    
    return success;
}

