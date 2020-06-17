(function ($) {
    let HistoryData = {
        //历史数据页面
        init() {
            this.render();
        },
        render() {
            $('.Mynav ul .nav-link.active ').removeClass('active');
            $('.Mynav ul .nav-link ').eq(3).addClass('active');
        }
    }
    HistoryData.init();
})(jQuery)