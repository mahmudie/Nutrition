function barline(code,year) {
    $.getJSON('js2/NutMapMonthYear.txt', function (data) {




        var prov = $.grep(data, function (element, index) {
            return element.ProvID === code && element.Year === year;
        });
        var mm = prov;

        //var data2 = [];

        //for (var i = 0; i < data.length ; i++) {
        //    if (data[i].ProvID === 11 && data[i].Year === 1393) {
        //        data2.push(data[i]);
        //    }
        //}

        Highcharts.each(mm, function (p, i) {
            mm[i].y = p.value;
            mm[i].name = p.Month;
        });
        new Highcharts.chart('container2', {
            chart: {
                type: 'column'
            },
            title: {
                text: 'My sample chart'
            },
            xAxis: {
                categories: true,
                crosshair: true,
                title:'Months'
            },
            yAxis: {
                title: {
                    text: 'admissions'
                }
            },
            legend: {
                layout: 'horizontal',
                align: 'center',
                horizontalAlign: 'middle'
            },

            plotOptions: {
                series: {
                    name:'admissions',
                    pointStart: 1
                }
            },

            series: [
                {
                    data: mm
                }
            ]

        });
    });
}