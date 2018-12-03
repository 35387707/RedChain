//0.1.打开sqlite的方法
function chatSrchBegin(uid, friend_uid, friend_status_id, msg_status_id) {
    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {
        chatUpdtNope(db, uid, friend_uid, friend_status_id, msg_status_id);
    }
}

//2.6.查询之前先把未读标志清空
function chatUpdtNope(db, uid, friend_uid, friend_status_id, msg_status_id) {

    var sqls = [];
    sqls.push("update tbl_msg set msg_status_id = 1 where uid = '" + uid + "' and friend_uid = '" + friend_uid + "'");
    debug(sqls);
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sqls), function(error) {

        if (error == 0) {

            chatSrchMsg(db, uid, friend_uid, friend_status_id, msg_status_id);

        } else {

            openToast("保存消息失败", 3000, 5, 0);
            uexDataBaseMgr.close(db);

        }

    });

}

//2.7.获取聊天记录的信息内容
function chatSrchMsg(db, uid, friend_uid, friend_status_id, msg_status_id) {

    var sql_sel = "select ";

    sql_sel += "* ";

    sql_sel += "from tbl_msg ";

    sql_sel += "where ";
    sql_sel += "uid = '" + uid + "' ";
    sql_sel += "and friend_uid = '" + friend_uid + "' ";
    //sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";

    sql_sel += "order by id ";
    sql_sel += "asc ";
    
    debug(sql_sel);

    uexDataBaseMgr.select(db, sql_sel, function(error, jsonList) {
        if (!error) {

            //长度为0，代表没有该条记录 直接用这个data即可，不需要eval
            chatDispMsg(db, jsonList);

        } else {

            debug("查询msg出错!");

        }

    });

}

//3.1.迭代聊天记录
function chatDispMsg(db, json) {

    var fileUrl = localStorage.getItem("fileUrl");

    var user_img = localStorage.getItem("user_img");
    var friend_img = localStorage.getItem("friend_img");

    var reg = /\[([^\]]+)\]/g;

    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var curJson = timeStampSplit(curTime);

    if (json.length == 0 || json == '') {

        debug('当前没有查询到任何聊天记录');

    } else {

        var msgStr = '';
        var firStr = '';
        var hiStr = '';
        $(json).each(function(i, v) {

            if (v.said_uid == v.friend_uid) {

                msgStr += '<div class="ub padd-left marg-bottom-normal">';
                msgStr += '<div class="ub marg-right">';

                msgStr += '<img ontouchstart="appcan.touch(&#39;btn-act&#39;)" src="';
                msgStr += fileUrl + friend_img;
                msgStr += '" class="ub user-icon-width-normal">';

                msgStr += '</div>';
                msgStr += '<div class="ub ub-ac ub-f1">';

                if (v.msg_cont_type_id == 0) {

                    msgStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="text-width ub ub-ac border-left ulev-1-1 font-color2 left-bg fw-nor">';

                    var str = trimText(v.msg_cont);
                    str = str.replace(reg, function($1) {
                        return emojiJson[$1] || $1;
                    });

                    msgStr += str;

                    msgStr += '</div>';

                } else if (v.msg_cont_type_id == 1) {

                    msgStr += '<img ontouchstart="appcan.touch(&#39;btn-act&#39;)" data-pic_p="' + fileUrl + v.msg_cont + '" src="';
                    msgStr += fileUrl + v.msg_cont;
                    msgStr += '" class="ub proimgwg bor-k7 disp-pic">';

                } else {

                    msgStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="text-width ub ub-ac border-left ulev-1-1 font-color2 left-bg fw-nor">';

                    msgStr += v.msg_cont;

                    msgStr += '</div>';

                }

                msgStr += '</div>';
                msgStr += '</div>';

            } else if (v.said_uid == v.uid) {

                msgStr += '<div class="ub padd-right marg-bottom-normal">';
                msgStr += '<div class="ub ub-ac ub-f1 ub-pe">';

                if (v.msg_cont_type_id == 0) {

                    msgStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="text-width ub ub-ac border-right ulev-1-1 fontc-8 right-bg fw-nor">';

                    var str = trimText(v.msg_cont);
                    str = str.replace(reg, function($1) {
                        return emojiJson[$1] || $1;
                    });

                    msgStr += str;

                    msgStr += '</div>';

                } else if (v.msg_cont_type_id == 1) {

                    msgStr += '<img ontouchstart="appcan.touch(&#39;btn-act&#39;)" data-pic_p="' + fileUrl + v.msg_cont + '" src="';
                    msgStr += fileUrl + v.msg_cont;
                    msgStr += '" class="ub proimgwg bor-k7 disp-pic">';

                } else {

                    msgStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="text-width ub ub-ac border-right ulev-1-1 fontc-8 right-bg fw-nor">';

                    msgStr += v.msg_cont;

                    msgStr += '</div>';

                }

                msgStr += '</div>';
                msgStr += '<div class="ub marg-left">';

                msgStr += '<img ontouchstart="appcan.touch(&#39;btn-act&#39;)" src="';
                msgStr += fileUrl + user_img;
                msgStr += '" class="ub user-icon-width-normal">';

                msgStr += '</div>';
                msgStr += '</div>';

            }

            if (json.length == 1) {

                //当只有一条记录的时候
                var timeArea = getDateDiff(json[0].msg_time, curTime, 'day');
                var timeJson = timeStampSplit(json[0].msg_time);
                firStr += '<div class="ub ub-ac ub-pc umh4-5 padbg">';
                firStr += '<div class="ub ub-ac ub-pc tx-c fontc-8 ulev-2 upt6 uc-a1 sjbg">';

                //debug(timeJson);

                switch (timeArea) {
                case 0:
                    if (curJson.day - timeJson.day == 1) {
                        firStr += '昨天';
                        firStr += ' ';
                    }
                    break;
                case 1:
                    if (curJson.day - timeJson.day == 2) {
                        firStr += timeJson.month;
                        firStr += '月';
                        firStr += timeJson.day;
                        firStr += '日';
                        firStr += ' ';
                    } else if (curJson.day - timeJson.day == 1) {
                        firStr += '昨天';
                        firStr += ' ';
                    }
                    break;
                default:
                    firStr += timeJson.month;
                    firStr += '月';
                    firStr += timeJson.day;
                    firStr += '日';
                    firStr += ' ';
                    break;
                }

                firStr += timeJson.quantum;
                firStr += timeJson.hour;
                firStr += ':';
                firStr += timeJson.minute;

                firStr += '</div>';
                firStr += '</div>';

            } else {

                if (i < json.length - 1) {

                    var startTime = json[i].msg_time;
                    var endTime = json[i + 1].msg_time;

                    //debug(i + 'startTime: ' + startTime + ' endTime: ' + endTime);

                    var judgTime = getDateDiff(startTime, endTime, 'minute');
                    if (judgTime > 5) {

                        if (i == 0) {

                            var timeArea = getDateDiff(startTime, curTime, 'day');
                            var timeJson = timeStampSplit(startTime);
                            firStr += '<div class="ub ub-ac ub-pc umh4-5 padbg">';
                            firStr += '<div class="ub ub-ac ub-pc tx-c fontc-8 ulev-2 upt6 uc-a1 sjbg">';
                            switch (timeArea) {
                            case 0:
                                if (curJson.day - timeJson.day == 1) {
                                    firStr += '昨天';
                                    firStr += ' ';
                                }
                                break;
                            case 1:
                                if (curJson.day - timeJson.day == 2) {
                                    firStr += timeJson.month;
                                    firStr += '月';
                                    firStr += timeJson.day;
                                    firStr += '日';
                                    firStr += ' ';
                                } else if (curJson.day - timeJson.day == 1) {
                                    firStr += '昨天';
                                    firStr += ' ';
                                }
                                break;
                            default:
                                firStr += timeJson.month;
                                firStr += '月';
                                firStr += timeJson.day;
                                firStr += '日';
                                firStr += ' ';
                                break;
                            }

                            firStr += timeJson.quantum;
                            firStr += timeJson.hour;
                            firStr += ':';
                            firStr += timeJson.minute;

                            firStr += '</div>';
                            firStr += '</div>';

                        } else {

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

                    } else {

                        if (i == 0) {

                            var timeArea = getDateDiff(json[0].msg_time, curTime, 'day');
                            var timeJson = timeStampSplit(json[0].msg_time);
                            firStr += '<div class="ub ub-ac ub-pc umh4-5 padbg">';
                            firStr += '<div class="ub ub-ac ub-pc tx-c fontc-8 ulev-2 upt6 uc-a1 sjbg">';

                            //debug(timeJson);

                            switch (timeArea) {
                            case 0:
                                if (curJson.day - timeJson.day == 1) {
                                    firStr += '昨天';
                                    firStr += ' ';
                                }
                                break;
                            case 1:
                                if (curJson.day - timeJson.day == 2) {
                                    firStr += timeJson.month;
                                    firStr += '月';
                                    firStr += timeJson.day;
                                    firStr += '日';
                                    firStr += ' ';
                                } else if (curJson.day - timeJson.day == 1) {
                                    firStr += '昨天';
                                    firStr += ' ';
                                }
                                break;
                            default:
                                firStr += timeJson.month;
                                firStr += '月';
                                firStr += timeJson.day;
                                firStr += '日';
                                firStr += ' ';
                                break;
                            }

                            firStr += timeJson.quantum;
                            firStr += timeJson.hour;
                            firStr += ':';
                            firStr += timeJson.minute;

                            firStr += '</div>';
                            firStr += '</div>';

                        }

                    }

                }

            }

            if (i + 1 == json.length) {

                debug('lastMsgTime:' + v.msg_time);

                localStorage.setItem("lastMsgTime", v.msg_time);

            }

        });

        $('#content').children().remove();
        $('#content').append(firStr + msgStr);
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

    //最后关闭数据库，释放内存
    uexDataBaseMgr.close(db);

}

