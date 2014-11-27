/*!

 * [formerly] Bootstrap's Gruntfile
 * http://getbootstrap.com
 * Copyright 2013-2014 Twitter, Inc.
 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
 
 *****Nov 12, 2014 NOTE******
 
Please Note that this file has been heavily pruned/hacked to perform wwt webclient 
less -> css compilation and js minification. test, docs, jekyl, fonts - 
essentially anything but watching and minifying our js and less files has 
been removed. This project uses the precompiled dist version of bootstrap's js lib.

We compile webclient.less with bootstrap.less

 */

module.exports = function(grunt) {
	'use strict';

	// Force use of Unix newlines
	grunt.util.linefeed = '\n';

	RegExp.quote = function(string) {
		return string.replace(/[-\\^$*+?.()|[\]{}]/g, '\\$&');
	};

	var fs = require('fs');
	var path = require('path');
	//var npmShrinkwrap = require('npm-shrinkwrap');
	var BsLessdocParser = require('./grunt/bs-lessdoc-parser.js');
	/*var getLessVarsData = function () {
	var filePath = path.join(__dirname, 'less/variables.less');
	var fileContent = fs.readFileSync(filePath, { encoding: 'utf8' });
	var parser = new BsLessdocParser(fileContent);
	return { sections: parser.parseFile() };
  };
  var generateRawFiles = require('./grunt/bs-raw-files-generator.js');
  var generateCommonJSModule = require('./grunt/bs-commonjs-generator.js');*/
	var configBridge = grunt.file.readJSON('./grunt/configBridge.json', { encoding: 'utf8' });

	Object.keys(configBridge.paths).forEach(function(key) {
		configBridge.paths[key].forEach(function(val, i, arr) {
			arr[i] = path.join('./docs/assets', val);
		});
	}); /**/

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
		//jqueryCheck: configBridge.config.jqueryCheck.join('\n'),
		//jqueryVersionCheck: configBridge.config.jqueryVersionCheck.join('\n'),

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
					'../ext/angular-route.js',
					'../ext/angular-animate.js',
					'../ext/angular-cookies.js',
					'../ext/angular-strap.js',
					'../ext/angular-strap.tpl.js',
					'../ext/intro.js',
					'../ext/angular-intro.js',
					'../app.js',
					'../directives/Scroll.js',
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
				browsers: configBridge.config.autoprefixerBrowsers
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
						cwd: '../views/',
						src: '**/*',
						dest: '<%= deployLoc %>views/',
						expand: true
					}, {
						cwd: '../',
						src: ['*.jpg', '*.png', '*.asax', '*.cs', '*.aspx', '*.ico', '*.js', '*.xap', '*.xml', '*.config'],
						dest: '<%= deployLoc %>',
						expand: true
					}
				]
			}
		},


	watch: {
		scripts: {
			files: '../**/*.js', 
			tasks: ['concat', 'uglify']
		},
		less: {
			files: '../css/*.less',
			tasks: 'less'
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

  // JS distribution task.
  grunt.registerTask('dist-js', ['concat', 'uglify:webclient','uglify:searchData']);

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
