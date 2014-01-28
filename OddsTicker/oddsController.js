(function () {
  'use strict';

  angular.module('app')
         .controller('oddsController', ['$scope', 'ticker', oddsController]);
  
  function oddsController($scope, ticker) {
    var vm = this;

    vm.activate = activate;
    vm.punterSentimentSplurge = [];
    vm.bookmakerOddSplurge = [];

    activate();
    
    function activate() {
      toastr.options.positionClass = 'toast-bottom-right';
      //onReceivedPunterSentimentUpdate();
      //onReceivedBookmakerPriceUpdate();
      onStartConnection();
    }

    function onStartConnection() {
      $scope.$on('ODDS_CONNECTION_STARTED', function (event, data) {
        toastr.success('Connection started');
      });
    }

    function onReceivedPunterSentimentUpdate() {
      $scope.$on('PUNTER_SENTIMENT_UPDATE_RECEIVED', function (event, data) {
        var messageParts =
          [
            data.TeamA,
            data.TeamB,
            data.HomeWinProbabilityCurrent,
            data.DrawProbabilityCurrent,
            data.AwayWinProbabilityCurrent,
            data.HomeWinProbabilityDelta,
            data.DrawProbabilityDelta,
            data.AwayWinProbabilityDelta
          ];
        var message = _.str.vsprintf('%s vs. %s - %.2f %.2f %.2f (Δ-[%.2f,%.2f,%.2f])', messageParts);
        vm.punterSentimentSplurge.push(message);
        if (vm.punterSentimentSplurge.length >= 20) {
          vm.punterSentimentSplurge.shift();
        }
      });
    }

    function onReceivedBookmakerPriceUpdate() {
      $scope.$on('BOOKMAKER_PRICE_UPDATE_RECEIVED', function (event, data) {
        var messageParts =
          [
            data.TeamA,
            data.TeamB,
            data.Bookmaker,
            data.HomeWinOdds,
            data.DrawOdds,
            data.AwayWinOdds,
            data.Overround
          ];
        var message = _.str.vsprintf('%s vs. %s - %s updated odds to: %.2f %.2f %.2f (%s)', messageParts);
        vm.bookmakerOddSplurge.push(message);
        if (vm.bookmakerOddSplurge.length >= 20) {
          vm.bookmakerOddSplurge.shift();
        }
      });
    }

  }
})();