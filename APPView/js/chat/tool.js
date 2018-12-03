//0.编辑发送msg的json
function chatMsgEditor(Api, TType, Rid, Gid, Type, Status, Content, fromPage) {

    var Sid = localStorage.getItem("uid");
    var Time = new Date().format("yyyy-MM-dd hh:mm:ss");

    //{"name":"15737315338","content":"是我啊"}

    var msg = {
        TType : TType,
        Rid : Rid,
        Gid : Gid,
        Type : Type,
        Content : JSON.stringify(Content),
    }

    return msg;

}

//1.返送图片的ws要特殊处理
function chatImgMsgEditor(Api, TType, Rid, Gid, Type, Status, Content, imgArr, i, length, fromPage) {

    var Sid = localStorage.getItem("uid");
    var Time = new Date().format("yyyy-MM-dd hh:mm:ss");

    var TrueName = '';
    var UserInfo = JSON.parse(localStorage.getItem("UserInfo"));
    if (UserInfo.Info.TrueName == '' || UserInfo.Info.TrueName == null || UserInfo.Info.TrueName == undefined) {
        TrueName = UserInfo.Info.Phone;
    } else {
        TrueName = UserInfo.Info.TrueName;
    }

    var temp = {
        name : TrueName,
        content : Content,
    }

    //var spec = JSON.stringify(JSON.stringify(temp));

    var msg = {
        TType : TType,
        Rid : Rid,
        Gid : Gid,
        Type : Type,
        Content : JSON.stringify(temp),
    }

    debug(msg);

    //queryMsgImg(Api, TType, Rid, Gid, Type, Status, Content, imgArr, i, length, fromPage, sendMsg)
    queryMsgImg(Api, TType, Rid, Gid, Type, Status, Content, imgArr, i, length, fromPage, msg);

}

//2.定义一些不必保存本地sqlite的ws, 比如打招呼
function chatSayHello(Api, TType, Rid, Gid, Type, Status, Content) {

    var Sid = localStorage.getItem("uid");
    var Time = new Date().format("yyyy-MM-dd hh:mm:ss");

    var msg = {
        TType : TType,
        Rid : Rid,
        Gid : Gid,
        Type : Type,
        Content : Content,
    }

    appcan.window.publish('sendMsg', JSON.stringify(msg));

}

/*
 * 查找当前用户的聊天记录
 */
function splCurUserChat(nextActn) {

    var json = JSON.parse(localStorage.getItem("getMsgJson"));

    debug(json);

    // var temp  = jsonSplit(data);
    //
    // debug(temp);
    //
    // var json = JSON.parse(temp);
    //
    // debug(json);

    var friend_uid = localStorage.getItem("friend_uid");

    var curUsrJson = [];

    $(json.list).each(function(i, v) {

        if (v.FUID == friend_uid || v.TUID == friend_uid) {

            debug(json.list[i]);
            curUsrJson.push(json.list[i]);

        }

    });

    if (curUsrJson.length == 0 || curUsrJson == '') {

    } else {
        var uid = localStorage.getItem("uid");
        var msg_status_id = 2;
        msgChatSrchLastBigin(uid, friend_uid, msg_status_id, curUsrJson, nextActn);
    }

}

function msgChatSrchLastBigin(uid, friend_uid, msg_status_id, json, nextActn) {

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {
        msgChatJudgTimeActn(db, uid, friend_uid, msg_status_id, json, nextActn);
    }

}

//3.1.判断最后一条时间
function msgChatJudgTimeActn(db, uid, friend_uid, msg_status_id, json, nextActn) {

    uexDataBaseMgr.close(db);

    debug(json);

    debug(json[0]);

    isJSON(json[0]);

    debug('json[0].UpdateTime: ' + msgTimeSplLast(json[0].UpdateTime));

    var reg = /\[([^\]]+)\]/g;

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
        var endTime = msgTimeSplLast(json[0].UpdateTime);
        var judgTime = getDateDiff(msgTime, endTime, 'minute');
        if (judgTime > 5) {

            timeStr += '<div class="ub ub-ac ub-pc umh4-5 padbg">';
            timeStr += '<div class="ub ub-ac ub-pc tx-c fontc-8 ulev-2 upt6 uc-a1 sjbg">';

            var timeArea = getDateDiff(endTime, curTime, 'day');
            var timeJson = timeStampSplit(endTime);
            switch (timeArea) {
            case 0:
                if (curJson.day - timeJson.day == 1) {
                    timeStr += '昨天';
                    timeStr += ' ';
                }
                break;
            case 1:
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
                break;
            default:
                timeStr += timeJson.month;
                timeStr += '月';
                timeStr += timeJson.day;
                timeStr += '日';
                timeStr += ' ';
                break;
            }

            timeStr += timeJson.quantum;
            timeStr += timeJson.hour;
            timeStr += ':';
            timeStr += timeJson.minute;

            timeStr += '</div>';
            timeStr += '</div>';

        }

    }

    debug(timeStr);

    $(json).each(function(i, v) {

        isJSON(v);

        if (v.FUID == friend_uid) {

            debug('friend_uid: ' + friend_uid);

            msgStr += '<div class="ub padd-left marg-bottom-normal">';
            msgStr += '<div class="ub marg-right">';

            msgStr += '<img src="';
            msgStr += fileUrl + friend_img;
            msgStr += '" class="ub user-icon-width-normal">';

            msgStr += '</div>';
            msgStr += '<div class="ub ub-ac ub-f1">';

            if (v.MType == 0) {

                msgStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="text-width ub ub-ac border-left ulev-1-1 font-color2 left-bg fw-nor">';

                var str = trimText(JSON.parse(v.Content).content);
                str = str.replace(reg, function($1) {
                    return emojiJson[$1] || $1;
                });

                msgStr += str;

                msgStr += '</div>';

            } else if (v.MType == 1) {

                msgStr += '<img data-pic_p="' + fileUrl + JSON.parse(v.Content).content + '" src="';
                msgStr += fileUrl + JSON.parse(v.Content).content;
                msgStr += '" class="ub proimgwg bor-k7 disp-pic">';

            } else {

                msgStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="text-width ub ub-ac border-left ulev-1-1 font-color2 left-bg fw-nor">';

                msgStr += JSON.parse(v.Content).content;

                msgStr += '</div>';

            }

            msgStr += '</div>';
            msgStr += '</div>';

        } else if (v.FUID == uid) {

            debug('uid: ' + uid);

            msgStr += '<div class="ub padd-right marg-bottom-normal">';
            msgStr += '<div class="ub ub-ac ub-f1 ub-pe">';

            if (v.MType == 0) {

                msgStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="text-width ub ub-ac border-right ulev-1-1 fontc-8 right-bg fw-nor">';

                var str = trimText(JSON.parse(v.Content).content);
                str = str.replace(reg, function($1) {
                    return emojiJson[$1] || $1;
                });

                msgStr += str;

                msgStr += '</div>';

            } else if (v.MType == 1) {

                msgStr += '<img data-pic_p="' + fileUrl + JSON.parse(v.Content).content + '" src="';
                msgStr += fileUrl + JSON.parse(v.Content).content;
                msgStr += '" class="ub proimgwg bor-k7 disp-pic">';

            } else {

                msgStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="text-width ub ub-ac border-right ulev-1-1 fontc-8 right-bg fw-nor">';

                msgStr += JSON.parse(v.Content).content;

                msgStr += '</div>';

            }

            msgStr += '</div>';
            msgStr += '<div class="ub marg-left">';

            msgStr += '<img src="';
            msgStr += fileUrl + user_img;
            msgStr += '" class="ub user-icon-width-normal">';

            msgStr += '</div>';
            msgStr += '</div>';

        }

        if (json.length >= 2) {

            if (i < json.length - 1) {

                var startTime = msgTimeSplLast(json[i].UpdateTime);
                var endTime = msgTimeSplLast(json[i + 1].UpdateTime);

                //debug(i + 'startTime: ' + startTime + ' endTime: ' + endTime);

                var judgTime = getDateDiff(startTime, endTime, 'minute');
                if (judgTime > 5) {

                    var timeArea = getDateDiff(endTime, curTime, 'day');
                    var timeJson = timeStampSplit(endTime);
                    msgStr += '<div class="ub ub-ac ub-pc umh4-5 padbg">';
                    msgStr += '<div class="ub ub-ac ub-pc tx-c fontc-8 ulev-2 upt6 uc-a1 sjbg">';
                    switch (timeArea) {
                    case 0:
                        if (curJson.day - timeJson.day == 1) {
                            msgStr += '昨天';
                            msgStr += ' ';
                        }
                        break;
                    case 1:
                        if (curJson.day - timeJson.day == 2) {
                            msgStr += timeJson.month;
                            msgStr += '月';
                            msgStr += timeJson.day;
                            msgStr += '日';
                            msgStr += ' ';
                        } else if (curJson.day - timeJson.day == 1) {
                            msgStr += '昨天';
                            msgStr += ' ';
                        }
                        break;
                    default:
                        msgStr += timeJson.month;
                        msgStr += '月';
                        msgStr += timeJson.day;
                        msgStr += '日';
                        msgStr += ' ';
                        break;
                    }

                    msgStr += timeJson.quantum;
                    msgStr += timeJson.hour;
                    msgStr += ':';
                    msgStr += timeJson.minute;

                    msgStr += '</div>';
                    msgStr += '</div>';

                }

            }

        }

        if (i + 1 == json.length) {

            debug('lastMsgTime:' + v.UpdateTime);

            localStorage.setItem("lastMsgTime", msgTimeSplLast(v.UpdateTime));

        }

    });

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

}
