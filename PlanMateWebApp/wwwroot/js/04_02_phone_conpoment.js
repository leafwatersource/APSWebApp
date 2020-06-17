// 消息队列组件
var messageBox = {
    props: ["resname"],
    data: function () {
        return {
            message:[]
        }
    },
    template: `
       <div class="message_box">
        <ul>
            <li v-if="message == undefined || message.length <= 0">
                <p class="text-success">消息队列空空如也</p>
            </li>
            <li v-for="item in message" v-else>
                <p class="message_time">
                    <span class="fa fa-tags"></span>
                   <span v-text="item.eventMessage+':'+item.mesEndTime"></span>
                </p>
                <div class="message_content">
                   <p>
                       <span v-text="'工单号码:'+item.workID+';'"></span>
                       <span v-text="'工序:'+item.opName"></span><span v-text="'计划数量:'+item.planQty+';'"></span>
                       <span v-text="'描述:'+item.description+';'"></span>
                       <span v-text="'设备名称:'+item.resName+'.'"></span></p>
                    <div class="message_btn">
                        <button v-text="item.resName"></button>
                        <button v-text="item.mesOperator" v-if="item.mesOperator"></button>
                    </div>
                </div>
            </li>
        </ul>
    </div>
    `,
    created: function () {
        this.get_event_message();
    },
    methods: {
        get_event_message: function () {
            this.message = [];
            var _this = this;
            $.get('/Phone/Tag_Event', { res_name: this.resname }).then(function (resEvent) {
                _this.message = resEvent;
                console.log(_this.message)
                for (var i = 0; i < resEvent.length; i++) {
                   
                }
            })
        }
    },
};
//用户信息组件
var featuresBox = {
    props: ['user'],
    template: `
         <div class="user_message">
        <div class="user_title">
            <p class="user_name" v-text="user.empName+',你好'"></p>
            <p class="user_desc">用户组:MES <span v-text="'电话:'+user.phoneNum"></span></p>
        </div>
        <div class="user_body">
            <ul>
                <li>
                    <span class="fa fa-address-card-o"></span>
                    <span>个人信息修改</span>
                </li>
                <li>
                    <span class="fa fa-address-card-o"></span>
                    <span>用户操作记录</span>
                </li>
                <li>
                    <span class="fa fa fa-envelope-o"></span>
                    <span>客户端信息</span>
                </li>
                <li>
                    <span class="fa fa fa-cog"></span>
                    <span>系统设置</span>
                </li>
            </ul>
        </div>
        <div class="user_footer">
            <div class="user_btn_box">
            <button>换班</button>
            <button class="user_cancel" v-on:click="UserCancel">退出</button>
            </div>
        </div>
    </div>
    `,
    created: function () {
        console.log('虚拟dom render')
    },
    methods: {
        UserCancel: function () {
            var cookies = document.cookie.split(";");
            $.get("/Public/DeleteUserInfo");//清空userModels里面的成员变量
            for (var i = 0; i < cookies.length; i++) {
                var cookie = cookies[i];
                var eqPos = cookie.indexOf("=");
                var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
                document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/";
            }
            if (cookies.length > 0) {
                for (var i = 0; i < cookies.length; i++) {
                    var cookie = cookies[i];
                    var eqPos = cookie.indexOf("=");
                    var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
                    var domain = location.host.substr(location.host.indexOf('.'));
                    document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/; domain=" + domain;
                }
            }
            window.location.href = "/Index/Index";
            window.reload();
        }
    }
        
};

//模态框组件,(包括开始换线)
var modelBox = {
    props: ["plan"],
    template: `
    <div class="modelBox">
            <div class="model_content">
                <div class="model_title">
                    <span class="model_alert" v-if="plan.taskFinishState==null || plan.taskFinishState=='1'">开始换线</span>
                    <span class="model_alert" v-if="plan.taskFinishState=='2'">结束换线</span>
                    <span class="model_alert" v-if="plan.taskFinishState=='5'">开始生产</span>
                    <span class="model_work" v-text="'工单号码:'+plan.workID"></span>
                    <span class="model_icon fa fa-remove" v-on:click="close_componet"></span>
                </div>
                <div class="model_body">
                    <div class="model_body_content">
                        <span class="body_icon fa fa-exclamation-triangle"></span>
                        <p class="model_body_alert_message"><span v-text="'工序名称:'+plan.pmOpName+';'"></span><span v-text="'计划数量:'+plan.plannedqty"></span><span v-text="'工单总数:'+plan.jobQty"></span>
                            <span v-text="'计划开始时间:'+plan.planStartTime"></span>
                            <span v-text="'计划结束时间:'+plan.planendtime"></span>
                            <span v-text="'属性:'+plan.itemAttr1"></span>
                        </p>
                    </div>
                </div>
                <div class="model_footer">
                    <div class="model_btn">
                        <button class="btn btn-primary" v-on:click="enter">确定</button>
                        <button class="btn btn-warning" v-on:click="close_componet">取消</button>
                        <button class="btn btn-danger">暂停</button>
                    </div>
                </div>
            </div>
        </div>
    `,
    methods: {
        close_componet: function () {
            //调用父类的方法
            this.$parent.close_component();
        },
        enter: function () {
            //调用父类方法
            if (this.plan.taskFinishState == null || this.plan.taskFinishState == '1') {
                this.$parent.ReportEvent(1, "", 0, "", this.$parent.nowTime(0), "", "", "换线中工单", "结束换线");
            }
            else if (this.plan.taskFinishState == '2' || this.plan.taskFinishState == '5') {
                this.$parent.ReportEvent(2, "", 0, "", this.$parent.nowTime(0), "", "", "生产中工单", "开始报工");
            }
        }
    }
};
//开始报工模态组件
var startMode = {
    props: ['plan'],
    data: function () {
        return {
            start_time: "",//报工开始的时间
            end_time:"",//结束的时间
        }
    },
    template: `
    <div class="modelBox">
            <div class="model_content">
                <div class="model_title">
                    <span class="model_alert">开始报工</span>
                    <span class="model_work" v-text="'工单号码:'+plan.workID"></span>
                    <span class="model_icon fa fa-remove" v-on:click="close"></span>
                </div>
                <div class="model_body">
                    <div class="model_body_content">
                        <span class="body_icon fa fa-exclamation-triangle"></span>
                        <p class="model_body_alert_message"><span v-text="'工序名称:'+plan.pmOpName"></span> <span v-text="'计划数量:'+plan.plannedqty"></span>
                            <span v-text="'工单总数:'+plan.jobQty"></span>
                            <span v-text="'计划开始时间:'+plan.planStartTime"></span>
                            <span v-text="'计划结束时间:'+plan.planendtime"></span>
                            <span v-text="'属性:'+plan.itemAttr1"></span>
                        </p>
                    </div>
                    <div class="model_options">
                        <p>
                            <label for="startTime">开始生产时间:</label>
                            <input type="text" id="startTime" class="form-control" readonly v-bind:value="start_time">
                        </p>
                        <p>
                        <label for="">报工时间:</label>
                            <input type="datetime-local" id="endTime" v-bind:value="end_time" class="form-control">
                        </p>
                        <p>
                            <label for="">本班次完成数量:</label>
                        <input type="text" class="form-control" id="finishQty" v-bind:placeholder="plan.plannedqty">
                        </p>
                        <label for="">工单不良数量:</label>
                        <input type="text" class="form-control" id='failQty' v-bind:placeholder="0">
                    </div>
                </div>
                <div class="model_footer">
                    <div class="model_btn">
                        <button class="btn btn-primary" v-on:click="enter">确定</button>
                        <button class="btn btn-warning" v-on:click="close">取消</button>
                        <button class="btn btn-danger">暂停</button>
                    </div>
                </div>
            </div>
        </div>
    `,
    created: function () {
        this.work_start_time();
        this.end_time = this.get_end_time();
        console.log(this.end_time)
    },
    methods: {
        close: function () {
            this.$parent.close_component();
        },
        work_start_time: function () {
            var _this = this;
            $.get("/Phone/WorkOrderStartTime", { resname: this.plan["pmResName"], timeType: "E" }).done(function (time) {
                _this.start_time = time;
            });
        },
        get_end_time: function () {
            var time = new Date();
            var year = time.getFullYear();
            var month = time.getMonth() + 1 > 10 ? time.getMonth() + 1 : '0' + parseInt(time.getMonth() + 1);
            var day = time.getDate() > 10 ? time.getDate() : '0' + time.getDate();
            var hour = time.getHours() >= 10 ? time.getHours() : '0' + time.getHours();
            var min = time.getMinutes() >= 10 ? time.getMinutes() : '0' + time.getMinutes();
            var sec = time.getSeconds() >= 10 ? time.getSeconds() : '0' + time.getSeconds();
            return year + '-' + month + '-' + day + 'T' + hour + ':' + min + ":" + sec;
        },
        enter: function () {
            var BadQty = $("#failQty").val() || $("#failQty").attr('placeholder');
            var finishQty = $("#finishQty").val() || $("#finishQty").attr('placeholder');
            var endTime = $("#endTime").val();
            var startTime = $("#startTime").val();
            this.$parent.ReportEvent(3, finishQty, BadQty, '0', startTime, endTime, "", "", "生产中工单", "开始报工");
        }
    }
};

//提示框组件
var alertBox = {
    props: ['alertmessage'],
    data: function () {
        return {

        }
    },
    template: `
       <div class="modelBox" v-on:click="close">
               <div class="alertBox">
                    <div class="alertTitle">
                        <span>信息</span>
                        <span class="alertIcon fa fa-window-close" v-on:click="close"></span>
                    </div>
                    <div class="alertContent">
                        <span class="alertContentIcon fa fa-exclamation-circle"></span>
                        <span style="font-weight:bolder" v-text="alertmessage"></span>
                        <p>
                            <button v-on:click="close">确定</button>
                        </p>
                   </div>
               </div>
           </div>
    `,
    methods: {
        close: function () {
            this.$parent.close_component();
            this.$parent.alert_title = "";
        },
    }
};
var parseBox = {
    props: ['plan'],
    data: function () {
        return {
            ErrorAttr: [1],
            errorMessage:"",
        }
    },
    template: `
       <div class="modelBox">
        <div class="model_content">
            <div class="model_title">
                <span class="model_alert">生产暂停</span>
                 <span class="model_work" v-text="'工单号码:'+plan.workID"></span>
                 <span class="model_icon fa fa-remove" v-on:click="close"></span>
            </div>
            <div class="model_body">
                <div class="model_body_content bg-danger">
                    <span class="body_icon fa fa-exclamation-triangle"></span>
                    <p class="model_body_alert_message"><span v-text="'工序名称:'+plan.pmOpName+';'"></span><span v-text="'计划数量:'+plan.plannedqty"></span><span v-text="'工单总数:'+plan.jobQty"></span>
                            <span v-text="'计划开始时间:'+plan.planStartTime"></span>
                            <span v-text="'计划结束时间:'+plan.planendtime"></span>
                            <span v-text="'属性:'+plan.itemAttr1"></span>
                    </p>
                </div>
                <textarea style="margin-top:15px" name="" class="parseText" id="parseText" cols="30" rows="5" placeholder="暂停生产的原因:" v-model="errorMessage">
                </textarea>
                <span class="clearErr"><a href="#" v-on:click="clearErr">清除</a></span>
                <ul class="parse_option">
                    <li v-for="(item,index) in ErrorAttr">
                        <button v-text="item.eventType" v-on:click="enter_message($event)"></button>
                    </li>
                </ul>
            </div>
            <div class="model_footer">
                <div class="model_btn">
                    <button class="btn btn-primary" v-on:click="enter">确定</button>
                    <button class="btn btn-warning" v-on:click="close">取消</button>
                </div>
            </div>
        </div>
    </div>
`,
    created: function () {
        this.get_parse_message();
    },
    methods: {
        enter: function () {
            //确定按钮的点击事件
            this.$parent.ReportEvent(5, "", 0, "", "", "", "", this.errorMessage,"开始生产");
        },
        close: function () {
            this.$parent.close_component();
        },
        enter_message: function (e) {
            var text = $(e.currentTarget).text();
            if (this.errorMessage == "") {
                this.errorMessage = text;
            } else{
                if (this.errorMessage.indexOf(text)== -1) {
                    this.errorMessage += "," + text;
                }
            }
        },
        get_parse_message: function () {
            var _this = this;
            $.get('/Phone/GetParseMessage').done(function (parseData) {
                _this.ErrorAttr = parseData;
            })
        },
        clearErr: function () {
            this.errorMessage = "";
        }
    }
}
var exceptionHandling = {

}//异常处理，功能待开发