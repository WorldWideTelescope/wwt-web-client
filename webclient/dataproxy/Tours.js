wwt.app.factory('Tours', ['$rootScope', '$http', '$q', '$timeout', 'Util',function ($rootScope, $http, $q, $timeout, util) {
    var api = {
        getRoot: getRoot,
        getChildren: getChildren,
        getTourById: getTourById,
        getToursById:getToursById
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
            tours.push(getTourById(item));
        });
        return guids.length > 1 ? tours : null;
    }

    function getRoot() {
        var deferred = $q.defer();
        initPromise.then(function () {
            rootFolders = root.get_children();
            deferred.resolve(transformData(rootFolders));
            $.each(rootFolders, function (i,folder) {
                var kids = folder.get_children();
                $.each(kids, function (j, tour) {
                    if (tour.get_isTour()) {
                        tour.authorThumb = '//cdn.worldwidetelescope.org/Community/AuthorThumbnail/' + tour.id;
                        tour.thumb = '//cdn.worldwidetelescope.org/Community/TourThumbnail/' + tour.get_thumbnailUrl().split('GUID=')[1];
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

    var transformData = function(items) {
        $.each(items, function (i, item) {
            try {
                item.name = item.get_name();
                item.isFolder = item.get_isFolder();
                if (!item.isFolder && item.name !== 'Up Level') {
                    item.authorThumb = '//cdn.worldwidetelescope.org/Community/AuthorThumbnail/' + item.id;
                    item.thumb = '//cdn.worldwidetelescope.org/Community/TourThumbnail/' + item.get_thumbnailUrl().split('GUID=')[1];
                    //item.tourUrl = '//cdn.worldwidetelescope.org/File/Download/' + item.id + '/' + item.get_name() + '/wtt';
                    item.tourUrl = '//wwtstaging.azurewebsites.net/community/gettour/' + item.id + '/' + item.get_name();
                } else { 
                    item.thumb = item.get_thumbnailUrl();
                }

            } catch (er) {
                util.log(item, er);
            }
        });
        return items;
    };

    var init = function () {
        var deferred = $q.defer();

        root = wwt.wc.createFolder();
        var toursUrl = 'https://wwtweb.blob.core.windows.net/tours/webclienttours.wtml';
        root.loadFromUrl(toursUrl, function () {
            //root.refresh();
            deferred.resolve(root.get_children());
        });
        
        return deferred.promise;
    };

   

    var initPromise = init();
    return api;
}]);


