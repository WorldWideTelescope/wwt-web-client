wwt.app.factory(
  'Places',
  [
    '$http',
    '$q',
    '$timeout',
    'Util',

    function ($http, $q, $timeout, util) {
      var api = {
        getRoot: getRoot,
        getSolarSystemPlaces: getSolarSystemPlaces,
        getChildren: getChildren,
        openCollection: openCollection,
        importImage: importImage,
        findChildById: findChildById
      };

      var root,
          rootFolders,
          openCollectionsFolder;

      function rewrite_urls(item) {
        if (item.get_thumbnailUrl) {
          item.thumb = wwtlib.URLHelpers.singleton.rewrite(item.get_thumbnailUrl());
        }

        if (item.get_url) {
          item.url = wwtlib.URLHelpers.singleton.rewrite(item.get_url());
        }
      }

      function getRoot() {
        var deferred = $q.defer();

        initPromise.then(function (folders) {
          rootFolders = folders;
          $.each(folders, function (i, item) {
            item.guid = item.get_name();
            rewrite_urls(item);
          });

          transformData(folders);
          deferred.resolve(root.get_children());
        });

        return deferred.promise;
      }

      function getChildren(obj) {
        var deferred = $q.defer();

        obj.childLoadCallback(function () {
          var children = obj.get_children();

          $.each(children, function (i, item) {
            item.guid = obj.guid + '.' + (item.get_isFolder() ? item.get_name() : i);
            rewrite_urls(item);
          });

          deferred.resolve(transformData(children));
        });

        return deferred.promise;
      }

      function getSolarSystemPlaces() {
        var deferred = $q.defer();
        getRoot().then(function (rf) {
          getChildren(rf[1]).then(function (d) {
            deferred.resolve(d.slice(1));
          });
        });
        return deferred.promise;
      };

      var transformData = function (items) {
        $.each(items, function (i, item) {
          if (typeof item.get_type == 'function') {
            item.isPlanet = item.get_type() === 1;
            //item.isFolder = item.get_type() === 0;
            item.isFGImage = item.get_type() === 2 && typeof item.get_camParams == 'function';
          }

          if (typeof item.get_dataSetType == 'function') {
            item.isEarth = item.get_dataSetType() === 0;
            item.isPanorama = item.get_dataSetType() === 3;
            item.isSurvey = item.get_dataSetType() === 2;
            item.isPlanet = item.get_dataSetType() === 1;
          }

          rewrite_urls(item);
        });

        return items;
      };

      var init = function () {
        var deferred = $q.defer();

        function tryInit() {
          if (!wwt.wc) {
            setTimeout(tryInit, 333);
            return;
          }

          root = wwt.wc.createFolder();

          var url = new URL('assets/webclient-explore-root.wtml', window.location).toString();

          root.loadFromUrl(url, function () {
            var collection;

            if (util.getQSParam('wtml') != null) {
              openCollectionsFolder = wwt.wc.createFolder();
              openCollectionsFolder.set_name('Open Collections');
              collection = wwt.wc.createFolder();
              collection.loadFromUrl(util.getQSParam('wtml'), function () {
                collection.get_children();
                openCollectionsFolder.addChildFolder(collection);
                root.addChildFolder(openCollectionsFolder);
                deferred.resolve(root.get_children());
              });
            } else if (location.href.indexOf('?image=') !== -1) {
              importImage(location.href.split('?image=')[1]).then(function (data) {
                deferred.resolve(root.get_children());
              });
            } else {
              deferred.resolve(root.get_children());
            }
          });
        }

        tryInit();

        return deferred.promise;
      };

      function openCollection(url) {
        var deferred = $q.defer();

        if (!openCollectionsFolder) {
          openCollectionsFolder = wwt.wc.createFolder();
          openCollectionsFolder.set_name('Open Collections');
          openCollectionsFolder.guid = 'f0';
          root.addChildFolder(openCollectionsFolder);
        }

        var collection = wwt.wc.createFolder();

        collection.loadFromUrl(url, function () {
          collection.url = url;
          openCollectionsFolder.addChildFolder(collection);

          if (collection.get_name() == '') {
            deferred.resolve(collection.get_children());
          } else {
            deferred.resolve(collection);
          }
        });

        return deferred.promise;
      }

      function importImage(url, manualData) {
        var deferred = $q.defer();

        if (!openCollectionsFolder) {
          openCollectionsFolder = wwt.wc.createFolder();
          openCollectionsFolder.set_name('Open Collections');
          root.addChildFolder(openCollectionsFolder);
        }

        var collection = wwt.wc.createFolder();
        collection.set_name("Imported image");
        //collection.url = url;

        var encodedUrl = url.indexOf('%2F%2F') != -1 ? url : encodeURIComponent(url);
        if (manualData) {
          encodedUrl += manualData;
        }

        var url = wwtlib.URLHelpers.singleton.coreDynamicUrl('/wwtweb/TileImage.aspx?imageurl=' + encodedUrl);

        collection.loadFromUrl(url, function () {
          if (collection.get_children()[0].get_RA() != 0 || collection.get_children()[0].get_dec() != 0) {
            openCollectionsFolder.addChildFolder(collection);

            getChildren(collection).then(function (children) {
              if (collection.get_name() == '') {
                deferred.resolve(collection.get_children());
              } else {
                deferred.resolve(collection);
              }
            });
          } else {
            deferred.resolve(false);
          }
        });

        return deferred.promise;
      }

      function findChildById(guid, collection) {
        var place = null;
        guid = guid.replace(/_/g, ' ');
        $.each(collection, function (i, item) {
          if (item.guid === guid) {
            place = item;
          }
        });
        return place;
      }

      var initPromise = init();

      return api;
    }
  ]
);
