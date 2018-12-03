/*
 * 迭代国家数据
 */
function dispCountryList() {

    //<div data-country="中国" data-areacode="86" class="sub-bord menu-sub tx-l" ontouchstart="appcan.touch('btn-act')">

    var country = '';

    $(countryJson).each(function(i, v) {

        country += '<div data-country="' + v.chineseName + '" data-areacode="' + v.region + '" class="sub-bord menu-sub tx-l" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';
        // country += '(';
        // country += v.country;
        // country += ')';
        // country += ' ';
        country += v.chineseName;
        country += '&nbsp;+&nbsp;';
        country += v.region;
        country += '</div>';

    });

    $("#country").children().remove();
    $('#country').append(country);

    $('.menu-sub').click(function() {

        var country = $(this).data('country');
        var areacode = $(this).data('areacode');

        localStorage.setItem("before", areacode);

        var area = {
            country : country,
            areacode : areacode,
        }
        
        debug(area);

        appcan.window.publish('area', JSON.stringify(area));
        appcan.window.closePopover('area');
        localStorage.setItem("area", 0);

    });

}
