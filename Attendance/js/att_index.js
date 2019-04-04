var user_inf = null;
var Token = null;
$(document).ready(function () {
    Token = localStorage.getItem("Token");
    if (!Token || typeof (Token) === "undefined") {
        $("#uid").val("");
        $("#psw").val("");
        $('#dlg_login').dialog('open');
        return;
    }
    
    user_inf = JSON.parse(localStorage.getItem("user_info"));
    
    if (user_inf.type === "0") {
        //$("#memu1").hide();
    }
    uid = localStorage.getItem("uid");

    var today = new Date();
    $("#start_time").textbox('setValue', today.getFullYear() + "-01-01");
    $('#end_time').textbox('setValue', myformatter(today));
});
function stc_leave() {
    $("#iframe_maincontent").attr("src", "/Att/StcOALeave");
}
function stc_cc() {
    $("#iframe_maincontent").attr("src", "/Att/StcOATrip");
}
function stc_leave_cc() {
    $("#iframe_maincontent").attr("src", "/Att/StcAttr");
}
function stc_att_old() {
    $("#iframe_maincontent").attr("src", "/Att/StcAttrOld");
}
function Login() {
    var uid = $("#uid").val();
    var psw = $("#psw").val();
    $("#pro1").html("<img src='/img/loading.gif' width=24 heigth=24>");
    var login_data = { uid: uid, psw: psw };
    //console.log(login_data);
    var url = "/API/APIAtt/Login";
    $.ajax({
        type: 'POST',
        url: url,
        data: login_data,
        success: function (data) {

            if (data.error_code === 0) {
                //console.log(data.data);
                //return;
                Token = data.data.Token;
                //console.log(Token);
                localStorage.setItem("Token", Token);
                localStorage.setItem("uid", uid);
                localStorage.setItem("user_info", JSON.stringify(data.data));

                window.location.href = "/Att/Index";
                $("#pro1").html("");
            }
            else {
                $("#pro1").html("");
                alert(data.message);
            }
        },
        error: function () { },
        dataType: "json"
    });
}
function Logout() {
    localStorage.removeItem("Token");
    localStorage.removeItem("uid");
    localStorage.removeItem("user_info");
    window.location.replace("/Att/Index");
}
function myformatter(date) {
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    return y + '-' + (m < 10 ? ('0' + m) : m) + '-' + (d < 10 ? ('0' + d) : d);
}
function myparser(s) {
    if (!s) return new Date();
    var ss = (s.split('-'));
    var y = parseInt(ss[0], 10);
    var m = parseInt(ss[1], 10);
    var d = parseInt(ss[2], 10);
    if (!isNaN(y) && !isNaN(m) && !isNaN(d)) {
        return new Date(y, m - 1, d);
    } else {
        return new Date();
    }
}