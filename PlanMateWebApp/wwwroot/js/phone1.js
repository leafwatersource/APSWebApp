var vm = new Vue({
    el: "#Wrapper",
    data: {
        show_menu_flag: true,//是否是展开菜单的内容
        res_list: false,//判断设备列表是否被点击
        tab_btn: true,//判断切换按钮
        phone_open: false,//小屏幕的时候点击
        content_title: "",//当前的设备名称
        menu_list: [],//菜单列表
        child_active: [],//判断哪些设备组的标签被点击
        cur_plan: { workID: "无", jobQty: "无", pmResName: "无", productID: "无", itemAttr1: "无", pmOpName:"无"},//点击的当前设备下的当前计划
        plan_list: { undone: [], finish: [] },//当前的设备下的所有计划
        cur_planIndex: null,//当前工单点击的索引
        next_workId: "无",//下一个工单的workid
        btn_start: "开始报工",//按钮的文字
        model_is_open: true,//模态框是否是打开的
        model_title: "",//模态框的标题
        model_content: "",//模态框的内容
        model_enter: true,//模态框的确定按钮是否是显示的
        start_time: "",//报工页面开始的时间
        end_time: "",//生产结束时间
        finish_qty: "",//工单完成数
        bad_qty: 0,//工单不良数
        ErrorAttr: "",//暂停生产的原因
        event_list: [],//事件的列表
        event_json: { workID: "", eventMessage: "", eventName: "" },//用于存取事件的json对象
        cur_product_work: { workID: "无", productID: "无", pmOpName:"无"},//当前的生产中工单是哪一个
    },
    created: function () {
    },
    mounted: function () {
        this.getMenu();
        $(".main").on("click", function () {
            $(".phone_open").removeClass("phone_open");
        });// 手机屏幕的时候点击就关闭菜单的展示
        this.tag_event();
        this.render_echart();
    },
    methods: {
        phone_menu: function () {
            this.phone_open = this.phone_open == true ? false : true;
        },//小屏幕点击时候展开菜单
        order_click: function (e) {
            console.log('order');
        },//每一个工单的点击事件
        show_menu: function () {
            this.show_menu_flag = this.show_menu_flag == true ? false : true;
            this.res_list = false;
        },//展开或者收起菜单栏
        res_list_click: function (e) {
            if (e.target.id) {
                $(".Active").removeClass("Active");
                $(e.currentTarget).addClass("Active");
                this.res_list = this.res_list == true ? false : true;
            }
        },//设备列表被点击的时候触发
        tab_left: function () {
            this.tab_btn = true;
        },//左边的切换按钮的点击事件
        tab_right: function () {
            this.tab_btn = false;
        },//右边的切换按钮的点击事件
        getMenu: function () {
            var _this = this;
            $.get('/Phone/Get_menu').done(function (menuList) {
                var viewarr = [];
                var view_json = { "viewName": "", "resName": [] };
                for (var i = 0; i < menuList.length; i++) {
                    if (viewarr.indexOf(menuList[i]["viewName"]) == -1) {
                        viewarr.push(menuList[i]["viewName"]);
                        view_json["viewName"] = menuList[i]["viewName"];
                        view_json["resName"].push(menuList[i]["resName"]);
                        _this.menu_list.push(view_json);
                        view_json = { "viewName": "", "resName": [] };
                    } else {
                        for (var v = 0; v < _this.menu_list.length; v++) {
                            if (_this.menu_list[v]["viewName"] == menuList[i]["viewName"]) {
                                _this.menu_list[v]["resName"].push(menuList[i]["resName"])
                            }
                        }
                    }
                }
                _this.cur_plan["pmResName"] = _this.menu_list[0]['resName'][0];
                _this.get_plan(_this.menu_list[0]['resName'][0]);
            });
        },//获取菜单
        get_plan: function (resname) {
            var _this = this;
            var list = [];//判断计划的状态
            $.post('/Phone/Get_plan', { 'resname': resname }).done(function (planList) {
                var list_json = { 'workID': "", "pmOpName": "", "dayShift": "", "plannedqty": "" };
                for (var i = 0; i < planList.length; i++) {
                    planList[i]["badQty"] = "0";
                    list_json["workID"] = planList[i]["workID"];
                    list_json["pmOpName"] = planList[i]["pmOpName"];
                    list_json["plannedqty"] = planList[i]["plannedqty"];
                    list_json["dayShift"] = planList[i]["dayShift"];
                    list.push(list_json);
                    list_json = { 'workID': "", "pmOpName": "", "dayShift": "", "plannedqty": "" };
                }
                _this.get_plan_state(planList, JSON.stringify(list));
            });
        },//获取所选的设备下的所有工单
        get_plan_state: function (planList, list) {
            var _this = this;
            this.plan_list = { undone: [], finish: [] };
            $.post('/Phone/Get_mes_state', { "planlist": list }).done(function (stateData) {
                if (stateData.length != 0) {
                    for (var i = 0; i < stateData.length; i++) {
                        for (var j = 0; j < planList.length; j++) {
                            planList[j]["StartTime"] = _this.formatTime(planList[j]["planStartTime"]);
                            planList[j]["finishedQty"] = planList[j]["finishedQty"] == null ? 0 : planList[j]["finishedQty"];
                            if (planList[j]["itemAttr1"] == null || planList[j]["itemAttr1"] == "") {
                                planList[j]["itemAttr1"] = "无";
                            }
                            planList[j]["StartTime"] = _this.formatTime(planList[j]["planStartTime"]);
                            if (planList[j]["workID"] == stateData[i]["workID"] && planList[j]["pmOpName"] == stateData[i]["opname"] && planList[j]["dayShift"] == stateData[i]["dayShift"] && planList[j]['pmResName'] == stateData[i]["pmResName"] ) {
                                planList[j]["taskFinishState"] = stateData[i]["state"];
                                planList[j]["badQty"] = stateData[i]["badQty"];
                                if (parseInt(stateData[i]["plannedqty"])) {
                                    planList[j]["plannedqty"] = parseInt(stateData[i]["plannedqty"]);
                                }
                                if (parseInt(stateData[i]["finishqty"])) {
                                    planList[j]["finishedQty"] = parseInt(stateData[i]["finishqty"]);
                                }
                            }
                        }
                    }
                    for (var i = 0; i < planList.length; i++) {
                        planList[i]["StartTime"] = _this.formatTime(planList[i]["planStartTime"]);
                        if (planList[i]["taskFinishState"] == "4") {
                            planList[i]["StateTitle"] = "完结工单";
                            planList[i]["StartTime"] = _this.get_date(planList[i]["planStartTime"]);
                            planList[i]["EndTime"] = _this.get_date(planList[i]["planStartTime"]);
                            _this.plan_list['finish'].push(planList[i])
                        } else {
                            if (planList[i]["taskFinishState"] == "0" || planList[i]["taskFinishState"] == null) {
                                planList[i]["StateTitle"] = "待生产工单";
                            } else if (planList[i]["taskFinishState"] == "2") {
                                planList[i]["StateTitle"] = "换线中工单";
                            } else if (planList[i]["taskFinishState"] == "3") {
                                planList[i]["StateTitle"] = "生产中工单";
                                _this.cur_product_work = planList[i];
                            } else {
                                planList[i]["StateTitle"] = "暂停中工单";
                            }
                            _this.plan_list['undone'].push(planList[i]);
                        }
                    }
                } else {
                    for (var i = 0; i < planList.length; i++) {
                        planList[i]["StartTime"] = _this.formatTime(planList[i]["planStartTime"]);
                        planList[i]["finishedQty"] = planList[i]["finishedQty"] == null ? 0 : planList[i]["finishedQty"];
                        if (planList[i]["itemAttr1"] == null || planList[i]["itemAttr1"] == "") {
                            planList[i]["itemAttr1"] = "无";
                        }
                        if (planList[i]["taskFinishState"] == "4") {
                            planList[i]["StateTitle"] = "完结工单";
                            _this.plan_list['finish'].push(planList[i]);
                        } else {
                            planList[i]["StateTitle"] = "待生产工单";
                            _this.plan_list['undone'].push(planList[i]);
                        }
                    }
                }
            });
        },//获取工单的状态，1和null是待生产工单，2是切换中工单，3是生产中工单，4是完结工单，5是暂停中工单
        res_group_click: function (index, e) {
            this.cur_plan = { workID: "无", jobQty: "无", pmResName: "无", productID: "无", itemAttr1: "无", pmOpName: "无" };
            this.btn_start = "开始报工";
            var value = this.child_active[index] == true ? false : true;
            this.$set(this.child_active, index, value);
            if (value) {
                this.plan_list = { undone: [], finish: [] };
                $(".child_active").removeClass("child_active");
                $(e.currentTarget).parent().find('.res_view p').eq(0).find('a').addClass("child_active");//默认第一个点击状态
                this.tag_event(e.currentTarget.innerText)//查看点击过后的事件
                this.get_plan($(e.currentTarget).parent().find('.res_view p').eq(0).find('a').text());
            }
            this.cur_plan["pmResName"] = this.menu_list[index]["resName"][0];
            this.tag_event(this.cur_plan['pmResName']);
            this.get_cur_product(this.cur_plan['pmResName'])
        },//设备组的点击事件
        res_view_click: function (resName,e) {
            $(".child_active").removeClass("child_active");
            $(e.target).addClass("child_active");
            this.content_title = e.target.text;
            this.cur_plan["pmResName"] = e.target.text;
            this.get_plan(e.target.text);
            this.tag_event(e.target.text);
            this.get_cur_product(e.target.text);
        },//设备组下面的设备的点击事件
        formatTime: function (value) {
            value = value.replace(/\-/g, '/')
            var date = new Date(value);
            var hour = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
            var min = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
            return hour + ":" + min;
        },//格式化时间格式
        undone_plan_click: function (index, e) {
            $(".order_active").removeClass("order_active");
            $(e.currentTarget).addClass("order_active");
            this.cur_planIndex = index;
            this.set_undone_attr(index);
        },//所有待生产工单的点击事件
        set_undone_attr: function (index, e) {
            //设置点击了计划的详情
            this.cur_plan = this.plan_list['undone'][index];
            if (index == this.plan_list['undone'].length - 1) {
                //判断是不是点击的最后一个工单
                this.next_workId = "无";
            } else {
                this.next_workId = this.plan_list['undone'][index + 1]['workID'];
            }
            if (this.plan_list['undone'][index]['taskFinishState'] == null) {
                this.btn_start = "开始换线";
                this.start_work = "开始换线";
            } else if (this.plan_list['undone'][index]['taskFinishState'] == '2') {
                this.btn_start = "结束换线";
                this.start_work = "结束换线";
            } else if (this.plan_list['undone'][index]['taskFinishState'] == "3") {
                this.btn_start = "开始报工";
                this.start_work = "开始报工";
            } else if (this.plan_list['undone'][index]['taskFinishState'] == "5") {
                this.btn_start = "开始生产";
                this.start_work = "开始生产";
            }
        },//设置信息框里面的内容
        enter_click: function () {
            if (this.cur_plan["StateTitle"] == "待生产工单") {
                this.ReportEvent(1, "", 0, "", this.nowTime(0), "", "", "", "换线中工单", "换线结束");
                $('#myModal').modal('hide')
            } else if (this.cur_plan["StateTitle"] == "换线中工单") {
                this.ReportEvent(2, "", 0, "", this.nowTime(0), "", "", "", "生产中工单", "开始报工");
                $('#myModal').modal('hide')
            } else if (this.cur_plan["StateTitle"] == "生产中工单") {
            } else if (this.plan_list["undone"][this.cur_planIndex]["taskFinishState"] == "5") {
                if (this.has_change()) {
                    this.ReportEvent(2, "",0, "", this.nowTime(0), "", "", "", "生产中工单", "开始报工");
                    $('#myModal').modal('hide')
                }
            }
        },//模态框里面的确定按钮被点击的事件
        has_change: function () {
            //判断是否有订单在生产
            for (var i = 0; i < this.plan_list.undone.length; i++) {
                if (this.plan_list.undone[i]["StateTitle"] == "换线中工单" || this.plan_list.undone[i]["StateTitle"] == "生产中工单") {
                    if (i !== this.cur_planIndex) {
                        this.model_title = "错误提示";
                        this.model_content = "当前设备有订单再换线或生产";
                        this.model_enter = false;
                        $('#myModal').modal('show');
                        return false;
                    }
                }
            }
            return true;
        },//判断是否有切换的工单,或者生产中的工单
        btn_change: function () {
            if (this.cur_plan["workID"] == "无") {
                this.model_title = "请选择一个工单再操作";
                this.model_content = '当前设备:' + this.cur_plan['pmResName'];
                this.model_enter = false;
                $('#myModal').modal('show');
                return;
            }
            //开始报工的那个按钮被点击的时候触发的事件
            if (this.plan_list['undone'][this.cur_planIndex]["taskFinishState"] != "3") {
                if (this.has_change()) {
                    this.model_title = "当前工单为:" + this.cur_plan['workID'];
                    this.model_content = "是否要" + this.btn_start;
                    this.model_enter = true;
                    $('#myModal').modal("show")
                }
            }
            else {
                $('#exampleModal').modal("show");
                this.end_time = this.nowTime(0);
                this.finish_qty = this.cur_plan['plannedqty']
                this.work_start_time();
            }
        },//change 开始报工的按钮点击事件
        ReportEvent: function (BtnState, FinishedQty, BadQty, ScrappedQty, MesStartTime, MesEndTime, MesOperator, ErrorAttr, type, btnText) {
            console.log(MesOperator)
            //提示框里面的确定按钮被点击的时候触发的事件
            var _this = this;
            var reportData = {};
            reportData.BtnState = BtnState;
            reportData.ResName = this.cur_plan["pmResName"];
            reportData.WorkID = this.cur_plan["workID"];
            reportData.OpName = this.cur_plan["pmOpName"];
            reportData.ProductID = this.cur_plan["productID"];
            reportData.Description = this.cur_plan["itemAttr1"];
            reportData.JobQty = this.cur_plan["jobQty"];
            reportData.FinishedQty = FinishedQty;
            reportData.ScrappedQty = ScrappedQty;
            reportData.BadQty = BadQty;
            reportData.MesOperator = MesOperator;
            reportData.MesStartTime = MesStartTime;
            reportData.MesEndTime = MesEndTime;
            reportData.dayShift = this.cur_plan["dayShift"];
            reportData.PlannedQty = this.cur_plan["plannedqty"];
            reportData.ErrorAttr = ErrorAttr;
            console.log(reportData)
            $.post('/Phone/BtnReportEvent', { ReportData: JSON.stringify(reportData) }).done(function (redata) {
                console.log(redata)
                _this.event_json["workID"] = reportData.WorkID;
                _this.event_json["description"] = reportData.Description;
                _this.event_json["mesOperator"] = redata[3];
                _this.event_json["mesEndTime"] = _this.nowTime(0);
                bad = parseInt(_this.plan_list["undone"][_this.cur_planIndex]["BadQty"]) + parseInt(BadQty);
                if (redata[2] == "2") {
                    Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "StateTitle", type);
                    Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "taskFinishState", redata[2]);
                    _this.btn_start = btnText;
                    _this.event_json["eventMessage"] = "开始生产切换";
                    _this.event_json["eventName"] = "StartProduct";
                } else if (redata[2] == "3") {
                    Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "StateTitle", type);
                    Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "taskFinishState", redata[2]);
                    _this.btn_start = btnText;
                    _this.event_json["eventMessage"] = "结束生产切换";
                    _this.event_json["eventName"] = "EndSetup";
                    if (redata[1] == "报工成功。") {
                        var finishedQty = parseInt(_this.finish_qty);
                        var plannedqty = parseInt(_this.plan_list["undone"][_this.cur_planIndex]["plannedqty"]) - finishedQty - parseInt(_this.bad_qty);
                        finishedQty = _this.plan_list["undone"][_this.cur_planIndex]["finishedQty"] + finishedQty;
                        Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "finishedQty", finishedQty);
                        Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "plannedqty", plannedqty); 
                        Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "BadQty", bad);
                        _this.event_json["eventMessage"] = "报工";
                        _this.event_json["eventName"] = "StartProduct";
                    }
                    $('#exampleModal').modal("hide");
                } else if (redata[2] == "4") {
                    _this.warn_box = true;
                    _this.work_start = true;
                    _this.starttime = "";
                    _this.endtime = "";
                    Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "StateTitle", "完结工单");
                    Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "BadQty", bad);
                    _this.plan_list["finish"].push(_this.plan_list["undone"].splice(_this.cur_planIndex, 1)[0]);
                    $('#exampleModal').modal("hide");
                    _this.event_json["eventMessage"] = "报工";
                    _this.event_json["eventName"] = "StartProduct";
                } else if (redata[2] == "5") {
                    _this.warn_box = true;
                    _this.parse_box = true;
                    Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "StateTitle", type);
                    Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "taskFinishState", redata[2]);
                    Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "BadQty", BadQty); 
                    _this.btn_start = btnText;
                    _this.event_json["eventMessage"] = "暂停生产";
                    _this.event_json["eventName"] = "StartRest";
                }
                _this.event_list.unshift(_this.event_json);
                _this.event_json = { workID: "", eventMessage: "", eventName: "" };
            });
        },//按钮的提交
        nowTime: function (value) {
            //获取当前的时间
            var date;
            if (value == 0) {
                date = new Date();
            } else {
                date = new Date(value);
            }
            var year = date.getFullYear();
            var months = date.getMonth() + 1;
            var month = months > 9 ? months : "0" + months;
            var day = date.getDate() > 9 ? date.getDate() : "0" + date.getDate();
            var hour = date.getHours() > 9 ? date.getHours() : "0" + date.getHours();
            var Minutes = date.getMinutes() > 9 ? date.getMinutes() : "0" + date.getMinutes();
            var Seconds = date.getSeconds() > 9 ? date.getSeconds() : "0" + date.getSeconds();
            return year + "/" + month + "/" + day + " " + hour + ":" + Minutes + ":" + Seconds;
        },//获取现在的时间
        btn_startWork: function () {
            //if (this.start_options_verification()) {
                this.ReportEvent(3, this.finish_qty, this.bad_qty, '0', this.start_time, this.end_time, this.opera_person, "", "生产中工单", "开始报工");
            //}
            
        },//确定报工的点击事件
        work_start_time: function () {
            var _this = this;
            $.get("/Phone/WorkOrderStartTime", { resname: this.cur_plan["pmResName"], timeType: "E" }).done(function (time) {
                _this.start_time = time;
            });
        },//报工的开始时间
        nowTime: function (value) {
            //获取当前的时间
            var date;
            if (value == 0) {
                date = new Date();
            } else {
                date = new Date(value);
            }
            var year = date.getFullYear();
            var months = date.getMonth() + 1;
            var month = months > 9 ? months : "0" + months;
            var day = date.getDate() > 9 ? date.getDate() : "0" + date.getDate();
            var hour = date.getHours() > 9 ? date.getHours() : "0" + date.getHours();
            var Minutes = date.getMinutes() > 9 ? date.getMinutes() : "0" + date.getMinutes();
            var Seconds = date.getSeconds() > 9 ? date.getSeconds() : "0" + date.getSeconds();
            return year + "/" + month + "/" + day + " " + hour + ":" + Minutes + ":" + Seconds;
        },//获取现在的时间
        get_date: function (value) {
            var date = new Date(value);
            var month = date.getMonth() + 1;
            var day = date.getDate();
            return month + "月" + day + "日";
        },//格式化时间格式 （月/日）
        btn_parse: function () {
            if (this.cur_plan["workID"] == "无" && this.cur_plan["pmOpName"] == "无") {
                this.model_title = '请选择一张工单后再操作';
                this.model_content = '当前设备' + this.cur_plan["pmResName"];
                this.model_enter = false;
                $('#myModal').modal('show');
            } else if (this.cur_plan["taskFinishState"] == "5") {
                this.model_title = '当前工单是暂停状态';
                this.model_content = '请选择其他工单';
                this.model_enter = false;
                $('#myModal').modal('show');
            } else {
                $('#parse_options').modal('show');
            }

        },//暂停生产的按钮点击事件
        parse_enter: function () {
            if (this.parse_options_verification()) {
                this.ReportEvent(5, "", 0, "", "", "", "", this.ErrorAttr, '暂停中工单', "开始生产");
            }
            $('#parse_options').modal('hide');
        },//暂停的模态框的点击事件
        parse_options_verification: function () {
            if (this.ErrorAttr == "") {
                this.model_title = "错误提示";
                this.model_content = "请输入暂停生产的原因";
                this.model_enter = false
                $("#myModal").modal("show");
                return false;
            } else {
                return true;
            }
        },//暂停的的信息验证
        start_options_verification: function () {
            //验证end_time
            if (/^\d{4}-\d{2}-\d{2} d{2}:d{2}:d{2}$/.test(this.end_time))
            var r = this.end_time.match(/^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2})$/);
            //验证finish_qty
            if (this.finish_qty == "") {
                this.model_title = "错误提示";
                this.model_content = "请输入完成数量";
                $('#myModal').modal('show');
                this.model_enter = false;
            }
            //验证bad_qty
            return false;
        },//开始报工的信息验证
        tag_event: function (resName) {
            var _this = this;
            $.get('/Phone/Tag_Event', { res_name: resName }).then(function (resEvent) {
                _this.event_list = resEvent;
                console.log(resEvent)
            })
        },//获取经过的事件
        render_echart: function () {
            var myChart = echarts.init(document.getElementById('main'));
            option = {
                series: [
                    {
                        type: 'pie',
                        radius: ['70%', '80%'],
                        label: {
                            show: false,
                            position: 'center'
                        },
                        emphasis: {
                            label: {
                                show: true,
                                position: 'center',
                                formatter: function () {
                                    return  "线体综合量率\r\n 60%"
                                },
                                textStyle: {
                                    fontSize: 14,
                                    color:'green'
                                }
                            }
                        },
                        labelLine: {
                            show: false
                        },
                        data: [
                            { value: 90, name: '线体综合良率' },
                            { value: 10, name: '' },
                        ]
                    }
                ]
            };
            myChart.dispatchAction({
                type: 'highlight', 
                seriesIndex: 1,
                dataIndex: 1
            });
            myChart.setOption(option);
            window.onresize = function () {
                myChart.resize();
            }
        },//渲染echart,读取线体综合率
        get_cur_product: function (resName) {
            console.log(123)
            var _this = this;
            $.get('/Phone/Get_Cur_Product', { res_name: resName }).done(function (product_data) {
                _this.cur_product_work = product_data;
            })
        }//获取当前设备下的生产中的工单
    }
});