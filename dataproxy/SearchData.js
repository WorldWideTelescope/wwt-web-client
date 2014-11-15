wwt.app.factory('SearchData', [
	'$http', '$q', '$timeout', 'Places','Util', function($http, $q, $timeout, places, util) {
		var api = {
			getData: getData,
			getIndex:getIndex
		};
		var data, searchIndex = {}, initPromise,ssData;

		function getData() {
			var deferred = $q.defer();
			initPromise.then(function () {
				deferred.resolve(data);
			});
			return deferred.promise;
		};
		function getIndex() {
			var deferred = $q.defer();
			initPromise.then(function () {
				deferred.resolve(searchIndex);
			});
			return deferred.promise;
		};

	var deferredInit = $q.defer();
	var init = function () {
		if (wwt.searchData) {
			data = wwt.searchData;
			var start = new Date();
			var isId = 100;
			$.each(data.Constellations, function (i, item) {
				/*if (item.name === 'SolarSystem') {
					item.places = ssData;
					return;
				}*/
				$.each(item.places, function(j, place) {
					var fgi = place.fgi,
						imgSet;
					if (fgi) {
						isId++;
						fgi.projection = fgi.projection.charAt(0).toLowerCase() + fgi.projection.substr(1);
						imgSet = wwtlib.Imageset.create(fgi.name,//name
							fgi.url,//url
							fgi.dataSetType || 2,//datasettype -default to sky
							fgi.bandPass,
							wwtlib.ProjectionType[fgi.projection],
							isId,//imagesetid
							parseFloat(fgi.baseLevel),
							parseFloat(fgi.levels),
							null,//tilesize
							parseFloat(fgi.baseTileDegrees),
							'',//extension
							fgi.bottomsUp === 'true',
							fgi.quadTreeTileMap,
							parseFloat(fgi.centerX),
							parseFloat(fgi.centerY),
							parseFloat(fgi.rotation),
							true,//sparse
							fgi.thumbnailUrl,
							fgi.defaultSet==='true',
							false,//elevationModel
							parseFloat(fgi.widthFactor),
							parseFloat(fgi.offsetX),
							parseFloat(fgi.offsetY),
							fgi.creditsText,
							fgi.creditsUrl,'','',fgi.meanRadius||0,null);
					}
					var pl = wwtlib.Place.create(place.n,
						place.d,//dec
						place.r,//ra
						place.c,//classification
						item.name,//constellation
						fgi ? fgi.dataSetType : 2,//type
						place.z//zoomfactor
						);
					if (imgSet) {
						pl.set_studyImageset(imgSet);
					}

					if (item.name === 'SolarSystem') {
						$.each(pl, function(k, member) {
							if (Type.canCast(member, wwtlib.CameraParameters)) {
								member.target = wwtlib.SolarSystemObjects.undefined;
							}
						});
					}
					if (fgi && fgi.creditsUrl) {
						util.log(pl);
					}
					pl.guid = i + "." + j;
					//re-place js data with place obj
					item.places[j] = pl;

					var addPlace = function(s, place) {
						var firstChar = s.charAt(0).toLowerCase();
						if (firstChar === "'") firstChar = s.charAt(1).toLowerCase();
						if (searchIndex[firstChar]) {
							if (searchIndex[firstChar][searchIndex[firstChar].length - 1] != place) {
								searchIndex[firstChar].push(place);
							}
						} else {
							searchIndex[firstChar] = [place];
						}
					}

					$.each(pl.get_names(), function (n, name) {
						if (name.indexOf(' ') !== -1) {
							var words = name.split(' ');
							$.each(words, function(w, word) {
								addPlace(word, pl);
							});
						}
						else if (name.charAt(0)) {
							addPlace(name, pl);
						}
					});
					
				});
			});
			var end = new Date();
			util.log('parsed places in ' + (end.valueOf() - start.valueOf()) + 'ms', data);

			deferredInit.resolve(data);
			
		} else {
			setTimeout(init, 333);
		}
		return deferredInit.promise;
	};
	/*places.getSolarSystemPlaces().then(function (d) {
		ssData = d;
	});*/
	initPromise = init();
	return api;
}]);