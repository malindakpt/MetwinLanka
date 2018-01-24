"use strict";


function registerButtonCallbacks()
{
	$(".btnchangecode").click(function () {
		var row = $(this).closest("tr");

		row.find(".btnchangecode").hide();
		row.find(".newcode").removeClass("hidden").removeAttr("disabled");
		row.find(".hiddencodechanged").removeAttr("disabled");
		row.find(".btncancelchangecode").removeClass("hidden");

		return false;
	});

	$(".btncancelchangecode").click(function () {
	    var row = $(this).closest("tr");

	    row.find(".btnchangecode").show();
	    row.find(".newcode").addClass("hidden").attr("disabled", "disabled");
	    row.find(".hiddencodechanged").attr("disabled", "disabled");
	    row.find(".btncancelchangecode").addClass("hidden");

	    return false;
	});
}

$(function () {
	registerButtonCallbacks();
	$(".btncancelchangecode").click(); //remove changes persisting after page reloads
});