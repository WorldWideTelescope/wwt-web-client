wwt.app.factory(
  'Tours',
  [
    '$rootScope',
    '$http',
    '$q',
    '$timeout',
    'Util',

    function ($rootScope, $http, $q, $timeout, util) {
      var api = {
        getRoot: getRoot,
        getChildren: getChildren,
        getTourById: getTourById,
        getToursById: getToursById
      };

      var root;
      var rootFolders;
      var tourHash = {};

      function getTourById(id) {
        return tourHash[id.toLowerCase()];
      }

      function getToursById(guids) {
        if (!guids) return null;
        var tours = [];
        $.each(guids.split(';'), function (i, item) {
          var tour = getTourById(item);
          if (tour)
            tours.push(tour);
        });
        return guids.length > 1 ? tours : null;
      }

      function getRoot() {
        var deferred = $q.defer();
        initPromise.then(function () {
          rootFolders = root.get_children();
          deferred.resolve(transformData(rootFolders));
          $.each(rootFolders, function (i, folder) {
            var kids = folder.get_children();
            $.each(kids, function (j, tour) {
              if (tour.get_isTour()) {
                tourHash[tour.id.toLowerCase()] = tour;
              }
            });
          });
        });
        return deferred.promise;
      }

      function getChildren(obj) {
        var deferred = $q.defer();

        obj.childLoadCallback(function () {
          deferred.resolve(transformData(obj.get_children()));


        });

        return deferred.promise;
      }

      var transformData = function (items) {
        $.each(items, function (i, item) {
          item.name = item.get_name();
          item.isFolder = item.get_isFolder();
          util.rewritePlaceUrls(item);
        });

        return items;
      };

      var init = function () {
        var deferred = $q.defer();

        root = wwt.wc.createFolder();
        var toursUrl = 'gettours_webclient.xml';
        root.loadFromUrl(toursUrl, function () {
          //root.refresh();
          deferred.resolve(root.get_children());
        });

        return deferred.promise;
      };

      var initPromise = init();
      return api;
    }
  ]
);
