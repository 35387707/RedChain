var send_stat = true;
$('#save').click(function() {

    if (send_stat) {
        send_stat = false;
        judgReqFld('updt');
    }

});

//判断必填的条件
function judgReqFld(fromPage) {

    var req_arr = [];
    var recname = trim($('#recname').val());
    if (recname == '') {
        req_arr.push('姓名');
    }
    var areid = $('#areacode').data('areid');
    if (areid == '' || areid == null || areid == undefined) {

    }
    var phone = trim($('#phone').val());
    if (phone == '') {
        req_arr.push('手机号码');
    }
    var areacode = $('#areacode').data('areacode');
    if (areacode == '' || areacode == null || areacode == undefined) {
        req_arr.push('所在地区');
    }
    //区域id, 也就是第三层的id
    var address = trim($('#address').val());
    if (address == '') {
        req_arr.push('详细地址');
    }

    if (req_arr.length == 0 || req_arr == '') {

        //judgImgUpld();
        openToast('恭喜发财', 3000, 5, 0);
        if (fromPage == 'add') {
            addUserAddress(fromPage);
        } else if (fromPage == 'updt') {
            updateUserAddress(fromPage);
        }

    } else {

        send_stat = true;
        openToast('请输入' + req_arr.join('、'), 3000, 5, 0);

    }

}

/*
 * 接口.55.新增收货地址
 */
function updateUserAddress(fromPage) {

    var recname = trim($('#recname').val());
    var areid = $('#areacode').data('areid');
    //区域id, 也就是第三层的id
    var address = trim($('#address').val());
    var phone = trim($('#phone').val());
    //var areacode = $('#areacode').data('areacode');
    var areacode = $('#areacode').data('areacode');
    //var areacode = new Array();
    //areacode = temp.split(";");
    //所有层的id 省市区
    var ID = $('#areacode').data('addrid');

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    //登录的逻辑
    appcan.ajax({
        url : ajaxUrl + "Account/UpdateUserAddress",
        type : "POST",
        data : {
            recname : recname,
            areid : areid,
            address : address,
            phone : phone,
            areacode : areacode.toString(),
            ID : ID,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    openToast('修改收货地址成功', 5000, 5, 0);
                    updtAddrSql(localStorage.getItem("uid"), ID, fromPage);

                } else {

                    send_stat = true;
                    openToast(data.msg, 5000, 5, 0);

                }

            } else {

                send_stat = true;
                openToast('修改收货地址失败', 5000, 5, 0);

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
            send_stat = true;
            openToast(localStorage.getItem("errorMsg"), 5000, 5, 0);

        },
    });

}

/*
 * 保存本地sqlite的方法
 */
function updtAddrSql(uid, addrid, fromPage) {

    var sql_tx = [];

    var recname = trim($('#recname').val());
    var areid = $('#areacode').data('areid');
    //区域id, 也就是第三层的id
    var address = trim($('#address').val());
    var phone = trim($('#phone').val());
    var areacode = $('#areacode').data('areacode');
    var areaname = trim($('#areacode').val());
    var indexArr = localStorage.getItem("indexArr");

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {

        var sql_sel = "select  * from tbl_address where uid = '" + uid + "' and addrid = '" + addrid + "'";
        debug(sql_sel);
        uexDataBaseMgr.select(db, sql_sel, function(error, data) {
            if (!error) {

                var sql = "";
                if (data.length == 0) {
                    sql = "insert into tbl_address (uid, addrid, recname, phone, areid, areacode, areaname, indexArr, address) values ('" + uid + "', '" + addrid + "', '" + recname + "','" + phone + "', '" + areid + "', '" + areacode + "', '" + areaname + "', '" + indexArr + "', '" + address + "')";
                } else {
                    sql = "update tbl_address set recname='" + recname + "', phone='" + phone + "', areid='" + areid + "', areacode='" + areacode + "', areaname='" + areaname + "', indexArr='" + indexArr + "', address='" + address + "' where uid = '" + uid + "' and addrid = '" + addrid + "'";
                }
                sql_tx.push(sql);

                updtAddrTx(db, uid, addrid, sql_tx, fromPage);

            } else {

                send_stat = true;
                uexDataBaseMgr.close(db);

            }

        });

    }

}

function updtAddrTx(db, uid, addrid, sql_tx, fromPage) {

    debug(sql_tx);
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

        if (error == 0) {

            delDefAddr(db, uid, addrid, [], fromPage);

        } else {

            send_stat = true;
            uexDataBaseMgr.close(db);

        }

    });

}

/*
 * 设置默认的逻辑
 */
function delDefAddr(db, uid, addrid, sql_tx, fromPage) {

    var temp = $('.switch').data('checked');

    debug(temp);

    if (temp == false || temp == 'false') {

        setDefAddr(db, uid, addrid, 0, [], fromPage);

    } else {

        var sql = "update tbl_address set defaultAddr=0 where uid = '" + uid + "'";
        sql_tx.push(sql);

        debug(sql_tx);
        uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

            if (error == 0) {

                setDefAddr(db, uid, addrid, 1, [], fromPage);

            } else {

                send_stat = true;
                uexDataBaseMgr.close(db);

            }

        });

    }

}

function setDefAddr(db, uid, addrid, defaultAddr, sql_tx, fromPage) {

    var sql = "update tbl_address set defaultAddr=" + defaultAddr + " where uid = '" + uid + "' and addrid = '" + addrid + "'";
    sql_tx.push(sql);

    debug(sql_tx);
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

        if (error == 0) {

            //friendDispList(db, uid, json);
            send_stat = true;
            uexDataBaseMgr.close(db);

            appcan.window.publish('refreshAddr', 1);

            if (fromPage == 'add') {

                openToast('新增收货地址成功', 5000, 5, 0);
                setTimeout(function() {
                    appcan.window.close(-1);
                }, 1500);

            } else if (fromPage == 'updt') {

                openToast('修改收货地址成功', 5000, 5, 0);
                setTimeout(function() {
                    appcan.window.close(-1);
                }, 1500);

            }

        } else {

            send_stat = true;
            uexDataBaseMgr.close(db);

        }

    });

}