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

/*
 * vip.广告编辑内容
 */
function setVipInfo() {

    var UserInfo = JSON.parse(localStorage.getItem("UserInfo"));
    var Phone = UserInfo.Info.Phone;

    var UserType = '';
    var MainTainRedCount = '';

    UserType = UserInfo.Info.UserType;
    MainTainRedCount = UserInfo.Info.MainTainRedCount;

    if (UserType == 0) {
        //usertype
        $('.UserType').text('福星');
    } else if (UserType == 1) {
        $('.UserType').text('福将(商家)');
    } else if (UserType == 2) {
        $('.UserType').text('福相(代理)');
    }

    $('.UserType').data('usertype', UserType);
    $('#MainTainRedCount').text(MainTainRedCount);

}

/*
 * 获取最新升级的福包记录
 */
function getNewRedPacketByUID() {

    var ajaxUrl = localStorage.getItem("ajaxUrl");

    appcan.ajax({
        url : ajaxUrl + "RedPacket/GetNewRedPacketByUID",
        type : "POST",
        data : {
        },
        timeout : 30000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1) {
                    setVipAdvert(data);
                } else {
                    openToast(data.msg, 5000, 5, 0);
                }

            } else {

                openToast('获取最新升级的福包广告失败', 5000, 5, 0);

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
            openToast(localStorage.getItem("errorMsg"), 5000, 5, 0);

        },
    });

}

/*
 * 设置最新升级的福包数据
 */
function setVipAdvert(json) {

    var fileUrl = localStorage.getItem("fileUrl");

    var RID = json.list.RID;
    var title = json.list.title;
    var imglist = json.list.imglist;

    $('#title').text(title);
    $('#title').data('rid', json.list.RID);

    var imgArr = new Array();
    if (imglist.indexOf(';') != -1) {
        imgArr = imglist.split(";");
    } else {
        imgArr = imglist;
    }

    debug(imgArr);

    if (isJSON(imgArr)) {

        $(imgArr).each(function(i, v) {

            var imgCont = '<div data-par_s="' + i + '" class="cam-par"><div ontouchstart="appcan.touch(&#39;btn-act&#39;)" data-img_s="' + i + '" data-pic_p="' + fileUrl + v + '" data-pic_n="' + v + '" class="ub-img1 disp-pic" style="background-image:url(' + fileUrl + v + ');"></div></div>';
            //var cont = '<div class="cam-par"><img class="disp-pic" src="' + imgPath + '" /></div>';
            $('.photo-album').append(imgCont);

        });

    } else {

        var imgCont = '<div data-par_s="' + imglist + '" class="cam-par"><div ontouchstart="appcan.touch(&#39;btn-act&#39;)" data-img_s="' + imglist + '" data-pic_p="' + fileUrl + imglist + '" data-pic_n="' + imglist + '" class="ub-img1 disp-pic" style="background-image:url(' + fileUrl + imglist + ');"></div></div>';
        //var cont = '<div class="cam-par"><img class="disp-pic" src="' + imgPath + '" /></div>';
        $('.photo-album').append(imgCont);

    }

    $('.disp-pic').unbind();
    $('.disp-pic').click(function() {

        //alert($('.photo-album').children("div:last-child").index());
        var picIndx = $(this).data('pic_p');

        //查看所有相关的图片
        var picArr = [];
        $('.disp-pic').each(function(j, s) {

            //debug($(this).data('pic_p'));
            //debug($(this).data('img_s'));

            if ($(this).data('pic_p')) {
                var picSgl = {
                    src : $(this).data('pic_p'),
                    desc : $(this).data('img_s'),
                };
                picArr.push(picSgl);
            }

        });

        localStorage.setItem("imgSgl", picIndx);
        localStorage.setItem("imgArr", JSON.stringify(picArr));

        //debug(picArr);
        //var tempPath = $(this)[0].src;
        setTimeout(function() {

            //dispCurImg(picIndx ,picArr);
            openBrowser();

        }, 300);

    });

    $('.disp-pic').on('touchstart', function(e) {
        e.stopPropagation();

        var img_s = $(this).data('img_s');

        var picIndx = $(this).data('pic_p');

        timeout = setTimeout(function() {

            //alert('pare: ' + $("div[data-par_s='" + img_s + "']").html());

            uexWindow.actionSheet({
                title : "选项",
                cancel : "取消",
                buttons : "查看图片,删除图片"
            }, function(index) {

                if (index == 0) {

                    var picArr = [];
                    $('.disp-pic').each(function(j, s) {

                        //debug($(this).data('pic_p'));
                        //debug($(this).data('img_s'));

                        if ($(this).data('pic_p')) {
                            var picSgl = {
                                src : $(this).data('pic_p'),
                                desc : $(this).data('img_s'),
                            };
                            picArr.push(picSgl);
                        }

                    });

                    localStorage.setItem("imgSgl", picIndx);
                    localStorage.setItem("imgArr", JSON.stringify(picArr));

                    setTimeout(function() {

                        //dispCurImg(picIndx, picArr);
                        openBrowser();

                    }, 300);

                } else if (index == 1) {

                    $("div[data-par_s='" + img_s + "']").remove();

                }
            });

        }, 750);

    });

    $('.disp-pic').on('touchend', function(e) {
        e.stopPropagation();
        clearTimeout(timeout);
    });

}

var send_stat = true;
$('#vip_adv').click(function() {

    if (send_stat) {

        send_stat = false;
        openToast('发布广告中...', 30000, 5, 1);
        judgVipAdvRedReq();

    }

});

//判断所有必填条件
function judgVipAdvRedReq() {

    var req_arr = [];
    var title = trim($('#title').val());
    if (title == '') {
        req_arr.push('广告内容');
    }

    if (req_arr.length == 0 || req_arr == '') {

        judgVipAdvImgUpld();

    } else {

        send_stat = true;
        openToast('请输入' + req_arr.join('、'), 3000, 5, 0);

    }

}

function judgVipAdvImgUpld() {

    //uexLoadingView.openCircleLoading();
    
    var whole = [];

    var imgArr = [];
    //获取所有的图片路径
    $('.disp-pic').each(function(i, v) {

        if ($(this).data('pic_p')) {
            
            debug($(this).data('pic_p'));
            
            var tmp = $(this).data('pic_p');
            
            whole.push(tmp);
            
            if (tmp.indexOf('http://www.fbddd.com') == -1) {

                imgArr.push(tmp);

            }
            
        }

    });

    //openToast('正在发送福包信息...', 60000, 8, 0);

    //如果当前没有图片上传
    if (whole.length == 0 || whole == '' || whole == null || whole == undefined) {

        send_stat = true;
        openToast('请至少选择一张图片', 3000, 5, 0);

    } else {
        
        if (imgArr.length == 0 || imgArr == '' || imgArr == null || imgArr == undefined) {
            
            locNetImgActn(imgArr);
            
        } else {
            doVipAdvImgUpld(imgArr, 0, imgArr.length, 'vip_adv');
        }

    }

}

//img.1.循环上传图片的逻辑
function doVipAdvImgUpld(imgArr, i, length, fromPage) {

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

                    doVipAdvImgUpld(imgArr, i + 1, length, fromPage);

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

        locNetImgActn(imgArr);

    }

}

/*
 * 操作本地、网络图片路径
 */
function locNetImgActn(imgArr) {

    var net = [];

    $('.disp-pic').each(function(i, v) {

        if ($(this).data('pic_n')) {
            debug($(this).data('pic_n'));
            net.push($(this).data('pic_n'));
        }

    });

    if (net.length == 0 || net == '' || net == null || net == undefined) {

    } else {

        $(net).each(function(i, v) {

            imgArr.push(v);

        });

    }

    upldVipAdvert(imgArr);

}

/*
 * 上传福包广告
 */
function upldVipAdvert(imgArr) {
    
    debug(imgArr);

    var RID = $('#title').data('rid');

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

    //debug(img);

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    //登录的逻辑
    appcan.ajax({
        url : ajaxUrl + "RedPacket/UpdateRedPacketByRID",
        type : "POST",
        data : {
            RID : RID,
            title : title,
            img : img,
        },
        timeout : 30000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    //uexLoadingView.close();
                    
                    openToast('编辑广告信息成功', 5000, 5, 0);

                    setTimeout(function() {

                        appcan.window.close(-1);

                    }, 1500);

                } else {

                    send_stat = true;
                    //uexLoadingView.close();

                    openToast(data.msg, 5000, 5, 0);

                }

            } else {

                send_stat = true;
                //uexLoadingView.close();
                openToast('编辑广告失败', 5000, 5, 0);

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
            openToast(localStorage.getItem("errorMsg"), 5000, 5, 0);

        },
    });

}
