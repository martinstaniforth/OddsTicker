(function () {
  'use strict';

  var serviceId = 'model';

  angular.module('app').factory(serviceId, model);

  function model() {

    var OddsTracker = function () {
      var self = this;

      self.currentMatches = {};
      self.currentMatchesEnum = [];
      self.historicProbabilities = {};
      self.historicProbabilitiesEnum = [];
      self.historicOdds = {};
      self.historicOddsEnum = [];

      return self;
    }

    OddsTracker.prototype = function () {

      var addSentiment = function (datum) {
        var key = createKey(datum);
        var newSentiment = {};
        newSentiment[key] =
          {
            key: key,
            home: datum.TeamA,
            away: datum.TeamB,
            outcomes:
              [
                { outcome: datum.TeamA, latestProb: datum.HomeWinProbabilityCurrent },
                { outcome: 'Draw', latestProb: datum.DrawProbabilityCurrent },
                { outcome: datum.TeamB, latestProb: datum.AwayWinProbabilityCurrent }
              ]
          };

        var history = this.historicProbabilities;
        if (!history.hasOwnProperty(key)) {
          history[key] = [];
          history[key].push({ outcome: datum.TeamA, historicProbs: [] }, { outcome: 'Draw', historicProbs: [] }, { outcome: datum.TeamB, historicProbs: [] });
        }

        history[key].forEach(function (outcomeSet) {
          var now = new Date();
          if (outcomeSet.outcome === datum.TeamA) {
            outcomeSet.historicProbs.push({ prob: datum.HomeWinProbabilityCurrent, timeStamp: now });
          } else if (outcomeSet.outcome === 'Draw') {
            outcomeSet.historicProbs.push({ prob: datum.DrawProbabilityCurrent, timeStamp: now });
          } else if (outcomeSet.outcome === datum.TeamB) {
            outcomeSet.historicProbs.push({ prob: datum.AwayWinProbabilityCurrent, timeStamp: now });
          }
        });

        $.extend(true, this.currentMatches, newSentiment);
        this.currentMatchesEnum = createMatchEnum(this.currentMatches);

        if (datum.ClosingPrediction === true) {
          var self = this;

          setTimeout(function () {
            self.removeData(key);

          }, 1000);

          setTimeout(function () {
            self.removeMatch(key);
          }, 2500);
        }
      };

      var addOdds = function (datum) {

        var key = createKey(datum);
        var now = new Date();

        var newOdds = {};
        newOdds[key] =
          {
            key: key,
            home: datum.TeamA,
            away: datum.TeamB,
            outcomes:
              [
                { outcome: datum.TeamA }, 
                { outcome: 'Draw' }, 
                { outcome: datum.TeamB }
              ]
          };

        $.extend(true, this.currentMatches, newOdds);

        this.currentMatches[key].outcomes.forEach(function (outcomeSet){
          if(!outcomeSet.hasOwnProperty('latestOdds')){
            outcomeSet.latestOdds = [];
          }
          var hasBookmaker = false;
          var theseOdds = outcomeSet.outcome === datum.TeamA ? datum.HomeWinOdds : 
                                                              (outcomeSet.outcome === 'Draw' ? datum.DrawOdds : 
                                                                                               datum.AwayWinOdds);

          outcomeSet.latestOdds.forEach(function(oddsSet){
            if (oddsSet.bookmaker === datum.Bookmaker){
              oddsSet.odd = theseOdds;
              oddsSet.timeStamp = now;
              hasBookmaker = true;
            }
          });
          if (!hasBookmaker){
            outcomeSet.latestOdds.push({bookmaker: datum.Bookmaker, odd: theseOdds, timeStamp: now});
          }
        });

        var history = this.historicOdds;
        if (!history.hasOwnProperty(key)) {
          history[key] = [];
          history[key].push({ outcome: datum.TeamA, historicOdds: [] }, { outcome: 'Draw', historicOdds: [] }, { outcome: datum.TeamB, historicOdds: [] });
        }

        history[key].forEach(function (outcomeSet) {
          if (outcomeSet.outcome === datum.TeamA) {
            outcomeSet.historicOdds.push({ bookmaker: datum.Bookmaker, odd: datum.HomeWinOdds, timeStamp: now });
          } else if (outcomeSet.outcome === 'Draw') {
            outcomeSet.historicOdds.push({ bookmaker: datum.Bookmaker, odd: datum.DrawOdds, timeStamp: now });
          } else if (outcomeSet.outcome === datum.TeamB) {
            outcomeSet.historicOdds.push({ bookmaker: datum.Bookmaker, odd: datum.AwayWinOdds, timeStamp: now });
          }
        });

        this.currentMatchesEnum = createMatchEnum(this.currentMatches);
      };

      var removeMatch = function (key) {
        if (this.currentMatches.hasOwnProperty(key)) {
          delete this.currentMatches[key];
          delete this.historicProbabilities[key];
          delete this.historicOdds[key];
        }
      };

      var removeData = function (key) {
        if (this.currentMatches.hasOwnProperty(key)) {
          this.currentMatches[key].currentOdds = [];
          this.currentMatches[key].historicOdds = [];
          this.currentMatches[key].historicProbabilities = [];
        }
      };

      var createOddsGrid = function () {
        var lookup = {};
      };

      return {
        addSentiment: addSentiment,
        addOdds: addOdds,
        removeMatch: removeMatch,
        removeData: removeData,
        createOddsGrid: createOddsGrid
      };
      // #region Private methods
      function createKey(datum) {
        return datum.TeamA + ' vs ' + datum.TeamB;
      }

      function createMatchEnum(matches) {
        return $.map(matches, function (value, key) { return value; })
                .sort(function (a, b) { return (a.key > b.key) ? 1 : ((b.key > a.key) ? -1 : 0); });
      }
    
      // #endregion
    }();

    var service = {
      OddsTracker: OddsTracker
    };

    return service;

  }
})();