// Copyright 2020 the .NET Foundation
// Licensed under the MIT License

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
      '* AAS WorldWide Telescope Web Client\n' +
      '* Copyright 2014-2020 .NET Foundation\n' +
      '* Licensed under the MIT License\n' +
      '* Git hash <%= gitinfo.local.branch.current.SHA %>\n' +
      '**/\n',

    // Triger the loading of the Git version info
    gitinfo: {},

    // Different build profiles. The "localtest" file is in .gitignore and is
    // intended to be used for temporary local testing. The other profiles are
    // in Git for reproducibility.
    profile: {
      dev: 'profile-dev.yml',
      prod: 'profile-prod.yml',
      localtest: 'profile-localtest.yml'
    },

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

          'directives/ContextMenu.js',
          'directives/CopyToClipboard.js',
          'directives/Localize.js',
          'directives/Scroll.js',
          'directives/editslidevalues.js',
          'directives/movable.js',

          'factories/AppState.js',
          'factories/FinderScope.js',
          'factories/HashManager.js',
          'factories/Localization.js',
          'factories/SearchUtil.js',
          'factories/Skyball.js',
          'factories/ThumbList.js',
          'factories/UILibrary.js',
          'factories/Util.js',
          'factories/autohidepanels.js',
          'factories/mediafile.js',

          'dataproxy/Astrometry.js',
          'dataproxy/Community.js',
          'dataproxy/Places.js',
          'dataproxy/SearchData.js',
          'dataproxy/Tours.js',

          'controllers/ContextPanelController.js',
          'controllers/IntroController.js',
          'controllers/LayerManagerController.js',
          'controllers/LoginController.js',
          'controllers/MainController.js',
          'controllers/MobileNavController.js',
          'controllers/modals/DataVizController.js',
          'controllers/modals/EmbedController.js',
          'controllers/modals/GreatCircleController.js',
          'controllers/modals/ObservingLocationController.js',
          'controllers/modals/ObservingTimeController.js',
          'controllers/modals/OpenItemController.js',
          'controllers/modals/ShareController.js',
          'controllers/modals/SlideSelectionController.js',
          'controllers/modals/TourSlideText.js',
          'controllers/modals/VOTableViewerController.js',
          'controllers/modals/VoConeSearchController.js',
          'controllers/modals/colorpickerController.js',
          'controllers/modals/refFrameController.js',
          'controllers/tabs/CommunityController.js',
          'controllers/tabs/CurrentTourController.js',
          'controllers/tabs/ExploreController.js',
          'controllers/tabs/SearchController.js',
          'controllers/tabs/SettingsController.js',
          'controllers/tabs/ToursController.js',
          'controllers/tabs/ViewController.js',

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
          var d = require('lodash').clone(grunt.config.get('profile_data'));
          d.shortSHA = grunt.config.get('gitinfo.local.branch.current.shortSHA');
          return d;
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
              'assets/*',
              'css/introjs.css',
              'css/angular-motion.css',
              'css/mcecontent.css',
              'css/skin.min.css',
              'default.aspx',
              'favicon.ico'
            ],
            dest: 'dist/'
          }, {
            expand: true,
            src: [
              'fonts/*',
              'images/*',
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

  grunt.registerMultiTask('profile', 'Specify the build profile', function() {
    var data = grunt.file.readYAML(this.data);
    grunt.config.set('profile_data', data);
  });

  var all_tasks = [
    'gitinfo',
    'concat:webclient',
    'uglify:webclient',
    'less:compileCore',
    'autoprefixer:core',
    'cssmin:minifyCore',
    'template:indexhtml',
    'copy:dist'
  ];

  for (let profname of ['dev', 'localtest', 'prod']) {
    grunt.registerTask('dist-' + profname, ['profile:' + profname].concat(all_tasks));
  }
};
