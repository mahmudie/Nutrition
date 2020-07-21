$.getJSON('/Dash/data/data_sparkline.txt', function (JSONdata) {
    Highcharts.SparkLine = function (a, b, c) {
        var hasRenderToArg = typeof a === 'string' || a.nodeName,
          options = arguments[hasRenderToArg ? 1 : 0],
          defaultOptions = {
              chart: {
                  renderTo: (options.chart && options.chart.renderTo) || this,
                  backgroundColor: null,
                  borderWidth: 0,
                  type: 'area',
                  margin: [2, 0, 2, 0],
                  width: 120,
                  height: 20,
                  style: {
                      overflow: 'visible'
                  },

                  // small optimalization, saves 1-2 ms each sparkline
                  skipClone: true
              },
              title: {
                  text: ''
              },
              credits: {
                  enabled: false
              },
              xAxis: {
                  categories: Highcharts.getOptions().lang.shortMonths,
                  labels: {
                      enabled: false
                  },
                  title: {
                      text: null
                  },
                  startOnTick: false,
                  endOnTick: false,
                  tickPositions: []
              },
              yAxis: {
                  endOnTick: false,
                  startOnTick: false,
                  labels: {
                      enabled: false
                  },
                  title: {
                      text: null
                  },
                  tickPositions: [0]
              },
              legend: {
                  enabled: false
              },
              tooltip: {
                  backgroundColor: null,
                  borderWidth: 0,
                  shadow: false,
                  useHTML: true,
                  hideDelay: 0,
                  shared: true,
                  padding: 0,
                  positioner: function (w, h, point) {
                      return {
                          x: point.plotX - w / 2,
                          y: point.plotY - h
                      };
                  }
              },
              plotOptions: {
                  series: {
                      animation: false,
                      lineWidth: 1,
                      shadow: false,
                      states: {
                          hover: {
                              lineWidth: 1
                          }
                      },
                      marker: {
                          radius: 1,
                          states: {
                              hover: {
                                  radius: 2
                              }
                          }
                      },
                      fillOpacity: 0.25
                  },
                  column: {
                      negativeColor: '#910000',
                      borderColor: 'silver'
                  }
              }
          };

        options = Highcharts.merge(defaultOptions, options);

        return hasRenderToArg ?
          new Highcharts.Chart(a, options, c) :
          new Highcharts.Chart(options, b);
    };

    var start = +new Date(),
      $tbody = $('#tbody-sparkline');

    // Creating 153 sparkline charts is quite fast in modern browsers, but IE8 and mobile
    // can take some seconds, so we split the input into chunks and apply them in timeouts
    // in order avoid locking up the browser process and allow interaction.
    function doChunk() {
        var i,
          len = JSONdata.length,
          $tr,
          stringdata,
          arr,
          chart,
          mtrendChart, ctrendChart,
          mData, cData;

        for (i = 0; i < len; i += 1) {
            $tr = $tbody.append('<tr><th>' + JSONdata[i]['Province'] +
              '</th><td>' + JSONdata[i]['Admissions'] +
              '</td><td class="mtrend-chart"></td><td>' + JSONdata[i]['Cures'] +
              '</td><td class="ctrend-chart"></td></tr>').find('tr');
            $tr = $($tr[$tr.length - 1]);
            mtrendChart = $tr.find('.mtrend-chart');
            ctrendChart = $tr.find('.ctrend-chart');

            mData = JSON.parse("[" + JSONdata[i]['Mtrend'] + "]");

            $(mtrendChart).highcharts('SparkLine', {
                series: [{
                    data: mData
                }],
                tooltip: {
                    headerFormat: '<span style="font-size: 10px">' + JSONdata[i]['Province'] + ', {point.key}:</span><br/>',
                    pointFormat: '<b>{point.y}.000</b> USD'
                }
            });


            arr = JSONdata[i]['CureTrend'].split(', ');
            cData = $.map(arr, parseFloat);
            cData.pop(); // remove series type

            $(ctrendChart).highcharts('SparkLine', {
                series: [{
                    data: cData
                }],
                tooltip: {
                    headerFormat: '<span style="font-size: 10px">' + JSONdata[i]['Province'] + ', {point.key}:</span><br/>',
                    pointFormat: '<b>{point.y}</b>'
                },
                chart: {
                    type: arr[arr.length - 1] // last item is the series type
                }
            });
        }
    }

    doChunk();
})