wwt.controllers.controller('SlideSelectionController', ['$scope', function ($scope) {
    var tourScope = angular.element('#currentTourPanel').scope();
    var overlays, selectionSet, selection;
    var init = function () {
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

    $('canvas').on('dblclick click', init);
    tourScope.$on('initSlides', init);
}]);