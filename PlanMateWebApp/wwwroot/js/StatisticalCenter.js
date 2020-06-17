(function ($) {
    let statisticalCenter = {
        init: function () {
            //默认页面的导航栏的样式
            $(".active").removeClass("active");
            $(".nav-link").eq(5).addClass("active");
            $(".Mynav").find("li.nav-link").eq(5).find("a").find(".fa-angle-right").removeClass("fa-angle-right").addClass("fa-angle-down");
            $(".Mynav").find("li.nav-link").eq(5).find(".navChild").find("li").find("a").eq(1).addClass("innerActive");
            this.GetTable(0, "");
            this.bindEvent();
        },
        bindEvent: function () {
            let self = this;
            //最后工序,中间工序,所有工序的点击事件
            $('#allop').add("#midop").add("#lastop").on("click", function () {
                $('.content').find('.card').find('.card-body').html("");
                if ($(this).attr('id') == "allop") {
                    self.GetTable(2, "")
                } else if ($(this).attr('id') == "midop") {
                    $.get("/StatisticalCenter/GetopName").done(function (response) {
                        if (response != "") {
                            response = JSON.parse(response);
                            let title = $('.Mynav').find('.nav-link').eq(2).find('.navChild').find('li').find('.innerActive').text();
                            let tempstr = "";
                            let tempstr1 = "";
                            let lock = false;
                            for (let i = 0; i < response.length; i++) {
                                if (i < 12) {
                                    tempstr += "<a href = '#' class='col-2'>" + response[i] + "</a>";
                                }
                                else {
                                    tempstr1 += "<a href = '#' class='col-2'>" + response[i] + "</a>";
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
                            self.GetTable(0, val);
                            self.ChildNavBindEvent(0);
                        }
                    })
                } else {
                    self.GetTable(1, "")
                }
            });
        },
        GetTable: function (isfina, filterTable) {
            //第一个参数是:0\1\2,第二个参数是右边子标签的内容
            $.post("/StatisticalCenter/GetProductOutput", { isfinal: isfina, opname: filterTable }).done(function (response) {
                console.log(JSON.parse( response))
                if (response!="") {
                    response = JSON.parse(response);
                    for (var i = 0; i < response.length; i++) {
                        //格式化日期,把后面的零清除掉
                        response[i]["产出日期"] = $.getDate(new Date(response[i]["产出日期"]))
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
        },
        ChildNavBindEvent: function (isfina) {
            let self = this;
            //右边的子标签的点击事件
            $('.content').find('.card').find('.card-body').find('.show').find('a').each(function (index, ele) {
                $(ele).on("click", function () {
                    $(".childnav-childactive").removeClass("childnav-childactive");
                    $(this).addClass('childnav-childactive');
                    //调用初始化表格
                    self.GetTable(isfina, $(this).text());
                })
            })
        },
        lookMore: function () {
            //点击查看更多按钮
            $('.content').find('.card').find('.card-body').find('#more').on('click', function () {
                $(this).text($(this).text() === '收起' ? '查看更多' : '收起');
                $(this).parent().find('.hide').slideToggle();
            })
        }
    }
    statisticalCenter.init();
})(jQuery)