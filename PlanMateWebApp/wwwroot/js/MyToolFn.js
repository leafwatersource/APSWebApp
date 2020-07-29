(function ($) {
    let pmuid = null;
    $.fn.extend({
        SetTable: function (url,data,column,func) {
            $(this).bootstrapTable('destroy').bootstrapTable({
                ajax: function (request) {                    //使用ajax请求
                    $.ajax({
                        type: "GET",
                        url: url,             //请求后台的URL（*）
                        contentType: 'application/json;charset=utf-8',
                        dataType: 'json',
                        data: data || request.data,
                        success: function (res) {
                            res.data.rows = JSON.parse(res.data.rows);
                            res.data.rows = func(res.data.rows);
                            request.success({
                                row: res.data,
                            });
                            $('#table-request').bootstrapTable('load', res.data.rows);
                            $.RemoveLoading('loading');
                        },
                        error: function (error) {
                            console.log(error);
                        }
                    })
                },
                toolbar: '#toolbar',                //工具按钮用哪个容器
                striped: true,                      //是否显示行间隔色
                cache: true,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
                pagination: true,                   //是否显示分页（*）
                search: true,
                strictSearch: true,
                pageList: [20, 40, 60, 100],        //可供选择的每页的行数（*）
                clickToSelect: true,
                height: 500,                        //行高，如果没有设置height属性，表格自动根据记录条数觉得表格高度
                columns: column,                 //列设置
                exportDataType: 'all',//'basic':当前页的数据, 'all':全部的数据, 'selected':选中的数据
                showExport: true,  //是否显示导出按钮
                buttonsAlign: "right",  //按钮位置
                exportTypes: ['excel'], 
            })
        },
        SetLoading: function (options) {
            var $this = $(this);
            var _this = this;
            return this.each(function () {
                var loadingPosition = '';
                var defaultProp = {
                    direction: 'column',												//方向，column纵向   row 横向
                    animateIn: 'fadeInNoTransform',    								//进入类型
                    title: '请稍等...',      										//显示什么内容
                    name: 'loadingName', 											//loading的data-name的属性值  用于删除loading需要的参数
                    type: 'origin', 			  									//pic   origin  
                    discription: '这是一个描述', 										//loading的描述
                    titleColor: 'rgba(255,255,255,0.7)',								//title文本颜色
                    discColor: 'rgba(255,255,255,0.7)',								//disc文本颜色
                    loadingWidth: 260,                									//中间的背景宽度width
                    loadingBg: 'rgba(0, 0, 0, 0.6)',  									//中间的背景色
                    borderRadius: 12,                 									//中间的背景色的borderRadius
                    loadingMaskBg: 'transparent',          								//背景遮罩层颜色
                    zIndex: 1000001,              									//层级
                    // 这是圆形旋转的loading样式  
                    originDivWidth: 20,           											//loadingDiv的width
                    originDivHeight: 20,           											//loadingDiv的Height
                    originWidth: 8,                  									//小圆点width
                    originHeight: 8,                  									//小圆点Height
                    originBg: '#fefefe',              								//小圆点背景色
                    smallLoading: false,                  								//显示小的loading

                    flexCenter: false, 													//是否用flex布局让loading-div垂直水平居中
                    flexDirection: 'row',													//row column  flex的方向   横向 和 纵向				
                    mustRelative: false, 													//$this是否规定relative
                };
                var opt = $.extend(defaultProp, options || {});
                //根据用户是针对body还是元素  设置对应的定位方式
                if ($this.selector == 'body') {
                    $('body,html').css({
                        overflow: 'hidden',
                    });
                    loadingPosition = 'fixed';
                } else if (opt.mustRelative) {
                    $this.css({
                        position: 'relative',
                    });
                    loadingPosition = 'absolute';
                } else {
                    loadingPosition = 'absolute';
                }
                defaultProp._showOriginLoading = function () {
                    var smallLoadingMargin = opt.smallLoading ? 0 : '-10px';
                    if (opt.direction == 'row') { smallLoadingMargin = '-6px' }
                    //悬浮层
                    _this.cpt_loading_mask = $('<div class="cpt-loading-mask animated ' + opt.animateIn + ' ' + opt.direction + '" data-name="' + opt.name + '"></div>').css({
                        'background': opt.loadingMaskBg,
                        'z-index': opt.zIndex,
                        'position': loadingPosition,
                    }).appendTo($this);
                    //中间的显示层
                    _this.div_loading = $('<div class="div-loading"></div>').css({
                        'background': opt.loadingBg,
                        'width': opt.loadingWidth,
                        'height': opt.loadingHeight,
                        '-webkit-border-radius': opt.borderRadius,
                        '-moz-border-radius': opt.borderRadius,
                        'border-radius': opt.borderRadius,
                    }).appendTo(_this.cpt_loading_mask);
                    if (opt.flexCenter) {
                        _this.div_loading.css({
                            "display": "-webkit-flex",
                            "display": "flex",
                            "-webkit-flex-direction": opt.flexDirection,
                            "flex-direction": opt.flexDirection,
                            "-webkit-align-items": "center",
                            "align-items": "center",
                            "-webkit-justify-content": "center",
                            "justify-content": "center",
                        });
                    }
                    //loading标题
                    _this.loading_title = $('<p class="loading-title txt-textOneRow"></p>').css({
                        color: opt.titleColor,
                    }).html(opt.title).appendTo(_this.div_loading);
                    //loading中间的内容  可以是图片或者转动的小圆球
                    _this.loading = $('<div class="loading ' + opt.type + '"></div>').css({
                        'width': opt.originDivWidth,
                        'height': opt.originDivHeight,
                    }).appendTo(_this.div_loading);
                    //描述
                    _this.loading_discription = $('<p class="loading-discription txt-textOneRow"></p>').css({
                        color: opt.discColor,
                    }).html(opt.discription).appendTo(_this.div_loading);
                    if (opt.type == 'origin') {
                        _this.loadingOrigin = $('<div class="div-loadingOrigin"><span></span></div><div class="div-loadingOrigin"><span></span></div><div class="div_loadingOrigin"><span></span></div><div class="div_loadingOrigin"><span></span></div><div class="div_loadingOrigin"><span></span></div>').appendTo(_this.loading);
                        _this.loadingOrigin.children().css({
                            "margin-top": smallLoadingMargin,
                            "margin-left": smallLoadingMargin,
                            "width": opt.originWidth,
                            "height": opt.originHeight,
                            "background": opt.originBg,
                        });
                    }
                    //关闭事件冒泡  和默认的事件
                    _this.cpt_loading_mask.on('touchstart touchend touchmove click', function (e) {
                        e.stopPropagation();
                        e.preventDefault();
                    });
                };
                defaultProp._createLoading = function () {
                    //不能生成两个loading data-name 一样的loading
                    if ($(".cpt-loading-mask[data-name=" + opt.name + "]").length > 0) {
                        return
                    }

                    defaultProp._showOriginLoading();
                };
                defaultProp._createLoading();
            });
        },
        FilterGroup: function (response) {
            //高级筛选部分
            $(this).append('<input type="text" placeholder="模糊搜索" id="filterInput" />\
                <input type="button" class="btn btn-info" id="Screening" value="高级筛选" />\
                <div class="filterDiv">\
                    <p>*请选择筛选条件 <i class="fa fa-times" aria-hidden="true"></i></p>\
                    <div class="filterBody"> </div>\
                    <div class="container-fluid filterBottom">\
                        <div class="row filterBtnGroup">\
                            <div class="col-6"><input type="button" value="重置" class="btn btn-danger btn-sm mr-0" id="reset" /></div>\
                            <div class="col-6"><input type="button" value="搜索" class="btn btn-success btn-sm" id="senior" /></div>\
                        </div>\
                    </div>\
                </div>');
            for (var k in response[0]) {
                //添加搜索的input框
                $(this).find(".filterBody").append(' <div class="filterItem">\
                            <div class="container-fluid">\
                                <div class="row">\
                                    <div class="col-3"> <label>'+ k + ':</label></div>\
                                    <div class="col-9"> <input type="text" /></div>\
                                </div>\
                            </div>\
                        </div>');
            }
            //高级筛选按钮的点击事件
            $("#Screening").on("click", function () {
                $(".filterDiv").fadeIn();
            });
            //关闭的图标的点击事件
            $(".filterDiv p i").on("click", function () {
                $(".filterDiv").fadeOut();
            });
            //重置按钮的点击事件
            $("#reset").on("click", function () {
                $(".filterDiv").find(".filterBody").find(".filterItem").each(function (index, ele) {
                    $(ele).find("input").val("");
                    $('#reportTable').Totable(response, true);
                });
            });
            //搜索按钮的点击事件
            $("#senior").on("click", function () {
                $("#filterInput").val("");//清空模糊搜索的框
                let target = $(".filterBody").find(".filterItem");
                //筛选条件
                let resultArr= FilterFunc(target, response)
                $('#reportTable').Totable(resultArr, true)
            });
            function FilterFunc(target,response) {
                let filter = {};
                for (let i = 0; i < target.length; i++) {
                    if ($(target).eq(i).find("input").val() != "") {
                        filter[$(target).eq(i).find("label").text().split(":")[0]] = $(target).eq(i).find("input").val().trim();
                    }
                }
                if(JSON.stringify(filter) == "{}"){
                        return response;
                }
                //拿到有值的参数
                let tempFilter = {};
                for (key in filter) {
                    if (typeof (filter[key]) != "undefined" && typeof (filter[key]) != "null" && filter[key] != null && filter[key] != "") {
                        tempFilter[key] = filter[key];
                    }
                }
                //筛选
                let resultArr = response.filter(
                    function (item) {
                        let flag = false;
                        for (key in tempFilter) {
                            if (item[key].toString().indexOf(tempFilter[key].toString()) >= 0) {
                                flag = true;
                            } else {
                                flag = false;
                                break;
                            }
                        }
                        if (flag) {
                            return item;
                        }
                    }
                );
                return resultArr;
            }
            //模糊搜索的input框按键输入事件
            $("#filterInput").on("input", function () {
                let temp = $(this).val().trim();
                let newArr = [];
                for (var i = 0; i < response.length; i++) {
                    for (var k in response[i]) {
                        if (newArr.indexOf(response[i]) == -1 && response[i][k].indexOf(temp) != -1) {
                            newArr.push(response[i]);
                        }
                    }
                }
                $('#reportTable').Totable(newArr, true)
            });
        },
        GetCookie: function (cookieKey) {
            if (!document.cookie.GetCookie("empid") && !document.cookie.GetCookie('uuid')) {
                window.replace = "/Index/Index";
            }
        }
    });
    $.extend({
        getDate: function (date) {
            var year = date.getFullYear();
            var month = date.getMonth() + 1;//月份显示0~11，需要加1
            //如果显示的时间是小于10的，只显示一位数，为了让显示的时间更加规范，添加一个判断条件
            month = month < 10 ? "0" + month : month;
            var day = date.getDate();
            day = day < 10 ? "0" + day : day;
            var dateTime = year + "/" + month + "/" + day;
            return dateTime;
        },
        getDateTime(dateTime) {
            var year = dateTime.getFullYear();
            var month = (dateTime.getMonth() + 1) < 10 ? "0" + (dateTime.getMonth() + 1) : (dateTime.getMonth() + 1);//月份显示0~11，需要加1
            var day = dateTime.getDate() < 10 ? "0" + dateTime.getDate() : dateTime.getDate();
            var hour = dateTime.getHours() < 10 ? "0" + dateTime.getHours() : dateTime.getHours();
            var min = dateTime.getMinutes() < 10 ? "0" + dateTime.getMinutes() : dateTime.getMinutes();
            var sec = dateTime.getSeconds() < 10 ? "0" + dateTime.getSeconds() : dateTime.getSeconds();
            var time = year + "/" + month + "/" + day + " " + hour + ":" + min + ":" + sec;
            return time;
        },
        DateFormat: function (data) {
            //更改日期的格式
            let timeArr = [];
            let len = data.length;
            let day = "";
            for (let i = 0; i < len; i++) {
                day = data[i].replace("/Date(", "").replace(")/", "");
                var ns = new Date(parseInt(day));
                var year = ns.getFullYear();
                var month = ns.getMonth() + 1;
                var date = ns.getDate();
                var hour = ns.getHours();
                var minute = ns.getMinutes();
                var second = ns.getSeconds();
                timeArr.push(year + "-" + month + "-" + date)
            }
            return timeArr;
        },
        Loading: function () {
            //loading动画
            $('body').SetLoading({
                loadingWidth: 220,
                title: '提示',
                name: 'loading',
                titleColor: '#fff',
                discColor: '#fff',
                discription: '等我一下,加载中',
                direction: 'column',
                type: 'origin',
                originBg: '#ECCFBB',
                originDivWidth: 20,
                originDivHeight: 20,
                originWidth: 4,
                originHeight: 4,
                smallLoading: false
            });
        },
        RemoveLoading: function (LoadingName) {
            //移除loading动画,传入loading的名称,默认是loading
            var loadingName = loadingName || '';
            //$('body,html').css({
            //    overflow: 'auto',
            //});
            if (loadingName == '') {
                $(".cpt-loading-mask").remove();
            } else {
                var name = loadingName || 'loadingName';
                $(".cpt-loading-mask[data-name=" + name + "]").remove();
            }
        },
        Toechart: function (id, source, count) {
            //传入元素的id值
            //source的值是图表的y轴坐标
            //count的值是图标的x轴坐标
            //此方法是初始化图标
            setTimeout(function () {
                var myChart = echarts.init(document.getElementById(id));
                window.onresize = myChart.resize;
                option = {
                    theme: 'light',
                    legend: {},
                    tooltip: {
                        trigger: 'axis',
                        showContent: false
                    },
                    dataset: {
                        source: [
                            source,
                            count[0].EarlyTime,
                            count[3].OnTime,
                            count[2].LateTime,
                            count[1].ErrorTime
                        ]
                    },
                    xAxis: { type: 'category' },
                    yAxis: { gridIndex: 0 },
                    grid: { top: '55%' },
                    series: [
                        { type: 'line', smooth: true, seriesLayoutBy: 'row' },
                        { type: 'line', smooth: true, seriesLayoutBy: 'row' },
                        { type: 'line', smooth: true, seriesLayoutBy: 'row' },
                        { type: 'line', smooth: true, seriesLayoutBy: 'row' },
                        {
                            type: 'pie',
                            id: 'pie',
                            radius: '30%',
                            center: ['50%', '25%'],
                            label: {
                                formatter: '{b}: {@' + source[1] + '} ({d}%)'
                            },
                            encode: {
                                itemName: source[0],
                                value: source[1],
                                tooltip: source[1]
                            }
                        }
                    ]
                };
                myChart.on('updateAxisPointer', function (event) {
                    var xAxisInfo = event.axesInfo[0];
                    if (xAxisInfo) {
                        var dimension = xAxisInfo.value + 1;
                        myChart.setOption({
                            series: {
                                id: 'pie',
                                label: {
                                    formatter: '{b}: {@[' + dimension + ']} ({d}%)'
                                },
                                encode: {
                                    value: dimension,
                                    tooltip: dimension
                                }
                            }
                        });
                    }
                });
                myChart.setOption(option);
            });
        },
        Edit: function (a, tableId) {
            $("#" + tableId).bootstrapTable("resetView");
            var tr = a.parentNode.parentNode, toEdit = a.innerHTML == '编辑';
            a.innerHTML = toEdit ? '保存' : '编辑';
            if (toEdit) {
                //点击了编辑按钮
                pmuid = tr.cells[0].innerHTML.replace(/"/g, '&quot;');
                for (let i = 1; i < tr.cells.length - 3; i++) {
                    tr.cells[i].innerHTML = '<input type="text" style="height: 100%;border: none;outline: none;" value="' + tr.cells[i].innerHTML.replace(/"/g, '&quot;') + '"/>';
                }
            }
            else {
                //点击了保存按钮
                var temp = $(tr.cells[0]).find('input').val();//主工单
                var EditArr = [];
                for (let i = 1; i < tr.cells.length - 3; i++) {
                    tr.cells[i].innerHTML = tr.cells[i].firstChild.value.replace(/</g, '&lt;').replace(/>/g, '&gt;');
                    EditArr.push(tr.cells[i].innerHTML);
                }
                $.post("/DataCenter/ChangeOrdelLog", { content: pmuid, temp: EditArr })
            }
        },
        Delete: function (a) {
            //点击了删除按钮
            var tr = a.parentNode.parentNode;
            var temp1 = tr.cells[0].innerHTML;
            $.post("/DataCenter/ChangeOrdelLog", { content: tr.cells[2].innerHTML, temp: null })
        },
        GetCookie: function (cookieName) {
            //获取cookie值传入cookie的名返回的是值
            var obj = {};
            var cookieValue = null;
            if (document.cookie && document.cookie != "") {
                var cookies = document.cookie.split('&');
                for (var i = 0; i < cookies.length; i++) {
                    if (i == 0) {
                        obj[cookies[i].split("=")[1]] = cookies[i].split("=")[2];
                    } else {
                        obj[cookies[i].split("=")[0]] = cookies[i].split("=")[1];
                    }
                }
            }
            return obj[cookieName] ? obj[cookieName] : "没有存入这个cookie名";
        }
    })
})(jQuery)