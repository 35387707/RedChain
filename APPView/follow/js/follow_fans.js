//接口.17.0.获取收藏列表
//Mtype    收藏类型    必填，0为红包收藏，1为用户收藏，2为消息收藏
function getFansList(uid, pageindex, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "MyCollection/GetFansList",
        type : "POST",
        data : {
            pageindex : pageindex,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    setFansList(uid, data, pageindex, fromPage);

                } else {
                    
                    openToast('获取关注列表失败', 3000, 5, 0);
                    
                }

            } else {

                openToast('获取关注列表失败', 3000, 5, 0);

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

//接口.17.1.迭代收藏列表数据
function setFansList(uid, json, pageindex, fromPage) {

    var fileUrl = localStorage.getItem("fileUrl");

    //debug(json);

    var content = '';
    var lastStr = '';

    //localStorage.setItem("pagecount", json.pagecount);
    
    $('#follow_fans').data('pageindex', pageindex);
    $('#follow_fans').data('pagecount', json.pagecount);

    if (pageindex == json.pagecount) {

        lastStr += '<div class="end-nope-number">';
        lastStr += '没有更多了';
        lastStr += '</div>';

    }

    if (json.list.length == 0) {

        content += '<div class="end-nope-number">';
        content += '当前没有数据';
        content += '</div>';

        openToast('当前没有数据', 3000, 5, 0);

    } else {

        $(json.list).each(function(k, r) {

            //debug(imgArr);

            content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub ub-f1 ubb list-style follow-sub" data-userid="' + r.UID + '" data-cid="' + r.ID + '">';

            content += '<div class="ub icon-style">';
            content += '<div class="ub-img1 icon-width" style="background-image:url(' + fileUrl + r.HeadImg1 + ');"></div>';
            content += '</div>';

            content += '<div class="icon-marg-right"></div>';

            content += '<div class="ub-f1 user-padd">';
            content += '<div class="nick-padd ub">';
            content += ' <div class="user-nick">';
            if (r.TrueName == '' || r.TrueName == null || r.TrueName == undefined) {
                content += r.Phone;
            } else {
                content += r.TrueName;
            }
            content += '</div>';
            //content += '<div class="user-stat">';
            //content += '<a class="stat-sty">推荐人 </a>';
            //content += '</div>';
            content += '</div>';

            content += '<div class="user-note overflow">';
            if (r.Descrition == '' || r.Descrition == null || r.Descrition == undefined) {
                content += '这家伙很懒, 暂时还没有签名...';
            } else {
                content += r.Descrition;
            }
            content += '</div>';

            content += '</div>';

            content += '<div class="arrow-style">';
            content += '<div class="ub-img right-arrow"></div>';
            content += '</div>';
            content += '</div>';

        });

        content += lastStr;

        openToast('加载数据成功', 3000, 5, 0);

    }
    
    if (fromPage == 'refresh_follow_fans') {

        $("#follow_fans").children().remove();
        appcan.frame.resetBounce(0);
        openToast('刷新数据成功', 3000, 5, 0);

    } else {

        appcan.frame.resetBounce(1);
        openToast('加载数据成功', 3000, 5, 0);

    }
    
    $('#follow_fans').append(content);

    $('.follow-sub').unbind();
    $('.follow-sub').click(function() {

        var userid = $(this).data('userid');
        localStorage.setItem("userid", userid);
        
        localStorage.setItem("fromPage", 'follow_fans');
        
        var name = new Date().format("yyyyMMddhhmmssSS");

        var localPack = localStorage.getItem('localPack');
        if (localPack == '1' || localPack == 1) {

            appcan.window.open({
                name : name,
                data : '../other/page_other.html',
                aniId : 10,
            });

        } else {

            uexWindow.open({
                name : name,
                data : '../other/page_other.html',
                animID : 10,
                flag : 1024
            });

        }

    });

}

//接口.17.2.判断当前的页码为多少
//pagecount
function judgCurPage(uid, item, fromPage) {

    var curpage = '';
    //$('.photo-album').children("div:last-child").index()
    var pagecount = parseInt($('#follow_fans').data('pagecount'));
    var pageindex = parseInt($('#follow_fans').data('pageindex'));

    if (fromPage == 'follow_mine') {
        curpage = $('#follow_mine').children("div:last-child").index() + 1;
    } else if (fromPage == 'follow_fans') {
        curpage = $('#follow_fans').children("div:last-child").index() + 1;
    }

    /*
     debug('curpage: ' + curpage);
     debug('parseInt(curpage): ' + parseInt(curpage));
     debug('pagecount: ' + pagecount);*/

    if (curpage == 1) {

        appcan.frame.resetBounce(1);
        openToast('当前没有数据', 3000, 5, 0);

    } else {

        if (curpage < item) {

            appcan.frame.resetBounce(1);
            openToast('所有的数据已经加载完成', 3000, 5, 0);

        } else {

            if (pageindex == pagecount) {

                appcan.frame.resetBounce(1);
                openToast('所有的数据已经加载完成', 3000, 5, 0);

            } else {

                if (pageindex < pagecount) {

                    getFansList(uid, pageindex + 1, fromPage);

                }

            }

        }

    }

}
