function GetImgCode(c) {
    $(c).attr("src", "/Verifycode/getVerifyCode?t=1&rd=" + Math.random());
}

function daojishi(c) {
    var n = $(c).attr("now");
    if (n > 0) {
        $(c).val(n + "s");
        $(c).text(n + "s");
        $(c).attr("now", n - 1);
        //$(c).attr("lastbg", $(c).css("background-color"));
        //$(c).css("background-color","gray");
    }
    else {
        $(c).removeAttr("disabled");
        $(c).val("重新获取");
        $(c).text("重新获取");
        //$(c).css("background-color", $(c).attr("lastbg"));
        clearInterval(interdjs);
    }
}

var interdjs;
function GetSMSCode(c, p,b) {
    //if (p.length != 11 || p[0]!="1" || isNaN(parseInt(p)))
    //{
    //    alert("请先填写正确的手机号码");
    //    return;
    //}
    if (p=="") {
        alert("请先填写正确的手机号码");
        return;
    }
    $(c).attr("disabled", "disabled");
    $.ajax({
        //提交数据的类型 POST GET
        type: "GET",
        //提交的网址
        url: "/Verifycode/getVerifyCode?t=2&r=" + p + "&b="+b+"&rd=" + Math.random(),
        //提交的数据
        data: "",
        //返回数据的格式
        datatype: "json",//"xml", "html", "script", "json", "jsonp", "text".
        //在请求之前调用的函数
        beforeSend: function () { },
        //成功返回之后调用的函数             
        success: function (data) {
            if (data == "1") {
                $(c).attr("now", "60");
                interdjs = setInterval(daojishi, 1000, c);
            }
            else {
                alert('短信获取失败！请重新点击获取。');
                $(c).removeAttr("disabled");
            }
        },
        //调用执行后调用的函数
        complete: function (XMLHttpRequest, textStatus) {
            //alert(textStatus);
        },
        //调用出错执行的函数
        error: function () {
            //请求出错处理
        }
    });
}

function GetEmailCode(c, p) {
    if (p.indexOf('@') < 1) {
        alert("请先填写正确的邮箱号码");
        return;
    }
    $(c).attr("disabled", "disabled");
    $.ajax({
        type: "GET",
        url: "/Verifycode/getVerifyCode?t=3&r=" + p + "&rd=" + Math.random(),
        datatype: "json",
        success: function (data) {
            if (data == "1") {
                $(c).attr("now", "60");
                interdjs = setInterval(daojishi, 1000, c);
            }
            else {
                alert('验证码获取失败！请重新点击获取。');
                $(c).removeAttr("disabled");
            }
        },
    });
}