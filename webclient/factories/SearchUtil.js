wwt.app.factory('SearchUtil', [
	'SearchData', '$q', '$timeout','Util',
	function (searchDataService, $q, $timeout,util) {
	
	var api = {
		runSearch: runSearch,
		findNearbyObjects: findNearbyObjects,
		getPlaceById:getPlaceById
	}
	function runSearch(q) {
		var deferred = $q.defer();

		searchDataService.getIndex().then(function (d) {
			var searchData = d;
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
			deferred.resolve(foundPlaces);
		});

		return deferred.promise;
	}
	function getPlaceById(id) {
		var deferred = $q.defer();

		searchDataService.getData().then(function (d) {
			var constellationIndex = parseInt(id.split('.')[0]);
			var placeIndex = parseInt(id.split('.')[1]);
			deferred.resolve(d.Constellations[constellationIndex].places[placeIndex]);
		});

		return deferred.promise;
	}
		
	function findNearbyObjects(args) {
		var deferred = $q.defer();

		searchDataService.getData().then(function(d) {
			var searchData = d;
			if (args.lookAt === 'Sky' || args.lookAt === 'SolarSystem') {
				//if (wwt.wc.getRA() != oldRa || wwt.wc.getDec() != oldDec || wwt.wc.get_fov() != oldZoom) {

				var ulCoords = args.singleton.getCoordinatesForScreenPoint(0, 0);
				var corner = wwtlib.Coordinates.raDecTo3d(ulCoords.x, ulCoords.y);
				var center = wwtlib.Coordinates.raDecTo3d(wwt.wc.getRA(), wwt.wc.getDec());
				var dist = wwtlib.Vector3d.subtractVectors(corner, center);

				var constellation = args.singleton.constellation;
				var constellationPlaces, ssPlaces;
				$.each(searchData.Constellations, function(i, item) {
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
				$.each(searchPlaces, function(i, place) {
					if (place.name != 'Earth') {
						try {
							var placeDist = wwtlib.Vector3d.subtractVectors(place.get_location3d(), center);
							if (dist.length() > placeDist.length()) {
								results.push(place);
							}
						} catch (er) {
						}
					}
				});
				deferred.resolve(results);

			} else {
				deferred.resolve([]);
			}
		});
		return deferred.promise;
	}

	return api;
}]);