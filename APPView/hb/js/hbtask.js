/*
 * 接口.66.获取今日已领取红包数量
 */
function getReciveRedPackCount(uid, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "RedPacket/GetReciveRedPackCount",
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
                    
                    //localStorage.setItem("fromPage", 'hbcomment');
                    setReciveRedPackCount(uid, data, fromPage)

                } else {

                    openToast(data.msg, 3000, 5, 0);

                }

            } else {

                openToast('获取任务失败', 3000, 5, 0);

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

//66.1.设置今日已领取红包数量
function setReciveRedPackCount(uid, json, fromPage) {
    
    var count = '';
    var maxcount = '';
    
    count = json.count;
    maxcount = json.maxcount;
    
    $('#count').text(count);
    $('#maxcount').text(maxcount);


}