/*
 * 接口.41.获取广告/商圈类型
 */
function findTypeId(name) {
    
    var itid = '';
    
    var json = JSON.parse(localStorage.getItem("tabJson"));
    $(json.list).each(function(i, v){
       
       if (v.Name == name) {
           itid = v.ITID;
       }
        
    });
    
    //debug(itid);
    
    return itid;

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

var send_stat = true;
$('#send_info').click(function() {

    if (send_stat) {

        send_stat = false;
        judgBizReqCont();

    }

});

//判断所有必填条件
function judgBizReqCont() {

    var req_arr = [];
    var title = trim($('#title').val());
    if (title == '') {
        req_arr.push('广告内容');
    }

    var itid = localStorage.getItem("itid");
    if (itid == '' || itid == null || itid == undefined) {
        req_arr.push('商圈类型');
    }

    if (req_arr.length == 0 || req_arr == '') {

        judgBizImgUpld();

    } else {

        send_stat = true;
        openToast('请输入' + req_arr.join('、'), 3000, 5, 0);

    }

}

function judgBizImgUpld() {
    
    debug('进入图片判断');

    var imgArr = [];
    //获取所有的图片路径
    $('.disp-pic').each(function(i, v) {

        if ($(this).data('pic_p')) {
            debug($(this).data('pic_p'));
            imgArr.push($(this).data('pic_p'));
        }

    });
    
    debug(imgArr);

    //如果当前没有图片上传
    if (imgArr.length == 0) {

        //sendRedPacket(imgArr);
        send_stat = true;
        openToast('请至少选择一张图片', 3000, 5, 0);

    } else {
        
        uexLoadingView.openCircleLoading();
        doBizImgUpld(imgArr, 0, imgArr.length, 'pub');

    }

}

//img.1.循环上传图片的逻辑
function doBizImgUpld(imgArr, i, length, fromPage) {

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

                    doBizImgUpld(imgArr, i + 1, length, fromPage);

                    break;
                case 2:
                    uexUploaderMgr.closeUploader(uploader);
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

        publishInfomation(imgArr);

    }

}

/*
 * 接口.42.发广告
 */
function publishInfomation(imgArr) {
    
    //需要传入的参数
    var type = parseInt(localStorage.getItem("itid"));

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
    
    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url: ajaxUrl + "Infomation/Publish",
        type: "POST",
        data: {
            type : type,
            title : title,
            img : img,
            linkto : linkto,
        },
        timeout: 10000,
        dataType: "json",
        success: function (data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")
            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {
                    
                    uexLoadingView.close();
                    
                    appcan.window.publish('srchNewBiz', type);

                    openToast('发布商圈信息成功', 3000, 5, 0);

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
                openToast('发布商圈广告失败', 5000, 5, 0);

            }

        },
        error: function (xhr, errorType, error, msg) {

            var debugMsg = {
                xhr: xhr,
                errorType: errorType,
                error: error,
                msg: msg,
            }
            debug(debugMsg);
            send_stat = true;
            uexLoadingView.close();
            openToast(localStorage.getItem("errorMsg"), 5000, 5, 0);

        },
        
    });

}
