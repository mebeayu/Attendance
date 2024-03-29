﻿var dg;
$(document).ready(function () {
    Token = localStorage.getItem("Token");
    uid = localStorage.getItem("uid");

    var today = new Date();
    var m = today.getMonth() + 1;
    $("#start_time").textbox('setValue', today.getFullYear() + "-" + (m < 10 ? ('0' + m) : m) + "-01");
    $('#end_time').textbox('setValue', myformatter(today));
    dg = $('#dg');
    dg.datagrid({
        onDblClickRow: function (index, row) {
            console.log(row);
            var s = $("#start_time").val();
            var e = $("#end_time").val();
            $('#dlg_show_person').dialog('open');
            $('#dg_leave').datagrid('loadData', []);
            $('#dg_trip').datagrid('loadData', []);
            $("#pro1").html("<img src='/img/loading.gif' width=24 heigth=24>");
            
            var request_data = { Token: Token, UID: row.UID, LOGINID:uid,LASTNAME: row.LASTNAME, StartDate: s, EndDate: e };
            var url = "/API/APIAtt/GetPersonTripandLeaveDetail";
            $.ajax({
                type: 'POST',
                url: url,
                data: request_data,
                success: function (data) {
                    if (data.error_code === 0) {
                        console.log(data.data);
                       

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
    LoadDays();
}

function LoadDays() {
    var s = $("#start_time").val();
    var e = $("#end_time").val();
    
    var list = [];
    $("#pro").html("<img src='/img/loading.gif' width=24 heigth=24>");
    
    $('#dg').datagrid('loadData', list);
    
    var request_data = { Token: Token, StartDate: s, EndDate: e };
    console.log(request_data);
    var url = "/API/APIAtt/QueryAttList";
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
function NameFormatter(value, row, index) {
    if (value > 0) {
        return "<a target='_blank' href='/Contract/SupplierEvaluateList?supplier_code=" + row.supplier_code + "'>" + value + "</a>";
    }
    else {
        return value;
    }

}
