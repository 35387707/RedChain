//接口.7.获取可取红包
//当点击首页底部导航栏的时候就要去获取可取红包列表
function getPacketList(fromPage) {

    if (fromPage == 'login') {
        //openToast('正在获取可取红包...', 30000, 5, 1);
    } else if (fromPage == 'home') {
        openToast('正在获取可领取福包...', 60000, 5, 1);
    }

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    //登录的逻辑
    appcan.ajax({
        url : ajaxUrl + "RedPacket/GetPacketList",
        type : "POST",
        data : {
        },
        timeout : 30000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    //doRacketActn(data, fromPage);
                    var db = uexDataBaseMgr.open(localStorage.getItem("uid") + ".db");
                    if (db != null) {
                        doRacketArr(db, data, fromPage);
                    }

                } else {

                    initError();
                    hideLoading(data.msg, 5000, 5, 0);

                }

            } else {

                initError();
                hideLoading('获取可取福包失败', 5000, 5, 0);

            }

        },
        error : function(xhr, errorType, error, msg) {

            var debugMsg = {
                xhr : xhr,
                errorType : errorType,
                error : error,
                msg : msg,
            }
            debug(debugMsg);
            initError();
            hideLoading('获取可取福包失败', 5000, 5, 0);

        },
    });

}

//接口.7.1.处理红包列表的逻辑
function doRacketActn(db, json, fromPage) {

    var sql_tx = [];

    var redArr = [];
    var float = '';

    if (json.list.length == 0 || json.list == '') {

        if (fromPage == '') {
            hideLoading('当前没有可获取的福包...', 3000, 5, 0);
        }

    } else {

        /*
         *  加0.000123
         */
        var middle = parseInt((json.list.length - 1) / 2);

        $(json.list).each(function(j, s) {

            float = randomFrom(1, 50) * 0.000123;

            var calc = '';
            //calc = parseFloat(calc.toFixed(6));
            if (j <= middle) {
                calc = j / randomFrom(100000, 200000) - float;
            } else {
                calc = j / randomFrom(10000, 20000) - float;
            }

            if (fromPage == 'login') {

                var id = s.Number + '_&_' + s.RID;
                var longitude = parseFloat(localStorage.getItem('longitude')) + calc;
                var latitude = parseFloat(localStorage.getItem('latitude')) + calc;
                var icon = 'res://bubble_hongbao@2x.png';
                var sql = "insert into tbl_red_packet(id, longitude, latitude, icon) values ('" + id + "', '" + longitude + "', '" + latitude + "', '" + icon + "')";
                sql_tx.push(sql);

            } else {

                var single = {
                    id : s.Number + '_&_' + s.RID,
                    longitude : parseFloat(localStorage.getItem('longitude')) + calc,
                    latitude : parseFloat(localStorage.getItem('latitude')) + calc,
                    icon : "res://bubble_hongbao@2x.png",
                }

                redArr.push(single);

            }

        });

    }

    if (fromPage == 'login') {

        var db = uexDataBaseMgr.open(localStorage.getItem("uid") + ".db");
        if (db != null) {
            redPacketTxActn(db, sql_tx, fromPage);
        }

    } else {

        randomAddMarkers(redArr, fromPage);

    }

}

/*
 * 接口.7.1.1.红包数组切割, 循环分配经纬度
 */
function doRacketArr(db, json, fromPage) {

    if (json.list.length != 0) {

        var arr = [[], [], [], [], [], [], [], [], [], [], [], []];

        $(json.list).each(function(i, v) {

            if (i + 1 <= 12) {

                arr[i].push(v);

            } else if (i + 1 <= 12 * 2 && i + 1 > 12) {

                arr[(i - 12)].push(v);

            } else if (i + 1 <= 12 * 3 && i + 1 > 12 * 2) {

                arr[(i - 12 * 2)].push(v)

            } else if (i + 1 <= 12 * 4 && i + 1 > 12 * 3) {

                arr[(i - 12 * 3)].push(v)

            } else if (i + 1 <= 12 * 5 && i + 1 > 12 * 4) {

                arr[(i - 12 * 4)].push(v)

            }

        });

        if (fromPage == 'login' || fromPage == 'reOpen') {
            doRacketSpl(db, arr, json, fromPage);
        } else if (fromPage == 'home') {
            removeRedPacketSrch(db, arr, json, fromPage);
        }

    } else {

        uexDataBaseMgr.close(db);

        if (fromPage == 'login') {

            appcan.window.publish('closLoadingView', 1);

            debug('木有数据就去消息列表页啊');

            setTimeout(function() {

                appcan.window.publish('msgStart', 1);

            }, 300);

        } else if (fromPage == 'home' || fromPage == 'reOpen') {

            openToast('当前没有可以领取的福包', 5000, 5, 0);

        }

    }

}

/*
 * 接口.7.1.2.处理分配经纬度的逻辑
 */
function doRacketSpl(db, arr, json, fromPage) {

    var sql_tx = [];

    var redArr = [];

    var longitude = '';
    var latitude = '';
    var icon = 'res://bubble_hongbao@2x.png';

    //9点钟方向
    $(arr[0]).each(function(i, v) {

        longitude = parseFloat(localStorage.getItem('longitude'));
        latitude = parseFloat(localStorage.getItem('latitude'));

        longitude = longitude - ((i + 1) * 0.001365);

        if (fromPage == 'login' || fromPage == 'home') {

            var id = v.Number + '_&_' + v.RID;
            var sql = "insert into tbl_red_packet(id, longitude, latitude, icon) values ('" + id + "', '" + longitude + "', '" + latitude + "', '" + icon + "')";
            sql_tx.push(sql);

        } else {

            var single = {
                id : v.Number + '_&_' + v.RID,
                longitude : longitude,
                latitude : latitude,
                icon : "res://bubble_hongbao@2x.png",
            }

            redArr.push(single);

        }

    });

    //10点钟方向
    $(arr[1]).each(function(i, v) {

        longitude = parseFloat(localStorage.getItem('longitude'));
        latitude = parseFloat(localStorage.getItem('latitude'));

        longitude = longitude - ((i + 1) * 0.001515);
        latitude = latitude + ((i + 1) * 0.000964);

        if (fromPage == 'login' || fromPage == 'home') {

            var id = v.Number + '_&_' + v.RID;
            var sql = "insert into tbl_red_packet(id, longitude, latitude, icon) values ('" + id + "', '" + longitude + "', '" + latitude + "', '" + icon + "')";
            sql_tx.push(sql);

        } else {

            var single = {
                id : v.Number + '_&_' + v.RID,
                longitude : longitude,
                latitude : latitude,
                icon : "res://bubble_hongbao@2x.png",
            }

            redArr.push(single);

        }

    });

    //11点钟方向
    $(arr[2]).each(function(i, v) {

        longitude = parseFloat(localStorage.getItem('longitude'));
        latitude = parseFloat(localStorage.getItem('latitude'));

        longitude = longitude - ((i + 1) * 0.000745);
        latitude = latitude + ((i + 1) * 0.001551);

        if (fromPage == 'login' || fromPage == 'home') {

            var id = v.Number + '_&_' + v.RID;
            var sql = "insert into tbl_red_packet(id, longitude, latitude, icon) values ('" + id + "', '" + longitude + "', '" + latitude + "', '" + icon + "')";
            sql_tx.push(sql);

        } else {

            var single = {
                id : v.Number + '_&_' + v.RID,
                longitude : longitude,
                latitude : latitude,
                icon : "res://bubble_hongbao@2x.png",
            }

            redArr.push(single);

        }

    });

    //12点钟方向
    $(arr[3]).each(function(i, v) {

        longitude = parseFloat(localStorage.getItem('longitude'));
        latitude = parseFloat(localStorage.getItem('latitude'));

        //longitude = longitude - ((i + 1) * 0.001120);
        latitude = latitude + ((i + 1) * 0.001378);

        if (fromPage == 'login' || fromPage == 'home') {

            var id = v.Number + '_&_' + v.RID;
            var sql = "insert into tbl_red_packet(id, longitude, latitude, icon) values ('" + id + "', '" + longitude + "', '" + latitude + "', '" + icon + "')";
            sql_tx.push(sql);

        } else {

            var single = {
                id : v.Number + '_&_' + v.RID,
                longitude : longitude,
                latitude : latitude,
                icon : "res://bubble_hongbao@2x.png",
            }

            redArr.push(single);

        }

    });

    //1点钟方向
    $(arr[4]).each(function(i, v) {

        longitude = parseFloat(localStorage.getItem('longitude'));
        latitude = parseFloat(localStorage.getItem('latitude'));

        longitude = longitude + ((i + 1) * 0.001150);
        latitude = latitude + ((i + 1) * 0.001437);

        if (fromPage == 'login' || fromPage == 'home') {

            var id = v.Number + '_&_' + v.RID;
            var sql = "insert into tbl_red_packet(id, longitude, latitude, icon) values ('" + id + "', '" + longitude + "', '" + latitude + "', '" + icon + "')";
            sql_tx.push(sql);

        } else {

            var single = {
                id : v.Number + '_&_' + v.RID,
                longitude : longitude,
                latitude : latitude,
                icon : "res://bubble_hongbao@2x.png",
            }

            redArr.push(single);

        }

    });

    //2点钟方向
    $(arr[5]).each(function(i, v) {

        longitude = parseFloat(localStorage.getItem('longitude'));
        latitude = parseFloat(localStorage.getItem('latitude'));

        longitude = longitude + ((i + 1) * 0.001489);
        latitude = latitude + ((i + 1) * 0.000914);

        if (fromPage == 'login' || fromPage == 'home') {

            var id = v.Number + '_&_' + v.RID;
            var sql = "insert into tbl_red_packet(id, longitude, latitude, icon) values ('" + id + "', '" + longitude + "', '" + latitude + "', '" + icon + "')";
            sql_tx.push(sql);

        } else {

            var single = {
                id : v.Number + '_&_' + v.RID,
                longitude : longitude,
                latitude : latitude,
                icon : "res://bubble_hongbao@2x.png",
            }

            redArr.push(single);

        }

    });

    //3点钟方向
    $(arr[6]).each(function(i, v) {

        longitude = parseFloat(localStorage.getItem('longitude'));
        latitude = parseFloat(localStorage.getItem('latitude'));

        longitude = longitude + ((i + 1) * 0.001864);
        //latitude = latitude + ((i + 1) * 0.001030);

        if (fromPage == 'login' || fromPage == 'home') {

            var id = v.Number + '_&_' + v.RID;
            var sql = "insert into tbl_red_packet(id, longitude, latitude, icon) values ('" + id + "', '" + longitude + "', '" + latitude + "', '" + icon + "')";
            sql_tx.push(sql);

        } else {

            var single = {
                id : v.Number + '_&_' + v.RID,
                longitude : longitude,
                latitude : latitude,
                icon : "res://bubble_hongbao@2x.png",
            }

            redArr.push(single);

        }

    });

    //4点钟方向
    $(arr[7]).each(function(i, v) {

        longitude = parseFloat(localStorage.getItem('longitude'));
        latitude = parseFloat(localStorage.getItem('latitude'));

        longitude = longitude + ((i + 1) * 0.001671);
        latitude = latitude - ((i + 1) * 0.000591);

        if (fromPage == 'login' || fromPage == 'home') {

            var id = v.Number + '_&_' + v.RID;
            var sql = "insert into tbl_red_packet(id, longitude, latitude, icon) values ('" + id + "', '" + longitude + "', '" + latitude + "', '" + icon + "')";
            sql_tx.push(sql);

        } else {

            var single = {
                id : v.Number + '_&_' + v.RID,
                longitude : longitude,
                latitude : latitude,
                icon : "res://bubble_hongbao@2x.png",
            }

            redArr.push(single);

        }

    });

    //5点钟方向
    $(arr[8]).each(function(i, v) {

        longitude = parseFloat(localStorage.getItem('longitude'));
        latitude = parseFloat(localStorage.getItem('latitude'));

        longitude = longitude + ((i + 1) * 0.001134);
        latitude = latitude - ((i + 1) * 0.001103);

        if (fromPage == 'login' || fromPage == 'home') {

            var id = v.Number + '_&_' + v.RID;
            var sql = "insert into tbl_red_packet(id, longitude, latitude, icon) values ('" + id + "', '" + longitude + "', '" + latitude + "', '" + icon + "')";
            sql_tx.push(sql);

        } else {

            var single = {
                id : v.Number + '_&_' + v.RID,
                longitude : longitude,
                latitude : latitude,
                icon : "res://bubble_hongbao@2x.png",
            }

            redArr.push(single);

        }

    });

    //6点钟方向
    $(arr[9]).each(function(i, v) {

        longitude = parseFloat(localStorage.getItem('longitude'));
        latitude = parseFloat(localStorage.getItem('latitude'));

        //longitude = longitude + ((i + 1) * 0.001060);
        latitude = latitude - ((i + 1) * 0.001374);

        if (fromPage == 'login' || fromPage == 'home') {

            var id = v.Number + '_&_' + v.RID;
            var sql = "insert into tbl_red_packet(id, longitude, latitude, icon) values ('" + id + "', '" + longitude + "', '" + latitude + "', '" + icon + "')";
            sql_tx.push(sql);

        } else {

            var single = {
                id : v.Number + '_&_' + v.RID,
                longitude : longitude,
                latitude : latitude,
                icon : "res://bubble_hongbao@2x.png",
            }

            redArr.push(single);

        }

    });

    //7点钟方向
    $(arr[10]).each(function(i, v) {

        longitude = parseFloat(localStorage.getItem('longitude'));
        latitude = parseFloat(localStorage.getItem('latitude'));

        longitude = longitude - ((i + 1) * 0.000801);
        latitude = latitude - ((i + 1) * 0.001263);

        if (fromPage == 'login' || fromPage == 'home') {

            var id = v.Number + '_&_' + v.RID;
            var sql = "insert into tbl_red_packet(id, longitude, latitude, icon) values ('" + id + "', '" + longitude + "', '" + latitude + "', '" + icon + "')";
            sql_tx.push(sql);

        } else {

            var single = {
                id : v.Number + '_&_' + v.RID,
                longitude : longitude,
                latitude : latitude,
                icon : "res://bubble_hongbao@2x.png",
            }

            redArr.push(single);

        }

    });

    //8点钟方向
    $(arr[11]).each(function(i, v) {

        longitude = parseFloat(localStorage.getItem('longitude'));
        latitude = parseFloat(localStorage.getItem('latitude'));

        longitude = longitude - ((i + 1) * 0.001311);
        latitude = latitude - ((i + 1) * 0.000967);

        if (fromPage == 'login' || fromPage == 'home') {

            var id = v.Number + '_&_' + v.RID;
            var sql = "insert into tbl_red_packet(id, longitude, latitude, icon) values ('" + id + "', '" + longitude + "', '" + latitude + "', '" + icon + "')";
            sql_tx.push(sql);

        } else {

            var single = {
                id : v.Number + '_&_' + v.RID,
                longitude : longitude,
                latitude : latitude,
                icon : "res://bubble_hongbao@2x.png",
            }

            redArr.push(single);

        }

    });

    if (fromPage == 'login' || fromPage == 'home' || fromPage == 'reOpen') {

        redPacketTxActn(db, sql_tx, fromPage);

    } else {

        uexDataBaseMgr.close(db);
        randomAddMarkers(redArr, fromPage);

    }

}

/*
 *接口.7.2.拿到红包数据之后，暂时保存到sqlite
 */
function redPacketTxActn(db, sql_tx, fromPage) {

    var sql = "delete from tbl_red_packet";
    uexDataBaseMgr.sql(db, sql, function(error) {
        if (!error) {

            debug(sql_tx);
            uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

                if (error == 0) {

                    srchRedPacket(db, fromPage);

                } else {

                    uexDataBaseMgr.close(db);
                    initError();
                    debug('redPacketTxActn出错');
                    hideLoading('获取可取福包失败', 5000, 5, 0);

                }

            });

        }

    });

}

/*
 * 7.3.第一次从sqlite查找红包列表
 */
function srchRedPacket(db, fromPage) {

    var sql_sel = "select id, longitude, latitude, icon from tbl_red_packet";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            debug(data);
            debug(data.length);

            uexDataBaseMgr.close(db);

            if (getDevInfo('2') == 'Apple') {
                //setMarkerOverlay(redArr);
                setIosRedInfo(data);

            } else {

                randomAddMarkers(data, fromPage);

            }

        } else {

            uexDataBaseMgr.close(db);
            initError();
            debug('srchRedPacket出错');
            hideLoading('获取可取福包失败', 5000, 5, 0);

        }

    });

}

/*
 * 7.5.查询之前的东西并把它干掉
 */
function removeRedPacketSrch(db, arr, json, fromPage) {

    var sql_sel = "select id from tbl_red_packet";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            debug(data);
            removeRedPacketArr(db, arr, json, data, fromPage)

        } else {

            uexDataBaseMgr.close(db);

        }

    });

}

/*
 * 7.6.定义规则数组
 */
function removeRedPacketArr(db, arr, json, delRed, fromPage) {

    var ids = [];

    $(delRed).each(function(i, v) {

        ids.push(v.id);

    });

    debug(ids);

    debug(JSON.stringify(ids));

    uexBaiduMap.removeMakersOverlay(JSON.stringify(ids));

    doRacketSpl(db, arr, json, fromPage);

}

/*
 * ios的红包数组
 */
function setIosRedInfo(json) {

    debug(json);

    /*

     {
     data:{},
     point:{
     lng:113.301649669//经度
     ,lat:23.1200491031//纬度
     }
     }

     */
    var redList = [];

    $(json).each(function(i, v) {

        var sgl = {
            data : {
                id : v.id
            },
            point : {
                lng : v.longitude,
                lat : v.latitude
            }
        }

        redList.push(sgl);

    });

    debug(redList);

    localStorage.setItem("redList", JSON.stringify(redList));

    appcan.window.open({
        name : 'home2',
        data : '../index/home2.html',
        aniId : 10,
    });

    //debug(app.redList);
    //app.redList = redList;

}

/*
 * remove单独的标注
 */
function findSglBubble() {

    var index = '';

    var redPacketId = localStorage.getItem("redPacketId");
    debug(redPacketId);
    var redList = JSON.parse(localStorage.getItem("redList"));
    debug(redList);
    debug(redList.length);
    $(redList).each(function(i, v) {

        if (v.data.id == redPacketId) {
            debug(v.data.id);
            index = v;
        }

    });

    debug(index);

    var lng = index.point.lng;

    debug(lng);

    return lng;
}
