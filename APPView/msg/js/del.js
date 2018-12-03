//3.0.isrtMsgBegin
function delMsgLogBegin() {
    
    var uid = localStorage.getItem("uid");
    var friend_uid = localStorage.getItem("friend_uid");

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {
        delMsgLogActn(db, uid, friend_uid, []);
    }

}

//3.1.查询出最后一条的聊天记录
function delMsgLogActn(db, uid, friend_uid, sql_tx) {

    var sql_del = "delete ";

    sql_del += "from tbl_msg ";

    sql_del += "where ";
    
    sql_del += "uid = '" + uid + "' ";
    sql_del += "and friend_uid = '" + friend_uid + "' ";

    sql_tx.push(sql_del);

    debug(sql_tx);
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

        if (error == 0) {
            
            uexDataBaseMgr.close(db);
            appcan.window.publish('removeMsgLog', friend_uid);
            $('#content').children().remove();
            openToast('聊天记录已清空', 3000, 5, 0);

        } else {

            uexDataBaseMgr.close(db);
            openToast('清空聊天记录失败', 3000, 5, 0);

        }

    });

}

