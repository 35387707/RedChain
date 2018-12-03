/*
 * 接口.22.关注用户
 */
function followUser(uid, user_uid, fromPage) {

    openToast("正在关注当前用户", 60000, 5, 1);

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "MyCollection/FollowUser",
        type : "POST",
        data : {
            uid : user_uid,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    isrtFollowSgl(uid, data.msg, user_uid, fromPage);

                } else {

                    openToast(data.msg, 3000, 5, 0);

                }

            } else {

                openToast('关注用户失败', 3000, 5, 0);

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

//22.0.关注成功后保存记录保存本地数据库
function isrtFollowSgl(uid, cid, user_uid, fromPage) {
    
    var sql_tx = [];

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {

        var sql = "insert into tbl_follow (uid, cid, user_uid) values ('" + uid + "', '" + cid + "', '" + user_uid + "')";
        sql_tx.push(sql);

        uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

            if (error == 0) {

                uexDataBaseMgr.close(db);
                setFollowTxt(cid, user_uid, fromPage);

            } else {

                uexDataBaseMgr.close(db);
                openToast('点赞失败', 3000, 5, 0);

            }

        });
    }

}

//22.1.关注成功后改变显示的文字
function setFollowTxt(cid, friend_uid, fromPage) {

    if (fromPage == 'follow_mine' || fromPage == 'follow_fans') {
        appcan.window.publish('refreshFollow', 1);
    }
    openToast('关注用户成功', 3000, 5, 0);
    $("div[data-follow_userid='" + friend_uid + "']").text('已关注');
    $("div[data-follow_userid='" + friend_uid + "']").data('follow', 1);
    $("div[data-follow_userid='" + friend_uid + "']").data('cid', cid);

}

//22.2.取消关注的逻辑
//接口.18.1.删除收藏
function notFollowSql(uid, cid, user_uid, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "MyCollection/Delete",
        type : "POST",
        data : {
            Id : cid,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    var db = uexDataBaseMgr.open(uid + ".db");
                    if (db != null) {
                        delFollowSql(db, uid, cid, user_uid, fromPage);
                    }

                } else {

                    openToast('取消关注失败', 3000, 5, 0);

                }

            } else {

                openToast('取消关注失败', 3000, 5, 0);

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
 * 接口.18.2.本地删除收藏的逻辑
 */
function delFollowSql(db, uid, cid, friend_uid, fromPage) {

    var sql_tx = [];
    var sql = "delete from tbl_follow where uid = '" + uid + "' and cid = '" + cid + "'";
    sql_tx.push(sql)
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

        if (error == 0) {

            //friendDispList(db, uid, json);
            uexDataBaseMgr.close(db);
            if (fromPage == 'follow_mine' || fromPage == 'follow_fans') {
                appcan.window.publish('removeFollow', cid);
            }
            $("div[data-follow_userid='" + friend_uid + "']").text('关注');
            $("div[data-follow_userid='" + friend_uid + "']").data('follow', 0);
            openToast('取消关注成功', 3000, 5, 0);

        } else {

            uexDataBaseMgr.close(db);
            openToast('取消关注失败', 3000, 5, 0);

        }

    });

}