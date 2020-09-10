function  ChangeLoginTitle() {

    if (document.getElementById("usercheck").checked == true) {
        loginTitle.innerText = "管理员登陆";
    }
    else {
        loginTitle.innerText = "用户登陆";
        if (document.cookie.MD5 != null) {
            document.cookie.MD5 = '';
        }
    }
    if (document.cookie !="") {
        deleteCookie();
    }
}


function deleteCookie() {
    var cookies = document.cookie.split(";");
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
    $("#BtnLogout").css({ display: 'none' });
    $("#BtnCancel").css({ display: 'none' });
    $("#BtnLogin").css({ display: "block" });
    $("#LoginMsg").text("");
    $("#userName").add("#userPass").val(""); 
}

function CheckInput() {

    var par = parseFloat($("#userName").val()).toString();
    if ($("#userName").val() == "") {
        $("#LoginMsg").text("用户名不允许为空");
    }
    else if (par != $("#userName").val()) {
        $("#LoginMsg").text("请输入员工的数字ID");
    }
    else {
        if ($("#userPass").val() == "") {
            $("#LoginMsg").text("请输入密码");
        }
        else {
            Login();
        }
    }
}

//function checkestate(checkid) {
//    if (document.getElementById(checkid).checked) {
//        return 1;
//    }
//    else {
//        return 0;
//    }
//}

function Login() {
    $.post("/Index/Login", { empID: $("#userName").val(), pwd: $("#userPass").val() }).done(function (response) {
        console.log(response)
        if (response["loginState"] == "2") {
            $("#LoginMsg").text(response["message"]);
                $("#BtnLogin").css({ display: "none" });
                $("#BtnLogout").css({ display: "block" });
                $("#BtnCancel").css({ display: "block" });
            }
        else if (response["loginState"] == "0") {
            $("#LoginMsg").text(response[1]);
            }
        else if (response["loginState"] == "1") {
               //window.location.href = '/Selector/Index';
            }
    })
}


function Logout() {
    $.post("/Index/Logout", { empID: $("#userName").val(), userPass: $("#userPass").val() }).done(function (response) {
        console.log(response)
        if (response["loginState"] == "1") {
            //document.cookie = "EmpID=" + response.empID;  
            //document.cookie = "UserGuid=" + response.userGuid;
            window.location.href = 'Selector';
        }
        else {
            $("#LoginMsg").text(response["message"]);
        }
    })
}


