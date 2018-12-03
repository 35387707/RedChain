/*
 * 接口.68.0.判断时间戳是否存在
 */
function judgConnTime(uid, fromPage) {

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

                getMyRecMessage(db, uid, time, fromPage);

            } else {

                uexDataBaseMgr.close(db);
                debug('judgConnTime出错');
                cnctAgain();

            }

        });

    }

}

/*
 * 接口.68.一开始获取最新的消息
 */
function getMyRecMessage(db, uid, Date, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Chat/GetMyRecMessage",
        type : "POST",
        data : {
            Date : Date,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            debug(data.list);
            debug(data.list.length);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    //judgMsgInit(db, uid, Date, data, fromPage);
                    recMsgSrchOne(db, uid, Date, 0, data.list.length, data, [], fromPage);

                } else {

                    debug(data.msg);
                    cnctAgain();

                }

            } else {

                debug('getMyRecMessage出错');
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

function recMsgSrchOne(db, uid, Date, i, length, json, sql_tx, fromPage) {

    if (i < length) {

        if (json.list[i].TType == 1 && (json.list[i].MType == 0 || json.list[i].MType == 1)) {

            var f_uid = '';
            if (json.list[i].FUID == uid) {
                f_uid = json.list[i].TUID;
            } else {
                f_uid = json.list[i].FUID;
            }

            var sql_sel = "select  * from tbl_msg where mid = '" + json.list[i].MID + "'";
            uexDataBaseMgr.select(db, sql_sel, function(error, data) {
                if (!error) {

                    //debug(sql_sel);

                    var sql = "";
                    if (data.length == 0) {
                        if (isJSON(json.list[i].Content) == false) {
                            sql = "insert into tbl_msg (mid, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, msg_time) values ('" + json.list[i].MID + "', '" + uid + "', '" + f_uid + "', '" + json.list[i].FUID + "', " + json.list[i].TType + ", 2, " + json.list[i].MType + ", '" + json.list[i].Content + "', '" + msgTimeSplLast(json.list[i].UpdateTime) + "')";
                        } else {
                            sql = "insert into tbl_msg (mid, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, msg_time) values ('" + json.list[i].MID + "', '" + uid + "', '" + f_uid + "', '" + json.list[i].FUID + "', " + json.list[i].TType + ", 2, " + json.list[i].MType + ", '" + JSON.parse(json.list[i].Content).content + "', '" + msgTimeSplLast(json.list[i].UpdateTime) + "')";
                        }

                    }
                    sql_tx.push(sql);

                    recMsgSrchOne(db, uid, Date, i + 1, length, json, sql_tx, fromPage);

                } else {

                    debug(error);

                    uexDataBaseMgr.close(db);
                    debug('judgMsgInit出错');
                    cnctAgain();

                }

            });

        } else {

            recMsgSrchOne(db, uid, Date, i + 1, length, json, sql_tx, fromPage);

        }

    } else {

        var tmpDt = json.timeStamp;
        var timeSql = '';
        if (Date == '') {
            timeSql = "insert into tbl_msg_time (uid, time) values ('" + uid + "', '" + tmpDt + "')";
        } else {
            timeSql = "update tbl_msg_time set time='" + tmpDt + "' where uid = '" + uid + "'";
        }
        sql_tx.push(timeSql);

        debug(sql_tx);

        recMsgTxLog(db, uid, json, sql_tx, fromPage);

    }

}

/*
 * 接口.68.1.事务保存的逻辑
 */
function recMsgTxLog(db, uid, json, sql_tx, fromPage) {

    debug(sql_tx);
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

        if (error == 0) {

            //friendDispList(db, uid, json);
            if (fromPage == 'login') {

                judgSysTime(db, uid, fromPage);

            }

        } else {

            uexDataBaseMgr.close(db);
            debug('recMsgTxLog出错');
            cnctAgain();

        }

    });

}