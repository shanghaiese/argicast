var map;
var marker;
function plotgoogle(displayedGrid, locs) {


    var infowindow;
    var latlng;
    var myOptions;
    var infoOptions;

    if (displayedGrid == "#LocList") {
        var locationArray = $.map(locs, function (location, index) { return ([new google.maps.LatLng(locs[index][1],locs[index][2])]) });
        locationArray = locationArray.splice(1, locationArray.length);
        var locationNameArray = $.map(locs, function (location, index) { return (locs[index][0]) });
    }
    else {
        if ($("#hdnLocSelected").val() == "0,0,") {
            $("#hdnLocSelected").val($("#hdnLatLng").val());
        }
        var Parent = $("#hdnLocSelected").val() != undefined? $("#hdnLocSelected").val() : $("#hdnLatLng").val();
        var locationArray = $.map(locs, function (location, index) { return ([new google.maps.LatLng(locs[index][4],locs[index][3],4)]) });
        locationArray = locationArray.splice(1, locationArray.length);
        var locationNameArray = $.map(locs, function (location, index) { return (locs[index][0]) });        
        if (Parent != "" && Parent.split('|')[3] == "#LocList") {
            var Name = (Parent.split('|')[2]);
            Parent = new google.maps.LatLng(Parent.split('|')[0], Parent.split('|')[1]);
            locationArray.push(Parent);
            locationNameArray.push(Name);
        }
        else {
            Parent = $("#hdnLocSelected").val() != undefined ? $("#hdnLocSelected").val() : "";
            if (Parent != "") {
                var Name = (Parent.split('|')[2]);
                Parent = new google.maps.LatLng(Parent.split('|')[0], Parent.split('|')[1]);
                locationArray.push(Parent);
                locationNameArray.push(Name);
            }
        }
    }

    var bounds = new google.maps.LatLngBounds();

    myOptions = { zoom: 11,
        panControl: false,
        zoomControl: true,
        scaleControl: false,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        scrollwheel: true,
        streetViewControl: false,
        mapTypeControl: false
    };
    map = new google.maps.Map(document.getElementById("mapImage"), myOptions);
    var coord;

    //Add for IM01894118:Agricast V2 - forcing a resizing of the page when updated - 20140627 - start
    google.maps.event.addListenerOnce(map, 'idle', function () {
        if (window["resizeMe"])
        {
            window["resizeMe"]();
        }
    });
    //Add for IM01894118:Agricast V2 - forcing a resizing of the page when updated - 20140627 - end

    for (coord in locationArray) {
        if (coord == (locationArray.length - 1) && displayedGrid == "#stnList") { //(locationArray.length - 1)

            marker = new google.maps.Marker({
                position: locationArray[coord],
                map: map,
                title: (parseFloat((locationArray[coord].toString().split(',')[0].substring(1)))).toFixed(4) + ',' + (parseFloat((locationArray[coord].toString().split(',')[1].substring(locationArray[coord].toString().count)))).toFixed(4),
                icon: new google.maps.MarkerImage("../../Images/marker_greenP.png"),
                clickable: false
            });
        }
        else if (displayedGrid == "#LocList") {
            marker = new google.maps.Marker({
                position: locationArray[coord],
                map: map,
                title: (parseFloat((locationArray[coord].toString().split(',')[0].substring(1)))).toFixed(4) + ',' + (parseFloat((locationArray[coord].toString().split(',')[1].substring(locationArray[coord].toString().count)))).toFixed(4) + ';' + locationNameArray[coord].toString(),
                icon: new google.maps.MarkerImage("../../Images/marker_greenP.png")
            });
        }
        else {
            marker = new google.maps.Marker({
                position: locationArray[coord],
                map: map,
                title: (parseFloat((locationArray[coord].toString().split(',')[0].substring(1)))).toFixed(4) + ',' + (parseFloat((locationArray[coord].toString().split(',')[1].substring(locationArray[coord].toString().count)))).toFixed(4) + ';' + locationNameArray[coord].toString(),
                icon: new google.maps.MarkerImage("../../Images/markerS.png")
            });

        }

        var place = locationNameArray[coord].toString();
        google.maps.event.addListener(marker, 'click', function (event) {

            toggleBounce(event.latLng, place, displayedGrid);
        });
        bounds.extend(locationArray[coord]);

    }
    map.setCenter(bounds.getCenter());
    if (coord.length > 0)
        map.fitBounds(bounds);
}
function toggleBounce(point, place, displayedGrid) {
    var input = String(point).substr(1, String(point).length - 2);
    var lat = parseFloat(input.split(",")[0]);
    var lng = parseFloat(input.split(",")[1]);
    var latlng = new google.maps.LatLng(lat, lng);
    if (displayedGrid == "#LocList") {
        locs = locs.splice(1, locs.length);
        var locationArray = $.map(locs, function (location, index) { return ([new google.maps.LatLng(locs[index][1], locs[index][2])]) });
        for (coord in locs) {
            if (locationArray[coord].lat().toString()==(lat.toString()) && locationArray[coord].lng().toString()==(lng.toString())) {
                place = locs[coord][0];
                break;
            }
        }

    }
    var strLatLng = (lat + '|' + lng + '|' + place + '|' + displayedGrid);

    $("#hdnLatLng").val(strLatLng);
    if (displayedGrid == "#LocList") {
        $("#hdnLocSelected").val(strLatLng);
    }
    $("#divLocation").addClass('hide');
    __doPostBack();


    //        geocoder.geocode({ 'latLng': latlng }, function (results, status) {
    //            if (status == google.maps.GeocoderStatus.OK) { 
    //                if (results[1]) {
    //                    infoOptions = {
    //                        position: latlng,
    //                        pixelOffset: new google.maps.Size(3, -25),
    //                        content: place
    //                    };
    //                    infowindow = new google.maps.InfoWindow(infoOptions);
    //                    infowindow.open(map);
    //                } else {
    //                    alert("No results found");
    //                }
    //            } else {
    //                alert("Geocoder failed due to: " + status);
    //            }
    //                });

    //     


}