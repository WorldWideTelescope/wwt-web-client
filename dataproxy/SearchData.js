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
        // the main webclient app loads asynchronously (see `app.js`).
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

                var band_pass = (fgi.bp !== undefined) ? fgi.bp : wwtlib.BandPass.visible;
                var projection = (fgi.pr !== undefined) ? fgi.pr : wwtlib.ProjectionType.tan;
                var base_tile_level = (fgi.bl !== undefined) ? fgi.bl : 0;
                var file_type = (fgi.ft !== undefined) ? fgi.ft : ".png";
                var tile_levels = (fgi.lv !== undefined) ? fgi.lv : 4;
                var bottoms_up = (fgi.bu !== undefined) ? fgi.bu : false;
                var quad_tree_map = (fgi.q !== undefined) ? fgi.q : "";
                var offset_x = (fgi.oX !== undefined) ? fgi.oX : 0;
                var offset_y = (fgi.oY !== undefined) ? fgi.oY : 0;
                var default_set = (fgi.ds !== undefined) ? fgi.ds : false; // "StockSet" in XML
                var rotation = (fgi.r !== undefined) ? fgi.r : 0;
                var width_factor = (fgi.wf !== undefined) ? fgi.wf : 2;

                imgSet = wwtlib.Imageset.create(
                  fgi.n, // name
                  fgi.u, // url
                  wwtlib.ImageSetType.sky, // data_set_type -- never changes (for now?)
                  band_pass,
                  projection,
                  imageset_id, // imageset id
                  base_tile_level,
                  tile_levels,
                  null, // tile_size
                  fgi.bd, // baseTileDegrees
                  file_type,
                  bottoms_up,
                  quad_tree_map,
                  fgi.cX, // centerX
                  fgi.cY,  // centerY
                  rotation,
                  true, // sparse
                  fgi.tu, // thumbnailUrl,
                  default_set,
                  false, // elevationModel
                  width_factor,
                  offset_x,
                  offset_y,
                  fgi.ct, // creditsText
                  fgi.cu, // creditsUrl
                  '', // demUrl
                  '', // altUrl
                  0, // meanRadius
                  null // referenceFrame
                );

                util.rewritePlaceUrls(imgSet);
              }

              var classification = (place.c !== undefined) ? place.c : wwtlib.Classification.unidentified;
              var zoom_factor = (place.z !== undefined) ? place.z : -1;

              var pl = wwtlib.Place.create(
                place.n, // name
                place.d, // dec
                place.r, // ra
                classification,
                item.name, // constellation
                wwtlib.ImageSetType.sky, // type -- never changes (for now?)
                zoom_factor,
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
