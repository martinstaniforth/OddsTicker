(function () {
    'use strict';

    // Module name is handy for logging
    var id = 'd3';

    // Create the module and define its dependencies.
    var d3Module = angular.module(id);
    
    d3Module.factory('d3Service', ['$document', '$window', '$q', '$rootScope', d3]);

    function d3($document, $window, $q, $rootScope) {
      var d = $q.defer();
      var service = {
          prime: prime
        };
      return service;

      function prime() {

        function onScriptLoad() {
          $rootScope.$apply(function () { d.resolve($window.d3); });
        }

        var scriptTag = $document[0].createElement('script');
        scriptTag.type = 'text/javascript';
        scriptTag.async = true;
        scriptTag.src = 'http://d3js.org/d3.v3.js';
        scriptTag.onreadystatemachine = function () {
          if (this.readyState == 'complete') onScriptLoad();
        };
        scriptTag.onload = onScriptLoad;

        var s = $document[0].getElementsByTagName('body')[0];
        s.appendChild(scriptTag);

        return d.promise;
      };

    }

})();