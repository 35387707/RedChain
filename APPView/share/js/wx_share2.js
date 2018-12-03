var appId = "wx4df89fa99fa5a101";
appcan.ready(function() {

    var error = uexWeiXin.registerApp(appId);
    debug(error);
    if (!error) {
        debug("注册微信成功");
    } else {
        debug("注册微信失败");
    }

});

//检查微信是否已安装
function judgWxInstall(scene) {

    var info = uexWeiXin.isWXAppInstalled();
    if (info) {

        wxShareLink(scene);

    } else {
        
        openToast("当前设备没有安装微信", 5000, 5, 0);
        
    };

}

//分享文本，支持分享到朋友圈与用户好友
function wxShareText(scene) {

    //'{"text":"这是来自AppCan平台对微信支持测试","scene":1}'

    var text = '';

    var txtJson = {
        text : text,
        scene : scene,
    }
    
    debug(txtJson);

    uexWeiXin.shareTextContent(JSON.stringify(txtJson), function(error) {

        debug(error);

        if (!error) {

            debug("分享成功");

        } else {

        }

    });

}

function wxShareImg(scene) {

    //'{"thumbImg":"res://logo_fangxing@2x.png","image":"res://logo_fangxing@2x.png","scene":1}'

    var thumbImg = '';
    var image = '';
    var title = '';

    var imgJson = {
        thumbImg : thumbImg,
        image : image,
        scene : scene,
        title : title,
    }
    
    debug(imgJson);

    uexWeiXin.shareImageContent(JSON.stringify(imgJson), function(error) {

        debug(error);

        if (!error) {

            debug("分享成功");

        } else {

            debug("分享失败");

        }

    });

}

function wxShareLink(scene) {

    //'{"thumbImg":"res://logo_fangxing@2x.png","wedpageUrl":"http://www.fbddd.com/account/regist","scene":1,"title":"你好,我是AppCan","description":"你好,我是AppCan描述"}'

    var shareLink = localStorage.getItem('wxShareLink');

    var thumbImg = 'res://logo_fangxing@2x.png';
    var wedpageUrl = shareLink;
    var title = '欢迎您加入福包多';
    var description = '福包多多，福报多多';

    var linkJson = {
        thumbImg : thumbImg,
        wedpageUrl : wedpageUrl,
        scene : scene,
        title : title,
        description : description,
    }
    
    debug(linkJson);

    uexWeiXin.shareLinkContent(JSON.stringify(linkJson), function(error) {
        
        debug(shareLink);

        debug(error);

        if (!error) {

            debug("分享成功");
            if (scene == 0) {
                openToast("分享微信好友成功", 5000, 5, 0);
            } else if (scene == 1) {
                openToast("分享朋友圈成功", 5000, 5, 0);
            }

            var sid = localStorage.getItem("sid");
            var fromPage = localStorage.getItem("fromPage");
            if (fromPage == 'fubao') {
                appcan.window.publish('fubaoShare', sid);
            } else if (fromPage == 'advert') {
                appcan.window.publish('advertShare', sid);
            }

            setTimeout(function() {

                appcan.window.closePopover('share');
                localStorage.setItem("share", 0);

            }, 2000);

        } else {

            debug("分享失败");

        }

    });

}