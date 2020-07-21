 function send(defyear) {
     var e = document.getElementById("selyear");
     var year = Number(e.options[e.selectedIndex].value);
     year = defyear ? defyear : year;
     document.getElementById('dashboard').style.display = 'none';
     $("#sendbtn").text('Loading..');
     $("#othertable tbody").empty();
     $("#datatable tbody").empty();

     document.getElementById("sendbtn").disabled = true;

     $.getJSON('/Dashboardm/dashStat/' + year, function(submit) {
         var provinces = $.number(submit.subProvinces);
         var districts = $.number(submit.subDistricts);
         var facilities = $.number(submit.subFacilities);
         var orgs = $.number(submit.subOrgs);
         $('#sprovinces').text(provinces)
         $('#sdistricts').text(districts)
         $('#sfacilities').text(facilities)
         $('#sorgs').text(orgs)
     });

     var mydata;
     var chart1;
     var chart2;
     var chart3;

     $.getJSON("/Dashboardm/provall/" + year, function(data) {
         mydata = data;
         $.each(data, function(i, item) {
             $('<tr>').html(
                 "<td class='left'><b>" + data[i].pname + "</b></td><td>" + data[i].value + "</td><td>" + data[i].cured + '(<b>' + Math.round(data[i].curedP, 0) + '</b>)' +
                 "</td><td>" + data[i].deaths + '(<b>' + Math.round(data[i].deathP, 0) + '</b>)' + "</td><td>" + data[i].defaulter + '(<b>' + Math.round(data[i].defaultP, 0) + '</b>)' +
                 "</td><td>" + data[i].nonCured + '(<b>' + Math.round(data[i].nonCuredP, 2) + '</b>)' +
                 "</td><td>" + data[i].male + '(<b>' + Math.round(data[i].maleP, 0) + '</b>)' + "</td><td>" + data[i].female + '(<b>' + Math.round(data[i].femalep, 0) + '</b>)' + "</td>").appendTo('#datatable tbody');
         });
         $.each(data, function(i, item) {
             $('<tr>').html(
                 "<td class='left'><b>" + data[i].pname + "</b></td><td>" + data[i].odema + "</td><td>" + data[i].zscore23 + "</td><td>" + data[i].muaC115 +
                 "</td><td>" + data[i].muaC12 + "</td><td>" + data[i].muaC23 +
                 "</td><td>" + data[i].referIn + "</td><td>" + data[i].referOuts + "</td>").appendTo('#tbody-sparkline');
         });
         var admits = 0;
         var childrenuder6 = 0;
         var children623 = 0;
         var children2459 = 0;
         var discharge = 0;
         var cured = 0;
         var deaths = 0;
         var defaults = 0;
         var male = 0;
         var female = 0;
         var noncured = 0;

         for (i = 0; i < mydata.length; i++) {
             admits += mydata[i].value;
             childrenuder6 += mydata[i].children6m;
             children623 += mydata[i].children23m;
             children2459 += mydata[i].children59m;
             discharge += mydata[i].discharge;
             cured += mydata[i].cured;
             deaths += mydata[i].deaths;
             defaults += mydata[i].defaulter;
             male += mydata[i].male;
             female += mydata[i].female;
             noncured += mydata[i].nonCured;
         }
         //get controlID
         $('#schildunder6').text($.number(childrenuder6));
         $('#sadmits').text($.number(admits));
         $('#schild623').text($.number(children623));
         $('#schild2459').text($.number(children2459));
         $('#sdischarge').text($.number(discharge));
         $('#scured').text($.number(cured));
         $('#sdeath').text($.number(deaths));
         $('#sdefults').text($.number(defaults));
         $('#sboys').text($.number(male));
         $('#sgirls').text($.number(female));
         $('#snoncured').text($.number(noncured));

         var perc6 = ((childrenuder6 / admits) * 100).toFixed(1);
         var perc23 = ((children623 / admits) * 100).toFixed(1);
         var perc59 = ((children2459 / admits) * 100).toFixed(1);

         var percCured = ((cured / discharge) * 100).toFixed(1);
         var percDeath = ((deaths / discharge) * 100).toFixed(1);
         var percDefault = ((defaults / discharge) * 100).toFixed(1);
         var percnonCured= ((noncured/discharge)*100).toFixed(1);

         var percMale = ((male / admits) * 100).toFixed(1);
         var percFemale = ((female / admits) * 100).toFixed(1);

         $('#sm23').text(" (" + perc23 + "%)");
         $('#sm6').text(" (" + perc6 + "%)");
         $('#sm59').text(" (" + perc59 + "%)");

         $('#scur').text(" (" + percCured + "%)");
         $('#sdead').text(" (" + percDeath + "%)");
         $('#sdef').text(" (" + percDefault + "%)");

         $('#sbo').text(" (" + percMale + "%)");
         $('#sgir').text(" (" + percFemale + "%)");
         $('#snon').text(" (" +percnonCured + "%)");
         //$("#padmits").width(pcu6 + '%');

         var data1 = [],
             data2 = [],
             data3 = [];

         Highcharts.each(mydata, function(p) {
             data1.push({
                 Pname: p.pname,
                 PROV_32_ID: parseInt(p.proV_32_ID),
                 z: p.children6m
             });
             data2.push({
                 Pname: p.pname,
                 PROV_32_ID: parseInt(p.proV_32_ID),
                 z: p.children23m
             });
             data3.push({
                 Pname: p.pname,
                 PROV_32_ID: parseInt(p.proV_32_ID),
                 z: p.children59m
             });
         });

         $('#multimap').highcharts('Map', {
             title: {
                 text: '<h2>New children breakdown by age category</h2>'
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
                 //backgroundColor: 'rgba(100,200,255,0.85)',
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
             series: [{
                     name: 'Children',
                     mapData: provinces,
                     dataLabels: {
                         enabled: true
                     },
                     animation: {
                         duration: 5000
                     },
                     nullColor: '#F5F5F5',
                     enableMouseTracking: true,
                 },
                 {
                     animation: {
                         duration: 6000
                     },
                     type: 'mapbubble',
                     data: data1,
                     mapData: provinces,
                     joinBy: ['PROV_32_ID', 'PROV_32_ID'],
                     name: 'Under 6 children',
                     minSize: 4,
                     maxSize: '20%',
                     color: 'rgba(255, 77, 210,0.5)',
                     states: {
                         hover: {
                             color: '#a4edba'
                         }
                     },
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
                    animation: {
                         duration: 8000
                     },
                     name: '6-23 children',
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
                     name: "24-59 children",
                     minSize: 4,
                     maxSize: '20%',
                    animation: {
                         duration: 15000
                     },
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

         var tCategories = [];
         for (i = 0; i < mydata.length; i++) {
             tCategories.push(mydata[i].pname);
         }

         //====================== CHILDREN ADMISSIONS BY PROVINCE ======================================================
         var options = {
             chart: {
                 renderTo: 'childrenchart',
                 type: 'bar'
                     //marginRight: 130,
                     //marginBottom: 125
             },
             plotOptions: {
                 series: {
                     stacking: 'normal',
                     animation: {
                         duration: 6000
                     }
                 }
             },
             title: {
                 text: 'Children admitted by province',
                 x: -10 //center
             },
             subtitle: {
                 text: '',
                 x: -10
             },
                credits: {
                    enabled: false
                },                
             xAxis: {
                 categories: [],
                 labels: {
                     useHTML: true
                 }
             },
             yAxis: {
                 title: {
                     text: ''
                 },
                 plotLines: [{
                     value: 0,
                     width: 1,
                     color: '#808080'
                 }]
             },
             tooltip: {
                 formatter: function() {
                     return '<b>' + this.series.name + '</b><br/>' +
                         this.x + ': ' + this.y;
                 }
             },
             legend: {
                 reversed: true
             },
             series: [],

         };

         var childunder6 = {

                 name: 'Under 6 children',
                 data: []
             },
             child623 = {
                 name: '6-23 children',
                 data: []
             },
             child2359 = {
                 name: '23-59 children',
                 data: []
             };
         //options.xAxis.categories = [];

         options.xAxis.categories = tCategories;
         Highcharts.each(mydata, function(p) {
             options.xAxis.categories.push(p.Province);
             childunder6.data.push(p.children6m);
             child623.data.push(p.children23m);
             child2359.data.push(p.children59m);
         })

         options.series[0] = childunder6;
         options.series[1] = child623;
         options.series[2] = child2359;

         chart3 = new Highcharts.Chart(options);

         inicator_chart();
         male_female();
         //buildHtmlTable(dta);
         //====================== PERFORMANCE INDICATORS ======================================================
         function inicator_chart() {
             var options = {
                 chart: {
                     renderTo: 'indchart',
                     type: 'bar',
                     //marginRight: 130,
                     //marginBottom: 125
                 },
                 plotOptions: {
                     series: {
                         stacking: 'percent'
                     }
                 },
                 title: {
                     text:'<h1>Performance Indicators</h1>',
                     x: -10 //center
                 },
                 subtitle: {
                     text: '',
                     x: -10
                 },
                 xAxis: {
                     categories: [],
                     labels: {
                         useHTML: true
                     }
                 },
                credits: {
                    enabled: false
                },                    
                 yAxis: {
                     title: {
                         text: 'percentage'
                     },
                     plotLines: [{
                         value: 0,
                         width: 1,
                         color: '#808080'
                     }]
                 },
                 tooltip: {
                     formatter: function() {
                         return '<b>' + this.series.name + '</b><br/>' +
                             this.x + ': ' + this.y;
                     }
                 },
                 legend: {
                     reversed: true
                         //layout: 'vertical',
                         //align: 'bottom',
                         //verticalAlign: 'top',
                         //x: -10,
                         //y: 100,
                         //borderWidth: 0
                 },
                 series: [],
             };

             var Cured = {
                     name: 'Cured',
                     data: []
                 },
                 Deaths = {
                     name: 'Deaths',
                     data: []
                 },
                 Defaults = {
                     name: 'Defaults',
                     data: []
                 };
             options.xAxis.categories = tCategories;
             Highcharts.each(mydata, function(p) {
                 options.xAxis.categories.push(p.Province);
                 Cured.data.push(p.cured);
                 Deaths.data.push(p.deaths);
                 Defaults.data.push(p.defaulter);
             });

             options.series[0] = Cured;
             options.series[1] = Deaths;
             options.series[2] = Defaults;
             chart1 = new Highcharts.Chart(options);
         }

         //====================== boys VS girls =====================================================
         function male_female() {
             var options = {
                 chart: {
                     renderTo: 'sexchart',
                     type: 'bar'
                 },
                 plotOptions: {
                     series: {
                         stacking: 'percent',
                         animation: {
                             duration: 4000
                         }
                     }
                 },
                credits: {
                    enabled: false
                },                    
                 title: {
                     text: 'Boys vs Girls',
                     x: -10 //center
                 },
                 subtitle: {
                     text: '',
                     x: -10
                 },
                 xAxis: {
                     categories: [],
                     labels: {
                         useHTML: true
                     }
                 },
                 yAxis: {
                     title: {
                         text: 'percentage'
                     },
                     className: 'highcharts-color-2',
                     plotLines: [{
                         value: 0,
                         width: 1,
                         color: '#808080'
                     }]
                 },
                 tooltip: {
                     formatter: function() {
                         return '<b>' + this.series.name + '</b><br/>' +
                             this.x + ': ' + this.y;
                     }
                 },
                 legend: {
                     reversed: true
                 },
                 series: [],

             };

             var Male = {
                     name: 'Boys',
                     data: []
                         // color: 'black',
                 },
                 Female = {
                     name: 'Girls',
                     data: [],
                     color: 'rgba(0,204,102,0.9)'
                 }
             options.xAxis.categories = tCategories;
             Highcharts.each(mydata, function(p) {
                 options.xAxis.categories.push(p.Province);
                 Male.data.push(p.male);
                 Female.data.push(p.female);
             });

             options.series[0] = Male;
             options.series[1] = Female;
             chart2 = new Highcharts.Chart(options);
         }

        //  $.getJSON('/Dash/data/provincial_all_years_month.txt', function(data2) {
             //var data2 = [];
             var meta = [
                 { Ord: 1, "Title": 'Total new children admitted ' },
                 { Ord: 2, "Title": 'Under 6 new children ' },
                 { Ord: 3, "Title": '6 - 23 new children ' },
                 { Ord: 4, "Title": '24 - 59 new children ' },
                 { Ord: 5, "Title": 'Total children discharged' },
                 { Ord: 6, "Title": 'Total children cured' },
                 { Ord: 7, "Title": 'Total children died' },
                 { Ord: 8, "Title": 'Total children defaulted' },
                 { Ord: 9, "Title": 'Total boys admitted' },
                 { Ord: 10, "Title": 'Total girls admitted'},
                 {  Ord: 11, "Title": 'Total children non-cured' }
             ];

             mapTitle = meta[0].Title;

             admissions = (function(d) {
                 var tab = [];
                 Highcharts.each(d, function(point, i) {
                     tab.push({
                         'PROV_32_ID': parseInt(point['proV_32_ID']),
                         'Pname': mydata[i].pname,
                         'value': mydata[i].value > 0 ? mydata[i].value : null
                     });
                 });
                 return tab;
             }(mydata));

             childrenu6 = (function(d) {
                 var tab2 = [];
                 Highcharts.each(d, function(point, i) {
                     tab2.push({
                         'PROV_32_ID': parseInt(point['proV_32_ID']),
                         'Pname': mydata[i].pname,
                         'value': mydata[i].children6m > 0 ? mydata[i].children6m : null
                     });
                 });
                 return tab2;
             }(mydata));

             childrenu23 = (function(d) {
                 var tab3 = [];
                 Highcharts.each(d, function(point, i) {
                     tab3.push({
                         'PROV_32_ID': parseInt(point['proV_32_ID']),
                         'Pname': mydata[i].pname,
                         'value': mydata[i].children23m > 0 ? mydata[i].children23m : null
                     });
                 });

                 return tab3;
             }(mydata));

             childrenu59 = (function(d) {
                 var tab5 = [];
                 Highcharts.each(d, function(point, i) {
                     tab5.push({
                         'PROV_32_ID': parseInt(point['proV_32_ID']),
                         'Pname': mydata[i].pname,
                         'value': mydata[i].children59m > 0 ? mydata[i].children59m : null
                     });
                 });
                 return tab5;
             }(mydata));

             totaldischarge = (function(d) {
                 var tab4 = [];
                 Highcharts.each(d, function(point, i) {
                     tab4.push({
                         'PROV_32_ID': parseInt(point['proV_32_ID']),
                         'Pname': mydata[i].pname,
                         'value': mydata[i].discharge > 0 ? mydata[i].discharge : null
                     });
                 });
                 return tab4;
             }(mydata));

             totalcured = (function(d) {
                 var tab6 = [];
                 Highcharts.each(d, function(point, i) {
                     tab6.push({
                         'PROV_32_ID': parseInt(point['proV_32_ID']),
                         'Pname': mydata[i].pname,
                         'value': mydata[i].cured > 0 ? mydata[i].cured : null
                     });
                 });
                 return tab6;
             }(mydata));

             totaldeaths = (function(d) {
                 var tab7 = [];
                 Highcharts.each(d, function(point, i) {
                     tab7.push({
                         'PROV_32_ID': parseInt(point['proV_32_ID']),
                         'Pname': mydata[i].pname,
                         'value': mydata[i].deaths > 0 ? mydata[i].deaths : null
                     });
                 });
                 return tab7;
             }(mydata));

             defaults = (function(d) {
                 var tab3 = [];
                 Highcharts.each(d, function(point, i) {
                     tab3.push({
                         'PROV_32_ID': parseInt(point['proV_32_ID']),
                         'Pname': mydata[i].pname,
                         'value': mydata[i].defaulter > 0 ? mydata[i].defaulter : null
                     });
                 });
                 return tab3;
             }(mydata));

             boys = (function(d) {
                 var tab3 = [];
                 Highcharts.each(d, function(point, i) {
                     tab3.push({
                         'PROV_32_ID': parseInt(point['proV_32_ID']),
                         'Pname': mydata[i].pname,
                         'value': mydata[i].male > 0 ? mydata[i].male : null
                     });
                 });
                 return tab3;
             }(mydata));

             girls = (function(d) {
                 var tab3 = [];
                 Highcharts.each(d, function(point, i) {
                     tab3.push({
                         'PROV_32_ID': parseInt(point['proV_32_ID']),
                         'Pname': mydata[i].pname,
                         'value': mydata[i].female > 0 ? mydata[i].female : null
                     });
                 });
                 return tab3;
             }(mydata));
             noncures = (function(d) {
                 var tab3 = [];
                 Highcharts.each(d, function(point, i) {
                     tab3.push({
                         'PROV_32_ID': parseInt(point['proV_32_ID']),
                         'Pname': mydata[i].pname,
                         'value': mydata[i].nonCured > 0 ? mydata[i].nonCured : null
                     });
                 });
                 return tab3;
             }(mydata));


             mapChart = Highcharts.mapChart('container', {
                     title: {
                         text: meta[0].Title
                     },
                     mapNavigation: {
                         enabled: true,
                         buttonOptions: {
                             verticalAlign: 'bottom'
                         }
                     },
                     legend: {
                         layout: 'vertical',
                         align: 'right',
                         borderWidth: 0,
                         symbolHeight: 150,
                         backgroundColor: 'rgba(255,255,255,0.85)',
                         floating: true,
                         verticalAlign: 'bottom',
                         y: 10
                     },
                     colorAxis: {
                         min: 1,
                         type: 'logarithmic',
                         minColor: '#EEEEFF',
                         maxColor: '#000022',
                         stops: [
                             [0, '#EFEFFF'],
                             [0.70, '#4444FF'],
                             [1, '#000022']
                         ]
                     },
                     credits: {
                         enabled: false
                     },
                     series: [{
                         animation: {
                             duration: 6000
                         },
                         data: $.extend(true, [], admissions),
                         mapData: provinces,
                         joinBy: ['PROV_32_ID', 'PROV_32_ID'],
                         name: meta[0].Title,
                         color: '#E0E0E0',
                         nullColor: 'red',
                         allowPointSelect: true,
                         cursor: 'pointer',
                         states: {
                             select: {
                                 color: '#32CD32'
                             },
                             hover: {
                                 color: '#a4edba'
                             }
                         },

                         dataLabels: {
                             enabled: true,
                             format: '{point.Pname}'
                         },

                         tooltip: {
                             pointFormat: '{point.value}'
                         }

                     }]

                 },
                 function(chart) {
                     $('#idAdmits').click(function() {
                         mapChart.series[0].setData($.extend(true, [], admissions));
                         mapChart.setTitle({ text: meta[0].Title });
                         mapChart.series[0].update({name:meta[0].Title});
                         $('#idAdmits').addClass("backg");
                         $('#idchild6').removeClass("backg");
                         $('#idchild23').removeClass("backg");
                         $('#idchild2359').removeClass("backg");
                         $('#iddischarge').removeClass("backg");
                         $('#idcured').removeClass('backg');
                         $('#iddeath').removeClass('backg');
                         $('#iddefaults').removeClass("backg");
                         $('#idboys').removeClass("backg");
                         $('#idgirls').removeClass("backg");
                         $('#idnoncured').removeClass("backg");
                     });
                     $('#idchild6').click(function() {
                         mapChart.series[0].setData($.extend(true, [], childrenu6));
                         mapChart.setTitle({ text: meta[1].Title });
                         mapChart.series[0].update({name:meta[1].Title});
                         $('#idAdmits').removeClass("backg");
                         $('#idchild6').addClass("backg");
                         $('#idchild23').removeClass("backg");
                         $('#idchild2359').removeClass("backg");
                         $('#iddischarge').removeClass("backg");
                         $('#idcured').removeClass('backg');
                         $('#iddeath').removeClass('backg');
                         $('#iddefaults').removeClass("backg");
                         $('#idboys').removeClass("backg");
                         $('#idgirls').removeClass("backg");
                         $('#idnoncured').removeClass("backg");
                     });

                     $('#idchild23').click(function() {
                         mapChart.series[0].setData($.extend(true, [], childrenu23));
                         mapChart.setTitle({ text: meta[2].Title });
                         mapChart.series[0].update({name:meta[2].Title});
                         $('#idAdmits').removeClass("backg");
                         $('#idchild6').removeClass("backg");
                         $('#idchild23').addClass("backg");
                         $('#idchild2359').removeClass("backg");
                         $('#iddischarge').removeClass("backg");
                         $('#idcured').removeClass("backg");
                         $('#iddeath').removeClass("backg");
                         $('#iddefaults').removeClass("backg");
                         $('#idboys').removeClass("backg");
                         $('#idgirls').removeClass("backg");
                         $('#idnoncured').removeClass("backg");

                     });
                     $('#idchild2359').click(function() {
                         mapChart.series[0].setData($.extend(true, [], childrenu59));
                         mapChart.setTitle({ text: meta[3].Title });
                         mapChart.series[0].update({name:meta[3].Title});
                         $('#idAdmits').removeClass("backg");
                         $('#idchild6').removeClass("backg");
                         $('#idchild23').removeClass("backg");
                         $('#idchild2359').addClass("backg");
                         $('#iddischarge').removeClass("backg");
                         $('#idcured').removeClass("backg");
                         $('#iddeath').removeClass("backg");
                         $('#iddefaults').removeClass("backg");
                         $('#idboys').removeClass("backg");
                         $('#idgirls').removeClass("backg");
                         $('#idnoncured').removeClass("backg");
                     });
                     $('#iddischarge').click(function() {
                         mapChart.series[0].setData($.extend(true, [], totaldischarge));
                         mapChart.setTitle({ text: meta[4].Title });
                         mapChart.series[0].update({name:meta[4].Title});
                         $('#idAdmits').removeClass("backg");
                         $('#idchild6').removeClass("backg");
                         $('#idchild23').removeClass("backg");
                         $('#idchild2359').removeClass("backg");
                         $('#iddischarge').addClass("backg");
                         $('#idcured').removeClass("backg");
                         $('#iddeath').removeClass("backg");
                         $('#iddefaults').removeClass("backg");
                         $('#idboys').removeClass("backg");
                         $('#idgirls').removeClass("backg");
                         $('#idnoncured').removeClass("backg");
                     });

                     $('#idcured').click(function() {
                         mapChart.series[0].setData($.extend(true, [], totalcured));
                         mapChart.setTitle({ text: meta[5].Title });
                         mapChart.series[0].update({name:meta[5].Title});
                         $('#idAdmits').removeClass("backg");
                         $('#idchild6').removeClass("backg");
                         $('#idchild23').removeClass("backg");
                         $('#idchild2359').removeClass("backg");
                         $('#iddischarge').removeClass("backg");
                         $('#idcured').addClass("backg");
                         $('#iddeath').removeClass("backg");
                         $('#iddefaults').removeClass("backg");
                         $('#idboys').removeClass("backg");
                         $('#idgirls').removeClass("backg");
                         $('#idnoncured').removeClass("backg");

                     });

                     $('#iddeath').click(function() {
                         mapChart.series[0].setData($.extend(true, [], totaldeaths));
                         mapChart.setTitle({ text: meta[6].Title });
                         mapChart.series[0].update({name:meta[6].Title});
                         $('#idAdmits').removeClass("backg");
                         $('#idchild6').removeClass("backg");
                         $('#idchild23').removeClass("backg");
                         $('#idchild2359').removeClass("backg");
                         $('#iddischarge').removeClass("backg");
                         $('#idcured').removeClass("backg");
                         $('#iddeath').addClass("backg");
                         $('#iddefaults').removeClass("backg");
                         $('#idboys').removeClass("backg");
                         $('#idgirls').removeClass("backg");
                         $('#idnoncured').removeClass("backg");
                     });

                     $('#iddefaults').click(function() {
                         mapChart.series[0].setData($.extend(true, [], defaults));
                         mapChart.setTitle({ text: meta[7].Title });
                         mapChart.series[0].update({name:meta[7].Title});
                         $('#idAdmits').removeClass("backg");
                         $('#idchild6').removeClass("backg");
                         $('#idchild23').removeClass("backg");
                         $('#idchild2359').removeClass("backg");
                         $('#iddischarge').removeClass("backg");
                         $('#idcured').removeClass("backg");
                         $('#iddeath').removeClass("backg");
                         $('#iddefaults').addClass("backg");
                         $('#idboys').removeClass("backg");
                         $('#idgirls').removeClass("backg");
                         $('#idnoncured').removeClass("backg");
                     });

                     $('#idboys').click(function() {
                         mapChart.series[0].setData($.extend(true, [], boys));
                         mapChart.setTitle({ text: meta[8].Title });
                         mapChart.series[0].update({name:meta[8].Title});
                         $('#idAdmits').removeClass("backg");
                         $('#idchild6').removeClass("backg");
                         $('#idchild23').removeClass("backg");
                         $('#idchild2359').removeClass("backg");
                         $('#iddischarge').removeClass("backg");
                         $('#idcured').removeClass("backg");
                         $('#iddeath').removeClass("backg");
                         $('#iddefaults').removeClass("backg");
                         $('#idboys').addClass("backg");
                         $('#idgirls').removeClass("backg");
                         $('#idnoncured').removeClass("backg");
                     });
                     $('#idgirls').click(function() {
                         mapChart.series[0].setData($.extend(true, [], girls));
                         mapChart.setTitle({ text: meta[9].Title });
                         mapChart.series[0].update({name:meta[9].Title});
                         $('#idAdmits').removeClass("backg");
                         $('#idchild6').removeClass("backg");
                         $('#idchild23').removeClass("backg");
                         $('#idchild2359').removeClass("backg");
                         $('#iddischarge').removeClass("backg");
                         $('#idcured').removeClass("backg");
                         $('#iddeath').removeClass("backg");
                         $('#iddefaults').removeClass("backg");
                         $('#idboys').removeClass("backg");
                         $('#idgirls').addClass("backg");
                         $('#idnoncured').removeClass("backg");
                     });
                        $('#idnoncured').click(function() {
                         mapChart.series[0].setData($.extend(true, [], noncures));
                         mapChart.setTitle({ text: meta[10].Title });
                         mapChart.series[0].update({name:meta[10].Title});
                         $('#idAdmits').removeClass("backg");
                         $('#idchild6').removeClass("backg");
                         $('#idchild23').removeClass("backg");
                         $('#idchild2359').removeClass("backg");
                         $('#iddischarge').removeClass("backg");
                         $('#idcured').removeClass("backg");
                         $('#iddeath').removeClass("backg");
                         $('#iddefaults').removeClass("backg");
                         $('#idboys').removeClass("backg");
                         $('#idgirls').removeClass("backg");
                         $('#idnoncured').addClass("backg");
                     });
                 });
             // Pre-select a province
             mapChart.series[0].data[0].select(false);

             /** 
    var mapChart, countryChart;

    var proveencies = [];


    var UniqProv = [];
    var UniqName = [];

    for (i = 0; i < mydata.length; i++) {
        if (UniqProv.indexOf(mydata[i].proV_32_ID) === -1) {
            UniqProv.push(mydata[i].proV_32_ID);
            UniqName.push(mydata[i].pname);
        }
    };


    //creating arrary for each Province (UniqProv)
    for (j = 0; j < UniqProv.length; j++) {
        var prv = UniqProv[j];
        var nm = UniqName[j];
        //filter a single province
        var prov = $.grep(mydata, function (element, index) {
            return element.proV_32_ID=== prv;
        });

    //    //move data value to arrary
        var arr = [];
        var cat = [];

        for (var p in prov) {
            arr.push({ name: prov[p].Time, y: prov[p].Children59m });
        }
        //adding all arrary together
        proveencies[j] = {
            code: parseInt(prv),
            name: nm,
            data: arr
        };
    };


    var data = [];
    for (var code in proveencies) {
        if (proveencies.hasOwnProperty(code)) {
            var value = null,
                year,
                itemData = proveencies[code].data,
                i = itemData.length;

            while (i--) {
                if (typeof itemData[i] === 'number') {
                    value = itemData[i];
                    year = categories[i];
                    break;
                }
            }
            data.push({
                name: proveencies[code].name,
                code: code,
                value: value,
                year: year,
                pointData: itemData
            });
        }
    }

    // Wrap point.select to get to the total selected points
    Highcharts.wrap(Highcharts.Point.prototype, 'select', function (proceed) {

        var categories = [];
        proceed.apply(this, Array.prototype.slice.call(arguments, 1));

        var points = mapChart.getSelectedPoints();
        if (points.length) {
            if (points.length === 1) {
                $('#info h2').html(points[0].Pname);
            } else {
                $('#info h4').html('');

            }
            $('#info .subheader').html('<small><em>Shift + Click on map to compare provinces</em></small>');

            if (!countryChart) {
                countryChart = Highcharts.chart('country-chart', {
                    chart: {
                        height: 250,
                        spacingLeft: 0
                    },
                    credits: {
                        enabled: false
                    },
                    title: {
                        text: null
                    },
                    subtitle: {
                        text: null
                    },
                    xAxis: {
                        crosshair: true,
                        uniqueNames: true,
                        type: 'category',
                        labels: {
                            rotation: -90
                        }
                    },
                    yAxis: {
                        title: null,
                        opposite: true
                    },
                    tooltip: {
                        split: true
                    },
                    plotOptions: {
                        series: {
                            animation: {
                                duration: 500
                            },
                            marker: {
                                enabled: false
                            },
                            threshold: 0,
                        }
                    }
                });
            }
            $.each(points, function (i, point) {
                Highcharts.each(data[point.index].pointData, function (p) {
                    if (categories.indexOf(p.name) === -1) {
                        categories.push(p.name);
                    }
                });
                categories.sort(function (a, b) {
                    return a - b;
                });
                countryChart.xAxis[0].update({ categories: categories });
                if (countryChart.series[i]) {
                    countryChart.series[i].update({
                        name: this.Pname,
                        data: data[point.index].pointData,
                        type: points.length > 1 ? 'line' : 'area'
                    }, false);
                } else {
                    countryChart.addSeries({
                        name: this.Pname,
                        data: data[point.index].pointData,
                        type: points.length > 1 ? 'line' : 'area'
                    }, false);
                }
            });
            while (countryChart.series.length > points.length) {
                countryChart.series[countryChart.series.length - 1].remove(false);
            }
            countryChart.redraw();

        } else {
            $('#info #flag').attr('class', '');
            $('#info h2').html('');
            $('#info .subheader').html('');
            if (countryChart) {
                countryChart = countryChart.destroy();
            }
        }
    });

**/
        //  });

     }).done(function() {
         document.getElementById('dashboard').style.display = 'block';
         $("#sendbtn").text('Get');
         document.getElementById("sendbtn").disabled = false;
         chart1.reflow();
         chart2.reflow();
         chart3.reflow();
     })

 }
 $(document).ready(function() {
     send(2015);
 });