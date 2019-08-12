$(document).ready(function () {
    Token = localStorage.getItem("Token");
    if (!Token || typeof (Token) === "undefined") {

        var oa_login_id = getQueryString("oa_login_id");
        if (oa_login_id == null) {
            alert("参数错误，请使用OA账户密码登录");
            window.location.href = "/Att/Index";
            return;
        }
        var request_data = { oa_login_id: oa_login_id };
        var url = "/API/APIAtt/TakePointToOA";
        $("#info").val("正在认证....");
        $.ajax({
            type: 'POST',
            url: url,
            data: request_data,
            success: function (data) {
                if (data.error_code === 0) {
                    Token = data.data.Token;
                    //console.log(Token);
                    localStorage.setItem("Token", Token);
                    localStorage.setItem("uid", data.data.oa_login_id);
                    localStorage.setItem("user_info", JSON.stringify(data.data));
                    window.location.href = "/Att/Index";
                }
                else {
                    alert("参数错误，请使用OA账户密码登录");
                    localStorage.removeItem("Token");
                    localStorage.removeItem("uid");
                    localStorage.removeItem("user_info");
                    window.location.href = "/Att/Index";

                }
            },
            error: function () {
                alert("服务器错误");
               
            },
            dataType: "json"
        });

    }
    else {
        window.location.href = "/Att/Index";
    }
});

//获取url中的参数
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i"); // 匹配目标参数
    var result = window.location.search.substr(1).match(reg); // 对querystring匹配目标参数
    if (result !== null) {
        return decodeURIComponent(result[2]);
    } else {
        return null;
    }
}