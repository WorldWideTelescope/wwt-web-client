/// <binding ProjectOpened='watch' />
/**!
 Gruntfile to perform wwt webclient less compilation,
 script concatenation/minification, and component updates
 (using grunt bower:install)

 Once you have run npm install and npm update
 and bower is installed (npm install -g bower)
 run grunt watch

 **/

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
    '* Copyright 2014-2015 WorldWide Telescope\n' +
    '* Licensed under <%= pkg.license.type %> (<%= pkg.license.url %>)\n' +
    '**/\n',

    // Task configuration.

    concat: {
      options: {
        banner: '<%= banner %>'
      },
      webclient: {
        src: [
          'ext/intro.js',
          'ext/angular-intro.js',
          'app.js',
          'directives/Scroll.js',
          'directives/Localize.js',
          'directives/ContextMenu.js',
          'directives/EditSlideValues.js',
          'directives/Movable.js',
          'directives/CopyToClipboard.js',
          'factories/appstate.js',
          'factories/autohidepanels.js',
          'factories/localization.js',
          'factories/FinderScope.js',
          'factories/ThumbList.js',
          'factories/Util.js',
          'factories/UILibrary.js',
          'factories/SearchUtil.js',
          'factories/Skyball.js',
          'factories/HashManager.js',
          'factories/MediaFile.js',
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
          'controllers/modals/refframeController.js',
          'controllers/modals/GreatCircleController.js',
          'controllers/modals/DataVizController.js',
          'controllers/modals/EmbedController.js',
          'controllers/modals/ObservingLocationController.js',
          'controllers/LoginController.js',
          'controls/move.js',
          'controls/util.js'
        ],
        dest: 'wwtwebclient.js'
      },
      sdk: {
        src: [
          'sdk/ss.js',
          'sdk/wwtlib.js'
        ],
        dest: 'sdk/wwtsdk.js'
      },
      oldsdk: {
        src: [
          'sdk/old/mscorlib.debug.js',
          'sdk/old/wwtlib.debug.js'
        ],
        dest: 'sdk/old/wwtlib_full.js'
      }
    },
    //remove the AMD dependancy from scriptsharp output
    replace: {
      wwtlib: {
        src: ['sdk/wwtlib.js'],
        dest: 'sdk/wwtlib.js',
        replacements: [
          {
            from: "define('wwtlib', ['ss'], function(ss) {",
            to: 'window.wwtlib = function(){'
          }, {
            from: 'return $exports;\n});',
            to: 'return $exports;\n}();'
          }
        ]
      }
    },

    uglify: {
      options: {
        preserveComments: 'some',
        banner: '<%= banner %>'
      },
      webclient: {
        src: '<%= concat.webclient.dest %>',
        dest: 'wwtwebclient.min.js'
      },
      searchData: {
        src: 'searchdataraw.js',
        dest: 'searchdata.min.js'
      },
      sdk: {
        src: '<%= concat.sdk.dest %>',
        dest: 'sdk/wwtsdk.min.js'
      },
      oldsdk: {
        src: '<%= concat.oldsdk.dest %>',
        dest: 'sdk/old/wwtlib_full.min.js'
      }
    },
    less: {
      compileCore: {
        options: {
          strictMath: true,
          sourceMap: true,
          outputSourceFiles: true,
          sourceMapURL: 'webclient.css.map',
          sourceMapFilename: 'css/webclient.css.map'
        },
        src: 'css/webclient.less',
        dest: 'css/webclient.css'
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
        src: 'css/webclient.css'
      }
    },
    cssmin: {
      options: {
        compatibility: 'ie10',
        keepSpecialComments: '*',
        noAdvanced: true
      },
      minifyCore: {
        src: 'css/webclient.css',
        dest: 'css/webclient.min.css'
      }
    },
    csscomb: {
      options: {
        config: 'bootstrap/less/.csscomb.json'
      },
      dist: {
        expand: true,
        cwd: 'css/',
        src: ['*.css', '!*.min.css'],
        dest: 'css/'
      }
    },

    watch: {
      sdk: {
        files: 'sdk/wwtlib.js',
        tasks: ['sdk']
      },

      // call out only the directories to watch prevents
      // watch from watching recursive node_modules folders e.g.: '../**/*.js'
      scripts: {
        files: [
          'controllers/**/*.js',
          'controllers/*.js',
          'controls/*.js',
          'directives/*.js',
          'dataproxy/*.js',
          'factories/*.js',
          'app.js'],
        tasks: ['dist-js']
      },


      less: {
        files: 'css/*.less',
        tasks: ['dist-css']
      }
    }
  });


  // Dependencies
  require('load-grunt-tasks')(grunt, {scope: 'devDependencies'});

  // JS concatenation and minification
  grunt.registerTask('dist-js', ['concat:webclient', 'uglify:webclient']);

  // Takes HTML5SDK generated script and packages into single usable lib. (scriptsharp v0.8).
  grunt.registerTask('sdk', ['replace:wwtlib', 'concat:sdk', 'uglify:sdk', 'concat:webclient', 'uglify:webclient']);

  // Minify the generated search data (rare - internal only)
  grunt.registerTask('dist-searchdata', ['uglify:searchData']);

  // CSS  (csscomb seems like too much, so commented out for now)
  grunt.registerTask('dist-css', ['less:compileCore', 'autoprefixer:core', /*'csscomb:dist',*/'cssmin:minifyCore']);

};
