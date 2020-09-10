(function ($) {
    let HistoryData = {
        //历史数据页面
        init() {
            this.render();
        },
        render() {
            $('.Mynav ul .nav-link.active ').removeClass('active');
            $('.Mynav ul .nav-link ').eq(4).addClass('active');
            this.getFiled();
        },
        getFiled() {
            let _this = this;
            $.get('/Datacenter/TableFiled', { "tableName": "History" }).then(function (tableFiled) {
                var columns = [];
                for (var key in tableFiled) {
                    var object = {};
                    object.field = tableFiled[key];
                    object.title = tableFiled[key];
                    object.width = 200;
                    object.align = 'center';
                    object.sortable = true;
                    columns.push(object)
                }
                _this.getTableData(columns);
            });
        },
        getTableData(columns) {
            $("#table-request").SetTable("/Datacenter/GetHistoryTable", {}, columns, formateData, '用户操作详情');
            function formateData(data) {
                $(".filterGroup").FilterGroup(data, columns, "用户操作详情");
                data.forEach(function (item) {
                    try {
                        item['事件时间'] = $.getDateTime(new Date(item['事件时间']));
                        item['实际时间'] = $.getDate(new Date(item['实际时间']));
                        //if (item['备注'] === null || item['备注'] == "") {
                        //    item['备注'] = "-";
                        //}
                        item['备注'] = item['备注'] ===null||item['备注'] == ''?'-':item['备注']
                    }
                    catch (e) {
                        console.log("列表转换错误,后端传过来没有这一列要转换的");
                    }
                });
                return data;
            }
        }
    }
    HistoryData.init();
})(jQuery)