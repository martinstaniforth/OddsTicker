(function () {
  'use strict';

  angular.module('myApp', [
    'myApp.controllers',
    'myApp.directives'
  ]);

  angular.module('d3', []);
  angular.module('myApp.controllers', []);
  angular.module('myApp.directives', ['d3']);

})();