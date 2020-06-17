(function ($) {
    let AuxiliaryResources = {
        init: function () {
            $(".active").removeClass("active");
            $(".nav-link").eq(5).addClass("active");
            $(".Mynav").find("li.nav-link").eq(5).find("a").find(".fa-angle-right").removeClass("fa-angle-right").addClass("fa-angle-down");
            $.get("/StatisticalCenter/GetScndUsuage").done(function (response) {
                if (response != "") {
                    response = JSON.parse(response);
                    $("#reportTable").Totable(response, true);
                    if ($(".filterGroup").children().length == 0) {
                        $(".filterGroup").show();
                        $(".filterGroup").FilterGroup(response);
                    }
                } else {
                    $("#reportTable").Totable(response);
                    if ($(".filterGroup").children().length == 0) {
                        $(".filterGroup").show();
                        $(".filterGroup").FilterGroup(response);
                    }
                }
            })
        }
    };
    AuxiliaryResources.init();
})(jQuery)