var vm = new Vue({
    el: "#section2",
    data: {
        username: "",
        password: "",
        enter_password: "",
        name: "",
        email: "",
        phone: "",
        CFM: null,
        ADMIN: null,
        BOARD:null,
        REP: null,
        VIEW: null,
        error_message: "",
        register_data: {}
    },
    created: function () {
    },
    methods: {
        enter_Registered: function () {
            console.log(this.CFM)
            var verify = this.verify_message()
            if(verify) {
                //注册的接口
                this.register_data['username'] = this.username;
                this.register_data['password'] = this.password;
                this.register_data['ADMIN'] = this.ADMIN;
                this.register_data['BOARD'] = this.BOARD;
                this.register_data['REP'] = this.REP;
                this.register_data['VIEW'] = this.VIEW;
                this.register_data['CFM'] = this.CFM;
                this.register_data['phone'] = this.phone;
                this.register_data['email'] = this.email;
                this.register_data['name'] = this.name;
                let _this = this;
                axios.get('/Registered/EnterRegistered', {
                    params: {
                        data: JSON.stringify(_this.register_data)
                    }
                }).then(function (register_state) {
                    _this.error_message = register_state['data'];

                });
            }
        },
    verify_message: function () {
            if (this.username === "") {
                this.error_message = "用户名不能为空";
                return false;
            }
            if (this.username.length < 6) {
                console.log(6);
                this.error_message = "用户名长度应大于6小于11,当前的长度" + this.username.length;
                return false;
            } else if (this.username.length > 11) {
                this.error_message = "用户名长度应大于6小于11,当前的长度" + this.username.length;
                return false;
            }
            if (this.password === '') {
                this.error_message = "密码不能为空,请输入密码";
                return false;
            }
            if (this.password.length < 6) {
                this.error_message = "密码长度不能少于6位";
                return false;
            }
            if (this.enter_password === "") {
                this.error_message = "请输入确认密码";
                return false;
            }
            if (this.password !== this.enter_password) {
                this.error_message = "两次输入的密码不一致,请重新输入";
                return false;
            }
            if (this.name == "") {
                this.error_message = "请输入用户备注信息";
                return false;
            }
            if (!(/^1(3|4|5|6|7|8|9)\d{9}$/.test(this.phone))) {
                this.error_message = '请输入有效的手机号码';
                return false;
            }
            if (true) {
                //验证邮箱
                var myreg = /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/;
                if (!myreg.test(this.email)) {
                    this.error_message = '请输入有效的E_mail！';
                    return false;
                }
            }
            if (this.ADMIN == "" && this.BOARD == "" && this.REP == "" && this.VIEW == "" && this.CFM == "") {
                this.error_message = "请选择用户的身份";
                return false;
            }
            return true;
        },
        reset: function () {
            this.username = "";
            this.password = "";
            this.enter_password = "";
            this.ADMIN = "";
            this.VIEW = "";
            this.BOARD = "";
            this.REP = "";
            this.CFM = "";
            this.email = "";
            this.phone = "";
            this.name = "";
        }
    }
});