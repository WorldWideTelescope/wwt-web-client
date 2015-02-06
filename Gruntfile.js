/**!
Gruntfile to perform wwt webclient less compilation,
script concatenation/minification, and component updates
(using grunt bower:install)

Once you have run npm install and npm update
and bower is installed (npm install -g bower)
run grunt watch

**/

module.exports = function(grunt) {
    'use strict';

    // Force use of Unix newlines
    grunt.util.linefeed = '\n';

    RegExp.quote = function(string) {
        return string.replace(/[-\\^$*+?.()|[\]{}]/g, '\\$&');
    };

    // Project configuration.
    grunt.initConfig({
        deployLoc: '../../wwtmvc5/WWTMVC5/webclient/',
        pkg: grunt.file.readJSON('package.json'),
        banner: '/**\n' +
            '* WorldWide Telescope Web Client\n' +
            '* Copyright 2014-2015 Microsoft Research\n' +
            '* Developed by Jonathan Fay and Ron Gilchrist\n' +
            '* Licensed under <%= pkg.license.type %> (<%= pkg.license.url %>)\n' +
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
                    'ext/jquery.js',
                    'ext/bootstrap.js',
                    'ext/angular.js',
                    'ext/angular-touch.js',
                    'ext/angular-route.js',
                    'ext/angular-animate.js',
                    'ext/angular-cookies.js',
                    'ext/angular-strap.js',
                    'ext/angular-strap.tpl.js',
                    'ext/intro.js',
                    'ext/angular-intro.js',
                    'app.js',
                    'directives/Scroll.js',
                    'directives/Localize.js',
                    'directives/ContextMenu.js',
                    'factories/appstate.js',
                    'factories/localization.js',
                    'factories/FinderScope.js',
                    'factories/ThumbList.js',
                    'factories/Util.js',
                    'factories/UILibrary.js',
                    'factories/SearchUtil.js',
                    'factories/Skyball.js',
                    'factories/HashManager.js',
                    'dataproxy/Places.js',
                    'dataproxy/Tours.js',
                    'dataproxy/SearchData.js',
                    'dataproxy/Astrometry.js',
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
                    'controllers/modals/ShareController.js',
                    'controllers/modals/OpenItemController.js',
                    'controllers/modals/ObservingTimeController.js',
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
        copy: {
            vendor: {
                files: [
                    {
                        expand: true,
                        flatten: true,
                        src: 'bower_components/jquery/dist/jquery.js',
                        dest: 'ext/'
                    }, {
                        dest: 'ext/',
                        expand: true,
                        flatten: true,
                        src: 'bower_components/bootstrap/dist/js/bootstrap.js'
                    }, {
                        dest: 'ext/',
                        expand: true,
                        flatten: true,
                        src: 'bower_components/angular/angular.js'
                    },
                    {
                        dest: 'ext/',
                        expand: true,
                        flatten: true,
                        src: 'bower_components/angular-touch/angular-touch.js'
                    },
                    {
                        dest: 'ext/',
                        expand: true,
                        flatten: true,
                        src: 'bower_components/angular-route/angular-route.js'
                    },
                    {
                        dest: 'ext/',
                        expand: true,
                        flatten: true,
                        src: 'bower_components/angular-animate/angular-animate.js'
                    },{
                        dest: 'ext/',
                        expand: true,
                        flatten: true,
                        src: 'bower_components/angular-cookies/angular-cookies.js'
                    }, {
                        dest: 'ext/',
                        expand: true,
                        flatten: true,
                        src: 'bower_components/angular-strap/dist/angular-strap.js'
                    }, {
                        dest: 'ext/',
                        expand: true,
                        flatten: true,
                        src: 'bower_components/angular-strap/dist/angular-strap.tpl.js'
                    }, {
                        cwd: 'bower_components/bootstrap/less/',
                        src: '**/*',
                        dest: 'bootstrap/less/',
                        expand: true
                    }
                ]
            },
            webclient: {
                files: [
                    {
                        cwd: 'App_Data/',
                        src: '**/*',
                        dest: '<%= deployLoc %>App_Data/',
                        expand: true
                    }, {
                        cwd: 'Bin/',
                        src: '**/*',
                        dest: '<%= deployLoc %>Bin/',
                        expand: true
                    }, {
                        cwd: 'clientbin/',
                        src: '**/*',
                        dest: '<%= deployLoc %>clientbin/',
                        expand: true
                    }, {
                        cwd: 'controllers/',
                        src: '**/*',
                        dest: '<%= deployLoc %>controllers/',
                        expand: true
                    }, {
                        cwd: 'controls/',
                        src: '**',
                        dest: '<%= deployLoc %>controls/',
                        expand: true
                    }, {
                        cwd: 'css/',
                        src: '*.css',
                        dest: '<%= deployLoc %>css/',
                        expand: true
                    }, {
                        cwd: 'css/',
                        src: '*.map',
                        dest: '<%= deployLoc %>css/',
                        expand: true
                    }, {
                        cwd: 'dataproxy/',
                        src: '**',
                        dest: '<%= deployLoc %>dataproxy/',
                        expand: true
                    }, {
                        cwd: 'directives/',
                        src: '**',
                        dest: '<%= deployLoc %>directives/',
                        expand: true
                    }, {
                        cwd: 'ext/',
                        src: '**/*',
                        dest: '<%= deployLoc %>ext/',
                        expand: true
                    }, {
                        cwd: 'factories/',
                        src: '**/*',
                        dest: '<%= deployLoc %>factories/',
                        expand: true
                    }, {
                        cwd: 'images/',
                        src: '**/*',
                        dest: '<%= deployLoc %>images/',
                        expand: true
                    }, {
                        cwd: 'sdk/',
                        src: '*.js',
                        dest: '<%= deployLoc %>sdk/',
                        expand: true
                    }, {
                        cwd: 'sdk/',
                        src: '*.aspx',
                        dest: '<%= deployLoc %>sdk/',
                        expand: true
                    }, {
                        cwd: 'views/',
                        src: '**/*',
                        dest: '<%= deployLoc %>views/',
                        expand: true
                    }, {
                        cwd: '',
                        src: ['*.jpg', '*.png', '*.asax', '*.cs', '*.aspx', '*.ico', '*.js', '*.xap', '*.xml', '*.wtml'],
                        dest: '<%= deployLoc %>',
                        expand: true
                    }
                ]
            }
        },
        watch: {
            sdk: {
                files: 'sdk/wwtlib.js', 
                tasks: ['sdk', 'deploy']
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
                tasks: ['dist-js', 'deploy']
            },
            vendor: { // will be triggered by 'bower install' when it finds updates
                files: [
                    'bower_components/jquery/dist/jquery.js',
                    'bower_components/bootstrap/dist/js/bootstrap.js',
                    'bower_components/angular/angular.js',
                    'bower_components/angular-strap/dist/angular-strap.js'
                ],
                tasks: ['vendor']
            },

            html: {
                files: [
                    'views/**/*.html',
                    'Default.aspx'
                ],
                tasks: ['deploy']
            },
            less: {
                files: 'css/*.less',
                tasks: ['dist-css', 'deploy']
            }
        },
        bower: {
            install: {
                options: {
                    targetDir:'bower_components',
                    install: true,
                    verbose: true,
                    cleanTargetDir: false,
                    cleanBowerDir: false,
                    bowerOptions: {}
                }
            }
        }
    });


    // Dependencies
    require('load-grunt-tasks')(grunt, { scope: 'devDependencies' });
    
    // JS concatenation and minification
    grunt.registerTask('dist-js', ['concat:webclient', 'uglify:webclient']);

    // Takes HTML5SDK generated script and packages into single usable lib. (scriptsharp v0.8).
    grunt.registerTask('sdk', ['replace:wwtlib', 'concat:sdk', 'uglify:sdk']);

    // Minify the generated search data (rare - internal only)
    grunt.registerTask('dist-searchdata', ['uglify:searchData']);

    // CSS  (csscomb seems like too much, so commented out for now)
    grunt.registerTask('dist-css', ['less:compileCore', 'autoprefixer:core', /*'csscomb:dist',*/'cssmin:minifyCore']);

    // Vendor JS libs
    grunt.registerTask('vendor', ['copy:vendor','dist-js','dist-css','deploy']);

    // Deploy to wwt web site (internal only)
    grunt.registerTask('deploy', ['copy:webclient']);

    // uncomment out the below task and comment out the above task to run locally
    //grunt.registerTask('deploy', []);
};
