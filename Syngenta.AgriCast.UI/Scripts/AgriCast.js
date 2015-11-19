(function (obj) {


    //server config
    var protocol = ('https:' == document.location.protocol ? 'https://' : 'http://');
    var httphandler = "AgriCastHandler.ashx";

    //list of required files to be loaded first
    /*Set the below values for "server" variable
    Production : agricast.syngenta.com/
    Stage : agricast-stage.syngenta.com/
    Dev : 15.141.25.102/
    Local : localhost:27196/Syngenta.AgriCast.UI/ */



    var server = "agricast.syngenta.com/";

    var application = "";
    var AgHandler = protocol + server + application + httphandler;
    var ag_cssfiles = [protocol + server + application + 'Styles/EmbedJSCommon.css',
         protocol + server + application + 'Styles/EmbedJSSite.css',
         protocol + server + application + 'Styles/Embedrateit.css',
         protocol + server + application + 'Styles/ui-lightness/jquery-ui-1.9.2.custom.css'];
    var ag_jsfiles = [protocol + server + application + 'Scripts/jquery-1.8.2.min.js',
            protocol + server + application + 'Scripts/jquery-ui-1.9.2.min.js',
         protocol + server + application + 'Scripts/jquery.rateit.js',
         protocol + server + application + 'Scripts/Main.js'];

    //prod 
    //var server = "agricast.syngenta.com/";	
    //stage
    //var server = "agricast-stage.syngenta.com/";	    

    //Test
    //    var server = "15.141.25.102/";


    //Dev
    //var server = "localhost:27196/";
    // var application = "Syngenta.AgriCast.UI/";
    // var AgHandler = protocol + server + application + httphandler;
    //var ag_cssfiles = ['Styles/EmbedJSCommon.css',
    //'Styles/EmbedJSSite.css',
    // 'Styles/ui-lightness/jquery-ui-1.8.17.custom.css'];
    // var ag_jsfiles = ['Scripts/jquery-1.7.min.js',
    // 'Scripts/jquery-ui.min.js',
    // 'Scripts/Main.js'];


    //show loading agricast
    document.write('<img  id="ag_imgSpin" src=' + protocol + server + application + 'Images/spin.gif alt="Loading..." />');
    var iefix = document.createElement('meta');
    iefix.setAttribute("http-equiv", "X-UA-Compatible");
    iefix.setAttribute("content", "IE=Edge");
    document.getElementsByTagName('head')[0].appendChild(iefix);


    function AddCSS(url) {
        css_style = document.createElement("link");
        css_style.setAttribute("rel", "stylesheet");
        css_style.setAttribute("type", "text/css");
        css_style.setAttribute("href", url);
        document.getElementsByTagName("head")[0].appendChild(css_style);
    };

    var loadNextCSS = function () {
        if (ag_cssfiles.length > 0) {
            AddCSS(ag_cssfiles.shift());
            loadNextCSS();
        }
    };
    loadNextCSS();

    // check for AgriCast existence     
    if (typeof (AgriCast) == 'undefined') {

        var loadNextScript = function () {
            if (ag_jsfiles.length > 0) {
                var script = document.createElement('script');
                script.src = ag_jsfiles.shift();
                document.getElementsByTagName("head")[0].appendChild(script);

                var done = false;
                script.onload = script.onreadystatechange = function () {
                    if (!done && (!this.readyState || this.readyState == "loaded" || this.readyState == "complete")) {
                        done = true;
                        // Handle memory leak in IE                         
                        script.onload = script.onreadystatechange = null;
                        loadNextScript();
                    }
                } //onready state end            

            } //sources length >0
            else {
                //call our main function once all scripts are loaded
                callback();
            }

        } //end of loadnext script function         

        loadNextScript();

    } //if lib exists
    else {
        callback();
    }

    //function to initialize agricast module
    var callback = function () {
        // Loading agricast container tags
        // jQuery will be available here
        var Tag;
        var QueryString = {};
        var agscript = $("script");

        for (var i = 0; i < agscript.length; i++) {
            //search for agricast.js script and get the parent element where it is added.
            if (agscript[i].src.toUpperCase().indexOf('AGRICAST.JS') != '-1') {

                Tag = agscript[i].parentElement;
                //get querystring appended to javascript file
                QueryString = (function (a) {
                    if (a == "")
                        return {};
                    var b = {};
                    for (var i = 0; i < a.length; ++i) {
                        var p = a[i].split('=', 2);
                        if (p[1]) p[1] = decodeURIComponent(p[1].replace(/\+/g, " "));
                        b[p[0]] = p[1];
                    }
                    return b;
                })((agscript[i].src.split('?'))[1].split('&'));

                break;
            }
        }


        $(document).ready(function () {

            //order -> pub, culture, country, place, lat, lng, key, overrideCookie
            var pub = (typeof (QueryString['pub']) != 'undefined') ? QueryString['pub'].toString() : "";
            var module = (typeof (QueryString['servicepage']) != 'undefined') ? QueryString['servicepage'].toString() : "";
            var culture = (typeof (QueryString['culture']) != 'undefined') ? QueryString['culture'].toString() : "";
            var country = (typeof (QueryString['country']) != 'undefined') ? QueryString['country'].toString() : "";
            var place = (typeof (QueryString['place']) != 'undefined') ? unescape(QueryString['place'].toString()) : "";
            var lat = (typeof (QueryString['lat']) != 'undefined') ? QueryString['lat'].toString() : "";
            var lng = (typeof (QueryString['lng']) != 'undefined') ? QueryString['lng'].toString() : "";
            var key = (typeof (QueryString['key']) != 'undefined') ? QueryString['key'].toString() : "";
            var overrideCookie = (typeof (QueryString['overridecookie']) != 'undefined') ? QueryString['overridecookie'].toString() : "";

            //Widget Creation for Weather icons
            var isWidget = (typeof (QueryString['widget']) != 'undefined') ? QueryString['widget'].toString() : "false";

            //Multi Cookie Implementation -Begin
            //LocationInfo is the default cookie used incase if no clientid is specified
            //change Default cookie of embed js :- EmbedJsDefCookie
            // this should be as the variable defined in agricast handler.
            //change "LocationInfo" to "EmbedJsDefCookie"
            var clientID = (typeof (QueryString['clientID']) != 'undefined') ? QueryString['clientID'].toString() : "EmbedJsDefCookie";
            //Multi Cookie Implementation -End

            //Units Implementation for Embed Js - Begin
            var unit = (typeof (QueryString['unit']) != 'undefined') ? QueryString['unit'].toString() : "metric";

            //unit should be either metric or imperial
            //if its misconfigured , then set unit to metric
            if (unit != '' && unit.toLowerCase() != "imperial")
                unit = "metric";
            //Units Implementation for Embed Js  -End

            var config = { 'protocol': protocol,
                'server': server,
                'application': application,
                'handler': httphandler,
                'handlerPath': AgHandler,
                'pub': pub,
                'module': module,
                'culture': culture,
                'country': country,
                'place': place,
                'lat': lat,
                'lng': lng,
                'key': key,
                'overrideCookie': overrideCookie,
                'idPrefix': 'ag_',
                'maxAutoSuggestOptions': 10,
                'isWidget': isWidget,
                'clientID': clientID, // Multi Cookie Implementation
                'unit': unit  //Units Implementation for Embed Js


            };
            //initialize AgriCast object
            AgriCast(config);

            //2012-dec-04 , publication specific CSS - Begin
            var pubSpecificCSS = protocol + server + application + 'pub/' + pub + '/EmbedSite' + '.css';
            AddCSS(pubSpecificCSS)

            //2012-dec-04 , publication specific CSS - End

            //if script is added in head or anywhere in body other than div or span
            //then create a new div and add agricast contents to that div and add this
            //div to body.
            if (Tag.nodeName != 'DIV' && Tag.nodeName != 'SPAN') {
                $('<div id="ag_tag"></div>').appendTo($('body'));
                AgriCast.LoadSearch('ag_tag');
            }
            else {
                //if div or span tag is not having an id then add an id to it.
                if ($.trim(Tag.id) != "") {
                    AgriCast.LoadSearch(Tag.id);
                }
                else {
                    Tag.id = "ag_tag";
                    AgriCast.LoadSearch(Tag.id);
                    //AgriCast.LoadModules('WeatherForecast',Tag.id);
                }

            }

        }); //end of document ready function 

    }; //end of callback function

})(this);