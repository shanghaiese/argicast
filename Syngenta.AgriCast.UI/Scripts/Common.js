

function common() {
    
    $("#searchbox").autocomplete("AutoComplete.ashx", {
        width: 250,
        scroll: false,
        formatItem: function (row) { return row[0]; },
        formatResult: function (row) { return row[1]; }
    });

    $("#btnSearch").click(function () {
        $("#divLocation .close").click();
    });

    if ($.browser.msie && $.browser.version < 9) {
        $("#ddlCountry").mousedown(function () {
            $(this).css("width", "auto");
        });

        $("#ddlCountry").change(function () {
            var textWidth = (($("#ddlCountry :selected").text().length) * 6 + 35) + "px";
            $(this).css("width", textWidth);

            //clear the contents of search box on drop down change
            $("#searchbox").val("");


        });
    }
    else {
        $("#ddlCountry").change(function () {
            var textWidth = (($("#ddlCountry :selected").text().length) * 6 + 35) + "px";
            $(this).css("width", textWidth);
        });
    }

    /*IM01277740 - New Agricast - add "Select" to country dropdown in both application and embed js - begin */
   // debugger;
    $("#ddlCountry").change(function () {
        CheckDropDownValue();
    });

    function CheckDropDownValue() {
        if ($("#ddlCountry :selected").text() == "--Select--") {
            $("#searchbox").attr('disabled', 'disabled');
            $("#btnSearch").attr('disabled', 'true');
        }
        else {
            $("#searchbox").removeAttr('disabled');
            $("#btnSearch").removeAttr('disabled');
        }
    }

    CheckDropDownValue();
    /*IM01277740 - New Agricast - add "Select" to country dropdown in both application and embed js - end */
    $("#rateit5").bind('rated', function (event, value) { var x = value });

    $("#ddlUnits").change(function () {
        if ($('#ddlUnits option:selected').val().toString() == "Custom...") {

            var offset = $('#ddlUnits').offset();
            var height = $('#ddlUnits').height();
            var width = $('#ddlUnits').width();

            $('#ddlUnits').scrollTop(0);
            $('#ddlUnits').scrollLeft(0);
            var posY = offset.top + height + "px";
            var posX = offset.left + "px";
            $('#divCustom').css({ 'position': 'absolute', 'top': posY, 'left': posX, 'width': 'auto', 'z-index': '100', 'border': '2px solid Grey', 'background': 'lightGrey', 'opacity': '0.8' });

            $('#divCustom').show();
        }
        else {
            __doPostBack(('#ddlUnits'), null);
        }
    });




    $('#btnOK').click(function () {
        $('#divCustom').hide();
    });



    $(".trigger").toggle(

      function () { $('#WeatherMenu').css('width', '2%'); $('#WeatherMenu').css('height', 'auto'); $("#divMenu").hide(); $("#divClose").show(); $("#divClose").removeClass("divHide"); $(this).toggleClass("active"); },
      function () {
          $('#WeatherMenu').css('width', '28%'); $('#WeatherMenu').css('height', 'auto'); $("#divMenu").show(); $("#divClose").hide(); $("#divClose").addClass("divHide"); $(this).toggleClass("active");
      });





    $('#hNearByPoint').click(function () {
        $('#divLocation').removeClass('hide');
        var objGrid = document.getElementById("hdnGridStatus");
        if (objGrid != null) {
            $("#hdnGridStatus").val('Show-Station');

        }
        //Add for IM01894118:Agricast V2 - forcing a resizing of the page when updated - 20140625 - start
        resizeMe();
        //Add for IM01894118:Agricast V2 - forcing a resizing of the page when updated - 20140625 - end
    });



    //Code to check whether name has changed while updating favourites. In case of no change throw an error message. In case of change proceed.
    $("form :input").change(function () { $(this).closest('form').data('changed', true); });
    $('input').click(function () {
        if (this.alt.toUpperCase().indexOf('UPDATE') != '-1') {
            if ($(this).closest('form').data('changed')) {
            }
            else {

                $('#lblError').text('No changes to be saved. Enter a new name or click cancel to proceed.');

                return false;
            }
        }
    });


    /*IM01246233 :- New Agricast - missing translation tags - Begin */
    //Code to alert user in case no name has been entered while saving a favourite.
    //Commented and moved to a new function
    //    $('#Fav_AddToFavorites').click(function () {
    //        if ($('#txtFavName').val().trim() == "") {
    //            alert('Please enter a valid name.');
    //            return false;
    //        }
    //    });



    /*IM01246233 :- New Agricast - missing translation tags - End */
    $('img').bind('click', function () {

        var attrid = $(this).attr('id');
        // For some browsers, 'attr' is undefined; for others, 'attr' is false.  Check for both. 
        if (typeof attrid !== 'undefined' && attrid !== false) {
            if ($(this).attr('id').startsWith('img1')) {

                var gridId = $(this).attr('id').substring(5);
                var divId = "div_" + gridId;

                if (this.src.toUpperCase().indexOf('MINUS') != '-1') {

                    $('#' + gridId).hide();
                    $('#divL_' + gridId).hide();
                    $('#lbl_' + gridId).hide();
                    $('#divLegend_' + gridId).hide();
                    $('#MainContent_divLegend_' + gridId).hide();
                    $('#divRulesetL_' + gridId).hide();
                    $('#ratingContainer_' + gridId).hide()
                    this.src = '../../Images/boxplus.gif';


                }
                else {

                    $('#' + gridId).show();
                    $('#divL_' + gridId).show();
                    $('#lbl_' + gridId).show();
                    $('#divLegend_' + gridId).show();
                    $('#MainContent_divLegend_' + gridId).show();
                    $('#divRulesetL_' + gridId).show();
                    $('#ratingContainer_' + gridId).show()
                    this.src = '../../Images/boxminus.gif';
                };
            }

            else if ($(this).attr('id').startsWith('imgSeries')) {
                if (this.src.toUpperCase().indexOf('MINUS') != '-1') {
                    if ($(this).attr('id').startsWith('imgSeriesNode')) {
                        var x = $(this).attr('id').substring(13);
                        var div = '#Div' + x;
                        $(div).hide();
                        this.src = '../../Images/boxplus.gif';


                        //code to maintain the tree view state
                        var sThisExpanded = this.id;
                        var expanded = $('#hdnExpandCollapse').val();
                        if (expanded != "") {
                            expanded = expanded.replace(sThisExpanded, "");
                            $('#hdnExpandCollapse').attr("value", expanded);
                        }
                    }

                    if ($(this).attr('id').startsWith('imgSeriesExpand')) {
                        $('#divSeries').hide();
                        this.src = '../../Images/boxplus.gif';
                    }
                    if ($(this).attr('id').startsWith('imgSeriesCrop')) {
                        $('#divCrop').hide();
                        this.src = '../../Images/boxplus.gif';
                    }
                    if ($(this).attr('id').startsWith('imgSeriesGDDExpand')) {
                        $('#divGddSeries').hide();
                        this.src = '../../Images/boxplus.gif';
                    }
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                    if ($(this).attr('id').startsWith('imgSeriesSoil')) {
                        $('#divSoil').hide();
                        this.src = '../../Images/boxplus.gif';
                    }
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
                }
                else {
                    if ($(this).attr('id').startsWith('imgSeriesNode')) {
                        var x = $(this).attr('id').substring(13);
                        var div = '#Div' + x;
                        $(div).show();
                        this.src = '../../Images/boxminus.gif';

                        //code to maintain the tree view state
                        var sThisExpanded = this.id;
                        var expanded = $('#hdnExpandCollapse').val();
                        if (expanded != "")
                            expanded = expanded + ';' + sThisExpanded;
                        else
                            expanded = sThisExpanded;
                        $('#hdnExpandCollapse').attr("value", expanded);
                    }

                    if ($(this).attr('id').startsWith('imgSeriesExpand')) {
                        $('#divSeries').show();
                        this.src = '../../Images/boxminus.gif';
                    }
                    if ($(this).attr('id').startsWith('imgSeriesCrop')) {
                        $('#divCrop').show();
                        this.src = '../../Images/boxminus.gif';
                    }
                    if ($(this).attr('id').startsWith('imgSeriesGDDExpand')) {
                        $('#divGddSeries').show();
                        this.src = '../../Images/boxminus.gif';
                    }
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                    if ($(this).attr('id').startsWith('imgSeriesSoil')) {
                        $('#divSoil').show();
                        this.src = '../../Images/boxminus.gif';
                    }
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
                }

            }
            else {

                var attr = $(this).attr('data-controls');
                // For some browsers, 'attr' is undefined; for others, 'attr' is false.  Check for both. 
                if (typeof attr !== 'undefined' && attr !== false) {
                    var ariaControls = '#' + $(this).attr('data-controls');
                    var ariaDescendants = $(this).attr('data-activedescendant');


                    $(ariaControls).removeClass('hide')
                            .addClass('show');

                    var objFav = document.getElementById("hdnGridImages_Status");
                    if (objFav != null) {
                        objFav.value = 'show' + ariaControls;
                    }
                    //split the descendants  
                    var arrDescendants = ariaDescendants.split(';');
                    if (arrDescendants != null) {
                        //loop thru each one 
                        for (var i = 0; i < arrDescendants.length; i++) {
                            //hide each of the descendants
                            $(('#' + arrDescendants[i])).addClass('hide');
                        }
                    }

                    if (ariaControls == "#email") {
                        $("#Subject").val($("#Subject").val().replace('@location', $("#hdnLatLng").val().split('|')[2]));
                        //Add for IM01894118:Agricast V2 - forcing a resizing of the page when updated - 20140625 - start
                        resizeMe();
                        //Add for IM01894118:Agricast V2 - forcing a resizing of the page when updated - 20140625 - end
                    }

                    //Add for IM01894118:Agricast V2 - forcing a resizing of the page when updated - 20140625 - start
                    if (ariaControls == "#feedback") {
                        resizeMe();
                    }
                    //Add for IM01894118:Agricast V2 - forcing a resizing of the page when updated - 20140625 - end

                    //Set the Hidden Field Value :- Favorite Grid Status
                    if (ariaControls == '#favorites') {
                        var objHdnFavGrid = document.getElementById("hdnGridFavoriteStatus");

                        if (objHdnFavGrid != null) {

                            $("#hdnGridFavoriteStatus").val('ShowFavorite');

                        }

                        //Show the Favorites Grid
                        var objGridFav = document.getElementById("gvFavorites")
                        var objGridDiv = document.getElementById("favorites")

                        if (objGridFav != null) {
                            $('#gvFavorites').show();
                        }

                        if (objGridDiv != null) {
                            $('#favorites').show();
                        }
                        //Add for IM01894118:Agricast V2 - forcing a resizing of the page when updated - 20140625 - start
                        resizeMe();
                        //Add for IM01894118:Agricast V2 - forcing a resizing of the page when updated - 20140625 - end
                    }
                    if (ariaControls == '#Print') {
                        var strid = 'WeatherMenu'
                        var prtContent = document.getElementById(strid);

                        window.print();

                    }
                    var displayedGrid;
                    if ($("#gvLocation").hasClass('show')) {
                        displayedGrid = "#LocList";
                    }
                    else if ($("#gvStations").hasClass("show")) {
                        displayedGrid = "#stnList";

                    }
                    if (ariaControls == "#mapImage") {
                        if (displayedGrid == "#LocList")
                            locs = $('#gvLocation tr').map(function () { return [$('td', this).map(function () { return $(this).text(); }).get()]; }).get();
                        else
                            locs = $('#gvStations tr').map(function () { return [$('td', this).map(function () { return $(this).text(); }).get()]; }).get();
                        plotgoogle(displayedGrid, locs);
                        //Add for IM01894118:Agricast V2 - forcing a resizing of the page when updated - 20140626 - start
                        resizeMe();
                        //Add for IM01894118:Agricast V2 - forcing a resizing of the page when updated - 20140626 - end
                        return false;              
                    }
                    //Add for IM01894118:Agricast V2 - forcing a resizing of the page when updated - 20140626 - start
                    if (ariaControls == "#List") {
                        resizeMe();
                    }
                    //Add for IM01894118:Agricast V2 - forcing a resizing of the page when updated - 20140626 - end
                    return false;
                }
            }
        }
    });



    $('div .close').click(function () {
        var ariaControls = '#' + $(this).attr('data-controls');
        $(ariaControls)
          .removeClass('show')
            .addClass('hide');

        //Set the hidden field value of grid Favorites name="ctl00$MainContent$ctl01$hdnGridNearByStations_Status" 
        if (ariaControls == '#favorites' || ariaControls == '#feedback' || ariaControls == '#email') {
            var objFav = document.getElementById("hdnGridImages_Status");
            if (objFav != null) {
                objFav.value = 'hide' + ariaControls;
            }
            //Add for IM01894118:Agricast V2 - forcing a resizing of the page when updated - 20140620 - start
            resizeMe();
            //Add for IM01894118:Agricast V2 - forcing a resizing of the page when updated - 20140620 - end
        }


        if (ariaControls == '#divLocation') {
            var objGrid = document.getElementById("hdnGridStatus");
            if (objGrid != null) {
                $("#hdnGridStatus").val('hide');
            }
            //Add for IM01894118:Agricast V2 - forcing a resizing of the page when updated - 20140620 - start
            resizeMe();
            //Add for IM01894118:Agricast V2 - forcing a resizing of the page when updated - 20140620 - end
        }

        //Set the Hidden Field Value :- Favorite Grid Status

        if (ariaControls == '#favorites') {
            var objGrid = document.getElementById("hdnGridFavoriteStatus");

            if (objGrid != null) {

                $("#hdnGridFavoriteStatus").val('HideFavorite');

            }


        }

    });


    jQuery("ul#tabs li").bind('click', function () {
        var img = $(this).children('img')[0].id;
        $('#' + img).click();
    });


    $('#btnSearch').click(function (event) {
        $('#searchbox').addClass('loading');

    });

    calendar();

    $('#hApply').click(function () {
        var cbChecked = "";
        $('input:checkbox').each(function () {
            var sThisVal = (this.checked ? this.id : "");
            if (sThisVal != "")
                if (cbChecked != "")
                    cbChecked = cbChecked + ';' + sThisVal;
                else
                    cbChecked = sThisVal;

        });


        $('#hdnSeries').attr("value", cbChecked);



    });

    //On check of any checkbox add the name to hidden field
    $('input:checkbox').click(function () {
        var cbChecked = "";
        $('input:checkbox').each(function () {
            var sThisVal = (this.checked ? this.id : "");
            if (sThisVal != "")
                if (cbChecked != "")
                    cbChecked = cbChecked + ';' + sThisVal;
                else
                    cbChecked = sThisVal;

        });
        $('#hdnSeries').attr("value", cbChecked);

        //On click of any gdd checkbox, uncheck all non-gdd checkboxes
        /*if (this.id.startsWith("gdd") && this.checked == true) {
        if ($("#hdnSeries").val() != "") {
        var series = $("#hdnSeries").val().split(';');
        for (coord in series) {
        if (series[coord].startsWith("gdd") != true) {
        $('#' + series[coord])[0].checked = false;
        }
        }
        }
        }

        //On click of any non-gdd checkbox, uncheck all gdd checkboxes
        if (this.id.startsWith("node") && this.checked == true) {
        if ($("#hdnSeries").val() != "") {
        var series = $("#hdnSeries").val().split(';');
        for (coord in series) {
        if (series[coord].startsWith("node") != true) {
        $('#' + series[coord])[0].checked = false;
        }
        }
        }
        }*/

    });



    $('#ddlDuration').change(function () {
        if ($('#ddlDuration').val() == "to_date")
            $('#divEnd').show();
        else $('#divEnd').hide();
    });

    $("img[id^='img1_']").css("margin-left", "10px");

    //Cancel function of AgriInfo
    $('#hCancel').click(function () {

        $('form').clearForm();
        return false;
    });

    $.fn.clearForm = function () {
        return this.each(function () {
            var type = this.type, tag = this.tagName.toLowerCase();
            if (tag == 'form')
                return $(':input', this).clearForm();
            if (type == 'text' || type == 'password' || tag == 'textarea')
                this.value = '';
            else if (type == 'checkbox' || type == 'radio')
                this.checked = false;
            else if (tag == 'select') {
                if (($(this).attr('id') != 'ddlCountry') & ($(this).attr('id') != 'ddlCulture') & ($(this).attr('id') != 'ddlUnits') & ($(this).attr('id') != 'ddlWind'))
                    this.selectedIndex = 0;

            }
            $('#imgChart').hide();
        });

    };
    var windowSizeArray = ["width=100,height=100", "width=100,height=100,scrollbars=yes"];

    $('a').click(function (event) {
        ////debugger;
        var attrid = $(this).attr('id');
        // For some browsers, 'attr' is undefined; for others, 'attr' is false.  Check for both. 
        if (typeof attrid !== 'undefined' && attrid !== false) {
            //Check whether the link is from agriinfo
            if ($(this).attr("id").startsWith('node') || $(this).attr("id").startsWith('gdd')) {
                //get checkbox name
                var check = $(this).attr("id").replace("_lb_", "_cb_");
                //checkbox should be checked to be proceed
                if ($('#' + check)[0].checked == true) {
                    //if checkbox is checked, proceed to open pop up by passing query params
                    var serie = $(this).attr("id").substring($(this).attr("id").indexOf("lb_") + 3);
                    var node = $(this).attr("id").substring(0, 5);
                    var url = "Popup.aspx?series=" + node + "&serie=" + serie;
                    $("#iframePop").attr("src", url);
                    $("#divPopup").removeClass('hide');

                    var options = {
                        title: $(this).attr('data-TransAdvOptionText').toString(),
                        width: '400',
                        height: '300',
                        modal: true,
                        resizable: false,
                        draggable: false,
                        top: 20
                    };


                    $("#divPopup").dialog(options);
                    $("#divPopup").dialog('open');
                    //                     event.preventDefault();
                    return false;
                }
                else {
                    var attr = $(this).attr('data-TransErrorText');
                    // For some browsers, 'attr' is undefined; for others, 'attr' is false.  Check for both. 
                    if (typeof attr !== 'undefined' && attr !== false) {

                        // alert("The advanced options can be set only for selected controls.");
                        alert(attr);
                        //                         event.preventDefault();
                        return false;
                    }
                }
            }
        }
    });



    //**********************Code for 5 star ratings*****************************


    $('div#rating').rateit({
        starwidth: 16,
        starheight: 16,
        min: 0,
        max: 5,
        step: 1,
        resetable: false
    });

    $("#rating").bind('rated', function (event, value) {

        $("#hdnFeedbackRating").val(value);
        $('.rateit-selected').height('16px');
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
        var x = $(this).attr('id') + '#' + value;
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
            type: "POST",
            url: "AgriCastRating.ashx",
            data: jQuery.parseJSON('{"rate":"' + x + '"}'),
            dataType: "json",
            success: function (msg) {

            }


        });
    });






    var tooltipvalues = ['bad', 'poor', 'ok', 'good', 'excellent'];
    $("#rating").bind('hover', function (event, value) {
        $('.rateit-hover').height('18px')
        $(this).attr('title', tooltipvalues[value - 1]);
    });



    $(function () {
        $("#gvLocation").tablesorter({ sortList: [[0, 0]] });

    });
    $(function () {
        $("#gvStations").tablesorter({ sortList: [[5, 0]] });

    });
    $(function () {
        $("#gvFavorites").tablesorter({ sortList: [[0, 0]], headers: { 3: { sorter: false }, 4: { sorter: false}} });
    });

};

//Code for Calendar control for the date fields.

function calendar() {

    var spnClickCalenderToolTip = $("#spnClickCalenderToolTip").html();
    var options = {
        //        yearRange: "-11:+0",
        showOn: "button",
        buttonImage: "../../images/icon_calendar.gif",
        changeYear: true,
        buttonImageOnly: true,
        buttonText: spnClickCalenderToolTip,
        hideIfNoPrevNext: true //,


    }

    var options1 = {
        yearRange: "1991:2023",
        showOn: "button",
        buttonImage: "../../images/icon_calendar.gif",
        changeYear: true,
        buttonImageOnly: true,
        buttonText: spnClickCalenderToolTip,
        hideIfNoPrevNext: true //,


    }


    // Current document language is at HTML root tag    
    var lang = $('html').attr("lang").toLowerCase();
    // Set datepicker language.    
    $.datepicker.setDefaults($.datepicker.regional[lang === 'en-us' ? '' : lang]);
    $("#txtPlantingDate").datepicker(options);
    $("#txtstartDate").datepicker(options1);
    $("#txtEndDate").datepicker(options1);

};
