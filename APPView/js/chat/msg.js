/*
 * msg.1.展示最新的消息 gin
 */
function chatMsgSrchGroup(db, uid, getMsgJson, signJson, friend_status_id, msg_status_id, fromPage) {

    var json = [];

    var sql_sel = "select ";
    sql_sel += "msg.id, ";
    sql_sel += "msg.uid, ";
    sql_sel += "msg.friend_uid, ";
    sql_sel += "msg.said_uid, ";
    sql_sel += "friend.true_name as friend_name, friend.head_img as friend_img, friend.phone as friend_phone, ";
    sql_sel += "msg.msg_type_id, ";
    sql_sel += "msg.msg_status_id, ";
    sql_sel += "msg.msg_cont_type_id, ";
    sql_sel += "msg.msg_cont, ";
    sql_sel += "msg.msg_time ";
    sql_sel += "from tbl_msg msg ";
    sql_sel += "left join tbl_friend friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    //sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "group by msg.friend_uid order by msg.id desc ";

    uexDataBaseMgr.select(db, sql_sel, function(error, jsonList) {
        if (!error) {

            if (jsonList.length != 0) {

                $(jsonList).each(function(i, data) {

                    var isrt_json = {
                        id : data.id,
                        uid : data.uid,
                        friend_uid : data.friend_uid,
                        said_uid : data.said_uid,
                        friend_name : data.friend_name,
                        friend_img : data.friend_img,
                        friend_phone : data.friend_phone,
                        msg_type_id : data.msg_type_id,
                        msg_status_id : data.msg_status_id,
                        msg_cont_type_id : data.msg_cont_type_id,
                        msg_cont : data.msg_cont,
                        msg_time : data.msg_time,
                        unread_num : '',
                    }

                    json.push(isrt_json);

                });

            }

            chatMsgSrchNope(db, uid, getMsgJson, signJson, friend_status_id, msg_status_id, json, fromPage);

        } else {

            uexDataBaseMgr.close(db);
            debug('chatMsgSrchGroup出错');
            appImConnet(uid, 'msg');

        }
    });

}

/*
 * msg.2.查询出未读的数目
 */
function chatMsgSrchNope(db, uid, getMsgJson, signJson, friend_status_id, msg_status_id, json, fromPage) {

    var sql_sel = "select ";
    sql_sel += "count(*) as unread_num, ";
    sql_sel += "friend_uid, msg_status_id ";
    sql_sel += "from tbl_msg ";
    sql_sel += "where ";
    sql_sel += "uid = '" + uid + "' ";
    //sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg_status_id = " + msg_status_id + " ";
    sql_sel += "group by ";
    sql_sel += "friend_uid, msg_status_id ";

    uexDataBaseMgr.select(db, sql_sel, function(error, jsonList) {
        if (!error) {

            if (jsonList.length != 0) {

                $(json).each(function(i, data) {

                    $(jsonList).each(function(j, data_j) {

                        if (data.friend_uid == data_j.friend_uid) {
                            data.unread_num = data_j.unread_num;
                        }

                    });

                });

            }

            //chatMsgDispList(db, uid, json);
            //newSrchNope(db, uid, 3, json);
            chatHomeSrchTotal(db, uid, getMsgJson, signJson, friend_status_id, msg_status_id, json, fromPage);

        } else {

            uexDataBaseMgr.close(db);
            debug('chatMsgSrchNope出错');
            appImConnet(uid, 'msg');

        }
    });

}

/*
 * home.1.展示最新的消息
 */
function chatHomeSrchTotal(db, uid, getMsgJson, signJson, friend_status_id, msg_status_id, msgList, fromPage) {

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

            chatSysSrchTotal(db, uid, getMsgJson, signJson, 2, msgList, jsonList, fromPage);
            //chatMsgDispList(db, uid, status, msgList, jsonList, fromPage);

        } else {

            uexDataBaseMgr.close(db);
            debug('chatHomeSrchTotal出错');
            appImConnet(uid, 'msg');

        }

    });

}

/*
 * say.1.主页显示    2为系统未读
 */
function chatSysSrchTotal(db, uid, getMsgJson, signJson, status, msgList, msgJson, fromPage) {

    var sql_sel = "select ";
    sql_sel += "count(*) as sys_unread_total, ";
    sql_sel += "id, uid, mid, fuid, content, create_time, status ";
    sql_sel += "from tbl_sys_msg ";
    sql_sel += "where ";
    sql_sel += "uid = '" + uid + "' ";
    sql_sel += "and status = " + status + " ";
    uexDataBaseMgr.select(db, sql_sel, function(error, jsonList) {

        if (!error) {

            debug(jsonList);

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

                            chatMsgDispList(db, uid, getMsgJson, signJson, status, msgList, msgJson, data, fromPage);

                        } else {

                            debug('chatSysSrchTotal出错in');
                            appImConnet(uid, 'msg');

                        }

                    });

                } else {

                    chatMsgDispList(db, uid, getMsgJson, signJson, status, msgList, msgJson, jsonList, fromPage);

                }

            }

        } else {

            uexDataBaseMgr.close(db);
            debug('chatSysSrchTotal出错out');
            appImConnet(uid, 'msg');

        }

    });

}

/*
 * msg.3.迭代列表
 * db, uid, status, msgList, msgJson, sysJson, newJson, sayJson, fromPage
 */
function chatMsgDispList(db, uid, getMsgJson, signJson, status, msgList, msgJson, sysJson, fromPage) {

    uexDataBaseMgr.close(db);

    debug(msgList);
    debug(msgJson);
    debug(sysJson);
    /*debug(newJson);
     debug(sayJson);*/

    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var curJson = timeStampSplit(curTime);

    var fileUrl = localStorage.getItem("fileUrl");

    var msgStr = '';
    $(msgList).each(function(i, v) {

        if (v.msg_cont_type_id == 19) {
            msgStr += '<div data-msg_cont_type_id="' + v.msg_cont_type_id + '" data-msg_uid="' + v.friend_uid + '" data-friend_uid="' + v.friend_uid + '" data-friend_name="' + v.friend_name + '" data-friend_img="' + v.friend_img + '" data-friend_phone="' + v.friend_phone + '" class="ub ubb bc-border msg-div msg-sub">';
        } else {
            msgStr += '<div data-msg_uid="' + v.friend_uid + '" data-friend_uid="' + v.friend_uid + '" data-friend_name="' + v.friend_name + '" data-friend_img="' + v.friend_img + '" data-friend_phone="' + v.friend_phone + '" class="ub ubb bc-border msg-div msg-sub">';
        }

        msgStr += '<ul ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub ub-f1 msg-ul">';

        msgStr += '<li class="ub icon-li">';

        msgStr += '<div class="ub-img1 msg-style" style="background-image:url(';
        msgStr += fileUrl + v.friend_img;
        msgStr += ');"></div>';

        msgStr += '<div class="ub ub-ac ub-pc pos-style">';

        if (v.unread_num == null || v.unread_num == '' || v.unread_num == undefined) {
            msgStr += '<div data-unread_num="' + v.friend_uid + '" class="ub ub-ac ub-pc pos-num no-dis">';
            msgStr += '';
            msgStr += '</div>';
        } else {
            msgStr += '<div data-unread_num="' + v.friend_uid + '" class="ub ub-ac ub-pc pos-num ub">';
            msgStr += v.unread_num;
            msgStr += '</div>';
        }

        msgStr += '</div>';

        msgStr += '</li>';

        msgStr += '<li class="ub ub-f1 cont-li">';

        msgStr += '<div class="ub ub-ver ub-f1 cont-style">';

        msgStr += '<div class="msg-title">';
        msgStr += '<span>';
        if (v.friend_name == null || v.friend_name == '' || v.friend_name == undefined) {
            msgStr += v.friend_phone;
        } else {
            msgStr += v.friend_name;
        }
        msgStr += '</span>';
        msgStr += '</div>';

        msgStr += '<div class="msg-desc">';
        if (v.msg_cont_type_id == 0) {

            msgStr += '<span data-chg_cont="' + v.friend_uid + '" class="txt-normal overflow">';

            msgStr += v.msg_cont;

            msgStr += '</span>';

        } else if (v.msg_cont_type_id == 1) {

            msgStr += '<span data-chg_cont="' + v.friend_uid + '" class="txt-normal overflow">';
            msgStr += '[图片]';
            msgStr += '</span>';

        } else if (v.msg_cont_type_id == 19) {

            msgStr += '<span data-chg_cont="' + v.friend_uid + '" class="txt-normal overflow">';
            msgStr += '福包敲门中';
            msgStr += '</span>';

        } else {

            msgStr += '<span data-chg_cont="' + v.friend_uid + '" class="txt-normal overflow">';

            msgStr += v.msg_cont;

            msgStr += '</span>';

        }
        msgStr += '</div>';

        msgStr += '</div>';

        msgStr += '<div class="ub ub-ver time-style">';

        msgStr += '<div class="time-top">';
        msgStr += '<span>';
        var timeArea = getDateDiff(v.msg_time, curTime, 'day');
        var timeJson = timeStampSplit(v.msg_time);
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

        msgStr += '</span>';

        msgStr += '</div>';

        if (v.msg_cont_type_id == 19) {

            msgStr += '<div class="time-bot">';
            msgStr += '<span>未领取</span>';
            msgStr += '</div>';

        } else {

            msgStr += '<div class="time-bot">';
            msgStr += '<span></span>';
            msgStr += '</div>';

        }

        msgStr += '</div>';

        msgStr += '</li>';

        msgStr += '</ul>';

        msgStr += '</div>';

    });

    $('#ws_msg').children().remove();
    $('#ws_msg').append(msgStr);

    var indx = '';

    var msgCount = '';
    if (msgJson[0].msg_unread_total == 0 || msgJson[0].msg_unread_total == '0' || msgJson[0].msg_unread_total == null || msgJson[0].msg_unread_total == '' || msgJson[0].msg_unread_total == undefined) {
        msgCount = 0;
    } else {
        msgCount = parseInt(msgJson[0].msg_unread_total);
    }

    var sysCount = ''
    if (sysJson[0].sys_unread_total == 0 || sysJson[0].sys_unread_total == '0' || sysJson[0].sys_unread_total == null || sysJson[0].sys_unread_total == '' || sysJson[0].sys_unread_total == undefined) {
        sysCount = 0;
    } else {
        sysCount = parseInt(sysJson[0].sys_unread_total);
    }

    var newCount = ''
    if (localStorage.getItem("newCount") == 0 || localStorage.getItem("newCount") == '0' || localStorage.getItem("newCount") == null || localStorage.getItem("newCount") == '' || localStorage.getItem("newCount") == undefined) {
        newCount = 0;
    } else {
        newCount = parseInt(localStorage.getItem("newCount"));
    }

    var sayCount = ''
    if (localStorage.getItem("sayCount") == 0 || localStorage.getItem("sayCount") == '0' || localStorage.getItem("sayCount") == null || localStorage.getItem("sayCount") == '' || localStorage.getItem("sayCount") == undefined) {
        sayCount = 0;
    } else {
        sayCount = parseInt(localStorage.getItem("sayCount"));
    }

    var home_unread_total = msgCount + sysCount + newCount + sayCount;
    if (home_unread_total == 0 || home_unread_total == '0' || home_unread_total == null || home_unread_total == '' || home_unread_total == undefined) {
        if (fromPage == 'login') {
            appcan.window.publish('homeInit', 0);
        } else {
            appcan.window.publish('homeItem', 0);
        }
    } else {
        //indx = '消息';
        //$("a[data-tab_items='" + indx + "']").removeClass('no-dis');
        //$("a[data-tab_items='" + indx + "']").text(home_unread_total);
        if (fromPage == 'login') {
            appcan.window.publish('homeInit', home_unread_total);
        } else {
            appcan.window.publish('homeItem', home_unread_total);
        }

    }

    var sysTimeStr = '';
    var sysTimeArea = getDateDiff(sysJson[0].create_time, curTime, 'day');
    var sysTimeJson = timeStampSplit(sysJson[0].create_time);
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
    sysTimeStr += sysTimeJson.minute;

    if (sysCount == 0) {

        indx = '系统消息';

        $("div[data-sys_count='" + indx + "']").addClass('no-dis');
        $("div[data-sys_count='" + indx + "']").text('');

        $("span[data-sys_txt='" + indx + "']").text(sysJson[0].content);
        $("span[data-sys_txt='" + indx + "']").addClass('overflow');
        $("span[data-sys_red='" + indx + "']").remove();
        $("span[data-sys_time='" + indx + "']").text(sysTimeStr);

    } else {

        indx = '系统消息';

        var newLast = localStorage.getItem("newLast");

        $("div[data-sys_count='" + indx + "']").removeClass('no-dis');
        $("div[data-sys_count='" + indx + "']").text(sysCount);

        $("span[data-sys_txt='" + indx + "']").text(sysJson[0].content);
        $("span[data-sys_txt='" + indx + "']").addClass('overflow');
        $("span[data-sys_red='" + indx + "']").remove();
        $("span[data-sys_time='" + indx + "']").text(sysTimeStr);

    }

    if (newCount == 0) {

        indx = '新的朋友';

        $("div[data-sys_count='" + indx + "']").addClass('no-dis');
        $("div[data-sys_count='" + indx + "']").text('');
        $("span[data-sys_red='" + indx + "']").text('0个好友申请');

    } else {

        indx = '新的朋友';

        var newLast = localStorage.getItem("newLast");

        $("div[data-sys_count='" + indx + "']").removeClass('no-dis');
        $("div[data-sys_count='" + indx + "']").text(newCount);
        //$("span[data-sys_txt='" + indx + "']").text('您有');
        $("span[data-sys_time='" + indx + "']").text(newLast);
        $("span[data-sys_red='" + indx + "']").text(newCount + '个好友申请');

    }

    if (sayCount == 0) {

        indx = '打招呼';

        $("div[data-sys_count='" + indx + "']").addClass('no-dis');
        $("div[data-sys_count='" + indx + "']").text('');
        $("span[data-sys_title='" + indx + "']").text('收到0个打招呼');

    } else {

        indx = '打招呼';

        var sayLast = localStorage.getItem("sayLast");
        var sayPrice = localStorage.getItem("sayPrice");

        $("div[data-sys_count='" + indx + "']").removeClass('no-dis');
        $("div[data-sys_count='" + indx + "']").text(sayCount);
        $("span[data-sys_title='" + indx + "']").text('收到' + sayCount + '个打招呼');
        //$("div[data-sys_txt='" + indx + "']").text(content.Content);
        $("span[data-sys_time='" + indx + "']").text(sayPrice + '元');
        $("div[data-sys_icon='" + indx + "']").removeClass('no-dis');

    }

    $('.msg-sub').unbind();
    $('.msg-sub').click(function() {

        if ($(this).data('msg_cont_type_id')) {

            openToast("正在等待对方领取福包私聊中，请稍后", 3000, 5, 0);

        } else {

            var indx = $(this).data('friend_uid');
            var num = $("div[data-unread_num='" + indx + "']").text().replace(/\s*/g, "");
            appcan.window.publish('refreshMsgNum', num);

            $("div[data-unread_num='" + indx + "']").addClass('no-dis');
            $("div[data-unread_num='" + indx + "']").text('');

            localStorage.setItem("friend_uid", $(this).data('friend_uid'));
            localStorage.setItem("friend_name", $(this).data('friend_name'));
            localStorage.setItem("friend_img", $(this).data('friend_img'));
            localStorage.setItem("friend_phone", $(this).data('friend_phone'));

            var localPack = localStorage.getItem('localPack');
            if (localPack == '1' || localPack == 1) {

                appcan.window.open({
                    name : 'chat_list_local',
                    data : '../msg/chat_list_local.html',
                    aniId : 10,
                });

            } else {

                uexWindow.open({
                    name : "chat_list",
                    data : "../msg/chat_list.html",
                    animID : 10,
                    flag : 1024
                });

            }

        }

    });

    judgPublish(uid, getMsgJson, signJson);

}

/*
 * 判断去向
 */
function judgPublish(uid, getMsgJson, signJson) {

    //继续去获取消息
    appImConnet(uid, 'msg');

    debug('话说怎么觉得有点不可思议呢');
    debug(getMsgJson);

    localStorage.setItem("getMsgJson", JSON.stringify(getMsgJson));

    appcan.window.publish('dispChatMsg', 'friend');
    appcan.window.publish('getNewFriend', JSON.stringify(signJson));
    appcan.window.publish('getSayHello', JSON.stringify(signJson));

}
