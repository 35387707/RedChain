
function doWSMessage(data) {
    if (data.TType > 2) { return;}
    //0文字/表情，1图片，6链接，4个人名片，5心跳包，7视频,8语音,2普通红包，9扫雷红包，3转账,10好友申请,9其他等
    
    if ((isGroup && data.Gid == gid) || (data.Sid == friendID && data.Rid == uid)) {
        switch (data.Type) {
            case 0://文字
                data.Content = js.lang.String.decodeHtml(data.Content);
                if (data.Sid == uid) {
                    $('#content').append("<div class='chat-message-wrap clearf'><img class='chat-userhead fr' src='" + HeadImg + "' /><p class='chat-message fr'><i class='r-sanjiao'></i>" + data.Content + "</p></div>");

                } else {
                    $('#content').append("<div class='chat-message-wrap clearf'><img class='chat-userhead fl' src='" + (isGroup?userHeadImg.GetHeadImg(data.Sid):fHeadImg) + "' /><p class='chat-message fl'><i class='l-sanjiao'></i>" + data.Content + "</p></div>");
                }
                break;
            case 1://照片
                if (data.Sid == uid) {
                    $('#content').append("<div class='chat-message-wrap clearf'><img class='chat-userhead fr' src='" + HeadImg + "'/><div class='photo-wrap photo-wrap-r fr'><i class='r-sanjiao'></i><a href='javascript:void(0);'><img onclick='photoClick(this)' class='chat-photo' src='" + data.Content + "'/></a></div></div>");
                } else {
                    $('#content').append("<div class='chat-message-wrap clearf'><img class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(data.Sid) : fHeadImg) + "'/><div class='photo-wrap photo-wrap-l fl'><i class='l-sanjiao'></i><a href='javascript:void(0);'><img onclick='photoClick(this)' class='chat-photo' src='" + data.Content + "'/></a></div></div>");
                }
                break;
            case 2://红包
                var c = data.Content.split("|");
                if (data.Sid == uid) {
                    $("#content").append("<div id=" + c[0] + " onclick='lookRed(\"" + c[0] + "\")'  class='chat-message-wrap clearf'><img class='chat-userhead fr' src='" + HeadImg + "' /><div class='redpocket redpocket-r fr'><div class='rp-top'><i class='rp-r-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + c[1] + "</p><p class='rp-font'>查看红包</p></div><div class='rp-bot'>玩信红包</div></div></div>");
                } else {
                    $("#content").append("<div id=" + c[0] + " onclick='redClick(\"" + c[0] + "\")' class='chat-message-wrap clearf'><img class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(data.Sid) : fHeadImg) + "' /><div class='redpocket redpocket-l fl'><div class='rp-top'><i class='rp-l-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + c[1] + "</p><p class='rp-font'>领取红包</p></div><div class='rp-bot'>玩信红包</div></div></div>");
                }
                break;
            case 9://如果是扫雷红包
                var strs = data.Content.split("|");
                if (data.Sid == uid) {
                    //$('#content').append("<div class='right'>" + data.Content + "</div>");
                    $("#content").append("<div id='" + strs[0] + "' class='chat-message-wrap clearf slfb'><img class='chat-userhead fr' src='" + HeadImg + "' /><div onclick='slhbclick(\"" + strs[0] + "\")' class='redpocket redpocket-r fr'><div class='rp-top'><i class='rp-r-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + strs[1] + "/" + strs[2] + "/" + strs[3] + "</p><p class='rp-font'>领取红包</p></div><div class='rp-bot'>扫雷发包</div></div></div>");
                } else {
                    $("#content").append("<div id='" + strs[0] + "' class='chat-message-wrap clearf slfb'><img class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(data.Sid) : fHeadImg) + "' /><div onclick='slhbclick(\"" + strs[0] + "\")' class='redpocket redpocket-l fl'><div class='rp-top'><i class='rp-l-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + strs[1] + "/" + strs[2] + "/" + strs[3] + "</p><p class='rp-font'>领取红包</p></div><div class='rp-bot'>扫雷发包</div></div></div>");
                    //$('#content').append("<div class='left'>" + data.Content + "</div>");
                }
                break;
        }
        toButtom();
    }
}
function toButtom() {
    var $list = $(".chat-window-wrap");
    $list[0].scrollTop = $list[0].scrollHeight - window.innerHeight;
}
//查看红包
function lookRed(mid) {
    location.href = "/Chat/RedDetail/" + mid;
}
//往下滑动加载数据
function doMessageRecoed(row) {
    switch (row.MType) {
        case 0:
            //alert(row.Content);
            row.Content = js.lang.String.decodeHtml(row.Content);
            //alert(row.Content);
            if (row.FUID == uid) {
                $('#content').prepend("<div class='chat-message-wrap clearf'><img class='chat-userhead fr' src='" + HeadImg + "' /><p class='chat-message fr'><i class='r-sanjiao'></i>" + row.Content + "</p></div>");

            } else {
                $('#content').prepend("<div class='chat-message-wrap clearf'><img class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "' /><p class='chat-message fl'><i class='l-sanjiao'></i>" + row.Content + "</p></div>");
            }
            break;
        case 1://照片
            if (row.FUID == uid) {
                $('#content').prepend("<div class='chat-message-wrap clearf'><img class='chat-userhead fr' src='" + HeadImg + "'/><div class='photo-wrap photo-wrap-r fr'><i class='r-sanjiao'></i><a href='javascript:void(0);'><img onclick='photoClick(this)' class='chat-photo' src='" + row.Content + "'/></a></div></div>");
            } else {
                $('#content').prepend("<div class='chat-message-wrap clearf'><img class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "'/><div class='photo-wrap photo-wrap-l fl'><i class='l-sanjiao'></i><a href='javascript:void(0);'><img onclick='photoClick(this)' class='chat-photo' src='" + row.Content + "'/></a></div></div>");
            }
            break;
        case 2://红包
            var c = row.Content.split("|");
            if (row.FUID == uid) {
                $("#content").prepend("<div id=" + row.MID + " onclick='lookRed(\"" + row.MID + "\")'  class='chat-message-wrap clearf'><img class='chat-userhead fr' src='" + HeadImg + "' /><div class='redpocket redpocket-r fr'><div class='rp-top'><i class='rp-r-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + c[1] + "</p><p class='rp-font'>查看红包</p></div><div class='rp-bot'>玩信红包</div></div></div>");
            } else {
                $("#content").prepend("<div id=" + row.MID + " onclick='redClick(\"" + row.MID + "\")' class='chat-message-wrap clearf'><img class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "' /><div class='redpocket redpocket-l fl'><div class='rp-top'><i class='rp-l-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + c[1] + "</p><p class='rp-font'>领取红包</p></div><div class='rp-bot'>玩信红包</div></div></div>");
            }
            break;
        case 9://如果是扫雷红包
            var strs = row.Content.split("|");
            if (row.FUID == uid) {
                //$('#content').append("<div class='right'>" + data.Content + "</div>");
                $("#content").prepend("<div id='" + strs[0] + "' class='chat-message-wrap clearf slfb'><img class='chat-userhead fr' src='" + HeadImg + "' /><div onclick='slhbclick(\"" + strs[0] + "\")' class='redpocket redpocket-r fr'><div class='rp-top'><i class='rp-r-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + strs[1] + "/" + strs[2] + "/" + strs[3] + "</p><p class='rp-font'>领取红包</p></div><div class='rp-bot'>扫雷发包</div></div></div>");
            } else {
                $("#content").prepend("<div id='" + strs[0] + "' class='chat-message-wrap clearf slfb'><img class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "' /><div onclick='slhbclick(\"" + strs[0] + "\")' class='redpocket redpocket-l fl'><div class='rp-top'><i class='rp-l-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + strs[1] + "/" + strs[2] + "/" + strs[3] + "</p><p class='rp-font'>领取红包</p></div><div class='rp-bot'>扫雷发包</div></div></div>");
                //$('#content').append("<div class='left'>" + data.Content + "</div>");
            }
            break;
        case 3://转账
            var strs = row.Content.split("|");
            if (row.FUID == uid) {
                $("#content").prepend("<div class='chat-message-wrap clearf'><img class='chat-userhead fr' src='" + HeadImg + "'/><div class='redpocket redpocket-r fr'><div class='rp-top'><i class='rp-r-sanjiao'></i><i class='rp-transfericon'></i><p class='rp-num'>转账给" + friendName + "</p><p class='rp-font'>&yen;" + strs[1] + "</p></div><div class='rp-bot'>玩信转账</div></div></div>");
                //$('#content').prepend("<div class='chat-message-wrap clearf'><img class='chat-userhead fr' src='/img/chat/head4.png' /><p class='chat-message fr'><i class='r-sanjiao'></i>转账给"+friendName +"￥"+strs[1] + "元</p></div>");

            } else {
                $("#content").prepend("<div class='chat-message-wrap clearf'><img class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "'/><div class='redpocket redpocket-l fl'><div class='rp-top'><i class='rp-l-sanjiao'></i><i class='rp-transfericon'></i><p class='rp-num'>收到" + friendName + "转账</p><p class='rp-font'>&yen;" + strs[1] + "</p></div><div class='rp-bot'>玩信转账</div></div></div>");
                //$('#content').prepend("<div class='chat-message-wrap clearf'><img class='chat-userhead fl' src='/img/chat/head4.png' /><p class='chat-message fl'><i class='l-sanjiao'></i>收到"+friendName+"转账 ￥" + strs[1] + "元</p></div><p class='chat-datetime'>" + row.CreateTime + "</p>");
            }
            break;
        default:
            break;
    }
}

//往上滑加载数据
function doMessage(row) {
    switch (row.MType) {
        case 0:
            //alert(row.Content);
            row.Content = js.lang.String.decodeHtml(row.Content);
            //alert(row.Content);
            if (row.FUID == uid) {
                $('#content').append("<div class='chat-message-wrap clearf'><img class='chat-userhead fr' src='" + HeadImg + "' /><p class='chat-message fr'><i class='r-sanjiao'></i>" + row.Content + "</p></div>");

            } else {
                $('#content').append("<div class='chat-message-wrap clearf'><img class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "' /><p class='chat-message fl'><i class='l-sanjiao'></i>" + row.Content + "</p></div><p class='chat-datetime'>" + row.CreateTime + "</p>");
            }
            break;
        case 2://红包
            var c = row.Content.split("|");
            if (row.FUID == uid) {
                $("#content").append("<div id=" + row.MID + " onclick='lookRed(\"" + row.MID + "\")'  class='chat-message-wrap clearf'><img class='chat-userhead fr' src='" + HeadImg + "' /><div class='redpocket redpocket-r fr'><div class='rp-top'><i class='rp-r-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + c[1] + "</p><p class='rp-font'>查看红包</p></div><div class='rp-bot'>玩信红包</div></div></div>");
            } else {
                $("#content").append("<div id=" + row.MID + " onclick='redClick(\"" + row.MID + "\")' class='chat-message-wrap clearf'><img class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "' /><div class='redpocket redpocket-l fl'><div class='rp-top'><i class='rp-l-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + c[1] + "</p><p class='rp-font'>领取红包</p></div><div class='rp-bot'>玩信红包</div></div></div>");
            }
            break;
        case 9://如果是扫雷红包
            var strs = row.Content.split("|");
            if (row.FUID == uid) {
                //$('#content').append("<div class='right'>" + data.Content + "</div>");
                $("#content").append("<div id='" + strs[0] + "' class='chat-message-wrap clearf slfb'><img class='chat-userhead fr' src='" + HeadImg + "' /><div onclick='slhbclick(\"" + strs[0] + "\")' class='redpocket redpocket-r fr'><div class='rp-top'><i class='rp-r-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + strs[1] + "/" + strs[2] + "/" + strs[3] + "</p><p class='rp-font'>领取红包</p></div><div class='rp-bot'>扫雷发包</div></div></div>");
            } else {
                $("#content").append("<div id='" + strs[0] + "' class='chat-message-wrap clearf slfb'><img class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "' /><div onclick='slhbclick(\"" + strs[0] + "\")' class='redpocket redpocket-l fl'><div class='rp-top'><i class='rp-l-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + strs[1] + "/" + strs[2] + "/" + strs[3] + "</p><p class='rp-font'>领取红包</p></div><div class='rp-bot'>扫雷发包</div></div></div>");
                //$('#content').append("<div class='left'>" + data.Content + "</div>");
            }
            break;
        case 3://转账
            var strs = row.Content.split("|");
            if (row.FUID == uid) {
                $("#content").append("<div class='chat-message-wrap clearf'><img class='chat-userhead fr' src='" + HeadImg + "'/><div class='redpocket redpocket-r fr'><div class='rp-top'><i class='rp-r-sanjiao'></i><i class='rp-transfericon'></i><p class='rp-num'>转账给" + friendName + "</p><p class='rp-font'>&yen;" + strs[1] + "</p></div><div class='rp-bot'>玩信转账</div></div></div>");
                //$('#content').prepend("<div class='chat-message-wrap clearf'><img class='chat-userhead fr' src='/img/chat/head4.png' /><p class='chat-message fr'><i class='r-sanjiao'></i>转账给"+friendName +"￥"+strs[1] + "元</p></div>");

            } else {
                $("#content").append("<div class='chat-message-wrap clearf'><img class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "'/><div class='redpocket redpocket-l fl'><div class='rp-top'><i class='rp-l-sanjiao'></i><i class='rp-transfericon'></i><p class='rp-num'>收到" + friendName + "转账</p><p class='rp-font'>&yen;" + strs[1] + "</p></div><div class='rp-bot'>玩信转账</div></div></div>");
                //$('#content').prepend("<div class='chat-message-wrap clearf'><img class='chat-userhead fl' src='/img/chat/head4.png' /><p class='chat-message fl'><i class='l-sanjiao'></i>收到"+friendName+"转账 ￥" + strs[1] + "元</p></div><p class='chat-datetime'>" + row.CreateTime + "</p>");
            }
            break;
        default:
            break;
    }
}
//上传照片
function ImgUpload(file) {
    if (file && file.files[0]) {
        if (file.files[0].type != "image/jpeg") {
            alert("文件类型不正确！");
            return;
        }
        var data = new FormData();
        data.append("file", file.files[0], file.files[0].name);
        $.ajax({
            url: "/File/ImgUpload",
            type: "POST",
            data: data,
            processData: false,
            contentType: false,
            success: function (respdata) {
                if (respdata.code == 1) {
                    sendPhoto(respdata.msg);
                } else {
                    alert(respdata.msg);
                }
                
            },
            error: function (data) {
                alert("错误");
            }
        });
    }
}
//图片点击事件
function photoClick(e) {
    $("#bigimg").attr("src", e.src);
    $("#bigimg-wrap").fadeIn(100);

}
function getWS(token) {
    //console.log(ws);
    alert('ws://' + window.location.hostname + ':' + window.location.port + '/api/IM?id=' + uid + '&token=' + token);
    ws = new WebSocket('ws://' + window.location.hostname + ':' + window.location.port + '/api/IM?id='+uid+ '&token='+token);
    ws.onopen = function () {
        console.log(ws.readyState);
        console.log("连接成功");

        //$('#msg').append('<p>已经连接</p>');
    }
    ws.onmessage = function (evt) {
       // console.log(evt.data);
        var data = JSON.parse(evt.data);
        doWSMessage(data);
        //$("#content").append("<p class='chat-datetime'>" + data.Time + "</p>");
        // $('#content').append(evt.data);
    }
    ws.onerror = function (evt) {
        alert("连接失败");
        return;
        getWS(token);
        console.log(evt);
        console.log("连接出错");
        //$('#msg').append('<p>' + JSON.stringify(evt) + '</p>');
    }
    ws.onclose = function () {
        return;
        getWS(token);
        console.log("连接已关闭");
        //$('#msg').append('<p>已经关闭</p>');
    }
}
$(function () {
    $("body").on("touchstart", function (e) {
        // e.preventDefault();
        startX = e.originalEvent.changedTouches[0].pageX;
        startY = e.originalEvent.changedTouches[0].pageY;

    });
    $("body").on("touchmove", function (e) {
        endX = e.originalEvent.changedTouches[0].pageX;
        endY = e.originalEvent.changedTouches[0].pageY;
        var y = endY - startY;
        if (Math.abs(y) > 100) {
            
            var $list = $(".chat-window-wrap");
            var oldheight = $list[0].scrollHeight;
            if (y > 0) {
                //console.log($list[0].scrollTop + "---总高度:" + $list[0].scrollHeight);
                if (syncTopStatus == 1 && $list[0].scrollTop == 0) {
                    syncTopStatus = 0;
                    getMessageRecord(function () {
                        syncTopStatus = 1;
                        $list[0].scrollTop = $list[0].scrollHeight - oldheight;
                    });

                }
                //alert("向下");
            } else {
                //console.log($list.scrollTop() + window.innerHeight+"----"+$list[0].scrollHeight);
                if (syncStatus == 1 && $list.scrollTop() + window.innerHeight == $list[0].scrollHeight) {
                    syncStatus = 0;
                    getMessageRecordButtom(function () {
                        syncStatus = 1;
                    });
                }
                //alert("向上");
            }
        }

    });
});