﻿$(document).ready(function () {
    Token = localStorage.getItem("Token");
    uid = localStorage.getItem("uid");
    var today = new Date();
    var m = today.getMonth() + 1;
    var m1 = today.getFullYear() + "-" + (m < 10 ? ('0' + m) : m);
    console.log(m1);
    $("#attYearMonth").val(m1);
    $('#dg1').datagrid({
        onLoadSuccess: function (data) {
            console.log("onLoadSuccess");
            var merges_a = [{
                index: 0,//合并第5行的列,从0开始，所以index=4是第五行。
                colspan: 3//合并列的数量
            }];
            for (var i = 0; i < merges_a.length; i++)
                $('#waterLevel').datagrid('mergeCells', {
                    index: merges_a[i].index,
                    field: 'no',//合并后单元格对应的属性值
                    colspan: merges_a[i].colspan
                });
        }

    });
    //yearmonth("#attYearMonth");
});
function GetAttDetail() {
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
    var url = "/API/APIAtt/GetAllAttDetailHtml";
    $.ajax({
        type: 'POST',
        url: url,
        data: request_data,
        success: function (data) {
            if (data.error_code === 0) {
                console.log(data.data);
                $("#data").html(data.data)
                //console.log(data.data);
                //$('#dg').datagrid('loadData', list);
                //$('#dg1').datagrid('loadData', data.data);
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
}