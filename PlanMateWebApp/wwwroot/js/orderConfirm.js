; (function ($) {
    let orderConfirm = {
        init: function () {
            let count = null;
            let source = null;
            $.Loading();
            //$.get('/Datacenter/StatisticalData').done(function (response) {
            //    //获取echart的横轴的数据
            //    response = JSON.parse(response);
            //    response = $.DateFormat(response);//更改日期的格式
            //    response.unshift("日期");
            //    source = response;
            //    $.get('/Datacenter/OrderPieCount').done(function (response) {
            //        //获取echart的纵轴的数据
            //        response = JSON.parse(response);
            //        response[0].EarlyTime.unshift("提前交货订单");
            //        response[1].ErrorTime.unshift("异常交货订单");
            //        response[2].LateTime.unshift("延迟交货订单");
            //        response[3].OnTime.unshift("正常交货订单");
            //        count = response;
            //        $.Toechart("echart", source, count);//初始化echart
            //    })
            //});
            this.render();

        },
        render: function () {
            let self = this;
            $.get("/DataCenter/DTworkOrder").done(function (response) {
                if (response != "") {
                    var aa = JSON.parse(response);
                    response = JSON.parse(response);
                    self.data = response;
                    self.bindEvent();
                    for (let i = 0; i < response.length; i++) {
                        //格式化日期,把后面的零清除掉
                        response[i]["需求日期"] = $.getDate(new Date(response[i]["需求日期"]));
                    }
                    $('#reportTable').Totable(response, true);//初始化表格的样式
                    $(".filterGroup").show();
                    $(".filterGroup").FilterGroup(response);
                } else {
                    $('#reportTable').Totable(response);//初始化表格的样式

                }
                $.RemoveLoading('loading');//数据加载完毕,移除loading动画

            });

        },
        bindEvent: function () {
            //筛选的input框
            let self = this;
            $("#filterInput").on("input", function () {
                let temp = $(this).val();
                let newArr = [];
                for (var i = 0; i < self.data.length; i++) {
                    for (var k in self.data[i]) {
                        if (newArr.indexOf(self.data[i]) == -1 && self.data[i][k].indexOf(temp) != -1) {
                            newArr.push(self.data[i]);
                        }
                    }
                }
                $('#reportTable').Totable(newArr, true)
                //console.log(newArr)
            })
        }
    }
    orderConfirm.init();
    })(jQuery);


