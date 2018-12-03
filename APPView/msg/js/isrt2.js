//3.0.isrtMsgBegin
function chatIsrtOne(uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont) {

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {
        chatSrchLast(db, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont);
    }

}

//3.1.查询出最后一条的聊天记录
function chatSrchLast(db, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont) {

    var sql_sel = "select ";

    sql_sel += "max(msg.id) as id, ";
    sql_sel += "msg.said_uid, ";
    sql_sel += "msg.uid, msg.friend_uid, ";

    sql_sel += "user.username, user.head_img as user_img, ";
    sql_sel += "friend.true_name as friend_name, friend.head_img as friend_img, friend.friend_status_id, friend.friend_status_name, ";

    sql_sel += "msg.msg_type_id, msg_typ.msg_type_name, ";
    sql_sel += "msg.msg_status_id, msg_stat.msg_status_name, ";
    sql_sel += "msg.msg_cont_type_id, msg_cont_typ.msg_cont_type_name, ";

    sql_sel += "msg.msg_cont, ";

    sql_sel += "msg.msg_time ";

    sql_sel += "from tbl_msg msg ";

    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";

    debug(sql_sel);
    uexDataBaseMgr.select(db, sql_sel, function(error, jsonList) {

        if (!error) {

            chatJudgTime(db, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, jsonList);

        } else {

            debug('error');

        }

    });

}

//3.2.判断最后一条数据的时间逻辑
function chatJudgTime(db, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, json) {

    var fileUrl = localStorage.getItem("fileUrl");

    var user_img = localStorage.getItem("user_img");
    var friend_img = localStorage.getItem("friend_img");

    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var timeJson = timeStampSplit(curTime);
    var msgStr = '';
    var timeStr = '';

    if (json[0].msg_time == null || json[0].msg_time == '' || json[0].msg_time == undefined) {

        timeStr += '<div class="ub ub-ac ub-pc umh4-5 padbg">';
        timeStr += '<div class="ub ub-ac ub-pc tx-c fontc-8 ulev-2 upt6 uc-a1 sjbg">';

        timeStr += timeJson.quantum;
        timeStr += timeJson.hour;
        timeStr += ':';
        timeStr += timeJson.minute;

        timeStr += '</div>';
        timeStr += '</div>';

    } else {

        var msgTime = json[0].msg_time;
        var judgTime = getDateDiff(msgTime, curTime, 'minute');
        if (judgTime > 5) {

            timeStr += '<div class="ub ub-ac ub-pc umh4-5 padbg">';
            timeStr += '<div class="ub ub-ac ub-pc tx-c fontc-8 ulev-2 upt6 uc-a1 sjbg">';

            timeStr += timeJson.quantum;
            timeStr += timeJson.hour;
            timeStr += ':';
            timeStr += timeJson.minute;

            timeStr += '</div>';
            timeStr += '</div>';

        }

    }

    var str = trimText(msg_cont);
    var reg = /\[([^\]]+)\]/g;
    str = str.replace(reg, function($1) {
        return emojiJson[$1] || $1;
    });

    var imgArr = new Array();
    imgArr = msg_cont.split(";");

    if (said_uid == localStorage.getItem("uid")) {

        if (msg_cont_type_id == 0) {

            msgStr += '<div class="ub padd-right marg-bottom-normal">';
            msgStr += '<div class="ub ub-ac ub-f1 ub-pe">';

            msgStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="text-width ub ub-ac border-right ulev-1-1 fontc-8 right-bg fw-nor">';

            msgStr += str;

            msgStr += '</div>';

            msgStr += '<div class="ub marg-left">';

            msgStr += '<img src="';
            msgStr += fileUrl + user_img;
            msgStr += '" class="ub user-icon-width-normal">';

            msgStr += '</div>';
            msgStr += '</div>';

        } else if (msg_cont_type_id == 1) {

            $(imgArr).each(function(i, v) {

                msgStr += '<div class="ub padd-right marg-bottom-normal">';
                msgStr += '<div class="ub ub-ac ub-f1 ub-pe">';

                msgStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="text-width ub ub-ac border-right ulev-1-1 fontc-8 right-bg fw-nor">';

                msgStr += '<div class="ub ub-ac">';
                msgStr += '<img src="';
                msgStr += fileUrl + v;
                msgStr += '" class="ub proimgwg bor-k7">';
                msgStr += '</div>';

                msgStr += '</div>';

                msgStr += '<div class="ub marg-left">';

                msgStr += '<img src="';
                msgStr += fileUrl + user_img;
                msgStr += '" class="ub user-icon-width-normal">';

                msgStr += '</div>';
                msgStr += '</div>';

            });

        }

    } else {

        if (msg_cont_type_id == 0) {

            msgStr += '<div class="ub padd-left marg-bottom-normal">';
            msgStr += '<div class="ub marg-right">';

            msgStr += '<img src="';
            msgStr += fileUrl + friend_img;
            msgStr += '" class="ub user-icon-width-normal">';

            msgStr += '</div>';
            msgStr += '<div class="ub ub-ac ub-f1">';

            msgStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="text-width ub ub-ac border-left ulev-1-1 font-color2 left-bg fw-nor">';
            msgStr += str;
            msgStr += '</div>';

            msgStr += '</div>';
            msgStr += '</div>';

        } else if (msg_cont_type_id == 1) {

            $(imgArr).each(function(i, v) {

                msgStr += '<div class="ub padd-left marg-bottom-normal">';
                msgStr += '<div class="ub marg-right">';

                msgStr += '<img src="';
                msgStr += fileUrl + friend_img;
                msgStr += '" class="ub user-icon-width-normal">';

                msgStr += '</div>';
                msgStr += '<div class="ub ub-ac ub-f1">';

                msgStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="text-width ub ub-ac border-left ulev-1-1 font-color2 left-bg fw-nor">';
                msgStr += '<div class="ub ub-ac">';
                msgStr += '<img src="';
                msgStr += fileUrl + v;
                msgStr += '" class="ub proimgwg bor-k7">';
                msgStr += '</div>';

                msgStr += '</div>';

                msgStr += '</div>';
                msgStr += '</div>';

            });

        }

    }

    debug(timeStr);

    $('.chat-bottom').remove();
    $('#content').append(timeStr + msgStr);
    $('#content').append('<div class="chat-bottom"></div>');

    chatSaveOne(db, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, curTime);

}

//3.3.保存聊天记录到本地
function chatSaveOne(db, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, msg_time) {

    var sqls = [];
    sqls.push("insert into tbl_msg (uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, msg_time) values ('" + uid + "', '" + friend_uid + "', '" + said_uid + "', " + msg_type_id + ", " + msg_status_id + ", " + msg_cont_type_id + ", '" + msg_cont + "', '" + msg_time + "')");
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sqls), function(error) {

        if (error == 0) {

            openToast("保存消息成功", 3000, 5, 0);
            uexDataBaseMgr.close(db);

            //去到消息页面刷新记录
            appcan.window.publish('refreshMsg', 1);

        } else {

            openToast("保存消息失败", 3000, 5, 0);
            uexDataBaseMgr.close(db);

        }

    });

}

//3.4.循环上传图片的聊天记录
function chatImgLoop(imgArr, i, length, fromPage) {

    //img = imgArr.join(';');

}
