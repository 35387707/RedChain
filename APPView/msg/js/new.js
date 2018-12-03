//接口.37.获取收到的好友申请列表(分页返回, 每页20条);
function applyGetList(uid, PageIndex, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Friend/GetFriendApplyList",
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

                        var newCount = localStorage.getItem("newCount");
                        if (newCount == 0 || newCount == '0' || newCount == null || newCount == '' || newCount == undefined || newCount == NaN) {

                            noNewListActn(db, uid, data, fromPage, []);

                        } else {

                            applySrchSgl(db, uid, 0, data.list.length, data, fromPage, []);

                        }

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

//如果列表0, 那么去更新旧的数据
function noNewListActn(db, uid, json, fromPage, sql_tx) {

    var status = 1;
    sql_tx.push("update tbl_new_friend set status=" + status + " where uid = '" + uid + "'");

    applyTxActn(db, uid, json, fromPage, sql_tx);

}

//接口.37.存储本地sqlite方法
function applySrchSgl(db, uid, i, length, json, fromPage, sql_tx) {

    if (i < length) {

        var sql_sel = "select  * from tbl_new_friend where uid = '" + uid + "' and mid = '" + json.list[i].MID + "'";
        uexDataBaseMgr.select(db, sql_sel, function(error, data) {
            if (!error) {

                var sql = "";
                if (data.length == 0) {
                    sql = "insert into tbl_new_friend (uid, mid, friend_uid, name, phone, headimg, content, status, last_date) values ('" + uid + "', '" + json.list[i].MID + "', '" + json.list[i].UID + "', '" + json.list[i].Name + "', '" + json.list[i].Phone + "', '" + json.list[i].HeadImg + "', '" + json.list[i].Content + "', 3, '" + timeStampFormat(json.list[i].CreateTime) + "')";
                } else {
                    sql = "update tbl_new_friend set friend_uid='" + json.list[i].UID + "', name='" + json.list[i].Name + "', phone='" + json.list[i].Phone + "', headimg='" + json.list[i].HeadImg + "', content='" + json.list[i].Content + "' where uid = '" + uid + "' and mid = '" + json.list[i].MID + "'";
                }
                sql_tx.push(sql);

                applySrchSgl(db, uid, i + 1, length, json, fromPage, sql_tx);

            } else {

                debug('获取消息出错');

            }

        });

    } else {

        applyTxActn(db, uid, json, fromPage, sql_tx);

    }

}

function applyTxActn(db, uid, json, fromPage, sql_tx) {

    debug(sql_tx);
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

        if (error == 0) {

            applySrchAll(db, uid, fromPage);

        } else {

            openToast("保存消息失败", 3000, 5, 0);
            uexDataBaseMgr.close(db);

        }

    });

}

//查询全部的数据
function applySrchAll(db, uid, fromPage) {

    var sql_sel = "select  * from tbl_new_friend where uid = '" + uid + "' order by id desc";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            debug(data);
            applyDispList(db, uid, data, fromPage);

        } else {

            debug('查询新的朋友出错');

        }
    });

}

//接口.37.2.迭代好友申请列表
function applyDispList(db, uid, json, fromPage) {

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

            listStr += '<div class="ub ubb bc-border msg-div" data-friend_uid="' + s.friend_uid + '" data-del_mid="' + s.mid + '" data-mid="' + s.mid + '">';

            listStr += '<ul class="ub ub-f1 msg-ul">';

            listStr += '<li class="ub icon-li">';
            listStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 sys-style" style="background-image:url(';
            listStr += fileUrl + s.headimg;
            listStr += ');"></div>';
            listStr += '</li>';

            listStr += '<li data-right="' + s.mid + '" class="ub ub-f1 cont-li">';
            listStr += '<div class="ub ub-ver ub-f1 cont-style">';
            listStr += ' <div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="msg-title">';
            listStr += '<span class="overflow">';
            if (s.name == '' || s.name == null || s.name == undefined) {
                listStr += s.phone;
            } else {
                listStr += s.name;
            }
            listStr += '</span>';
            listStr += '</div>';

            listStr += '<div class="msg-desc">';
            listStr += '<span ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="txt-normal overflow">';
            if (isJSON(s.content) == true) {
                var temp = JSON.parse(s.content);
                listStr += temp.content;
            } else {
                listStr += s.content;
            }
            listStr += '</span>';
            listStr += '<span class="txt-spec">';
            listStr += '';
            listStr += '</span>';
            listStr += '</div>';
            listStr += '</div>';

            if (s.status == 2 || s.status == '2') {

                listStr += '<div class="ub ub-ver info-style">';
                listStr += '<div class="info-temp"></div>';
                listStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="info-top">';
                listStr += '<span>等待通过</span>';
                listStr += '</div>';
                listStr += '<div class="info-bot"></div>';
                listStr += '</div>';

            } else if (s.status == 3 || s.status == '3') {

                listStr += '<div data-spec="' + s.mid + '" class="spec-middle">';
                listStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" data-friend_uid="' + s.friend_uid + '" data-mid="' + s.mid + '" class="ignore-btn">';
                listStr += '拒绝';
                listStr += '</div>';
                listStr += '</div>';

                listStr += '<div data-spec="' + s.mid + '" class="spec-middle">';
                listStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" data-friend_uid="' + s.friend_uid + '" data-mid="' + s.mid + '" class="agree-btn">';
                listStr += '同意';
                listStr += '</div>';
                listStr += '</div>';

            } else if (s.status == 1 || s.status == '1') {

                listStr += '<div class="ub ub-ver info-style">';
                listStr += '<div class="info-temp"></div>';
                listStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="info-top">';
                listStr += '<span>已添加</span>';
                listStr += '</div>';
                listStr += '<div class="info-bot"></div>';
                listStr += '</div>';

            } else if (s.status == 0 || s.status == '0') {

                listStr += '<div class="ub ub-ver info-style">';
                listStr += '<div class="info-temp"></div>';
                listStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="info-top">';
                listStr += '<span>已拒绝</span>';
                listStr += '</div>';
                listStr += '<div class="info-bot"></div>';
                listStr += '</div>';

            }

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

    if (fromPage == 'new_friend') {

        $('#new_friend').append(listStr);

    } else if (fromPage == 'sys_msg') {

        $('#sys_msg').append(listStr);

    }

    $('.ignore-btn').unbind();
    $('.ignore-btn').click(function() {

        var mid = $(this).data('mid');
        var friend_uid = $(this).data('friend_uid');
        var status = 0;
        var id = 0;
        applyAgreeNew(id, uid, mid, friend_uid, status);

    });
    $('.agree-btn').unbind();
    $('.agree-btn').click(function() {

        var mid = $(this).data('mid');
        var friend_uid = $(this).data('friend_uid');
        var status = 1;
        var id = 1;
        applyAgreeNew(id, uid, mid, friend_uid, status);

    });

    //appcan.frame.resetBounce(1);
    uexDataBaseMgr.close(db);

}

//接口.23.同意好友添加
function applyAgreeNew(id, uid, mid, friend_uid, status) {

    //id 同意/拒绝
    //mid 添加好友请求的id

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Friend/Agree",
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
                        applyUpdtNew(db, uid, mid, friend_uid, status);
                    }

                } else {

                    if (data.msg == '已处理' || data.msg == '已拒绝') {

                        openToast(data.msg, 3000, 5, 0);

                        var db = uexDataBaseMgr.open(uid + ".db");
                        if (db != null) {
                            applyUpdtNew(db, uid, mid, friend_uid, 0);
                        }

                    } else {
                        openToast(data.msg, 3000, 5, 0);
                    }

                }

            } else {

                if (status == 1) {
                    openToast('同意好友添加失败', 3000, 5, 0);
                } else {
                    openToast('拒绝好友添加失败', 3000, 5, 0);
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
            openToast(localStorage.getItem("errorMsg"), 3000, 5, 0);

        },
    });

}

//status 3 为新的  1为已添加  2为等待通过 0为已拒绝

//接口.23.2.更新记录
function applyUpdtNew(db, uid, mid, friend_uid, status) {

    var sqls = [];
    var sql = '';

    if (status == 0) {
        sql = "delete from tbl_new_friend where uid = '" + uid + "' and mid = '" + mid + "'";
    } else if (status == 1) {
        sql = "update tbl_new_friend set status=" + status + " where uid = '" + uid + "' and mid = '" + mid + "'";
    }

    sqls.push(sql);
    debug(sqls);
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sqls), function(error) {

        if (error == 0) {

            if (status == 1) {
                applyApndNew(db, uid, mid, friend_uid, status);
            } else {
                removeNewFriend(db, uid, mid, friend_uid, status);
            }

        } else {

            openToast("保存消息失败", 3000, 5, 0);
            uexDataBaseMgr.close(db);

        }

    });

}

//接口.23.3.append好友申请html
function applyApndNew(db, uid, mid, friend_uid, status) {

    var tempStr = '';
    if (status == 2) {

        tempStr += '<div class="ub ub-ver info-style">';
        tempStr += '<div class="info-temp"></div>';
        tempStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="info-top">';
        tempStr += '<span>等待通过</span>';
        tempStr += '</div>';
        tempStr += '<div class="info-bot"></div>';
        tempStr += '</div>';

    } else if (status == 0) {

        tempStr += '<div class="ub ub-ver info-style">';
        tempStr += '<div class="info-temp"></div>';
        tempStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="info-top">';
        tempStr += '<span>已拒绝</span>';
        tempStr += '</div>';
        tempStr += '<div class="info-bot"></div>';
        tempStr += '</div>';

    } else if (status == 1) {

        tempStr += '<div class="ub ub-ver info-style">';
        tempStr += '<div class="info-temp"></div>';
        tempStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="info-top">';
        tempStr += '<span>已添加</span>';
        tempStr += '</div>';
        tempStr += '<div class="info-bot"></div>';
        tempStr += '</div>';

    }

    $("div[data-spec='" + mid + "']").remove();
    $("li[data-right='" + mid + "']").append(tempStr);

    var newTemp = parseInt(localStorage.getItem("newCount"));
    var newCalc = '';
    if (newTemp == NaN || newTemp == undefined || newTemp == 0) {

    } else {
        newCalc = newTemp - 1;
        localStorage.setItem("newCount", newCalc);
    }

    //appcan.window.publish('newFriendMsg', 1);
    //appcan.window.publish('refreshMsgNum', 1);
    //msgNewIsrtOne(uid, friend_uid, uid, 1, 2, 0, '你已成功添加对方为好友，现在可以开始聊天了');
    msgNewSaveOne(db, uid, friend_uid, uid, 1, 2, 0, '你已成功添加对方为好友，现在可以开始聊天了');

}

/*
 * 移除对应的记录
 */
function removeNewFriend(db, uid, mid, friend_uid, status) {

    //del_mid
    $("div[data-del_mid='" + mid + "']").remove();
    uexDataBaseMgr.close(db);

    var newTemp = parseInt(localStorage.getItem("newCount"));
    var newCalc = '';
    if (newTemp == NaN || newTemp == undefined || newTemp == 0) {

    } else {
        newCalc = newTemp - 1;
        localStorage.setItem("newCount", newCalc);
    }

    appcan.window.publish('newFriendMsg', 1);

}
