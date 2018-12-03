//msg.7.当判断处于srch_friend页面的时候的判断逻辑
function srchChatMsg(json) {

    json = JSON.parse(json);

    var waitStr = '';

    var curWindow = localStorage.getItem("curWindow");
    var homeIndex = localStorage.getItem("homeIndex");
    var friend_uid = localStorage.getItem("friend_uid");

    var TType = parseInt(json.TType);
    var Type = parseInt(json.Type);

    if (curWindow == 'srch_friend' && homeIndex == '') {

        //chat前台, 聊天的id与接收者id相同
        if (TType == 3) {

            if (Type == 10) {

                if (json.Content == '已经发送过好友请求') {

                    waitStr = '<div class="wait-passed">';
                    waitStr += '待通过';
                    waitStr += '</div>';

                    var Rid = localStorage.getItem("tempRid");

                    openToast(json.Content, 5000, 5, 0);
                    $("div[data-spec='" + Rid + "']").children().remove();
                    $("div[data-spec='" + Rid + "']").append(waitStr);

                }

            } else if (Type == 11) {

                waitStr = '<div class="wait-passed">';
                waitStr += '已添加';
                waitStr += '</div>';

                openToast('已添加好友成功', 5000, 5, 0);

                $("div[data-spec='" + json.Content + "']").children().remove();
                $("div[data-spec='" + json.Content + "']").append(waitStr);

            }

        }

    }

}