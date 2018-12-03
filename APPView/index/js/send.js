//接口20.获取发红包的可选金额选项
function getRedPrice() {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    //登录的逻辑
    appcan.ajax({
        url : ajaxUrl + "RedPacket/GetRedPrice",
        type : "POST",
        data : {
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            //debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    if (data.list.length == 0 || data.list == '') {

                        closeToast();
                        openToast('当前没有可获取的红包金额...', 3000, 5, 0);

                    } else {

                        localStorage.setItem("RedPrice", JSON.stringify(data));

                        var localPack = localStorage.getItem('localPack');
                        if (localPack == '1' || localPack == 1) {

                            appcan.window.open({
                                name : 'hbeditor',
                                data : '../hb/hbeditor.html',
                                aniId : 10,
                            });

                        } else {

                            uexWindow.open({
                                name : "hbeditor",
                                data : "../hb/hbeditor.html",
                                animID : 10,
                                flag : 1024
                            });

                        }

                    }

                } else {

                    openToast('获取红包金额失败', 3000, 5, 0);

                }

            } else {

                openToast('获取红包金额失败', 3000, 5, 0);

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
            openToast('未知错误', 3000, 5, 0);

        },
    });

}

//填充select数据
function dispSelData() {

    var redStr = '';

    var RedPrice = JSON.parse(localStorage.getItem("RedPrice"));
    $(RedPrice.list).each(function(j, s) {

        //$("#redmoney").append("<option value=" + s + ">" + s + "</option>");
        redStr += '<div class="money-width">';
        redStr += '<div class="money-inside">';
        redStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="money-style money-sub" data-money="' + s + '">';
        redStr += s;
        redStr += '</div>';
        redStr += '</div>';
        redStr += '</div>';

    });

    $("#redmoney").append(redStr);

    var tempMoney = localStorage.getItem("tempMoney");
    $("div[data-money='" + tempMoney + "']").addClass('money-choose');

    $('.money-sub').unbind();
    $('.money-sub').click(function() {

        $(".money-choose").removeClass('money-choose');
        $(this).addClass('money-choose');

        localStorage.setItem("tempMoney", $(this).text());

        localStorage.setItem("redmoney", $(this).text());

        appcan.window.publish('refreshMoney', $(this).text());

        appcan.window.closePopover('money');
        localStorage.setItem("money", 0);

    });

}