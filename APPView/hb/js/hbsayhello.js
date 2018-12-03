$('#say_hello').click(function() {
    
    openToast("开始打招呼了...", 3000, 5, 1);

    var TrueName = '';
    var UserInfo = JSON.parse(localStorage.getItem("UserInfo"));
    if (UserInfo.Info.TrueName == '' || UserInfo.Info.TrueName == null || UserInfo.Info.TrueName == undefined) {
        TrueName = UserInfo.Info.Phone;
    } else {
        TrueName = UserInfo.Info.TrueName;
    }

    var price = $('.fu-money').text().replace(/\s*/g, "");

    var content = {
        sign : '打招呼',
        name : TrueName,
        price : price,
        content : '我是' + TrueName,
    }
    
    debug(content);
    
    queryMessages('sayHello', 1, localStorage.getItem("userid"), '', 19, 0, content, 'say_hello');

});

//插入sqlite的逻辑
function sayHelloSqlSrch(uid, OtherUser, friend_status_id, mid) {
    
    debug(OtherUser);

    var sql_tx = [];

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {

        var sql_sel = "select  * from tbl_friend where uid = '" + uid + "' and friend_uid = '" + OtherUser.Info.ID + "'";
        uexDataBaseMgr.select(db, sql_sel, function(error, data) {
            if (!error) {

                var sql = "";
                if (data.length == 0) {
                    sql = "insert into tbl_friend (uid, friend_uid, friend_status_id, true_name, name, head_img, phone) values ('" + uid + "', '" + OtherUser.Info.ID + "', " + friend_status_id + ",'" + OtherUser.Info.TrueName + "', '', '" + OtherUser.Info.HeadImg + "', '" + OtherUser.Info.Phone + "')";
                } else {
                    sql = "update tbl_friend set friend_status_id=" + friend_status_id + ", true_name='" + OtherUser.Info.TrueName + "', head_img='" + OtherUser.Info.HeadImg + "', phone='" + OtherUser.Info.Phone + "' where uid = '" + uid + "' and friend_uid = '" + OtherUser.Info.ID + "'";
                }
                sql_tx.push(sql);

                sayHelloSqlTx(db, uid, OtherUser, friend_status_id, sql_tx, mid);

            } else {

                uexDataBaseMgr.close(db);
                debug('查询朋友信息出错');

            }

        });

    }

}

//事务处理
function sayHelloSqlTx(db, uid, OtherUser, friend_status_id, sql_tx, mid) {

    debug(sql_tx);
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

        if (error == 0) {

            msgSaySaveTmp(db, uid, OtherUser.Info.ID, uid, 1, 1, 19, '福包敲门中', mid);
            //msgSaySaveTmp(db, uid, friend_uid, said_uid, msg_type_id, msg_status_id, msg_cont_type_id, msg_cont, mid)

        } else {

            openToast("保存临时消息成功", 5000, 5, 0);
            uexDataBaseMgr.close(db);

        }

    });

}
