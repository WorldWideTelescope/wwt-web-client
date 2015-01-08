/*!

Gruntfile to perform wwt webclient pre-deploy tasks.
We compile webclient.less with bootstrap.less and 
concat/minify all scripts together here.

Bootstrap upgrade note: 
To upgrade bootstrap, copy the 4-color gradient out of the 
mixins/gradients.less into the latest version.
webclient.less depends on that file.

 */

module.exports = function(grunt) {
    'use strict';

    // Force use of Unix newlines
    grunt.util.linefeed = '\n';

    RegExp.quote = function(string) {
        return string.replace(/[-\\^$*+?.()|[\]{}]/g, '\\$&');
    };


    // Project configuration.
    grunt.initConfig({
        deployLoc: '../../../wwtmvc5/WWTMVC5/webclient/',
        // Metadata.
        pkg: grunt.file.readJSON('package.json'),
        banner: '/**\n' +
            '* WorldWide Telescope Web Client\n' +
            '* Copyright 2014 Microsoft Research\n' +
            '* Developed by Jonathan Fay and Ron Gilchrist\n' +
            '**/\n',
        

        // Task configuration.
        clean: {
            deployLoc: '<%=deployLoc%>'
        },
        concat: {
            options: {
                banner: '<%= banner %>'
            },
            webclient: {
                src: [
                    '../ext/angular.js',
                    '../ext/angular-touch.js',
                    '../ext/angular-route.js',
                    '../ext/angular-animate.js',
                    '../ext/angular-cookies.js',
                    '../ext/angular-strap.js',
                    '../ext/angular-strap.tpl.js',
                    '../ext/intro.js',
                    '../ext/angular-intro.js',
                    '../app.js',
                    '../directives/Scroll.js',
                    '../directives/Localize.js',
                    '../directives/ContextMenu.js',
                    '../factories/appstate.js',
                    '../factories/localization.js',
                    '../factories/FinderScope.js',
                    '../factories/Util.js',
                    '../factories/UILibrary.js',
                    '../factories/SearchUtil.js',
                    '../factories/Skyball.js',
                    '../factories/HashManager.js',
                    '../dataproxy/Places.js',
                    '../dataproxy/Tours.js',
                    '../dataproxy/SearchData.js',
                    '../dataproxy/Astrometry.js',
                    '../controllers/MainController.js',
                    '../controllers/ViewController.js',
                    '../controllers/ThumbnailController.js',
                    '../controllers/ToursController.js',
                    '../controllers/SettingsController.js',
                    '../controllers/IntroController.js',
                    '../controllers/AdsController.js',
                    '../controllers/MobileNavController.js',
                    '../controllers/popovers/ObservingTimeController.js',
                    '../controllers/LayerManagerController.js',
                    '../controllers/ShareController.js',
                    '../controllers/modals/OpenItemController.js',
                    '../controls/move.js',
                    '../controls/util.js'
                ],
                dest: '../wwtwebclient.js'
            },
            sdk: {
                src: [
                    '../sdk/ss.js',
                    '../sdk/wwtlib.js'
                ],
                dest: '../sdk/wwtsdk.js'
            },
            oldsdk: {
                src: [
                    '../sdk/old/mscorlib.debug.js',
                    '../sdk/old/wwtlib.debug.js'
                ],
                dest: '../sdk/old/wwtlib_full.js'
            }
        },
        //remove the AMD dependancy from scriptsharp output
        replace: {
            wwtlib: {
                src: ['../sdk/wwtlib.js'],         
                dest: '../sdk/wwtlib.js',          
                replacements: [{
                    from: '"use strict";',                   
                    to: ''
                }, {
                    from: "define('wwtlib', ['ss'], function(ss) {",      
                    to: 'window.wwtlib = function(){'
                }, {
                    from: 'return $exports;\n});',
                    to: 'return $exports;\n}();'
                }]
            }
        },
        
        uglify: {
            options: {
                preserveComments: 'some',
                banner: '<%= banner %>'
            },
            webclient: {
                src: '<%= concat.webclient.dest %>',
                dest: '../wwtwebclient.min.js'
            },
            searchData: {
                src: '../searchdataraw.js',
                dest:'../searchdata.min.js'
            },
            sdk: {
                src: '<%= concat.sdk.dest %>',
                dest:'../sdk/wwtsdk.min.js'
            },
            oldsdk: {
                src: '<%= concat.oldsdk.dest %>',
                dest: '../sdk/old/wwtlib_full.min.js'
            }

        },

        less: {
            compileCore: {
                options: {
                    strictMath: true,
                    sourceMap: true,
                    outputSourceFiles: true,
                    sourceMapURL: 'webclient.css.map',
                    sourceMapFilename: '../css/webclient.css.map'
                },
                src: '../css/bootstrap.less',
                dest: '../css/webclient.css'
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
                src: '../css/webclient.css'
            }
        },

        cssmin: {
            options: {
                compatibility: 'ie10',
                keepSpecialComments: '*',
                noAdvanced: true
            },
            minifyCore: {
                src: '../css/webclient.css',
                dest: '../css/webclient.min.css'
            }
        },
        copy: {
            webclient: {
                files: [
                    {
                        cwd: '../App_Data/',
                        src: '**/*',
                        dest: '<%= deployLoc %>App_Data/',
                        expand: true
                    }, {
                        cwd: '../Bin/',
                        src: '**/*',
                        dest: '<%= deployLoc %>Bin/',
                        expand: true
                    }, {
                        cwd: '../clientbin/',
                        src: '**/*',
                        dest: '<%= deployLoc %>clientbin/',
                        expand: true
                    }, {
                        cwd: '../controllers/',
                        src: '**/*',
                        dest: '<%= deployLoc %>controllers/',
                        expand: true
                    }, {
                        cwd: '../controls/',
                        src: '**',
                        dest: '<%= deployLoc %>controls/',
                        expand: true
                    }, {
                        cwd: '../css/',
                        src: '*.css',
                        dest: '<%= deployLoc %>css/',
                        expand: true
                    }, {
                        cwd: '../css/',
                        src: '*.map',
                        dest: '<%= deployLoc %>css/',
                        expand: true
                    }, {
                        cwd: '../dataproxy/',
                        src: '**',
                        dest: '<%= deployLoc %>dataproxy/',
                        expand: true
                    }, {
                        cwd: '../directives/',
                        src: '**',
                        dest: '<%= deployLoc %>directives/',
                        expand: true
                    }, {
                        cwd: '../ext/',
                        src: '**/*',
                        dest: '<%= deployLoc %>ext/',
                        expand: true
                    }, {
                        cwd: '../factories/',
                        src: '**/*',
                        dest: '<%= deployLoc %>factories/',
                        expand: true
                    }, {
                        cwd: '../images/',
                        src: '**/*',
                        dest: '<%= deployLoc %>images/',
                        expand: true
                    }, {
                        cwd: '../sdk/',
                        src: '*.js',
                        dest: '<%= deployLoc %>sdk/',
                        expand: true
                    }, {
                        cwd: '../sdk/',
                        src: '*.aspx',
                        dest: '<%= deployLoc %>sdk/',
                        expand: true
                    }, {
                        cwd: '../views/',
                        src: '**/*',
                        dest: '<%= deployLoc %>views/',
                        expand: true
                    }, {
                        cwd: '../',
                        src: ['*.jpg', '*.png', '*.asax', '*.cs', '*.aspx', '*.ico', '*.js', '*.xap', '*.xml', '*.wtml'],
                        dest: '<%= deployLoc %>',
                        expand: true
                    }
                ]
            }
        },


    watch: {
        sdk: {
            files: '../sdk/wwtlib.js', 
            tasks: ['replace:wwtlib', 'concat:sdk', 'uglify:sdk']
        },

        // call out only the directories to watch prevents
        // watch from watching recursive node_modules folders e.g.: '../**/*.js'
        scripts: {
            files: ['../controllers/*.js', 
                '../controls/*.js',
                '../directives/*.js',
                '../dataproxy/*.js',
                '../factories/*.js',
                '../app.js'],
            tasks: ['concat:webclient', 'uglify:webclient', 'copy:webclient']
        },
        
        less: {
            files: '../css/*.less',
            tasks: ['less', 'copy:webclient']
        }
    },

    exec: {
      npmUpdate: {
        command: 'npm update'
      }
    }
      
  });


  // These plugins provide necessary tasks.
  require('load-grunt-tasks')(grunt, { scope: 'devDependencies' });
  require('time-grunt')(grunt);
  grunt.loadNpmTasks('grunt-text-replace');
  

    // JS distribution task.
  grunt.registerTask('dist-js', ['concat:webclient', 'uglify:webclient']);

    // Older SDK distribution task (scriptsharp v0.74). 
  grunt.registerTask('oldsdk', ['concat:oldsdk', 'uglify:oldsdk']);
    // SDK distribution task. (scriptsharp v0.8).
  grunt.registerTask('sdk', ['replace:wwtlib', 'concat:sdk', 'uglify:sdk']);

  // Minify the generated search data
  grunt.registerTask('dist-searchdata', ['uglify:searchData']);

  // CSS distribution task.
  grunt.registerTask('less-compile', ['less:compileCore']);
  grunt.registerTask('dist-css', ['less-compile', 'autoprefixer:core', 'cssmin:minifyCore']);

  // Full distribution task.
  grunt.registerTask('dist', ['dist-css', 'dist-js']);
  //deploy to wwt web site
  grunt.registerTask('deploy', ['copy:webclient']);
  //compile js/less and deploy to wwt web site
  grunt.registerTask('deploy-full', ['dist', 'copy:webclient']);

    
};
