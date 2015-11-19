$(document).ready(function () {
    $("#gv_valuevew_spraygrnd tr:gt(0)").each(function () {
        $(this).find("td:gt(0)").each(function () {
            $(this).html("<span>" + $(this).text() + "</span>");
        });
    });
});