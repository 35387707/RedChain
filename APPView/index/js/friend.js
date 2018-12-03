//接口.24.获取好友列表
function friendAjaxList(uid, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Friend/GetFriendList",
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

                    localStorage.setItem("maxcount", data.maxcount);

                    var db = uexDataBaseMgr.open(uid + ".db");
                    if (db != null) {
                        friendSrchOne(db, uid, 0, data.list.length, data, 1, [], fromPage);
                    }

                } else {

                    debug(data.msg);
                    if (fromPage == 'login') {
                        hideLoading('登录失败', 5000, 5, 0);
                    } else {
                        hideLoading(data.msg, 3000, 5, 0);
                    }

                }

            } else {

                debug('获取好友列表失败');
                if (fromPage == 'login') {
                    hideLoading('登录失败', 5000, 5, 0);
                } else {
                    hideLoading('获取好友列表失败', 3000, 5, 0);
                }

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
            if (fromPage == 'login') {
                hideLoading('登录失败', 5000, 5, 0);
            } else {
                hideLoading('获取好友列表失败', 3000, 5, 0);
            }

        },
    });

}

/*
 * friend.0.单个查询的方法
 */
function friendSrchOne(db, uid, i, length, json, friend_status_id, sql_tx, fromPage) {

    if (i < length) {

        var sql_sel = "select  * from tbl_friend where uid = '" + uid + "' and friend_uid = '" + json.list[i].ID + "'";
        uexDataBaseMgr.select(db, sql_sel, function(error, data) {
            if (!error) {

                var sql = "";
                if (data.length == 0) {
                    sql = "insert into tbl_friend (uid, friend_uid, friend_status_id, true_name, descrition, head_img, phone) values ('" + uid + "', '" + json.list[i].ID + "', " + friend_status_id + ",'" + json.list[i].Remark + "', '', '" + json.list[i].headimg + "', '" + json.list[i].Phone + "')";
                } else {
                    sql = "update tbl_friend set friend_status_id=" + friend_status_id + ", true_name='" + json.list[i].Remark + "', head_img='" + json.list[i].headimg + "', phone='" + json.list[i].Phone + "' where uid = '" + uid + "' and friend_uid = '" + json.list[i].ID + "'";
                }
                sql_tx.push(sql);

                friendSrchOne(db, uid, i + 1, length, json, friend_status_id, sql_tx, fromPage);

            } else {

                uexDataBaseMgr.close(db);
                debug('friendSrchOne出错');
                if (fromPage == 'login') {
                    hideLoading('登录失败', 5000, 5, 0);
                } else {
                    hideLoading('获取好友列表失败', 3000, 5, 0);
                }

            }

        });

    } else {

        friendTxActn(db, uid, json, sql_tx, fromPage);

    }

}

/*
 * friend.1.
 */
function friendTxActn(db, uid, json, sql_tx, fromPage) {

    debug(sql_tx);
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

        if (error == 0) {

            //friendDispList(db, uid, json);
            if (fromPage == 'login') {

                uexDataBaseMgr.close(db);
                //getPacketList(fromPage);
                //undoMessageCount(uid, fromPage);
                thingsToSimgple();

            } else {

                friendSrchLocal(db, uid, fromPage);

            }

        } else {

            uexDataBaseMgr.close(db);
            debug('friendTxActn出错');
            if (fromPage == 'login') {
                hideLoading('登录失败', 5000, 5, 0);
            } else {
                hideLoading('获取好友列表失败', 3000, 5, 0);
            }

        }

    });

}

/*
 * friend.1.本地开始的查找朋友的逻辑
 */
function friendSrchBegin(uid, fromPage) {

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {
        friendSrchLocal(db, uid, fromPage);
    }

}

/*
 * friend.3.查找sqlite的好友
 */
function friendSrchLocal(db, uid, fromPage) {

    var sql_sel = "select  * from tbl_friend where uid = '" + uid + "' and friend_status_id = 1";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            debug(data);
            friendDispList(db, uid, data, fromPage);

        } else {

            uexDataBaseMgr.close(db);
            debug('friendSrchLocal出错');
            cnctAgain();

        }

    });

}

//接口.24.0.迭代好友列表
function friendDispList(db, uid, json, fromPage) {

    var fileUrl = localStorage.getItem("fileUrl");

    var friend_list = '';
    var makecount = 0;
    if (json.length != 0) {

        $(json).each(function(j, s) {

            if (s.true_name == '' || s.true_name == null || s.true_name == undefined) {
                friend_list += '<ul ontouchstart="appcan.touch(&#39;btn-act&#39;)" data-friend_uid="' + s.friend_uid + '" data-friend_name="' + s.phone + '" data-friend_phone="' + s.phone + '" data-friend_img="' + s.head_img + '" class="ub ub-f1 friend-padd friend-sub">';
            } else {
                friend_list += '<ul ontouchstart="appcan.touch(&#39;btn-act&#39;)" data-friend_uid="' + s.friend_uid + '" data-friend_name="' + s.true_name + '" data-friend_phone="' + s.phone + '" data-friend_img="' + s.head_img + '" class="ub ub-f1 friend-padd friend-sub">';
            }

            friend_list += '<li class="ub">';
            friend_list += '<div class="ub-img1 friend-width" style="background-image:url(' + fileUrl + s.head_img + ');"></div>';
            friend_list += '</li>';

            friend_list += '<li class="ub ub-f1">';
            friend_list += '<div class="friend-info">';
            if (s.true_name == '' || s.true_name == null || s.true_name == undefined) {
                friend_list += s.phone;
            } else {
                friend_list += s.true_name;
            }
            friend_list += '</div>';
            friend_list += '</li>';

            friend_list += '</ul>';

            friend_list += '<div class="defn-line"></div>';

        });

        makecount = json.length;

    }

    debug(friend_list);

    var maxcount = localStorage.getItem("maxcount");
    $('#maxcount').text(maxcount);
    $('#makecount').text(makecount);

    $('#friend_list').children().remove();
    $('#friend_list').append(friend_list);

    $('#friend_list').data('isempty', '1');

    $('.friend-sub').unbind();
    $('.friend-sub').click(function() {

        var indx = $(this).data('friend_uid');
        if ($("div[data-unread_num='" + indx + "']").text()) {
            var num = $("div[data-unread_num='" + indx + "']").text().replace(/\s*/g, "");
            debug(num);
            appcan.window.publish('refreshMsgNum', num);

            $("div[data-unread_num='" + indx + "']").addClass('no-dis');
            $("div[data-unread_num='" + indx + "']").text('');
        } else {
            debug('第一次打开的东西');
        }

        localStorage.setItem("friend_uid", $(this).data('friend_uid'));
        localStorage.setItem("friend_name", $(this).data('friend_name'));
        localStorage.setItem("friend_img", $(this).data('friend_img'));
        localStorage.setItem("friend_phone", $(this).data('friend_phone'));

        var localPack = localStorage.getItem('localPack');
        if (localPack == '1' || localPack == 1) {

            appcan.window.open({
                name : 'chat_list_local',
                data : '../msg/chat_list_local.html',
                aniId : 10,
            });

        } else {

            uexWindow.open({
                name : "chat_list",
                data : "../msg/chat_list.html",
                animID : 10,
                flag : 1024
            });

        }

    });

    //msg.0.执行msg获取未读记录的入口
    msgSrchGroup(db, uid, 1, 2, fromPage);

}
