"use strict";

$(function ()
{
    $('input[type=datetime]').datetimepicker({
        weekStart: 1,
        todayHighlight: true,
        todayBtn: "linked",
        showMeridian: true,
        format: "yyyy-mm-dd   HH:ii P"
    });
    $('input[type=date]').datepicker(
        {
        weekStart: 1,
        format: "yyyy-mm-dd",
        todayHighlight: true,
        todayBtn : "linked"
        }
    );

    $('.combosearch').each(function (i, e) { searcheableSelect(e); })
}
);

function generate_id()
{
    return _.uniqueId("autoid");
}


function formatPrice(floatval)
{
    //    var formatted = numeral(floatval).format('0,0.00');
    return floatval.toFixed(2);
}

function searcheableSelect(elem)
{
    $(elem).select2();
}

function disableSelectSearch(elem)
{
    $(elem).select2('destroy');
}


function getSelectedValuesOfSelect(elem) {
    var opts = $(elem).find("option:selected").toArray();
    var values = _.map(opts, function (opt) { return $(opt).val(); });
    return values;
}


function attachHiddenElementsForArray(formElem, values, nameTemplate, replacePart) {
    $.map(values, function (val,index) {
        var name = nameTemplate.replace(replacePart, String(index));
        var hidden = $("<input type='hidden'>").attr('name', name).val(val);
        $(formElem).append(hidden);
    });
}

function htmlDecode(encodedText) {
    return $("<div/>").html(encodedText).text();
}

function htmlEncode(text)
{
    return $("<div/>").text(text).html();
}

//$.validator.addMethod("time", function (value, element) {
//    return this.optional(element) || /^(([0-1]?[0-9])|([2][0-3])):([0-5]?[0-9])(:([0-5]?[0-9]))?$/i.test(value);
//}, "Please enter a valid time.");

