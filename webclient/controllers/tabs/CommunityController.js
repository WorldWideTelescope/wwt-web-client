wwt.controllers.controller('CommunityController',
    ['$scope',
    'Util',
    '$timeout', 'ThumbList','Community',
    function ($scope, util, $timeout, thumbList, community) {
        var depth = 1;
        var bc;
        var cache = [];
        $scope.initCommunityView = function () {
            thumbList.init($scope, 'communities');
            
                community.getRoot().then(function (result) {
                    $('body').append($('#researchMenu'));
                    $scope.collection = result;

                    var collectionsString = $scope.getFromEn('Collections');
                    if (collectionsString.then) {
                        collectionsString.then(function (s) {
                            $scope.breadCrumb = bc = [s];
                        });
                    } else {
                        $scope.breadCrumb = bc = [collectionsString];
                    } 

                    cache = [result];
                    calcPageSize();

                });
            

        };
        var calcPageSize = function () {
            thumbList.calcPageSize($scope, false);
        };
        $scope.clickThumb = function (item, folderCallback) {
            var outParams = {
                breadCrumb: bc,
                depth: depth,
                cache: cache
            };
            var newParams = thumbList.clickThumb(item, $scope, outParams, folderCallback);
            bc = newParams.breadCrumb;
            cache = newParams.cache;
            depth = newParams.depth;
        };

        $scope.expanded = false;


        $scope.breadCrumbClick = function (index) {
            $scope.collection = cache[index];
            while (bc.length - 1 > index) {
                bc.pop();
                cache.pop();
            }
            $scope.currentPage = 0;
            calcPageSize();
        };



        $(window).on('resize', function () {
            $scope.currentPage = 0;
            calcPageSize();
        });


        $scope.preventClickBubble = function (event) {
            event.stopImmediatePropagation();
        };

        $scope.initCommunityView();
        
    }
]);