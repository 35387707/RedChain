//填充select数据
function dispSelData() {

    var redStr = '';

    var RedPrice = JSON.parse(localStorage.getItem("tabJson"));
    $(RedPrice.list).each(function(j, s) {

        //$("#redmoney").append("<option value=" + s + ">" + s + "</option>");
        redStr += '<div class="money-width">';
        redStr += '<div class="money-inside">';
        redStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="money-style money-sub" data-itid="' + s.ITID + '" data-name="' + s.Name + '">';
        redStr += s.Name;
        redStr += '</div>';
        redStr += '</div>';
        redStr += '</div>';

    });

    $("#type_list").append(redStr);

    $('.money-sub').unbind();
    $('.money-sub').click(function() {

        $(".money-choose").removeClass('money-choose');
        $(this).addClass('money-choose');

        var itid = $(this).data('itid');

        localStorage.setItem("itid", itid);

        $('#biz_type').text($(this).data('name'));

    });

}