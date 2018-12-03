/*
 * 接口.41.获取广告/商圈类型
 */
function getInfomationTypes() {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Infomation/GetInfomationTypes",
        type : "POST",
        data : {
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")
            
            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {
                    
                    localStorage.setItem("tabJson", JSON.stringify(data));
                    setInfomationTypes(data);

                } else {
                    openToast(data.msg, 3000, 5, 0);
                }

            } else {

                openToast('获取广告/商圈类型失败', 3000, 5, 0);

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
 * 接口.41.0.设置商圈类型的数据
 */
function setInfomationTypes(json) {
    
    var tabArr = [];
    var tabStr = '';
    var bizUrl = [];

    $(json.list).each(function(i, v) {

        localStorage.setItem("sub_" + i, v.ITID);
        
        var sglUrl = {
            inPageName : "sub_" + i,
            inUrl : "sub_" + i + ".html",
        }
        bizUrl.push(sglUrl);
        
        tabArr.push(v.Name);
        
        if (i == 0) {
            
            tabStr += '<div data-itid="' + v.ITID + '" data-name="' + v.Name + '" data-desr="' + v.Desr + '" data-stat="' + v.Status + '" ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="swiper-slide nav-sub active">';
            tabStr += v.Name;
            tabStr += '</div>';
            
        } else {
            
            tabStr += '<div data-itid="' + v.ITID + '" data-name="' + v.Name + '" data-desr="' + v.Desr + '" data-stat="' + v.Status + '" ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="swiper-slide nav-sub">';
            tabStr += v.Name;
            tabStr += '</div>';
            
        }
         

    });
    
    debug(tabStr);
    
    localStorage.setItem("bizUrl", JSON.stringify(bizUrl));
    
    localStorage.setItem("tabStr", tabStr);
    
    localStorage.setItem("tabArr", tabArr.toString());
    
    debug(localStorage.getItem("tabArr"));

    var localPack = localStorage.getItem('localPack');
    if (localPack == '1' || localPack == 1) {

        appcan.window.open({
            name : 'biz',
            data : '../biz/biz.html',
            aniId : 10,
        });

    } else {

        uexWindow.open({
            name : 'biz',
            data : '../biz/biz.html',
            animID : 10,
            flag : 1024
        });

    }

}