; (function ($) {
    let orderConfirm = {
        init: function () {
            //let count = null;
            //let source = null;
            $.Loading();
            //$.get('/Datacenter/StatisticalData').done(function (response) {
            //获取echart的横轴的数据
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
            this.getColumns(); 
        },
        bindEvent: function () {
          
        },
        getColumns: function () {
            var columns = [];
            let self = this;
            $.get('/DataCenter/TableFiled', { "tableName": "WorkOrder" }).done(function (fileds) {
                fileds.forEach(function (item, index) {
                    var object = {};
                    object.field = item;
                    object.title = item;
                    object.align = 'center';
                    object.sortable = true;
                    columns.push(object);
                });
                self.renderTable(columns);
            });
           
        },
        renderTable: function (column) {
            let self = this;
            $('#table-request').SetTable('/DataCenter/DTworkOrder', {}, column, formateData,"订单列表");
            function formateData(data) {
                console.log(data);
                $(".filterGroup").FilterGroup(data, column, "订单列表");
                data.forEach(function (item) {
                    item['需求日期'] = $.getDate(new Date(item['需求日期']));
                });
                return data;
            }
           
        }
    }
    orderConfirm.init();
    })(jQuery);


