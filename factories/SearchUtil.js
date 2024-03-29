wwt.app.factory(
  'SearchUtil',
  [
    'SearchData',
    '$q',
    'Util',
    '$rootScope',

    function (searchDataService, $q, util, $rootScope) {
      var api = {
        runSearch: runSearch,
        findNearbyObjects: findNearbyObjects,
        getPlaceById: getPlaceById
      }

      function runSearch(q) {
        var deferred = $q.defer();

        searchDataService.getIndex().then(function (d) {
          var searchData = wwt.searchDataIndexed;
          var foundPlaces = [];

          if (q.length < 2) {
            foundPlaces = searchData[q];
          } else {
            var subset = searchData[q.charAt(0).toLowerCase()];

            $.each(subset, function (i, place) {
              var names = place.get_names();
              var placeChosen = false;

              $.each(names, function (j, name) {
                if (q.indexOf(' ') === -1 && name.split(' ').length > 1) {
                  var words = name.split(' ');

                  $.each(words, function (k, word) {
                    if (word.toLowerCase().indexOf(q.toLowerCase()) === 0 && !placeChosen) {
                      foundPlaces.push(place);
                      placeChosen = true;
                    }
                  });
                } else if (name.toLowerCase().indexOf(q.toLowerCase()) === 0 && !placeChosen) {
                  foundPlaces.push(place);
                  placeChosen = true;
                }
              });
            });
          }

          deferred.resolve(foundPlaces.sort(sortByImagery));
        });

        return deferred.promise;
      }

      var sortByImagery = function (p1, p2) {
        return p1.fromCenter - p2.fromCenter;
      }

      // This interface has serious issues -- the mechanism for constructing
      // place IDs is based on indices into the search data, which are *not*
      // things that ought to be required to stay stable over time. A different
      // approach should be used.
      function getPlaceById(id) {
        var deferred = $q.defer();

        searchDataService.getData().then(function (d) {
          var constellationIndex = parseInt(id.split('.')[0]);
          var placeIndex = parseInt(id.split('.')[1]);
          var p = d.Constellations[constellationIndex].places[placeIndex];
          deferred.resolve(p || null);
        });

        return deferred.promise;
      }

      function findNearbyObjects(args) {
        var deferred = $q.defer();

        searchDataService.getData().then(function (d) {
          var searchData = wwt.searchData;

          if ($rootScope.viewport && (args.lookAt === 'Sky' || args.lookAt === 'SolarSystem')) {
            var ulCoords = args.singleton.getCoordinatesForScreenPoint(0, 0);
            var corner = wwtlib.Coordinates.raDecTo3d(ulCoords.x, ulCoords.y);
            var center = wwtlib.Coordinates.raDecTo3d($rootScope.viewport.RA, $rootScope.viewport.Dec);
            var dist = wwtlib.Vector3d.subtractVectors(corner, center).length();
            var constellation = args.singleton.constellation;
            var constellationPlaces, ssPlaces;

            $.each(searchData.Constellations, function (i, item) {
              if (item.name === constellation) {
                constellationPlaces = item.places;
              } else if (item.name === 'SolarSystem') {
                ssPlaces = item.places;
              }
            });

            if (args.lookAt === 'SolarSystem') {
              deferred.resolve(ssPlaces);
            }

            var searchPlaces = ssPlaces.concat(constellationPlaces);
            var results = [];
            var solsys = [];
            var imgsets = [];

            $.each(searchPlaces, function (i, place) {
              if (place && place.get_name() !== 'Earth') {
                try {
                  var placeDist = wwtlib.Vector3d.subtractVectors(place.get_location3d(), center);

                  if (dist > placeDist.length()) {
                    place.fromCenter = placeDist.length();

                    if (place.get_constellation() === 'SolarSystem') {
                      solsys.push(place)
                    } else if (place.get_studyImageset()) {
                      imgsets.push(place);
                    } else {
                      results.push(place);
                    }
                  }
                } catch (er) {
                  util.log(er, place);
                }
              }
            });

            solsys = solsys.sort(sortByImagery);
            deferred.resolve(solsys.concat(imgsets.sort(sortByImagery), results.sort(sortByImagery)));
          } else {
            deferred.resolve([]);
          }
        });

        return deferred.promise;
      }

      return api;
    }
  ]
);
