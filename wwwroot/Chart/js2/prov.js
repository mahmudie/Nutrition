function provincialchart(column,title,xTitle,yTitle,year) {
    $.getJSON('js2/NutProvinceData.txt', function (data) {


        //var prov = $.grep(data, function (element, index) {
        //    return element.PROV_32_ID === code && element.Year === year;
        //});
        //var mm = prov;

        ////var data2 = [];

        ////for (var i = 0; i < data.length ; i++) {
        ////    if (data[i].ProvID === 11 && data[i].Year === 1393) {
        ////        data2.push(data[i]);
        ////    }
        ////}

        Highcharts.each(data, function (p, i) {
            if (column === 1) {
                data[i].y = p.value;
            }
            else if (column === 2) {
                data[i].y = p.C623months;
            }
            else if (column === 3) {
                data[i].y = p.Ccured;
            }
            data[i].name = p.Pname;
        });
        new Highcharts.chart('container3', {
            chart: {
                type: 'column'
            },
            title: {
                text: title+' '+year
            },
            xAxis: {
                categories: true,
                crosshair: true,
                title: xTitle
            },
            yAxis: {
                title: {
                    text: yTitle
                }
            },
            legend: {
                layout: 'horizontal',
                align: 'center',
                horizontalAlign: 'middle'
            },

            plotOptions: {
                series: {
                    name: xTitle,
                    pointStart: 1
                }
            },

            series: [
                {
                    data: data
                }
            ]

        });
    });
}