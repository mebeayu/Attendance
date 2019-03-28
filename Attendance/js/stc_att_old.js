$(document).ready(function () {
    Token = localStorage.getItem("Token");
    uid = localStorage.getItem("uid");

    var today = new Date();
    var m = today.getMonth() + 1;
    $("#start_time").textbox('setValue', today.getFullYear() + "-" + (m < 10 ? ('0' + m) : m) + "-01");
    $('#end_time').textbox('setValue', myformatter(today));
});
function Stc_Att() {

    var formData = new FormData();
    formData.append("start_time", $("#start_time").val());
    formData.append("end_time", $("#end_time").val());
    if ($("#kq_file")[0].files.length > 0) {
        formData.append("kq_file", $("#kq_file")[0].files[0]);
    }
    else {
        alert("请选择上传文件");
        return;
    }
    $("#pro").html("<img src='/img/loading.gif' width=24 heigth=24>");
    $.ajax({
        method: "POST",
        url: "/API/APIAtt/StcAttOld",
        data: formData,
        dataType: "json",
        contentType: false, //传文件必须！
        processData: false, //传文件必须！
        success: function (data) {
            if (data.error_code === 0) {

                //console.log(list);
                $("#pro").html("");
                $('#dg').datagrid('loadData', data.data);
            }
            else {
                alert(data.error_code + data.message);
                $("#pro").html("");

            }
        },
        error: function () {
            alert("服务器错误");
            $("#pro").html("");
        }
    });

}
function formatNum(val, row) {
    if (val === 0) return "";
    else return val;
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
function getUrl(file) {
    //var tmpFile = file[0];
    //var reader = new FileReader();
    //reader.readAsDataURL(tmpFile);
    //reader.onload = function (e) {
    //    var url = e.target.result;
    //    console.log(url);
    //}

}