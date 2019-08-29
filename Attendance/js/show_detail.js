﻿$(document).ready(function () {
    Token = localStorage.getItem("Token");
    uid = localStorage.getItem("uid");
    var today = new Date();
    var m = today.getMonth() + 1;
    var m1 = today.getFullYear() + "-" + (m < 10 ? ('0' + m) : m);
    console.log(m1);
    $("#attYearMonth").val(m1);
    GetPersonAtt();
    //yearmonth("#attYearMonth");
});
function GetPersonAtt() {
    var month = $("#attYearMonth").val();
    if (month === "") {
        alert("请选择月份");
        return;
    }
    $("#pro").html("<img src='/img/loading.gif' width=24 heigth=24>");
    var is_show_all = $("#is_show_all").is(':checked');
    var request_data = { Token: Token, LOGINID: uid, Month: month, is_show_all: is_show_all };
    $("#btn_see").linkbutton("disable");
    console.log(request_data);
    var url = "/API/APIAtt/GetPersonAtt";
    $.ajax({
        type: 'POST',
        url: url,
        data: request_data,
        success: function (data) {
            if (data.error_code === 0) {

                //$('#dg').datagrid('loadData', list);
                $('#dg1').datagrid('loadData', data.data);
                $("#pro").html("");
            }
            else {
                alert(data.error_code + data.message);
                $("#pro").html("");

            }
            $("#btn_see").linkbutton("enable")
        },
        error: function () {
            alert("服务器错误");
            $("#pro").html("");
            $("#btn_see").linkbutton("enable")
        },
        dataType: "json"
    });
}
function formatTagGongchu(val, row, rowIndex) {
    //console.log(rowIndex);
    if (row.is_holiday === false && val === null && (row.first_str===""||row.last_str==="")) {
        return "<a title='发起公出申请审批' onclick=pushOS('" + row.date_day + "','" + rowIndex+"')>" + "<img src='/Img/application_form_add.png'/>" + "</a>";
    }
    else {
        return val;
    }
}
var SelrowIndex = -1;
function pushOS(date, rowIndex) {
    //console.log(rowIndex);

    
    SelrowIndex = rowIndex;
    $("#date_time_gc").val(date);
    $('#dlg_form').dialog('open');
}
function submit_oa() {

    var person_type = $("#person_type").val();
    var date_time_gc = $("#date_time_gc").val();
    var range = $("#range").val();
    var reason = $("#reason").val();
    var addrss = $("#addrss").val();
    if (person_type==="-") {
        alert("请选择人员类型");
        return;
    }

    if (range === "-") {
        alert("请选择公出的时间范围");
        return;
    }
    if (reason === "" || date_time_gc === "" || addrss === "") {
        alert("请正确填写原因、日期、地点信息");
        return;
    }
    var request_data = { Token: Token, person_type: person_type, date_time_gc: date_time_gc, range: range, reason: reason, addrss: addrss };
    //console.log(request_data);
    $("#pro1").html("<img src='/img/loading.gif' width=24 heigth=24>");
    $("#btn_push").linkbutton("disable");
    //console.log(request_data);
    var url = "/API/APIAtt/PushGongchuForm";
    $.ajax({
        type: 'POST',
        url: url,
        data: request_data,
        success: function (data) {
            if (data.error_code === 0) {

                var rows = $('#dg1').datagrid("getRows");
                rows[SelrowIndex]["range"] = "[审批中]";
                rows[SelrowIndex]["memo"] = reason;
                $('#dg1').datagrid('refreshRow', SelrowIndex);
                $("#pro1").html("");
                alert("已提交到OA");
                $('#dlg_form').dialog('close');
            }
            else {
                alert(data.error_code + data.message);
                $("#pro1").html("");

            }
            $("#btn_push").linkbutton("enable")
        },
        error: function () {
            alert("服务器错误");
            $("#pro1").html("");
            $("#btn_push").linkbutton("enable")
        },
        dataType: "json"
    });
}
function formatTagLate(val, row) {
    //console.log(row);
    if (row.late_tag === true) return  "<span style='background: orangered'>" + val+"</span>";
    else return val;
}
function formatTagEarly(val, row) {
    if (row.early_tag === true) return  "<span style='background: orangered'>" + val+"</span>";
    else return val;
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