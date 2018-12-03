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
            }
        }
    }
}

var chatJson = '';
appcan.ready(function() {

    var footHeight = $('#footer').height();
    $('#content').css('padding-bottom', footHeight);

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

    setTimeout(function() {
        window.scrollTo(0, document.body.scrollHeight);
    }, 300);

});
