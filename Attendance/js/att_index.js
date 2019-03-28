$(document).ready(function () {
    Token = localStorage.getItem("Token");
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