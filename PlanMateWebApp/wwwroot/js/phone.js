var vm = new Vue({
    el: "#wrapper",
    data: {
        resLi: true,//设备分组是否是点击状态
        res_group: "无设备",
        tab_left: true,//选项卡左边是否打开
        tab_right: false,//选项卡右边是否打开
        cur_plan: {},//当前选中的计划
        cur_workId: "无",//当前的工单
        cur_productId: "无",//当前的料号
        cur_planIndex: null,//点击的是哪一个计划
        product_num: "无",//产品的数量
        process_name: "无",//工序的名称
        product_attr: "无",//产品的描述
        next_workId: "无",//预备工单
        btn_start: "开始报工",//按钮的文字
        btn_error: "异常停机",//异常停机的按钮
        menu_list: [],//菜单列表
        child_active: [],//判断哪些子集的标签被点击
        content_title: "",//tab标题
        plan_list: { undone: [], finish: [] },//当前的计划
        warn_box: true,//包裹提示框和报工选项框的盒子
        alert_box:true,//提示框是否是显示,默认是关闭的
        warn_text_title: "",//提示框里面的标题
        warn_text_content: "是否要切换此工单",//提示框里面的内容
        enter_state: true,//提示框内的确定按钮是否是正常的状态
        work_start: true,//开始报工框是否是关闭状态,默认是关闭的
        starttime: "",//开始时间,开始报工的时候的开始时间
        endtime: "2020/01/01 24:00:00",//结束时间,开始报工的结束时间
        finishnum: "",//开始报工工单完成数量
        failnum: 0,//开始报工工单不良的数量,默认情况下是0
        parse_box: true,//判断暂停的选项框是否是显示的状态默认情况下是关闭的
        ErrorAttr:"",//工单暂停的原因
    },
    created: function () {
        this.getMenu();//获取菜单按钮
    },
    methods: {
        getMenu: function () {
            //获取菜单
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
                _this.content_title = _this.menu_list[0]['resName'][0];
                _this.get_plan(_this.menu_list[0]['resName'][0]);
            });

        },//获取菜单
        res_div_click: function (e) {
            // 菜单按钮设备列表被点击的时候触发
            if (!e.target.id) {
                this.resLi = this.resLi === true ? false : true;
            }
        },
        res_group_click: function (index, e) {
            //设备组被点击的时候触发
            var value = this.child_active[index] == true ? false : true;
            this.$set(this.child_active, index, value);
            if (value) {
                this.plan_list = { undone: [], finish: [] };
                $(".Child_active").removeClass("Child_active");
                $(e.currentTarget).parent().find('.res p').eq(0).find('a').addClass("Child_active");//默认第一个点击状态
                this.content_title = $(e.currentTarget).parent().find('.res p').eq(0).find('a').text();
                this.get_plan($(e.currentTarget).parent().find('.res p').eq(0).find('a').text());
            }

        },//设备组的点击事件
        tab_left_click: function () {
            //选项卡左边的点击事件
            this.tab_left = true;
            this.tab_right = false;
        },//生产工单的切换按钮点击事件
        tab_right_click: function () {
            //选项卡右边的点击事件
            this.tab_left = false;
            this.tab_right = true;
        },//完结工单切换按钮点击事件
        produced_content_click: function (e) {
            // 左边每个工单点击的时候触发
            $('.Active').removeClass('Active');
            $(e.currentTarget).addClass('Active');
        },
        undone_plan_click: function (index, e) {
            //未完成的工单的点击事件
            $('.Active').removeClass('Active');
            $(e.currentTarget).addClass('Active');
            this.cur_planIndex = index;
            this.set_undone_attr(index);
        },//所有待生产工单的点击事件
        set_undone_attr: function (index) {
            //设置点击了计划的详情
            this.cur_plan = this.plan_list['undone'][index];
            this.cur_workId = this.plan_list['undone'][index]['workID'];
            this.cur_productId = this.plan_list['undone'][index]['productID'];
            this.product_num = this.plan_list['undone'][index]['jobQty'];
            this.process_name = this.plan_list['undone'][index]['pmOpName'];
            this.product_attr = this.plan_list['undone'][index]['itemAttr1'];
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
        get_plan: function (resname) {
            var _this = this;
            var dayShift = 1;
            var list = [];//判断计划的状态
            $.post('/Phone/Get_plan', { 'resname': resname }).done(function (planList) {
                var list_json = { 'workID': "", "pmOpName": "", "DayShift": "", "plannedqty": "" };
                for (var i = 0; i < planList.length; i++) {
                    if (i != 0 && planList[i]["productID"] == planList[i - 1]["productID"] && planList[i]["pmOpName"] == planList[i - 1]["pmOpName"]) {
                        dayShift++;
                    } else {
                        dayShift = 1;
                    }
                    planList[i]["DayShift"] = dayShift;
                    list_json["workID"] = planList[i]["workID"];
                    list_json["pmOpName"] = planList[i]["pmOpName"];
                    list_json["DayShift"] = planList[i]["DayShift"];
                    list_json["plannedqty"] = planList[i]["plannedqty"];
                    //list_json["DayShift"] = planList[i]["DayShift"];
                    list.push(list_json);
                    list_json = { 'workID': "", "pmOpName": "", "DayShift": "", "plannedqty":""};
                }
                _this.get_plan_state(planList, JSON.stringify(list))
            })
        },//获取所选的设备下的所有工单
        child_res_click: function () {
            this.plan_list = { undone: [], finish: [] };
            this.cur_workId = "无";
            this.cur_productId = "无";
            this.product_num = "无";
            this.process_name = "无";
            this.product_attr = "无";
            this.next_workId = "无";
        },
        Menuevent: function (e) {
            $(".Child_active").removeClass("Child_active");
            $(e.target).addClass("Child_active");
            this.content_title = e.target.text;
            this.get_plan(e.target.text);
        },//设备组下面的设备的点击事件
        get_plan_state: function (planList, list) {
            //获取工单的状态
            var _this = this;
            $.post('/Phone/Get_mes_state', { "planlist": list }).done(function (stateData) {
                if (stateData.length != 0) {
                    for (var i = 0; i < stateData.length; i++) {
                        for (var j = 0; j < planList.length; j++) {
                            planList[j]["StartTime"] = _this.formatTime(planList[j]["planStartTime"]);
                            if (planList[j]["itemAttr1"] == null || planList[j]["itemAttr1"] == "") {
                                planList[j]["itemAttr1"] = "无";
                            }
                            planList[j]["StartTime"] = _this.formatTime(planList[j]["planStartTime"]);
                            if (planList[j]["workID"] == stateData[i]["workID"] && planList[j]["pmOpName"] == stateData[i]["opname"] && planList[j]["DayShift"] == stateData[i]["dayShift"]) {
                                planList[j]["taskFinishState"] = stateData[i]["state"];
                            }
                        }
                    }
                    for (var i = 0; i < planList.length; i++) {
                        planList[i]["StartTime"] = _this.formatTime(planList[i]["planStartTime"]);
                        if (planList[i]["taskFinishState"] == "4") {
                            planList[i]["StateTitle"] = "完结工单";
                            _this.plan_list['finish'].push(planList[i])
                        } else {
                            if (planList[i]["taskFinishState"] == "0" || planList[i]["taskFinishState"] == null) {
                                planList[i]["StateTitle"] = "待生产工单";
                            } else if (planList[i]["taskFinishState"] == "2") {
                                planList[i]["StateTitle"] = "换线中工单";
                            } else if (planList[i]["taskFinishState"] == "3") {
                                planList[i]["StateTitle"] = "生产中工单";
                                console.log(planList[i]);
                            } else {
                                planList[i]["StateTitle"] = "暂停中工单";
                            }
                            _this.plan_list['undone'].push(planList[i]);
                        }
                    }
                } else {
                    for (var i = 0; i < planList.length; i++) {
                        planList[i]["StartTime"] = _this.formatTime(planList[i]["planStartTime"]);
                    
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
                console.log(planList)
            });
        },//获取工单的状态
        btn_click: function () {
            //change开始报工的按钮被点击的事件
            if (JSON.stringify(this.cur_plan) == "{}") {
                this.alert_box = false;
                this.show_warn_box("错误提示", "请选择一个工单再操作", false);
                this.enter_state = false;
                return;
            }
            //开始报工的那个按钮被点击的时候触发的事件
            if (this.plan_list['undone'][this.cur_planIndex]["taskFinishState"] != "3") {
                this.alert_box = false;
                    this.show_warn_box("当前工单为:" + this.cur_workId, "是否要" + this.btn_start, false);
            } 
            else {
                //报工窗口展示
                this.alert_box = true;
                this.warn_box = false;
                this.work_start = false;
                this.endtime = this.nowTime(0);//获取当前的时间
                this.finishnum = this.cur_plan["plannedqty"];
                this.work_start_time()//获取工单的开始时间
            }
        },//change 开始报工按钮的点击事件
        work_start_time: function () {
            //获取工单的开始时间
            var _this = this;
            $.get("/Phone/WorkOrderStartTime", { resname: this.cur_plan["pmResName"], timeType: "E" }).done(function (time) {
                _this.starttime = time;
            });
        },//报工的开始时间
        btn_parse: function () {
            if (this.cur_workId == "无" && this.cur_productId == "无" && this.product_num == "无" && this.process_name == "无" && this.product_attr == "无") {
                this.alert_box = false;
                this.show_warn_box("错误提示", "请选择一个工单再操作", false);
                this.enter_state = false;
            } else {
                this.warn_box = false;
                this.parse_box = false;  
            }
        },//暂停按钮的点击事件
        parse_cancel: function () {
            //暂停选项的返回按钮的点击事件
            this.warn_box = true;
            this.parse_box = true;
        },//取消暂停按钮的点击事件
        enter_parse: function () {
            //确定暂停按钮的点击事件
        
            this.ReportEvent(5, "", "", "", "", "", "", this.ErrorAttr,'暂停中工单',"开始生产");
        },//确定暂停的点击事件
        enter_click: function () {
            //提示框里面的确定按钮点击时候触发
            if (this.enter_state) {
                if (this.cur_plan["StateTitle"] == "待生产工单") {
                    if (this.has_change()) {
                        this.ReportEvent(1, "", "", "", this.nowTime(0), "", "", "", "换线中工单", "换线结束");
                        this.warn_box = true;
                        this.alert_box = true;
                    }
                } else if (this.cur_plan["StateTitle"] == "换线中工单") {
                    this.ReportEvent(2, "", "", "", this.nowTime(0), "", "", "", "生产中工单", "开始报工");
                    this.warn_box = true;
                    this.alert_box = true;
                } else if (this.cur_plan["StateTitle"] == "生产中工单") {
                } else if (this.plan_list["undone"][this.cur_planIndex]["taskFinishState"] == "5") {
                    
                    if (this.has_change()) {
                        console.log("here")
                        this.ReportEvent(2, "", "", "", this.nowTime(0), "", "", "", "生产中工单", "开始报工");
                    }
                   
                } else if (this.cur_plan["StateTitle"]) {

                }
            } else {
                this.enter_state = true;
                this.show_warn_box("", "", true);
            }
        },//提示框确定按钮的点击事件
        cancel_startWork: function () {
            //取消报工
            this.warn_box = true;
            this.work_start = true;
        },//取消报工的点击事件
        btn_startWork: function () {
            //开始报工按钮
            this.ReportEvent(3, this.finishnum, this.failnum, '0', this.starttime, this.endtime, 'yyy',"","生产中工单", "开始报工");
        },//确定报工的点击事件
        has_change: function () {
            //判断是否有订单在生产
            for (var i = 0; i < this.plan_list.undone.length; i++) {
                if (this.plan_list.undone[i]["StateTitle"] == "换线中工单" || this.plan_list.undone[i]["StateTitle"] == "生产中工单") {
                    this.show_warn_box("错误提示", "当前有订单在换线或在生产");
                    this.enter_state = false;
                    return false;
                }
            }
            return true;
        },//判断是否有切换烘的工单,后者生产中的工单
        ReportEvent: function (BtnState, FinishedQty, FailedQty, ScrappedQty, MesStartTime, MesEndTime, MesOperator, ErrorAttr, type, btnText) {
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
            reportData.FailedQty = FailedQty;
            reportData.MesOperator = MesOperator;
            reportData.MesStartTime = MesStartTime;
            reportData.MesEndTime = MesEndTime;
            reportData.DayShift = this.cur_plan["DayShift"];
            reportData.plannedqty = this.cur_plan["plannedqty"];
            reportData.ErrorAttr = ErrorAttr;
            $.post('/Phone/BtnReportEvent', { ReportData: JSON.stringify(reportData) }).done(function (redata) {
                console.log(redata)
                if (redata[2] == "2") {
                    Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "StateTitle", type);
                    Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "taskFinishState", redata[2]);
                    _this.btn_start = btnText;
                } else if (redata[2] == "3") {
                    _this.warn_box = true;
                    _this.work_start = true;
                  
                    Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "StateTitle", type);
                    Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "taskFinishState", redata[2]);
                    //Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "taskFinishState", redata[2]);
                    _this.btn_start = btnText;
                    if (redata[1] == "报工成功。") {
                        var finishedQty = parseInt(_this.finishnum);
                        var plannedqty = parseInt(_this.plan_list["undone"][_this.cur_planIndex]["plannedqty"]) - finishedQty - parseInt(_this.failnum);
                        Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "finishedQty", finishedQty);
                        Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "plannedqty", plannedqty);
                    }
                } else if (redata[2] == "4") {
                    _this.warn_box = true;
                    _this.work_start = true;
                    _this.starttime = "";
                    _this.endtime = "";
                    Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "StateTitle", "完结工单");
                    _this.plan_list["finish"].push(_this.plan_list["undone"].splice(_this.cur_planIndex, 1)[0]);
                } else if (redata[2] == "5") {
                    _this.warn_box = true;
                    _this.parse_box = true;
                    Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "StateTitle", type);
                    Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "taskFinishState", redata[2]);
                    _this.btn_start = btnText;
                }
            });
        },//按钮的提交
        cancel_click: function () {
            //提示框里面的取消按钮被点击的时候触发
            this.enter_state = this.enter_state == false ? true : true;
            this.show_warn_box("", "", true);
        },//提示框里面的取消按钮
        show_warn_box: function (title, content, warn_state) {
            //设置提示框标题以及确定按钮是否是正常的状态
            this.warn_box = warn_state;
            this.warn_text_title = title;
            this.warn_text_content = content;
            if (warn_state) {
                $('.warn_box').css({ opacity:1});
            }
        },//提示框里面的内容
        formatTime: function (value) {
            //格式化时间格式
            var date = new Date(value);
            var hour = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
            var min = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
            return hour + ":" + min;
        },//格式化时间格式
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
    }
});