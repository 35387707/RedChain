/*
 * 替代ws的方法, 使用web微信的旧方法
 */
function appImConnet(uid, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "APPIM/Connet",
        type : "POST",
        data : {
        },
        timeout : 60000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data == 0 || data == '0') {

                appImConnet(uid, 'msg');

            } else if (data.code == -97 || data.code == '-97') {

                //showLoading(data.msg, 1);
                appcan.window.openToast(data.msg, 60000, 5, 1);
                
                localStorage.setItem("uid", '');
                localStorage.setItem("password", '');
                
                setTimeout(function() {

                    uexWidgetOne.restart();

                }, 3000);

            } else if (data == 1 || data == '1') {

                //等于1的时候就去获取最新的消息
                //openToast(data.msg, 5000, 5, 0);
                judgConnDate(uid, 'msg');

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
            appImConnet(uid, 'msg');
        },
    });

}

/*
 * 接口.68.0.判断时间戳是否存在
 */
function judgConnDate(uid, fromPage) {

    var time = '';

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {

        var sql_sel = "select  * from tbl_msg_time where uid = '" + uid + "'";
        uexDataBaseMgr.select(db, sql_sel, function(error, data) {
            if (!error) {

                if (data.length != 0) {
                    time = data[0].time;
                }

                debug('time: ' + time);

                getMyRecMsg(db, uid, time, fromPage);

            } else {

                uexDataBaseMgr.close(db);
                debug('judgConnDate出错');
                appImConnet(uid, 'msg');
            }

        });

    }

}

/*
 * 接口.68.一开始获取最新的消息
 */
function getMyRecMsg(db, uid, Date, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Chat/GetMyRecMessage",
        type : "POST",
        data : {
            Date : Date,
        },
        timeout : 1000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    judgMsgStat(db, uid, Date, data, fromPage);

                } else {

                    uexDataBaseMgr.close(db);
                    debug(data.msg);
                    appImConnet(uid, 'msg');

                }

            } else {

                uexDataBaseMgr.close(db);
                debug('获取未读消息失败');
                appImConnet(uid, 'msg');

            }

        },
        error : function(xhr, errorType, error, msg) {

            var debugMsg = {
                xhr : xhr,
                errorType : errorType,
                error : error,
                msg : msg,
            }
            uexDataBaseMgr.close(db);
            debug(debugMsg);
            appImConnet(uid, 'msg');
        },
    });

}

/*
 * 接口.68.2.判断返回的sql
 */
function judgMsgStat(db, uid, Date, json, fromPage) {

    var curWindow = localStorage.getItem("curWindow");
    var homeIndex = localStorage.getItem("homeIndex");
    var friend_uid = localStorage.getItem("friend_uid");

    var sayTmp = 0;
    var newTmp = 0;
    var sql_tx = [];

    debug(json.list.length);

    $(json.list).each(function(i, v) {

        if (v.TType == 1 && v.MType == 19) {

            if (localStorage.getItem("sayCount") == 0 || localStorage.getItem("sayCount") == '0' || localStorage.getItem("sayCount") == null || localStorage.getItem("sayCount") == '' || localStorage.getItem("sayCount") == undefined) {
                localStorage.setItem("sayCount", 1);
            } else {
                localStorage.setItem("sayCount", parseInt(localStorage.getItem("sayCount")) + 1);
            }
            sayTmp = sayTmp + 1;
            localStorage.setItem("sayLast", judgMsgTime(msgTimeSplLast(v.UpdateTime)));
            localStorage.setItem("sayPrice", JSON.parse(v.Content).price);

        } else if (v.TType == 1 && v.MType == 10) {

            debug(v.Content);

            if (isJSON(v.Content) == false) {

            } else {

                if (JSON.parse(v.Content).sign == '好友申请') {

                    if (localStorage.getItem("newCount") == 0 || localStorage.getItem("newCount") == '0' || localStorage.getItem("newCount") == null || localStorage.getItem("newCount") == '' || localStorage.getItem("newCount") == undefined) {
                        localStorage.setItem("newCount", 1);
                    } else {
                        localStorage.setItem("newCount", parseInt(localStorage.getItem("newCount")) + 1);
                    }
                    newTmp = newTmp + 1;
                    localStorage.setItem("newLast", judgMsgTime(msgTimeSplLast(v.UpdateTime)));

                }

            }

        } else if (v.TType == 3 && v.MType == 11) {

            fromPage = 'friend';
            var sql = '';
            sql = "insert into tbl_msg (mid, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, msg_time) values ('" + v.MID + "', '" + uid + "', '" + v.Content + "', '" + uid + "', " + v.TType + ", 2, " + v.MType + ", '对方通过了你的朋友验证请求，现在可以开始聊天了', '" + msgTimeSplLast(v.UpdateTime) + "')";
            sql_tx.push(sql);

        } else if (v.TType == 1 && (v.MType == 0 || v.MType == 1)) {

            var f_uid = '';
            if (v.FUID == uid) {
                f_uid = v.TUID;
            } else {
                f_uid = v.FUID;
            }

            var sql = '';
            if (curWindow == 'chat' && homeIndex == '' && v.FUID == friend_uid) {

                if (isJSON(v.Content) == false) {
                    sql = "insert into tbl_msg (mid, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, msg_time) values ('" + v.MID + "', '" + uid + "', '" + f_uid + "', '" + v.FUID + "', " + v.TType + ", 1, " + v.MType + ", '" + v.Content + "', '" + msgTimeSplLast(v.UpdateTime) + "')";
                } else {
                    sql = "insert into tbl_msg (mid, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, msg_time) values ('" + v.MID + "', '" + uid + "', '" + f_uid + "', '" + v.FUID + "', " + v.TType + ", 1, " + v.MType + ", '" + JSON.parse(v.Content).content + "', '" + msgTimeSplLast(v.UpdateTime) + "')";
                }

            } else {

                if (isJSON(v.Content) == false) {
                    sql = "insert into tbl_msg (mid, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, msg_time) values ('" + v.MID + "', '" + uid + "', '" + f_uid + "', '" + v.FUID + "', " + v.TType + ", 2, " + v.MType + ", '" + v.Content + "', '" + msgTimeSplLast(v.UpdateTime) + "')";
                } else {
                    sql = "insert into tbl_msg (mid, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, msg_time) values ('" + v.MID + "', '" + uid + "', '" + f_uid + "', '" + v.FUID + "', " + v.TType + ", 2, " + v.MType + ", '" + JSON.parse(v.Content).content + "', '" + msgTimeSplLast(v.UpdateTime) + "')";
                }

            }

            sql_tx.push(sql);

        }

    });

    var signJson = {
        sayTmp : sayTmp,
        newTmp : newTmp,
        curWindow : curWindow,
        homeIndex : homeIndex,
        friend_uid : friend_uid,
        fromPage : fromPage,
    }

    debug(signJson);

    var tmpDt = json.timeStamp;
    var timeSql = '';
    if (Date == '') {
        timeSql = "insert into tbl_msg_time (uid, time) values ('" + uid + "', '" + tmpDt + "')";
    } else {
        timeSql = "update tbl_msg_time set time='" + tmpDt + "' where uid = '" + uid + "'";
    }
    sql_tx.push(timeSql);

    recMsgTxActn(db, uid, sql_tx, json, signJson, fromPage);

}

/*
 * 接口.68.1.事务保存的逻辑
 */
function recMsgTxActn(db, uid, sql_tx, getMsgJson, signJson, fromPage) {

    debug(sql_tx);

    uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

        if (error == 0) {

            if (fromPage == 'friend') {

                chatFriendAjaxList(db, uid, getMsgJson, signJson, fromPage);

            } else {

                chatMsgSrchGroup(db, uid, getMsgJson, signJson, 1, 2, fromPage);

            }

            /*
            uexDataBaseMgr.close(db);

            if (curWindow == 'chat' && homeIndex == '') {

            localStorage.setItem("getMsgJson", JSON.stringify(getMsgJson));

            if (fromPage == 'friend') {

            localStorage.setItem("nextActn", 'friend');
            appcan.window.publish('dispChatMsg', 'friend');

            } else {

            localStorage.setItem("nextActn", 'chat');
            appcan.window.publish('dispChatMsg', 'chat');

            }

            } else {

            if (fromPage == 'friend') {

            appcan.window.publish('newFriendMsg', 'msg');

            } else {

            appcan.window.publish('refreshMsg', 'msg');

            }

            }*/

            //appcan.window.publish('getNewFriend', JSON.stringify(signJson));
            //appcan.window.publish('getSayHello', JSON.stringify(signJson));

        } else {

            uexDataBaseMgr.close(db);
            appImConnet(uid, 'msg');
            debug('recMsgTxActn出错');

        }

    });

}

function judgMsgTime(timeStr) {

    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var curJson = timeStampSplit(curTime);

    var newTimeStr = '';

    var newTimeArea = getDateDiff(timeStr, curTime, 'day');
    var newTimeJson = msgTimeStampSplit(timeStr);
    switch (newTimeArea) {
    case 0:
        if (curJson.day - newTimeJson.day == 1) {
            newTimeStr += '昨天';
            newTimeStr += ' ';
        }
        break;
    case 1:
        if (curJson.day - newTimeJson.day == 2) {
            newTimeStr += newTimeJson.month;
            newTimeStr += '月';
            newTimeStr += newTimeJson.day;
            newTimeStr += '日';
            newTimeStr += ' ';
        } else if (curJson.day - newTimeJson.day == 1) {
            newTimeStr += '昨天';
            newTimeStr += ' ';
        }
        break;
    default:
        newTimeStr += newTimeJson.month;
        newTimeStr += '月';
        newTimeStr += newTimeJson.day;
        newTimeStr += '日';
        newTimeStr += ' ';
        break;
    }

    newTimeStr += newTimeJson.quantum;
    newTimeStr += newTimeJson.hour;
    newTimeStr += ':';
    newTimeStr += newTimeJson.minute;

    return newTimeStr;

}
