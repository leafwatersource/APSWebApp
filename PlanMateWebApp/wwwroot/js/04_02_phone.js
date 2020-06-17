var vm = new Vue({
    el: "#Wrapper",
    components: {
        messageBox: messageBox,
        featuresBox: featuresBox,
        modelBox: modelBox,
        startMode: startMode,
        parseBox: parseBox,
        alertBox: alertBox,
        exceptionHandling: exceptionHandling
    },
    data: {
        angle_icon_open: false,//判断是否点击了设备列表
        menu_close: false,//判断是否是折叠按钮
        tab_right: false,//判断是否是点击了右边的切换按钮
        res_list_open: [],//判断设备组是否被点击
        component_item: {},//显示的组件
        component: false,//组件框是否显示
        menu_list: [],//菜单列表
        cur_plan: {},//点击的当前设备下的当前计划
        btn_start: "开始换线",//按钮的文字，默认是开始换线
        child_active: [],//判断哪些设备组的标签被点击
        plan_list: { undone: [], finish: [] },//当前的设备下的所有计划
        content_title: "",//当前的设备名称
        user_message: {},//用户的信息
        cur_planIndex: null,//当前点击的工单的索引
        cur_product_work: {},//当前设备下的生产中工单
        alert_title:"",//提示框里面的标题
    },
    created: function () {
        this.set_nav_attr();
        this.set_loading();
        this.getMenu();
        this.get_user_message();
    },
    methods: {
        set_loading: function () {
            window.onload = function () {
                var width = document.documentElement.clientWidth;
                if (width < 768) {
                    $('#Wrapper').html('<h3 style="position: absolute;left:50%;top:50%;-webkit-transform: translate(-50%,-50%);-moz-transform: translate(-50%,-50%);-ms-transform: translate(-50%,-50%);-o-transform: translate(-50%,-50%);transform: translate(-50%,-50%);">请使用paid或电脑打开网页</h3>').show();
                } else {
                    $('#Wrapper').show();
                }
            };
        },//默认加载的动画
        set_nav_attr: function () {
            var width = document.documentElement.clientWidth;
            var _this = this;
            if (width <= 1024 || width <= 1366 || width <= 768) {
                if (Math.abs(window.orientation) == 0) {
                    _this.menu_close = true;
                } else {
                    _this.menu_close = false;
                }

                window.addEventListener("orientationchange", function () {
                    //检测浏览器横竖屏
                    width = document.documentElement.clientWidth;
                    if (width <= 1024 || width <= 1366 || width <= 768) {
                        console.log(Math.abs(window.orientation))
                        if (Math.abs(window.orientation) == 0) {
                            _this.menu_close = true;

                        } else {

                            _this.menu_close = false;

                        }
                        console.log(_this.menu_close)
                    }
                });
            }
            window.onresize = function () {
                //监听窗口大小改变
                width = document.documentElement.clientWidth;
                if (width <= 1024 || width <= 1366 || width <= 768) {
                    window.addEventListener("orientationchange", function () {
                        // Announce the new orientation number
                        if (Math.abs(window.orientation) == 0) {

                            _this.menu_close = true;
                        } else {
                            _this.menu_close = false;
                        }
                    }, false);

                }
            }
        },//判断设备是否是小于1366的小于1344使用隐藏导航栏的样式
        tab_left_click: function (e) {
            1
            $('.tab_active').removeClass('tab_active');
            $(e.currentTarget).addClass('tab_active');
            this.tab_right = false;
        },//左边的切换按钮的点击事件
        tab_right_click: function (e) {
            $('.tab_active').removeClass('tab_active');
            $(e.currentTarget).addClass('tab_active');
            this.tab_right = true;
        },//右边的切换按钮的点击事件
        undone_work_li_click: function (undoneIndex, e) {
            this.curIndex = undoneIndex;
            this.btn_start = "开始换线";
            this.cur_plan = this.plan_list['undone'][undoneIndex];
            $('.undone_active').removeClass('undone_active');
            $(e.currentTarget).addClass('undone_active');
            this.set_btn_text();
        },//未完成的工单的点击事件
        set_btn_text: function () {
            if (this.cur_plan['taskFinishState'] == null) {
                this.btn_start = "开始换线";
            } else if (this.cur_plan['taskFinishState'] == '2') {
                this.btn_start = "结束换线";
            } else if (this.cur_plan['taskFinishState'] == "3") {
                this.btn_start = "开始报工";
            } else if (this.cur_plan['taskFinishState'] == "5") {
                this.btn_start = "开始生产";
            }
        },
        res_list_click: function (index,e) {
            this.angle_icon_open = this.angle_icon_open === true ? false : true;
        },//设备列表的点击事件
        res_click: function (e) {
            this.cur_plan = {};
            this.btn_start = "开始换线";
            var res = $(e.currentTarget).text();
            $('.resourceActive').removeClass('resourceActive');
            $(e.currentTarget).find('p').eq(0).addClass('resourceActive');
            this.content_title = res;
            this.get_plan(res);
            this.get_cur_product(res);
           
        },//设备的点击事件
        sc_menu: function () {
            this.menu_close = this.menu_close === true ? false : true;
        },//展开或者折叠菜单
        components_btn_click: function (component_name) {
            if (!this.component_item['component_name']) {
                this.$set(this.component_item, 'component_name', component_name);
                this.component = true;
            } else {
                if (component_name == this.component_item['component_name']) {
                    this.$set(this.component_item, 'component_name', "");
                    this.component = false;
                } else {
                    this.$set(this.component_item, 'component_name', component_name);
                    this.component = true;
                }
            }
        },//组件的显示或隐藏
        close_component: function () {
            this.component_item = {};
            this.component = false;
        },//关闭组件
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
                _this.content_title = _this.menu_list[0]['resName'][0];
              
            });
        },//获取菜单
        res_group_click: function (index, e) {
            this.cur_plan = {};
            $('.resourceActive').removeClass('resourceActive');
            $(e.currentTarget).next().find('li').eq(0).find('p').addClass('resourceActive');
            this.cur_planIndex = 0;
            this.cur_plan = {};
            this.btn_start = "开始换线";
            var value = this.child_active[index] == true ? false : true;
            this.$set(this.child_active, index, value);
            this.cur_plan["pmResName"] = this.menu_list[index]["resName"][0];
            if (value) {
                this.plan_list = { undone: [], finish: [] };
                $(".resActive").removeClass("resActive");
                $(e.currentTarget).parent().addClass("resActive");//默认第一个点击状态
                this.get_plan(this.cur_plan['pmResName']);
            }
            this.content_title = this.menu_list[index]["resName"][0];
            this.get_cur_product(this.cur_plan['pmResName']);
            if (this.plan_list["undone"].length != 0) {
                this.cur_plan = this.plan_list["undone"][0];
                this.set_btn_text();
            }
        },//设备组的点击事件
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
        get_cur_product: function (resName) {
            var _this = this;
            $.get('/Phone/Get_Cur_Product', { res_name: resName }).done(function (product_data) {
                _this.cur_product_work = product_data[0];
                try {
                    _this.cur_product_work["mesStartTime"] = _this.formatTime(_this.cur_product_work["mesStartTime"], false, true);
                    _this.cur_product_work["mesEndTime"] = _this.formatTime(_this.cur_product_work["mesEndTime"], false, true);
                    _this.cur_product_work["mesDate"] = _this.formateDate(_this.cur_product_work["mesDate"]);
                } catch (e) {

                }
            })
        },//获取当前设备下的生产中的工单
         get_plan_state: function (planList, list) {
            var _this = this;
            this.plan_list = { undone: [], finish: [] };
            $.post('/Phone/Get_mes_state', { "planlist": list }).done(function (stateData) {
                if (stateData.length != 0) {
                    for (var i = 0; i < stateData.length; i++) {
                        for (var j = 0; j < planList.length; j++) {
                            planList[j]["StartTime"] = _this.formatTime(planList[j]["planStartTime"]);
                            planList[j]["EndTime"] = _this.formatTime(planList[j]["planendtime"]);
                            planList[j]["finishedQty"] = planList[j]["finishedQty"] == null ? 0 : planList[j]["finishedQty"];
                            if (planList[j]["itemAttr1"] == null || planList[j]["itemAttr1"] == "") {
                                planList[j]["itemAttr1"] = "无";
                            }
                            if (planList[j]["itemAttr2"] == null || planList[j]["itemAttr2"] == "") {
                                planList[j]["itemAttr2"] = "无";
                            }
                            if (planList[j]["itemAttr3"] == null || planList[j]["itemAttr3"] == "") {
                                planList[j]["itemAttr3"] = "无";
                            }
                            if (planList[j]["itemAttr4"] == null || planList[j]["itemAttr4"] == "") {
                                planList[j]["itemAttr4"] = "无";
                            }
                            planList[j]["StartTime"] = _this.formatTime(planList[j]["planStartTime"]);
                            if (planList[j]["workID"] == stateData[i]["workID"] && planList[j]["pmOpName"] == stateData[i]["opname"] && planList[j]["dayShift"] == stateData[i]["dayShift"] && planList[j]['pmResName'] == stateData[i]["pmResName"]) {
                                planList[j]["taskFinishState"] = stateData[i]["state"];
                                planList[j]["badQty"] = stateData[i]["badQty"];
                                if (parseInt(stateData[i]["finishqty"])) {
                                    planList[j]["finishedQty"] = parseInt(stateData[i]["finishqty"]);
                                }
                            }
                        }
                    }
                    for (var i = 0; i < planList.length; i++) {
                        planList[i]["StartTime"] = _this.formatTime(planList[i]["planStartTime"]);
                        planList[i]["EndTime"] = _this.formatTime(planList[i]["planendtime"]);
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
                            } else {
                                planList[i]["StateTitle"] = "暂停中工单";
                            }
                            _this.plan_list['undone'].push(planList[i]);
                        }
                    }
                } else {
                    for (var i = 0; i < planList.length; i++) {
                        planList[i]["StartTime"] = _this.formatTime(planList[i]["planStartTime"]);
                        planList[i]["EndTime"] = _this.formatTime(planList[i]["planendtime"]);
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
                if (_this.plan_list["undone"].length != 0) {
                    _this.cur_plan = _this.plan_list["undone"][0];
                    _this.set_btn_text();
                }
            });
        },//获取工单的状态，1和null是待生产工单，2是切换中工单，3是生产中工单，4是完结工单，5是暂停中工单
        formatTime: function (value, yms, sec) {
            value = value.replace(/\-/g, '/');
            if (yms) {
                return value;
            } else {
                var date = new Date(value);
                var month = date.getMonth() + 1 < 10 ? "0" + parseInt(date.getMonth() + 1) : date.getMonth() + 1;
                var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
                var hour = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
                var min = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
                var sec = date.getSeconds() < 10 ? "0" + date.getSeconds() : date.getSeconds();
                if (sec) {
                    return month + "/" + day + " " + hour + ":" + min +":"+sec;
                }
                else {
                    return month + "/" + day + " " + hour + ":" + min;
                }
               
            }
        },//格式化时间格式
        formateDate: function (value) {
            value = value.replace(/\-/g, "/");
            var date = new Date(value);
            var month = date.getMonth() + 1 >= 10 ? date.getMonth() + 1 : "0" + parseInt(date.getMonth() + 1);
            var day = date.getDate() > 10 ? date.getDate() + 1 : "0" + date.getDate();
            return month + "/" + day;
        },//格式化日期格式
        start_btn: function () {
            if (this.cur_plan['workID']) {
                if (this.cur_plan['taskFinishState'] == null || this.cur_plan['taskFinishState'] == '2' || this.cur_plan['taskFinishState'] == "5") {
                    //"开始换线",换线结束
                    if (this.cur_plan['taskFinishState'] == null || this.cur_plan['taskFinishState'] == '2') {
                        this.hasChange();
                    }
                    this.components_btn_click("modelBox");
                    //this.btn_start = "结束换线";
                } else if (this.cur_plan['taskFinishState'] == "3") {
                    //this.btn_start = "开始报工";
                    this.components_btn_click("startMode");
                } 
            } else {
                this.alert_title = "当前没有工单再生产!";
                this.components_btn_click("alertBox");
            }
            
        },//根据工单的状态展示哪个组件
        get_user_message: function () {
            var _this = this;
            $.get('/Phone/GetUserMessage').done(function (user_message) {
                _this.user_message = user_message[0];
            })
        },//获取用户信息
        ReportEvent: function (BtnState, FinishedQty, BadQty, ScrappedQty, MesStartTime, MesEndTime, ErrorAttr, type, btnText) {
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
            reportData.MesOperator = _this.user_message.empName;
            reportData.MesStartTime = MesStartTime;
            reportData.MesEndTime = MesEndTime;
            reportData.dayShift = this.cur_plan["dayShift"];
            reportData.PlannedQty = this.cur_plan["plannedqty"];
            reportData.ErrorAttr = ErrorAttr;
            $.post('/Phone/BtnReportEvent', { ReportData: JSON.stringify(reportData) }).done(function (redata) {
                _this.btn_start = btnText;
                Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "taskFinishState", redata[2]);
                _this.close_component();
                console.log(redata[2] == "3")
                if (redata[2] == "3") {
                    _this.cur_product_work = _this.cur_plan;
                }
                if (redata[2] == "4") {
                    _this.btn_start = '开始换线';
                    Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "StateTitle", "完结工单");
                    Vue.set(_this.plan_list["undone"][_this.cur_planIndex], "badQty", BadQty);
                    _this.plan_list["finish"].push(_this.plan_list["undone"].splice(_this.cur_planIndex, 1)[0]);
                } else if (redata[2] == "5") {
                    _this.btn_start = '开始生产';
                    _this.btn_start = btnText;
                }
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
            return year + "-" + month + "-" + day + " " + hour + ":" + Minutes + ":" + Seconds;
        },//获取现在的时间
        parse_btn_click: function () {
            if (this.cur_plan['workID']) {
                if (this.cur_plan["taskFinishState"] != "5") {
                    this.components_btn_click("parseBox");
                } else {
                    this.alert_title = "工单已暂停!";
                    this.components_btn_click("alertBox");
                }
            } else {
                this.alert_title = "请选择一张工单!";
                this.components_btn_click("alertBox");
            }
        },//暂停按钮的点击事件
        componentClose: function (e) {
            if (e.target == e.currentTarget) {
                this.close_component();
            }
        },
        hasChange: function () {
            for (var i = 0; i < this.planlist.length;i++) { }
        },//判断是否有换线中或者是生产中的工单

    }
});