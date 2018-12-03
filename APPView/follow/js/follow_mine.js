//接口.17.0.获取收藏列表
//Mtype    收藏类型    必填，0为红包收藏，1为用户收藏，2为消息收藏
function getFollowList(uid, Index, Mtype, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "MyCollection/GetList",
        type : "POST",
        data : {
            Index : Index,
            Mtype : Mtype,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    //setCollectionList(uid, data, Index, fromPage);
                    var db = uexDataBaseMgr.open(uid + ".db");
                    if (db != null) {
                        followSrchOne(db, uid, 0, data.list.length, data, [], Index, fromPage);
                    }

                } else {
                    
                    openToast(data.msg, 3000, 5, 0);
                    
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

//接口.17.0.收藏列表保存到本地
function followSrchOne(db, uid, i, length, json, sql_tx, PageIndex, fromPage) {

    if (i < length) {

        var sql_sel = "select  * from tbl_follow where uid = '" + uid + "' and user_uid = '" + json.list[i].UID + "'";
        uexDataBaseMgr.select(db, sql_sel, function(error, data) {
            if (!error) {

                var sql = "";
                if (data.length == 0) {
                    sql = "insert into tbl_follow (uid, cid, user_uid) values ('" + uid + "', '" + json.list[i].ID + "', '" + json.list[i].UID + "')";
                } else {
                    sql = "update tbl_follow set cid='" + json.list[i].ID + "' where uid = '" + uid + "' and user_uid = '" + json.list[i].UID + "'";
                }
                sql_tx.push(sql);

                followSrchOne(db, uid, i + 1, length, json, sql_tx, PageIndex, fromPage);

            } else {

                uexDataBaseMgr.close(db);
                openToast('获取好友列表失败', 3000, 5, 0);

            }

        });

    } else {

        followTxActn(db, uid, json, sql_tx, PageIndex, fromPage);

    }

}

//接口.17.1.事务处理
function followTxActn(db, uid, json, sql_tx, PageIndex, fromPage) {

    debug(sql_tx);
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

        if (error == 0) {

            //friendDispList(db, uid, json);
            uexDataBaseMgr.close(db);
            //getPacketList(fromPage);
            setFollowList(uid, json, PageIndex, fromPage);

        } else {

            uexDataBaseMgr.close(db);
            openToast('获取好友列表失败', 3000, 5, 0);

        }

    });

}

//接口.17.1.迭代收藏列表数据
function setFollowList(uid, json, PageIndex, fromPage) {

    var fileUrl = localStorage.getItem("fileUrl");

    //debug(json);

    var content = '';
    var lastStr = '';

    $('#follow_mine').data('pageindex', PageIndex);
    $('#follow_mine').data('pagecount', json.pagecount);

    if (PageIndex == json.pagecount) {

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

    }

    if (fromPage == 'refresh_follow_mine') {

        $("#follow_mine").children().remove();
        appcan.frame.resetBounce(0);
        openToast('刷新数据成功', 3000, 5, 0);

    } else {

        appcan.frame.resetBounce(1);
        openToast('加载数据成功', 3000, 5, 0);

    }
    
    $('#follow_mine').append(content);

    $('.follow-sub').unbind();
    $('.follow-sub').click(function() {

        var userid = $(this).data('userid');
        localStorage.setItem("userid", userid);
        
        localStorage.setItem("fromPage", 'follow_mine');
        
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
function judgCurPage(uid, item, Mtype, fromPage) {

    var curpage = '';
    //$('.photo-album').children("div:last-child").index()
    var pagecount = parseInt($('#follow_mine').data('pagecount'));
    var pageindex = parseInt($('#follow_mine').data('pageindex'));

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

                    getFollowList(uid, pageindex + 1, Mtype, fromPage);

                }

            }

        }

    }

}
