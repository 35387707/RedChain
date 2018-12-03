//滚动条在Y轴上的滚动距离
function getScrollTop() {
    var scrollTop = 0,
        bodyScrollTop = 0,
        documentScrollTop = 0;
    if (document.body) {
        bodyScrollTop = document.body.scrollTop;
    }
    if (document.documentElement) {
        documentScrollTop = document.documentElement.scrollTop;
    }
    scrollTop = (bodyScrollTop - documentScrollTop > 0) ? bodyScrollTop : documentScrollTop;
    return scrollTop;
}

//文档的总高度
function getScrollHeight() {
    var scrollHeight = 0,
        bodyScrollHeight = 0,
        documentScrollHeight = 0;
    if (document.body) {
        bodyScrollHeight = document.body.scrollHeight;
    }
    if (document.documentElement) {
        documentScrollHeight = document.documentElement.scrollHeight;
    }
    scrollHeight = (bodyScrollHeight - documentScrollHeight > 0) ? bodyScrollHeight : documentScrollHeight;
    return scrollHeight;
}

//浏览器视口的高度
function getWindowHeight() {
    var windowHeight = 0;
    if (document.compatMode == "CSS1Compat") {
        windowHeight = document.documentElement.clientHeight;
    } else {
        windowHeight = document.body.clientHeight;
    }
    return windowHeight;
}

var isAndroid = (window.navigator.userAgent.indexOf('Android') >= 0) || (window.navigator.userAgent.indexOf('Linux') >= 0);

var androidPaddingTop = 2.8125;
//Android PaddingTop
var isSend = true;
//本人发送的消息
var chatListViewInstance = {
    getElementHeight : function() {
        var retHeight = 0;
        var eles = document.getElementById('content') && document.getElementById('content').children;
        if (eles && eles.length > 0) {
            for (var i = 0; i < eles.length; i++) {
                retHeight += eles[i].offsetHeight;
            }
        } else {
            retHeight = document.getElementById('content').offsetHeight
        }
        return retHeight;
    },
    /**
     * 发送成功页面滚到最底部
     */
    toDown : function() {
        var isBottom = 0;
        //判断滚动条是否在底部
        if (getScrollHeight() - getScrollTop() - getWindowHeight() < 20) {
            isBottom = 1;
        }

        window.scrollTo(0, document.body.scrollHeight);
        if (isBottom || isSend) {
            if (isAndroid) {
                window.scrollTo(0, document.body.scrollHeight);
            } else {
                var heightFrame = this.getElementHeight();
                uexChatKeyboard.changeWebViewFrame(heightFrame);
            }
        }
    }
}

var chatJson = '';
appcan.ready(function() {

    var TrueName = '';
    var UserInfo = JSON.parse(localStorage.getItem("UserInfo"));
    if (UserInfo.Info.TrueName == '' || UserInfo.Info.TrueName == null || UserInfo.Info.TrueName == undefined) {
        TrueName = UserInfo.Info.Phone;
    } else {
        TrueName = UserInfo.Info.TrueName;
    }

    setSwipeCloseEnable(1);

    appcan.window.enableBounce();

    chatSrchBegin(localStorage.getItem("uid"), localStorage.getItem("friend_uid"), 1, 2);

    appcan.window.subscribe('recvMsg', function(msg) {

        if (msg != "" && msg != null && msg != undefined) {

            chatWsMsg(msg);

        }

    });

    appcan.window.subscribe('upldChatImg', function(msg) {

        if (msg != "" && msg != null && msg != undefined) {

            debug('开始上传聊天图片了哦');

            var imgArr = JSON.parse(msg);
            doImgUpload(imgArr, 0, imgArr.length, 'chat');

        }

    });

    appcan.window.subscribe('delMsgLog', function(msg) {

        if (msg != "" && msg != null && msg != undefined) {

            delMsgLogBegin();

        }

    });

    appcan.window.subscribe('dispChatMsg', function(msg) {

        if (msg != "" && msg != null && msg != undefined) {

            debug('似乎是真的进来了哦');

            splCurUserChat(msg);

        }

    });

    if (!isAndroid) {

        appcan.initBounce();

    } else {

        //默认不要被内容挡住, 自动滚到最下面
        //$("#content").css("padding-bottom", androidPaddingTop + "em");

    }

    setTimeout(function() {
        window.scrollTo(0, document.body.scrollHeight);
    }, 300);

    if (isAndroid) {
        chatJson = {
            "emojicons" : "res://emojicons/emojicons.xml",
            "shares" : "res://shares/shares.xml",
            "placeHold" : "请输入内容",
            "touchDownImg" : "res://audio_start@2x.png",
            "dragOutsideImg" : "res://audio_stop@2x.png",
            "textColor" : "#FFF",
            "textSize" : "15.5",
            "sendBtnbgColorUp" : "#FF5265",
            "sendBtnbgColorDown" : "#298409",
            "sendBtnText" : "发送",
            "sendBtnTextSize" : "15.5",
            "sendBtnTextColor" : "#FFF",
        };
    } else {
        chatJson = {
            "emojicons" : "res://emojicons/emojicons.xml",
            "shares" : "res://shares/shares_ios.xml",
            "textColor" : "#FFF",
            "textSize" : "15.5"
        };
    }

    uexChatKeyboard.open(JSON.stringify(chatJson));

    uexChatKeyboard.onKeyBoardShow = function(json) {

        appcan.logs("onKeyBoardShow:" + json);

        var data = JSON.parse(json);

        if (isAndroid) {
            setTimeout(function() {
                window.scrollTo(0, document.body.scrollHeight);
            }, 300);
        }

        if (data.status == 1) {//键盘弹出状态

            $('.chat-bottom').remove();
            if (isAndroid) {
                //$("#content").css("padding", androidPaddingTop + "em 0 0");
            } else {
                var heightFrame = chatListViewInstance.getElementHeight();
                appcan.logs("IOS" + heightFrame);
                uexChatKeyboard.changeWebViewFrame(heightFrame);
            }

        } else {

            $('#content').append('<div class="chat-bottom"></div>');
            if (isAndroid) {
                //$("#content").css("padding", androidPaddingTop + "em 0");
            } else {
                var heightFrame = chatListViewInstance.getElementHeight();
                appcan.logs("IOS" + heightFrame);
                uexChatKeyboard.changeWebViewFrame(heightFrame);
            }

        }

    };

    uexChatKeyboard.onCommit = function(json) {//发送文本内容时的的监听状态,成功失败

        var msg = eval('(' + trimText(json) + ')');
        msg = msg.emojiconsText;
        if (trim(msg) == '') {
            openToast("不能发送空格", 1500, 5, 0);
            return;
        }
        /*
         var str = trimText(msg);
         var reg = /\[([^\]]+)\]/g;
         str = str.replace(reg, function($1) {
         return emojiJson[$1] || $1;
         });

         debug(str);

         if (str.indexOf('../wgtRes/emojicons/ace_emoji') != -1) {

         } else {

         }*/

        var content = {
            name : TrueName,
            content : msg,
        }

        queryMessages('chat', 1, localStorage.getItem("friend_uid"), '', 0, 0, content, 'chat');

        //alert(msg1);
        chatListViewInstance.toDown();

    }
    //点击分享里选项的监听方法
    uexChatKeyboard.onShareMenuItem = function(menu) {
        if (menu == 0) {

            //openToast("正在打开照相机...", 2000, 5, 1);
            openCamera('chat');

        } else if (menu == 1) {

            //openToast("正在打开相册...", 2000, 5, 1);
            //openPicker('chat');
            toImgPicker(9, 'chat');

        } else if (menu == 2) {

            openToast("功能正在开发中，敬请期待...", 3000, 5, 0);

        }

    }

    uexChatKeyboard.onVoiceAction = function(data) {
        var statJson = JSON.parse(data);
        /*
         if (statJson.status == 1) {
         }*/
        debug(statJson);
        openToast("录音功能暂不开放", 3000, 5, 0);

    };

});
