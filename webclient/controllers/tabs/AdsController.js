wwt.controllers.controller('ADSController',
    ['$scope',
    'Util',
    '$timeout',
    function ($scope, util, $timeout) {
       
        $scope.adsFilter = 'All';
        
        var years = ["date-pre1800_512", "date-1800_1850_512", "date-1850_1900_512", "date-1900_1910_512",
    "date-1910_1920_512", "date-1920_1930_512", "date-1930_1940_512", "date-1940_1945_512", "date-1945_1950_512",
    "date-1950_1955_512", "date-1955_1960_512", "date-1960_1965_512", "date-1965_1970_512", "date-1970_1975_512",
    "date-1975_1980_512", "date-1980_1985_512", "date-1985_1990_512", "date-1990_512", "date-1991_512",
    "date-1992_512", "date-1993_512", "date-1994_512", "date-1995_512", "date-1996_512", "date-1997_512",
    "date-1998_512", "date-1999_512", "date-2000_512", "date-2001_512", "date-2002_512", "date-2003_512",
    "date-2004_512", "date-2005_512", "date-2006_512", "date-2007_512", "date-2008_512", "date-2009_512",
    "date-2010_512", "date-2011_512", "date-2012_512", "date-2013_512"
        ];
        
        var collections = [{
                label: 'GLIMPSE',
                name: 'GLIMPSE/MIPSGAL'
            }, {
                label:'All',
                name:'allSources_512'
            }, {
                label: 'WISE',
                name: 'WISE All Sky (Infrared)'
            }, {
                label: 'X-ray',
                name: 'X_512'
            }, {
                label: 'Ultraviolet',
                name: 'UV_512'
            }, {
                label: 'Star',
                name: 'Star_512'
            }, {
                label: 'Radio',
                name: 'Radio_512'
            }, {
                label: 'Other',
                name: 'Other_512'
            }, {
                label: 'Nebula',
                name: 'Nebula_512'
            }, {
                label: '',
                name: 'lut_y'
            }, {
                label: '',
                name: 'lut_x'
            }, {
                label: 'Infrared',
                name: 'Infrared_512'
            }, {
                label: 'HII regions',
                name: 'HII-region_512'
            }, {
                label: 'Galaxy',
                name: 'Galaxy_512'
            }
        ];
        $scope.initAds = function () {
            wwt.wc.add_collectionLoaded(defaultLayers);
            wwt.wc.loadImageCollection('adsass.wtml');
            var bar = $('.year-slider a.btn');
            var ys = new wwt.Move({
                el: bar,
                bounds: {
                    x: [-55, 45],
                    y: [0, 0]
                },
                onstart: function () {
                    bar.addClass('moving');
                    setYear();
                },
                onmove: function () {
                    setYear();
                },
                oncomplete: function () {
                    bar.removeClass('moving');
                }
            });
        }
       
        var setYear = function () {
            $timeout(function () {
                $scope.fgImagery = 'year';
                var left = $('.year-slider a.btn').position().left / 100;
                var y = years[Math.max(0, Math.round(left * years.length) - 1)];

                $scope.year = y.split('ate-')[1].split('_512')[0].replace(/_/, '-');
                wwt.wc.setForegroundImageByName(y);
                wwt.wc.setForegroundOpacity($('.cross-fader a.btn').position().left);
            });
        }

        $scope.bgChange = function() {
            $scope.setSurveyBg($scope.bgImagery);
        };
        $scope.adsChange = function () {
            if ($scope.fgImagery === 'year') {
                setYear();
                return;

            }
            $.each(collections, function (i, c) {

                if (c.label === $scope.fgImagery) {
                    wwt.wc.setForegroundImageByName(c.name);
                    wwt.wc.setForegroundOpacity($('.cross-fader a.btn').position().left);
                }
            });
        }

        function defaultLayers() {
            var ra = parseFloat(util.getQSParam('ra') || 0);
            var dec = parseFloat(util.getQSParam('dec') || 0);
            var fov = parseFloat(util.getQSParam('fov') || 60);
            var layer = (util.getQSParam('layer') || 'allSources');
            if (layer.slice(layer.length - 4) !== '_512') {
                layer = layer + '_512';
            }

            wwt.wc.setForegroundImageByName(layer);

            $('#facet-list a').each(function (i, o) {
                if ($(o).attr('href') === layer) {
                    o.click();
                    $('#foreground-lbl').text($(o).text());
                    return;
                }
            });


            wwt.wc.setForegroundOpacity(50);

            wwt.wc.gotoRaDecZoom(ra, dec, fov, true);
            $timeout(function() {
                $scope.fgImagery = 'All';
                $scope.bgImagery = 'WISE All Sky (Infrared)';
                $scope.bgChange();
                
                $scope.setSurveyBg('WISE All Sky (Infrared)');
                
                
            }, 1300);
        }
        
    }
]);