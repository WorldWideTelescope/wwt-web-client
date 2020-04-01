/**
 * Copyright 2014, 2015  Microsoft Research
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use, copy,
 * modify, merge, publish, distribute, sublicense, and/or sell copies
 * of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
 * BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
 * ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 **/

// Note that, unlike the other source files, this file is processed through
// Grunt's templating system. This is done so that we can insert various
// build-time parameters. Note also that the variables here must references to
// the Grunt config object; the templating in index.html uses different
// variables.

var wwt = {
  // If adding new items, also add them to the AngularJS $rootScope in factories/Util.js.
  gitShortSha: '<%= gitinfo.local.branch.current.shortSHA %>',
  staticAssetsPrefix: '<%= profile_data.webclient_static_assets_url_prefix %>',
  userwebUrlPrefix: '<%= profile_data.userweb_url_prefix %>',
  coreStaticUrlPrefix: '<%= profile_data.core_static_url_prefix %>',
  communitiesUrlPrefix: '<%= profile_data.communities_url_prefix %>',
  msLiveOAuthAppId: '<%= profile_data.microsoft_live_oauth_app_id %>',
  msLiveOAuthRedirUrl: '<%= profile_data.microsoft_live_oauth_redir_url %>',

  app: angular.module('wwtApp', [
    'mgcrea.ngStrap',
    'ngTouch',
    'ngAnimate',
    'ngRoute',
    'wwtControllers',
    'ngCookies',
    'angular-intro'
  ]).config(function($sceDelegateProvider) {
    $sceDelegateProvider.resourceUrlWhitelist([
      // same origin
      'self',

      // In production, we get assets through the CDN. We'll be a little
      // sloppy and unconditionally allow both HTTP and HTTPS.
      'https://web.wwtassets.org/**',
      'http://web.wwtassets.org/**',
      'https://cdn.worldwidetelescope.org/**',
      'http://cdn.worldwidetelescope.org/**',
      'https://beta-cdn.worldwidetelescope.org/**',
      'http://beta-cdn.worldwidetelescope.org/**'
    ]);
  }),

  controllers: angular.module('wwtControllers', [])
};

$(window).on('load', function() {
  // Load search data after everything else.
  //
  // Historically the search data were managed in the webclient repository,
  // but they're really a "core" asset, since they index the core WWT
  // datasets.
  var scr = document.createElement('script');
  scr.setAttribute("src", wwtlib.URLHelpers.singleton.coreStaticUrl('data/searchdata.min.js'));
  document.getElementsByTagName("head")[0].appendChild(scr);
});
