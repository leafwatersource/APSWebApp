(function ($) {
    let ArrearsList = {
        init: function () {
            $.Loading();
            //workplanid = 656
            //默认的左边的导航栏的样式
            $(".active").removeClass("active");
            $(".Mynav").find("li.nav-link").eq(3).addClass("active");
            $(".Mynav").find("li.nav-link").eq(3).find("a").find(".fa-angle-right").removeClass("fa-angle-right").addClass("fa-angle-down");
            $(".Mynav").find("li.nav-link").eq(3).find(".navChild").find("li").find("a").eq(1).addClass("innerActive");
            this.render();
            //传给后台两个导出的参数,左边导航栏
            let data = $(".nav-link.active").find(".navChild").find(".innerActive").text();
            //$.post("/Public/ExportData", { data: data });
        },
        render: function () {
            //获取表格中的数据
            $.get("/Materials/GetMaterialsTable", { choose: 1 }).done(function (response) {
                if (response!="") {
                    response = JSON.parse(response);
                    for (var i = 0; i < response.length; i++) {
                        //格式化日期
                        response[i]["使用日期"] = $.getDate(new Date(response[i]["使用日期"]));
                        if (response[i]["物料需求日"] != "") {
                            response[i]["物料需求日"] = $.getDate(new Date(response[i]["物料需求日"]));
                        }
                    }
                    $("#reportTable").Totable(response, true);
                    if ($(".filterGroup").children().length == 0) {
                        $(".filterGroup").show();
                        $(".filterGroup").FilterGroup(response);
                    }
                    //if ($(".filterGroup").html()=="") {
                    //    $(".filterGroup").show();
                    //    $(".filterGroup").FilterGroup(response);
                    //}
                   
                } else {
                    $("#reportTable").Totable(response);
                    if ($(".filterGroup").children().length == 0) {
                        $(".filterGroup").show();
                        $(".filterGroup").FilterGroup(response);
                    }
                    //$(".filterGroup").show();
                    //$(".filterGroup").FilterGroup(response);
                }
                $.RemoveLoading();
            });
        }
    }
    ArrearsList.init();
})(jQuery)