//自定义调试函数
//8928209c-2cb7-4ae0-a538-79835746a574
function debug(msg) {

    //模式说明: 0为开发模式   1为测试模式   2为用户模式
    var appMode = localStorage.getItem('appMode');
    if (appMode == 0) {

        if (jQuery.type(msg) === 'object') {
            msg = JSON.stringify(msg);
        } else if (jQuery.type(msg) === 'array') {

            var sign = 0;
            $(msg).each(function(i, v) {

                if (jQuery.type(msg[i]) === 'object') {
                    sign = 'json';
                }

            });

            if (sign == 'json') {
                msg = JSON.stringify(msg);
            } else {
                msg = msg.toString();
            }

        } else if (jQuery.type(msg) === 'date') {
            //msg = msg.toTimeString();
        }

        alert(msg);

    }

}

function iosDebug(msg) {

    //模式说明: 0为开发模式   1为测试模式   2为用户模式
    alert('进入这里了木有');

    if (jQuery.type(msg) === 'object') {
        msg = JSON.stringify(msg);
    } else if (jQuery.type(msg) === 'array') {

        var sign = 0;
        $(msg).each(function(i, v) {

            if (jQuery.type(msg[i]) === 'object') {
                sign = 'json';
            }

        });

        if (sign == 'json') {
            msg = JSON.stringify(msg);
        } else {
            msg = msg.toString();
        }

    } else if (jQuery.type(msg) === 'date') {
        //msg = msg.toTimeString();
    }

    alert(msg);

}

/*
 * 发送命令去关掉服务器的ws连接
 */
function ajaxCloseWs() {

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

            debug(data);

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
