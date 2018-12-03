/*
 * 发送私聊之前查询
 */
function isFriendSrch() {

    var json = {
        friend : 0,
        follow : 0,
        cid : '',
    }

    var uid = localStorage.getItem("uid");
    var friend_uid = localStorage.getItem("userid");
    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {
        var sql_sel = "select  * from tbl_friend where uid = '" + uid + "' and friend_uid = '" + friend_uid + "'";
        uexDataBaseMgr.select(db, sql_sel, function(error, data) {
            if (!error) {

                //isFriendJudg(db, friend_uid, data);
                if (data.length != 0) {
                    json.friend = data[0].friend_status_id;
                }
                isFollowSrch(db, uid, friend_uid, json, data);

            } else {

                uexDataBaseMgr.close(db);
                openToast('查询私聊信息失败', 3000, 5, 0);

            }

        });

    }

}

/*
 * 查询判断是否已关注
 */
function isFollowSrch(db, uid, friend_uid, json, friend_json) {

    var sql_sel = "select  * from tbl_follow where uid = '" + uid + "' and user_uid = '" + friend_uid + "'";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            //isFriendJudg(db, friend_uid, data);
            if (data.length != 0) {
                json.follow = 1;
                json.cid = data[0].cid;
            }
            isCondJudg(db, uid, friend_uid, json, friend_json);

        } else {

            uexDataBaseMgr.close(db);
            openToast('查询关注信息失败', 3000, 5, 0);

        }

    });

}

/*
 * 发送私聊之前判断
 */
function isCondJudg(db, uid, friend_uid, json, friend_json) {

    debug(json);
    debug(friend_json);

    uexDataBaseMgr.close(db);

    if (json.friend == 19 || json.friend == '19') {

        $("div[data-chat_userid='" + friend_uid + "']").text('未领取');

    } else if (json.friend == 1 || json.friend == '1') {

        localStorage.setItem("friend_uid", friend_uid);
        localStorage.setItem("friend_name", friend_json[0].true_name);
        localStorage.setItem("friend_img", friend_json[0].head_img);
        localStorage.setItem("friend_phone", friend_json[0].phone);

    }
    
    if (json.follow == 1 || json.follow == '1') {
        $("div[data-follow_userid='" + friend_uid + "']").text('已关注');
    }
    
    //$('#chat').data('chat', json.friend);
    //$('#follow').data('follow', json.follow);
    //$('#follow').data('cid', json.cid);
    
    $("div[data-chat_userid='" + friend_uid + "']").data('chat', json.friend);
    $("div[data-follow_userid='" + friend_uid + "']").data('follow', json.follow);
    $("div[data-follow_userid='" + friend_uid + "']").data('cid', json.cid);

}