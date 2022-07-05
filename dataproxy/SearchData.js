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
      var allDataDeferred = $q.defer();
      var allDataPromise = (function () { return allDataDeferred.promise; })();

      function getData(all) {
        var deferred = $q.defer();

        if (all) {
          allDataPromise.then(function () {
            deferred.resolve(data);
          });
        } else {
          initPromise.then(function () {
            deferred.resolve(data);
          });
        }

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
        if (wwt.searchData) {
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

          var urlbase = wwtlib.URLHelpers.singleton.coreStaticUrl('data/client_v6/');

          importWtml(urlbase + 'Wise.wtml').then(function () {
            importWtml(urlbase + 'Hubble.wtml').then(function () {
              importWtml(urlbase + 'ESO.wtml').then(function () {
                importWtml(urlbase + 'Chandra.wtml').then(function () {
                  importWtml(urlbase + 'Spitzer.wtml').then(function () {
                    allDataDeferred.resolve(true);
                  });
                });
              });
            });
          });

          deferredInit.resolve(data);
        } else {
          setTimeout(init, 333);
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

      function importWtml(wtmlPath) {
        var deferred = $q.defer();

        $.ajax({
          url: wtmlPath + '?v=' + $('body').data('resVersion')
        }).done(function () {
          var wtml = $($.parseXML(arguments[0]));

          wtml.find('Place').each(function (i, place) {
            place = $(place);
            var constellation, ra = parseFloat(place.attr('RA')), dec = parseFloat(place.attr('Dec'));

            if (ra !== 0 || dec !== 0) {
              constellation = wwtlib.Constellations.containment.findConstellationForPoint(ra, dec);

              var fgi = place.find('ImageSet').length ? place.find('ImageSet') : null;

              var wwtPlace = wwtlib.Place.create(
                place.attr('Name'),
                dec,
                ra,
                place.attr('DataSetType'),
                constellation,
                fgi ? util.getImageSetType(fgi.attr('DataSetType')) : 2, //type
                parseFloat(place.find('ZoomLevel')) //zoomfactor
              );

              if (fgi != null) {
                imageset_id++;

                var imgset = wwtlib.Imageset.create(
                  fgi.attr('Name'),
                  fgi.attr('Url'),
                  util.getImageSetType(fgi.attr('DataSetType')),
                  fgi.attr('BandPass'),
                  wwtlib.ProjectionType[fgi.attr('Projection').toLowerCase()],
                  imageset_id, // imagesetid
                  parseInt(fgi.attr('BaseTileLevel')),
                  parseInt(fgi.attr('TileLevels')),
                  null, // tilesize
                  parseFloat(fgi.attr('BaseDegreesPerTile')),
                  fgi.attr('FileType'),
                  fgi.attr('BottomsUp') === 'True',
                  '', // quadTreeTileMap
                  parseFloat(fgi.attr('CenterX')),
                  parseFloat(fgi.attr('CenterY')),
                  parseFloat(fgi.attr('Rotation')),
                  true, // sparse
                  fgi.find('ThumbnailUrl').text(), // thumbnailUrl,
                  false, // defaultSet,
                  false, // elevationModel
                  parseFloat(fgi.attr('WidthFactor')), // widthFactor,
                  parseFloat(fgi.attr('OffsetX')),
                  parseFloat(fgi.attr('OffsetY')),
                  fgi.find('Credits').text(),
                  fgi.find('CreditsUrl').text(),
                  '',
                  '',
                  0, // meanRadius
                  null
                );

                util.rewritePlaceUrls(imgset);
                wwtPlace.set_studyImageset(imgset);
              }

              indexPlaceNames(wwtPlace);

              var cIndex = constellations.indexOf(constellation);
              var constellationPlaces = wwt.searchData.Constellations[cIndex].places;
              wwtPlace.guid = cIndex + '.' + constellationPlaces.length;
              util.rewritePlaceUrls(wwtPlace);
              constellationPlaces.push(wwtPlace);
            }
          });

          deferred.resolve(true);
        });

        return deferred.promise;
      }

      initPromise = init();

      return api;
    }
  ]
);
