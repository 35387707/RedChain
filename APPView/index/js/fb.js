/*
 * 发一个福包一块钱可以加一个好友
 */
function chgFbFriendNum() {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Friend/GetFriendList",
        type : "POST",
        data : {
        },
        timeout : 30000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    localStorage.setItem("maxcount", data.maxcount);

                    var maxcount = data.maxcount;

                    var makecount = data.list.length;

                    $('#maxcount').text(maxcount);
                    $('#makecount').text(makecount);

                } else {

                    debug(data.msg);

                }

            } else {

                debug('获取好友列表失败');

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

        },
    });

}
