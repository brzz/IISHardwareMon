

//最小值
Array.prototype.min = function () {
    var min = this[0];
    var len = this.length;
    for (var i = 1; i < len; i++) {
        if (this[i] < min) {
            min = this[i];
        }
    }
    return min;
}
//最大值
Array.prototype.max = function () {
    var max = this[0];
    var len = this.length;
    for (var i = 1; i < len; i++) {
        if (this[i] > max) {
            max = this[i];
        }
    }
    return max;
}

//根据数量级确定舍入范围
function FixNumb(numb, fixed) {
    var n = parseFloat(numb);

    if (fixed == 0) return numb;
    if (fixed < 0) {
        //范围是小数部分
        fixed = -fixed;
        var f = parseFloat(parseInt(fixed));
        if (f < 1) f = 1;
        return Math.round(n / (f / 10)) * (f / 10);
    }
    if (fixed > 0) {
        //范围是整数范围
        var f = parseFloat(parseInt(fixed));
        if (f < 1) f = 1;
        return Math.round(n / (f * 10)) * (f * 10);
    }
}
//获取某两个值之外的整数值,scale为比列系数
function GetNearbyNumbMax(max, min, scale) {
    var tmp = (max - min) / (1 / scale);
    var tmpmax = max + tmp;

    // IE notsupport Math.log10
    //return FixNumb(tmpmax, Math.log10(tmp));
}

//获取某两个值之外的整数值,scale为比列系数
function GetNearbyNumbMin(max, min, scale) {
    var tmp = (max - min) / (1 / scale);
    var tmpmin = min - tmp;
    //if (min >= 0) return 0;

    // IE notsupport Math.log10
    //return FixNumb(tmpmin, Math.log10(tmp));
}