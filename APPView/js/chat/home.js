function homeChgCnt(indx) {

    if ($("div[data-tab_items='" + indx + "']").hasClass('no-dis')) {

        $("div[data-tab_items='" + indx + "']").removeClass('no-dis');
        $("div[data-tab_items='" + indx + "']").text(1);

    } else {

        var temp = parseInt($("div[data-tab_items='" + indx + "']").text().replace(/\s*/g, ""));
        $("div[data-tab_items='" + indx + "']").text(temp + 1);

    }

}

/*
* msg.对ws发送过来的消息进行解析
*/

//msg.1.当判断处于主页的时候的判断逻辑
function homeChatMsg(json) {

    json = JSON.parse(json);

    debug(json);

    //(TType == 3) && json.Content == '连接成功'

    //uexWindow.statusBarNotification('状态栏消息', '您有一条好友申请');
    //好友申请特殊点， 因为需要响应  11改为“好友通过你的验证”

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

    var bodyWidth = localStorage.getItem("bodyWidth");
    var bodyHeight = localStorage.getItem("bodyHeight");

    if (curWindow == 'chat' && homeIndex == '') {

        if (json.Sid == friend_uid) {

            if (TType == 1) {

                if (Type == 19) {

                    if (isJSON(json.Content) == true) {

                        var temp = json.Content;

                        if (temp.sign == '打招呼') {

                            uexWindow.statusBarNotification(temp.name, '福包敲门中');

                        }
                        
                        homeChgCnt(indx);

                    }

                }

            } else if (TType == 2) {

            } else if (TType == 3) {

                if (Type == 0) {

                    if (json.Content == '连接成功') {

                    } else if (json.Content == '对方未在线') {

                    }

                } else if (Type == 10) {

                    if (isJSON(json.Content) == true) {

                        var temp = json.Content;

                        if (temp.sign == '好友申请') {

                            uexWindow.statusBarNotification(temp.name, '请求添加你为好友');

                        }
                        
                        homeChgCnt(indx);

                    }

                } else if (Type == -3) {

                    showLoading(json.Content, 1);
                    localStorage.setItem("uid", '');
                    localStorage.setItem("password", '');
                    setTimeout(function() {

                        uexWidgetOne.restart();

                    }, 3000);

                }

            }

        } else {

            if (TType == 1) {

                if (Type == 0) {

                    if (isJSON(json.Content) == true) {

                        var temp = json.Content;

                        uexWindow.statusBarNotification(temp.name, temp.content);

                    } else {

                        uexWindow.statusBarNotification('福包多', '好友发来新消息');

                    }

                } else if (Type == 1) {

                    if (isJSON(json.Content) == true) {

                        var temp = json.Content;
                        uexWindow.statusBarNotification(temp.name, '[图片]');

                    } else {

                        uexWindow.statusBarNotification('福包多', '好友发来新消息');

                    }

                } else if (Type == 19) {

                    if (isJSON(json.Content) == true) {

                        var temp = json.Content;

                        if (temp.sign == '打招呼') {

                            uexWindow.statusBarNotification(temp.name, '福包敲门中');

                        }
                        
                        homeChgCnt(indx);

                    }

                }

            } else if (TType == 2) {

            } else if (TType == 3) {

                if (Type == 0) {

                    if (json.Content == '连接成功') {

                    } else if (json.Content == '对方未在线') {

                    }

                } else if (Type == 10) {

                    if (isJSON(json.Content) == true) {

                        var temp = json.Content;

                        if (temp.sign == '好友申请') {

                            uexWindow.statusBarNotification(temp.name, '请求添加你为好友');

                        }
                        
                        homeChgCnt(indx);

                    }

                } else if (Type == -3) {

                    showLoading(json.Content, 1);
                    localStorage.setItem("uid", '');
                    localStorage.setItem("password", '');
                    setTimeout(function() {

                        uexWidgetOne.restart();

                    }, 3000);

                }

            }

        }

    } else {

        if (TType == 1) {

            if (Type == 0) {

                if (isJSON(json.Content) == true) {

                    var temp = json.Content;
                    uexWindow.statusBarNotification(temp.name, temp.content);

                } else {

                    uexWindow.statusBarNotification('福包多', '好友发来新消息');

                }

            } else if (Type == 1) {

                if (isJSON(json.Content) == true) {

                    var temp = json.Content;
                    uexWindow.statusBarNotification(temp.name, '[图片]');

                } else {

                    uexWindow.statusBarNotification('福包多', '好友发来新消息');

                }

            } else if (Type == 19) {

                if (isJSON(json.Content) == true) {

                    var temp = json.Content;

                    if (temp.sign == '打招呼') {

                        uexWindow.statusBarNotification(temp.name, '福包敲门中');

                    }
                    
                    homeChgCnt(indx);

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

                        uexWindow.statusBarNotification(temp.name, '请求添加你为好友');

                    }
                    
                    homeChgCnt(indx);

                } else {

                    openToast('已经发送过好友请求', 3000, 5, 0);

                }

            } else if (Type == -3) {

                showLoading(json.Content, 1);
                localStorage.setItem("uid", '');
                localStorage.setItem("password", '');
                setTimeout(function() {

                    uexWidgetOne.restart();

                }, 3000);

            }

        }

    }

}