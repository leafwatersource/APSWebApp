(function ($) {
    $.get("/DataCenter/DTworkOrder").done(function (response) {
        response = JSON.parse(response);
        response.forEach(function (ele) {
            ele["操作"] = "<a href='#' class='editor' onclick = '$.Edit(this)'>编辑</a>";
            ele["刪除"] = "<a href='#' class='editor' onclick = '$.Delete(this)' style='color:red'>刪除</a>";
        });
        $('#reportTable').Totable(response, true);//初始化表格的样式
        //$('.editor').on('click', function () {

        //})
        //$('.editor').each(function (index,ele) {
        //    console.log(index)
        //    $(ele).on("click", function () {
        //添加一个列,可以操作前面几个列
        //        $.Edit(this);
        //    });
        //})
    })
})(jQuery)