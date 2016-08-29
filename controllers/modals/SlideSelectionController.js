wwt.controllers.controller('SlideSelectionController', ['$scope', '$timeout', function ($scope,$timeout) {
    var tourScope = angular.element('#currentTourPanel').scope();
    var overlays, selectionSet, selection;
    var init = $scope.init = function () {
        selection = tourScope.tourEdit.tourEditorUI.selection;
        selectionSet = $scope.selectionSet = selection.selectionSet;
        
        overlays = $scope.overlays = tourScope.selectedSlide._overlays;

        $scope.$applyAsync(function () {
            overlays.forEach(function (overlay, j) {
                overlay.selected = selection.isOverlaySelected(overlay);
            });
        });
    }

    $scope.selectionChange = function (overlay) {
        selection.clearSelection();
        var range = [];
        overlays.forEach(function (overlay, i) {
            if (overlay.selected) {
                range.push(overlay);
            }
        });
        selection.addSelectionRange(range);

    }

    var rebind = function () {
        init.apply($scope, []);
    }

    $('canvas').on('dblclick click keyup', rebind);
    tourScope.$on('initSlides', rebind);
    $timeout(rebind, 100);
}]);