wwt.app.factory('Places', ['$http', '$q', '$timeout', 'Util',
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

    function getRoot() {
      var deferred = $q.defer();
      initPromise.then(function (folders) {
        rootFolders = folders;
        $.each(folders, function (i, item) {
          item.guid = item.get_name();
          if (item.get_thumbnailUrl) {
            fixThumb(item);
          }
        });
        transformData(folders);
        deferred.resolve(root.get_children());
      });
      return deferred.promise;
    }

    var cleanseUrl = function (fieldName, item) {
      if (item[fieldName])
        item[fieldName] = item[fieldName].replace("www.worldwidetelescope.org", "worldwidetelescope.org").replace("http://", "//");
    }
    var fixThumb = function (item) {
      item.thumb = item.get_thumbnailUrl().replace("wwtstaging.azurewebsites.net/Content/Images/", "wwtweb.blob.core.windows.net/images/")
        .replace("worldwidetelescope.org/Content/Images/", "wwtweb.blob.core.windows.net/images/")
        .replace("worldwidetelescope.org/Content/Images/", "wwtweb.blob.core.windows.net/images/")
        .replace("cdn.worldwidetelescope.org/wwtweb", "worldwidetelescope.org/wwtweb");
      Object.keys(item).forEach(function (key) {
        var lk = key.toLowerCase();
        if (lk.indexOf('url') > -1 || lk.indexOf('thumb') > -1) {
          cleanseUrl(key, item);
        }
      });
    }

    function getChildren(obj) {
      var deferred = $q.defer();

      obj.childLoadCallback(function () {
        var children = obj.get_children();
        $.each(children, function (i, item) {
          item.guid = obj.guid + '.' + (item.get_isFolder() ? item.get_name() : i);
          if (item.get_thumbnailUrl) {
            fixThumb(item);
          }
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
      //console.warn({items:items});
      $.each(items, function (i, item) {
        try {
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
          Object.keys(item).forEach(function (key) {
            var lk = key.toLowerCase();
            if (lk.indexOf('url') > -1 || lk.indexOf('thumb') > -1) {
              cleanseUrl(key, item);
            }
          });
        } catch (er) {
          util.log(item, er);
        }
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

        root.loadFromUrl('//worldwidetelescope.org/wwtweb/catalog.aspx?W=WCExploreRoot', function () {
          var collection;
          if (util.getQSParam('wtml') != null) {
            openCollectionsFolder = wwt.wc.createFolder();
            openCollectionsFolder.set_name('Open Collections');
            collection = wwt.wc.createFolder();
            collection.loadFromUrl(util.getQSParam('wtml'), function () {
              collection.get_children();
              openCollectionsFolder.addChildFolder(collection);
              root.addChildFolder(openCollectionsFolder);
              //addVampFeeds();
              deferred.resolve(root.get_children());
            });
          } else if (location.href.indexOf('?image=') !== -1) {
            //addVampFeeds();
            importImage(location.href.split('?image=')[1]).then(function (data) {
              deferred.resolve(root.get_children());
            });

          } else {
            //addVampFeeds();
            deferred.resolve(root.get_children());
          }
        });
      }

      tryInit();

      return deferred.promise;
    };

    function openCollection(url) {
      url = url.replace("www.worldwidetelescope.org", "worldwidetelescope.org");//.replace("http://", "//");
      var deferred = $q.defer();
      if (!openCollectionsFolder) {
        openCollectionsFolder = wwt.wc.createFolder();
        openCollectionsFolder.set_name('Open Collections');
        openCollectionsFolder.guid = 'f0';
        root.addChildFolder(openCollectionsFolder);
      }
      var collection = wwt.wc.createFolder();
      collection.loadFromUrl(url, function () {
        //collection.get_children();
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

    function addVampFeeds() {
      var vampFolder = wwt.wc.createFolder();
      vampFolder.set_name('New Imagery');
      vampFolder.guid = '0v0';
      vampFolder.set_url('//worldwidetelescope.org/wwtweb/catalog.aspx?W=vampfeeds');
      root.addChildFolder(vampFolder);

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
      collection.loadFromUrl('//worldwidetelescope.org/WWTWeb/TileImage.aspx?imageurl=' + encodedUrl, function () {
        //collection.get_children();
        //collection.url = url;
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
      /*wwt.wc.add_collectionLoaded(function(data) {
        console.log(data);
        $.each(data, function(k, fld) {
          if (ss.canCast(fld, wwtlib.Folder)) {
            $.each(fld, function(ky, wf) {
              if (ss.canCast(wf, wwtlib.WebFile)) {
                console.log(wf);
              }
            });
          }
        });
        deferred.resolve(collection);
      });
      wwt.wc.loadImageCollection('//worldwidetelescope.org/WWTWeb/TileImage.aspx?imageurl=' + encodeURIComponent(url));*/
      //});
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
  }]);


/*

folder
get_type = 0
get_subType = null
get_dataSetType = undefined

panorama
get_type = undefined
get_subType = undefined
get_dataSetType = 3
get_camParams = undefined

image (e.g. hubble galaxy)
get_type = 2
get_subType = undefined
get_dataSetType = undefined
get_camParams = function

survey
get_type = undefined
get_subType = undefined
get_dataSetType = 2
get_camParams = undefined

earth
get_type = undefined
get_subType = undefined
get_dataSetType = 0
get_camParams = undefined

planet
get_type = 1
get_subType = undefined
get_dataSetType = undefined

--or--

get_type = undefined
get_subType = undefined
get_dataSetType = 1

*/
