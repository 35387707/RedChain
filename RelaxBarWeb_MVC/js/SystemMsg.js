function GetMessage(imtoken) {
    return;
    $.post("/AjaxIM/IM", "imtoken="+imtoken, function (data) {
        
        //data = JSON.parse(data);
        for (var i = 0; i < data.length; i++) {
            if (data[i].TType == 3) {
                DoSystemMsg(data[i]);
                if (data[i].MType == -3) {
                    return;
                }
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
function DoSystemMsg(row) {
    switch (row.MType) {
        case -3:
            alert(row.Content);
            location.href = '/Account/LoginOut';
            break;
        case -2:
            SaveRedMsg(row.MID, row.Content);
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
$(function () {
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