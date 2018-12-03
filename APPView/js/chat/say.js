//msg.6.当判断处于say_hello页面的时候的判断逻辑
function sayChatMsg(json) {

    json = JSON.parse(json);

    var curWindow = localStorage.getItem("curWindow");
    var homeIndex = localStorage.getItem("homeIndex");
    var friend_uid = localStorage.getItem("friend_uid");

    var TType = parseInt(json.TType);
    var Type = parseInt(json.Type);

    if (curWindow == 'say_hello' && homeIndex == '') {

        if (TType == 1) {

            if (Type == 19) {

                if (isJSON(json.Content) == true) {

                    var temp = json.Content;
                    if (temp.sign == '打招呼') {

                        getSayHelloList(localStorage.getItem("uid"), 1, 'say_hello');

                    }

                }

            }

        }

    }

}