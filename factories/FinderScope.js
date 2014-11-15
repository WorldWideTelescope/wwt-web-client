wwt.app.factory('FinderScope',
    ['SearchData',
    '$timeout','Util',
    function (searchDataService, $timeout, util) {
        var api = {
            init: init,
            scopeMove:scopeMove
        };
        var searchData;
        function init() {
            searchDataService.getData().then(function(d) {
                searchData = d;
                scopeMove();
                //console.log(scopeMove());
            });
        };

        function scopeMove() {
            if (!searchData) {
                return false;
            }
            var pos = $('.finder-scope').position();
            var offsetX = 301;
            var offsetY = 87;
            
            var scopeCoords = wwtlib.WWTControl.singleton.getCoordinatesForScreenPoint(pos.left + offsetX, pos.top + offsetY);
            scopeCoords.x = (scopeCoords.x + 720) % 360;
            var scope = wwtlib.Coordinates.raDecTo3d(scopeCoords.x, scopeCoords.y);
            
            var constellation = wwtlib.Constellations.containment.findConstellationForPoint(scopeCoords.x, scopeCoords.y);
            var closestDist, closestPlace;
            var constellationPlaces,ssPlaces;
            $.each(searchData.Constellations, function (i, item) {
                if (item.name === constellation) {
                    constellationPlaces = item.places;
                } else if (item.name === 'SolarSystem') {
                    ssPlaces = item.places;
                }
            });
            var searchPlaces = ssPlaces.concat(constellationPlaces);
            $.each(searchPlaces, function (i, place) {
                try {
                    var placeDist = wwtlib.Vector3d.subtractVectors(place.get_location3d(), scope);
                    if ((i === 0) || closestDist.length() > placeDist.length()) {
                        closestPlace = place;
                        closestDist = placeDist;
                    }
                    
                    
                } catch (er) {
                    if (place && place.get_name()!='Earth')
                    util.log(er);
                }
            });

            util.getAstroDetails(closestPlace);
            /*if (details.bValid)
            {
                riseText.Text = UiTools.FormatDecimalHours(details.Rise);
                transitText.Text = UiTools.FormatDecimalHours(details.Transit);
                setText.Text = UiTools.FormatDecimalHours(details.Set);
            }*/
            return closestPlace;
        }

        
        return api;
    }
    ]);