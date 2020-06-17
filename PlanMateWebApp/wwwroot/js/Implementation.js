(function ($) {
    let Implementation = {
        init: function () {
            $('.nav-link.active').removeClass('active');
            $('.nav-link').eq(2).addClass('active')
        }
    }
    Implementation.init();
})(jQuery)