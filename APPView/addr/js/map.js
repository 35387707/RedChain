var titHeight = $('#header').offset().height;
appcan.ready(function() {

    setSwipeCloseEnable(0);

    var bodyHeight = $('body').height();
    var footHeight = $('#footer').height();
    var contHeight = bodyHeight - titHeight - footHeight;
    $('#content').css('height', contHeight);

    var fbpos = localStorage.getItem("fbpos");
    if (fbpos == '' || fbpos == null || fbpos == undefined) {

    } else {
        $("#fbpos").text(fbpos);
    }

    var arealimit = localStorage.getItem("arealimit");
    $('.check-img').removeClass('checked');
    $("div[data-check_icon='" + arealimit + "']").addClass('checked');

    var initRedPacket = localStorage.getItem("initRedPacket");
    if (initRedPacket == 0 || initRedPacket == '0') {
        localStorage.setItem("initRedPacket", 3);
    } else if (initRedPacket == 1 || initRedPacket == '1') {
        localStorage.setItem("initRedPacket", 2);
    }

    //1.一开始获取当前的位置
    var longitude = localStorage.getItem('longitude');
    //当前纬度
    var latitude = localStorage.getItem('latitude');
    //当前经度

    uexBaiduMap.open(0, titHeight, $('#content').width(), $('#content').height(), longitude, latitude, function() {

        debug('打开百度地图成功');

    });

    /*

    setCenter(longitude, latitude);

    setZoomEnable();

    setZoomLevel();

    zoomControlsEnabled();

    */

    // 拦截返回按钮
    uexWindow.setReportKey(0, 1);
    // 拦截后的事件绑定
    uexWindow.onKeyPressed = function(keyCode) {
        if (keyCode == 0) {
            //alert('点击了返回键')
            //uexWidgetOne.exit(1);
            var place = localStorage.getItem("place");
            if (place == '1' || place == 1) {

                appcan.window.closePopover('place');
                localStorage.setItem("place", 0);

            } else {

                var homeEnter = localStorage.getItem('homeEnter');
                if (homeEnter == 1 || 'homeEnter' == '1') {
                    appcan.window.publish('homeEnter', 1);
                }
                appcan.window.close(-1);

            }

        } else {

            alert('点击了其他键');

        }

    }

    appcan.window.subscribe('areacode', function(msg) {

        if (msg != "" && msg != null && msg != undefined) {

            var areacode = JSON.parse(msg);

            //$('#Content').val('');
            var fbtxt = areacode[0].value + ' ' + areacode[1].value + ' ' + areacode[2].value;
            localStorage.setItem("fbpos", fbtxt);
            $("#fbpos").text(fbtxt);

        }

    });

    appcan.window.subscribe('closeAddrMap', function(msg) {

        if (msg != "" && msg != null && msg != undefined) {

            closeMap();

        }

    });

});

function closeMap() {
    debug('关掉位置地图了哦');
    uexBaiduMap.close();
}


$('#nav_left').click(function() {

    appcan.window.close(-1);

    var homeEnter = localStorage.getItem('homeEnter');
    if (homeEnter == 1 || 'homeEnter' == '1') {
        appcan.window.publish('homeEnter', 1);
    }

});

$('.chk-sub').click(function() {
    //check_enter check_icon
    var index = $(this).data('check_enter');
    var arealimit = $(this).data('arealimit');
    //alert(arealimit);
    if ($("div[data-check_icon='" + index + "']").hasClass('checked')) {
        $("div[data-check_icon='" + index + "']").removeClass('checked');
        localStorage.setItem("arealimit", 0);
    } else {
        $('.check-img').removeClass('checked');
        $("div[data-check_icon='" + index + "']").addClass('checked');
        localStorage.setItem("arealimit", arealimit);
    }

});

$('.sel-loc').click(function() {

    localStorage.setItem("place", 1);
    var bodyWidth = localStorage.getItem("bodyWidth");
    var bodyHeight = localStorage.getItem("bodyHeight");

    appcan.window.openPopover({
        name : 'place',
        dataType : 0,
        url : "../pop/place.html",
        top : 0,
        left : 0,
        width : bodyWidth,
        height : bodyHeight,
    });

});

$('#sure_fb').click(function() {

    var arealimit = localStorage.getItem("arealimit");
    appcan.window.publish('arealimit', arealimit);
    var homeEnter = localStorage.getItem('homeEnter');
    if (homeEnter == 1 || 'homeEnter' == '1') {
        appcan.window.publish('homeEnter', 1);
    }
    setTimeout(function() {

        appcan.window.close(-1);

    }, 300);

});
