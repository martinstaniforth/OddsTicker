(function () {
  'use strict';

  // Factory name is handy for logging
  var serviceId = 'reusableCharts';

  // Define the factory on the module.
  // Inject the dependencies. 
  // Point to the factory definition function.
  // TODO: replace app with your module name
  angular.module('reusableCharts').factory(serviceId, [reusableCharts]);

  function reusableCharts() {
    // Define the functions and properties to reveal.
    var service = {
      horizontalStackedBarChart: horizontalStackedBarChart,
      oddsMatrix: oddsMatrix
    };

    return service;

    function oddsMatrix() {
      var margin = { top: 10, right: 10, bottom: 10, left: 10 },
          width = 600,
          height = 100,
          ease = 'cubic-in-out';
      var svg, duration = 500, matrix, cells, captions, bookmakerHeadings, outcomeHeadings;

      var Matrix = function () {
        var width = 1,
            height = 1;

        var matrix = function (data) {
          var cols = data.bookmakers.length + 1,
              rows = data.outcomes.length + 1,
              sizeW = Math.floor(width / cols),
              sizeH = Math.floor(height / rows);

          data.outcomeHead = [],
          data.bookmakerHead = [];

          data.values.forEach(function (d) {
            d.x = (data.bookmakers.indexOf(d.bookmaker) + 1) * sizeW;
            d.y = (data.outcomes.indexOf(d.outcome) + 1) * sizeH;
            d.dx = sizeW;
            d.dy = sizeH;
          });

          data.bookmakers.forEach(function (d) {
            var b = {};
            b.x = (data.bookmakers.indexOf(d) + 1) * sizeW + sizeW / 2;
            b.y = sizeH / 2;
            b.heading = d;
            b.anchor = 'middle';

            data.bookmakerHead.push(b);
          });

          data.outcomes.forEach(function (d) {
            var o = {};
            o.x = sizeW - 5;
            o.y = (data.outcomes.indexOf(d) + 1) * sizeH + sizeH / 2;
            o.heading = d;
            o.anchor = 'end';

            data.outcomeHead.push(o);
          });

          return data;
        };

        matrix.setSize = function (w, h) {
          width = w;
          height = h;
          return matrix;
        };

        matrix.values = function (data) {
          return matrix(data).values;
        };

        matrix.bookmakers = function (data) {
          return matrix(data).bookmakerHead;
        };

        matrix.outcomes = function (data) {
          return matrix(data).outcomeHead;
        };

        return matrix;

      };
      
      function exports(_selection) {
        _selection.each(function (_data) {
          var newData = modifyData(_data);

          if (!svg) {
            svg = d3.select(this)
                    .append('svg')
                    .classed('matrix', true)
                    .attr("width", width)
                    .attr("height", height);

            matrix = new Matrix().setSize(width, height);
          }

          function cell() {
            this
              .attr(
                {
                  x: function (d) { return d.x; },
                  y: function (d) { return d.y; },
                  width: function (d) { return d.dx - 5; },
                  height: function (d) { return d.dy - 5; }
                });
          }

          function caption() {
            this
              .text(function (d) { return d.odd; })
               .attr('x', function (d) { return d.x + (d.dx / 2); })
               .attr('y', function (d) { return d.y + (d.dy / 2); })
               .attr('font-family', 'sans-serif')
               .attr('font-size', '12px')
               .attr('fill', 'black')
               .attr('text-anchor', 'middle');

          }

          function headings() {
            this
              .text(function (d) { return d.heading; })
              .attr({
                x: function (d) {
                  return d.x;
                },
                y: function (d) {
                  return d.y;
                },
                'font-family': 'sans-serif',
                'font-size': '12px',
                'fill': 'black',
                'text-anchor': function (d) {
                  return d.anchor;
                }
              });

          }
          
          
          cells = svg.data([newData])
                     .selectAll('rect')
                     .data(matrix.values);

          captions = svg.data([newData])
                        .selectAll('text.caption')
                        .data(matrix.values);

          bookmakerHeadings
            = svg.data([newData])
                 .selectAll('text.heading')
                 .data(matrix.bookmakers);

          outcomeHeadings
            = svg.data([newData])
                 .selectAll('text.outcome-heading')
                 .data(matrix.outcomes);

          cells.enter().append('svg:rect')
               .classed('odds', true)
               .classed('neutral-odds', true)
               .attr({
                 x: function (d) { return 0; },
                 y: function (d) { return 0; },
                 width: function (d) { return 0; },
                 height: function (d) { return 0; }
               })
               .transition()
               .delay(function (d, i) { return i * 20; })
               .duration(500)
               .call(cell);

          captions.enter().append('text')
              .classed('caption', true)
              .attr({
                x: function (d) { return 0; },
                y: function (d) { return 0; }
              })
              .transition()
               .delay(function (d, i) { return i * 20; })
               .duration(500)
               .call(caption);

          bookmakerHeadings.enter().append('text')
             .classed('heading', true)
             .attr({
               x: function (d) { return 0; },
               y: function (d) { return 0; }
             })
             .transition()
             .delay(function (d, i) { return i * 10; })
             .duration(250)
             .call(headings);

          outcomeHeadings.enter().append('text')
             .classed('outcome-heading', true)
             .attr({
               x: function (d) { return 0; },
               y: function (d) { return 0; }
             })
             .transition()
             .delay(function (d, i) { return i * 10; })
             .duration(250)
             .call(headings);

          cells.transition()
               .delay(function (d, i) { return i * 20; })
               .duration(500)
               .call(cell);

          captions.transition()
               .delay(function (d, i) { return i * 20; })
               .duration(500)
               .call(caption);

          bookmakerHeadings.transition()
               .delay(function (d, i) { return i * 10; })
               .duration(250)
               .call(headings);

          outcomeHeadings.transition()
               .delay(function (d, i) { return i * 10; })
               .duration(250)
               .call(headings);

          cells.exit().transition()
               .delay(function (d, i) { return i * 20; })
               .duration(500)
               .attr({
                 x: function (d, i) { return -400; },
                 y: function (d, i) { return -400; }
               })
               .remove();

          captions.exit().transition()
                .delay(function (d, i) { return i * 20; })
                .duration(500);

          function modifyData(data) {
            var bookmakers = [],
                outcomes = [],
                values = [];

            data.forEach(function (outcome) {
              if (outcomes.indexOf(outcome.outcome) < 0) {
                outcomes.push(outcome.outcome);
              }
              outcome.latestOdds.forEach(function (odd) {
                if (bookmakers.indexOf(odd.bookmaker) < 0) {
                  bookmakers.push(odd.bookmaker);
                }
                values.push({ bookmaker: odd.bookmaker, outcome: outcome.outcome, odd: odd.odd });
              });
            });

            return {
              bookmakers: bookmakers,
              outcomes: outcomes,
              values: values
            };
          }



        });
      }

      exports.width = function (_x) {
        if (!arguments.length) return width;
        width = parseInt(_x);
        //duration = 0;
        return this;
      };
      exports.height = function (_x) {
        if (!arguments.length) return height;
        height = parseInt(_x);
        //duration = 0;
        return this;
      };
      exports.ease = function (_x) {
        if (!arguments.length) return height;
        ease = _x;
        return this;
      };

      return exports;

      function computeCols(n, width, captionMax) {
        var remainingWidth = Math.min(captionMax, width / 3);
        var ret = [];
        ret.push(remainingWidth);
        for (var i = 0; i < n; i++) {
          ret.push(Math.floor(remainingWidth / n));
        }
        return ret;
      }


    }


    function horizontalStackedBarChart() {
      //initialisation
      var margin = { top: 0, right: 0, bottom: 0, left: 0 },
          width = 100,
          height = 30,
          n = 3,
          ease = 'cubic-in-out';

      var svg, duration = 500;
      var dispatch = d3.dispatch('customHover');

      function exports(_selection) {
        _selection.each(function (_data) {
          var chartW = width - margin.left - margin.right,
              chartH = height - margin.top - margin.bottom,
              m = 1,// _data.length,
              stack = d3.layout.stack(),
              layers = stack(d3.range(n).map(function (d) {
                var a = [];
                a[0] = {
                  x: 0,
                  y: _data[d].latestProb,
                  outcome: _data[d].outcome,
                  latestProb: _data[d].latestProb
                };
                return a;
              })),
              labels = _data.map(function (d) { return d.outcome; });

          var yGroupMax = d3.max(layers, function (layer) { return d3.max(layer, function (d) { return d.y; }); });
          //the largest stack
          var yStackMax = d3.max(layers, function (layer) { return d3.max(layer, function (d) { return d.y0 + d.y; }); });

          var yScale = d3.scale.ordinal()
                         .domain(d3.range(m))
                         .rangeRoundBands([2, chartH], 0.08);

          var xScale = d3.scale.linear()
                         .domain([0, yStackMax])
                         .range([0, chartW]);


          var color = d3.scale.linear()
                        .domain([0, n - 1])
                        .range(['#aad', '#556']);

          if (!svg) {
            svg = d3.select(this)
                    .append('svg')
                    .classed('chart', true);
            var container = svg.append('g').classed('container-group', true);
            container.append('g').classed('chart-group', true);
          }

          svg.transition().duration(duration).attr({ width: width, height: height });
          svg.select('.container-group')
             .attr({ transform: 'translate(' + margin.left + ',' + margin.top + ')' });


          var layer = svg.select('.chart-group')
                         .selectAll('.layer')
                         .data(layers);

          layer.enter()
               .append('g')
               .classed('layer', true)
               .style('fill', function (d, i) { return color(i); });

          layer.selectAll('rect')
               .data(function (d) { return d; })
               .enter()
               .append('rect')
               .attr({
                 x: function (d) { return xScale(d.y0); },
                 y: function (d) { return yScale(d.x); },
                 height: yScale.rangeBand(),
                 width: function (d) { return xScale(d.y); }
               });

          layer.selectAll('rect')
               .data(function (d) { return d; })
               .transition()
               .duration(duration)
               .ease(ease)
               .attr({
                 x: function (d) { return xScale(d.y0); },
                 y: function (d) { return yScale(d.x); },
                 height: yScale.rangeBand(),
                 width: function (d) { return xScale(d.y); }
               });

          var caption = svg.select('.chart-group')
                         .selectAll('.caption')
                         .data(layers);

          caption.enter()
               .append('g')
               .classed('caption', true);


          caption.selectAll('text')
                  .data(function (d) { return d; })
                  .enter()
                  .append('text')
                  .attr('x', function (d) {
                    return xScale(d.y0 + (d.y / 2));
                  })
                  .attr('y', function (d) {
                    return chartH / 2 + 5;
                  })
                  .text(function (d) {
                    return d.outcome + ' (' + (d.latestProb * 100).toFixed() + '%)';
                  })
                  .attr('font-family', 'sans-serif')
                  .attr('font-size', '12px')
                  .attr('fill', 'white')
                  .attr('text-anchor', 'middle');

          caption.selectAll('text')
                 .data(function (d) { return d; })
                 .transition()
                 .duration(duration)
                 .ease(ease)
                 .attr('x', function (d) {
                    return xScale(d.y0 + (d.y / 2));
                  })
                  .attr('y', function (d) {
                    return chartH / 2 + 5;
                  })
                  .text(function (d) { return d.outcome + ' (' + (d.latestProb * 100).toFixed() + '%)'; })
                  .attr('font-family', 'sans-serif')
                  .attr('font-size', '12px')
                  .attr('fill', 'white')
                  .attr('text-anchor', 'middle');


          duration = 500;

        });
      }
      exports.width = function (_x) {
        if (!arguments.length) return width;
        width = parseInt(_x);
        //duration = 0;
        return this;
      };
      exports.height = function (_x) {
        if (!arguments.length) return height;
        height = parseInt(_x);
        //duration = 0;
        return this;
      };
      exports.ease = function (_x) {
        if (!arguments.length) return height;
        ease = _x;
        return this;
      };

      return exports;
    }

    //#region Internal Methods        

    //#endregion
  }
})();