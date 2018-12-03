//msg.4.当判断处于sys_msg页面的时候的判断逻辑
function sysChatMsg(json) {

    json = JSON.parse(json);

    var curWindow = localStorage.getItem("curWindow");
    var homeIndex = localStorage.getItem("homeIndex");
    var friend_uid = localStorage.getItem("friend_uid");

    var TType = parseInt(json.TType);
    var Type = parseInt(json.Type);

    if (curWindow == 'chat' && homeIndex == '') {

        //chat前台, 聊天的id与接收者id相同
        if (json.Sid == friend_uid) {

            if (TType == 1) {

                if (Type == 0 || Type == 1) {

                    var msg_cont = '';
                    if (isJSON(json.Content) == true) {

                        var temp = json.Content;
                        msg_cont = temp.content;

                    } else {

                        msg_cont = json.Content;

                    }

                    chatIsrtOne(json.Rid, json.Sid, json.Sid, TType, 2, Type, msg_cont);

                }

            } else if (TType == 2) {

            } else if (TType == 3) {

            }

        } else {

            if (TType == 3) {

                if (json.Content == '对方未在线') {

                    openToast(json.Content, 5000, 5, 0);

                }

            }

        }

    }

}