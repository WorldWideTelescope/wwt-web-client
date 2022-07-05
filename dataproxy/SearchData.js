wwt.app.factory(
  'SearchData',
  [
    '$q',
    'Util',

    function ($q, util) {
      var api = {
        getData: getData,
        getIndex: getIndex
      };

      var data,
        searchIndex = {},
        initPromise,
        constellations = [];
      var deferredInit = $q.defer();

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

      var imageset_id = 100;

      var init = function () {
        // The special `wwt.searchData` global is assigned in the JS file that
        // the main webclient HTML loads asynchronously.
        if (!wwt.searchData) {
          setTimeout(init, 333);
        } else {
          // searchDataIndexed is used in `factories/SearchUtil.js`.
          wwt.searchDataIndexed = [];

          data = wwt.searchData;
          var start = new Date();

          $.each(data.Constellations, function (i, item) {
            constellations[i] = item.name;

            $.each(item.places, function (j, place) {
              var fgi = place.fgi,
                imgSet;

              if (fgi) {
                imageset_id++;

                imgSet = wwtlib.Imageset.create(
                  fgi.n, // name
                  fgi.u, // url
                  fgi.dt || 2, // data_set_type - default to sky
                  fgi.bp, // bandPass
                  fgi.pr, // projection
                  imageset_id, // imageset id
                  fgi.bl, // base_tile_level
                  fgi.lv, // tile_levels
                  null, // tile_size
                  fgi.bd, // baseTileDegrees
                  '', // extension
                  fgi.bu, // bottomsUp
                  fgi.q, // quadTreeTileMap,
                  fgi.cX, // centerX
                  fgi.cY,  // centerY
                  fgi.r, // rotation
                  true, // sparse
                  fgi.tu, // thumbnailUrl,
                  fgi.ds, // defaultSet,
                  false, // elevationModel
                  fgi.wf, // widthFactor,
                  fgi.oX, // offsetX
                  fgi.oY, // offsetY
                  fgi.ct, // creditsText
                  fgi.cu, // creditsUrl
                  '', // demUrl
                  '', // altUrl
                  0, //meanRadius
                  null // referenceFrame
                );

                util.rewritePlaceUrls(imgSet);
              }

              var pl = wwtlib.Place.create(
                place.n, // name
                place.d, // dec
                place.r, // ra
                place.c, // classification
                item.name, // constellation
                fgi ? fgi.dt : 2, // type
                place.z // zoomfactor
              );

              if (imgSet) {
                pl.set_studyImageset(imgSet);
              }

              if (item.name === 'SolarSystem') {
                $.each(pl, function (k, member) {
                  if (wwtlib.ss.canCast(member, wwtlib.CameraParameters)) {
                    member.target = wwtlib.SolarSystemObjects.undefined;
                  }
                });
              }

              pl.guid = i + "." + j;
              util.rewritePlaceUrls(pl);
              item.places[j] = pl;
              indexPlaceNames(pl);
            });
          });

          var end = new Date();
          util.log('parsed places in ' + (end.valueOf() - start.valueOf()) + 'ms', data);
          deferredInit.resolve(data);
        }

        return deferredInit.promise;
      };

      var indexPlaceNames = function (pl) {
        var addPlace = function (s, place) {
          var firstChar = s.charAt(0).toLowerCase();

          if (firstChar === "'")
            firstChar = s.charAt(1).toLowerCase();

          if (searchIndex[firstChar]) {
            if (searchIndex[firstChar][searchIndex[firstChar].length - 1] !== place) {
              searchIndex[firstChar].push(place);
              wwt.searchDataIndexed = searchIndex;
            }
          } else {
            try {
              wwt.searchDataIndexed[firstChar] = searchIndex[firstChar] = [place];
            } catch (er) {
              console.error(er);
            }
          }
        };

        $.each(pl.get_names(), function (n, name) {
          if (name.indexOf(' ') !== -1) {
            var words = name.split(' ');
            $.each(words, function (w, word) {
              addPlace(word, pl);
            });
          } else if (name.charAt(0)) {
            addPlace(name, pl);
          }
        });
      };

      initPromise = init();

      return api;
    }
  ]
);
