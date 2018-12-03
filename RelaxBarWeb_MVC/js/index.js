function GetMessage(imtoken) {
    $.post("/AjaxIM/IM","imtoken="+imtoken, function (data) {
       

        //data = JSON.parse(data);
        //console.log(data);
        for (var i = 0; i < data.length; i++) {
            UpdateChatItem(data[i]);
            if (data[i].MType == -3) {
                return;
            }
        }
        GetMessage(imtoken);
    }, "json");
}
//function getWS(token) {
//    $.post("/AjaxIM/Init", null, function (data) {
//        GetMessage();
//    }, "json");
//}
function GetFriend(flist,id) {
    for (var i = 0; i < flist.length; i++) {
        if (flist[i].ID == id) {
            
            return flist[i];
        }
    }
}
function DoSystemMsg(row) {
    switch (row.MType) {
        case -3:
            alert(row.Content);
            location.href = '/Account/LoginOut';
            break;
        case -2:
            SaveRedMsg(row.MID, row.Content);
            break;
        default:
            break;
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
}
function UpdateChatItem(row) {
    console.log(row);
    if (row.TType == 3) {
        DoSystemMsg(row);
        return;
    }
    if (row.TType > 2) { return;}
    var FUID = row.FUID;
    var TType = row.TType;
    var MType = row.MType;
    var TUID = row.TUID;
    var Content = row.Content;
    var CreateTime = row.CreateTime;
    var finddivid;
    switch (row.TType) {
        case 1:
            finddivid = row.FUID;
            break;
        case 2:
            finddivid = row.TUID;
            break;
    }
   
    var $domdiv = $("#" + finddivid);
    //console.log($domdiv);
    if ($domdiv.length<1) {//如果聊天的对象没在首页
        var dom = [];
        var flist = JSON.parse(localStorage.getItem("friendList"));
        var friend;
        console.log(row);
        if (row.TType == 1) {//个人
            friend = GetFriend(flist, row.FUID);
            //console.log(friend);
            //dom.push("<div class='chat-item'  data-top='" + friend.IsTop + "' id='" + row.FUID + "'><img onclick='uheadClick(\"" + row.FUID + "\",0)' class='chat-headimg halfTop' src='" + friend.headimg + "' /><p class='chat-name'  onClick=\"userClick('" + row.ID + "'," + row.Type + "," + row.Gtype + ")\">" + friend.Remark + "</p>");
            dom.push("<div class='chat-item'  data-top='" + friend.IsTop + "' id='" + row.FUID + "' onClick=\"userClick('" + row.FUID + "'," + row.TType + "," + row.Gtype + ")\"><img class='chat-headimg halfTop' src='" + friend.headimg + "' /><p class='chat-name' >" + friend.Remark + "</p>");
            dom.push("<p class='chat-lastmsg'>" + getLastContent(row.Content, row.MType) + "</p><font class='chat-time'>" + getLastTime(row.CreateTime) + "</font><span class='msgcount'>1</span></div>");
        } else if (row.TType == 2) {//群
            friend = GetFriend(flist, row.TUID);
            //console.log(friend);
            //dom.push("<div class='chat-item'  data-top='" + friend.IsTop + "' id='" + row.TUID + "'><img onclick='uheadClick(\"" + row.TUID + "\"," + row.Gtype + ")' class='chat-headimg halfTop' src='/img/chat/group.jpg' /><p class='chat-name'  onClick=\"userClick('" + row.ID + "'," + row.Type + "," + row.Gtype + ")\">" + friend.Remark + "</p>");
            dom.push("<div class='chat-item'  data-top='" + friend.IsTop + "' id='" + row.TUID + "' onClick=\"userClick('" + row.TUID + "'," + row.TType + "," + row.Gtype + ")\"><img class='chat-headimg halfTop' src='/img/chat/group.jpg' /><p class='chat-name' >" + friend.Remark + "</p>");
            dom.push("<p class='chat-lastmsg'>" + getLastContent(row.Content, row.MType) + "</p><font class='chat-time'>" + getLastTime(row.CreateTime) + "</font><span class='msgcount'>1</span></div>");
        }
        insertElement(dom.join(""), friend.IsTop);
        //$("#flist").prepend(dom.join(""));
    } else {
        var $cdom = $domdiv.find(".msgcount");
        if ($cdom.length<1) {
            $domdiv.append("<span class='msgcount'>1</span>");
        } else {
            $cdom.text(parseInt($cdom.text())+1);
        }
        $domdiv.find(".chat-lastmsg").text(getLastContent(row.Content, row.MType));
        $domdiv.find(".chat-time").text(getLastTime(row.CreateTime));
        UpdateElement($domdiv, parseInt($($domdiv).attr("data-top")));
    }
    
    //console.log(row);
    //flist.push("<div class='chat-item'><img onclick='uheadClick(\"" + list[i].ID + "\"," + list[i].Gtype + ")' class='chat-headimg halfTop' src='" + GetHeadImg(list[i]) + "' /><p class='chat-name'  onClick=\"userClick('" + list[i].ID + "'," + list[i].Type + "," + list[i].Gtype + ")\">" + list[i].Remark + "</p>");
    //flist.push("<p class='chat-lastmsg'>" + getLastContent(list[i].lastContent, list[i].MType) + "</p><font class='chat-time'>" + getLastTime(list[i].lastTime) + "</font>" + getMessageCount(list[i].WDCount) + "</div>");

}
//修改排序
function UpdateElement($dom, IsTop) {
    var fitem = $("#flist").children();
    for (var i = 0; i < fitem.length; i++) {
        var item = fitem[i];
        var $countitem = $(item).find(".msgcount");
        if (IsTop == 1) {
            
            $(item).before($dom);
            return;
        } else if ($countitem.length == 0) {
            $(item).before($dom);
            return;
        } else {
            if ($(item).attr("data-top") == "1") {
                continue;
            } else {
                $(item).before($dom);
                return;
            }
            
        }
    }
}
//新增排序
function insertElement(dom,IsTop) {
    var fitem = $("#flist").children();
    for (var i = 0; i < fitem.length; i++) {
        var item = fitem[i];
        var $countitem = $(item).find(".msgcount");
        if (IsTop == 1) {
            $(item).before($(dom));
            return;
        }else if ($countitem.length == 0) {//如果没消息
            $(item).before($(dom));
            return;
        } else {
            if ($(item).attr("data-top") == "1") {//如果置顶
                continue;
            } else {
                $(item).before($(dom));
                return;
            }
        }
    }
}
$(function () {
    setTimeout(function () {
        insertElement();
    }, 1000);
    
    GetMessage(localStorage.getItem("imtoken"));
});
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