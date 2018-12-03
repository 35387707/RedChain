//接口.19.3.用户实体类, 登陆成功之后获取所有的用户信息  footQuan
function getUserInfo(id, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Account/GetUserInfo",
        type : "POST",
        data : {
            Id : id,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")
            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    if (fromPage == 'page_other') {

                        localStorage.setItem("OtherUser", JSON.stringify(data));
                        setUserInfo(id, data, fromPage);

                    } else {

                        localStorage.setItem("UserInfo", JSON.stringify(data));
                        if (fromPage == 'login') {
                            friendAjaxList(id, fromPage);
                        } else {
                            setUserInfo(id, data, fromPage);
                        }

                    }

                } else {

                    debug("getUserInfo出错");

                    if (fromPage == 'login') {
                        hideLoading('登录失败', 5000, 5, 0);
                    } else {
                        hideLoading(data.msg, 3000, 5, 0);
                    }

                }

            } else {

                debug("getUserInfo出错");

                if (fromPage == 'login') {
                    hideLoading('登录失败', 3000, 5, 0);
                } else {
                    hideLoading('获取用户信息失败', 3000, 5, 0);
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
                hideLoading('登录失败', 3000, 5, 0);
            } else {
                hideLoading('获取用户信息失败', 3000, 5, 0);
            }
        },
    });

}

//1.0.设置用户信息
function setUserInfo(ID, UserInfo, fromPage) {

    //debug(UserInfo);

    var fileUrl = localStorage.getItem("fileUrl");

    //var ID = '';
    //用户唯一id
    var HeadImg = '';
    //用户头像
    var Name = '';
    //用户账号
    var RealCheck = '';
    //实名验证状态, 1为通过, 0为不通过
    var TrueName = '';
    //用户昵称
    var Phone = '';
    //用户手机号
    var Sex = '';
    //用户性别
    var Balance = '';
    //用户余额
    var RedBalance = '';
    //用户红包池
    var Score = '';
    //用户积分
    var UserType = '';
    //用户类型
    var Descrition = '';
    //个人签名
    var CardNumber = '';
    //推荐码
    var AllRewards = '';
    //总收入
    var TodayRewards = '';
    //今日收入
    var Address = '';
    //福包共享池
    var FubaoShare = ''
    //超级福包共享池
    var BigFubaoShare = ''
    //剩余福包数
    var MainTainRedCount = '';
    //推荐人名字
    var FID = '';
    var FUserName = '';

    if (UserInfo.Info.TrueName == '' || UserInfo.Info.TrueName == null || UserInfo.Info.TrueName == undefined) {
        TrueName = UserInfo.Info.Phone;
    } else {
        TrueName = UserInfo.Info.TrueName;
    }

    if (fromPage == 'mine' || fromPage == 'minePay' || fromPage == 'mineSpec' || fromPage == 'refresh_mine') {

        HeadImg = '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 umwh-avator uc-a2" style="background-image:url(' + fileUrl + UserInfo.Info.HeadImg + ');"></div>';

    } else if (fromPage == 'personal_data') {

        HeadImg = '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="edit-data-img edit-data-userimg uc-a2" style="background-image:url(' + fileUrl + UserInfo.Info.HeadImg + ');"></div>';

    } else if (fromPage == 'hbknock') {

        HeadImg = '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 knock-img icon-cont uc-a2" style="background-image:url(' + fileUrl + UserInfo.Info.HeadImg + ');"></div>';

    } else if (fromPage == 'page_mine') {

        HeadImg = '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 umwh-avator uc-a2" style="background-image:url(' + fileUrl + UserInfo.Info.HeadImg + ');"></div>';

    } else if (fromPage == 'page_other') {

        //<div class="ub-img other-img uc-a2 ub-a2" ontouchstart="appcan.touch('btn-act')"></div>
        HeadImg = '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 other-img uc-a2" style="background-image:url(' + fileUrl + UserInfo.Info.HeadImg + ');"></div>';

    }

    if (UserInfo.Info.Sex == 0) {

        Sex = '女';

    } else if (UserInfo.Info.Sex == 1) {

        Sex = '男';

    }
    //Sex = UserInfo.Info.Sex;

    if (UserInfo.Info.RedBalance == 0 || UserInfo.Info.RedBalance == '0' || UserInfo.Info.RedBalance == '' || UserInfo.Info.RedBalance == null || UserInfo.Info.RedBalance == undefined) {
        RedBalance = 0;
    } else {
        RedBalance = UserInfo.Info.RedBalance;
    }

    if (UserInfo.Info.FubaoShare == 0 || UserInfo.Info.FubaoShare == '0' || UserInfo.Info.FubaoShare == '' || UserInfo.Info.FubaoShare == null || UserInfo.Info.FubaoShare == undefined) {
        FubaoShare = 0;
    } else {
        FubaoShare = UserInfo.Info.FubaoShare;
    }

    if (UserInfo.Info.BigFubaoShare == 0 || UserInfo.Info.BigFubaoShare == '0' || UserInfo.Info.BigFubaoShare == '' || UserInfo.Info.BigFubaoShare == null || UserInfo.Info.BigFubaoShare == undefined) {
        BigFubaoShare = 0;
    } else {
        BigFubaoShare = UserInfo.Info.BigFubaoShare;
    }

    if (UserInfo.Info.Balance == 0 || UserInfo.Info.Balance == '0' || UserInfo.Info.Balance == '' || UserInfo.Info.Balance == null || UserInfo.Info.Balance == undefined) {
        Balance = 0;
    } else {
        Balance = UserInfo.Info.Balance;
    }

    if (UserInfo.Info.Descrition == '' || UserInfo.Info.Descrition == null || UserInfo.Info.Descrition == undefined) {
        Descrition = '这家伙很懒, 暂时还没有签名...';
    } else {
        Descrition = UserInfo.Info.Descrition;
    }

    CardNumber = UserInfo.Info.CardNumber;

    AllRewards = UserInfo.Info.AllRewards;

    TodayRewards = UserInfo.Info.TodayRewards;

    if (UserInfo.Info.Address == '' || UserInfo.Info.Address == null || UserInfo.Info.Address == undefined) {
        Address = '';
    } else {
        Address = UserInfo.Info.Address;
    }

    UserType = UserInfo.Info.UserType;
    MainTainRedCount = UserInfo.Info.MainTainRedCount;
    FID = UserInfo.Info.FID;
    FUserName = UserInfo.Info.FUserName;

    localStorage.setItem("before", UserInfo.Info.AreaCode);

    $('#HeadImg').children().remove();
    $('#HeadImg').append(HeadImg);

    if (fromPage == 'TrueName') {
        $('#TrueName').val(TrueName);
    } else {
        $('#TrueName').text(TrueName);
    }

    $('#Sex').text(Sex);
    $('#Balance').text(Balance);

    $('#RedBalance').text(RedBalance);
    $('#FubaoShare').text(FubaoShare);
    $('#BigFubaoShare').text(BigFubaoShare);

    if (fromPage == 'Descrition') {
        $('#Descrition').val(Descrition);
    } else {
        $('#Descrition').text(Descrition);
    }

    if (fromPage == 'Address') {
        $('#Address').val(Address);
    } else {
        $('#Address').text(Address);
    }

    $('#CardNumber').text(CardNumber);
    $('#AllRewards').text(AllRewards);
    $('#TodayRewards').text(TodayRewards);

    if (UserType == 0) {
        //usertype
        $('.UserType').text('福星');
    } else if (UserType == 1) {
        $('.guanggaofubao').removeClass('no-dis');
        $('.adv-edit').removeClass('no-dis');
        $('.UserType').text('福将');
    } else if (UserType == 2) {
        $('.guanggaofubao').removeClass('no-dis');
        $('.UserType').text('福相');
        $('.adv-edit').removeClass('no-dis');
    }
    $('.UserType').data('usertype', UserType);
    $('#MainTainRedCount').text(MainTainRedCount);

    if (FUserName == '' || FUserName == null || FUserName == undefined) {
        $('#FUserName').text('');
        $('#FUserName').data('fid', '');
        $('.FUserName').addClass('no-dis');
    } else {
        $('#FUserName').text(FUserName);
        $('#FUserName').data('fid', FID);
        $('.FUserName').removeClass('no-dis');
    }

    localStorage.setItem("CardNumber", CardNumber);

    if (fromPage == 'page_mine') {

        getSendPacketList(1, ID, fromPage);

    } else if (fromPage == 'page_other') {

        appcan.window.publish('setInfo', JSON.stringify(UserInfo));

        getSendPacketList(1, ID, fromPage);

    } else if (fromPage == 'refresh_mine') {

        openToast('刷新用户数据成功', 3000, 5, 0);

    }

}

//接口.21.更改头像的url
function updateHeadImg(HeadImg) {
    
    var fileUrl = localStorage.getItem("fileUrl");

    debug(HeadImg);

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Account/UpdateHeadImg",
        type : "POST",
        data : {
            HeadImg : HeadImg,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {
                    
                    localStorage.setItem("user_img", HeadImg);

                    var imgStr = '<div class="edit-data-img edit-data-userimg uc-a2 ub-a2" style="background-image:url(' + fileUrl + HeadImg + ');"></div>';

                    $('#HeadImg').children().remove();
                    $('#HeadImg').append(imgStr);

                    openToast('上传头像成功', 3000, 5, 0);
                    setTimeout(function() {

                        appcan.window.publish('chgSucc', HeadImg);
                        closeToast();

                    }, 300);

                } else {

                    openToast(data.msg, 3000, 5, 0);

                }

            } else {

                openToast('上传头像失败', 3000, 5, 0);

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

//接口.21.更改用户信息
function updateUserInfo(fromPage) {

    var TrueName = trim($('#TrueName').val());
    var Sex = trim($('#Sex').text());
    var Descrition = trim($('#Descrition').val());
    var Address = trim($('#Address').val());

    if (Sex == '男') {
        Sex = 1;
    } else if (Sex == '女') {
        Sex = 0;
    }

    var come = {
        TrueName : TrueName,
        Descrition : Descrition,
        Sex : Sex,
        Address : Address,
    }

    debug(come);

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Account/UpdateUserinfo",
        type : "POST",
        data : {
            TrueName : TrueName,
            Descrition : Descrition,
            Sex : Sex,
            Address : Address,
        },
        timeout : 30000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    appcan.window.publish('chgSucc', 0);
                    if (fromPage == 'personal_nick') {

                        appcan.window.publish('chgNick', TrueName);
                        openToast('更改昵称成功', 3000, 5, 0);
                        setTimeout(function() {

                            appcan.window.close(-1);

                        }, 500);

                    } else if (fromPage == 'personal_note') {

                        appcan.window.publish('chgNote', Descrition);
                        openToast('更改个人说明成功', 3000, 5, 0);
                        setTimeout(function() {

                            appcan.window.close(-1);

                        }, 500);

                    } else if (fromPage == 'personal_addr') {

                        appcan.window.publish('chgAddr', Address);
                        openToast('更改地址成功', 3000, 5, 0);
                        setTimeout(function() {

                            appcan.window.close(-1);

                        }, 500);

                    } else if (fromPage == 'personal_sex') {

                        appcan.window.publish('chgSex', Sex);
                        openToast('设置性别成功', 3000, 5, 0);
                        setTimeout(function() {

                            appcan.window.close(-1);

                        }, 500);

                    } else {

                        openToast('更改用户信息成功', 3000, 5, 0);

                    }

                } else {

                    if (fromPage == 'personal_nick') {
                        openToast('更改昵称失败', 3000, 5, 0);
                    } else if (fromPage == 'personal_note') {
                        openToast('更改个人说明失败', 3000, 5, 0);
                    } else if (fromPage == 'personal_addr') {
                        openToast('更改地址失败', 3000, 5, 0);
                    } else if (fromPage == 'personal_sex') {
                        openToast('设置性别失败', 3000, 5, 0);
                    } else {
                        openToast('更改用户信息失败', 3000, 5, 0);
                    }

                }

            } else {

                if (fromPage == 'personal_nick') {
                    openToast('更改昵称失败', 3000, 5, 0);
                } else if (fromPage == 'personal_note') {
                    openToast('更改个人说明失败', 3000, 5, 0);
                } else if (fromPage == 'personal_addr') {
                    openToast('更改地址失败', 3000, 5, 0);
                } else if (fromPage == 'personal_sex') {
                    openToast('设置性别失败', 3000, 5, 0);
                } else {
                    openToast('更改用户信息失败', 3000, 5, 0);
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
