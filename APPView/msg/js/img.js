//3.0.isrtMsgBegin
function chatImgIsrtOne(uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, imgArr, i, length, fromPage, mid) {

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {
        //chatImgSrchLast(db, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, imgArr, i, length, fromPage, mid);
        chatImgJudgTime(db, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, imgArr, i, length, fromPage, mid);
    }

}

//3.1.查询出最后一条的聊天记录
function chatImgSrchLast(db, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, imgArr, i, length, fromPage, mid) {

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

            chatImgJudgTime(db, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, jsonList, imgArr, i, length, fromPage, mid);

        } else {

            debug('error');

        }

    });

}

//3.2.判断最后一条数据的时间逻辑
function chatImgJudgTime(db, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, imgArr, i, length, fromPage, mid) {

    var fileUrl = localStorage.getItem("fileUrl");

    var user_img = localStorage.getItem("user_img");
    var friend_img = localStorage.getItem("friend_img");

    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var timeJson = timeStampSplit(curTime);
    var msgStr = '';
    var timeStr = '';
    
    var lastMsgTime = localStorage.getItem("lastMsgTime");

    if (lastMsgTime == null || lastMsgTime == '' || lastMsgTime == undefined) {

        timeStr += '<div class="ub ub-ac ub-pc umh4-5 padbg">';
        timeStr += '<div class="ub ub-ac ub-pc tx-c fontc-8 ulev-2 upt6 uc-a1 sjbg">';

        timeStr += timeJson.quantum;
        timeStr += timeJson.hour;
        timeStr += ':';
        timeStr += timeJson.minute;

        timeStr += '</div>';
        timeStr += '</div>';

    } else {

        var msgTime = lastMsgTime;
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

    if (said_uid == localStorage.getItem("uid")) {

        msgStr += '<div class="ub padd-right marg-bottom-normal">';
        msgStr += '<div class="ub ub-ac ub-f1 ub-pe">';

        if (msg_cont_type_id == 0 || msg_cont_type_id == '0') {
            
            msgStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="text-width ub ub-ac border-right ulev-1-1 fontc-8 right-bg fw-nor">';
            msgStr += str;
            msgStr += '</div>';
            
        } else if (msg_cont_type_id == 1 || msg_cont_type_id == '1') {
            
            msgStr += '<img ontouchstart="appcan.touch(&#39;btn-act&#39;)" data-pic_p="' + fileUrl + msg_cont + '" src="';
            msgStr += fileUrl + msg_cont;
            msgStr += '" class="ub proimgwg bor-k7 disp-pic">';
            
        }
        
        msgStr += '</div>';

        msgStr += '<div class="ub marg-left">';

        msgStr += '<img ontouchstart="appcan.touch(&#39;btn-act&#39;)" src="';
        msgStr += fileUrl + user_img;
        msgStr += '" class="ub user-icon-width-normal">';

        msgStr += '</div>';
        msgStr += '</div>';

    } else {

        msgStr += '<div class="ub padd-left marg-bottom-normal">';
        msgStr += '<div class="ub marg-right">';

        msgStr += '<img ontouchstart="appcan.touch(&#39;btn-act&#39;)" src="';
        msgStr += fileUrl + friend_img;
        msgStr += '" class="ub user-icon-width-normal">';

        msgStr += '</div>';
        msgStr += '<div class="ub ub-ac ub-f1">';

        if (msg_cont_type_id == 0 || msg_cont_type_id == '0') {
            
            msgStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="text-width ub ub-ac border-left ulev-1-1 font-color2 left-bg fw-nor">';
            msgStr += str;
            msgStr += '</div>';
            
        } else if (msg_cont_type_id == 1 || msg_cont_type_id == '1') {
            
            msgStr += '<img ontouchstart="appcan.touch(&#39;btn-act&#39;)" data-pic_p="' + fileUrl + msg_cont + '" src="';
            msgStr += fileUrl + msg_cont;
            msgStr += '" class="ub proimgwg bor-k7 disp-pic">';
            
        }

        msgStr += '</div>';
        msgStr += '</div>';

    }

    debug(timeStr);

    $('.chat-bottom').remove();
    $('#content').append(timeStr + msgStr);
    $('#content').append('<div class="chat-bottom"></div>');
    
    $('.disp-pic').unbind();
    $('.disp-pic').click(function() {

        var picIndx = $(this).data('pic_p');

        //查看所有相关的图片
        var picArr = [];
        $('.disp-pic').each(function(j, s) {

            //debug($(this).data('pic_p'));
            //debug($(this).data('img_s'));

            if ($(this).data('pic_p')) {
                var picSgl = {
                    src : $(this).data('pic_p'),
                    desc : '',
                };
                picArr.push(picSgl);
            }

        });

        localStorage.setItem("imgSgl", picIndx);
        localStorage.setItem("imgArr", JSON.stringify(picArr));
        setTimeout(function() {

            openBrowser();

        }, 300);

    });

    chatImgSaveOne(db, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, curTime, imgArr, i, length, fromPage, mid);

}

//3.3.保存聊天记录到本地
function chatImgSaveOne(db, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, msg_time, imgArr, i, length, fromPage, mid) {

    var sqls = [];
    sqls.push("insert into tbl_msg (mid, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, msg_time) values ('" + mid + "', '" + uid + "', '" + friend_uid + "', '" + said_uid + "', " + msg_type_id + ", " + msg_status_id + ", " + msg_cont_type_id + ", '" + msg_cont + "', '" + msg_time + "')");
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sqls), function(error) {

        if (error == 0) {

            uexDataBaseMgr.close(db);
            
            localStorage.setItem("lastMsgTime", msg_time);
            //循环上传图片
            doImgUpload(imgArr, i + 1, length, fromPage);

        } else {

            openToast("发送图片失败", 3000, 5, 0);
            uexDataBaseMgr.close(db);

        }

    });

}