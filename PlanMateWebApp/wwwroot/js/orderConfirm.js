var Fileds = /** @class */ (function () {
    function Fileds() {
    }
    return Fileds;
}());
var OrderConfirm = /** @class */ (function () {
    function OrderConfirm($) {
        this.$ = $;
    }
    ;
    OrderConfirm.prototype.init = function () {
        this.$.Loading();
        this.getColumns();
        this.BindEvent();
    };
    ;
    OrderConfirm.prototype.getColumns = function () {
        var columns = [];
        var self = this;
        this.$.get('/DataCenter/TableFiled', { "tableName": "WorkOrder" }).done(function (fileds) {
            console.log(fileds);
            for (var key in fileds) {
                var object = new Fileds();
                object.field = fileds[key];
                object.title = fileds[key];
                object.align = 'center';
                object.sortable = true;
                columns.push(object);
            }
            self.renderTable(columns);
        });
    };
    ;
    OrderConfirm.prototype.renderTable = function (column) {
        var self = this;
        self.$('#table-request').SetTable('/DataCenter/DTworkOrder', {}, column, formateData, "订单列表");
        function formateData(data) {
            self.$(".filterGroup").FilterGroup(data, column, "订单列表");
            data.forEach(function (item) {
                try {
                    item['需求日期'] = self.$.getDate(new Date(item['需求日期']));
                    item['计划开始时间'] = self.$.getDateTime(new Date(item['计划开始时间']));
                    item['计划结束时间'] = self.$.getDateTime(new Date(item['计划结束时间']));
                }
                catch (e) {
                }
            });
            return data;
        }
    };
    OrderConfirm.prototype.BindEvent = function () {
        var ExportFlag = true;
        $("#ExportBtn").on("click", function () {
            if (ExportFlag) {
                ExportFlag = false;
                $.post('/Datacenter/ExportDataCenter').then(function (path) {
                    ExportFlag = true;
                    window.open(path); 
                }, function () {
                    ExportFlag = true;
                });
            }
        });
    }
    return OrderConfirm;
}());
(function ($) {
    var order = new OrderConfirm($);
    order.init();
})(jQuery);



