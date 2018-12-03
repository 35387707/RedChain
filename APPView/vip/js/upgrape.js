function setAcctInfo() {

    var UserInfo = JSON.parse(localStorage.getItem("UserInfo"));
    var Phone = UserInfo.Info.Phone;
    $('#phone').val(Phone);

}

//2.0.打开底部导航栏
function openEditorActn(fromPage) {

    var last = $('.photo-album').children("div:last-child").index() + 1;
    var max = 0;

    if (last != 10) {

        max = 10 - last;

        uexWindow.actionSheet({
            title : "添加图片",
            cancel : "取消",
            buttons : "拍照,从手机相册选中"
        }, function(index) {
            if (index == 0) {
                openCamera(fromPage);
            } else if (index == 1) {
                toImgPicker(max, fromPage);
            }
        });

    } else {

        openToast('最多只能输入9张图片', 3000, 5, 0);

    }

};

$("#send_code").click(function() {

    var mobile = $('#phone').val();
    var before = parseInt(localStorage.getItem("before"));
    getJudgCode(before, mobile);

});

var upgr_stat = true;
$('#upgrape').click(function() {

    var phone = $('#phone').val();
    var fromPage = 'vip_upgr';
    
    if (upgr_stat) {

        upgr_stat = false;
        validateMobile(phone, fromPage);

    }

});

//判断所有必填条件
function judgVipReqCont() {

    var req_arr = [];

    var nt = $('#nt').data('nt');
    if (nt == '' || nt == null || nt == undefined) {
        req_arr.push('会员类型');
    }

    var judg_code = trim($('#judg_code').val());
    if (judg_code == '' || judg_code == null || judg_code == undefined) {
        req_arr.push('验证码');
    }

    /*
    var credid = trim($('#credid').val());
        if (credid == '' || credid == null || credid == undefined) {
            req_arr.push('韬唤璇�);
        }*/
    

    var title = trim($('#title').val());
    if (title == '') {
        req_arr.push('广告内容');
    }

    if (req_arr.length == 0 || req_arr == '') {
        
        /*
         upgr_stat = true;

         if (checkIdentity(credid)) {
         upgradeUserType();
         } else {
         openToast('韬唤璇佹牸寮忛敊璇�, 3000, 5, 0);
         }*/
        judgVipImgUpld();
        
    } else {

        upgr_stat = true;
        openToast('请输入' + req_arr.join('、'), 3000, 5, 0);

    }

}

function judgVipImgUpld() {

    var imgArr = [];
    //获取所有的图片路径
    $('.disp-pic').each(function(i, v) {

        if ($(this).data('pic_p')) {
            debug($(this).data('pic_p'));
            imgArr.push($(this).data('pic_p'));
        }

    });

    //如果当前没有图片上传
    if (imgArr.length == 0) {

        //sendRedPacket(imgArr);
        upgr_stat = true;
        openToast('请至少选择一张图片', 3000, 5, 0);

    } else {

        //uexLoadingView.openCircleLoading();
        //doVipImgUpld(Pwd, imgArr, 0, imgArr.length, 'vip_upgr');
        upgr_stat = true;
        judgHasPayPwd();

    }

}

function beginVipImgUpld(Pwd) {

    var imgArr = [];
    //获取所有的图片路径
    $('.disp-pic').each(function(i, v) {

        if ($(this).data('pic_p')) {
            debug($(this).data('pic_p'));
            imgArr.push($(this).data('pic_p'));
        }

    });

    //如果当前没有图片上传
    if (imgArr.length == 0) {

        //sendRedPacket(imgArr);
        upgr_stat = true;
        openToast('请至少选择一张图片', 3000, 5, 0);

    } else {

        //uexLoadingView.openCircleLoading();
        doVipImgUpld(Pwd, imgArr, 0, imgArr.length, 'vip_upgr');

    }

}

//img.1.循环上传图片的逻辑
function doVipImgUpld(Pwd, imgArr, i, length, fromPage) {

    var url = localStorage.getItem("upldUrl");

    if (i < length) {

        var uploader = uexUploaderMgr.create({
            url : url,
            type : 2
        });

        if (uploader != null) {

            uexUploaderMgr.uploadFile(uploader, imgArr[i], "imgArr", 2, 1080, function(packageSize, percent, responseString, status) {
                switch (status) {
                case 0:
                    //alert("文件大小:" + packageSize + "<br>上传进度:" + percent + "%");
                    break;
                case 1:

                    responseString = JSON.parse(responseString);
                    //debug(responseString);
                    //debug(responseString.msg);
                    imgArr[i] = responseString.msg;

                    doVipImgUpld(Pwd, imgArr, i + 1, length, fromPage);

                    break;
                case 2:
                    commonAlert('图片上传失败！ ');
                    upgr_stat = true;
                    //uexLoadingView.close();
                    break;
                }

            });

        } else {

            commonAlert('图片上传出错！ ');
            //uexLoadingView.close();
            upgr_stat = true;

        }

    } else {

        upgradeUserType(imgArr, Pwd);

    }

}

function upgradeUserType(imgArr, Pwd) {

    var nt = $('#nt').data('nt');
    var code = trim($('#judg_code').val());
    //var credid = trim($('#credid').val());
    var credid = '';
    var payment = 0;
    
    var title = trim($('#title').val());
    
    var img = '';
    if (imgArr.length == 0 || imgArr == '' || imgArr == null || imgArr == undefined) {
        //debug('没有图片的红包提交');
    } else {
        //debug('有图片的红包提交');
        img = imgArr.join(';');
    }

    if (img == '' || img == null || img == undefined) {
        img = '';
    }

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    //登录的逻辑
    appcan.ajax({
        url : ajaxUrl + "pay/UpgradeUserType",
        type : "POST",
        data : {
            nt : nt,
            code : code,
            credid : credid,
            payment : payment,
            title : title,
            img : img,
            Pwd : Pwd,
        },
        timeout : 30000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    //uexLoadingView.close();
                    if (nt == 1 || nt == '1') {
                        localStorage.setItem("succTxt", '恭喜 升级福将成功！');
                        payMsgToast('升级福将成功', 5000, 5, 0);
                    } else if (nt == 2 || nt == '2') {
                        localStorage.setItem("succTxt", '恭喜 升级福相成功！');
                        payMsgToast('升级福相成功', 5000, 5, 0);
                    }
                    
                    var fromPage = localStorage.getItem("fromPage");
                    if (fromPage == 'hbknock') {
                        appcan.window.publish('vipSucc', nt);
                    } else {
                        appcan.window.publish('upgrSucc', nt);
                    }

                    appcan.window.publish('newFriendMsg', nt);

                    setTimeout(function() {

                        appcan.window.close(-1);

                    }, 1500);

                } else if (data.code == -113 || data.code == '-113') {

                    upgr_stat = true;
                    //uexLoadingView.close();

                    setTimeout(function() {

                        appcan.window.publish('wrongPwd', 1);

                    }, 300);

                } else {

                    upgr_stat = true;
                    //uexLoadingView.close();

                    payMsgToast(data.msg, 5000, 5, 0);

                }

            } else {

                upgr_stat = true;
                payMsgToast('升级会员失败', 5000, 5, 0);

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
            upgr_stat = true;
            //uexLoadingView.close();
            payMsgToast(localStorage.getItem("errorMsg"), 5000, 5, 0);

        },
    });

}
