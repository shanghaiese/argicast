$(document).ready(function () {
    //$("#ctl00_PageFooter_footNav li").replaceWith("<li>Prognoza dostarczana przez <a href='http://www.wetteronline.de/' target='_blank'>www.wetteronline.de</a></li>");
    var content = $("#ctl00_PageFooter_footNav li").html();
    var afterchange = content.replace("Sprostowanie", "");
    $("#ctl00_PageFooter_footNav li").html(afterchange);

});