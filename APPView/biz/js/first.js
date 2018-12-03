/*
 * 接口.44.获取广告详情
 */
function judgFirstTime(IID) {
    
    localStorage.setItem("iid", IID);

    var localPack = localStorage.getItem('localPack');

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Infomation/ViewInfo",
        type : "POST",
        data : {
            IID : IID,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")
            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    debug(data.IsFirstTime);

                    //setInfomationList(data, PageIndex, fromPage);
                    localStorage.setItem("IsFirstTime", data.IsFirstTime);
                    localStorage.setItem("ViewInfo", JSON.stringify(data));
                    //getComment(uid, IID, data, 1, fromPage);
                    if (data.IsFirstTime == 1) {

                        if (localPack == '1' || localPack == 1) {

                            appcan.window.open({
                                name : 'cvr',
                                data : '../biz/cvr.html',
                                aniId : 10,
                            });

                        } else {

                            uexWindow.open({
                                name : 'cvr',
                                data : '../biz/cvr.html',
                                animID : 10,
                                flag : 1024
                            });

                        }

                    } else if (data.IsFirstTime == 0) {

                        if (localPack == '1' || localPack == 1) {

                            appcan.window.open({
                                name : 'adv',
                                data : '../biz/adv.html',
                                aniId : 10,
                            });

                        } else {

                            uexWindow.open({
                                name : 'adv',
                                data : '../biz/adv.html',
                                animID : 10,
                                flag : 1024
                            });

                        }

                    }

                } else {

                    openToast(data.msg, 3000, 5, 0);

                }

            } else {

                openToast('获取商圈数据失败', 3000, 5, 0);

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