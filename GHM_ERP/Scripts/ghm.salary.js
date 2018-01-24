"use strict";

function updateTotal()
{
    var total = 0;
    $(".accAmount").each(function (idx, elem) {
        var val = $(elem).val();
        var amount = Number(val);
        if(!isNaN(amount) && amount > 0)
        {
            total += amount;
            $(elem).parent("td").addClass("has-success").removeClass("has-error");
        }
        else if (val) {
            $(elem).parent("td").addClass("has-error").removeClass("has-success");
        }
        else {
            $(elem).parent("td").removeClass("has-error").removeClass("has-success");
        }
    });

    $("#totalAmountSpan").text(formatPrice(total));
    $("#totalAmountInp").val(formatPrice(total));
}

function initBindings() {
    $(".clearbutton").click(function () {
        var row = $(this).closest("tr");
        var inp = row.find(".accAmount");
        inp.val("");
        updateTotal();
    });

    $(".accAmount").change(updateTotal);
}

$(initBindings);
$(updateTotal);
