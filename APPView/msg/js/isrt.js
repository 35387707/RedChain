//3.0.isrtMsgBegin
function chatIsrtOne(uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, mid) {

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {
        chatJudgTime(db, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, mid);
    }

}

//3.2.判断最后一条数据的时间逻辑
function chatJudgTime(db, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, mid) {

    var fileUrl = localStorage.getItem("fileUrl");

    var user_img = localStorage.getItem("user_img");
    var friend_img = localStorage.getItem("friend_img");

    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var curJson = timeStampSplit(curTime);
    var msgStr = '';
    var timeStr = '';

    var lastMsgTime = localStorage.getItem("lastMsgTime");

    if (lastMsgTime == null || lastMsgTime == '' || lastMsgTime == undefined) {

        timeStr += '<div class="ub ub-ac ub-pc umh4-5 padbg">';
        timeStr += '<div class="ub ub-ac ub-pc tx-c fontc-8 ulev-2 upt6 uc-a1 sjbg">';

        timeStr += curJson.quantum;
        timeStr += curJson.hour;
        timeStr += ':';
        timeStr += curJson.minute;

        timeStr += '</div>';
        timeStr += '</div>';

    } else {

        var msgTime = lastMsgTime;
        var judgTime = getDateDiff(msgTime, curTime, 'minute');
        if (judgTime > 5) {

            timeStr += '<div class="ub ub-ac ub-pc umh4-5 padbg">';
            timeStr += '<div class="ub ub-ac ub-pc tx-c fontc-8 ulev-2 upt6 uc-a1 sjbg">';

            timeStr += curJson.quantum;
            timeStr += curJson.hour;
            timeStr += ':';
            timeStr += curJson.minute;

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

    chatSaveOne(db, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, curTime, mid);

}

//3.3.保存聊天记录到本地
function chatSaveOne(db, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, msg_time, mid) {

    var sqls = [];
    sqls.push("insert into tbl_msg (mid, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, msg_time) values ('" + mid + "', '" + uid + "', '" + friend_uid + "', '" + said_uid + "', " + msg_type_id + ", " + msg_status_id + ", " + msg_cont_type_id + ", '" + msg_cont + "', '" + msg_time + "')");
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sqls), function(error) {

        if (error == 0) {

            uexDataBaseMgr.close(db);

            localStorage.setItem("lastMsgTime", msg_time);

            //去到消息页面刷新记录
            if (msg_cont_type_id == 0) {
                appcan.window.publish('msgChgCont', msg_cont);
            } else if (msg_cont_type_id == 1) {
                appcan.window.publish('msgChgCont', '[图片]');
            } else {
                appcan.window.publish('msgChgCont', msg_cont);
            }

        } else {

            openToast("保存消息失败", 3000, 5, 0);
            uexDataBaseMgr.close(db);

        }

    });

}