/*
 * home.0.打开db
 */
function homeSrchBegin(uid, friend_status_id, msg_status_id, fromPage) {
    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {
        homeSrchTotal(db, uid, friend_status_id, msg_status_id, msgList, fromPage);
    }
}

/*
 * home.1.展示最新的消息
 */
function homeSrchTotal(db, uid, friend_status_id, msg_status_id, msgList, fromPage) {

    var sql_sel = "select ";
    sql_sel += "count(*) as msg_unread_total, ";
    sql_sel += "msg_status_id ";
    sql_sel += "from tbl_msg msg ";
    sql_sel += "where ";
    sql_sel += "uid = '" + uid + "' ";
    //sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg_status_id = " + msg_status_id;

    uexDataBaseMgr.select(db, sql_sel, function(error, jsonList) {
        if (!error) {

            sysSrchTotal(db, uid, 2, msgList, jsonList, fromPage);
            //msgDispList(db, uid, status, msgList, jsonList, fromPage);

        } else {

            debug('homeSrchTotal出错');
            cnctAgain();

        }

    });

}

/*
 * say.1.主页显示    2为系统未读
 */
function sysSrchTotal(db, uid, status, msgList, msgJson, fromPage) {

    var sql_sel = "select ";
    sql_sel += "count(*) as sys_unread_total, ";
    sql_sel += "id, uid, mid, fuid, content, create_time, status ";
    sql_sel += "from tbl_sys_msg ";
    sql_sel += "where ";
    sql_sel += "uid = '" + uid + "' ";
    sql_sel += "and status = " + status + " ";
    uexDataBaseMgr.select(db, sql_sel, function(error, jsonList) {

        if (!error) {

            //debug(jsonList);

            if (jsonList.length != 0) {

                if (jsonList[0].sys_unread_total == 0 || jsonList[0].sys_unread_total == '0') {

                    var sql_max = "select ";
                    sql_max += "max(id), ";
                    sql_max += "uid, mid, fuid, content, create_time, status ";
                    sql_max += "from tbl_sys_msg ";
                    sql_max += "where ";
                    sql_max += "uid = '" + uid + "' ";
                    uexDataBaseMgr.select(db, sql_max, function(error, data) {
                        if (!error) {

                            var temp = {
                                sys_unread_total : 0,
                            }

                            data.push(temp);

                            msgDispList(db, uid, status, msgList, msgJson, data, fromPage);

                        } else {

                            debug('sysSrchNope出错');
                            cnctAgain();

                        }

                    });

                } else {

                    msgDispList(db, uid, status, msgList, msgJson, jsonList, fromPage);

                }

            }

        } else {

            debug('sysSrchTotal出错');
            cnctAgain();

        }

    });

}

/*
 * new.1.主页显示    3为新的好友请求
 */
function newSrchTotal(db, uid, status, msgList, msgJson, sysJson, fromPage) {

    var sql_sel = "select ";
    sql_sel += "count(*) as new_unread_total, ";
    sql_sel += "name, headimg, content, status, last_date ";
    sql_sel += "from tbl_new_friend ";
    sql_sel += "where ";
    sql_sel += "uid = '" + uid + "' ";
    sql_sel += "and status = " + status + " ";
    uexDataBaseMgr.select(db, sql_sel, function(error, jsonList) {
        if (!error) {

            saySrchTotal(db, uid, status, msgList, msgJson, sysJson, jsonList, fromPage);

        } else {

            debug('获取新的朋友出错');

        }

    });

}

/*
 * say.1.主页   5为打招呼的好友状态
 */
function saySrchTotal(db, uid, status, msgList, msgJson, sysJson, newJson, fromPage) {

    var sql_sel = "select ";
    sql_sel += "count(*) as say_unread_total, ";
    sql_sel += "name, headimg, content, status, last_date ";
    sql_sel += "from tbl_say_hello ";
    sql_sel += "where ";
    sql_sel += "uid = '" + uid + "' ";
    sql_sel += "and status = " + status + " ";

    uexDataBaseMgr.select(db, sql_sel, function(error, jsonList) {
        if (!error) {

            //homeDispTotal(db, uid, msgList, msgJson, sysJson, newJson, jsonList);
            msgDispList(db, uid, status, msgList, msgJson, sysJson, newJson, jsonList, fromPage);

        } else {

            debug('获取打招呼出错');

        }

    });

}

/*
 * home.2.展示查询出来的未读总数 申请好友未读  打招呼未读
 */
function homeDispTotal(db, uid, msgList, msgJson, sysJson, newJson, sayJson, fromPage) {

    var indx = '';

    var msg_unread_total = '';
    if (msgJson[0].msg_unread_total == null || msgJson[0].msg_unread_total == '' || msgJson[0].msg_unread_total == undefined) {
        msg_unread_total = 0;
    } else {
        msg_unread_total = parseInt(msgJson[0].msg_unread_total);
    }

    var sys_unread_total = ''
    if (sysJson[0].sys_unread_total == null || sysJson[0].sys_unread_total == '' || sysJson[0].sys_unread_total == undefined) {
        sys_unread_total = 0;
    } else {
        sys_unread_total = parseInt(sysJson[0].sys_unread_total);
    }

    var new_unread_total = ''
    if (newJson[0].new_unread_total == null || newJson[0].new_unread_total == '' || newJson[0].new_unread_total == undefined) {
        new_unread_total = 0;
    } else {
        new_unread_total = parseInt(newJson[0].new_unread_total);
    }

    var say_unread_total = ''
    if (sayJson[0].say_unread_total == null || sayJson[0].say_unread_total == '' || sayJson[0].say_unread_total == undefined) {
        say_unread_total = 0;
    } else {
        say_unread_total = parseInt(sayJson[0].say_unread_total);
    }

    var home_unread_total = msg_unread_total + sys_unread_total + new_unread_total + say_unread_total;
    if (home_unread_total == 0 || home_unread_total == '0' || home_unread_total == null || home_unread_total == '' || home_unread_total == undefined) {

    } else {
        indx = '消息';
        $("a[data-tab_items='" + indx + "']").removeClass('no-dis');
        $("a[data-tab_items='" + indx + "']").text(home_unread_total);

    }

    if (sys_unread_total == 0 || sys_unread_total == '0' || sys_unread_total == null || sys_unread_total == undefined || sys_unread_total == NaN) {

    } else {

    }

    if (new_unread_total == 0 || new_unread_total == '0' || new_unread_total == null || new_unread_total == undefined || new_unread_total == NaN) {

    } else {

        indx = '新的朋友';

        $("div[data-sys_count='" + indx + "']").removeClass('no-dis');
        $("div[data-sys_count='" + indx + "']").text(new_unread_total);
        //$("span[data-sys_txt='" + indx + "']").text('您有');
        $("span[data-sys_red='" + indx + "']").text(new_unread_total + '个好友申请');

    }

    if (say_unread_total == 0 || say_unread_total == '0' || say_unread_total == null || say_unread_total == undefined || say_unread_total == NaN) {

    } else {

        indx = '打招呼';

        var content = JSON.parse(sayJson[0].msg_cont);

        $("div[data-sys_count='" + indx + "']").removeClass('no-dis');
        $("div[data-sys_count='" + indx + "']").text(say_unread_total);
        $("div[data-sys_title='" + indx + "']").text('收到' + say_unread_total + '个打招呼');
        $("span[data-sys_txt='" + indx + "']").text(content.Content);
        $("span[data-sys_time='" + indx + "']").text(content.price);
        $("span[data-sys_icon='" + indx + "']").removeClass('no-dis');

    }

    uexDataBaseMgr.close(db);

}

/*
 * 接口.48.获取未处理的好友申请及打招呼数量
 */
function undoMessageCount(uid, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Message/UndoMessageCount",
        type : "POST",
        data : {
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    judgMessageCount(uid, data, fromPage);

                } else {

                    debug(data.msg);
                    cnctAgain();

                }

            } else {

                debug('undoMessageCount出错 ');
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

//接口.48.1.解析获取未处理的好友申请及打招呼数量的列表数据
function judgMessageCount(uid, json, fromPage) {

    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var curJson = timeStampSplit(curTime);

    //localStorage.setItem("password", password);
    /*
     debug(json.req.lt);
     debug(json.req.content);

     debug(json.say.content);*/

    var sayJson = '';
    if (isJSON(json.say.content) == true) {
        sayJson = JSON.parse(json.say.content);
    }
    var newTimeStr = '';
    if (json.req.lt == null || json.req.lt == '' || json.req.lt == undefined || json.req.lt == NaN) {

    } else {
        var newTimeArea = getDateDiff(timeStampFormat(json.req.lt), curTime, 'day');
        var newTimeJson = timeStampSplit(timeStampFormat(json.req.lt));
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

    }

    var sayTimeStr = '';
    if (json.say.lt == '' || json.say.lt == null || json.say.lt == undefined || json.say.lt == NaN) {

    } else {

        var sayTimeArea = getDateDiff(timeStampFormat(json.say.lt), curTime, 'day');
        var sayTimeJson = timeStampSplit(timeStampFormat(json.say.lt));
        switch (sayTimeArea) {
        case 0:
            if (curJson.day - sayTimeJson.day == 1) {
                sayTimeStr += '昨天';
                sayTimeStr += ' ';
            }
            break;
        case 1:
            if (curJson.day - sayTimeJson.day == 2) {
                sayTimeStr += sayTimeJson.month;
                sayTimeStr += '月';
                sayTimeStr += sayTimeJson.day;
                sayTimeStr += '日';
                sayTimeStr += ' ';
            } else if (curJson.day - sayTimeJson.day == 1) {
                sayTimeStr += '昨天';
                sayTimeStr += ' ';
            }
            break;
        default:
            sayTimeStr += sayTimeJson.month;
            sayTimeStr += '月';
            sayTimeStr += sayTimeJson.day;
            sayTimeStr += '日';
            sayTimeStr += ' ';
            break;
        }

        sayTimeStr += sayTimeJson.quantum;
        sayTimeStr += sayTimeJson.hour;
        sayTimeStr += ':';
        sayTimeStr += sayTimeJson.minute;
    }

    /*
     var sysTimeStr = '';
     var sysTimeArea = getDateDiff(newJson[0].last_date, curTime, 'day');
     var sysTimeJson = timeStampSplit(newJson[0].last_date);
     switch (sysTimeArea) {
     case 0:
     if (curJson.day - sysTimeJson.day == 1) {
     sysTimeStr += '昨天';
     sysTimeStr += ' ';
     }
     break;
     case 1:
     if (curJson.day - sysTimeJson.day == 2) {
     sysTimeStr += sysTimeJson.month;
     sysTimeStr += '月';
     sysTimeStr += sysTimeJson.day;
     sysTimeStr += '日';
     sysTimeStr += ' ';
     } else if (curJson.day - sysTimeJson.day == 1) {
     sysTimeStr += '昨天';
     sysTimeStr += ' ';
     }
     break;
     default:
     sysTimeStr += sysTimeJson.month;
     sysTimeStr += '月';
     sysTimeStr += sysTimeJson.day;
     sysTimeStr += '日';
     sysTimeStr += ' ';
     break;
     }

     sysTimeStr += sysTimeJson.quantum;
     sysTimeStr += sysTimeJson.hour;
     sysTimeStr += ':';
     sysTimeStr += sysTimeJson.minute; */

    var newCount = json.req.count;
    var newLast = newTimeStr;

    var sayCount = json.say.count;
    var sayLast = sayTimeStr;
    var sayPrice = '';

    localStorage.setItem("newCount", newCount);

    localStorage.setItem("newLast", newLast);

    if (sayCount == 0) {
        sayPrice = 0;
    } else {
        sayPrice = sayJson.price;

    }

    localStorage.setItem("sayCount", sayCount);

    localStorage.setItem("sayLast", sayLast);

    localStorage.setItem("sayPrice", sayPrice);

    debug({
        newCount : newCount,
        newLast : newLast,
        sayCount : sayCount,
        sayLast : sayLast,
        sayPrice : sayPrice,
    });

    //judgSysTime(uid, fromPage);

    //getPacketList(fromPage);

    //friendSrchBegin(localStorage.getItem("uid"), 'login');
    judgConnTime(uid, fromPage);

}
