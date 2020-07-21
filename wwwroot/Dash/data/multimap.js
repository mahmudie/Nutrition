$.getJSON('/Dash/data/provincial_all_years.txt', function (data) {
    var data1 = [],
        data2 = [],
        data3 = [];

    Highcharts.each(data, function(p) {
        data1.push({
            Pname: p.Pname,
            PROV_32_ID: p.PROV_32_ID,
            z: p.Children6m
        });
        data2.push({
            Pname: p.Pname,
            PROV_32_ID: p.PROV_32_ID,
            z: p.Children23m
        });
        data3.push({
            Pname: p.Pname,
            PROV_32_ID: p.PROV_32_ID,
            z: p.Children59m
        });
    });

    $('#multimap').highcharts('Map', {
        title: {
            text: '<h2>multiple shape map</h2>'
        },

        mapNavigation: {
            enabled: true,
            buttonOptions: {
                verticalAlign: 'bottom'
            }
        },
        legend: {
            enabled: true,
            layout: 'horizontal',
            borderWidth: 0,
            //backgroundColor: 'rgba(255,255,255,0.85)',
            floating: true,
            verticalAlign: 'bottom',
            y: 25,
            symbolWidth: 0,
            symbolHeight: 0,
            symbolPadding: 20
        },
        colorAxis: {
            min: 1,
            type: 'logarithmic',
            minColor: '#E0E0E0',
            maxColor: '#000022',
            stops: [
                [0, '#EFEFFF'],
                [0.67, '#4444FF'],
                [1, '#caccce']
            ]
        },
        credits: {
            enabled: false
        },
        series: [
        {
            name: 'Children',
            mapData: provinces,
            dataLabels: {
                enabled: true
            },
            color: '#000022',
            enableMouseTracking: false,
            nullColor: 'rgb(204, 204, 179,0.4)',
        },
        {
            animation: {
                duration: 4000
            },
            type: 'mapbubble',
            data: data1,
            mapData: provinces,
            joinBy: ['PROV_32_ID', 'PROV_32_ID'],
            name: 'children under 6 months',
            minSize: 4,
            maxSize: '20%',
            color: 'rgba(255, 77, 210,0.5)',
            states: {
                hover: {
                    color: '#a4edba'
                }
            },
            //dataLabels: {
            //    enabled: true,
            //    style: {
            //        fontSize: '0.8em'
            //    },
            //    format: '{point.Pname}'
            //},

            //name: 'Vaccinated',
            tooltip: {
                pointFormat: '{point.Children6m}'
            },
            marker: {
                fillOpacity: 0.3
            }

        },
        {
            mapData: provinces,
            type: 'mapbubble',
            joinBy: ['PROV_32_ID', 'PROV_32_ID'],
            data: data2,
            minSize: 4,
            maxSize: '20%',
            name: 'Children 6-23',
            tooltip: {
                pointFormat: '{point.Pname}: {point.z}'
            },
            marker: {
                fillOpacity: 0.3
            }
        },
        {
            mapData: provinces,
            type: 'mapbubble',
            joinBy: ['PROV_32_ID', 'PROV_32_ID'],
            data: data3,
            name: "children 24-59",
            minSize: 4,
            maxSize: '20%',
            color: 'rgba(255, 51, 0,0.3)',
            tooltip: {
                pointFormat: '{point.Pname}: {point.z}'
            },
            marker: {
                fillOpacity: 0.3
            }
        }
        ]
    });
})