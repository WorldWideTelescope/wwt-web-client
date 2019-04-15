wwt.controllers.controller('ObservingLocationController',
  ['$scope', 'AppState', '$http', '$rootScope', '$timeout','UILibrary',function ($scope, appState, $http,$rootScope,$timeout,UILibrary) {
    $scope.pages = ['Select city', 'Choose on globe'];

    var ol = $rootScope.observingLocation
    var pageIndex = ol && ol.type ? $scope.pages.indexOf(ol.type):0;


    $scope.setPage=function(p){
      $scope.page = p;
      if (p === $scope.pages[1]) {
        if ($scope.lookAt !== 'Earth') {
          $scope.setLookAt('Earth', 'Bing Maps Aerial');
          wwt.wc.settings._showCrosshairs = true;
          if (ol.ra !== undefined && ol.dec !== undefined) {
            setTimeout(function () {
              wwt.wc.gotoRaDecZoom(
                parseFloat(ol['ra']) * 15,
                parseFloat(ol['dec']),
                parseFloat(ol['fov']),
                true
              );
            });
          }
        }
        $timeout(viewportChange,500);
      }
    };
    $scope.setPage($scope.pages[pageIndex]);
    var viewportChange = function(){
      if ($scope.page === $scope.pages[1]) {
          var lng = (((180 - (($scope.coords.get_RA()) / 24.0 * 360) - 180) + 540) % 360) - 180;
          var alt = $rootScope.singleton.renderContext.getEarthAltitude($scope.coords.get_lat(),lng, true);
          $scope.altitude=alt.toFixed(1);// + 'm';
        }
    };
    $rootScope.$on('viewportchange', viewportChange);
    $scope.datasets = [{
      name: 'World Cities',
      regions: 'Content/worldregions.txt',
      regiondata: [],
      index: 0
    }, {
      name: 'US Cities',
      regions: 'content/usaregions.txt',
      regiondata: [],
      index: 1
    }, {
      name: 'Canadian Cities',
      regions: 'content/canadaregions.txt',
      regiondata: [],
      index: 2
    }];
    $scope.selectedDatasetIndex = 0;
    $scope.selectedRegionIndex = 0;
    $scope.selectedRegion = {};
    $scope.selectedCity = null;
    $scope.cities = [];
    $scope.cityIndex = 0;
    $scope.selectedDataset = $scope.datasets[0];
    $scope.locationName = ol && ol.name ? ol.name : 'My location';

    $scope.datasetChange = function (index) {
      $scope.selectedDataset = $scope.datasets[index !== undefined ? index : $scope.selectedDatasetIndex];
      loadRegions();
    };
    $scope.regionChange = function (index) {
      $scope.selectedRegionIndex = index !== undefined ? index : $scope.selectedRegionIndex;
      if ($scope.selectedDataset.regiondata.length) {
        $scope.selectedRegion = $scope.selectedDataset.regiondata[$scope.selectedRegionIndex];
        $scope.cityIndex = 0;
        $scope.cities = $scope.selectedRegion.cities.map(function (c, i) {
          if (c.name) {
            return c;
          }
          return {
            name: c[0],
            lat: c[1],
            lng: c[2],
            el: c[3],
            index: i
          };
        });
        $scope.selectedCity = {};
        $scope.cityIndex = 0;
        //console.log({cities: $scope.cities})
      }
    };
    $scope.selectCity = function (index) {
      var c = $scope.cities[index];
      $scope.selectedCity = c;
    };


    var loadRegions = $scope.loadRegions = function (dataset, init) {
      if (!dataset) {
        dataset = $scope.selectedDataset;
      }
      $scope.selectedRegionIndex = 0;
      $scope.selectedRegion = {};
      $scope.selectedCity = {};
      $scope.cityIndex = 0;
      $scope.cities = [];
      var dataLoaded = function (regionData) {
        if (!regionData.length) {
          return;
        }
        var off = init ? 1 : 0;
        dataset.regiondata = regionData.map(function (r, i) {

          r.index = i + off;
          return r;
        });
        if (init) {
          var all = {
            name: 'All Regions',
            index: 0,
            cities: [].concat.apply(this, dataset.regiondata.map(function (r) {
              return r.cities.map(function (c) {
                var clone = JSON.parse(JSON.stringify(c));
                clone[0] += ' (' + r.name + ')';
                return clone;
              })
            }))
          };
          all.cities.splice(0, 1);
          dataset.regiondata.splice(0, 0, all);

        }

        $scope.selectedRegionIndex = 0;
        $scope.regionChange();
      };
      if (dataset.regiondata.length) {
        return dataLoaded(dataset.regiondata);
      }
      $http.get(dataset.regions).success(function (data) {
        //console.log({data: data});
        dataLoaded(data);
      });
    };
    var hadCrosshairsOff = wwt.wc.settings._showCrosshairs == false;

    $scope.setLocation = function(city){
      if(hadCrosshairsOff){
        wwt.wc.settings.set_showCrosshairs = false;
      }
      var lng = (((180 - (($scope.coords.get_RA()) / 24.0 * 360) - 180) + 540) % 360) - 180;
      var lat = $scope.coords.get_lat();
      var altitude = $rootScope.singleton.renderContext.getEarthAltitude($scope.coords.get_lat(), lng, true) + 500;
      console.log($scope.locationName);
      var observing = {
        lat:lat,
        lng:lng,
        altitude:altitude,
        name:$rootScope.observingLocation.name,
        type:$scope.page,
        ra:$scope.coords.get_RA(),
        dec:$scope.coords.get_dec(),
        fov:wwt.wc.get_fov()
      };
     UILibrary.setObservingLocation(observing);
      appState.set('observingLocation',observing);

      if (hadCrosshairsOff){
        wwt.wc.settings._showCrosshairs=false;
      }
      $scope.$hide();
    };
    loadRegions(null,true);
  }
]);
