//接口.36.判断传入的手机号码正不正确
function validateMobile(mobile) {

    if (mobile.length == 0 || mobile == '' || mobile == undefined) {
        $('#srch_list').children().remove();
        return false;
    } else {

        if (mobile.length != 11) {
            $('#srch_list').children().remove();
            return false;
        } else {

            //var regExps = /^(((13[0-9]{1})|(15[0-9]{1})|(18[0-9]{1}))+\d{8})$/;
            var regExps = /^1[3|4|5|7|8][0-9]\d{4,8}$/;
            if (!regExps.test(mobile)) {
                $('#srch_list').children().remove();
                return false;
            } else {

                judgMobileSelf(mobile);

            }

        }

    }

}

//接口.36.0.首先判断输入的手机号码是不是本人
function judgMobileSelf(phone) {

    var UserInfo = JSON.parse(localStorage.getItem("UserInfo"));
    if (UserInfo.Info.Phone == phone) {

        doFriendSelf(UserInfo);

    } else {

        var uid = localStorage.getItem("uid");
        var db = uexDataBaseMgr.open(uid + ".db");
        if (db != null) {
            srchMobileSgl(db, uid, phone);
        }

    }

}

//接口.36.1.本地sql查找, 如果对方已经是你的好友那么就什么都不用做了
function srchMobileSgl(db, uid, phone) {

    var sql_sel = "select  * from tbl_friend where uid = '" + uid + "' and phone = '" + phone + "'";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            judgMobileSgl(db, uid, phone, data);

        } else {

            uexDataBaseMgr.close(db);
            openToast('获取好友列表失败', 3000, 5, 0);

        }

    });

}

//接口.36.2.判断处理查找的数据
function judgMobileSgl(db, uid, phone, json) {

    uexDataBaseMgr.close(db);

    if (json.length == 0) {

        //等于0的时候就去调用接口
        srchFriendNew(phone);

    } else {

        dispMobileSgl(json);

    }

}

//接口.36.搜索账户列表
function srchFriendNew(key) {
    //openToast('手机号码有效', 2000, 5, 0);
    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Account/Search",
        type : "POST",
        data : {
            key : key,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")
            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {
                    doFriendNew(data);
                } else {
                    openToast(data.msg, 3000, 5, 0);
                }

            } else {

                openToast('搜索好友失败', 3000, 5, 0);

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

//接口.36.1.处理搜索到的好友数据
function doFriendNew(json) {
    
    debug(json);

    var TrueName = '';
    var UserInfo = JSON.parse(localStorage.getItem("UserInfo"));
    if (UserInfo.Info.TrueName == '' || UserInfo.Info.TrueName == null || UserInfo.Info.TrueName == undefined) {
        TrueName = UserInfo.Info.Phone;
    } else {
        TrueName = UserInfo.Info.TrueName;
    }

    var waitStr = '<div class="wait-passed">';
    waitStr += '待通过';
    waitStr += '</div>';

    var fileUrl = localStorage.getItem("fileUrl");
    var newStr = '';
    if (json.list.length == 0 || json.list == '') {

        openToast('账号不存在', 3000, 5, 0);
        newStr += '<div class="end-nope-number">';
        newStr += '<a class="add-hongyun">';
        newStr += '账号不存在';
        newStr += '</a>';
        newStr += '</div>';

    } else {

        $(json.list).each(function(i, v) {

            newStr += '<div class="ub ubb bc-border msg-div">';
            newStr += '<ul class="ub ub-f1 msg-ul">';
            newStr += '<li class="ub icon-li">';
            newStr += '<div class="ub-img1 sys-style" style="background-image:url(' + fileUrl + v.HeadImg + ');"></div>';
            newStr += '</li>';

            newStr += '<li class="ub ub-f1 cont-li">';
            newStr += '<div class="ub ub-ver ub-f1 cont-style">';
            newStr += '<div class="msg-title">';
            newStr += '<span class="overflow">';
            if (v.TrueName == null || v.TrueName == '' || v.TrueName == undefined) {
                newStr += v.Phone;
            } else {
                newStr += v.TrueName;
            }
            newStr += '</span>';
            newStr += '</div>';
            newStr += '<div class="msg-desc">';
            newStr += '<span class="txt-normal overflow">'
            if (v.Descrition == null || v.Descrition == '' || v.Descrition == undefined) {
                newStr += '这家伙很懒，暂时还没有签名...';
            } else {
                newStr += v.Descrition;
            }
            newStr += '</span>';
            newStr += '</div>';
            newStr += '</div>';

            newStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="spec-middle" data-spec="' + v.ID + '">';
            newStr += '<div class="add-btn" data-id="' + v.ID + '">';
            newStr += '添加';
            newStr += '</div>';
            newStr += '</div>';

            newStr += '</li>';
            newStr += '</ul>';
            newStr += '</div>';

        });

    }

    $('#srch_list').children().remove();
    $('#srch_list').append(newStr);

    /*
     $('.add-btn').unbind();
     $('.add-btn').click(function() {

     var id = 1;
     var mid = $(this).data('id');
     $("div[data-spec='" + mid + "']").children().remove();
     $("div[data-spec='" + mid + "']").append(waitStr);
     debug(mid);
     //agreeNewFriend(id, mid);
     });*/

    $('.spec-middle').unbind();
    $('.spec-middle').click(function() {
        
        debug('hey');

        if ($(this).html().indexOf('添加') != -1) {

            var content = {
                sign : '好友申请',
                name : TrueName,
                content : '我是' + TrueName,
            }

            var Rid = $(this).data('spec');
            queryMessages('sendToUser', 3, Rid, '', 10, 0, content, 'srch_friend');

        } else if ($(this).html().indexOf('待通过') != -1) {

            openToast('已经发送好友申请，请耐心等候回复', 3000, 5, 0);

        }

    });

}

//接口.36.1.处理搜索到的好友数据
function doFriendSelf(json) {

    var fileUrl = localStorage.getItem("fileUrl");
    var newStr = '';

    $(json.Info).each(function(i, v) {

        newStr += '<div class="ub ubb bc-border msg-div">';
        newStr += '<ul ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub ub-f1 msg-ul">';
        newStr += '<li class="ub icon-li">';
        newStr += '<div class="ub-img1 sys-style" style="background-image:url(' + fileUrl + v.HeadImg + ');"></div>';
        newStr += '</li>';

        newStr += '<li class="ub ub-f1 cont-li">';
        newStr += '<div class="ub ub-ver ub-f1 cont-style">';
        newStr += '<div class="msg-title">';
        newStr += '<span class="overflow">';
        if (v.TrueName == null || v.TrueName == '' || v.TrueName == undefined) {
            newStr += v.Phone;
        } else {
            newStr += v.TrueName;
        }
        newStr += '</span>';
        newStr += '</div>';
        newStr += '<div class="msg-desc">';
        newStr += '<span class="txt-normal overflow">';
        if (v.Descrition == null || v.Descrition == '' || v.Descrition == undefined) {
            newStr += '这家伙很懒，暂时还没有签名...';
        } else {
            newStr += v.Descrition;
        }
        newStr += '</span>';
        newStr += '</div>';
        newStr += '</div>';

        newStr += '<div class="ub ub-ver info-style" data-spec="' + v.ID + '">';
        newStr += '<div class="info-temp"></div>';
        newStr += '<div class="info-top" data-id="' + v.ID + '">';
        newStr += '<span>本人</span>';
        newStr += '</div>';
        newStr += '<div class="info-bot"></div>';
        newStr += '</div>';

        newStr += '</li>';
        newStr += '</ul>';
        newStr += '</div>';

    });

    $('#srch_list').children().remove();
    $('#srch_list').append(newStr);

    $('.msg-ul').unbind();
    $('.msg-ul').click(function() {

        openToast('-_-|||，这个是你本人啊', 3000, 5, 0);

    });

}

//接口.36.1.处理搜索到的好友数据
function dispMobileSgl(json) {
    
    debug(json);

    var TrueName = '';
    var UserInfo = JSON.parse(localStorage.getItem("UserInfo"));
    if (UserInfo.Info.TrueName == '' || UserInfo.Info.TrueName == null || UserInfo.Info.TrueName == undefined) {
        TrueName = UserInfo.Info.Phone;
    } else {
        TrueName = UserInfo.Info.TrueName;
    }

    var waitStr = '<div class="wait-passed">';
    waitStr += '待通过';
    waitStr += '</div>';

    var fileUrl = localStorage.getItem("fileUrl");
    var newStr = '';

    $(json).each(function(i, v) {

        if (v.friend_status_id == 1) {

            newStr += '<div class="ub ubb bc-border msg-div msg-alr">';
            newStr += '<ul ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub ub-f1 msg-ul">';
            newStr += '<li class="ub icon-li">';
            newStr += '<div class="ub-img1 sys-style" style="background-image:url(' + fileUrl + v.head_img + ');"></div>';
            newStr += '</li>';

            newStr += '<li class="ub ub-f1 cont-li">';
            newStr += '<div class="ub ub-ver ub-f1 cont-style">';
            newStr += '<div class="msg-title">';
            newStr += '<span class="overflow">';
            if (v.true_name == null || v.true_name == '' || v.true_name == undefined) {
                newStr += v.phone;
            } else {
                newStr += v.true_name;
            }
            newStr += '</span>';
            newStr += '</div>';
            newStr += '<div class="msg-desc">';
            newStr += '<span class="txt-normal overflow">';
            if (v.descrition == null || v.descrition == '' || v.descrition == undefined) {
                newStr += '这家伙很懒，暂时还没有签名...';
            } else {
                newStr += v.descrition;
            }
            newStr += '</span>';
            newStr += '</div>';
            newStr += '</div>';

            newStr += '<div class="ub ub-ver info-style" data-spec="' + v.friend_uid + '">';
            newStr += '<div class="info-temp"></div>';
            newStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="info-top" data-id="' + v.friend_uid + '">';
            newStr += '<span>已添加</span>';
            newStr += '</div>';
            newStr += '<div class="info-bot"></div>';
            newStr += '</div>';

            newStr += '</li>';
            newStr += '</ul>';
            newStr += '</div>';

        } else {

            newStr += '<div class="ub ubb bc-border msg-div">';
            newStr += '<ul class="ub ub-f1 msg-ul">';
            newStr += '<li class="ub icon-li">';
            newStr += '<div class="ub-img1 sys-style" style="background-image:url(' + fileUrl + v.head_img + ');"></div>';
            newStr += '</li>';

            newStr += '<li class="ub ub-f1 cont-li">';
            newStr += '<div class="ub ub-ver ub-f1 cont-style">';
            newStr += '<div class="msg-title">';
            newStr += '<span class="overflow">';
            if (v.true_name == null || v.true_name == '' || v.true_name == undefined) {
                newStr += v.phone;
            } else {
                newStr += v.true_name;
            }
            newStr += '</span>';
            newStr += '</div>';
            newStr += '<div class="msg-desc">';
            newStr += '<span class="txt-normal overflow">';
            if (v.descrition == null || v.descrition == '' || v.descrition == undefined) {
                newStr += '这家伙很懒，暂时还没有签名...';
            } else {
                newStr += v.descrition;
            }
            newStr += '</span>';
            newStr += '</div>';
            newStr += '</div>';

            newStr += '<div class="spec-middle" data-spec="' + v.friend_uid + '">';
            newStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="add-btn" data-id="' + v.friend_uid + '">';
            newStr += '添加';
            newStr += '</div>';
            newStr += '</div>';

            newStr += '</li>';
            newStr += '</ul>';
            newStr += '</div>';

        }

    });

    $('#srch_list').children().remove();
    $('#srch_list').append(newStr);

    /*
     $('.add-btn').unbind();
     $('.add-btn').click(function() {

     var id = 1;
     var mid = $(this).data('id');
     $("div[data-spec='" + mid + "']").children().remove();
     $("div[data-spec='" + mid + "']").append(waitStr);
     debug(mid);
     //agreeNewFriend(id, mid);
     });*/
    
    $('.msg-alr').unbind();
    $('.msg-alr').click(function(){
        openToast('对方已经是你的好友', 3000, 5, 0);
    });

    $('.spec-middle').unbind();
    $('.spec-middle').click(function() {
        
        debug('hey');

        if ($(this).html().indexOf('已添加') != -1) {

            openToast('对方已经是你的好友', 3000, 5, 0);

        } else if ($(this).html().indexOf('待通过') != -1) {

            openToast('已经发送好友申请，请耐心等候回复', 3000, 5, 0);

        } else {

            var content = {
                sign : '好友申请',
                name : TrueName,
                content : '我是' + TrueName,
            }
            
            debug(content);

            var Rid = $(this).data('spec');
            queryMessages('sendToUser', 3, Rid, '', 10, 0, content, 'srch_friend');

            /*
            setTimeout(function() {
            
                            $("div[data-spec='" + Rid + "']").children().remove();
                            $("div[data-spec='" + Rid + "']").append(waitStr);
                            debug(mid);
            
                        }, 750);*/
            

        }

    });

}