let select = new Vue({
    el: "#section1",
    data: {},
    created: function () {
        axios.get('/Registered/All_User', { params: { 'page': 1 } }).then(function (user_table) {
            console.log(user_table);
        })
    },
})