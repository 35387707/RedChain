/*
 * 接口.61.1.设置推荐码以及二维码
 */
function setQRCode() {

    var qrCodeUrl = localStorage.getItem("qrCodeUrl") + '?UID=' + localStorage.getItem("uid");
    var CardNumber = localStorage.getItem("CardNumber");

    $('#CardNumber').text(CardNumber);

    var qrcode = '';
    qrcode += '<div data-pic_p="' + qrCodeUrl + '" ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img qrcode-cont disp-pic" style="background-image:url(' + qrCodeUrl + ');"></div>';
    $('#qrcode').children().remove();
    $('#qrcode').append(qrcode);

}
