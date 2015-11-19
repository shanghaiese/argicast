
function fnShowLoading() {
    //$.mobile.loadingMessage = "Please Wait..."
    $.mobile.showPageLoadingMsg();
}

function fnHideLoading() {
    $.mobile.hidePageLoadingMsg();
}

function setCookie(name) {

    document.cookie = name + '=; expires=Thu, 01-Jan-70 00:00:01 GMT;'; 

    if (is_mobile()) {
        createCookie(name, '1', 1);
        //document.cookie = name + '=; expires=Thu, 01-Jan-70 00:00:01 GMT;';
    }
}

function createCookie(name, value, days) {
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    }
    else var expires = "";
    document.cookie = name + "=" + value + expires + ";";
}

function is_mobile() {
    //agents for major browsers in mobile devices based on priority
    //1st - Major mobile OS, and commonly used devices
    //2nd - Optional browsers
    //3rd - Others native browsers by mobile carrier providers
    var agents = ['android', 'iphone', 'ipad', 'ipod', 'blackberry', 'symbian', 
                  'webos', 'opera mini', 'iemobile', 'meego', 'Dolphin',
                  'SonyEricsson', 'Samsung', 'nokia', 'Vodafone', 'HTC'];
    for (i in agents) {
        if (navigator.userAgent.match(new RegExp(agents[i], "i"))) {
            return true;
        }
    }
    return false;
}

function pageLoad(sender, args) {

    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(fnShowLoading);
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(fnHideLoading);
    createPage();
}

function collapseSearch() {
    $('#divChangeLoc').trigger('collapse');
}

function createPage() {
    // to handle page update via asp updatepanel
    // recreate the page to apply css for mobile.
    $('div[data-role="page"]').trigger('create');
}

function back() {
//    var page;
//    if (document.URL.indexOf("#") > 0) {
//        page = document.URL.split('#')[1];
//        page = page.substring(page.lastIndexOf('/')+1, page.length);         
//    }
    history.back();
//    if (page.toLowerCase() == "changesettings.aspx") {
//        //$.mobile.loadPage("default.aspx", { reloadPage: true, showLoadMsg: true });
//        window.location.href = page;
//        window.location.reload();
//    }
}
$(document).bind("mobileinit", function () {
    // As of Beta 2, jQuery Mobile's Ajax navigation does not work in all cases (e.g., 
    // when navigating from a mobile to a non-mobile page), hence disabling it. 
    $.mobile.ajaxEnabled = false;
}); 
//    function getParameterByName(d, b) {
//        var a;
//        if (b.length == 0) a = window.location;
//        else a = b;
//        // var c = RegExp("[?&]" + d + "=([^&]*)").exec(a);
//        var c = RegExp("[?&]" + d + "=(\[^&]*)?(&|$)|^" + d + "=(\[^&]*)?(&|$)").exec(a);
//        return c && decodeURIComponent(c[1].replace(/\+/g, " "))
//    }