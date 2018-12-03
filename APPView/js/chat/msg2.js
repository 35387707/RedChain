function msgChgNew(newTime) {

    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var curJson = timeStampSplit(curTime);

    var newTimeStr = '';
    if (newTime == null || newTime == '' || newTime == undefined || newTime == NaN) {

    } else {
        var newTimeArea = getDateDiff(newTime, curTime, 'day');
        var newTimeJson = timeStampSplit(newTime);
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

    var indx = '新的朋友';

    if ($("div[data-sys_count='" + indx + "']").hasClass('no-dis')) {

        $("div[data-sys_count='" + indx + "']").removeClass('no-dis');
        $("div[data-sys_count='" + indx + "']").text(1);
        //$("span[data-sys_txt='" + indx + "']").text('您有');
        $("span[data-sys_red='" + indx + "']").text('1个好友申请');
        $("span[data-sys_time='" + indx + "']").text(newTimeStr);
        localStorage.setItem("newCount", 1);

    } else {

        var temp = parseInt($("div[data-sys_count='" + indx + "']").text().replace(/\s*/g, ""));
        $("div[data-sys_count='" + indx + "']").text(temp + 1);
        //$("span[data-sys_txt='" + indx + "']").text('您有');
        $("span[data-sys_red='" + indx + "']").text((temp + 1) + '个好友申请');
        $("span[data-sys_time='" + indx + "']").text(newTimeStr);
        localStorage.setItem("newCount", (temp + 1));

    }

}

function msgChgSay(price) {

    var indx = '打招呼';

    $("div[data-sys_icon='" + indx + "']").removeClass('no-dis');

    if ($("div[data-sys_count='" + indx + "']").hasClass('no-dis')) {

        $("div[data-sys_count='" + indx + "']").removeClass('no-dis');
        $("div[data-sys_count='" + indx + "']").text(1);
        $("div[data-sys_title='" + indx + "']").text('收到1个打招呼');
        //$("div[data-sys_txt='" + indx + "']").text(content.Content);
        $("span[data-sys_time='" + indx + "']").text(price + '元');
        $("div[data-sys_icon='" + indx + "']").removeClass('no-dis');
        localStorage.setItem("sayCount", 1);

    } else {

        var temp = parseInt($("div[data-sys_count='" + indx + "']").text().replace(/\s*/g, ""));
        $("div[data-sys_count='" + indx + "']").text(temp + 1);
        $("div[data-sys_title='" + indx + "']").text('收到' + (temp + 1) + '个打招呼');
        $("span[data-sys_time='" + indx + "']").text(price + '元');
        $("div[data-sys_icon='" + indx + "']").removeClass('no-dis');
        localStorage.setItem("sayCount", (temp + 1));

    }

}

//msg.2.当判断处于msg页面的时候的判断逻辑
function msgChatMsg(json) {

    json = JSON.parse(json);

    var curWindow = localStorage.getItem("curWindow");
    var homeIndex = localStorage.getItem("homeIndex");
    var friend_uid = localStorage.getItem("friend_uid");

    var indx = '';

    var TType = parseInt(json.TType);
    var Type = parseInt(json.Type);

    //获取当前窗口处于前台还是后台  Number类型,0:前台;1:后台
    //判断是否处于聊天界面
    if (curWindow == 'chat' && homeIndex == '') {

        if (json.Sid == friend_uid) {

            if (TType == 1) {

                if (Type == 19) {

                    if (isJSON(json.Content) == true) {

                        var temp = json.Content;

                        if (temp.sign == '打招呼') {

                            //getSayHelloList(localStorage.getItem("uid"), 1, 'say_hello');
                            msgChgSay(temp.price);

                        }

                    }

                }

            } else if (TType == 2) {

            } else if (TType == 3) {

                if (Type == 0 || Type == 1) {

                    if (json.Content == '连接成功') {

                    } else if (json.Content == '对方未在线') {

                    }

                } else if (Type == 10) {

                    if (isJSON(json.Content) == true) {

                        var temp = json.Content;

                        if (temp.sign == '好友申请') {

                            //applyGetList(localStorage.getItem("uid"), 1, 'new_friend');
                            msgChgNew(json.Time);

                        }

                    }

                } else if (Type == 11) {

                    msgHiIsrtOne(json.Rid, json.Content, json.Rid, TType, 2, Type, '对方通过了你的朋友验证请求，现在可以开始聊天了');

                }

            }

        } else {

            if (TType == 1) {

                if (Type == 0 || Type == 1) {

                    var msg_cont = '';
                    if (isJSON(json.Content) == true) {

                        var temp = json.Content;
                        msg_cont = temp.content;

                    } else {

                        msg_cont = json.Content;

                    }

                    msgIsrtOne(json.Rid, json.Sid, json.Sid, TType, 1, Type, msg_cont);

                } else if (Type == 19) {

                    if (isJSON(json.Content) == true) {

                        var temp = json.Content;

                        if (temp.sign == '打招呼') {

                            //getSayHelloList(localStorage.getItem("uid"), 1, 'say_hello');
                            msgChgSay(temp.price);

                        }

                    }

                }

            } else if (TType == 2) {

            } else if (TType == 3) {

                if (Type == 0 || Type == 1) {

                    if (json.Content == '连接成功') {

                    } else if (json.Content == '对方未在线') {

                    }

                } else if (Type == 10) {

                    if (isJSON(json.Content) == true) {

                        var temp = json.Content;

                        if (temp.sign == '好友申请') {

                            //applyGetList(localStorage.getItem("uid"), 1, 'new_friend');
                            msgChgNew(json.Time);

                        }

                    }

                } else if (Type == 11) {

                    msgHiIsrtOne(json.Rid, json.Content, json.Rid, TType, 2, Type, '对方通过了你的朋友验证请求，现在可以开始聊天了');

                }

            }

        }

    } else if (curWindow == 'new_friend' && homeIndex == '') {

        if (TType == 1) {

            if (Type == 0 || Type == 1) {

                var msg_cont = '';
                if (isJSON(json.Content) == true) {

                    var temp = json.Content;
                    msg_cont = temp.content;

                } else {

                    msg_cont = json.Content;

                }

                msgIsrtOne(json.Rid, json.Sid, json.Sid, TType, 1, Type, msg_cont);

            } else if (Type == 19) {

                if (isJSON(json.Content) == true) {

                    var temp = json.Content;

                    if (temp.sign == '打招呼') {

                        //getSayHelloList(localStorage.getItem("uid"), 1, 'say_hello');
                        msgChgSay(temp.price);

                    }

                }

            }

        } else if (TType == 2) {

        } else if (TType == 3) {

            if (Type == 0 || Type == 1) {

                if (json.Content == '连接成功') {

                } else if (json.Content == '对方未在线') {

                }

            } else if (Type == 10) {

                if (isJSON(json.Content) == true) {

                    var temp = json.Content;

                    if (temp.sign == '好友申请') {

                        msgChgNew(json.Time);

                    }

                }

            } else if (Type == 11) {

                msgHiIsrtOne(json.Rid, json.Content, json.Rid, TType, 2, Type, '对方通过了你的朋友验证请求，现在可以开始聊天了');

            }

        }

    } else if (curWindow == 'say_hello' && homeIndex == '') {

        if (TType == 1) {

            if (Type == 0 || Type == 1) {

                var msg_cont = '';
                if (isJSON(json.Content) == true) {

                    var temp = json.Content;
                    msg_cont = temp.content;

                } else {

                    msg_cont = json.Content;

                }

                msgIsrtOne(json.Rid, json.Sid, json.Sid, TType, 1, Type, msg_cont);

            } else if (Type == 19) {

                if (isJSON(json.Content) == true) {

                    var temp = json.Content;

                    if (temp.sign == '打招呼') {

                        msgChgSay(temp.price);

                    }

                }

            }

        } else if (TType == 2) {

        } else if (TType == 3) {

            if (Type == 0 || Type == 1) {

                if (json.Content == '连接成功') {

                } else if (json.Content == '对方未在线') {

                }

            } else if (Type == 10) {

                if (isJSON(json.Content) == true) {

                    var temp = json.Content;

                    if (temp.sign == '好友申请') {

                        //applyGetList(localStorage.getItem("uid"), 1, 'new_friend');
                        msgChgNew(json.Time);

                    }

                }

            } else if (Type == 11) {

                msgHiIsrtOne(json.Rid, json.Content, json.Rid, TType, 2, Type, '对方通过了你的朋友验证请求，现在可以开始聊天了');

            }

        }

    } else if (curWindow == 'sys_msg' && homeIndex == '') {

        if (TType == 1) {

            if (Type == 0 || Type == 1) {

                var msg_cont = '';
                if (isJSON(json.Content) == true) {

                    var temp = json.Content;
                    msg_cont = temp.content;

                } else {

                    msg_cont = json.Content;

                }

                msgIsrtOne(json.Rid, json.Sid, json.Sid, TType, 1, Type, msg_cont);

            } else if (Type == 19) {

                if (isJSON(json.Content) == true) {

                    var temp = json.Content;

                    if (temp.sign == '打招呼') {

                        //getSayHelloList(localStorage.getItem("uid"), 1, 'say_hello');
                        msgChgSay(temp.price);

                    }

                }

            }

        } else if (TType == 2) {

        } else if (TType == 3) {

            if (Type == 0 || Type == 1) {

                if (json.Content == '连接成功') {

                } else if (json.Content == '对方未在线') {

                }

            } else if (Type == 10) {

                if (isJSON(json.Content) == true) {

                    var temp = json.Content;

                    if (temp.sign == '好友申请') {

                        //applyGetList(localStorage.getItem("uid"), 1, 'new_friend');
                        msgChgNew(json.Time);

                    }

                }

            } else if (Type == 11) {

                msgHiIsrtOne(json.Rid, json.Content, json.Rid, TType, 2, Type, '对方通过了你的朋友验证请求，现在可以开始聊天了');

            }

        }

    } else {

        if (TType == 1) {

            if (Type == 0 || Type == 1) {

                var msg_cont = '';
                if (isJSON(json.Content) == true) {

                    var temp = json.Content;
                    msg_cont = temp.content;

                } else {

                    msg_cont = json.Content;

                }

                msgIsrtOne(json.Rid, json.Sid, json.Sid, TType, 1, Type, msg_cont);

            } else if (Type == 19) {

                if (isJSON(json.Content) == true) {

                    var temp = json.Content;

                    if (temp.sign == '打招呼') {

                        //getSayHelloList(localStorage.getItem("uid"), 1, 'say_hello');
                        msgChgSay(temp.price);

                    }

                }

            }

        } else if (TType == 2) {

        } else if (TType == 3) {

            if (Type == 0 || Type == 1) {

                if (json.Content == '连接成功') {

                } else if (json.Content == '对方未在线') {

                }

            } else if (Type == 10) {

                if (isJSON(json.Content) == true) {

                    var temp = json.Content;
                    if (temp.sign == '好友申请') {

                        //applyGetList(localStorage.getItem("uid"), 1, 'new_friend');
                        msgChgNew(json.Time);

                    }

                }

            } else if (Type == 11) {

                msgHiIsrtOne(json.Rid, json.Content, json.Rid, TType, 2, Type, '对方通过了你的朋友验证请求，现在可以开始聊天了');

            }

        }

    }

}