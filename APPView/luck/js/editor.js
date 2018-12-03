//2.0.打开底部导航栏
function openEditorActn(fromPage) {

    var max = 1;

    uexWindow.actionSheet({
        title : "更换奖品图片",
        cancel : "取消",
        buttons : "拍照,从手机相册选中"
    }, function(index) {
        if (index == 0) {
            openCamera(fromPage);
        } else if (index == 1) {
            toImgPicker(max, fromPage);
        }
    });

};

$('.luck-cond').click(function() {

    uexWindow.actionSheet({
        title : "开奖条件",
        cancel : "取消",
        buttons : "按时间自动开奖,按人数自动开奖,手动开奖"
    }, function(index) {
        if (index == 0) {

            $('#luck_cond').text('按时间自动开奖');

        } else if (index == 1) {

            $('#luck_cond').text('按人数自动开奖');

        } else if (index == 2) {

            $('#luck_cond').text('手动开奖');

        }
    });

});

$(".luck-name").click(function() {

    var orgVal = trim($("#luck_name").text());
    //获取对应房型的id
    uexWindow.prompt({
        title : "奖品名称",
        message : "请输入奖品名称",
        defaultValue : orgVal,
        buttonLabels : "确认,取消",
        mode : 0,
    }, function(index, data) {

        if (index == 0) {

            //data
            if (data == null || data == '' || data == undefined) {

            } else {
                $("#luck_name").text(data);
            }

        }

    });

});

$(".luck-num").click(function() {

    var orgVal = trim($("#luck_num").text());
    //获取对应房型的id
    uexWindow.prompt({
        title : "奖品数量",
        message : "请输入奖品数量",
        defaultValue : orgVal,
        buttonLabels : "确认,取消",
        hint : "请输入数字",
        mode : 1,
    }, function(index, data) {

        if (index == 0) {

            //data
            if (data == null || data == '' || data == undefined) {

            } else {

                if (isIntNumber(data)) {
                    $("#luck_num").text(data);
                } else {
                    openToast('请输入整数', 3000, 5, 0);
                }

            }

        }

    });

});

$('.luck-date').click(function() {

    var Date_value = new Date().format("yyyy,MM,dd");
    var data_arry = Date_value.split(",");
    //alert(data_arry[0] + ":" + data_arry[1] + ":" + data_arry[2])
    var params = {
        initDate : {
            year : data_arry[0],
            month : data_arry[1],
            day : data_arry[2]
        },
        minDate : {
            limitType : 1,
            data : {
                day : 0,
            }
        },
    }
    var data = JSON.stringify(params);

    //最终的时间回调函数
    var callback = function(data) {
        //$("#aa").val(data.year + "年" + data.month + "月" + data.day + "日");
        $("#luck_date").text(data.year + "年" + data.month + "月" + data.day + "日");
        //return data.year + "-" + data.month + "-" + data.day;
    }
    uexControl.openDatePickerWithConfig(data, callback);

});

$('.luck-time').click(function() {

    var luck_time = trim($("#luck_time").text());
    var hour = 0;
    var minute = 0;
    if (luck_time == null || luck_time == '' || luck_time == undefined) {
        hour = new Date().format("hh");
        minute = new Date().format("mm");
    } else {
        hour = luck_time.substring(0, luck_time.indexOf('时'));
        minute = luck_time.substring(luck_time.indexOf('时') + 1, luck_time.lastIndexOf('分'));
    }

    hour = parseInt(hour);
    minute = parseInt(minute);

    uexControl.openTimePicker(hour, minute, function(data) {
        $("#luck_time").text(data.hour + "时" + data.minute + "分");
    });

});

var send_stat = true;
$('#luck_start').click(function() {

    if (send_stat) {

        send_stat = false;
        judgReqCont();

    }

    /*
     localStorage.setItem("pay", 1);

     var bodyWidth = localStorage.getItem("bodyWidth");
     var bodyHeight = localStorage.getItem("bodyHeight");
     appcan.window.openPopover({
     name : 'pay',
     dataType : 0,
     url : "../pop/pay.html",
     top : 0,
     left : 0,
     width : bodyWidth,
     height : bodyHeight,
     });
     */

});

//判断所有必填条件
function judgReqCont() {

    var req_arr = [];
    var title = $('#title').val();
    if (title == '' || title == null || title == undefined) {
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

        judgImgUpld();

    } else {

        send_stat = true;
        openToast('请输入' + req_arr.join('、'), 3000, 5, 0);

    }

}

function judgImgUpld() {

    uexLoadingView.openCircleLoading();

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

        sendRedPacket(imgArr);

    } else {

        doRedImgUpld(imgArr, 0, imgArr.length, 'hbinfo');

    }

}

//img.1.循环上传图片的逻辑
function doRedImgUpld(imgArr, i, length, fromPage) {

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

                    doRedImgUpld(imgArr, i + 1, length, fromPage);

                    break;
                case 2:
                    commonAlert('图片上传失败！ ');
                    send_stat = true;
                    uexLoadingView.close();
                    break;
                }

            });

        } else {

            commonAlert('图片上传出错！ ');
            uexLoadingView.close();
            send_stat = true;

        }

    } else {

        sendRedPacket(imgArr);

    }

}

function sendRedPacket(imgArr) {

    //debug(imgArr);

    //需要传入的参数
    var redmoney = localStorage.getItem("redmoney");

    var Payment = 0;
    //支付方式
    var title = $('#title').val();
    if (title == '' || title == null || title == undefined) {
        title = '';
    }

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
    var arealimit = 0;
    //当前默认为全国

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
            ctype : ctype,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    uexLoadingView.close();

                    appcan.window.publish('doGetRedPacket', 1);

                    openToast('发送福包信息成功', 3000, 5, 0);

                    setTimeout(function() {

                        appcan.window.close(-1);

                    }, 1500);

                } else {

                    send_stat = true;
                    uexLoadingView.close();
                    openToast(data.msg, 5000, 5, 0);

                }

            } else {

                send_stat = true;
                uexLoadingView.close();
                openToast('发送福包信息失败', 5000, 5, 0);

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
            uexLoadingView.close();
            openToast(localStorage.getItem("errorMsg"), 5000, 5, 0);

        },
    });

}
