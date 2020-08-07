(function () {
    //$.Loading();
    let NavName = {
        init: function () {
            $.Loading();
            this.ajaxGet = null;//默认ajax为空
            this.defaultLeftnav();
        },
        defaultLeftnav: function () {
            //默认左边的导航栏数据
            let self = this;
            this.ajaxGet = $.get("/DataCenter/NavName").done(function (response) {
                if (response != "") {
                    response = JSON.parse(response);
                    let navChild = $('<ul class="navChild"></ul>');
                    let listr = "";
                    response.forEach(function (ele) {
                        listr += "<li><a href='javascript:void(0)'>" + ele + "</li>";
                    });
                    $('.Mynav').find('.nav-link').eq(2).append($(navChild).append(listr));
                    $('.Mynav').find('.nav-link.active').children("a").find("span").after('<i class="fas fa-angle-down" aria-hidden="true"></i>');
                    $('.Mynav').find('.nav-link').eq(2).find('.navChild').find('li').find('a').eq(0).addClass('innerActive');
                    self.defaultRIghtNav($('.Mynav').find('.nav-link').eq(2).find('.navChild').find('li').find('a').eq(0).text(), 'fristIndex');
                    self.bindEvent();
                    self.GetAllEquipment("OtherClick");
                } else {
                    //如果获取的导航栏数据为空时
                    self.defaultRIghtNav($('.Mynav').find('.nav-link').eq(2).find('.navChild').find('li').find('a').eq(0).text(), 'fristIndex');
                    $.removeLoading("loading");
                }
            });
        },
        defaultRIghtNav: function (value, curIndex) {
            //默认右边的导航部分的数据
            let self = this;
            this.ajax = $.get("/DataCenter/getNavTable", { value: value }).done(function (response) {
                if (response !== "") {
                    $('.content').find('.card').find('.card-body').html("");
                    let title = $('.Mynav').find('.nav-link').eq(2).find('.navChild').find('li').find('.innerActive').text();
                    response = JSON.parse(response);
                    let tempstr = "";
                    let tempstr1 = "";
                    let lock = false;
                    for (let i = 0; i < response.length; i++) {
                        if (i < 12) {
                            tempstr += "<a href = '#' class='col-md-2 col-sm-6'>" + response[i].resName + "</a>";
                        }
                        else {
                            tempstr1 += "<a href = '#' class='col-md-2 col-sm-6'>" + response[i].resName + "</a>";
                        }
                    }
                    $('.content').find('.card').find('.card-body').append($('<div class="show row"></div>').append(tempstr));
                    if (tempstr1 !== "") {
                        $('.content').find('.card').find('.card-body').append($('<div class="hide row"></div>').append(tempstr1));
                        $('.content').find('.card').find('.card-body').find('.hide').css({ display: 'none' });
                        $('.content').find('.card').find('.card-body').append('<a style="cursor:pointer;font-size:12px;display:block;text-align:center;margin-top:8px;" id="more">查看更多</a>');
                        self.lookMore();
                    }
                    let plan = $('.content').find('.card').find('.card-body').find('.show').find('a').eq(0).text();
                    self.defaultTable(plan, curIndex);
                    //传给后台导出数据的两个参数temp是左边导航栏的内容,val是右边的导航部分(默认传导航栏第一个的内容)
                    let temp = $('.Mynav').find('.nav-link').eq(2).find('.navChild').find('li').find('a.innerActive').text();
                    let val = $('.content').find('.card').find('.card-body').find('.show').find('a').eq(0).text();
                    $.post("/Public/ExportData", { data: "{" + temp + ":" + val + "}" });
                    self.rightNavClick();
                } else {
                    //获取右边导航栏为空时候
                    self.defaultTable("", curIndex);
                }
            });
        },
        defaultTable: function (plan, curIndex, ViewName) {
            //渲染表格
            if (curIndex == "bindClick" || curIndex == "fristIndex") {
                $('.content').find('.card').find('.card-body').find('.show').find('a').eq(0).css({ color: '#000' });
            }
            if (plan != "") {
                var columns = [];
                $.get('/DataCenter/TableFiled', { "tableName": "WorkPlan" }).done(function (fileds) {
                    fileds.forEach(function (item, index) {
                        var object = {};
                        object.field = item;
                        object.title = item;
                        object.width = 200;
                        object.align = 'center';
                        object.sortable = true;
                        columns.push(object)
                    });
                    renderTable(columns)
                });
                function renderTable(column) {
                    $('#table-request').SetTable('/DataCenter/WorkPlanBar', { "plan": plan }, column, func, "执行计划-" + plan);
                    function func(data) {
                        $(".filterGroup").FilterGroup(data, column, "执行计划-" + plan);
                        data.forEach(function (item) {
                            item['需求日期'] = $.getDate(new Date(item['需求日期']));
                            item['计划开始'] = $.getDateTime(new Date(item['计划开始']));
                            item['计划结束'] = $.getDateTime(new Date(item['计划结束']));
                            item['切换开始'] = $.getDateTime(new Date(item['切换开始']));
                        });
                        return data;
                    }
                }
            } else {
                //获取到没有数据的处理
                $('#reportTable').Totable({}, true);
                if ($(".filterGroup").children().length == 0) {
                    $(".filterGroup").show();
                    $(".filterGroup").FilterGroup(response);
                }
            }
        },
        bindEvent: function () {
            //点击事件
            let self = this;
            $('.Mynav').find('.nav-link').eq(2).find('.navChild').find('li').each(function (index, ele) {
                $(ele).on('click', function () {
                    //$.Loading();
                    if (self.ajaxGet != null)
                        self.ajaxGet.abort();
                    self.defaultRIghtNav($(this).find('a').text(), 'bindClick');
                    $('.innerActive').removeClass('innerActive');
                    $(this).find('a').addClass('innerActive');
                })
            })
        },
        lookMore: function () {
            //点击查看更多按钮
            $('.content').find('.card').find('.card-body').find('#more').on('click', function () {
                $(this).text($(this).text() === '收起' ? '查看更多' : '收起');
                $(this).parent().find('.hide').slideToggle();
            })
        },
        rightNavClick: function () {
            //右边的导航条被点击事件
            let self = this;
            $('.content').find('.card').find('.card-body').find('.row').find('a').not('#more').each(function (index, ele) {
                $(ele).on('click', function () {
                    if (self.ajaxGet != null)
                        self.ajaxGet.abort();//若点击了链接则取消刚刚的ajax
                    $('.content').find('.card').find('.card-body').find('.row').find('a').css({ color: '#32A4F9' })
                    $(this).css({ color: '#000' });
                    self.defaultTable($(this).text(), 'OtherClick');
                    //点击后传给导出的参数temp是左边导航栏点击的部分,val是右边导航栏点击的部分
                    let temp = $('.Mynav').find('.nav-link').eq(2).find('.navChild').find('li').find('a.innerActive').text();
                    let val = $(this).text();
                    $.post("/Public/ExportData", { data: "{" + temp + ":" + val + "}" });
                });
            });
        },
        GetAllEquipment: function (curIndex) {
            //获取所有的数据
            let self = this;
            $("#allEquipment").unbind();
            $('#allEquipment').find('a').on('click', function () {
                let ViewName = $('.Mynav').find('li.nav-link').eq(2).find('.navChild').find('li').find('a.innerActive').text();
                //$.Loading();
                self.defaultTable(null, curIndex, ViewName);
            });
        }
    }
    NavName.init();
})(jQuery);