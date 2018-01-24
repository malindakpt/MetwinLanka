"use strict";


$(function () {
    $(".retqty").change(quantityChanged);
    quantityChanged();
}
);

function quantityChanged() {
    var itemRows = $(".itemrow");

    var totalQty = 0.0;
    var totalRetPrice = 0.0;

    $.each(itemRows, function (index, row) {
        var unitPrice = parseFloat($(row).attr("data-unit-price"));
        var qty = parseFloat($(row).find(".retqty").val());
        var length = parseFloat($(row).attr("data-length"));

        if (!isNaN(qty)) {
            totalQty += qty;
            var price = qty * unitPrice * length;
            totalRetPrice += price;

            $(row).find(".retvalue").text(formatPrice(price));
        }
    });

    $("#totalRetQty").text(totalQty);
    $("#totalRetPrice").text(formatPrice(totalRetPrice));
}
