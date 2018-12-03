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

var send_stat = true;
$('#send_fubao').click(function() {

    if (send_stat) {

        send_stat = false;
        judgRedReq();

    }

});

//判断所有必填条件
function judgRedReq() {

    var req_arr = [];
    var title = trim($('#title').val());
    if (title == '') {
        req_arr.push('广告内容');
    }

    var redmoney = localStorage.getItem("redmoney");
    if (redmoney == 0 || redmoney == '0' || redmoney == '' || redmoney == null || redmoney == undefined) {
        req_arr.push('福包金额');
    }

    var ctype = localStorage.getItem("ctype");
    if (ctype == 0 || ctype == '0') {
        req_arr.push('兑换类型');
    }

    if (req_arr.length == 0 || req_arr == '') {

        send_stat = true;

        //judgImgUpld();
        judgHasPayPwd();

    } else {

        send_stat = true;
        //openToast('请输入' + req_arr.join('、'), 3000, 5, 0);
        var chgTypIndx = req_arr.indexOf('兑换类型');
        if (req_arr.length != 1) {

            if (chgTypIndx > -1) {
                req_arr.splice(chgTypIndx, 1);
                openToast('请输入' + req_arr.join('、') + '以及勾选兑换类型', 3000, 5, 0);
            } else {
                openToast('请输入' + req_arr.join('、'), 3000, 5, 0);
            }

        } else {

            if (chgTypIndx > -1) {
                req_arr.splice(chgTypIndx, 1);
                openToast('请勾选兑换类型', 3000, 5, 0);
            } else {
                openToast('请输入' + req_arr.join('、'), 3000, 5, 0);
            }

        }

    }

}

function judgImgUpld(Pwd, Payment) {

    //uexLoadingView.openCircleLoading();

    var imgArr = [];
    //获取所有的图片路径
    $('.disp-pic').each(function(i, v) {

        if ($(this).data('pic_p')) {
            debug($(this).data('pic_p'));
            imgArr.push($(this).data('pic_p'));
        }

    });

    //openToast('正在发送福包信息...', 60000, 8, 0);

    //如果当前没有图片上传
    if (imgArr.length == 0 || imgArr == '' || imgArr == null || imgArr == undefined) {

        sendRedPacket(Pwd, imgArr, Payment);

    } else {

        doRedImgUpld(Pwd, imgArr, 0, imgArr.length, 'hbinfo', Payment);

    }

}

//img.1.循环上传图片的逻辑
function doRedImgUpld(Pwd, imgArr, i, length, fromPage, Payment) {

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
                
                    uexUploaderMgr.closeUploader(uploader);

                    responseString = JSON.parse(responseString);
                    //debug(responseString);
                    //debug(responseString.msg);
                    imgArr[i] = responseString.msg;

                    doRedImgUpld(Pwd, imgArr, i + 1, length, fromPage, Payment);

                    break;
                case 2:
                    commonAlert('图片上传失败！ ');
                    uexUploaderMgr.closeUploader(uploader);
                    send_stat = true;
                    //uexLoadingView.close();
                    break;
                }

            });

        } else {

            commonAlert('图片上传出错！ ');
            //uexLoadingView.close();
            send_stat = true;

        }

    } else {

        sendRedPacket(Pwd, imgArr, Payment);

    }

}

function sendRedPacket(Pwd, imgArr, Payment) {

    //debug(imgArr);

    //需要传入的参数
    var redmoney = localStorage.getItem("redmoney");

    //支付方式
    var title = trim($('#title').val());

    var linkto = '';
    if (linkto == '' || linkto == null || linkto == undefined) {
        linkto = '';
    }

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

    var areacode = '';
    //经纬度
    var arealimit = trim($('#arealimit').text());
    if (arealimit == '') {
        arealimit = 0;
    } else if (arealimit == '全国') {
        arealimit = 0;
    } else if (arealimit == '全市') {
        arealimit = 1;
    } else if (arealimit == '全区/县') {
        arealimit = 1;
    } else if (arealimit == '5公里') {
        arealimit = 2;
    }
    //当前默认为全国
    var sex = trim($('#sex').text());
    if (sex == '不限') {
        sex = '';
    } else if (sex == '只限男生') {
        sex = 1;
    } else if (sex == '只限女生') {
        sex = 0;
    }

    var cow = {
        Pwd : Pwd,
        Payment : Payment,
        arealimit : arealimit,
        sex : sex,
    }

    debug(cow);

    var ctype = parseInt(localStorage.getItem("ctype"));

    //debug(img);

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    //登录的逻辑
    appcan.ajax({
        url : ajaxUrl + "RedPacket/SendRedPacket",
        type : "POST",
        data : {
            redmoney : redmoney,
            Payment : Payment,
            title : title,
            img : img,
            linkto : linkto,
            areacode : areacode,
            arealimit : arealimit,
            sex : sex,
            ctype : ctype,
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

                    appcan.window.publish('doGetRedPacket', 1);
                    
                    appcan.window.publish('chgFbFriendNum', 1);

                    payMsgToast('发送福包信息成功', 5000, 5, 0);

                    setTimeout(function() {

                        appcan.window.close(-1);

                    }, 1500);

                } else if (data.code == -113 || data.code == '-113') {

                    send_stat = true;
                    //uexLoadingView.close();

                    setTimeout(function() {

                        appcan.window.publish('wrongPwd', 1);

                    }, 300);

                } else {

                    send_stat = true;
                    //uexLoadingView.close();

                    payMsgToast(data.msg, 5000, 5, 0);

                }

            } else {

                send_stat = true;
                //uexLoadingView.close();
                payMsgToast('发送福包信息失败', 5000, 5, 0);

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
            //uexLoadingView.close();
            payMsgToast(localStorage.getItem("errorMsg"), 5000, 5, 0);

        },
    });

}
