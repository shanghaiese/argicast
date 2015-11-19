
(function (window, undefined) {

    function getCookie(name) {
        ////debugger;
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0)
                return c.substring(nameEQ.length, c.length);
        }
        return null;
    };

    // parseUri 1.2.2
    // (c) Steven Levithan <stevenlevithan.com>
    // MIT License 
    function parseUri(str) {
        var o = parseUri.options,
		m = o.parser[o.strictMode ? "strict" : "loose"].exec(str),
		uri = {},
		i = 14;

        while (i--) uri[o.key[i]] = m[i] || "";

        uri[o.q.name] = {};
        uri[o.key[12]].replace(o.q.parser, function ($0, $1, $2) {
            if ($1) uri[o.q.name][$1] = $2;
        });

        return uri;
    };

    parseUri.options = {
        strictMode: false,
        key: ["source", "protocol", "authority", "userInfo", "user", "password", "host", "port", "relative", "path", "directory", "file", "query", "anchor"],
        q: {
            name: "queryKey",
            parser: /(?:^|&)([^&=]*)=?([^&]*)/g
        },
        parser: {
            strict: /^(?:([^:\/?#]+):)?(?:\/\/((?:(([^:@]*)(?::([^:@]*))?)?@)?([^:\/?#]*)(?::(\d*))?))?((((?:[^?#\/]*\/)*)([^?#]*))(?:\?([^#]*))?(?:#(.*))?)/,
            loose: /^(?:(?![^:@]+:[^:@\/]*@)([^:\/?#.]+):)?(?:\/\/)?((?:(([^:@]*)(?::([^:@]*))?)?@)?([^:\/?#]*)(?::(\d*))?)(((\/(?:[^?#](?![^?#\/]*\.[^?#\/.]+(?:[?#]|$)))*\/?)?([^?#\/]*))(?:\?([^#]*))?(?:#(.*))?)/
        }
    };

    function AIS_getDomainName() {
        var url = window.location.hostname;
        var sav = url;
        var result = "";

        // REMOVES '.??.??' OR '.???.??' FROM END - e.g. '.CO.UK', '.COM.AU'
        if (url.match(new RegExp(/\.[a-z]{2,3}\.[a-z]{2}$/i))) {
            url = url.replace(new RegExp(/\.[a-z]{2,3}\.[a-z]{2}$/i), "");

            // REMOVES '.??' or '.???' or '.????' FROM END - e.g. '.US', '.COM', '.INFO', '.INTRA'
        } else if (url.match(new RegExp(/\.[a-z]{2,5}$/i))) {
            url = url.replace(new RegExp(/\.[a-z]{2,5}$/i), "");
        }
        result = sav.replace(url, "");

        if (url.indexOf(".") > -1)
            result = url.substr(url.lastIndexOf(".")) + result;
        else
            result = url + result;

        return (result);
    }; //end of AIS_getDomainName

    function AIS_WriteCookie(name, value) {
        var argv = AIS_WriteCookie.arguments;
        var argc = AIS_WriteCookie.arguments.length;
        var expires = (argc > 2) ? argv[2] : null;
        var path = (argc > 3) ? argv[3] : null;
        var secure = (argc > 5) ? argv[5] : false;

        var longDate = new Date();
        longDate.setFullYear(2020);

        var CookieString = name + "=" + escape(value) +
	((expires == null) ? ("; expires=" + longDate.toGMTString()) : ("; expires=" + expires.toGMTString())) +
	"; path=/" +
	((secure == true) ? "; secure" : "");

        var domain = AIS_getDomainName();
        document.cookie = CookieString;
        //         +
        //        ((domain == null) ? "" : ("; domain=" + domain));
    }; //
    function AIS_deleteCookie(name) {
        document.cookie = name + '=; expires=Thu, 01-Jan-70 00:00:01 GMT;';
    }

    //New Widget Request
    function HideControlsForWidget() {

        $('#cngStat').addClass('hide'); //Hide the Stations link for Widget Request
        //Commented code - Retain Country Dropdown for widgets
        //        $('#' + AgriCast.idPrefix + 'lblCountry').addClass('widgetHide'); //hide country label in search bar
        //        $('#' + AgriCast.idPrefix + 'ddlCountry').addClass('widgetHide'); //hide country dropdown in search bar
        //        // $('#' + AgriCast.idPrefix + 'lblFor').addClass('widgetHide'); //hide country  place label in search bar
        //        $('#' + AgriCast.idPrefix + 'stnName').addClass('widgetHide'); //hide country  Station details in Location Search Div
    };

    function getcookievalues(cookiename) {

        var values = unescape(getCookie(cookiename)).toString().split('|');
        var i = 0;
        var locInfo = {

            'ServiceName': (values[i++]),
            'Module': (values[i++]),
            'Culture': (values[i++]),
            'CountryCode': (values[i++]),
            'placeName': (values[i++]),
            'latitude': (values[i++]),
            'longitude': (values[i++]),
            'OverRideCookie': (values[i++]),
            'Widget': (values[i++]),
            'Unit': (values[i++])


        };

        AgriCast.Pub = locInfo.ServiceName;
        AgriCast.Module = locInfo.Module;
        AgriCast.Culture = locInfo.Culture;
        AgriCast.SelectedCountry = locInfo.CountryCode;
        AgriCast.Place = locInfo.placeName;
        AgriCast.Lat = locInfo.latitude;
        AgriCast.Lng = locInfo.longitude;
        AgriCast.unit = locInfo.Unit;
        AgriCast.OverrideCookie = locInfo.OverRideCookie;
        AgriCast.isWidget = locInfo.Widget;
        AgriCast.clientID = cookiename;
    };

    function readcookievalues(cookiename) {

        var values = unescape(getCookie(cookiename)).toString().split('|');
        var i = 0;
        var locInfo = {

            'ServiceName': (values[i++]),
            'Module': (values[i++]),
            'Culture': (values[i++]),
            'CountryCode': (values[i++]),
            'placeName': (values[i++]),
            'latitude': (values[i++]),
            'longitude': (values[i++]),
            'OverRideCookie': (values[i++]),
            'Widget': (values[i++]),
            'Unit': (values[i++])

        };

        return locInfo;
    };

    /*IM01277740 - New Agricast - add "Select" to country dropdown in both application and embed js - begin */

    function CheckDropDownValue() {

        if ($('#' + AgriCast.idPrefix + 'ddlCountry :selected').val() == "XX") {
            $('#' + AgriCast.idPrefix + 'txtPlace').attr('disabled', 'disabled');
            $('#' + AgriCast.idPrefix + 'btnSearch').attr('disabled', 'true');
        }
        else {
            $('#' + AgriCast.idPrefix + 'txtPlace').removeAttr('disabled');
            $('#' + AgriCast.idPrefix + 'btnSearch').removeAttr('disabled');
        }
    }

    /*IM01277740 - New Agricast - add "Select" to country dropdown in both application and embed js - end */
    var AgriCast = function (config) {
        
        AgriCast.Protocol = config.protocol,
        AgriCast.Server = config.server,
        AgriCast.Application = config.application,
        AgriCast.HttpHandler = config.handler,
        AgriCast.HttpHandlerPath = config.handlerPath,
        AgriCast.Pub = config.pub;
        AgriCast.idPrefix = config.idPrefix;
        AgriCast.DivID = null;
        AgriCast.Key = config.key;
        AgriCast.maxAutoSuggestOptions = config.maxAutoSuggestOptions;
        AgriCast.isWidget = config.isWidget;

        // Multi Cookie Implementation - Begin
        AgriCast.clientID = config.clientID;
        // Multi Cookie Implementation - End

        //Units Implementation for Embed Js - Begin
        AgriCast.unit = config.unit;
        //Units Implementation for Embed Js - End

        //change Default cookie of embed js :- EmbedJsDefCookie
        // this should be as the variable defined in agricast handler.
        //change "LocationInfo" to "EmbedJsDefCookie"
        /*IM01176629- New_Agricast_Embedded_JS_failed_to_get_the_country_from_cookie - BEGIN*/
        //if (config.overrideCookie == '' || config.overrideCookie == 'true' || getCookie('EmbedJsDefCookie') == null) {
        if (config.overrideCookie == 'true' || getCookie(AgriCast.clientID) == null) {
            /*IM01176629- New_Agricast_Embedded_JS_failed_to_get_the_country_from_cookie - END*/

            //AgriCast.Module = config.module;
            //AgriCast.Culture = config.culture;
            //            AgriCast.SelectedCountry = config.country;
            //            AgriCast.Place = unescape(config.place);
            //            AgriCast.Lat = config.lat;
            //            AgriCast.Lng = config.lng;
            //            AgriCast.StationName = "";
            //            AgriCast.StationLat = "";
            //            AgriCast.StationLng = "";
            //            AgriCast.OverrideCookie = true;


            AgriCast.Module = config.module;
            AgriCast.Culture = config.culture;
            AgriCast.SelectedCountry = config.country;
            AgriCast.Place = unescape(config.place);
            AgriCast.Lat = config.lat;
            AgriCast.Lng = config.lng;
            AgriCast.StationName = "";
            AgriCast.StationLat = "";
            AgriCast.StationLng = "";
            AgriCast.OverrideCookie = config.overrideCookie;

            AIS_WriteCookie(AgriCast.clientID, AgriCast.Pub + '|' + AgriCast.Module + '|' + AgriCast.Culture + '|' +
                                                                AgriCast.SelectedCountry + '|' + AgriCast.Place + '|' + AgriCast.Lat + '|' + AgriCast.Lng + '|' +
                                                                 AgriCast.OverrideCookie + '|' + AgriCast.isWidget + '|' +
                                                                 AgriCast.unit);

        }
        else {
            getcookievalues(AgriCast.clientID);

            AgriCast.Culture = config.culture;

        } //end of else 

        return AgriCast;
    };

    AgriCast.ReportError = function (divid, err) {

        $('#' + divid).html('');
        $('#' + divid).html('Some error occurred. Cannot process your request now. Please contact the Administrator.<br/>');
        $('<span style="color:red;">Error Message: ' + err + ' </span>').appendTo($('#' + divid));
    };

    //New widget Request
    AgriCast.LoadSearch = function (divid) {
   
        AgriCast.DivID = divid;
        $.ajax({
            dataType: 'jsonp',
            contentType: "application/json; charset=utf-8",
            responseType: 'jsonp',
            cache: true,
            url: AgriCast.HttpHandlerPath,
            data: "pub=" + AgriCast.Pub + "&method=LocationSearch&culture=" + AgriCast.Culture + "&country=" + AgriCast.SelectedCountry + "&place=" + escape(AgriCast.Place) + "&iswidget=" + AgriCast.isWidget + "&clientID=" + AgriCast.clientID
            + "&unit=" + AgriCast.unit + "&overrideCookie=" + AgriCast.OverrideCookie,
            success: function (data) {
                $('#' + divid).html(data.PgHTML);

                if (AgriCast.Lat != '' && AgriCast.Lng != '') {
                    $('#' + AgriCast.idPrefix + 'divLocSearch').attr('class', 'hide');

                }
                //Disable the Dropdown if the country count is one(1)

                if ($('#' + AgriCast.idPrefix + 'ddlCountry option').length == 1) {
                    $('#' + AgriCast.idPrefix + 'ddlCountry')[0].disabled = true;
                }

                /*IM01277740 - New Agricast - add "Select" to country dropdown in both application and embed js - begin */
                CheckDropDownValue();
                /*IM01277740 - New Agricast - add "Select" to country dropdown in both application and embed js - end */

                //end of change
                //Multi Cookie Impelmentation - Begin
                //jQuery17023714336048654444_1353059112505 was not called issue fix 
                if (AgriCast.SelectedCountry == '') {
                    AgriCast.SelectedCountry = $('#' + AgriCast.idPrefix + 'ddlCountry').val();
                }
                //Multi Cookie Impelmentation - End


               if ($.trim($('#' + AgriCast.idPrefix + 'txtPlace').val()) != '') {
                    /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  BEGIN*/
                    // AgriCast.btnSearchClick(this);
                    AgriCast.btnSearchClick("onLoadSearch");
                    /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  END*/
                }
                /*BEGIN - Jan 14th 2013 - AIS Web Part Issue - show error message when nolocation is defined on first time load of widget - BEGIN*/
                else {
                    //show the Generic Error label with "No location Set error message"
                    $("#" + AgriCast.idPrefix + 'lblGenericError').removeClass('hide').addClass('show');
                }
                /*END - Jan 14th 2013 - AIS Web Part Issue - show error message when nolocation is defined on first time load of widget - END*/

                //hide the image spin when the modules are fully loaded
                $('#ag_imgSpin').addClass('hide');
                $('#ag_imgSpin').hide();
                $("#ag_stnName").hide();

                //New Widget Request
                if (AgriCast.isWidget == 'true') {

                    HideControlsForWidget();
                }
            },
            error: function (request, status, error) {
                AgriCast.ReportError(divid, request.responseText);
                // AgriCast.ReportError(divid, error);
                //                    alert(error.Message);
                //                    alert(request.responseText);
            }
        });

        //bind autocomplete event to newly created textbox
        $('#' + divid).delegate('#' + AgriCast.idPrefix + 'divLocSearch', "focus", function () {

            if (!AgriCast.SelectedCountry) {
                AgriCast.SelectedCountry = $("#" + this.idPrefix + "ddlCountry" + " option:selected").val();
            }

            //unbind the events already added.
            $('#' + AgriCast.idPrefix + 'ddlCountry').unbind('change');
            $('#' + AgriCast.idPrefix + 'btnSearch').unbind('click');

            //add the events again
            $('#' + AgriCast.idPrefix + 'ddlCountry').change(function () {
                /*IM01277740 - New Agricast - add "Select" to country dropdown in both application and embed js - begin */
                CheckDropDownValue();
                /*IM01277740 - New Agricast - add "Select" to country dropdown in both application and embed js - end */
                AgriCast.SelectedCountry = this.value;
                $('#' + AgriCast.idPrefix + 'divLocation').attr('class', 'hide');
                $('#' + AgriCast.idPrefix + 'divStations').attr('class', 'hide');
                $('#' + AgriCast.idPrefix + 'divModMaster').html('');
            });

            $('#' + AgriCast.idPrefix + 'btnSearch').click(AgriCast.btnSearchClick);

            $('#' + AgriCast.idPrefix + 'txtPlace').autocomplete({

                //$('#' + AgriCast.idPrefix + 'txtPlace').click({
                source: function (request, response) {
                    $.ajax({
                        dataType: 'jsonp',
                        contentType: "application/json; charset=utf-8",
                        responseType: 'jsonp',
                        url: AgriCast.HttpHandlerPath,
                        data: "pub=" + AgriCast.Pub + "&method=LocDetAuto&country=" + AgriCast.SelectedCountry + "&culture=" + AgriCast.Culture + "&place=" + escape($('#' + AgriCast.idPrefix + 'txtPlace').val()) + "&clientID=" + AgriCast.clientID + "&unit=" + AgriCast.unit + "&overrideCookie=" + AgriCast.OverrideCookie,
                        success: function (data) {
                            var rows = new Array();
                            var length = (data.geonames[0].totalResultsCount > AgriCast.maxAutoSuggestOptions) ? AgriCast.maxAutoSuggestOptions : data.geonames[0].totalResultsCount;
                            data = data.geoname;
                            if (length != 0)
                                for (var i = 0; i < length; i++) {
                                    rows[i] = { data: data[i],
                                        value: data[i].name,
                                        result: data[i].lat + ',' + data[i].lng,
                                        label: data[i].name + ', ' + data[i].adminName1
                                    };
                                }
                            else {
                                rows[0] = { data: "No data found",
                                    value: "",
                                    result: "",
                                    label: "No data found"
                                };
                            }
                            response(rows);
                        },
                        error: function (request, status, error) {
                            AgriCast.ReportError(divid, error);
                            //                    alert(error.Message);
                            //                    alert(request.responseText);
                        },
                        beforeSend: function () {
                            //hide the search results
                            $('#' + AgriCast.idPrefix + 'divLocation').attr('class', 'hide');
                        }
                    })
                },
                minLength: 3,
                search: function () { $(this).addClass('loading'); },
                open: function () { $(this).removeClass('loading'); },
                select: function (event, ui) {
                    $('#' + AgriCast.idPrefix + 'txtPlace').val(ui.item.value);
                    AgriCast.Lat = ui.item.result.split(',')[0];
                    AgriCast.Lng = ui.item.result.split(',')[1];
                    //$('#' + AgriCast.idPrefix + 'btnSearch').click(ui.item);
                    //autosuggest issue
                    //AgriCast.btnSearchClick(ui.item);
                    AgriCast.btnSearchClick("auto");
                    return false;
                }
            }); //end of autocomplete            
        });
    };

    //Show invalid lat long search error message - begin
    var ValidToLoadModules;
    //Show invalid lat long search error message - end
    AgriCast.btnSearchClick = function (obj) {
       
        if ($.trim($('#' + AgriCast.idPrefix + 'txtPlace').val()) == '') {
            alert('Please enter place name');
            return false;
        }

        AgriCast.Place = unescape($('#' + AgriCast.idPrefix + 'txtPlace').val());

        //Lat Long Format Change - implementation - begin
        var place = AgriCast.Place;
        if (place.indexOf(',') != -1) {
            var placeArray = place.split(',');
            //length will be three if place is specified in the query string as "basel , 47.5,7.6 "
            if (placeArray.length == 3) {
                $('#' + AgriCast.idPrefix + 'txtPlace').val(placeArray[0]);
            }
        }

        //Lat Long Format Change - implementation - end

        //Multiple Location issue
        //Clear the latitude and longitude everytime whenever the search button is clicked.
        //this is to identify if the search is triggered from button click or from the location grid
        // incase of location grid , the lat and long of the place is already assign on the click event of grid row and load stations is called based on this
        if (obj != "tab" && obj != "auto" && obj != "onLoadSearch") {
            /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  Begin*/
            AgriCast.Lat = "";
            AgriCast.Lng = "";
            /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  End*/
        }
        //end of change

        AgriCast.StationName = '';
        //clear the divs for location and modules
        $('#' + AgriCast.idPrefix + 'gvLocation').find("tr:gt(0)").remove();
        $('#' + AgriCast.idPrefix + 'gvStations').find("tr:gt(0)").remove();
        //$('#' + AgriCast.idPrefix + 'gvLocation').html('');
        //$('#' + AgriCast.idPrefix + 'gvStations').html('');
        $('#' + AgriCast.idPrefix + 'divModMaster').html('');


        //generic label for error - begin
        //hide the error message once the location is searched.
        $('#' + AgriCast.idPrefix + 'lblGenericError').removeClass('show').addClass('hide');
        //generic label for error - end

        //show loading for button click
        var imgSrc = $('#' + AgriCast.idPrefix + 'btnSearch')[0].src;
        $('#' + AgriCast.idPrefix + 'btnSearch')[0].src = AgriCast.Protocol + AgriCast.Server + 'Images/vsli1.gif';

        AIS_WriteCookie(AgriCast.clientID, AgriCast.Pub + '|' + AgriCast.Module + '|' + AgriCast.Culture + '|' +
                                                                AgriCast.SelectedCountry + '|' + AgriCast.Place + '|' + AgriCast.Lat + '|' + AgriCast.Lng + '|' +
                                                                 AgriCast.OverrideCookie + '|' + AgriCast.isWidget + '|' +
                                                                 AgriCast.unit);
        //get the data from server
        $.ajax({
            dataType: 'jsonp',
            contentType: "application/json; charset=utf-8",
            responseType: 'jsonp',
            cache: true,
            url: AgriCast.HttpHandlerPath,
            data: "pub=" + AgriCast.Pub + "&method=LocDetStn&module=" + AgriCast.Module + "&country=" + AgriCast.SelectedCountry + "&culture=" + AgriCast.Culture + "&place=" + escape(AgriCast.Place) + "&lat=" + AgriCast.Lat + "&lng=" + AgriCast.Lng + "&iswidget=" + AgriCast.isWidget + "&clientID=" + AgriCast.clientID + "&unit=" + AgriCast.unit + "&overrideCookie=" + AgriCast.OverrideCookie,
            success: function (data) {
                //Locations
                var actData = data;
                var length = data.geonames[0].totalResultsCount;
                data = data.geoname;
                //                $('#' + AgriCast.idPrefix + 'gvLocation').append("<tr><th>Name</th></tr>");
                if (length > 0) {
                    $('#' + AgriCast.idPrefix + 'Loc_NoMatchFor').removeClass('show').addClass('hide');
                    $('#' + AgriCast.idPrefix + 'gvLocation').show();
                    $('#' + AgriCast.idPrefix + 'gvStations').show();
                }
                else {
                    $('#' + AgriCast.idPrefix + 'Loc_NoMatchFor').removeClass('hide').addClass('show');
                    $('#' + AgriCast.idPrefix + 'gvLocation').hide();
                    $('#' + AgriCast.idPrefix + 'gvStations').hide();
                    $("#ag_stnName").hide();
                }

                for (var i = 0; i < length; i++) {
                    if (AgriCast.isWidget != 'true') {
                        $('#' + AgriCast.idPrefix + 'gvLocation > tbody:last').append('<tr id=' + i + '><td style="border:1px solid grey ; padding-left:5px; cursor:pointer;">' + data[i].name + '</td>' +
                                                                         '<td style="border:1px solid grey ; padding-left:5px; cursor:pointer;">' + data[i].lat + '</td>' +
                                                                         '<td style="border:1px solid grey ; padding-left:5px; cursor:pointer;">' + data[i].lng + '</td>' +
                                                                         '<td style="border:1px solid grey ; padding-left:5px; cursor:pointer;">' + data[i].adminName1 + '</td>' +
                                                                         '<td style="border:1px solid grey ; padding-left:5px; cursor:pointer;">' + data[i].adminName2 + '</td></tr>');
                        //$('#' + AgriCast.idPrefix + 'gvLocation').append("<tr id='" + i + "'><td>" + data[i].name + "</td></tr>");

                    }
                    else {
                        $('#' + AgriCast.idPrefix + 'gvLocation > tbody:last').append('<tr id=' + i + '><td style="border:1px solid grey ; padding-left:5px; cursor:pointer;">' + data[i].name + ',' + data[i].adminName1 + '</td></tr>');
                    }
                };

                $('#' + AgriCast.idPrefix + 'gvLocation td').each(function (inx) {
                    $(this).click(function () {
                        AgriCast.Place = unescape(data[this.parentNode.id].name);
                        //Multiple Locations issue - 16th November
                        //Take the place from textbox only incase of buton searchand auto suggest.
                        $('#' + AgriCast.idPrefix + 'txtPlace').val(data[this.parentNode.id].name);
                        //not from locations grid click

                        //End of change - 16th November

                        AgriCast.Lat = data[this.parentNode.id].lat;
                        AgriCast.Lng = data[this.parentNode.id].lng;
                        AgriCast.btnSearchClick("tab");
                        //AgriCast.LoadModules(AgriCast.Module);
                    });
                });

                $('#' + AgriCast.idPrefix + 'btnSearch')[0].src = imgSrc;

                //Stations
                //Show invalid lat long search error message - begin
                var lengthStn = 0;
                var dataStn = null;
                ValidToLoadModules = "true";

                if (typeof actData.totalStations != "undefined") {
                    lengthStn = actData.totalStations[0].Total;
                    dataStn = actData.Stations;
                }
                else {
                    ValidToLoadModules = "false";
                }
                //Show invalid lat long search error message - end

                if (lengthStn > 1) {
                    $('#' + AgriCast.idPrefix + 'Loc_NoStation').removeClass('show').addClass('hide');
                    $('#' + AgriCast.idPrefix + 'gvStations').show();
                    //$('#' + AgriCast.idPrefix + 'gvStations').append("<tr><th>Station Name</th><th>Distance</th></tr>");
                    for (var i = 0; i < lengthStn; i++) {
                        $('#' + AgriCast.idPrefix + 'gvStations > tbody:last').append('<tr id=' + i + '><td style="border:1px solid grey; padding-left:5px;">' + dataStn[i].Name + '</td><td style="border:1px solid grey; padding-left:5px;">' + dataStn[i].DistText + '</td></tr>');
                        // $('#' + AgriCast.idPrefix + 'gvStations').append("<tr id='" + i + "'><td>" + dataStn[i].Name + "</td><td>" + dataStn[i].DistText + "</td></tr>");
                    };

                    //set the station name and other details
                    /*IM01175094 - New Agricast - Embedded JS - showing/hidding the locationinformation - BEGIN*/
                    //hide this div always
                    //  $("#ag_stnName").show();
                    $("#ag_lblStnName")[0].innerHTML = dataStn[0].Name;
                    $("#ag_lblDistText")[0].innerHTML = dataStn[0].DistText;

                    $('#' + AgriCast.idPrefix + 'gvStations td').each(function (inx) {
                        $(this).click(function () {
                            AgriCast.StationName = dataStn[this.parentNode.id].Name;
                            AgriCast.StationLat = dataStn[this.parentNode.id].Latitude;
                            AgriCast.StationLng = dataStn[this.parentNode.id].Longitude;
                            $("#ag_lblStnName")[0].innerHTML = dataStn[this.parentNode.id].Name;
                            $("#ag_lblDistText")[0].innerHTML = dataStn[this.parentNode.id].DistText;
                            AgriCast.LoadModules(AgriCast.Module);
                        });
                    });
                }
                else if (lengthStn == 1) {
                    //set the station name and other details
                    //$('#' + AgriCast.idPrefix + 'ag_stnName').show();
                    $('#' + AgriCast.idPrefix + 'lblStnName')[0].innerHTML = dataStn[0].Name;
                    $('#' + AgriCast.idPrefix + 'lblDistText')[0].innerHTML = dataStn[0].DistText;
                    $('#' + AgriCast.idPrefix + 'gvStations > tbody > tr:first').hide();
                    $('#' + AgriCast.idPrefix + 'gvStations > tbody:last').append('<tr id=1><td style="border:1px solid grey; padding-left:5px;">No nearby station found.</td></tr>');
                    //$('#' + AgriCast.idPrefix + 'gvStations').append("<tr><td>No Stations Found</td></tr>");

                    // $('#' + AgriCast.idPrefix + 'Loc_NoStation').removeClass('hide').addClass('show');

                }
                else {
                    //$('#' + AgriCast.idPrefix + 'gvStations').append("<tr><td>No Stations Found</td></tr>");
                    //$('#' + AgriCast.idPrefix + 'Loc_NoStation').removeClass('hide').addClass('show');

                    $('#' + AgriCast.idPrefix + 'gvStations').hide();
                    $("#ag_stnName").hide();
                    $('#cngStat').click();

                    /*May 13th - changes for No stations found issue begin*/
                    if ($('#' + AgriCast.idPrefix + 'divStations').hasClass('show'))
                        $('#' + AgriCast.idPrefix + 'divStations').removeClass('show').addClass('hide');

                    ValidToLoadModules = "false";
                    /*May 13th - changes for No stations found issue end*/
                }
                //end of stations

                //for location
                //New widget request
                //incase of mulitple locations , show the locations grid

                if ((length > 1 && obj != "tab") && (obj.result == null || obj.result == "")) {
                    $('#' + AgriCast.idPrefix + 'divLocation').attr('class', 'show');
                    $('#' + AgriCast.idPrefix + 'divLocSearch').attr('class', 'container');

                }

                else if (length == 1) {

                    if (ValidToLoadModules == "true") {
                        //mutliple locations issue
                        //set the searched location's latitude and longitude
                        AgriCast.Lat = data[0].lat;
                        AgriCast.Lng = data[0].lng;
                        /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  Begin*/
                        if (typeof data[0].countryCode != "undefined") {
                            AgriCast.SelectedCountry = data[0].countryCode;
                            $('#' + AgriCast.idPrefix + 'ddlCountry').val(data[0].countryCode);
                        }
                        AIS_WriteCookie(AgriCast.clientID, AgriCast.Pub + '|' + AgriCast.Module + '|' + AgriCast.Culture + '|' +
                                                                AgriCast.SelectedCountry + '|' + AgriCast.Place + '|' + AgriCast.Lat + '|' + AgriCast.Lng + '|' +
                                                                 AgriCast.OverrideCookie + '|' + AgriCast.isWidget + '|' +
                                                                 AgriCast.unit);
                        /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  Begin*/
                        AgriCast.LoadModules(AgriCast.Module); //Load module only if the location is found
                    } else {

                        /*May 13th - changes for No stations found issue begin*/
                        //if the location is found and no stations are found
                        if (length != 0 && lengthStn == 0) {
                            if ($('#' + AgriCast.idPrefix + 'lblGenericError').hasClass('hide')) {

                                $('#' + AgriCast.idPrefix + 'lblGenericError').removeClass('hide').addClass('show');
                            }
                            //Fetch the Error Message from the Datable and assign it
                            if (typeof actData.ErrorMessages != "undefined") {
                                var strValue = actData.ErrorMessages[0].Message;
                                $('#' + AgriCast.idPrefix + 'lblGenericError').html(strValue);
                            }
                        }
                        else {
                            /*May 13th - changes for No stations found issue End*/
                            var lblLocMatchFor = $('#' + AgriCast.idPrefix + 'Loc_NoMatchFor').html();

                            if (typeof actData.ErrorMessages != "undefined") {
                                var strValue = actData.ErrorMessages[0].Message;
                                $('#' + AgriCast.idPrefix + 'lblGenericError').html(strValue);

                                $('#' + AgriCast.idPrefix + 'lblGenericError').removeClass('hide').addClass('show');

                            }
                        }
                    }

                }
                else {

                    /*May 13th - changes for No stations found issue End*/
                    //show the location div
                    //but the location grid will be hidden since no location found
                    //COOKIE ISSUE 
                    //RE ASSIGN THE MESSAGE OF Loc_NoMatchFor LABEL
                    // MESSGAE FORMAT : TRANSLATED TEXT 'PLACE NAME'
                    // GET THE TEXT WITH THE QUOTES AND REPLACE WITH CURRENT LOCATION NAME
                    //IF THE MESSAGE FORMAT IS CHANGED IN DB, THIS WILL NOT WORK


                    /*May 13th - changes for No stations found issue - Begin */
                    /* commented for changes for No stations found issue 
                    var lblLocMatchFor = $('#' + AgriCast.idPrefix + 'Loc_NoMatchFor').html();
                    var strtIndex = lblLocMatchFor.indexOf("'");
                    var endIndex = lblLocMatchFor.lastIndexOf("'");
                    var strValue = lblLocMatchFor.replace(lblLocMatchFor.substring(strtIndex + 1, endIndex), AgriCast.Place);
                    *changes for No stations found issue 
                    */

                    //                    $('#' + AgriCast.idPrefix + 'Loc_NoMatchFor').html(strValue);
                    //                    $('#' + AgriCast.idPrefix + 'divLocation').attr('class', 'show');
                    //                    $('#' + AgriCast.idPrefix + 'divStations').removeClass('show').addClass('hide');

                    if (typeof lblLocMatchFor != "undefined" && lblLocMatchFor.val().length > 0)
                        var strValue = lblLocMatchFor.substring(0, lblLocMatchFor.indexOf("'")) + " - " + AgriCast.Place;
                    /*May 13th - changes for No stations found issue End*/
                    $('#' + AgriCast.idPrefix + 'lblGenericError').html(strValue);

                    $('#' + AgriCast.idPrefix + 'lblGenericError').removeClass('hide').addClass('show');
                }


            },
            error: function (request, status, error) {


                AgriCast.ReportError(AgriCast.DivID, request.responseText);
                //    AgriCast.ReportError(divid, error);
                // alert(error.Message);
                //alert(request.responseText);
            }

        });

        return false;
    };

    AgriCast.ChangeLocation = function (divid) {
        //hide location search controls
        //divLocSearch, divLocation, divStations
        $('#' + AgriCast.idPrefix + 'divLocSearch').attr('class', 'hide');
        $('#' + AgriCast.idPrefix + 'divLocation').attr('class', 'hide');
        $('#' + AgriCast.idPrefix + 'divStations').attr('class', 'hide');
        if ($('#' + AgriCast.idPrefix + 'close').length != 0) {
            $('#' + AgriCast.idPrefix + 'close').remove();
        };

        var closeImg = AgriCast.Protocol + AgriCast.Server + 'Images/minus.png';

        $('#' + AgriCast.idPrefix + 'ChangeLoc').removeClass("hide");
        $('#' + AgriCast.idPrefix + 'ChangeLoc').addClass("show");

        if ($('#' + AgriCast.idPrefix + 'ChangeLoc').length != 0) {
            // $('<div id="' + AgriCast.idPrefix + 'ChangeLoc"><a id="cngLoc" href="">Change Location</a>&nbsp;&nbsp;<a id="cngStat" href="">NearByStations</a></div>').prependTo($('#' + AgriCast.idPrefix + 'divModMaster').parent('div'));            
            //$('#'+AgriCast.idPrefix + 'ChangeLoc').toggle();            
        };

        $('#' + AgriCast.idPrefix + 'ChangeLoc a[id=cngLoc]').click(function () {
            //New Widget request
            //add the new css class for divLocsearch incase of widget
            if (AgriCast.isWidget == 'true') {
                $('#' + AgriCast.idPrefix + 'divLocSearch').attr('class', 'widgetContainer'); //Width of Location Search div
            }
            else {
                $('#' + AgriCast.idPrefix + 'divLocSearch').attr('class', 'container');
            }
            //end of change

            //$('#' + AgriCast.idPrefix + 'divLocation').attr('class', 'show');

            //New Widget request

            if (AgriCast.isWidget != 'true') {
                /*IM01175094 - New Agricast - Embedded JS - showing/hidding the locationinformation - BEGIN*/
                // Issue - there are two div displaying the station details "divStationDetails" and "ag_stnName"
                // Fix - hide the ag_stnname div 
                //hide the station details when change location is clicked
                // $('#' + AgriCast.idPrefix + 'divStationDetails').attr('class', 'hide');
                $('#' + AgriCast.idPrefix + 'divStationDetails').attr('class', 'show StationDetails');

                /*IM01175094 - New Agricast - Embedded JS - showing/hidding the locationinformation - END*/
            }

            //commented for multiple locations issue
            //            if (AgriCast.isWidget == 'true') {
            // $('#' + AgriCast.idPrefix + 'divLocation').addClass('widgetDivLocation'); // alignment of the location div
            $('#' + AgriCast.idPrefix + 'divLocation').attr('class', 'hide');
            //            }
            //            else {
            //                $('#' + AgriCast.idPrefix + 'divLocation').attr('class', 'show');
            //            }
            //end of change

            $('#' + AgriCast.idPrefix + 'divStations').attr('class', 'hide');

            if ($('#' + AgriCast.idPrefix + 'close').length == 0) {
                $('<div id="' + AgriCast.idPrefix + 'close" class="btnclose"></div>"').appendTo($('#' + AgriCast.idPrefix + 'ChangeLoc'));
            }
            $('#' + AgriCast.idPrefix + 'close').click(function () {
                $('#' + AgriCast.idPrefix + 'divLocSearch').attr('class', 'hide');
                $('#' + AgriCast.idPrefix + 'divLocation').attr('class', 'hide');
                $('#' + AgriCast.idPrefix + 'divStations').attr('class', 'hide');
                //Show invalid lat long search error message - begin
                $('#' + AgriCast.idPrefix + 'lblGenericError').removeClass('show').addClass('hide');
                //Show invalid lat long search error message - end

                //New Widget Request

                /*IM01175094 - New Agricast - Embedded JS - showing/hidding the locationinformation - BEGIN*/
                // Issue - there are two div displaying the station details "divStationDetails" and "ag_stnName"
                // Fix - hide the ag_stnname div 
                //show the station details when change location is clicked
                // $('#' + AgriCast.idPrefix + 'divStationDetails').attr('class', 'show');
                $('#' + AgriCast.idPrefix + 'divStationDetails').attr('class', 'show StationDetails');
                /*IM01175094 - New Agricast - Embedded JS - showing/hidding the locationinformation - END*/
                //end of change
                //$(this).parent('div').remove();
                $(this).remove();
                return false;
            });
            //$('#' + AgriCast.idPrefix + 'ChangeLoc').remove();
            return false;
        });

        $('#' + AgriCast.idPrefix + 'ChangeLoc a[id=cngStat]').click(function () {
            $('#' + AgriCast.idPrefix + 'divLocSearch').attr('class', 'hide');
            $('#' + AgriCast.idPrefix + 'divLocation').attr('class', 'hide');
            $('#' + AgriCast.idPrefix + 'divStations').attr('class', 'show');

            if ($('#' + AgriCast.idPrefix + 'close').length == 0) {
                $('<div id="' + AgriCast.idPrefix + 'close" class="btnclose"></div>"').appendTo($('#' + AgriCast.idPrefix + 'ChangeLoc'));
            }
            $('#' + AgriCast.idPrefix + 'close').click(function () {
                $('#' + AgriCast.idPrefix + 'divLocSearch').attr('class', 'hide');
                $('#' + AgriCast.idPrefix + 'divLocation').attr('class', 'hide');
                $('#' + AgriCast.idPrefix + 'divStations').attr('class', 'hide');
                $(this).remove();
                return false;
            });

            //$('#' + AgriCast.idPrefix + 'ChangeLoc').remove();
            return false;
        });

    };

    AgriCast.LoadModules = function (mod, divid) {

        String.prototype.startsWith = function (prefix) {
            return this.indexOf(prefix) === 0;
        };
        String.prototype.endsWith = function (suffix) {
            return this.match(suffix + "$") == suffix;
        };

        AgriCast.DivID = divid;
        var place = '';
        if (AgriCast.Place != null) {
            place = unescape(AgriCast.Place);
        }
        else {
            if (typeof ($('#' + AgriCast.idPrefix + 'txtPlace').val()) != 'undefined') {
                place = $('#' + AgriCast.idPrefix + 'txtPlace').val();
            }

        };

        //remove the spin gif from main div if present
        if ($('#' + divid + ' img')[0] != null && $('#' + divid + ' img')[0].src.toUpperCase().indexOf('SPIN.GIF') != '-1') {
            $('#' + divid).html('');
        };

        //create the holder div if not present
        if ($('#' + AgriCast.idPrefix + 'divModMaster').length == 0) {
            $('#' + divid).append('<div id="' + AgriCast.idPrefix + 'divModMaster"></div>');
        };

        AgriCast.ChangeLocation(divid);

        //New Widget Request
        //these controls are added from handler.. any changes in the control names will affect this script
        if (AgriCast.isWidget == 'true') {
            HideControlsForWidget();

        }



        $('#' + AgriCast.idPrefix + 'divModMaster').html('');
        $('#' + AgriCast.idPrefix + 'divModMaster').html('<img src=' + AgriCast.Protocol + AgriCast.Server + 'Images/spin.gif alt="Loading..." />');
        $.ajax({
            dataType: 'jsonp',
            contentType: "application/json; charset=utf-8",
            responseType: 'jsonp',
            cache: true,
            url: AgriCast.HttpHandlerPath,
            data: "pub=" + AgriCast.Pub + "&method=GetModules&module=" + mod + "&country=" + AgriCast.SelectedCountry + "&culture=" + AgriCast.Culture + "&place=" + escape(place) + "&lat=" + AgriCast.Lat + "&lng=" + AgriCast.Lng
            + "&stnname=" + AgriCast.StationName + "&iswidget=" + AgriCast.isWidget + "&clientID=" + AgriCast.clientID + "&unit=" + AgriCast.unit + "&overrideCookie=" + AgriCast.OverrideCookie,
            success: function (data) {


                //Agricast Case Closure
                //Get the html of stationdetails div ag_stnName and add it to divModMaster
                //Add the station details only for non widget request
                if (typeof data != "undefined") //this occurs when the first time load if the location does not belong to the country selected
                {
                    /*IM01175094 - New Agricast - Embedded JS - showing/hidding the locationinformation - BEGIN*/
                    // Issue - there are two div displaying the station details "divStationDetails" and "ag_stnName"
                    // Fix - hide the ag_stnname div 

                    if (AgriCast.isWidget != "true") {
                        //Change the display format of station Details
                        // var strStnDetail = $('#ag_stnName').html();
                        var strStnDetail = "";
                        $('#ag_stnName span').each(function () {
                            if (strStnDetail == "")
                                strStnDetail = $(this).text();
                            else
                                strStnDetail = strStnDetail + ' - ' + $(this).text();
                        });


                        var ctrl = $('<div />').attr('id', AgriCast.idPrefix + 'divStationDetails').addClass('StationDetails').html(strStnDetail);
                        $('#' + AgriCast.idPrefix + 'divModMaster').html(ctrl);
                        $('#' + AgriCast.idPrefix + 'divModMaster').append(data.ModHTML);

                        /* Hide the Bigger Chart Hover Text - Begin*/
                        //$('#' + AgriCast.idPrefix +'_tag '+ 'imgChart').removeAttr('title');
                        $('#imgChart').removeAttr('title');
                        $('#imgChart').removeAttr('style');
                        $('#imgChart').removeAttr('onclick');
                        /* Hide the Bigger Chart Hover Text - End*/
                    }
                    else {
                        $('#' + AgriCast.idPrefix + 'divModMaster').html(data.ModHTML);
                    }
                }
                /*IM01175094 - New Agricast - Embedded JS - showing/hidding the locationinformation - END*/
                //end of change
                //change the image file paths
                $('#' + AgriCast.idPrefix + 'divModMaster img').each(function () {
                    if (parseUri(this.src).directory.toLowerCase().endsWith('/images/')) {
                        this.src = AgriCast.Protocol + AgriCast.Server + 'Images/' + parseUri(this.src).file;
                    }
                    else if (parseUri(this.src).directory.toLowerCase().endsWith('/images/icons/')) {
                        this.src = AgriCast.Protocol + AgriCast.Server + 'Images/Icons/' + parseUri(this.src).file;
                    }
                    else if (parseUri(this.src).directory.toLowerCase().endsWith('/temp/cfx/')) {
                        this.src = AgriCast.Protocol + AgriCast.Server + 'temp/cfx/' + parseUri(this.src).file;
                    }
                    /*Wind icons for embed js - BEGIN*/
                    else if (parseUri(this.src).directory.toLowerCase().endsWith('/temp/icons/')) {
                        this.src = AgriCast.Protocol + AgriCast.Server + 'temp/icons/' + parseUri(this.src).file;
                    }
                    /*Wind icons for embed js - END*/
                    else {
                        this.src = AgriCast.Protocol + AgriCast.Server + parseUri(this.src).path.substring(1, parseUri(this.src).path.length);
                    }

                });
                $('.RateIt').rateit({
                    starwidth: 16,
                    starheight: 16,
                    min: 0,
                    max: 5,
                    step: 1,
                    resetable: false
                });
                var t;
                $('div[data-accessKey="rate"]').bind('rated', function (event, value) {

                    var y;
                    var x = $(this).attr('id') + '%' + value;
                    if ($('#hdnRate').val() == "")
                        y = x;
                    else
                        y = $('#hdnRate').val() + ',' + x;

                    $('#hdnRate').val(y);

                    t = y;
                    $('#hidden').val(t);
                    $('hdnText').attr('text', t);
                    $('hdnText').val(t);

                    $.ajax({
                        responseType: "jsonp",
                        contentType: "application/json; charset=utf-8",
                        url: AgriCast.Protocol + AgriCast.Server + AgriCast.Application + "AgriCastRating.ashx",
                        data: "rate=" + x + "&module=" + AgriCast.Module + "&service=" + AgriCast.Pub,
                        dataType: "jsonp",
                        success: function (data) {
                            var mes = data.msg;
                        }
                    });
                });

                //add click event handler to image plus and minus to expand/collapse the divs.
                $('.divOuter img').click(function () {

                    if ($(this).attr('id').startsWith('img1')) {

                        var gridId = $(this).attr('id').substring(5);
                        var divId = "div_" + gridId;
                        var scope = $(this).parent('div').parent('div');
                        var plus = AgriCast.Protocol + AgriCast.Server + 'Images/boxplus.gif';
                        var minus = AgriCast.Protocol + AgriCast.Server + 'Images/boxminus.gif';

                        if (this.src.toUpperCase().indexOf('MINUS') != '-1') {

                            $('#' + gridId, scope).hide();
                            $('#divL_' + gridId, scope).hide();
                            $('#lbl_' + gridId, scope).hide();
                            $('#divLegend_' + gridId, scope).hide();
                            $('#divRulesetL_' + gridId, scope).hide();
                            this.src = plus;
                        }
                        else {

                            $('#' + gridId, scope).show();
                            $('#divL_' + gridId, scope).show();
                            $('#lbl_' + gridId, scope).show();
                            $('#divLegend_' + gridId, scope).show();
                            $('#divRulesetL_' + gridId, scope).show();
                            this.src = minus;
                        };
                    }

                    else if ($(this).attr('id').startsWith('imgSeries')) {
                        if (this.src.toUpperCase().indexOf('MINUS') != '-1') {
                            if ($(this).attr('id').startsWith('imgSeriesDaily')) {
                                $('#DivDaily', scope).hide();
                                this.src = plus;
                            }
                            if ($(this).attr('id').startsWith('imgSeriesExtreme')) {
                                $('#DivExtreme', scope).hide();
                                this.src = plus;
                            }
                            if ($(this).attr('id').startsWith('imgSeriesLongTerm')) {
                                $('#DivLongTerm', scope).hide();
                                this.src = plus;
                            }
                            if ($(this).attr('id').startsWith('imgSeriesExpand')) {
                                $('#divSeries', scope).hide();
                                this.src = plus;
                            }
                            if ($(this).attr('id').startsWith('imgSeriesCrop')) {
                                $('#divCrop', scope).hide();
                                this.src = plus;
                            }
                        }
                        else {
                            if ($(this).attr('id').startsWith('imgSeriesDaily')) {
                                $('#DivDaily', scope).show();
                                this.src = minus;
                            }
                            if ($(this).attr('id').startsWith('imgSeriesExtreme')) {
                                $('#DivExtreme', scope).show();
                                this.src = minus;
                            }
                            if ($(this).attr('id').startsWith('imgSeriesLongTerm')) {
                                $('#DivLongTerm', scope).show();
                                this.src = minus;
                            }
                            if ($(this).attr('id').startsWith('imgSeriesExpand')) {
                                $('#divSeries', scope).show();
                                this.src = minus;
                            }
                            if ($(this).attr('id').startsWith('imgSeriesCrop')) {
                                $('#divCrop', scope).show();
                                this.src = minus;
                            }

                        }

                    }


                    //prevent postback
                    return false;
                }); //end of img click


            },
            error: function (request, status, error) {
                AgriCast.ReportError(AgriCast.DivID, request.responseText);
                // AgriCast.ReportError(divid, error);
                //                    alert(error.Message);
                //                    alert(request.responseText);
            }

        });

    };

    window.AgriCast = AgriCast;

})(window);