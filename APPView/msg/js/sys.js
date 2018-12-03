//接口.51.获取系统消息列表
function getSysMsg(db, uid, time, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Message/SystemMessage",
        type : "POST",
        data : {
            time : time,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")
            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    sysMsgSrchOne(db, uid, 0, data.list.length, data, [], fromPage);

                } else {
                    debug(data.msg);
                    cnctAgain();
                }

            } else {

                debug('getSysMsg出错');
                cnctAgain();

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
            cnctAgain();
        },
    });

}

//一个小时访问一次的逻辑
function oneHourReqSysMsg() {
    
    var uid = localStorage.getItem("uid");
    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {
        judgSysTime(db, localStorage.getItem("uid"), 'sys_msg');
    }
    
}

//接口.51.判断系统时间的逻辑
function judgSysTime(db, uid, fromPage) {

    var time = '';

    var sql_sel = "select  * from tbl_sys_msg_time where uid = '" + uid + "'";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            if (data.length != 0) {
                time = data[0].time;
            }

            debug(time);

            getSysMsg(db, uid, time, fromPage);

        } else {

            uexDataBaseMgr.close(db);
            debug('judgSysTime出错');
            cnctAgain();

        }

    });

}

//接口.51.0.单个查询的方法
function sysMsgSrchOne(db, uid, i, length, json, sql_tx, fromPage) {

    if (i < length) {

        var sql_sel = "select  * from tbl_sys_msg where uid = '" + uid + "' and mid = '" + json.list[i].MID + "'";
        uexDataBaseMgr.select(db, sql_sel, function(error, data) {
            if (!error) {

                var sql = "";
                if (data.length == 0) {
                    sql = "insert into tbl_sys_msg (uid, mid, type, fuid, content, create_time, status) values ('" + uid + "', '" + json.list[i].MID + "', " + json.list[i].MType + ", '" + json.list[i].FUID + "','" + json.list[i].Content + "', '" + timeStampFormat(json.list[i].CreateTime) + "', 2)";
                } else {
                    sql = "update tbl_sys_msg set type=" + json.list[i].MType + ", fuid='" + json.list[i].FUID + "', content='" + json.list[i].Content + "', create_time='" + timeStampFormat(json.list[i].CreateTime) + "' where uid = '" + uid + "' and mid = '" + json.list[i].MID + "'";
                }
                sql_tx.push(sql);

                sysMsgSrchOne(db, uid, i + 1, length, json, sql_tx, fromPage);

            } else {

                uexDataBaseMgr.close(db);
                debug('sysMsgSrchOne失败');
                cnctAgain();

            }

        });

    } else {

        var sql_sel = "select  * from tbl_sys_msg_time where uid = '" + uid + "'";
        uexDataBaseMgr.select(db, sql_sel, function(error, data) {
            if (!error) {

                var sql = "";
                if (data.length == 0) {
                    sql = "insert into tbl_sys_msg_time (uid, time) values ('" + uid + "', '" + timeStampFormat(json.time) + "')";
                } else {
                    sql = "update tbl_sys_msg_time set time='" + timeStampFormat(json.time) + "' where uid = '" + uid + "'";
                }
                sql_tx.push(sql);

                sysMsgTxActn(db, uid, json, sql_tx, fromPage);

            } else {

                uexDataBaseMgr.close(db);
                debug('srchSysMsgTime失败');
                cnctAgain();

            }

        });

    }

}

//接口.51.2.事务操作的方法
function sysMsgTxActn(db, uid, json, sql_tx, fromPage) {

    debug(sql_tx);
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

        if (error == 0) {

            //friendDispList(db, uid, json);
            if (fromPage == 'login') {

                //uexDataBaseMgr.close(db);
                //getPacketList(fromPage);
                //undoMessageCount(fromPage);
                friendSrchLocal(db, localStorage.getItem("uid"), 'login');

            } else if (fromPage == 'sys_msg') {

                uexDataBaseMgr.close(db);
                setTimeout("oneHourReqSysMsg()", 3600000);

            }

        } else {

            uexDataBaseMgr.close(db);
            debug('sysMsgTx失败');
            cnctAgain();

        }

    });

}

//接口.51.2.查询本地数据库的系统消息
function sysMsgSrchBegin(uid, fromPage) {

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {
        sysMsgSrchLocal(db, uid, fromPage);
    }

}

//接口.51.3.查找sqlite的系统消息
function sysMsgSrchLocal(db, uid, fromPage) {

    var sql_sel = "select  * from tbl_sys_msg where uid = '" + uid + "' order by id desc";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            debug(data);
            setSysMsg(db, uid, data, fromPage);

        } else {

            uexDataBaseMgr.close(db);

        }

    });

}

//接口.51.1.处理搜索到的好友数据
function setSysMsg(db, uid, json, fromPage) {

    uexDataBaseMgr.close(db);

    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var curJson = timeStampSplit(curTime);

    var fileUrl = localStorage.getItem("fileUrl");

    var listStr = '';
    var lastStr = '';

    if (json.length == 0) {

        listStr += '<div class="end-nope-number">';
        listStr += '当前没有数据';
        listStr += '</div>';

        openToast('当前没有数据', 3000, 5, 0);

    } else {

        $(json).each(function(j, s) {

            var timeStr = '';

            var timeArea = getDateDiff(s.create_time, curTime, 'day');
            var timeJson = timeStampSplit(s.create_time);
            if (timeArea == 0 || timeArea == '0') {
                if (curJson.day - timeJson.day == 1) {
                    timeStr += '昨天';
                    timeStr += ' ';
                }
            } else if (timeArea == 1 || timeArea == '1') {
                if (curJson.day - timeJson.day == 2) {
                    timeStr += timeJson.month;
                    timeStr += '月';
                    timeStr += timeJson.day;
                    timeStr += '日';
                    timeStr += ' ';
                } else if (curJson.day - timeJson.day == 1) {
                    timeStr += '昨天';
                    timeStr += ' ';
                }
            } else {
                timeStr += timeJson.month;
                timeStr += '月';
                timeStr += timeJson.day;
                timeStr += '日';
                timeStr += ' ';
            }

            timeStr += timeJson.quantum;
            timeStr += timeJson.hour;
            timeStr += ':';
            timeStr += timeJson.minute;

            listStr += '<div data-mid="' + s.mid + '" data-status="' + s.status + '" data-content="' + s.content + '" data-create_time="' + timeStr + '" class="ub ubb bc-border msg-div sys-sub">';

            listStr += '<ul ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub ub-f1 msg-ul">';

            listStr += '<li class="ub icon-li">';
            listStr += '<div class="ub-img msg-style sys-img"></div>';

            if (s.status == 2) {
                listStr += '<div data-pos="' + s.mid + '" class="ub ub-ac ub-pc pos-style">';
                listStr += '<div class="ub ub-ac ub-pc pos-num">';
                listStr += '1';
                listStr += '</div>';
                listStr += '</div>';
            }

            listStr += '</li>';

            listStr += '<li class="ub ub-f1 cont-li">';
            listStr += '<div class="ub ub-ver ub-f1 cont-style">';

            listStr += '<div class="msg-title">';
            listStr += '<span class="overflow">';
            listStr += s.content;
            listStr += '</span>';
            listStr += '</div>';

            listStr += '<div class="msg-desc">';
            listStr += '<span class="txt-normal overflow">';
            //listStr += s.CreateTime;

            listStr += timeStr;

            listStr += '</span>';
            listStr += '</div>';

            listStr += '</div>';

            /*
             listStr += '<div class="ub ub-ver time-style">';
             listStr += '<div class="time-top">';
             listStr += '</div>';
             listStr += '<div class="time-bot">';
             if (s.status == 2) {
             listStr += '<span data-unread="' + s.mid + '" class="unread">鏈</span>';
             }
             listStr += '</div>';
             listStr += '</div>';*/

            listStr += '</li>';

            listStr += '</ul>';

            listStr += '</div>';

            if (json.length == j + 1) {
                lastStr += '<div class="end-nope-number">';
                lastStr += '没有更多了';
                lastStr += '</div>';
            }

        });

        openToast('加载数据成功', 3000, 5, 0);

    }

    listStr += lastStr;

    $('#sys_msg').children().remove();
    $('#sys_msg').append(listStr);

    $('.sys-sub').unbind();
    $('.sys-sub').click(function() {

        var mid = $(this).data('mid');
        var status = $(this).data('status');
        var content = $(this).data('content');
        var create_time = $(this).data('create_time');

        var temp = {
            mid : mid,
            status : status,
            content : content,
            create_time : create_time,
        }

        setSysMsgRead(uid, mid, status, temp);

    });

}

//sys msg 设置已读消息标志
function setSysMsgRead(uid, mid, status, json) {

    if (status == 2) {

        var sql_tx = [];
        var sql = "update tbl_sys_msg set status = 1 where uid = '" + uid + "' and mid = '" + mid + "'";
        sql_tx.push(sql);

        var db = uexDataBaseMgr.open(uid + ".db");
        if (db != null) {

            debug(sql_tx);
            uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

                if (error == 0) {

                    uexDataBaseMgr.close(db);

                    $("div[data-mid='" + mid + "']").data('status', 1);
                    $("span[data-unread='" + mid + "']").remove();

                    $("div[data-pos='" + mid + "']").remove();

                    appcan.window.publish('refreshMsg', 1);

                    localStorage.setItem("SysMsg", JSON.stringify(json));

                    var localPack = localStorage.getItem('localPack');
                    if (localPack == '1' || localPack == 1) {

                        appcan.window.open({
                            name : 'sys_annc',
                            data : '../msg/sys_annc.html',
                            aniId : 10,
                        });

                    } else {

                        uexWindow.open({
                            name : 'sys_annc',
                            data : '../msg/sys_annc.html',
                            animID : 10,
                            flag : 1024
                        });

                    }

                } else {

                    uexDataBaseMgr.close(db);

                }

            });

        }

    } else {

        localStorage.setItem("SysMsg", JSON.stringify(json));

        var localPack = localStorage.getItem('localPack');
        if (localPack == '1' || localPack == 1) {

            appcan.window.open({
                name : 'sys_annc',
                data : '../msg/sys_annc.html',
                aniId : 10,
            });

        } else {

            uexWindow.open({
                name : 'sys_annc',
                data : '../msg/sys_annc.html',
                animID : 10,
                flag : 1024
            });

        }

    }

}
