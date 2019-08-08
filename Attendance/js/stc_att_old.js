var dg;
$(document).ready(function () {
    Token = localStorage.getItem("Token");
    uid = localStorage.getItem("uid");
    //yearmonth("#attYearMonth");
    var today = new Date();
    var m = today.getMonth() + 1;
    $("#start_date").textbox('setValue', today.getFullYear() + "-" + (m < 10 ? ('0' + m) : m) + "-01");
    $('#end_date').textbox('setValue', myformatter(today));
    dg = $('#dg');
    dg.datagrid({
        onDblClickRow: function (index, row) {
            console.log(row);
            var start_date = $("#start_date").val();
            var end_date = $("#end_date").val();
            $('#dg_daily').datagrid('loadData', []);
            $('#dlg_show_person').dialog('open');

            $("#pro1").html("<img src='/img/loading.gif' width=24 heigth=24>");

            var request_data = {
                Token: Token, UID: row.UID, LOGINID: row.LOGINID, LASTNAME: row.LASTNAME,
                StartDate: start_date, EndDate: end_date, att_userid: row.att_userid
            };
            console.log(request_data);
            var url = "/API/APIAtt/GetPersonTripandLeaveDetail";
            $.ajax({
                type: 'POST',
                url: url,
                data: request_data,
                success: function (data) {
                    if (data.error_code === 0) {
                        console.log(data.data);
                        
                        console.log(data.data);
                        $('#dg_daily').datagrid('loadData', data.data.list_att);
                        $('#dg_leave').datagrid('loadData', data.data.list_leave);
                        $('#dg_trip').datagrid('loadData', data.data.list_trip);
                        $("#pro1").html("");
                    }
                    else {
                        alert(data.error_code + data.message);
                        $("#pro1").html("");

                    }
                },
                error: function () {
                    alert("服务器错误");
                    $("#pro1").html("");
                },
                dataType: "json"
            });
        }
    });
});
function Stc_Att() {

    //var formData = new FormData();
    //var month = $("#attYearMonth").val();
    //if (month === "") {
    //    alert("请选择月份");
    //    return;
    //}
    //arr = month.split("-");
    //var days = mGetDate(arr[0], arr[1]);
    //console.log(days);
    //formData.append("start_time", month+"-01");
    //formData.append("end_time", month + "-" + days);
    //if ($("#kq_file")[0].files.length > 0) {
    //    formData.append("kq_file", $("#kq_file")[0].files[0]);
    //}
    //else {
    //    alert("请选择上传文件");
    //    return;
    //}
    var start_date = $("#start_date").val();
    var end_date = $("#end_date").val();
    var request_data = { Token: Token, start_date: start_date, end_date: end_date };
    var url = "/API/APIAtt/StcAttReport";
    $("#pro").html("<img src='/img/loading.gif' width=24 heigth=24>");
    $.ajax({
        type: 'POST',
        url: url,
        data: request_data,
        success: function (data) {
            if (data.error_code === 0) {

                //console.log(data.data);
                $('#dg').datagrid('loadData', data.data);
                $("#pro").html("");
            }
            else {
                alert(data.error_code + data.message);
                $("#pro").html("");

            }
        },
        error: function () {
            alert("服务器错误");
            $("#pro").html("");
        },
        dataType: "json"
    });
    //$.ajax({
    //    method: "POST",
    //    url: "/API/APIAtt/StcAttReport",
    //    data: formData,
    //    dataType: "json",
    //    contentType: false, //传文件必须！
    //    processData: false, //传文件必须！
    //    success: function (data) {
    //        if (data.error_code === 0) {

    //            //console.log(list);
    //            $("#pro").html("");
    //            $('#dg').datagrid('loadData', data.data);
    //        }
    //        else {
    //            alert(data.error_code + data.message);
    //            $("#pro").html("");

    //        }
    //    },
    //    error: function () {
    //        alert("服务器错误");
    //        $("#pro").html("");
    //    }
    //});

}
function formatSumAtt(val, row) {
    if (val < row.WorkDay) {
        return "<font color=red>" + val + "</font>";
    }
    else if (val === row.WorkDay) {
        return "<font color=green>" + val + "</font>";
    }
    else {
        return val;
    }
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
function mGetDate(year, month) {
    var d = new Date(year, month, 0);
    return d.getDate();
}
function yearmonth(id) {
    $(id).datebox({
        onShowPanel: function () {//显示日趋选择对象后再触发弹出月份层的事件，初始化时没有生成月份层
            span.trigger('click'); //触发click事件弹出月份层
            if (!tds) setTimeout(function () {//延时触发获取月份对象，因为上面的事件触发和对象生成有时间间隔
                tds = p.find('div.calendar-menu-month-inner td');
                tds.click(function (e) {
                    e.stopPropagation(); //禁止冒泡执行easyui给月份绑定的事件
                    var year = /\d{4}/.exec(span.html())[0]//得到年份
                        , month = parseInt($(this).attr('abbr'), 10); //月份，这里不需要+1
                    $(id).datebox('hidePanel')//隐藏日期对象
                        .datebox('setValue', year + '-' + month); //设置日期的值
                });
            }, 0);
            yearIpt.unbind();//解绑年份输入框中任何事件
        },
        parser: function (s) {
            if (!s) return new Date();
            var arr = s.split('-');
            return new Date(parseInt(arr[0], 10), parseInt(arr[1], 10) - 1, 1);
        },
        formatter: function (d) {
            var month = "";
            if ((d.getMonth() + 1) < 10) month = "0" + (d.getMonth() + 1)
            else month = d.getMonth() + 1;
            return d.getFullYear() + '-' + month;/*getMonth返回的是0开始的，忘记了。。已修正*/
        }
    });
    var p = $(id).datebox('panel'), //日期选择对象
        tds = false, //日期选择对象中月份
        yearIpt = p.find('input.calendar-menu-year'),//年份输入框
        span = p.find('span.calendar-text'); //显示月份层的触发控件
    console.log(yearIpt)
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