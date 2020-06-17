; (function ($) {
    function bindEvent() {
        $('.top').find('.title').find('p').find('.min-span').on('click', function () {
            if (!$('.Mynav').hasClass('min-nav')) {
                $('.Mynav').addClass('min-nav');
                $('.content').addClass('minContent');
                $('#echart').find('div').css({ width: '100%' }).find('canvas').css({ width: '100%' });
            } else {
                $('.Mynav').removeClass('min-nav');
                $('.content').removeClass('minContent');
            }
        });
        $('.top').find('.title').find('p').find('.max-span').on('click', function () {
            $('.Mynav').removeClass('min-nav');
            if (!$('.Mynav').hasClass('max-nav')) {
                $('.Mynav').addClass('max-nav');
                $('.content').addClass('maxContent');
            } else {
                $('.Mynav').removeClass('max-nav');
                $('.content').removeClass('maxContent');
            }
        });
        $('#user').on('click', function () {
            $('.userSeting').slideToggle();
            $('.navChild').css({ display: 'none' });
        });
        $('.userSeting').find('li').on('click', function () {
            $(this).parent().css({ display: 'none' });
        });
        $('#Record').on('click', function () {
            window.location.href = "DataCenter/Record";
        });

        $("#btnlogout").click(function () {
            //退出按钮点击,清除cookie
            var cookies = document.cookie.split(";");
            $.get("/Public/DeleteUserInfo");//清空userModels里面的成员变量
            for (var i = 0; i < cookies.length; i++) {
                var cookie = cookies[i];
                var eqPos = cookie.indexOf("=");
                var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
                document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/";
            }
            if (cookies.length > 0) {
                for (var i = 0; i < cookies.length; i++) {
                    var cookie = cookies[i];
                    var eqPos = cookie.indexOf("=");
                    var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
                    var domain = location.host.substr(location.host.indexOf('.'));
                    document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/; domain=" + domain;
                }
            }
            window.location.href = "/Index/Index";
            window.reload();
        });
    }
   
    bindEvent();
    $(window).resize(function () {
        //窗口大小发生改变
        if ($(window).width() >= 751) {
            if ($('.content').hasClass('minContent')) {
                $('.content').removeClass('minContent');
                $('.Mynav').removeClass('min-nav');
            }
        } else {
            if ($('.content').hasClass('maxContent')) {
                $('.content').removeClass('maxContent');
                $('.Mynav').removeClass('max-nav');
            }
        } 
    });
})(jQuery);