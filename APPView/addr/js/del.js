var del_stat = true;
$('.del-addr').click(function() {
    
    var uid = localStorage.getItem("uid");
    var ID = $('#areacode').data('addrid');
    var fromPage = 'addr_edit';
    debug(ID);

    uexWindow.actionSheet({
        title : "",
        cancel : "取消",
        buttons : "删除收货地址"
    }, function(index) {

        if (index == 0) {

            if (del_stat) {
                del_stat = false;
                delUserAddress(uid, ID, fromPage);
            }

        }

    });

});

/*
 * 接口.54.新增收货地址
 */
function delUserAddress(uid, ID, fromPage) {

    openToast('正在删除收货地址', 10000, 5, 1);

    //所有层的id 省市区

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    //登录的逻辑
    appcan.ajax({
        url : ajaxUrl + "Account/DeleteUserAddress",
        type : "POST",
        data : {
            ID : ID,
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
                        delUserAddrSql(db, uid, ID, fromPage);
                    }

                } else {

                    del_stat = true;                   
                    openToast(data.msg, 5000, 5, 0);

                }

            } else {

                del_stat = true;
                openToast('删除收货地址失败', 5000, 5, 0);

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
            del_stat = true;
            openToast(localStorage.getItem("errorMsg"), 5000, 5, 0);

        },
    });

}

/*
 * 接口.18.2.本地删除收藏的逻辑
 */
function delUserAddrSql(db, uid, addrid, fromPage) {

    var sql_tx = [];
    var sql = "delete from tbl_address where uid = '" + uid + "' and addrid = '" + addrid + "'";
    sql_tx.push(sql)
    debug(sql_tx);
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

        if (error == 0) {

            //friendDispList(db, uid, json);
            uexDataBaseMgr.close(db);
            appcan.window.publish('removeAddress', addrid);
            openToast('删除收货地址成功', 3000, 5, 0);
            setTimeout(function() {

                //appcan.window.openToast(msg, 1000000, 5, type);
                appcan.window.close(-1);

            }, 500);

        } else {

            del_stat = true;
            uexDataBaseMgr.close(db);

        }

    });

}