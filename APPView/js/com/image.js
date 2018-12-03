/*
* 离线操作
* 一开始没有的记录中插入图片信息
*/
//img.1.循环上传图片的逻辑
function doImgUpload(imgArr, i, length, fromPage) {

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
                    
                    if (fromPage == 'chat') {
                        
                        //isrtMsgBegin(uid, friend_uid, uid, 1, 2, 1, msg, 'chat');
                        //uexUploaderMgr.closeUploader(uploader);
                        //chatImgMsgEditor(Api, TType, Rid, Gid, Type, Status, Content, imgArr, i, length, fromPage)
                        chatImgMsgEditor('chat', 1, localStorage.getItem("friend_uid"), '', 1, 1, responseString.msg, imgArr, i, length, fromPage);
                        
                    } else {
                        
                        doImgUpload(imgArr, i + 1, length, fromPage);
                        
                    }

                    break;
                case 2:
                    uexUploaderMgr.closeUploader(uploader);
                    commonAlert('照片上传失败！ ');
                    break;
                }

            });

        } else {

            commonAlert('照片上传出错！ ');

        }

    } else {

        if (fromPage == 'hbinfo') {

            //debug('开始正式提交红包数据')
            closeToast();
            openToast('正在提交红包数据', 60000, 5, 1);
            sendRedPacket(imgArr);

        } else if (fromPage == 'personal_data') {

            updateHeadImg(imgArr[0]);

        } else if (fromPage == 'chat') {

            //去到消息页面刷新记录
            appcan.window.publish('refreshMsg', 1);

        }

    }

}

//img.3.点击
function doImgClick() {

    $('.disp-pic').unbind();
    $('.disp-pic').click(function() {

        var tempPath = $(this)[0].src;
        //debug(tempPath);
        dispCurImg(tempPath);

    });

}
