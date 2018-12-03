//登录的逻辑
function reLoginActn() {

    var username = localStorage.getItem("username");
    var password = localStorage.getItem("password");
    var before = localStorage.getItem("before");

    var ajaxUrl = localStorage.getItem("ajaxUrl");

    appcan.ajax({
        url : ajaxUrl + "Account/DoLogin",
        type : "POST",
        data : {
            before : before,
            phone : username,
            pwd : password,
            remember : 1,
            ostype : '',
            device : '',
        },
        timeout : 30000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            //debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '') {

                    reOpenWs();

                } else {

                    hideLoading(data.msg, 3000, 5, 0);

                }

            } else {

                hideLoading('登录失败', 3000, 5, 0);

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
            hideLoading(localStorage.getItem("errorMsg"), 3000, 5, 0);

        },
        
    });

}

/*
 * 发送命令去关掉服务器的ws连接
 */
function reCloseWs() {

    var ajaxUrl = localStorage.getItem("ajaxUrl");

    appcan.ajax({
        url : ajaxUrl + "Account/LogOut",
        type : "POST",
        data : {
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            reOpenWs();

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

function reOpenWs(){
    
    var wsUrl = localStorage.getItem("wsUrl");
    var uid = localStorage.getItem("uid");
    var token = '';
    var data = {
        url : wsUrl + '/IM/WSConnet?id=' + uid + '&token=' + token,
    }
    debug(data);
    uexWebSocket.open(data);

}
