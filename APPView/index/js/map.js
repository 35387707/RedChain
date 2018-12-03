//var titHeight = $('#header').offset().height;
appcan.ready(function() {

    uexWindow.setMultilPopoverFlippingEnbaled(1);

    /*
     //1.一开始获取当前的位置
     var longitude = localStorage.getItem('longitude');
     //当前纬度
     var latitude = localStorage.getItem('latitude');
     //当前经度
     */

    openMap(localStorage.getItem('longitude'), localStorage.getItem('latitude'), 'login');

    //setCircle(longitude, latitude);

    //addMarkers(longitude, latitude, redPacketId);

    // 地理编码的回调方法
    uexBaiduMap.cbGeoCodeResult = function(data) {
        searchAddressObj = eval('(' + data + ')');
        //alert(searchAddressObj.longitude);
        //alert(searchAddressObj.latitude);

        setCenter(searchAddressObj.longitude, searchAddressObj.latitude);
        addMark(searchAddressObj.longitude, searchAddressObj.latitude);

    }
    //获取当前位置的回调方法
    uexBaiduMap.cbCurrentLocation = function(data) {
        userAddressObj = eval('(' + data + ')');
        //alert(userAddressObj);
        var longitude = userAddressObj.longitude;
        var latitude = userAddressObj.latitude;
        //alert(longitude);
        //alert(latitude);

        setCenter(longitude, latitude);

        uexBaiduMap.setUserTrackingMode(0);

    }

    uexBaiduMap.onMarkerClickListener = function(data) {

        //alert('onMarkerClickListener---> ' + data);

        debug('onMarkerClickListener---> ' + data);
        if (data != 'position_mine') {

            localStorage.setItem("redPacketId", data);

            openToast("正在打开红包...", 500, 5, 1);
            setTimeout(function() {

                var localPack = localStorage.getItem('localPack');

                if (localPack == '1' || localPack == 1) {

                    appcan.window.open({
                        name : 'hbknock',
                        data : '../hb/hbknock.html',
                        aniId : 10,
                    });

                } else {

                    uexWindow.open({
                        name : "hbknock",
                        data : "../hb/hbknock.html",
                        animID : 10,
                        flag : 1024
                    });

                }

            }, 500);

        }

    }

    appcan.window.subscribe('reOpenMap', function(msg) {

        if (msg != "" && msg != null && msg != undefined) {

            reOpenMap(localStorage.getItem('longitude'), localStorage.getItem('latitude'), msg);

        }

    });

    appcan.window.subscribe('hideMap', function(msg) {

        if (msg != "" && msg != null && msg != undefined) {

            if (msg == 0) {

                if (getDevInfo('2') == 'Apple') {
                    
                } else {
                    uexBaiduMap.showMap();
                }

            } else {

                if (getDevInfo('2') == 'Apple') {
                    localStorage.setItem("initRedPacket", 2);
                    alert('关掉地图试试');
                    uexBaiduMap.close();
                } else {
                    uexBaiduMap.hideMap();
                }

            }

        }

    });

    appcan.window.subscribe('getRedPacket', function(msg) {

        if (msg != "" && msg != null && msg != undefined) {

            //getHongBaoList();
            getPacketList('home');

        }

    });

    appcan.window.subscribe('initRedPacket', function(msg) {

        if (msg != "" && msg != null && msg != undefined) {

            getPacketList(msg);

        }

    });

    appcan.window.subscribe('srchRedPacket', function(msg) {

        if (msg != "" && msg != null && msg != undefined) {

            //getHongBaoList();
            var db = uexDataBaseMgr.open(localStorage.getItem("uid") + ".db");
            if (db != null) {

                srchRedPacket(db, 'home');

            }

        }

    });

    appcan.window.subscribe('removeRedPacket', function(msg) {

        if (msg != "" && msg != null && msg != undefined) {

            //getHongBaoList();
            removeMakers();

        }

    });

    appcan.window.subscribe('closeMap', function(msg) {

        if (msg != "" && msg != null && msg != undefined) {

            closeMap();

        }

    });

});

//打开百度地图功能
function openMap(longitude, latitude, fromPage) {

    //2.拿到当前的经纬度之后打开百度地图功能
    uexBaiduMap.open(0, 0, $('#content').width(), $('#content').height(), longitude, latitude, function() {

        //打开地图成功后, 创建打开浮动view
        var bodyWidth = parseInt(localStorage.getItem("cResolutionRatioWidth"));
        var bodyHeight = parseInt(localStorage.getItem("cResolutionRatioHeight"));

        /*
         var headerWidth = bodyWidth * 4.0 / 10;
         var headerLeft = (bodyWidth - headerWidth) / 2;
         appcan.window.openPopover({
         name : "header",
         dataType : 0,
         url : "../pop/header.html",
         top : 24,
         left : headerLeft,
         width : headerWidth,
         height : 130,
         });*/

        var rightTop = 0;
        //bodyHeight / 2
        if (bodyHeight > 1280) {
            rightTop = bodyHeight / 2;
        } else {
            rightTop = bodyHeight / 4;
        }
        var rightLeft = bodyWidth - 120;
        appcan.window.openPopover({
            name : "right",
            dataType : 0,
            url : "../pop/right.html",
            top : rightTop,
            left : rightLeft,
            width : 120,
            height : 600,
        });

        if (fromPage == 'both') {

            getPacketList('login');

        } else if (fromPage == 'reOpen') {

            getPacketList('reOpen');

        } else if (fromPage == 'login') {

            getPacketList('login');

        }

        //关闭首页遮罩层
        //appcan.window.publish('openWebSocket', 1);
        //appcan.window.publish('closLoadingView', 1);

    });

    addMarkers(longitude, latitude);

    setCenter(longitude, latitude);

    setZoomEnable();

    setZoomLevel();

    zoomControlsEnabled();

}

function reOpenMap(longitude, latitude, fromPage) {

    appcan.window.closePopover("right");
    appcan.window.closePopover("header");

    //2.拿到当前的经纬度之后打开百度地图功能
    uexBaiduMap.open(0, 0, $('#content').width(), $('#content').height(), longitude, latitude, function() {

        //打开地图成功后, 创建打开浮动view
        var bodyWidth = parseInt(localStorage.getItem("cResolutionRatioWidth"));
        var bodyHeight = parseInt(localStorage.getItem("cResolutionRatioHeight"));

        /*
         var headerWidth = bodyWidth * 4.0 / 10;
         var headerLeft = (bodyWidth - headerWidth) / 2;
         appcan.window.openPopover({
         name : "header",
         dataType : 0,
         url : "../pop/header.html",
         top : 24,
         left : headerLeft,
         width : headerWidth,
         height : 130,
         });*/

        var rightTop = 0;
        //bodyHeight / 2
        if (bodyHeight > 1280) {
            rightTop = bodyHeight / 2;
        } else {
            rightTop = bodyHeight / 4;
        }
        var rightLeft = bodyWidth - 120;
        appcan.window.openPopover({
            name : "right",
            dataType : 0,
            url : "../pop/right.html",
            top : rightTop,
            left : rightLeft,
            width : 120,
            height : 600,
        });

        addMarkers(longitude, latitude);

        setCenter(longitude, latitude);

        setZoomEnable();

        setZoomLevel();

        zoomControlsEnabled();

        if (fromPage == 'both') {

            getPacketList('login');

        } else if (fromPage == 'reOpen') {

            getPacketList('home');

        }

        //关闭首页遮罩层
        //appcan.window.publish('openWebSocket', 1);
        //appcan.window.publish('closLoadingView', 1);

    });

}

//关闭百度地图功能
function closeMap() {
    debug('关掉福包地图了哦');
    uexBaiduMap.close();
}

//定义高德地图缩放级别
function setZoomLevel() {
    uexBaiduMap.setZoomLevel(18);
}

//定义百度地图标注

function addMarkers(longitude, latitude) {
    //alert(markId);

    var params = [{
        id : 'position_mine',
        longitude : longitude,
        latitude : latitude,
        icon : "res://position_mine@2x.png"
    }];
    var data = JSON.stringify(params);
    //alert(data);
    var ids = uexBaiduMap.addMarkersOverlay(data);
    if (!ids) {
        //alert('添加失败');
    } else {
        //alert('添加成功');
    }

}

//随机显示红包的逻辑
function randomAddMarkers(redArr, fromPage) {
    //alert(markId);

    debug(redArr);

    localStorage.setItem("initRedPacket", 1);

    var data = JSON.stringify(redArr);
    //alert(data);
    var ids = uexBaiduMap.addMarkersOverlay(data);
    if (ids) {

        closeToast();

        if (fromPage == 'login' || fromPage == 'both') {
            //appcan.window.publish('openWebSocket', 1);
            //appcan.window.publish('msgInit', 1);
            appcan.window.publish('closLoadingView', 1);
            debug('去发送一点奇葩的东西啊');
            appcan.window.publish('msgStart', 1);
        }

    }

    //显示出来看下吧
    //uexBaiduMap.showBubble(markId);

}

/*
 * 随机设置标注的逻辑
 *
 */

function setMarkerOverlay(redArr) {
    
    alert('进入设置标注的逻辑了哦');
    alert(JSON.stringify(redArr));

    var makerInfo = '{"makerInfo":{"bubble":{"title":"福包来了"}}}';
    $(redArr).each(function(i, v) {

        uexBaiduMap.setMarkerOverlay(v.id, makerInfo);

    });

}

//移除标志
function removeMakers() {
    //alert(markId);
    var redPacketId = localStorage.getItem("redPacketId");
    var ids = '["' + redPacketId + '"]';
    uexBaiduMap.removeMakersOverlay(ids);

}

/*
* ctrl+t
* shift
* 等比例放大放小
* 裁剪
*/

//通过地址获得经纬度信息
function getGeocode(city, address) {

    var jsonstr = {
        city : "北京",
        address : address
    };
    var data = JSON.stringify(jsonstr);
    //alert(data);
    uexBaiduMap.geocode(data);
}

//设置地图类型 1-标准地图，2-卫星地图
var change = 0;
function setMapStyle() {
    change++;
    if (change % 2 != 0) {
        var typeOne = 2;
        uexBaiduMap.setMapType(typeOne);

    } else {
        var typeTwo = 1;
        uexBaiduMap.setMapType(typeTwo);
    }
}

//获取当前位置
function getCurrentLocation() {
    uexBaiduMap.getCurrentLocation();
}

//开启或关闭实时路况
var flag = 0;
function setTrafficEnable() {
    flag++;
    if (flag % 2 != 0) {
        var params = 1;
        uexBaiduMap.setTrafficEnabled(params);
    } else {
        var params = 0;
        uexBaiduMap.setTrafficEnabled(params);
    }
}

//开启或关闭手势缩放
function setZoomEnable() {
    uexBaiduMap.setZoomEnable(1);
}

//显示或隐藏缩放控件
function zoomControlsEnabled() {
    uexBaiduMap.zoomControlsEnabled(0);
}

//添加一个圆形覆盖物
function setCircle(longitude, latitude) {

    var indx = new Date().format("yyyyMMddhhmmssSS");

    var data = {
        fillColor : "",
        id : indx,
        longitude : longitude,
        latitude : latitude,
        lineWidth : "3",
        radius : "1250",
        strokeColor : "#FA5747"
    };
    var id = uexBaiduMap.addCircleOverlay(data);
    if (!id) {
        debug("添加失败");
    }

}

//设置百度地图中心点
function setCenter(longitude, latitude) {
    uexBaiduMap.setCenter(longitude, latitude);
}
