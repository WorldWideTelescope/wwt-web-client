module.exports = function (grunt) {
  'use strict';

  // Force use of Unix newlines
  grunt.util.linefeed = '\n';

  RegExp.quote = function (string) {
    return string.replace(/[-\\^$*+?.()|[\]{}]/g, '\\$&');
  };

  // Project configuration.
  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    banner: '/**\n' +
      '* WorldWide Telescope Web Client\n' +
      '* Copyright 2014-2020 .NET Foundation\n' +
      '* Licensed under <%= pkg.license.type %> (<%= pkg.license.url %>)\n' +
      '* Git hash <%= gitinfo.local.branch.current.SHA %>\n' +
      '**/\n',

    // Triger the loading of the Git version info
    gitinfo: {},

    // Task configuration.

    concat: {
      options: {
        banner: '<%= banner %>',

        process: function(src, filepath) {
          if (filepath == 'app.js' || filepath == 'index.html') {
            return grunt.template.process(src);
          } else {
            return src;
          }
        },
      },

      webclient: {
        src: [
          'ext/intro.js',
          'ext/angular-intro.js',
          'app.js',
          'directives/Scroll.js',
          'directives/Localize.js',
          'directives/ContextMenu.js',
          'directives/editslidevalues.js',
          'directives/movable.js',
          'directives/CopyToClipboard.js',
          'factories/AppState.js',
          'factories/autohidepanels.js',
          'factories/Localization.js',
          'factories/FinderScope.js',
          'factories/ThumbList.js',
          'factories/Util.js',
          'factories/UILibrary.js',
          'factories/SearchUtil.js',
          'factories/Skyball.js',
          'factories/HashManager.js',
          'factories/mediafile.js',
          'dataproxy/Places.js',
          'dataproxy/Tours.js',
          'dataproxy/SearchData.js',
          'dataproxy/Astrometry.js',
          'dataproxy/Community.js',
          'controllers/ContextPanelController.js',
          'controllers/MainController.js',
          'controllers/IntroController.js',
          'controllers/MobileNavController.js',
          'controllers/LayerManagerController.js',
          'controllers/tabs/AdsController.js',
          'controllers/tabs/ExploreController.js',
          'controllers/tabs/SearchController.js',
          'controllers/tabs/SettingsController.js',
          'controllers/tabs/ViewController.js',
          'controllers/tabs/ToursController.js',
          'controllers/tabs/CommunityController.js',
          'controllers/tabs/CurrentTourController.js',
          'controllers/modals/TourSlideText.js',
          'controllers/modals/ShareController.js',
          'controllers/modals/OpenItemController.js',
          'controllers/modals/ObservingTimeController.js',
          'controllers/modals/SlideSelectionController.js',
          'controllers/modals/VoConeSearchController.js',
          'controllers/modals/VOTableViewerController.js',
          'controllers/modals/colorpickerController.js',
          'controllers/modals/refFrameController.js',
          'controllers/modals/GreatCircleController.js',
          'controllers/modals/DataVizController.js',
          'controllers/modals/EmbedController.js',
          'controllers/modals/ObservingLocationController.js',
          'misc/move.js',
          'misc/util.js'
        ],
        dest: 'dist/wwtwebclient.js',
        nonull: true,
      }
    },

    uglify: {
      options: {
        preserveComments: 'some',
        banner: '<%= banner %>'
      },
      webclient: {
        src: '<%= concat.webclient.dest %>',
        dest: 'dist/wwtwebclient.min.js'
      },
    },

    less: {
      compileCore: {
        options: {
          strictMath: true,
          sourceMap: true,
          outputSourceFiles: true,
          sourceMapURL: 'webclient.css.map',
          sourceMapFilename: 'dist/css/webclient.css.map'
        },
        src: 'css/webclient.less',
        dest: 'dist/css/webclient.css'
      }
    },

    autoprefixer: {
      options: {
        browsers: [
          "Android 2.3",
          "Android >= 4",
          "Chrome >= 20",
          "Firefox >= 24",
          "Explorer >= 10",
          "iOS >= 6",
          "Opera >= 12",
          "Safari >= 6"
        ]
      },
      core: {
        options: {
          map: true
        },
        src: '<%= less.compileCore.dest %>'
      }
    },

    cssmin: {
      options: {
        compatibility: 'ie10',
        keepSpecialComments: '*',
        noAdvanced: true
      },
      minifyCore: {
        src: '<%= autoprefixer.core.src %>',
        dest: 'dist/css/webclient.min.css'
      }
    },

    template: {
      options: {
        data: function() {
          return {
            shortSHA: grunt.config.get('gitinfo.local.branch.current.shortSHA')
          }
        }
      },

      indexhtml: {
        files: {
          'dist/index.html': 'index.html'
        }
      }
    },

    copy: {
      dist: {
        files: [
          {
            expand: false,
            src: [
              'css/introjs.css',
              'css/angular-motion.css',
              'css/skin.min.css',
              'favicon.ico'
            ],
            dest: 'dist/'
          }, {
            expand: true,
            src: [
              'fonts/*',
              'Images/*',
              'views/**'
            ],
            dest: 'dist/',
            filter: 'isFile'
          }
        ]
      }
    }
  });

  require('load-grunt-tasks')(grunt, {scope: 'devDependencies'});

  grunt.registerTask('dist-js', ['gitinfo', 'concat:webclient', 'uglify:webclient']);
  grunt.registerTask('dist-css', ['less:compileCore', 'autoprefixer:core', 'cssmin:minifyCore']);
  grunt.registerTask('dist-all', ['dist-js', 'dist-css', 'template:indexhtml', 'copy:dist']);
};
