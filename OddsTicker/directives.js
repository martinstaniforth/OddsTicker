(function () {
  'use strict';

  var app = angular.module('app');

  app.directive('otOddsGrid', ['$window', '$timeout', 'reusableCharts',
    function ($window, $timeout, reusableCharts) {

      var directive = {
        link: link,
        restrict: 'EA',
        scope: {
          data: '='
        }
      }

      return directive;

      function link(scope, element, attrs) {
        var chart = reusableCharts.oddsMatrix();
        var chartElement = d3.select(element[0]);
        var renderTimeout;

        $window.onresize = function () {
          scope.$apply();
        };

        scope.$watch('data', function (newData) {
          if (newData && newData[0].latestOdds) {
            scope.render(newData);
          }
        }, true);

        scope.render = function (data) {
          if (!data) return;
          if (renderTimeout) clearTimeout(renderTimeout);

          renderTimeout = $timeout(function () {
            chartElement.datum(data).call(chart);
          }, 200);
        };
      }
    }]);

  app.directive('otD3Bars', ['$window', '$timeout', 'reusableCharts',
    function ($window, $timeout, reusableCharts) {
      
      return {
        link: link,
        restrict: 'EA',
        scope: {
          data: '=',
        }
      }
      function link(scope, element, attrs) {
        var chart = reusableCharts.horizontalStackedBarChart()
        var chartElement = d3.select(element[0]);
        var renderTimeout;

        $window.onresize = function () {
          scope.$apply();
        };

        scope.$watch('data', function (newData) {
          if (newData) {
            scope.render(newData);
          }
        }, true);

        scope.$watch(function () {
          return angular.element($window)[0].innerWidth;
        }, function () {
          scope.resize();
        });

        scope.render = function (data) {
          if (!data) return;
          if (renderTimeout) clearTimeout(renderTimeout);

          renderTimeout = $timeout(function () {
            chartElement.datum(data).call(chart);
          }, 200);
        }

        scope.resize = function () {
          if (renderTimeout) clearTimeout(renderTimeout);
          renderTimeout = $timeout(function () {
            var width = d3.select(element[0]).node().parentNode.offsetWidth;
            chartElement.call(chart.width(width));
          }, 200);
        }

      }
    }]);
})();