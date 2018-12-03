//0.编辑发送msg的json
function wsMsgEditor(Api, TType, Rid, Gid, Type, Status, Content, fromPage) {

    var Sid = localStorage.getItem("uid");
    var Time = new Date().format("yyyy-MM-dd hh:mm:ss");

    var msg = {
        Api : Api,
        Sid : Sid,
        TType : TType,
        Rid : Rid,
        Gid : Gid,
        Type : Type,
        Status : Status,
        Time : Time,
        Content : Content,
    }

    appcan.window.publish('sendMsg', JSON.stringify(msg));

    if (fromPage == 'chat') {

        setTimeout(function() {

            chatIsrtOne(Sid, Rid, Sid, 1, 2, Type, Content);

        }, 360);

    }

}

//1.返送图片的ws要特殊处理
function wsImgMsgEditor(Api, TType, Rid, Gid, Type, Status, Content, imgArr, i, length, fromPage) {

    var Sid = localStorage.getItem("uid");
    var Time = new Date().format("yyyy-MM-dd hh:mm:ss");

    var msg = {
        Api : Api,
        Sid : Sid,
        TType : TType,
        Rid : Rid,
        Gid : Gid,
        Type : Type,
        Status : Status,
        Time : Time,
        Content : Content,
    }

    appcan.window.publish('sendMsg', JSON.stringify(msg));

    if (fromPage == 'chat') {

        setTimeout(function() {

            //uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, fromPage)
            chatImgIsrtOne(Sid, Rid, Sid, 1, 2, Type, Content, imgArr, i, length, fromPage);

        }, 250);

    }

}

//2.定义一些不必保存本地sqlite的ws, 比如打招呼
function wsSayHello(Api, TType, Rid, Gid, Type, Status, Content) {

    var Sid = localStorage.getItem("uid");
    var Time = new Date().format("yyyy-MM-dd hh:mm:ss");

    var msg = {
        Api : Api,
        Sid : Sid,
        TType : TType,
        Rid : Rid,
        Gid : Gid,
        Type : Type,
        Status : Status,
        Time : Time,
        Content : Content,
    }

    appcan.window.publish('sendMsg', JSON.stringify(msg));

}

/*
* msg.对ws发送过来的消息进行解析
*/

//msg.1.当判断处于主页的时候的判断逻辑
function homeWsMsg(json) {

    //(TType == 3) && json.Content == '连接成功'

    //uexWindow.statusBarNotification('状态栏消息', '您有一条好友申请');
    //好友申请特殊点， 因为需要响应  11改为“好友通过你的验证”

    json = JSON.parse(json);

    var TrueName = '';
    var UserInfo = JSON.parse(localStorage.getItem("UserInfo"));
    if (UserInfo.Info.TrueName == '' || UserInfo.Info.TrueName == null || UserInfo.Info.TrueName == undefined) {
        TrueName = UserInfo.Info.Phone;
    } else {
        TrueName = UserInfo.Info.TrueName;
    }

    var curWindow = localStorage.getItem("curWindow");
    var homeIndex = localStorage.getItem("homeIndex");
    var friend_uid = localStorage.getItem("friend_uid");

    var indx = '消息';

    var TType = parseInt(json.TType);
    var Type = parseInt(json.Type);

    if (curWindow == 'chat' && homeIndex == '') {

        if (json.Sid == friend_uid) {

            if (TType == 1) {

            } else if (TType == 2) {

            } else if (TType == 3) {

                if (Type == 0) {

                    if (json.Content == '连接成功') {

                    } else if (json.Content == '对方未在线') {

                    }

                } else if (Type == 10) {

                    if (json.Content == '好友申请') {

                        uexWindow.statusBarNotification(temp.name, '请求添加你为好友');
                        if ($("a[data-tab_items='" + indx + "']").hasClass('no-dis')) {

                            $("a[data-tab_items='" + indx + "']").removeClass('no-dis');
                            $("a[data-tab_items='" + indx + "']").text(1);

                        } else {

                            var temp = parseInt($("a[data-tab_items='" + indx + "']").text());
                            $("a[data-tab_items='" + indx + "']").text(temp + 1);

                        }

                    }

                }

            }

        } else {

            if (TType == 1) {

                if (Type == 0 || Type == 1) {

                    uexWindow.statusBarNotification('福包多', '好友发来新消息');

                    if ($("a[data-tab_items='" + indx + "']").hasClass('no-dis')) {

                        $("a[data-tab_items='" + indx + "']").removeClass('no-dis');
                        $("a[data-tab_items='" + indx + "']").text(1);

                    } else {

                        var temp = parseInt($("a[data-tab_items='" + indx + "']").text());
                        $("a[data-tab_items='" + indx + "']").text(temp + 1);

                    }

                }

            } else if (TType == 2) {

            } else if (TType == 3) {

                if (Type == 0) {

                    if (json.Content == '连接成功') {

                    } else if (json.Content == '对方未在线') {

                    }

                } else if (Type == 10) {

                    if (json.Content == '好友申请') {

                        uexWindow.statusBarNotification(temp.name, '请求添加你为好友');
                        if ($("a[data-tab_items='" + indx + "']").hasClass('no-dis')) {

                            $("a[data-tab_items='" + indx + "']").removeClass('no-dis');
                            $("a[data-tab_items='" + indx + "']").text(1);

                        } else {

                            var temp = parseInt($("a[data-tab_items='" + indx + "']").text());
                            $("a[data-tab_items='" + indx + "']").text(temp + 1);

                        }

                    }

                }

            }

        }

    } else if (curWindow == 'new_friend' || homeIndex == '') {

        if (TType == 1) {

            if (Type == 0 || Type == 1) {

                uexWindow.statusBarNotification('福包多', '好友发来新消息');
                if ($("a[data-tab_items='" + indx + "']").hasClass('no-dis')) {

                    $("a[data-tab_items='" + indx + "']").removeClass('no-dis');
                    $("a[data-tab_items='" + indx + "']").text(1);

                } else {

                    var temp = parseInt($("a[data-tab_items='" + indx + "']").text());
                    $("a[data-tab_items='" + indx + "']").text(temp + 1);

                }

            }

        } else if (TType == 2) {

        } else if (TType == 3) {

            if (Type == 0 || Type == 1) {

                if (json.Content == '连接成功') {

                } else if (json.Content == '对方未在线') {

                } else {
                    uexWindow.statusBarNotification('福包多', '好友发来新消息');
                }

            } else if (Type == 10) {

                if (json.Content == '好友申请') {

                    uexWindow.statusBarNotification(temp.name, '请求添加你为好友');
                    if ($("a[data-tab_items='" + indx + "']").hasClass('no-dis')) {

                        $("a[data-tab_items='" + indx + "']").removeClass('no-dis');
                        $("a[data-tab_items='" + indx + "']").text(1);

                    } else {

                        var temp = parseInt($("a[data-tab_items='" + indx + "']").text());
                        $("a[data-tab_items='" + indx + "']").text(temp + 1);

                    }

                }

            }

        }

    } else {

        if (TType == 1) {

            if (Type == 0 || Type == 1) {

                uexWindow.statusBarNotification('福包多', '好友发来新消息');

                if ($("a[data-tab_items='" + indx + "']").hasClass('no-dis')) {

                    $("a[data-tab_items='" + indx + "']").removeClass('no-dis');
                    $("a[data-tab_items='" + indx + "']").text(1);

                } else {

                    var temp = parseInt($("a[data-tab_items='" + indx + "']").text());
                    $("a[data-tab_items='" + indx + "']").text(temp + 1);

                }

            }

        } else if (TType == 2) {

        } else if (TType == 3) {

            if (Type == 0 || Type == 1) {

                if (json.Content == '连接成功') {

                } else if (json.Content == '对方未在线') {

                }

            } else if (Type == 10) {

                if (json.Content == '好友申请') {

                    uexWindow.statusBarNotification(temp.name, '请求添加你为好友');
                    if ($("a[data-tab_items='" + indx + "']").hasClass('no-dis')) {

                        $("a[data-tab_items='" + indx + "']").removeClass('no-dis');
                        $("a[data-tab_items='" + indx + "']").text(1);

                    } else {

                        var temp = parseInt($("a[data-tab_items='" + indx + "']").text());
                        $("a[data-tab_items='" + indx + "']").text(temp + 1);

                    }

                } else {

                    openToast('已经发送过好友请求', 5000, 5, 0);

                }

            }

        }

    }

}

//msg.2.当判断处于msg页面的时候的判断逻辑
function msgWsMsg(json) {

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

            } else if (TType == 2) {

            } else if (TType == 3) {

                if (Type == 0 || Type == 1) {

                    if (json.Content == '连接成功') {

                    } else if (json.Content == '对方未在线') {

                    }

                } else if (Type == 10) {

                    if (json.Content == '好友申请') {

                        indx = '新的朋友';
                        if ($("div[data-sys_count='" + indx + "']").hasClass('no-dis')) {

                            $("div[data-sys_count='" + indx + "']").removeClass('no-dis');
                            $("div[data-sys_count='" + indx + "']").text(1);
                            $("span[data-sys_txt='" + indx + "']").text('您有');
                            $("span[data-sys_red='" + indx + "']").text('1个好友申请');
                            $("div[data-sys_time='" + indx + "']").text(json.Time);

                        } else {

                            var temp = parseInt($("div[data-sys_count='" + indx + "']").text());
                            $("div[data-sys_count='" + indx + "']").text(temp + 1);
                            $("span[data-sys_txt='" + indx + "']").text('您有');
                            $("span[data-sys_red='" + indx + "']").text((temp + 1) + '个好友申请');
                            $("div[data-sys_time='" + indx + "']").text(json.Time);

                        }

                    }

                } else if (Type == 11) {

                    msgHiIsrtOne(json.Rid, json.Content, json.Rid, TType, 2, Type, '对方通过了你的朋友验证请求，现在可以开始聊天了');

                }

            }

        } else {

            if (TType == 1) {

                if (Type == 0 || Type == 1) {

                    msgIsrtOne(json.Rid, json.Sid, json.Sid, TType, 1, Type, json.Content);

                }

            } else if (TType == 2) {

            } else if (TType == 3) {

                if (Type == 0 || Type == 1) {

                    if (json.Content == '连接成功') {

                    } else if (json.Content == '对方未在线') {

                    }

                } else if (Type == 10) {

                    if (json.Content == '好友申请') {

                        indx = '新的朋友';
                        if ($("div[data-sys_count='" + indx + "']").hasClass('no-dis')) {

                            $("div[data-sys_count='" + indx + "']").removeClass('no-dis');
                            $("div[data-sys_count='" + indx + "']").text(1);
                            $("span[data-sys_txt='" + indx + "']").text('您有');
                            $("span[data-sys_red='" + indx + "']").text('1个好友申请');
                            $("div[data-sys_time='" + indx + "']").text(json.Time);

                        } else {

                            var temp = parseInt($("div[data-sys_count='" + indx + "']").text());
                            $("div[data-sys_count='" + indx + "']").text(temp + 1);
                            $("span[data-sys_txt='" + indx + "']").text('您有');
                            $("span[data-sys_red='" + indx + "']").text((temp + 1) + '个好友申请');
                            $("div[data-sys_time='" + indx + "']").text(json.Time);

                        }

                    }

                } else if (Type == 11) {

                    msgHiIsrtOne(json.Rid, json.Content, json.Rid, TType, 2, Type, '对方通过了你的朋友验证请求，现在可以开始聊天了');

                }

            }

        }

    } else {

        if (TType == 1) {

            if (Type == 0 || Type == 1) {

                msgIsrtOne(json.Rid, json.Sid, json.Sid, TType, 1, Type, json.Content);

            }

        } else if (TType == 2) {

        } else if (TType == 3) {

            if (Type == 0 || Type == 1) {

                if (json.Content == '连接成功') {

                } else if (json.Content == '对方未在线') {

                }

            } else if (Type == 10) {

                if (json.Content == '好友申请') {

                    indx = '新的朋友';
                    if ($("div[data-sys_count='" + indx + "']").hasClass('no-dis')) {

                        $("div[data-sys_count='" + indx + "']").removeClass('no-dis');
                        $("div[data-sys_count='" + indx + "']").text(1);
                        $("span[data-sys_txt='" + indx + "']").text('您有');
                        $("span[data-sys_red='" + indx + "']").text('1个好友申请');
                        $("div[data-sys_time='" + indx + "']").text(json.Time);

                    } else {

                        var temp = parseInt($("div[data-sys_count='" + indx + "']").text());
                        $("div[data-sys_count='" + indx + "']").text(temp + 1);
                        $("span[data-sys_txt='" + indx + "']").text('您有');
                        $("span[data-sys_red='" + indx + "']").text((temp + 1) + '个好友申请');
                        $("div[data-sys_time='" + indx + "']").text(json.Time);

                    }

                }

            } else if (Type == 11) {

                msgHiIsrtOne(json.Rid, json.Content, json.Rid, TType, 2, Type, '对方通过了你的朋友验证请求，现在可以开始聊天了');

            }

        }

    }

}

//msg.3.当判断处于chat页面的时候的判断逻辑
function chatWsMsg(json) {

    json = JSON.parse(json);

    var curWindow = localStorage.getItem("curWindow");
    var homeIndex = localStorage.getItem("homeIndex");
    var friend_uid = localStorage.getItem("friend_uid");

    var TType = parseInt(json.TType);
    var Type = parseInt(json.Type);

    if (curWindow == 'chat' && homeIndex == '') {

        //chat前台, 聊天的id与接收者id相同
        if (json.Sid == friend_uid) {

            if (TType == 1) {

                if (Type == 0 || Type == 1) {

                    chatIsrtOne(json.Rid, json.Sid, json.Sid, TType, 2, Type, json.Content);

                }

            }

        } else {

            if (TType == 3) {

                if (Type == 0) {

                    if (json.Content == '对方未在线') {

                        openToast(json.Content, 5000, 5, 0);

                    }

                }

            }

        }

    }

}

//msg.4.当判断处于sys_msg页面的时候的判断逻辑
function sysWsMsg(json) {

    json = JSON.parse(json);

    var curWindow = localStorage.getItem("curWindow");
    var homeIndex = localStorage.getItem("homeIndex");
    var friend_uid = localStorage.getItem("friend_uid");

    var TType = parseInt(json.TType);
    var Type = parseInt(json.Type);

    if (curWindow == 'chat' && homeIndex == '') {

        //chat前台, 聊天的id与接收者id相同
        if (json.Sid == friend_uid) {

            if (TType == 1) {

                if (Type == 0 || Type == 1) {

                    chatIsrtOne(json.Rid, json.Sid, json.Sid, TType, 2, Type, json.Content);

                }

            } else if (TType == 2) {

            } else if (TType == 3) {

            }

        } else {

            if (TType == 3) {

                if (json.Content == '对方未在线') {

                    openToast(json.Content, 5000, 5, 0);

                }

            }

        }

    }

}

//msg.5.当判断处于new_friend页面的时候的判断逻辑
function newWsMsg(json) {

    json = JSON.parse(json);

    var curWindow = localStorage.getItem("curWindow");
    var homeIndex = localStorage.getItem("homeIndex");
    var friend_uid = localStorage.getItem("friend_uid");

    var TType = parseInt(json.TType);
    var Type = parseInt(json.Type);

    if (curWindow == 'new_friend' && homeIndex == '') {

        if (TType == 1) {

            if (Type == 99) {

                //wsSayHello('打招呼', 1, json.Sid, '', 0, 0, '我通过了你的朋友验证请求，现在我们可以开始聊天了');
                //

            }

        } else if (TType == 3) {

            if (Type == 10) {

                if (json.Content == '好友申请') {

                    applyGetList(localStorage.getItem("uid"), 1, 'new_friend');

                }

            }

        }

    }

}

//msg.6.当判断处于say_hello页面的时候的判断逻辑
function sayWsMsg(json) {

    json = JSON.parse(json);

    var curWindow = localStorage.getItem("curWindow");
    var homeIndex = localStorage.getItem("homeIndex");
    var friend_uid = localStorage.getItem("friend_uid");

    var TType = parseInt(json.TType);
    var Type = parseInt(json.Type);

    if (curWindow == 'say_hello' && homeIndex == '') {

        //chat前台, 聊天的id与接收者id相同
        if (json.Sid == friend_uid) {

            if (TType == 1) {

                if (Type == 0 || Type == 1) {

                    chatIsrtOne(json.Rid, json.Sid, json.Sid, TType, 2, Type, json.Content);

                }

            } else if (TType == 2) {

            } else if (TType == 3) {

            }

        } else {

            if (TType == 3) {

                if (json.Content == '对方未在线') {

                    openToast(json.Content, 5000, 5, 0);

                }

            }

        }

    }

}

//msg.7.当判断处于srch_friend页面的时候的判断逻辑
function srchWsMsg(json) {

    json = JSON.parse(json);

    var waitStr = '';

    var curWindow = localStorage.getItem("curWindow");
    var homeIndex = localStorage.getItem("homeIndex");
    var friend_uid = localStorage.getItem("friend_uid");

    var TType = parseInt(json.TType);
    var Type = parseInt(json.Type);

    if (curWindow == 'srch_friend' && homeIndex == '') {

        //chat前台, 聊天的id与接收者id相同
        if (TType == 3) {

            if (Type == 10) {

                if (json.Content == '已经发送过好友请求') {

                    waitStr = '<div class="wait-passed">';
                    waitStr += '待通过';
                    waitStr += '</div>';

                    var Rid = localStorage.getItem("tempRid");

                    openToast(json.Content, 5000, 5, 0);
                    $("div[data-spec='" + Rid + "']").children().remove();
                    $("div[data-spec='" + Rid + "']").append(waitStr);

                }

            } else if (Type == 11) {

                waitStr = '<div class="wait-passed">';
                waitStr += '已添加';
                waitStr += '</div>';

                openToast('已添加好友成功', 5000, 5, 0);

                $("div[data-spec='" + json.Content + "']").children().remove();
                $("div[data-spec='" + json.Content + "']").append(waitStr);

            }

        }

    }

}

