(function ($) {
    $.fn.extend({
        Advertisement: function (options) {
            this.wrap = this;
            this.wrap.text(options);
        }
    });
    var selector = {
        init: function () {
            this.btn = $(".userBtn");
            this.bindEvent();
            this.render();
            this.Advertisement = ['适应多行业生产排程需求，根据客户需求设计解决方案，为大型企业和中小企业提供不同投资成本的方案。', '按照设备产能和物料自动排程，产生准确的交货和生产计划，准确评估产能和交期，避免给出不能实现的承诺，提早作出资源准备。', '准确的长期生产计划，降低库存水平减少欠料，总装与零部件分厂协同计划，降低半品库存，按库存约束排程减少紧急切换。', '多种自动优化方式，提高设备利用率，降低生产成本，平衡交期与生产效率，在保证按时交货的基础上降低成本。'];
            $('.BottomContent').text(this.Advertisement[0]);
        },
        bindEvent: function () {
            this.btn.on('mousedown', function () {
                $(this).css({ transform: "scale(.9,.9)" });
                let self = this;
                $(document).on('mouseup', function () {
                    $(self).css({ transform: "scale(1,1)" });
                })
            });
            let self = this;
            this.btn.each(function (index, ele) {
                $(ele).hover(function () {
                    $('.BottomContent').Advertisement(self.Advertisement[index]);
                }, function () { $('.BottomContent').Advertisement(self.Advertisement[0]); })
            })
        },
        render: function () {
            $.get("/Selector/IsAdmin").done(function (response) {
                if (response == false) {
                    //查看用户是否有权限
                    $.get("/Selector/FunctionResult").done(function (response) {
                        response = JSON.parse(response);
                        console.log(response)
                        response.forEach(function (ele) {
                            $("#" + ele).css({ display: 'block' });
                        })
                    })
                } else if (response == true) {
                    $(".userBtn").css({ display: 'block' })
                }
            });
        }
    }
    selector.init();
})(jQuery)

function divlink(linkname) {
    if (linkname == "planboard") {
        window.location.href = "/Bord/Index";
    }
    else if (linkname == "datacenter") {
        window.location.href = "/Datacenter/Index";
    }
    else if (linkname == "systemsetting") {
        window.location.href = "/Registered/Index";
    }
    else if (linkname == "reportsystem") {
        window.location.href = "/Phone/Index";
    }
    else {
        alert("link name error , please check your link name.");
    }

}