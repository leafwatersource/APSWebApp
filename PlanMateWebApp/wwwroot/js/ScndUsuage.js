(function ($) {
    let ScndUsuage = {
        init: function () {
            $(".active").removeClass("active");
            $(".Mynav").find("li.nav-link").eq(5).addClass("active");
            $(".Mynav").find("li.nav-link").eq(5).find("a").find(".fa-angle-right").removeClass("fa-angle-right").addClass("fa-angle-down");
            $(".Mynav").find("li.nav-link").eq(5).find(".navChild").find("li").find("a").eq(1).addClass("innerActive");
            this.render();
        },
        render: function () {
            let self = this;
            $.get("/StatisticalCenter/GetResGroup").done(function (response) {
                console.log(typeof response)
                //默认右边的两个按钮
                if (response != "") {
                    response = JSON.parse(response);
                    let temp = "";
                    for (var i = 0; i < response.length; i++) {
                        temp += '<span style="float:right;margin-right:.5em;" class="itemList"><a href="#">' + response[i].ViewName + '</a></span>'
                    }
                    $("#myHead").append(temp);
                    //$.post("/StatisticalCenter/GetResNameList", { resGroup: response[0].ViewName });
                    //默认调用右边第一个
                    self.GetChildName(response[1].ViewName);
                    self.Chilnav();
                }
            })
        },
        Chilnav: function () {
            let self = this;
            $("#myHead").find('.itemList').each(function (index, ele) {
                $(ele).on("click", function () {
                    let resName = $(ele).text();
                    $('.content').find('.card').find('.card-body').html("");
                    self.GetChildName(resName);
                })
            });
        },
        GetChildName: function (resName) {
            //设备组信息
            let self = this;
            $.post("/StatisticalCenter/GetResNameList", { resGroup: resName }).done(function (response) {
                if (response != "") {
                    response = JSON.parse(response);
                    let title = $('.Mynav').find('.nav-link').eq(2).find('.navChild').find('li').find('.innerActive').text();
                    let tempstr = "";
                    let tempstr1 = "";
                    let lock = false;
                    for (let i = 0; i < response.length; i++) {
                        if (i < 12) {
                            tempstr += "<a href = '#' class='col-2'>" + response[i]["resname"] + "</a>";
                        }
                        else {
                            tempstr1 += "<a href = '#' class='col-2'>" + response[i]["resname"] + "</a>";
                        }
                    }
                    $('.content').find('.card').find('.card-body').append($('<div class="show row"></div>').append(tempstr));
                    if (tempstr1 !== "") {
                        $('.content').find('.card').find('.card-body').append($('<div class="hide row"></div>').append(tempstr1));
                        $('.content').find('.card').find('.card-body').find('.hide').css({ display: 'none' });
                        $('.content').find('.card').find('.card-body').append('<a style="cursor:pointer;font-size:12px;display:block;text-align:center;margin-top:8px;" id="more">查看更多</a>');
                        self.lookMore();
                    }
                    //传给后台导出数据的两个参数temp是左边导航栏的内容,val是右边的导航部分(默认传导航栏第一个的内容)
                    let temp = $('.Mynav').find('.nav-link').eq(2).find('.navChild').find('li').find('a.innerActive').text();
                    let val = $('.content').find('.card').find('.card-body').find('.show').find('a').eq(0).text();
                    $('.content').find('.card').find('.card-body').find('.show').find('a').eq(0).addClass("childnav-childactive")
                    filterTable = val;
                    $.post("/Public/ExportData", { data: "{" + temp + ":" + val + "}" });
                    self.GetTable(val);
                    self.ChildNavBindEvent();
                }
            })
        },
        lookMore: function () {
            //点击查看更多按钮
            $('.content').find('.card').find('.card-body').find('#more').on('click', function () {
                $(this).text($(this).text() === '收起' ? '查看更多' : '收起');
                $(this).parent().find('.hide').slideToggle();
            })
        },
        ChildNavBindEvent: function () {
            let self = this;
            //右边的子标签的点击事件
            $('.content').find('.card').find('.card-body').find('.show').find('a').each(function (index, ele) {
                $(ele).on("click", function () {
                    console.log('here')
                    $(".childnav-childactive").removeClass("childnav-childactive");
                    $(this).addClass('childnav-childactive');
                    //调用初始化表格
                    self.GetTable( $(this).text());
                })
            })
            $('.content').find('.card').find('.card-body').find('.hide').find('a').each(function (index, ele) {
                console.log("被点击了")
                $(ele).on("click", function () {
                    console.log('here')
                    $(".childnav-childactive").removeClass("childnav-childactive");
                    $(this).addClass('childnav-childactive');
                    //调用初始化表格
                    self.GetTable($(this).text());
                })
            })
        },
        GetTable: function (filterTable) {
            $.post("/StatisticalCenter/GetResUsuage", { resName: filterTable }).done(function (response) {
                if (response!="") {
                    response = JSON.parse(response);
                    for (var i = 0; i < response.length; i++) {
                        //格式化日期,把后面的零清除掉
                        if (response[i]["日期"] != "") {
                            response[i]["日期"] = $.getDate(new Date(response[i]["日期"]));
                        }
                        response[i]["稼动率"] += "%";
                    }
                    //初始化表格
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

    }
    ScndUsuage.init();
})(jQuery)

