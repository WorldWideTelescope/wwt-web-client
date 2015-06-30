# WorldWide Telescope Web Client
## Prerequisites
* Visual Studio 2013 or later ([Free Community Edition](http://www.visualstudio.com/) 
* [NodeJs](http://nodejs.org/download/) - Only needed if you want to compile the sdk into javascript or run the minified web client js code. 

## Included Projects
There are two solutions in the repository. The standalone webclient sln includes only the webclient implementation of the Html5sdk. The Html5sdk contains a sln which loads both the html5sdk AND the webclient. Opening the html5sdk sln enables you to compile the c# files into javascript, then run the webclient locally using the modified js output. 

## Getting Started
* Decide which .sln you want to run (see above) and open in Visual Studio 2013 or higher.
* Right-click webclient\default.aspx and point to View In Browser

## To Enable grunt (LESS compilation, HTML5SDK Compilation, and JS Minification)
* In a command prompt, navigate to the webclient root and type `npm install` to install grunt's dependencies. If you encounter any errors (I always do on a clean machine), ensure `%USERPROFILE%\AppData\Roaming\npm` exists (create it if not) and your system path includes `;%USERPROFILE%\AppData\Roaming\npm`. You can run the ensurenpm.cmd to solve this.
* Once you have installed the node_modules (via `npm install` above), install the grunt client. type `npm install -g grunt-cli` from the same command prompt.
* Edit webclient\gruntfile.js and remove `'copy:webclient'` in the bottom task. Leave an empty string. This task copies the webclient to the worldwidetelescope web site codebase and will clutter your filesystem if you are not developing on the worldwidetelescope.org web site. However if you want to reuse this command to deploy to your own site, edit the `deployLoc` variable.
* If you do not have bower installed, run `npm install -g bower`
* Run `npm install` to install dependencies
* Run `grunt watch` from the same command prompt. 
* In a new command window run grunt bower:install to get the libraries the web client depends on (jquery, angular, bootstrap, etc.). You should see the watcher window detect the dependent libraries and run the 'vendor' grunt task
* Disable any LESS compilers you might be using as visual studio extensions. Grunt watch will do everything for you.

Once you have completed the above you are set to make your own modifications to the code and your LESS and JS will be compiled/uglified. You can view the webclient by right-clicking webclient\default.aspx > view in browser.

To make changes to the HTML5 SDK, simply compile the html5sdk project (named wwtlib) while the grunt watch thread is active. Grunt will take the js output and create the wwtsdk.js file. To use this file instead of the production file, add `?debug=local` to the query string.

## Web client dependencies - further reading
### AngularJS
If you are not familiar with [Angular JS](https://angularjs.org/), I recommend taking some time to learn angularjs as this is a very heavy angular app. 

### Bootstrap
We also use the [bootstrap](http://getbootstrap.com) library and compile our own less with the bootstrap less lib. We use their dropdowns, popovers, and modals quite a bit.

### Angular-strap
We use the [angular-strap](http://mgcrea.github.io/angular-strap/) directives library to integrate bootstrap components into angular. 

### ScriptSharp
We use ScriptSharp to compile the c# classes into javascript. We recently upgraded to [ScriptSharp v0.8](https://github.com/nikhilk/scriptsharp/wiki/Release-Notes)
