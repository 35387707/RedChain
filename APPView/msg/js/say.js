//接口.37.获取打招呼列表(分页返回, 每页20条);
function getSayHelloList(uid, PageIndex, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Friend/GetSayHelloList",
        type : "POST",
        data : {
            PageIndex : PageIndex,
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
                        saySrchSgl(db, uid, 0, data.list.length, data, fromPage, []);
                    }

                } else {

                    openToast(data.msg, 3000, 5, 0);

                }

            } else {

                openToast('获取好友申请失败', 3000, 5, 0);

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

//接口.37.存储本地sqlite方法
function saySrchSgl(db, uid, i, length, json, fromPage, sql_tx) {

    if (i < length) {

        var sql_sel = "select  * from tbl_say_hello where uid = '" + uid + "' and mid = '" + json.list[i].MID + "'";
        uexDataBaseMgr.select(db, sql_sel, function(error, data) {
            if (!error) {

                var sql = "";
                if (data.length == 0) {
                    sql = "insert into tbl_say_hello (uid, mid, friend_uid, name, phone, headimg, content, status, last_date) values ('" + uid + "', '" + json.list[i].MID + "', '" + json.list[i].UID + "', '" + json.list[i].Name + "', '" + json.list[i].Phone + "', '" + json.list[i].HeadImg + "', '" + json.list[i].Content + "', 3, '" + timeStampFormat(json.list[i].CreateTime) + "')";
                } else {
                    sql = "update tbl_say_hello set friend_uid='" + json.list[i].UID + "', name='" + json.list[i].Name + "', phone='" + json.list[i].Phone + "', headimg='" + json.list[i].HeadImg + "', content='" + json.list[i].Content + "' where uid = '" + uid + "' and mid = '" + json.list[i].MID + "'";
                }
                sql_tx.push(sql);

                saySrchSgl(db, uid, i + 1, length, json, fromPage, sql_tx);

            } else {

                debug('获取消息出错');

            }

        });

    } else {

        sayTxActn(db, uid, json, fromPage, sql_tx);

    }

}

function sayTxActn(db, uid, json, fromPage, sql_tx) {

    debug(sql_tx);
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

        if (error == 0) {

            saySrchAll(db, uid, fromPage);

        } else {

            openToast("保存消息失败", 3000, 5, 0);
            uexDataBaseMgr.close(db);

        }

    });

}

//查询全部的数据
function saySrchAll(db, uid, fromPage) {

    var sql_sel = "select  * from tbl_say_hello where uid = '" + uid + "' and status = 3 order by id desc";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            debug(data);
            sayDispList(db, uid, data, fromPage);

        } else {

            debug('查询新的朋友出错');

        }
    });

}

//接口.37.2.迭代好友申请列表
function sayDispList(db, uid, json, fromPage) {
    
    uexDataBaseMgr.close(db);

    //debug('PageIndex: ' + PageIndex);

    var fileUrl = localStorage.getItem("fileUrl");

    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var curJson = timeStampSplit(curTime);

    var listStr = '';
    var lastStr = '';
    
    if (json.length == 0) {

        listStr += '<div class="end-nope-number">';
        listStr += '当前没有数据';
        listStr += '</div>';

        openToast('当前没有数据', 3000, 5, 0);

    } else {

        $(json).each(function(j, s) {
            
            debug(s.content);
            
            var price = '';

            if (isJSON(s.content) == true) {
                var temp = JSON.parse(s.content);
                price = temp.price;
            } else {
                price = s.content;
            }

            listStr += '<div class="ub ubb bc-border msg-div say-sub" data-headimg="' + s.headimg + '" data-phone="' + s.phone + '" data-name="' + s.name + '" data-friend_uid="' + s.friend_uid + '" data-mid="' + s.mid + '" data-price="' + price + '">';

            listStr += '<ul ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub ub-f1 msg-ul">';

            listStr += '<li class="ub icon-li">';
            listStr += '<div class="ub-img1 msg-style" style="background-image:url(';
            listStr += fileUrl + s.headimg;
            listStr += ');"></div>';
            listStr += '</li>';

            listStr += '<li class="ub ub-f1 cont-li">';
            listStr += '<div class="ub ub-ver ub-f1 cont-style">';
            listStr += '<div class="msg-title">';
            listStr += '<span class="overflow">';
            if (s.name == '' || s.name == null || s.name == undefined) {
                listStr += s.phone;
            } else {
                listStr += s.name;
            }
            listStr += '</span>';
            listStr += '</div>';

            listStr += '<div class="msg-desc">';
            listStr += '<span class="txt-normal overflow">';
            listStr += '福包敲门中';
            listStr += '</span>';
            listStr += '</div>';
            listStr += '</div>';

            listStr += '<div class="ub ub-ver info-style">';
            listStr += '<div class="info-temp"></div>';
            listStr += '<div class="info-top">';
            listStr += '<span>';
            listStr += price + '元';
            listStr += '</span>';
            listStr += '</div>';
            listStr += '<div class="info-bot"><img src="../hb/img/hongbao_kai@2x.png"/></div>';
            
            listStr += '</div>';

            listStr += '</li>';
            listStr += '</ul>';
            listStr += '</div>';

            if (json.length == j + 1) {
                lastStr += '<div class="end-nope-number">';
                lastStr += '没有更多了';
                lastStr += '</div>';
            }

        });

        openToast('加载数据成功', 3000, 5, 0);

    }

    listStr += lastStr;
    
    debug(listStr);

    if (fromPage == 'new_friend') {

        $('#new_friend').append(listStr);

    } else if (fromPage == 'sys_msg') {

        $('#sys_msg').append(listStr);

    } else if (fromPage == 'say_hello') {
        
        $('#say_hello').append(listStr);

    }

    $('.say-sub').unbind();
    $('.say-sub').click(function() {
        
        var headimg = $(this).data('headimg');
        var name = $(this).data('name');
        var price = $(this).data('price');
        var phone = $(this).data('phone');
        
        if (name == '' || name == null || name == undefined) {
            name = phone;
        }
        var redUser = {
            headimg : headimg,
            name : name,
            price : price,
        }
        
        localStorage.setItem("redUser", JSON.stringify(redUser));
        
        var mid = $(this).data('mid');
        var friend_uid = $(this).data('friend_uid');
        var status = 1;
        var id = 1;
        
        sayRecHello(id, uid, mid, friend_uid, status);

    });

    //appcan.frame.resetBounce(1);
    
}

//接口.23.同意好友添加
function sayRecHello(id, uid, mid, friend_uid, status) {

    //id 同意/拒绝
    //mid 添加好友请求的id

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Friend/RecHello",
        type : "POST",
        data : {
            id : id,
            mid : mid,
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

                        var localPack = localStorage.getItem('localPack');

                        if (localPack == '1' || localPack == 1) {

                            appcan.window.open({
                                name : 'hbopen',
                                data : '../hb/hbopen.html',
                                aniId : 10,
                            });

                        } else {

                            uexWindow.open({
                                name : "hbopen",
                                data : "../hb/hbopen.html",
                                animID : 10,
                                flag : 1024
                            });

                        }

                        setTimeout(function() {

                            sayUpdtNew(db, uid, mid, friend_uid, status);

                        }, 500);

                    }

                } else {
                    openToast(data.msg, 3000, 5, 0);
                }

            } else {

                openToast('同意好友添加失败', 3000, 5, 0);

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

//status 3 为新的  1为已添加  2为等待通过 0为已拒绝

//接口.23.2.更新记录
function sayUpdtNew(db, uid, mid, friend_uid, status) {

    var sqls = [];
    sqls.push("update tbl_say_hello set status=" + status + " where uid = '" + uid + "' and mid = '" + mid + "'");
    //debug(sqls);
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sqls), function(error) {

        if (error == 0) {

            sayCmplNew(db, uid, mid, friend_uid, status);

        } else {

            openToast("保存消息失败", 3000, 5, 0);
            uexDataBaseMgr.close(db);

        }

    });

}

//接口.23.3.append好友申请html
function sayCmplNew(db, uid, mid, friend_uid, status) {

    $("div[data-mid='" + mid + "']").remove();

    //appcan.window.publish('newFriendMsg', 1);
    //appcan.window.publish('refreshMsgNum', 1);
    //msgSayIsrtOne(uid, friend_uid, friend_uid, 1, 2, 0, '谢谢你的敲门福包');
    //msgSayIsrtOne(uid, friend_uid, uid, 1, 2, 0, );
    msgSaySaveOne(db, uid, friend_uid, uid, 1, 2, 0, '你已领取对方的敲门福包，现在可以开始聊天了');

}
