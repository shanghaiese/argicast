
//server config

var protocol = ('https:' == document.location.protocol ? 'https://' : 'http://');
var httphandler = "AgriCastHandler1.ashx";

//list of required files to be loaded first
/*Set the below values for "server" variable
Production : agricast.syngenta.com/
Stage : agricast-stage.syngenta.com/
Dev : 15.141.25.195/
Local : localhost:27196/Syngenta.AgriCast.UI/ */



var server = "agricast.syngenta.com/";
var application = "";

var AgHandler = protocol + server + application + httphandler;
var ag_cssfiles = [protocol + server + application + 'Styles/EmbedJSCommon1.css',
                       protocol + server + application + 'Styles/EmbedJSSite1.css',
    		       protocol + server + application + 'Styles/Embedrateit.css',
    		       protocol + server + application + 'Styles/ui-lightness/jquery-ui-1.9.2.custom.css'];
var ag_jsfiles = [protocol + server + application + 'Scripts/jquery-1.8.2.min.js',
		      protocol + server + application + 'Scripts/jquery-ui-1.9.2.min.js',
    		      protocol + server + application + 'Scripts/jquery.rateit.js',
                      protocol + server + application + 'Scripts/Main1.js'];



var iefix = document.createElement('meta');
iefix.setAttribute("http-equiv", "X-UA-Compatible");
iefix.setAttribute("content", "IE=Edge");
document.getElementsByTagName('head')[0].appendChild(iefix);

 
 function AddCSS(url) {
        var count = 0;
        var cssname = url.split('/');
        for (var i = 0; i < document.getElementsByTagName("link").length; i++) {
            //search for agricast.js script and get the parent element where it is added.
            if (document.getElementsByTagName("link")[i].href.toUpperCase().indexOf(cssname[cssname.length - 1].toUpperCase()) != '-1') {
                count++;
                break;
            }
        }
        if (count == 0) {
            css_style = document.createElement("link");
            css_style.setAttribute("rel", "stylesheet");
            css_style.setAttribute("type", "text/css");
            css_style.setAttribute("href", url);
            document.getElementsByTagName("head")[0].appendChild(css_style);
        }
    };

    var loadNextCSS = function () {
        if (ag_cssfiles.length > 0) {
            AddCSS(ag_cssfiles.shift());
            loadNextCSS();
        }
    };

    loadNextCSS();
    

        var loadNextScript = function () {
            if (ag_jsfiles.length > 0) {
                var countjs = 0;
                var script = document.createElement('script');
                script.src = ag_jsfiles.shift();
                var jsname = script.src.split('/');
                for (var i = 0; i < document.getElementsByTagName("script").length; i++) {
                    //search for agricast.js script and get the parent element where it is added.
                    if (document.getElementsByTagName("script")[i].src.toUpperCase().indexOf(jsname[jsname.length - 1].toUpperCase()) != '-1') {
                        countjs++;
                        break;
                    }

                }
                if (countjs == 0) {
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
                } //countjs=0
                else {
                    //load the next script if this one is already loaded.
                    loadNextScript();
                }
               

            } //sources length >0
            else {
                //call our main function once all scripts are loaded
                //callback();
            }

        } //end of loadnext script function       

        //loadNextScript();
   

    //function to initialize agricast module
    function callback(ipub, imodule, iculture, icountry, iplace, ilat, ilng, ioverrideCookie, iWidget, iunit, iclientID) {

        //show loading agricast
        document.write('<img  id="ag_imgSpin" src=' + protocol + server + application + 'Images/spin.gif alt="Loading..." class="ag_imgSpin"/>');

        //order -> pub, culture, country, place, lat, lng, key, overrideCookie
        //Multi Cookie Implementation -End
        var pub = (typeof (ipub) != 'undefined') ? ipub.toString() : "";
        var module = (typeof (imodule) != 'undefined') ? imodule.toString() : "";
        var culture = (typeof (iculture) != 'undefined') ? iculture.toString() : "";
        var country = (typeof (icountry) != 'undefined') ? icountry.toString() : "";
        var place = (typeof (iplace) != 'undefined') ? unescape(iplace.toString()) : "";
        var lat = (typeof (ilat) != 'undefined') ? ilat.toString() : "";
        var lng = (typeof (ilng) != 'undefined') ? ilng.toString() : "";
      
        var overrideCookie = (typeof (ioverrideCookie) != 'undefined') ? ioverrideCookie.toString() : "false";

        //Widget Creation for Weather icons
        var isWidget = (typeof (iWidget) != 'undefined') ? iWidget.toString() : "false";

        //Multi Cookie Implementation -Begin
        //LocationInfo is the default cookie used incase if no clientid is specified
        //change Default cookie of embed js :- EmbedJsDefCookie
        // this should be as the variable defined in agricast handler.
        //change "LocationInfo" to "EmbedJsDefCookie"
        var clientID = (typeof (iclientID) != 'undefined') ? iclientID.toString() : "EmbedJsDefCookie";
        //Multi Cookie Implementation -End
        //Units Implementation for Embed Js - Begin
        var unit = (typeof (iunit) != 'undefined') ? iunit.toString() : "metric";

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
            'overrideCookie': overrideCookie,
            'idPrefix': clientID,
            'maxAutoSuggestOptions': 10,
            'isWidget': isWidget,
            'clientID': clientID, // Multi Cookie Implementation
            'unit': unit  //Units Implementation for Embed Js

        };
        //$(document).ready(function () {

            //initialize AgriCast object
            AgriCast(config);

            //2012-dec-04 , publication specific CSS - Begin
            var pubSpecificCSS = protocol + server + application + 'pub/' + pub + '/EmbedSite' + '.css';
            AddCSS(pubSpecificCSS);

            //2012-dec-04 , publication specific CSS - End

            //if script is added in head or anywhere in body other than div or span
            //then create a new div and add agricast contents to that div and add this
            //div to body.
            //            if (Tag.nodeName != 'DIV' && Tag.nodeName != 'SPAN') {
            //               
            document.write('<div id=' + clientID + ' class="ag_tag"></div>');


            AgriCast.LoadSearch(clientID, culture, country, place);
            //            }
            //            else {
            //                //if div or span tag is not having an id then add an id to it.
            //                if ($.trim(Tag.id) != "") {
            //                    AgriCast.LoadSearch(clientID);
            //                }
            //                else {

            //                    AgriCast.LoadSearch(clientID);
            //                    //AgriCast.LoadModules('WeatherForecast',Tag.id);
            //                }

            //            }

        //});  //end of document ready function 

    }; //end of callback function



//})(this);


