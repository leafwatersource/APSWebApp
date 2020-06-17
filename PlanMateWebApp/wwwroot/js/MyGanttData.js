(function ($) {
    var GanteJson = { };
    var GanteItem;
    var jobdata = [];
    var temp = 0;
    $.post("/Datacenter/GetGantt").done(function (GantaData) {
        GanteItem = GantaData;
        setGante();
    });
    function setGante() {
        GanteItem.forEach(function (ele, index) {
            var jobjson = {};
            var activitiesJson = {};
            if (jobdata.length == 0) {
                jobjson["description"] = ele["realOpName"];
                jobjson["activities"] = [];
                activitiesJson["code"] = "OK";
                activitiesJson["begin"] = createDate(ele["plannedStart"])
                activitiesJson["end"] = createDate(ele["plannedFinish"])
                activitiesJson["description"] = ele["productId"]
                jobjson["activities"].push(activitiesJson)
                jobdata.push(jobjson)
                jobjson = {};
                activitiesJson = {};
            } else {
                for (var i = 0; i < jobdata.length; i++) {
                    if (jobdata[i]["description"] == ele["realOpName"]) {
                        activitiesJson["code"] = "OK";
                        activitiesJson["begin"] = createDate(ele["plannedStart"])
                        activitiesJson["end"] = createDate(ele["plannedFinish"])
                        activitiesJson["description"] = ele["productId"]
                        jobdata[i]["activities"].push(activitiesJson)
                        activitiesJson = {};
                        return;
                    }
                }
                jobjson["description"] = ele["realOpName"];
                jobjson["activities"] = [];
                activitiesJson["code"] = "OK";
                activitiesJson["description"] = ele["ProductID"];
                activitiesJson["begin"] = createDate(ele["plannedStart"])
                activitiesJson["end"] = createDate(ele["plannedFinish"])
                activitiesJson["description"] = ele["productId"]
                jobjson["activities"].push(activitiesJson)
                jobdata.push(jobjson)
                jobjson = {};
                activitiesJson = {};
            }
        });
        initGanta();
    }
    function createDate(time,isTime,daysToSum) {
        var split = time.split(':');
        var ret = new Date();
        if (isTime) {
            split = time.split(':');
            ret.setHours(split[0]);
            ret.setMinutes(split[1]);
        } else {
            split = time.split(" ")[0].split("/");
            var t = time.split(" ")[1].split(':');
            ret.setFullYear(split[0]);
            ret.setMonth(split[1]);
            ret.setDate(split[2]);
            //ret.setHours(t[0]);
            //ret.setMinutes(t[1])
        }
        //console.log(ret)
        if (daysToSum) ret.setDate(ret.getDate() + daysToSum);
        return ret;
    }
    function initGanta() {
        //var nights = [{
        //    begin: createDate('19:00',true),
        //    end: createDate('07:00',true),
        //    color: "#5cea67"
        //}, {
        //        begin: createDate('07:00', true),
        //        end: createDate('19:00', true),
        //        color: "#5cea67"
        //    }
        //];
        
        var joboptions = {
            data: jobdata,
            generalMarkers: [],
            //generalHighlights: nights,
            style: {
                activityStyle: {
                    'OK': {
                        color: "green"
                    },
                    'WARNING': {
                        color: "yellow"
                    },
                    'ERROR': {
                        color: "red"
                    },
                },
                showDateOnHeader: true,
                showTimeOnHeader: true,
                hourWidth: 50,
                formatDate: function (date) {
                    return ('0' + (date.getMonth() + 1)).substr(-2)+("月");
                },
                dateHeaderFormat: function (date) {
                    return  ('0' + (date.getDate())).substr(-2);
                },
                descriptionContainerWidth: '150px'
            }
        };
        $(document).ready(function () {
            console.log(jobdata)
            // var $timeline = $('#timeline').stackedGantt(timeoptions);
            var jobschedules = $('#jobschedules').stackedGantt(joboptions);
        });
    }
})(jQuery)