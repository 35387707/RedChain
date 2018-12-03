var otherMsgCount = 0;

var CurrentDate = new Date();//当前日期
var MinDate = null;
function doWSMessage(data) {
    if (data.TType > 2) { return;}
    //0文字/表情，1图片，6链接，4个人名片，5心跳包，7视频,8语音,2普通红包，9扫雷红包，3转账,10好友申请,9其他等
    if ((isGroup && data.Gid == gid) || ((data.Sid == friendID && data.Rid == uid) || (data.Sid == uid && data.Rid == friendID))) {
        switch (data.Type) {
            case -2:
                console.log(data);
                break;
            case -1://撤销消息
                $("#" + data.Content).remove();
                break;
            case 0://文字
                data.Content = js.lang.String.decodeHtml(data.Content);
                if (data.Sid == uid) {
                    $('#content').append("<div class='chat-message-wrap clearf'><img class='chat-userhead fr' src='" + HeadImg + "' /><p class='chat-message fr'><i class='r-sanjiao'></i>" + data.Content + "</p></div>");

                } else {
                    $('#content').append("<div class='chat-message-wrap clearf'><img class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(data.Sid) : fHeadImg) + "' /><p class='chat-message fl'><i class='l-sanjiao'></i>" + data.Content + "</p></div>");
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
                    $("#content").append("<div id=" + c[0] + " onclick='lookRed(\"" + c[0] + "\")'  class='chat-message-wrap clearf'><img class='chat-userhead fr'  src='" + HeadImg + "' /><div class='redpocket redpocket-r fr'><div class='rp-top'><i class='rp-r-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + c[1] + "</p><p class='rp-font'>查看红包</p></div><div class='rp-bot'>玩信红包</div></div></div>");
                } else {
                    $("#content").append("<div id=" + c[0] + " onclick='redClick(\"" + c[0] + "\")' class='chat-message-wrap clearf'><img class='chat-userhead fl' data-sid='" + data.Sid + "' src='" + (isGroup ? userHeadImg.GetHeadImg(data.Sid) : fHeadImg) + "' /><div class='redpocket redpocket-l fl'><div class='rp-top'><i class='rp-l-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + c[1] + "</p><p class='rp-font'>领取红包</p></div><div class='rp-bot'>玩信红包</div></div></div>");
                }
                break;
            case 4:
                var str = row.Content.split("|");
                if (data.Sid == uid) {
                    $("#content").append("<div id=" + str[4] + " onclick='CardClick(\"" + str[0] + "\")'  class='chat-message-wrap clearf'><img class='chat-userhead fr' src='" + HeadImg + "'/><div class='calling-card fr'><i class='r-sanjiao'></i><div class='cc-wrap'><div class='cc-top'><img class='cc-top-img' src='" + str[3] + "' alt='' /><p class='cc-top-name'>" + str[1] + "</p><p class='cc-top-nickname'>" + str[2] + "</p></div><p class='cc-bot'>个人名片</p></div></div></div>");
                } else {
                    $("#content").append("<div id=" + str[4] + " onclick='CardClick(\"" + str[0] + "\")'  class='chat-message-wrap clearf'><img class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(data.Sid) : fHeadImg) + "'/><div class='calling-card fl'><i class='l-sanjiao'></i><div class='cc-wrap'><div class='cc-top'><img class='cc-top-img' src='" + str[3] + "' alt='' /><p class='cc-top-name'>" + str[1] + "</p><p class='cc-top-nickname'>" + str[2] + "</p></div><p class='cc-bot'>个人名片</p></div></div></div>");
                }
                break;
            case 9://如果是扫雷红包
                var strs = data.Content.split("|");
                if (data.Sid == uid) {
                    //$('#content').append("<div class='right'>" + data.Content + "</div>");
                    $("#content").append("<div id='" + strs[0] + "' class='chat-message-wrap clearf slfb'><img class='chat-userhead fr' src='" + HeadImg + "' /><div onclick='slhbclick(\"" + strs[0] + "\")' class='redpocket redpocket-r fr'><div class='rp-top'><i class='rp-r-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + strs[1] + "/" + strs[2] + "/" + strs[3] + "</p><p class='rp-font'>领取红包</p></div><div class='rp-bot'>扫雷发包</div></div></div>");
                } else {
                    $("#content").append("<div id='" + strs[0] + "' class='chat-message-wrap clearf slfb'><img class='chat-userhead fl' data-sid='"+data.Sid+"' src='" + (isGroup ? userHeadImg.GetHeadImg(data.Sid) : fHeadImg) + "' /><div onclick='slhbclick(\"" + strs[0] + "\")' class='redpocket redpocket-l fl'><div class='rp-top'><i class='rp-l-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + strs[1] + "/" + strs[2] + "/" + strs[3] + "</p><p class='rp-font'>领取红包</p></div><div class='rp-bot'>扫雷发包</div></div></div>");
                    //$('#content').append("<div class='left'>" + data.Content + "</div>");
                }
                break;
        }
        toButtom();
    } else {
        $("#otherMsgCount").text(++otherMsgCount);
        
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
function GoFriend(id) {
    location.href = "/Friend/FriendInfo/" + id;
}
//往下滑动时间展示
function showDate(row) {
    var tempdate = row.CreateTime.split(/[- : \/]/);
    //var date = new Date(row.CreateTime);
    var date = new Date(tempdate[0], tempdate[1]-1, tempdate[2], tempdate[3], tempdate[4]);
    if (MinDate == null) {
        MinDate = date;
    }
    // console.log(date.getDate() + "==" + date.getMonth() + "===" + date.getFullYear());
    var eqmonth = date.getMonth() == MinDate.getMonth();
    var eqday = date.getDate() == MinDate.getDate();
    var eqhour = date.getHours() == MinDate.getHours();
    if (eqmonth && eqday && eqhour && date.getMinutes() < MinDate.getMinutes()) {
        $("#content").prepend("<p class='chat-datetime'>" + dateformart("yyyy-MM-dd hh:mm", MinDate) + "</p>");
        MinDate = date;
    }else if (eqmonth && eqday && date.getHours() < MinDate.getHours()) {
        $("#content").prepend("<p class='chat-datetime'>" + dateformart("yyyy-MM-dd hh:mm", MinDate) + "</p>");
        MinDate = date;
    }
    else if (eqmonth && date.getDate() < MinDate.getDate()) {
        $("#content").prepend("<p class='chat-datetime'>" + dateformart("yyyy-MM-dd hh:mm", MinDate) + "</p>");
        MinDate = date;
    } else if (date.getMonth() < MinDate.getMonth()) {
        $("#content").prepend("<p class='chat-datetime'>" + dateformart("yyyy-MM-dd hh:mm", MinDate) + "</p>");
        MinDate = date;
    } else if (date.getFullYear() < MinDate.getFullYear()) {
        $("#content").prepend("<p class='chat-datetime'>" + dateformart("yyyy-MM-dd hh:mm", MinDate) + "</p>");
        MinDate = date;
    }
}
/*
function showDate(row) {
    var date = new Date(row.CreateTime);
    
    if (MinDate == null) {
        MinDate = date;
    }
   // console.log(date.getDate() + "==" + date.getMonth() + "===" + date.getFullYear());
    if ((date.getMonth() == MinDate.getMonth()) && date.getDate() < MinDate.getDate()) {
        $("#content").prepend("<p class='chat-datetime'>" + dateformart("yyyy-MM-dd hh:mm", MinDate) + "</p>");
        MinDate = date;
    } else if (date.getMonth() < MinDate.getMonth()) {
        $("#content").prepend("<p class='chat-datetime'>" + dateformart("yyyy-MM-dd hh:mm", MinDate) + "</p>");
        MinDate = date;
    } else if (date.getFullYear() < MinDate.getFullYear()) {
        $("#content").prepend("<p class='chat-datetime'>" + dateformart("yyyy-MM-dd hh:mm", MinDate) + "</p>");
        MinDate = date;
    }

}*/
//往上滑动时间展示
function showDateUp(row) {
    var tempdate = row.CreateTime.split(/[- : \/]/);
    //var date = new Date(row.CreateTime);
    var date = new Date(tempdate[0], tempdate[1]-1, tempdate[2], tempdate[3], tempdate[4]);
    if (MinDate == null) {
        MinDate = date;
        $("#content").append("<p class='chat-datetime'>" + dateformart("yyyy-MM-dd hh:mm", MinDate) + "</p>");
    }
    //console.log(date.getDate() + "==" + date.getMonth() + "===" + date.getFullYear()+"时间变大");
    //console.log(date);
    
    var eqmonth = date.getMonth() == MinDate.getMonth();
    var eqday = date.getDate() == MinDate.getDate();
    var eqhour = date.getHours() == MinDate.getHours();
    if (eqmonth && eqday && eqhour && date.getMinutes() > MinDate.getMinutes()) {
        MinDate = date;
        $("#content").append("<p class='chat-datetime'>" + dateformart("yyyy-MM-dd hh:mm", MinDate) + "</p>");
    }else if (eqmonth && eqday && date.getHours() > MinDate.getHours()) {
        MinDate = date;
        $("#content").append("<p class='chat-datetime'>" + dateformart("yyyy-MM-dd hh:mm", MinDate) + "</p>");
    } else if (eqmonth&& date.getDate() > MinDate.getDate()) {
        MinDate = date;
        $("#content").append("<p class='chat-datetime'>" + dateformart("yyyy-MM-dd hh:mm", MinDate) + "</p>");

    } else if (date.getMonth() > MinDate.getMonth()) {
        MinDate = date;
        $("#content").append("<p class='chat-datetime'>" + dateformart("yyyy-MM-dd hh:mm", MinDate) + "</p>");

    } else if (date.getFullYear() > MinDate.getFullYear()) {
        MinDate = date;
        $("#content").append("<p class='chat-datetime'>" + dateformart("yyyy-MM-dd hh:mm", MinDate) + "</p>");

    }
}
/*function showDateUp(row) {
    var date = new Date(row.CreateTime);
    if (MinDate == null) {
        MinDate = date;
        $("#content").append("<p class='chat-datetime'>" + dateformart("yyyy-MM-dd hh:mm", MinDate) + "</p>");
    }
    //console.log(date.getDate() + "==" + date.getMonth() + "===" + date.getFullYear()+"时间变大");
    if ((date.getMonth() == MinDate.getMonth()) && date.getDate() > MinDate.getDate()) {
        MinDate = date;
        $("#content").append("<p class='chat-datetime'>" + dateformart("yyyy-MM-dd hh:mm", MinDate) + "</p>");
        
    } else if (date.getMonth() > MinDate.getMonth()) {
        MinDate = date;
        $("#content").append("<p class='chat-datetime'>" + dateformart("yyyy-MM-dd hh:mm", MinDate) + "</p>");
        
    } else if (date.getFullYear() > MinDate.getFullYear()) {
        MinDate = date;
        $("#content").append("<p class='chat-datetime'>" + dateformart("yyyy-MM-dd hh:mm", MinDate) + "</p>");
        
    }

}*/
//往下滑动加载数据 默认加载
function doMessageRecoed(row) {
    
    if (row.TType == 3) {
        DoSystemMsg(row);
        return;
    }
    
    
    
    switch (row.MType) {
        case -1://撤销消息
            $("#" + row.Content).remove();
            break;
        case 0:
            //alert(row.Content);
            row.Content = js.lang.String.decodeHtml(row.Content);
            //alert(row.Content);
            if (row.FUID == uid) {
                $('#content').prepend("<div id=" + row.MID + " data-mtype='0' class='chat-message-wrap clearf' ontouchstart='msgmousedown(this)' ontouchend='msgmouseup(this)'><img class='chat-userhead fr' src='" + HeadImg + "' /><p class='chat-message fr'><i class='r-sanjiao'></i>" + row.Content + "</p></div>");

            } else {
                $('#content').prepend("<div id=" + row.MID + " data-mtype='0' class='chat-message-wrap clearf' ontouchstart='msgmousedown(this)' ontouchend='msgmouseup(this)'><img onclick=\"GoFriend('" + row.FUID + "')\" class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "' /><p class='chat-message fl'><i class='l-sanjiao'></i>" + row.Content + "</p></div>");
            }
            showDate(row);
            break;
        case 1://照片
            if (row.FUID == uid) {
                $('#content').prepend("<div id=" + row.MID + " data-mtype='1' class='chat-message-wrap clearf' ontouchstart='msgmousedown(this)' ontouchend='msgmouseup(this)'><img class='chat-userhead fr' src='" + HeadImg + "'/><div class='photo-wrap photo-wrap-r fr'><i class='r-sanjiao'></i><a href='javascript:void(0);'><img onclick='photoClick(this)' class='chat-photo' src='" + row.Content + "'/></a></div></div>");
            } else {
                $('#content').prepend("<div id=" + row.MID + " data-mtype='1' class='chat-message-wrap clearf' ontouchstart='msgmousedown(this)' ontouchend='msgmouseup(this)'><img onclick=\"GoFriend('" + row.FUID + "')\" class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "'/><div class='photo-wrap photo-wrap-l fl'><i class='l-sanjiao'></i><a href='javascript:void(0);'><img onclick='photoClick(this)' class='chat-photo' src='" + row.Content + "'/></a></div></div>");
            }
            showDate(row);
            break;
        case 2://红包
            var c = row.Content.split("|");
            if (row.FUID == uid) {
                $("#content").prepend("<div id=" + row.MID + " data-mtype='2' class='chat-message-wrap clearf'><img class='chat-userhead fr' src='" + HeadImg + "' /><div class='redpocket redpocket-r fr' onclick='lookRed(\"" + row.MID + "\")'><div class='rp-top'><i class='rp-r-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + c[1] + "</p><p class='rp-font'>查看红包</p></div><div class='rp-bot'>玩信红包</div></div><div class='rp-got-msg-block'></div></div>");
            } else {
                $("#content").prepend("<div id=" + row.MID + " data-mtype='2' class='chat-message-wrap clearf'><img data-sid='" + row.FUID + "' onclick=\"GoFriend('" + row.FUID + "')\" class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "' /><div class='redpocket redpocket-l fl' onclick='redClick(\"" + row.MID + "\")'><div class='rp-top'><i class='rp-l-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + c[1] + "</p><p class='rp-font'>领取红包</p></div><div class='rp-bot'>玩信红包</div></div><div class='rp-got-msg-block'></div></div>");
            }
            GetRedLocal(row.MID);
            showDate(row);
            break;
        case 3://转账
            var strs = row.Content.split("|");
            if (row.FUID == uid) {
                $("#content").prepend("<div id=" + row.MID + " data-mtype='3' class='chat-message-wrap clearf'><img class='chat-userhead fr' src='" + HeadImg + "'/><div class='redpocket redpocket-r fr' onclick='transClick(\"" + strs[0] + "\")'><div class='rp-top'><i class='rp-r-sanjiao'></i><i class='rp-transfericon'></i><p class='rp-num'>转账给" + friendName + "</p><p class='rp-font'>&yen;" + strs[1] + "</p></div><div class='rp-bot'>玩信转账</div></div></div>");
                //$('#content').prepend("<div class='chat-message-wrap clearf'><img class='chat-userhead fr' src='/img/chat/head4.png' /><p class='chat-message fr'><i class='r-sanjiao'></i>转账给"+friendName +"￥"+strs[1] + "元</p></div>");

            } else {
                $("#content").prepend("<div id=" + row.MID + " data-mtype='3' class='chat-message-wrap clearf'><img onclick=\"GoFriend('" + row.FUID + "')\" class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "'/><div class='redpocket redpocket-l fl' onclick='transClick(\"" + strs[0] + "\")'><div class='rp-top'><i class='rp-l-sanjiao'></i><i class='rp-transfericon'></i><p class='rp-num'>收到" + friendName + "转账</p><p class='rp-font'>&yen;" + strs[1] + "</p></div><div class='rp-bot'>玩信转账</div></div></div>");
                //$('#content').prepend("<div class='chat-message-wrap clearf'><img class='chat-userhead fl' src='/img/chat/head4.png' /><p class='chat-message fl'><i class='l-sanjiao'></i>收到"+friendName+"转账 ￥" + strs[1] + "元</p></div><p class='chat-datetime'>" + row.CreateTime + "</p>");
            }
            showDate(row);
            break;
        case 4:
            var str = row.Content.split("|");
            if (row.FUID == uid) {
                $("#content").prepend("<div id=" + str[4] + "  data-mtype='4' class='chat-message-wrap clearf' ontouchstart='msgmousedown(this)' ontouchend='msgmouseup(this)'><img class='chat-userhead fr' src='" + HeadImg + "'/><div class='calling-card fr'><i class='r-sanjiao'></i><div class='cc-wrap'><div class='cc-top' onclick='CardClick(\"" + str[0] + "\")'><img class='cc-top-img' src='" + str[3] + "' alt='' /><p class='cc-top-name'>" + str[1] + "</p><p class='cc-top-nickname'>" + str[2] + "</p></div><p class='cc-bot'>个人名片</p></div></div></div>");
            } else {
                $("#content").prepend("<div id=" + str[4] + "  data-mtype='4' class='chat-message-wrap clearf' ontouchstart='msgmousedown(this)' ontouchend='msgmouseup(this)'><img onclick=\"GoFriend('" + row.FUID + "')\" class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.Sid) : fHeadImg) + "'/><div class='calling-card fl'><i class='l-sanjiao'></i><div class='cc-wrap'><div class='cc-top' onclick='CardClick(\"" + str[0] + "\")'><img class='cc-top-img' src='" + str[3] + "' alt='' /><p class='cc-top-name'>" + str[1] + "</p><p class='cc-top-nickname'>" + str[2] + "</p></div><p class='cc-bot'>个人名片</p></div></div></div>");
            }
            showDate(row);
            break;
        case 9://如果是扫雷红包
            var strs = row.Content.split("|");
            if (row.FUID == uid) {
                //$('#content').append("<div class='right'>" + data.Content + "</div>");
                $("#content").prepend("<div id='" + strs[0] + "' data-mtype='9' class='chat-message-wrap clearf slfb'><img class='chat-userhead fr' src='" + HeadImg + "' /><div onclick='slhbclick(\"" + strs[0] + "\")' class='redpocket redpocket-r fr'><div class='rp-top'><i class='rp-r-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + strs[1] + "/" + strs[2] + "/" + strs[3] + "</p><p class='rp-font'>领取红包</p></div><div class='rp-bot'>扫雷发包</div></div><div class='rp-got-msg-block'></div></div>");
            } else {
                $("#content").prepend("<div id='" + strs[0] + "'  data-mtype='9' class='chat-message-wrap clearf slfb'><img data-sid='" + row.FUID + "' onclick=\"GoFriend('" + row.FUID + "')\" class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "' /><div onclick='slhbclick(\"" + strs[0] + "\")' class='redpocket redpocket-l fl'><div class='rp-top'><i class='rp-l-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + strs[1] + "/" + strs[2] + "/" + strs[3] + "</p><p class='rp-font'>领取红包</p></div><div class='rp-bot'>扫雷发包</div></div><div class='rp-got-msg-block'></div></div>");
                //$('#content').append("<div class='left'>" + data.Content + "</div>");
            }
            GetRedLocal(row.MID);
            showDate(row);
            break;
        
        default:
            break;
    }
}

//往上滑加载数据  接收消息处理
function doMessage(row) {
  //  console.log(row);
    if (row.TType == 3) {
        DoSystemMsg(row);
        return;
    }
    if (row.TType > 2) { return; }
    //  console.log(row);
    if ((isGroup && row.TUID == gid) || (("undefined" != typeof (friendID) && row.FUID == friendID && row.TUID == uid) || ("undefined" != typeof (friendID) && row.FUID == uid && row.TUID == friendID))) {
       
        switch (row.MType) {
            case -1://撤销消息
                $("#" + row.Content).remove();
                break;
            case 0:
                showDateUp(row);
                //alert(row.Content);
                row.Content = js.lang.String.decodeHtml(row.Content);
                //alert(row.Content);
                if (row.FUID == uid) {
                    $('#content').append("<div class='chat-message-wrap clearf' data-mtype='0' id='" + row.MID + "'  ontouchstart='msgmousedown(this)' ontouchend='msgmouseup(this)'><img class='chat-userhead fr' src='" + HeadImg + "' /><p class='chat-message fr'><i class='r-sanjiao'></i>" + row.Content + "</p></div>");

                } else {
                    $('#content').append("<div class='chat-message-wrap clearf' data-mtype='0' id='" + row.MID + "'  ontouchstart='msgmousedown(this)' ontouchend='msgmouseup(this)'><img onclick=\"GoFriend('" + row.FUID + "')\" class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "' /><p class='chat-message fl'><i class='l-sanjiao'></i>" + row.Content + "</p></div>");
                }
                
                break;
            case 1://照片
                showDateUp(row);
                if (row.FUID == uid) {
                    $('#content').append("<div class='chat-message-wrap clearf' data-mtype='1' id='" + row.MID + "' ontouchstart='msgmousedown(this)' ontouchend='msgmouseup(this)'><img class='chat-userhead fr' src='" + HeadImg + "'/><div class='photo-wrap photo-wrap-r fr'><i class='r-sanjiao'></i><a href='javascript:void(0);'><img onclick='photoClick(this)' class='chat-photo' src='" + row.Content + "'/></a></div></div>");
                } else {
                    $('#content').append("<div class='chat-message-wrap clearf' data-mtype='1' id='" + row.MID + "' ontouchstart='msgmousedown(this)' ontouchend='msgmouseup(this)'><img onclick=\"GoFriend('" + row.FUID + "')\" class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "'/><div class='photo-wrap photo-wrap-l fl'><i class='l-sanjiao'></i><a href='javascript:void(0);'><img onclick='photoClick(this)' class='chat-photo' src='" + row.Content + "'/></a></div></div>");
                }
                break;
            case 2://红包
                showDateUp(row);
                var c = row.Content.split("|");
                if (row.FUID == uid) {
                    $("#content").append("<div id='" + row.MID + "' data-mtype='2'  class='chat-message-wrap clearf'><img class='chat-userhead fr' src='" + HeadImg + "' /><div class='redpocket redpocket-r fr'><div class='rp-top' onclick='lookRed(\"" + row.MID + "\")'><i class='rp-r-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + c[1] + "</p><p class='rp-font'>查看红包</p></div><div class='rp-bot'>玩信红包</div></div><div class='rp-got-msg-block'></div></div>");
                } else {
                    $("#content").append("<div id='" + row.MID + "' data-mtype='2' class='chat-message-wrap clearf'><img data-sid='" + row.FUID + "' onclick=\"GoFriend('" + row.FUID + "')\" class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "' /><div class='redpocket redpocket-l fl' onclick='redClick(\"" + row.MID + "\")'><div class='rp-top'><i class='rp-l-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + c[1] + "</p><p class='rp-font'>领取红包</p></div><div class='rp-bot'>玩信红包</div></div><div class='rp-got-msg-block'></div></div>");
                }
                GetRedLocal(row.MID);
                break;
            case 3://转账
                showDateUp(row);
                var strs = row.Content.split("|");
                if (row.FUID == uid) {
                    $("#content").append("<div class='chat-message-wrap clearf'  data-mtype='3'><img class='chat-userhead fr' src='" + HeadImg + "'/><div class='redpocket redpocket-r fr' onclick='transClick(\"" + strs[0] + "\")'><div class='rp-top'><i class='rp-r-sanjiao'></i><i class='rp-transfericon'></i><p class='rp-num'>转账给" + friendName + "</p><p class='rp-font'>&yen;" + strs[1] + "</p></div><div class='rp-bot'>玩信转账</div></div></div>");
                    //$('#content').prepend("<div class='chat-message-wrap clearf'><img class='chat-userhead fr' src='/img/chat/head4.png' /><p class='chat-message fr'><i class='r-sanjiao'></i>转账给"+friendName +"￥"+strs[1] + "元</p></div>");

                } else {
                    $("#content").append("<div class='chat-message-wrap clearf' data-mtype='3'><img onclick=\"GoFriend('" + row.FUID + "')\" class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "'/><div class='redpocket redpocket-l fl'  onclick='transClick(\"" + strs[0] + "\")'><div class='rp-top'><i class='rp-l-sanjiao'></i><i class='rp-transfericon'></i><p class='rp-num'>收到" + friendName + "转账</p><p class='rp-font'>&yen;" + strs[1] + "</p></div><div class='rp-bot'>玩信转账</div></div></div>");
                    //$('#content').prepend("<div class='chat-message-wrap clearf'><img class='chat-userhead fl' src='/img/chat/head4.png' /><p class='chat-message fl'><i class='l-sanjiao'></i>收到"+friendName+"转账 ￥" + strs[1] + "元</p></div><p class='chat-datetime'>" + row.CreateTime + "</p>");
                }
                break;
            case 4:
                showDateUp(row);
                var str = row.Content.split("|");
                if (row.FUID == uid) {
                    $("#content").append("<div id='" + str[4] + "' data-mtype='4'  class='chat-message-wrap clearf' ontouchstart='msgmousedown(this)' ontouchend='msgmouseup(this)'><img class='chat-userhead fr' src='" + HeadImg + "'/><div class='calling-card fr'><i class='r-sanjiao'></i><div class='cc-wrap'><div class='cc-top' onclick='CardClick(\"" + str[0] + "\")'><img class='cc-top-img' src='" + str[3] + "' alt='' /><p class='cc-top-name'>" + str[1] + "</p><p class='cc-top-nickname'>" + str[2] + "</p></div><p class='cc-bot'>个人名片</p></div></div></div>");
                } else {
                    $("#content").append("<div id='" + str[4] + "'  data-mtype='4' class='chat-message-wrap clearf' ontouchstart='msgmousedown(this)' ontouchend='msgmouseup(this)'><img onclick=\"GoFriend('" + row.FUID + "')\" class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.Sid) : fHeadImg) + "'/><div class='calling-card fl'><i class='l-sanjiao'></i><div class='cc-wrap'><div class='cc-top' onclick='CardClick(\"" + str[0] + "\")'><img class='cc-top-img' src='" + str[3] + "' alt='' /><p class='cc-top-name'>" + str[1] + "</p><p class='cc-top-nickname'>" + str[2] + "</p></div><p class='cc-bot'>个人名片</p></div></div></div>");
                }
                break;
            case 9://如果是扫雷红包
                showDateUp(row);
                var strs = row.Content.split("|");
                if (row.FUID == uid) {
                    //$('#content').append("<div class='right'>" + data.Content + "</div>");
                    $("#content").append("<div id='" + strs[0] + "' data-mtype='9' class='chat-message-wrap clearf slfb'><img class='chat-userhead fr' src='" + HeadImg + "' /><div onclick='slhbclick(\"" + strs[0] + "\")' class='redpocket redpocket-r fr'><div class='rp-top'><i class='rp-r-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + strs[1] + "/" + strs[2] + "/" + strs[3] + "</p><p class='rp-font'>领取红包</p></div><div class='rp-bot'>扫雷发包</div></div><div class='rp-got-msg-block'></div></div>");
                } else {
                    $("#content").append("<div id='" + strs[0] + "' data-mtype='9' class='chat-message-wrap clearf slfb'><img data-sid='" + row.FUID + "' onclick=\"GoFriend('" + row.FUID + "')\" class='chat-userhead fl' src='" + (isGroup ? userHeadImg.GetHeadImg(row.FUID) : fHeadImg) + "' /><div onclick='slhbclick(\"" + strs[0] + "\")' class='redpocket redpocket-l fl'><div class='rp-top'><i class='rp-l-sanjiao'></i><i class='rp-pocketicon'></i><p class='rp-num'>" + strs[1] + "/" + strs[2] + "/" + strs[3] + "</p><p class='rp-font'>领取红包</p></div><div class='rp-bot'>扫雷发包</div></div><div class='rp-got-msg-block'></div></div>");
                    //$('#content').append("<div class='left'>" + data.Content + "</div>");
                }
                GetRedLocal(row.MID);
                $("#content").append("<p class='chat-datetime'>" + row.CreateTime + "</p>");
                break;

            default:
                break;
        }
    } else {
        $("#otherMsgCount").text(++otherMsgCount);

    }
}
var AllImgExt = ['jpg', 'jpeg', 'gif', 'bmp', 'png'];
//上传照片
function ImgUpload(file) {
    var tempname = file.files[0].name.split(".");
    if (tempname.length != 2) {
        return alert("文件类型不正确！");

    }
    var isimg=0;
    for ( var i=0; i < AllImgExt.length; i++) {
        if (tempname[1].toLowerCase() == AllImgExt[i]) {
            isimg = 1;
            break;
        }
        
    }
    if (isimg == 0) {
        return alert("文件类型不正确！");
    }
    if (file && file.files[0]) {
        
        var data = new FormData();
        if (file.files[0].size / 1024 / 1024 > 10) {
            alert("图片不能大于10M");
            return;
        }
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
                alert(JSON.stringify(data));
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
function CardClick(id) {
    location.href = "/Friend/FriendInfo/"+id;
}
function GetMessage(imtoken) {
    $.post("/AjaxIM/IM", "imtoken=" + imtoken, function (data) {
        
        
        //data = JSON.parse(data);
        //console.log(data);
        for (var i = 0; i < data.length; i++) {
            // console.log(data[i]);
            if (data[i].MType == -3 && data[i].MType == -3) {
                alert(data[i].Content);
                location.href = '/Account/LoginOut';
                return;
            }
            doMessage(data[i]);
            toButtom();
        }
        GetMessage(imtoken);
    }, "json");
    //var evtSource = new EventSource("/AjaxIM/IM");
    ////收到服务器发生的事件时触发
    //evtSource.onmessage = function (e) {
    //    console.log(e.data);
    //}
    ////成功与服务器发生连接时触发
    //evtSource.onopen = function () {
    //    console.log("Server open")
    //}
    ////出现错误时触发
    //evtSource.onerror = function () {
    //    console.log("Error")
    //}
    ////自定义事件
    //evtSource.addEventListener("myEvent", function (e) {
    //    console.log(e.data);
    //}, false)
}
function getWS(token) {
    $.post("/AjaxIM/Init", "token=" + getCookie("imtoken"), function (data) {
        GetMessage(getCookie("imtoken"));
    }, "json");
    /*
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
    */
}
//获得最后一条消息id
function GetLastMessageID() {
    var $lastMsg = $(".chat-message-wrap").last();
    return $lastMsg.prop("id");
}

function pushHistory() {
    var state = {
        title: "title",
        url: "#"
    };
    window.history.pushState(state, "title", "#");
}
function backtoindex() {
    var lastMSGID = GetLastMessageID();
    if (isGroup) {
        $.post("/Message/SetGroupLastMessageID", "GID=" + gid, function (data) { }, "json");
    } else {
        $.post("/Message/SetFriendLastMessageID", "FriendID=" + friendID, function (data) { }, "json");
    }
    location.href = "/Account/ChatPage";
}
$(function () {
    pushHistory();
    //手机返回事件
    window.addEventListener("popstate", function (e) {
        backtoindex();
        //var lastMSGID = GetLastMessageID();
        //if (isGroup) {
        //    $.post("/Message/SetGroupLastMessageID", "GID=" + gid, function (data) { }, "json");
        //    window.history.back();
        //} else {
        //    $.post("/Message/SetFriendLastMessageID", "FriendID=" + friendID, function (data) { }, "json");
        //    window.history.back();
        //}
        //根据自己的需求实现自己的功能
    }, false);




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
var timer = null;
function msgmousedown(e) {
    var that = $(e);
    timer = setTimeout(function (ev) {
        var id=$(e).attr("id");
        var oul = document.createElement("ul");
        if (that.find(".chat-userhead").hasClass("fr")) {
            oul.className = "operate-list";
            oul.innerHTML = '<li onclick="copyText(this)">复制</li>' +
                        '<li onclick="zfMSG(\'' + id + '\')">转发</li>' +
                        '<li onclick="RevokeMSG(\'' + id + '\')">撤回</li>' +
                        '<li onclick="DeleteMSG(\'' + id + '\')">删除</li>';
        } else {
            oul.className = "df-cz-list";
            oul.innerHTML = '<li onclick="copyText(this)">复制</li>' +
                        '<li onclick="zfMSG(\'' + id + '\')">转发</li>';
        }
        
        that.append(oul);
    }, 1000);
}
function msgmouseup(e) { 
    clearTimeout(timer);
    timer = null;
}
function zfMSG(id) {
    $("#zfmid").val(id);
    $(".zf-block").show();
}
$(function () {
    $("body").on("click", function () {
        $(".operate-list").remove();
        $(".df-cz-list").remove();
    });
    //$("wrap").on("click", function (e) {
    //    e.stopPropagation();
    //});
    var localfriendList = localStorage.getItem("friendList");
    var friendList = JSON.parse(localfriendList);
    if (friendList == null) return;
   // console.log(friendList);
    var tempFL = [];
    for (var i = 0; i < friendList.length; i++) {
        tempFL.push(' <li data-id="' + friendList[i].ID + '" data-type="' + friendList[i].Type + '" onclick="selectzf(this)" class="zf-contant-item"><i class="zf-checkbox"></i><img class="zf-contact-img" src="' + GetHeadImg(friendList[i]) + '" alt="" />' + friendList[i].Remark + '</li>');
    }
    $("#zf-contact-list").html(tempFL.join(""));
    $(".zf-btn-ok").on("click", function () {
        var zfmid= $("#zfmid").val();
        if(zfmid==""||zfmid.length==0){
            return alert("消息为空，请重新选择");
        }
        var zflistid = [];
        var zflisttype = [];
        $("#zf-contact-list li").each(function () {
            if ($(this).find("i").hasClass("active")) {
                zflistid.push($(this).attr("data-id"));
                zflisttype.push($(this).attr("data-type"));
            }
        });
        
        $.post("/Chat/ForwardMessage", "MID=" + zfmid + "&UIDS=" + zflistid.join(",") + "&TType=" + zflisttype.join(","), function (data) {
            if (data.code == "1") {
                alert("转发成功");
                $("#" + zfmid).find("ul").remove();
                $(".zf-contant-item i").removeClass("active");
                $(".zf-block").hide();
            }

        }, "json");
    });
    $(".zf-btn-cancel").on("click", function () {
        $(".zf-contant-item i").removeClass("active");
        $(".zf-block").hide();
    });
});
function GetHeadImg(row) {
    switch (row.Gtype) {
        case 0:
            return row.headimg;
        case 1:
            return "/img/chat/group.jpg";
        case 2:
            return "/img/chat/group.jpg";
        default:

    }
}
function selectzf(e) {
    if ($(e).find(".zf-checkbox").hasClass("active")) {
        //取消选中；
        $(e).find(".zf-checkbox").removeClass("active");
        //if (!isChecked()) {
        //    $("#zf-qx-i").removeClass("active");
        //};
    } else {
        //选中操作；
        $(e).find(".zf-checkbox").addClass("active");
        //if (isChecked()) {
        //    $("#zf-qx-i").addClass("active");
        //};
    };
}
function copyText(e) {
    var type = $(e).parents(".chat-message-wrap").attr("data-mtype");
    var text;
    switch (type) {
        case "0":
            text = $(e).parent().prev().text();
            break;
        case "1":
            text =window.location.host+$(e).parent().prev().find("img").attr("src");
            break;
        case "4":
            text = $(e).parent().prev().find(".cc-top-nickname").text();
            break;
        default:

    }
   // console.log(text);
    Copy(text);
    $(e).parent().remove();
}
function Copy(str){  
    var save = function(e){
        e.clipboardData.setData("text/plain", str);    
        e.preventDefault();  
    }
    document.addEventListener("copy", save);
    document.execCommand("copy");
    document.removeEventListener("copy", save);
}
function DoSystemMsg(row) {
    if (row.MType == -2) {//处理红包消息
        SaveRedMsg(row.MID, row.Content);
    }
}
function SaveRedMsg(MID, Content) {
    var localdata = localStorage.getItem(MID);
    if (localdata == null || localdata.length == 0) {
        localdata = [];
        localdata.push(Content);
    } else {
        localdata = localdata.split("||");
        localdata.push(Content);
    }
    localStorage.setItem(MID, localdata.join("||"));
    GetRedLocal(MID);
}
function GetRedLocal(MID) {
    var localdata = localStorage.getItem(MID);
    if (localdata == null || localdata.length == 0) {
        return "";
    }
    try {
        var local = localdata.split("||");
        var $msg = $("#" + MID).find(".rp-got-msg-block");
        $msg.html("");
        if ($msg) {
            
            for (var i = 0; i < local.length; i++) {
                $msg.append('<div class="rp-got-msg"><i class="rp-got-icon"></i>'+local[i]+'</div>');
            }
        }
    } catch (e) {
        return "";
    }
}

/**************************************时间格式化处理************************************/
function dateformart(fmt, date) { //author: meizz   
    var o = {
        "M+": date.getMonth() + 1,                 //月份   
        "d+": date.getDate(),                    //日   
        "h+": date.getHours(),                   //小时   
        "m+": date.getMinutes(),                 //分   
        "s+": date.getSeconds(),                 //秒   
        "q+": Math.floor((date.getMonth() + 3) / 3), //季度   
        "S": date.getMilliseconds()             //毫秒   
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}
function getCookie(c_name) {
    if (document.cookie.length > 0) {
        c_start = document.cookie.indexOf(c_name + "=")
        if (c_start != -1) {
            c_start = c_start + c_name.length + 1
            c_end = document.cookie.indexOf(";", c_start)
            if (c_end == -1) c_end = document.cookie.length
            return unescape(document.cookie.substring(c_start, c_end))
        }
    }
    return ""
}

function redOpenSuccess(price, mid) {
    document.getElementById("bg-music").play();
    $("#redprice-show").text(price);
    $(".lookred").attr("href", "/Chat/RedDetail/" + mid);
    $(".redopen").show();
}
function redOpenClose() {
    $(".redopen").hide();
}
function checklocalstorage() {
        if (localStorage.length == 1500) {
            alert("当前缓存过多，请退出登陆后重新登陆清理缓存");
        }
}
$(function () {
    checklocalstorage();
});