(function () {
  'use strict';

  var controllerId = 'graphController';

  angular.module('app').controller(controllerId,
      ['$scope', 'model', 'ticker', graphController]);

  function graphController($scope, model, ticker) {
    var vm = this;
    var oddsTracker = new model.OddsTracker();

    vm.activate = activate;
    vm.title = 'graphController';
    vm.currentMatches = oddsTracker.currentMatches;

    activate();
    
    function activate() {
      toastr.options.positionClass = 'toast-bottom-right';
      onReceivedPunterSentimentUpdate();
      onReceivedBookmakerPriceUpdate();
      onStartConnection();
    }

    //#region Internal Methods        
    function onReceivedPunterSentimentUpdate() {
      $scope.$on('PUNTER_SENTIMENT_UPDATE_RECEIVED', function (event, data) {
        oddsTracker.addSentiment(data);
      });
    }

    function onReceivedBookmakerPriceUpdate() {
      $scope.$on('BOOKMAKER_PRICE_UPDATE_RECEIVED', function (event, data) {
        oddsTracker.addOdds(data);
      });
    }

    function onStartConnection() {
      $scope.$on('ODDS_CONNECTION_STARTED', function (event, data) {
        toastr.success('Connection started');
      });
    }
    //#endregion
  }
})();
