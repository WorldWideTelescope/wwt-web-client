wwt.app.factory('Tours', ['$rootScope', '$http', '$q', '$timeout', 'Util', function ($rootScope, $http, $q, $timeout, util) {
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
            try {
                item.name = item.get_name();
                item.thumb = item.get_thumbnailUrl();
                //console.log(item.thumb);
                item.isFolder = item.get_isFolder();
                /*item.isImage = item.get_isImage();
                if (typeof item.get_type == 'function') {
                    item.isPlanet = item.get_type() === 1;
                    item.isFolder = item.get_type() === 0;
                    item.isFGImage = item.get_type() === 2 && typeof item.get_camParams == 'function';
                }
                if (typeof item.get_dataSetType == 'function') {
                    item.isEarth = item.get_dataSetType() === 0;
                    item.isPanorama = item.get_dataSetType() === 3;
                    item.isSurvey = item.get_dataSetType() === 2;
                }

                */

            } catch (er) {
                util.log(item, er);
            }
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
}]);


