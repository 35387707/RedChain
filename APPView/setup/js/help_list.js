/*
 * 接口.30.获取系统帮助列表
 */
function getHelpList(uid, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "System/GetHelpList",
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
                    //setCollectionList(uid, data, Index, fromPage);
                    setHelpList(uid, data, fromPage);

                } else {
                    openToast(data.msg, 3000, 5, 0);
                }

            } else {

                openToast('获取广告收藏失败', 3000, 5, 0);

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

//接口.30.1.迭代收藏列表数据
function setHelpList(uid, json, fromPage) {

    var fileUrl = localStorage.getItem("fileUrl");

    //debug(json);

    var content = '';
    var lastStr = '';

    if (json.list.length == 0) {

        content += '<div class="end-nope-number">';
        content += '当前没有数据';
        content += '</div>';

        openToast('当前没有数据', 3000, 5, 0);

    } else {

        $(json.list).each(function(k, r) {

            //debug(imgArr);

            content += '<div data-hid="' + r.ID + '" data-title="' + r.Title + '" class="ub ub-ver ub-fv list-view help-sub" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';

            content += '<div class="ub ubb bc-border list-cont-padd c-wh ub-ac">';

            content += '<div class="left-txt-color ulev0 ub-f1">';
            content += r.Title;
            content += '</div>';

            content += '<li>';
            content += '<div class="ub-img li-right-arrow"></div>';
            content += '</li>';

            content += '</div>';
            content += '</div>';

        });

    }

    if (fromPage == 'refresh_help') {
        $("#help_list").children().remove();
        appcan.frame.resetBounce(0);
        openToast('刷新数据成功', 3000, 5, 0);
    } else {
        appcan.frame.resetBounce(1);
        openToast('加载数据成功', 3000, 5, 0);
    }
    $('#help_list').append(content);

    $('.help-sub').unbind();
    $('.help-sub').click(function() {

        var hid = $(this).data('hid');
        var title = $(this).data('title');
        //var cont = $(this).data('content');
        var cont = srchHelpCont(hid, json);

        var helpCont = {
            hid : hid,
            title : title,
            content : cont,
        }
        
        debug(helpCont);

        localStorage.setItem("helpCont", JSON.stringify(helpCont));

        var localPack = localStorage.getItem('localPack');

        if (localPack == '1' || localPack == 1) {

            appcan.window.open({
                name : 'help_detail',
                data : '../setup/help_detail.html',
                aniId : 10,
            });

        } else {

            uexWindow.open({
                name : "help_detail",
                data : "../setup/help_detail.html",
                animID : 10,
                flag : 1024
            });

        }

    });

}

function srchHelpCont(hid, json) {
    
    var cont = '';
    
    $(json.list).each(function(i,v){
        
        if (v.ID == hid) {
            
            cont = v.Content
            
        }
        
    });
    
    return cont;
    
}

//接口.30.2.设置帮助详情
function setHelpCont(uid, fromPage) {

    var helpCont = JSON.parse(localStorage.getItem("helpCont"));
    var title = helpCont.title;
    var content = helpCont.content;

    $('#title').html(title);
    $('#content').html(content);

}

