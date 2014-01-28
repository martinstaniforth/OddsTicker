(function () {
  'use strict';

  angular.module('app')
         .factory('ticker', ['$rootScope', ticker]);

  function ticker($rootScope) {
    var oddsHub = $.connection.oddsHub;

    var service = {
      //not yet sending anything back to the server
    };

    init();

    return service;

    function init() {
      startConnection();
      onReceivedPunterSentimentUpdate();
      onReceivedBookmakerPriceUpdate();
    }

    function startConnection() {
      $.connection.hub.start().done(function () {
        $rootScope.$broadcast('ODDS_CONNECTION_STARTED', { data: 'Started' });
      });
    }

    function onReceivedPunterSentimentUpdate() {
      oddsHub.client.broadcastPunterSentimentUpdate = function (sentimentDeltas) {
        $rootScope.$broadcast('PUNTER_SENTIMENT_UPDATE_RECEIVED', sentimentDeltas);
        $rootScope.$apply();
      };
    }

    function onReceivedBookmakerPriceUpdate() {
      oddsHub.client.broadcastBookmakerPriceUpdate = function (bookmakerOdd) {
        $rootScope.$broadcast('BOOKMAKER_PRICE_UPDATE_RECEIVED', bookmakerOdd);
        $rootScope.$apply();
      };
    }

  }

})();