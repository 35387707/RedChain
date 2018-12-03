/*
 * 接口.69.发送消息
 */
function queryMessages(Api, TType, Rid, Gid, Type, Status, Content, fromPage) {

    var uid = localStorage.getItem("uid");

    var sendMsg = chatMsgEditor(Api, TType, Rid, Gid, Type, Status, Content, fromPage);

    debug(sendMsg);

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Chat/QueryMessages",
        type : "POST",
        data : sendMsg,
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    if (fromPage == 'srch_friend') {

                        var waitStr = '<div class="wait-passed">';
                        waitStr += '待通过';
                        waitStr += '</div>';

                        $("div[data-spec='" + Rid + "']").children().remove();
                        $("div[data-spec='" + Rid + "']").append(waitStr);

                    } else if (fromPage == 'say_hello') {
                        
                        var OtherUser = JSON.parse(localStorage.getItem('OtherUser'));
                        sayHelloSqlSrch(uid, OtherUser, 19, data.msg);

                    } else {

                        chatIsrtOne(uid, Rid, uid, 1, 1, Type, Content.content, data.msg);

                    }

                } else {

                    openToast(data.msg, 5000, 5, 0);

                }

            } else {

                openToast('发送消息失败', 5000, 5, 0);

            }

        },
        error : function(xhr, errorType, error, msg) {

            var debugMsg = {
                xhr : xhr,
                errorType : errorType,
                error : error,
                msg : msg,
            }
            debug(debugMsg);
            openToast(localStorage.getItem("errorMsg"), 3000, 5, 0);

        },
        
    });

}

/*
 * 接口.69.1.发送图片的逻辑
 */
function queryMsgImg(Api, TType, Rid, Gid, Type, Status, Content, imgArr, i, length, fromPage, sendMsg) {

    var Sid = localStorage.getItem("uid");
    
    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Chat/QueryMessages",
        type : "POST",
        data : sendMsg,
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    //chatImgIsrtOne(uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, imgArr, i, length, fromPage, mid)
                    chatImgIsrtOne(Sid, Rid, Sid, TType, Status, Type, Content, imgArr, i, length, fromPage, data.msg);

                } else {

                    openToast(data.msg, 3000, 5, 0);

                }

            } else {

                openToast('发送消息失败', 3000, 5, 0);

            }

        },
        error : function(xhr, errorType, error, msg) {

            var debugMsg = {
                xhr : xhr,
                errorType : errorType,
                error : error,
                msg : msg,
            }
            debug(debugMsg);
            openToast(localStorage.getItem("errorMsg"), 3000, 5, 0);

        },
    });

}