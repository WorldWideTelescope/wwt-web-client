<%@ Page Language="C#" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Security.Cryptography.X509Certificates" %>

<script runat="server">

    public bool Debug = false;
    public bool DebugChrome = false;
    public bool ADS = false;
    public string DotMin = ".min.js";
    // This enables the webclient to load in both the worldwidetelescope.org site
    // with js/css/imagery on the cdn and in standalone mode without a cdn
    public string ResourcesLocation;
    public string ResourcesVersion = ConfigurationManager.AppSettings["ResourcesVersion"];
    public string DebugQs = "?v=" + ConfigurationManager.AppSettings["ResourcesVersion"];
    public string BodyClass;
    public string SDKLocation = "sdk/wwtsdk.min.js";
    public string ImgDir = "https://wwtweb.blob.core.windows.net/webclient/";
    public enum Clients
    {
      Html5 = 0,
      Silverlight = 1,
      WWT = 2,
      Mobile = 3
    };
    public Clients Client = Clients.Html5;

    protected void Page_Load(object sender, EventArgs e)
    {

      ResourcesLocation = ConfigurationManager.AppSettings["WebClientResourcesLocation"] ?? ConfigurationManager.AppSettings["ResourcesLocation"] + "/webclient";
      bool isMobile = Request.QueryString["mobile"] != null && Request.QueryString["mobile"] != "0";

      BodyClass = string.Format("fs-player wwt-webclient-wrapper {0}", isMobile ? "mobile" : "desktop");

      if (Request.QueryString["debug"] != null)
      {
        Random rnd = new Random();
        //ResourcesVersion = rnd.Next(1, 999999).ToString();
        //DebugQs = "?debug=true&v=" + ResourcesVersion;
        Debug = true;
        DotMin = ".js";
        if (Request.QueryString["debug"] == "chrome")
        {
          Debug = false;
          DebugChrome = true;
          DebugQs = "";
        }
        else if (Request.QueryString["debug"] == "local")
        {
          DotMin = ".min.js" + DebugQs;
        }
        
      }
      if (Request.QueryString["ads"] != null)
      {
        ADS = true;
      }
      if (Request.Cookies["preferredClient"] != null)
      {
        switch (Request.Cookies["preferredClient"].Value)
        {
          case "SL":
            Client = Clients.Silverlight;
            break;
          case "WWT":
            Client = Clients.WWT;
            break;
          case "Mobile":
            Client = Clients.Mobile;
            break;
          default:
            Client = Clients.Html5;
            break;
        }
      }
      if (Request.QueryString["client"] != null && !isMobile)
      {
        HttpCookie cookie = Request.Cookies["preferredClient"] ?? new HttpCookie("preferredClient");
        char c = Request.QueryString["client"].ToString(CultureInfo.InvariantCulture).ToLower().ToCharArray()[0];
        if (c == 'h')
        {
          Client = Clients.Html5;
          cookie.Value = "HTML5";
        }
        else if (c == 's')
        {
          Client = Clients.Silverlight;
          cookie.Value = "SL";
        }
        else if (c == 'm')
        {
          Client = Clients.Mobile;
          cookie.Value = "Mobile";
        }
        else if (c == 'w')
        {
          Client = Clients.WWT;
          cookie.Value = "WWT";
        }

        HttpContext.Current.Response.Cookies.Add(cookie);

      }
      else if (isMobile) {
        Client = Clients.Mobile;
      }
      //if (Client == Clients.Html5 && isMobile)
      //{
      //    Response.Redirect(string.Format("/webclient/?client=mobile{0}", Debug ? "&debug=true" : ""));
      //}
      //else if (Client == Clients.Mobile && !isMobile)
      //{
      //    Response.Redirect(string.Format("/webclient/?client=html5{0}", Debug ? "&debug=true" : ""));
      //}

    }
</script>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>WorldWide Telescope Web Client</title>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
    <meta name="description" content="Worldwide Telescope enables your computer to function as a virtual telescope, bringing together imagery from the best telescopes in the world." />
    <meta name="ROBOTS" content="INDEX, FOLLOW">
    <meta property="og:url" content="//worldwidetelescope.org/webclient" />
    <meta property="og:title" content="WorldWide Telescope Web Client" />
    <meta property="og:description" content="Worldwide Telescope enables your computer to function as a virtual telescope, bringing together imagery from the best earth and space-based telescopes." />
    <meta property="og:image" content="https://wwtweb.blob.core.windows.net/webclient/wwtlogo.png" />
    <link rel="icon" href="favicon.ico"/>
    <% if (Client == Clients.Html5 || Client == Clients.Mobile)
       {
        string css = "<link href=\"css/webclient.min.css?v="+ResourcesVersion+"\" rel=\"stylesheet\" />";
        css+="<link href=\"css/angular-motion.css?v="+ResourcesVersion+"\" rel=\"stylesheet\" />";
        css+="<link href=\"ext/introjs.css?v="+ResourcesVersion+"\" rel=\"stylesheet\" />";
        %>
    <%=css%>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/jScrollPane/2.0.23/style/jquery.jscrollpane.css" rel="stylesheet"/>
    <link href="//maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css" rel="stylesheet"/>
  <!-- Global Site Tag (gtag.js) - Google Analytics -->
<script async src="https://www.googletagmanager.com/gtag/js?id=UA-107473046-1"></script>
<script>
    window.dataLayer = window.dataLayer || [];
    function gtag() { dataLayer.push(arguments) };
    gtag('js', new Date());
    gtag('config', 'UA-107473046-1');
</script>
    <style>
      body .finder-scope {
            background: url(Images/finder-scope.png?v=<%= ResourcesVersion %>) no-repeat;
      }
    </style>
    <script src="//js.live.net/v5.0/wl.js"></script>
    <script src="sdk/wwtsdk<%=DotMin %>"></script>
    <script src="//code.jquery.com/jquery-2.1.4.min.js"></script>
    <script src="//maxcdn.bootstrapcdn.com/bootstrap/3.3.5/js/bootstrap<%=DotMin %>"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/angular.js/1.5.8/angular<%=DotMin %>"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/angular.js/1.5.8/angular-touch<%=DotMin %>"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/angular.js/1.5.8/angular-route<%=DotMin %>"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/angular.js/1.5.8/angular-cookies<%=DotMin %>"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/angular.js/1.5.8/angular-animate<%=DotMin %>"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/angular-strap/2.3.8/angular-strap<%=DotMin %>"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/angular-strap/2.3.8/angular-strap.tpl<%=DotMin %>"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-mousewheel/3.1.13/jquery.mousewheel<%=DotMin %>"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jScrollPane/2.0.23/script/jquery.jscrollpane.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pako/1.0.3/pako_inflate.min.js"></script>
    <% if (Debug || DebugChrome)
       { %>

    <script src="<%= ResourcesLocation %>/ext/intro.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/ext/angular-intro.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/app.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/dataproxy/Places.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/dataproxy/Tours.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/dataproxy/SearchData.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/dataproxy/Astrometry.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/dataproxy/Community.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/directives/Scroll.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/directives/Localize.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/directives/EditSlideValues.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/directives/ContextMenu.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/directives/Movable.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/directives/CopyToClipboard.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/appstate.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/autohidepanels.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/localization.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/FinderScope.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/ThumbList.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/Util.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/UILibrary.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/SearchUtil.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/Skyball.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/HashManager.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/MediaFile.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/ContextPanelController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/IntroController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/LayerManagerController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/MainController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/MobileNavController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/modals/ObservingTimeController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/modals/ShareController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/modals/OpenItemController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/modals/TourSlideText.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/modals/SlideSelectionController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/modals/VoConeSearchController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/modals/VOTableViewerController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/modals/colorpickerController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/modals/refFrameController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/modals/GreatCircleController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/modals/DataVizController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/modals/EmbedController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/tabs/AdsController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/tabs/ExploreController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/tabs/SearchController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/tabs/SettingsController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/tabs/ToursController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/tabs/ViewController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/tabs/CommunityController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/tabs/CurrentTourController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/LoginController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controls/move.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controls/util.js?v=<%= ResourcesVersion%>"></script>
    <% }
       else
       { %>
    <script src="wwtwebclient<%=DotMin%>?v=<%= ResourcesVersion%>"></script>
    <% } %>

    <% }
       else if (Client == Clients.Silverlight)
       { %>


        <style type="text/css">
            html, body {
                height: 100%;
                overflow: hidden;
            }
            body {
                padding: 0;
                margin: 0;
            }
            #silverlightControlHost {
                height: 100%;
            }
        </style>
     <script src="fblogin.js" type="text/javascript"></script>
      <script
           src="//static.ak.connect.facebook.com/js/api_lib/v0.4/FeatureLoader.js.php"
           type="text/javascript">
      </script>

    <script type="text/javascript">
        function onSilverlightError(sender, args) {
            return;
        }
    </script>
    <% } %>


</head>
<body
    class="<%= BodyClass %>"
    data-ng-app="wwtApp"
    data-res-location="<%= ResourcesLocation%>"
    data-version="1"
    data-res-version="<%= ResourcesVersion%>"
    data-liveId="<%=ConfigurationManager.AppSettings["LiveClientId"]%>"
    data-standalone-mode="<%=ConfigurationManager.AppSettings["Standalone"]%>">
    <% if (Client == Clients.Html5 || Client == Clients.Mobile)
       { %>
<div data-ng-controller="MainController" ng-cloak ng-init="initUI()" class="<%=Client == Clients.Mobile?"mobile":"desktop" %>">

    <div id="WorldWideTelescopeControlHost">
        <div id="WWTCanvas" ng-context-menu="<%=Client == Clients.Mobile? "" : "showFinderScope"%>"></div>
    </div>

    <div id="contextmenu" class="contextmenu">

    </div>
    <div id="popoutmenu" class="contextmenu">

    </div>

    <div id="histogram" class="histogram">
      <div class="header">
        <div id="histogramClose">
          <a><i class="fa fa-close"></i></a>
        </div>
        <span localize="Histogram"></span>

      </div>
      <div style="margin: 0px;">
        <canvas id="graph" src="" width="256" height="150" style="margin: 0px;" />
        </div>
        <div class="select">
            <select id="ScaleTypePicker" >
               <option localize="Linear"></option>
               <option localize="Log"></option>
               <option localize="Power"></option>
               <option localize="Square Root"></option>
               <option localize="Histogram Equalization"></option>
            </select>
        </div>
    </div>

  <div class="modal wwt-modal simpleinput" id="simplemodal" tabindex="-1" role="dialog">
    <div class="modal-dialog" id="simpleinput">
      <div class="modal-content">
        <div class="modal-header">
        <i class="fa fa-close pull-right" id="simpleinputcancel"></i>
          <h5 id="simpletitle"></h5>
        </div>
        <div class="modal-body">
          <div id="inputlabel">Enter Text</div>
          <input type="text" id="inputtext" />

        </div>
        <div class="modal-footer">
          <a class="btn" id="simpleinputok" localize="OK"></a>
          <a class="btn" id="simpleinputcancel" localize="Cancel"></a>
        </div>
      </div>
    </div>
  </div>


    <!--Layer Manager Recursive Tree Template (shared)-->
    <script type="text/ng-template" id="tree-toggle">
      <i class="fa" ng-if="hasChildren(node)"
        ng-class="collapsed(node) ? 'fa-plus-square-o' : 'fa-minus-square-o'"
        ng-click="collapse(node)"></i>
      <i ng-if="!hasChildren(node)" class="fa">&nbsp;&nbsp;&nbsp;</i>
    </script>
    <script type="text/ng-template" id="tree-node">
     <div ng-show="name || node.name">
       <ng-include src="'tree-toggle'"></ng-include>
       <div ng-if="isObjectNode(node)" class="checkbox" ng-right-click="showMenu(node,$event)" ng-class="{activelayer:node.active}">
         <label data-ng-class="{checked:node.enabled, disabledChild:!node.enabled}" localize="{{name}}" ng-click="selectionChanged(node,$event)"
                localize-only="title">
           <input type="checkbox" ng-model="node.enabled"/>
           <span localize="{{name}}"></span>
         </label>
       </div>
       <div class="checkbox" ng-if="!isObjectNode(node)" ng-class="{activelayer:node.active}">
          <label data-ng-class="{checked:node.checked, disabledChild:node.disabled}" localize="{{node.name}}" localize-only="title" ng-click="selectionChanged(node,$event)">
            <input type="checkbox" ng-model="node.checked" ng-disabled="node.disabled"  ng-change="nodeChange(node)" />
            <span localize="{{node.name}}"></span>
          </label>
        </div>
       <div class="indent"
            ng-class="collapsed(node) ? ' collapsed' : ''"
            ng-repeat="(name,node) in getChildren(node)"
            ng-include="'tree-node'">
       </div>
     </div>

     </script>

<% if (Client == Clients.Mobile)
   { %>

    <a  data-bs-popover="popover" tabindex="0"
        localize="Share this place"
        localize-only="title"
        data-content-template="views/popovers/shareplace.html?v=<%=ResourcesVersion%>"
        data-ng-class="searchModal ? 'hide':'btn share-button'"
        data-placement="bottom-left"
        data-ng-hide="showMobileTracking()"
        >
        <i class="fa fa-share-alt"></i>
        <span localize="Share"></span>
    </a>

    <div ng-show="showMobileTracking()" class="tracking-container">
        <div title="{{trackingObj.get_name()}}" class="small"><strong localize="Tracking"></strong> <br/>{{trackingObj.get_name()}}</div>
        <br />
        <a class="btn" data-bs-popover="popover" tabindex="0"
            localize="Share this place"
            localize-only="title"
            data-content-template="views/popovers/shareplace.html?v=<%=ResourcesVersion%>"
            data-container="body"
            data-placement="bottom-left"
            >
            <i class="fa fa-share-alt"></i>
            <span localize="Share"></span>
        </a>
    </div>
    <ng-include src="'views/modals/mobile-explore.html?v=<%=ResourcesVersion%>'"></ng-include>

    <div class="navbar navbar-inverse navbar-fixed-top" ng-controller="MobileNavController"  ng-show="!tourPlaying">
        <div class="container">
            <div class="navbar-header">
                <button class="btn navbar-toggle" data-target=".navbar-collapse" data-toggle="collapse" type="button">
                    <i class="fa fa-bars"></i>
                </button>

            </div>
            <div class="navbar-collapse collapse">
                <ul class="nav navbar-nav">
                    <li class="dropdown">
                        <a href="javascript:void(0)" localize="Look At" localize-only="title" class="dropdown-toggle" data-toggle="dropdown">
                            <span localize="Look At"></span>
                            <span class="small">({{lookAt}})</span><i class="fa fa-angle-down"></i>
                        </a>
                        <ul class="dropdown-menu" >
                            <li ng-repeat="type in lookTypes" ng-click="hideMenu()">
                                <a href="javascript:void(0)" ng-click="lookAtDropdownChanged(type);">
                                    {{type}}
                                </a>
                            </li>
                        </ul>
                    </li>
                    <li class="dropdown">
                        <a href="javascript:void(0)" localize="Imagery" localize-only="title" class="dropdown-toggle" data-toggle="dropdown">
                            <span localize="Imagery"></span>
                            <span class="small">({{backgroundImagery.get_name()}})</span><i class="fa fa-angle-down"></i>
                        </a>
                        <ul class="dropdown-menu" >
                            <li ng-repeat="s in surveys" ng-click="hideMenu()">
                                <a href="javascript:void(0)" ng-click="setSurveyBg(s.name);">
                                    {{::s.name}}
                                </a>
                            </li>
                        </ul>

                    </li>
                    <li>
                        <hr/>
                    </li>

                    <li class="modal-buttons">
                        <a class="btn" ng-repeat="m in modalButtons" ng-click="showModal(m)">
                            <i class="fa {{::m.icon}}"></i>
                            {{::m.text}}
                        </a>
                    </li>
                    <li>
                        <hr class="clearfix"/>
                    </li>
                    <li>
                        <a ng-click="resetCamera()" localize="Reset Camera"></a>
                    </li>
                    <li>
                        <hr class="clearfix"/>
                    </li>
                    <li>
                        <a ng-click="gotoPage('/home')" target="_blank" localize="WorldWide Telescope Home"></a>
                    </li>
                    <li>
                        <a ng-click="gotoPage('/Learn')" target="_blank" localize="Getting Started (Help)"></a>
                    </li>
                    <li>
                        <a ng-click="gotoPage('/Terms')" target="_blank" localize="WorldWide Telescope Terms of Use"></a>
                    </li>
                    <li>
                        <a ng-click="gotoPage('/About')" localize="About WorldWide Telescope"></a>
                    </li>
                    <li>
                        <a ng-click="gotoPage('/Support/IssuesAndBugs')" localize="Product Support"></a>
                    </li>
                </ul>

            </div>
        </div>
    </div>
    <div ng-class="isLoading ?  'mobile-loading' : 'hide'">
        <a href="//aas.org" target="_blank" style="position:relative;left:3px;z-index:5"
            class="pull-right">
            <img ng-src="https://wwtweb.blob.core.windows.net/images/aas-white110.png"
                    localize="American Astronomical Society (AAS) Logo"
                    localize-only="alt" style="width:60px;height:60px;"/>
        </a>
        <a class="pull-left" href="/home" style="margin-left: -11px;">
            <img src='<%=ImgDir %>wwtlogo.png?v=<%= ResourcesVersion%>'
                localize="WorldWide Telescope Logo"
                localize-only="alt"
                style="height:60px;width:60px"
             />
        </a>
        <h3>
            <div class="small text-white">American Astronomical Society</div>
            World<span class="brand-blue">Wide Telescope</span>
        </h3>
        <br />
        <h4 localize="Welcome to the WorldWide Telescope Web Client"></h4>
        <br />
        <p style="text-align:center">
            <i class="fa fa-spin fa-spinner"></i>
            <span localize="WorldWide Telescope is loading."></span><br />
            <span localize="Please wait."></span>

        </p>
        <br />
        <p class="small" localize="Please ensure you have a strong connection to the internet for the best experience.">
            Please ensure you have a strong connection to the internet for the best experience.
        </p>

        <div class="checkbox pull-left">
            <label data-ng-class="iswebclientHome ? 'checked' : ''">
                <input type="checkbox" ng-model="iswebclientHome" ng-change="homePrefChange(iswebclientHome)" />
                <span localize="Show the web client as WWT landing page"></span>
            </label>
        </div>
    </div>
    <ng-include src="'views/modals/mobile-nearby-objects.html?v=<%=ResourcesVersion%>'"></ng-include>
    <div class="context-panel">
        <div class="nearby-objects" ng-if="nbo.length && (lookAt == 'Sky' || lookAt=='SolarSystem')" ng-show="!tourPlaying">
            <a ng-click="showNbo()" title="{{nbo[0].get_name()}}" class="thumbnail">
                <img ng-src="{{nbo[0].get_thumbnailUrl()}}" alt="Thumbnail of {{nbo[0].get_name()}}" />
                <label localize="Nearby"></label>
            </a>
            <div class="nbo-count">{{nboCount}}</div>
        </div>


        <div class="control x-fader-mobile" ng-show="displayXFader()">
            <label localize="Image Crossfade"></label>
            <div class="cross-fader">
                <a class="btn">&nbsp;</a>
            </div>

        </div>
        <div class="fov-panel mobile" ng-show="lookAt=='Sky' && !tourPlaying">
            <div class="left">
                <p localize="N"></p>
                <div class="sphere mobile" id="skyball">
                    <div class="v-ellipse"></div>
                    <div class="h-ellipse"></div>
                    <div class="x-axis"></div>
                    <div class="y-axis"></div>
                </div>
            </div>
        </div>
    </div>
    <% } %>
    <%if (Client == Clients.Html5)
   { %>
    <ng-include src="'views/research-menu.html?v=<%=ResourcesVersion%>'"></ng-include>
    <ng-include src="'views/modals/finder-scope.html?v=<%=ResourcesVersion%>'" onload="initFinder()"></ng-include>


    <div data-ng-controller="ViewController"></div>

    <ul class="dropdown-menu" role="menu" id="topMenu"></ul>


    <div id="ribbon">
        <span class="pull-right" ng-controller="LoginController">
        <a class="btn pull-right" ng-click="login()" ng-show="liveAppId && liveAppId.length && !loggedIn">
            <span localize="Sign In"></span>
        </a>
        <a class="btn pull-right" ng-click="logout()" ng-show="liveAppId && liveAppId.length && loggedIn">
            <span localize="Sign Out"></span>
        </a>

        </span>
        <a class="btn pull-right" href="/Download/" target="wwt">
            <i class="fa fa-download"></i>
            <span localize="Install Windows Client"></span>
        </a>
        <a class="home-icon" href="/home">
            <i class="fa fa-home"></i>
        </a>
        <ul class="wwt-tabs">

            <li data-ng-repeat="tab in ribbon.tabs" data-ng-class="activePanel == tab.label ? 'active' : ''">
                <div class="outer">
                    <a href="javascript:void(0)">
                        <span class="label" data-ng-click="tabClick(tab)" id="{{tab.button}}" localize="{{tab.label}}"></span>
                        <div class="menu" data-ng-click="menuClick(tab.menu)" id="tabMenu{{$index}}" data-target="#menu{{$index}}">
                            <i class="fa fa-caret-down"></i>
                        </div>
                    </a>
                </div>
            </li>
            <li data-ng-class="activePanel == 'currentTour'  ? 'active' : currentTour && currentTour._title ? '' :'hide'">
                <div class="outer">
                    <a href="javascript:void(0)" ng-click="activePanel = 'currentTour';initSlides()">
                        <span class="label" style="padding-right:22px">{{currentTour._title}}</span>
                        <span ng-click="closeTour($event)" class="close-tour"><i class="fa fa-close"></i></span>
                    </a>

                </div>
            </li>
        </ul>

    </div>

    <div class="{{topExpanded && activePanel.toLowerCase() == expandedPanel ? 'top-panel top-expanded' : 'top-panel'}}" id="topPanel" ng-switch="activePanel">
        <div
            ng-show="!loadingUrlPlace"
            ng-switch-when="Explore"
            class="{{expanded ? 'explore-panel rel expanded' : 'explore-panel rel'}}"
            ng-controller="ExploreController"
            >
            <span ng-repeat="bc in breadCrumb" class="bc"><a href="javascript:void(0)" ng-click="breadCrumbClick($index)">{{bc}}</a>&nbsp;>&nbsp;</span><br />
            <div class="explore-thumbs">
                <div class="ribbon-thumbs" ng-repeat="item in collectionPage">
                    <ng-include src="'views/thumbnail.html?v=<%=ResourcesVersion%>'"></ng-include>
                </div>

            </div>
            <label class="wwt-pager">
                <a href="javascript:void(0)" data-ng-disabled="currentPage == 0" ng-click="goBack()">
                    <i class="fa fa-play reverse"></i>
                </a>
                {{(currentPage+1)}} <span localize="of"></span> {{pageCount}}
                <a href="javascript:void(0)" ng-disabled="currentPage == pageCount - 1" ng-click="goFwd()">
                    <i class="fa fa-play"></i>
                </a>
            </label>
            <a class="{{expanded ? 'expanded btn tn-expander' : 'btn tn-expander'}}" ng-click="expandThumbnails()">
                <i class="fa fa-caret-down" ng-if="!expanded"></i>
                <i class="fa fa-caret-up" ng-if="expanded"></i>
            </a>
        </div>
        <div ng-switch-when="Guided Tours" id="toursPanel" ng-controller="ToursController">
            <span ng-repeat="bc in breadCrumb" class="bc"><a href="javascript:void(0)" ng-click="breadCrumbClick($index)">{{bc}}</a>&nbsp;>&nbsp;</span><br />

            <div class="ribbon-thumbs" ng-repeat="item in tourList">

                <span class="tour-thumb" id="tourthumb{{$index}}" ng-if="item.thumb.length > 15 && $index >= currentPage * pageSize && $index < (currentPage+1) * pageSize">
                    <a ng-click="clickThumb(item)" ng-mouseenter="tourPreview($event, item)"
                       title="{{item.get_name()}}"
                       ng-class="item.thumb + item.get_name() == activeItem ? 'thumbnail active' : 'thumbnail'" >
                        <img ng-src="{{item.thumb}}" alt="Thumbnail of {{item.get_name()}}" />
                        <label>{{item.get_name()}}</label>
                    </a>

                </span>
                <span ng-if="item.thumb.length > 15 && !($index >= currentPage * pageSize && $index < (currentPage+1) * pageSize)">
                    <img ng-src="{{item.thumb}}" class="offscreen" />
                </span>
            </div>
            <label class="wwt-pager">
                <a href="javascript:void(0)" data-ng-disabled="currentPage == 0" ng-click="currentPage = currentPage == 0 ? currentPage : currentPage - 1">
                    <i class="fa fa-play reverse"></i>
                </a>
                {{(currentPage + 1)}} <span localize="of"></span> {{pageCount}}
                <a href="javascript:void(0)" ng-disabled="currentPage == pageCount - 1" ng-click="currentPage = currentPage == pageCount - 1 ? currentPage : currentPage + 1">
                    <i class="fa fa-play"></i>
                </a>
            </label>
        </div>
        <div ng-switch-when="Search" ng-controller="SearchController" id="searchPanel" class="{{expanded ? 'explore-panel rel expanded' : 'explore-panel rel'}}">
            <div class="search-controls">
                <div class="iblock input-group">
                    <input type="search" id="txtSearch" ng-model="q" ng-keydown="searchKeyPress()" localize="Object Search" localize-only="placeholder" />
                    <span class="fa fa-search form-control-feedback"></span>
                </div>
                <div class="control-group">
                    <div class="select">
                        <select ng-model="SearchType">
                            <option value="J2000" localize="J2000"></option>
                            <%--<!--
                                <option localize="Alt/Az"></option>
                                <option localize="Galactic"></option>
                                <option localize="Ecliptic"></option>-->--%>
                        </select>
                    </div>
                    &nbsp;
                    <label>
                        <span localize="RA"></span>
                        <input type="text" ng-model="goto.RA" />
                    </label>

                    <label>
                        <span localize="Dec"></span>
                        <input type="text" ng-model="goto.Dec" />
                    </label>
                    <a class="btn" ng-click="gotoCoord()" localize="Go"></a>
                </div>
            </div>
            <div class="search-results">
                <div class="ribbon-thumbs" ng-repeat="item in collectionPage">
                    <ng-include src="'views/thumbnail.html?v=<%=ResourcesVersion%>'"></ng-include>
                </div>
            </div>
            <label class="wwt-pager" ng-show="collection.length > 0">
                <a href="javascript:void(0)" data-ng-disabled="currentPage == 0" ng-click="goBack()">
                    <i class="fa fa-play reverse"></i>
                </a>
                {{(currentPage + 1)}} <span localize="of"></span> {{ pageCount }}
                <a href="javascript:void(0)" ng-disabled="currentPage == pageCount - 1" ng-click="goFwd()">
                    <i class="fa fa-play"></i>
                </a>
            </label>
            <a class="{{expanded ? 'expanded btn tn-expander' : 'btn tn-expander'}}" ng-click="expandThumbnails()">
                <i class="fa fa-caret-down" ng-if="!expanded"></i>
                <i class="fa fa-caret-up" ng-if="expanded"></i>
            </a>
        </div>

        <div
            ng-show="!loadingUrlPlace"
            ng-switch-when="Communities"
            id="communityPanel"
            class="{{expanded ? 'explore-panel rel expanded' : 'explore-panel rel'}}"
            ng-controller="CommunityController"
            >
            <span ng-repeat="bc in breadCrumb" class="bc"><a href="javascript:void(0)" ng-click="breadCrumbClick($index)">{{bc}}</a>&nbsp;>&nbsp;</span><br />
            <div class="explore-thumbs">
                <div class="ribbon-thumbs" ng-repeat="item in collectionPage">
                    <ng-include src="'views/thumbnail.html?v=<%=ResourcesVersion%>'"></ng-include>
                </div>

            </div>
            <label class="wwt-pager">
                <a href="javascript:void(0)" data-ng-disabled="currentPage == 0" ng-click="goBack()">
                    <i class="fa fa-play reverse"></i>
                </a>
                {{(currentPage+1)}} <span localize="of"></span> {{pageCount}}
                <a href="javascript:void(0)" ng-disabled="currentPage == pageCount - 1" ng-click="goFwd()">
                    <i class="fa fa-play"></i>
                </a>
            </label>
            <a class="{{expanded ? 'expanded btn tn-expander' : 'btn tn-expander'}}" ng-click="expandThumbnails()">
                <i class="fa fa-caret-down" ng-if="!expanded"></i>
                <i class="fa fa-caret-up" ng-if="expanded"></i>
            </a>
        </div>
        <div ng-switch-when="View" data-ng-controller="ViewController">
            <div class="layer-manager-toggle">
                <label localize="Use Layer Manager to Control User Settings"></label>
                <a href="javascript:void(0)"
                    class="layer-manager-icon"
                    localize="Show/Hide Layer Manager"
                    localize-only="title"
                    ng-click="toggleLayerManager()">
                    &nbsp;
                </a>
            </div>
            <fieldset>
                <div>
                    <%-- Text intentionally left in elements so localization directive will keep the colon
                        and no need to wrap an additional span inside. Directive will replace with regexp. --%>
                    <label localize="Name">Name:</label>
                    <label>{{locationName}}</label>
                </div>
                <div>
                    <div class="pull-right">
                        <label localize="Alt">Alt:</label>
                        <label>{{UITools.formatDistance(ctl.settings.get_locationAltitude())}}</label>
                    </div>
                    <label localize="Lat">Lat:</label>
                    <label>{{formatHms(ctl.settings.get_locationLat())}}</label>
                </div>
                <div class="clearfix">
                    <label localize="Lng">Lng:</label>
                    <label>{{formatHms(ctl.settings.get_locationLng())}}</label>
                </div>
                <div class="checkbox">
                    <label data-ng-class="viewFromLocation ? 'checked' : ''">
                        <input type="checkbox" ng-model="viewFromLocation" ng-change="setViewFromLocation()" />
                        <span localize="View From This Location"></span>
                    </label>
                </div>
            </fieldset>
            <fieldset>
                <a class="btn" bs-popover
                   localize="Observing Time"
                   localize-only="title"
                   data-content-template="views/popovers/observing-time.html?v=<%=ResourcesVersion%>"
                   ng-controller="ObservingTimeController"
                   data-animation="am-flip-x"
                   data-placement="bottom-right">
                    {{now | date:'yyyy/MM/dd HH:mm:ss'}} &nbsp; <i class="fa fa-caret-down"></i>
                </a>
                <div><label>{{TimeMode}}</label></div>
                <div class="time-buttons">
                    <a class="btn" ng-click="fastBack_Click()">
                        <i class="fa fa-fast-backward"></i>

                    </a>
                    <a class="btn" ng-click="back_Click()">
                        <i class="fa fa-backward"></i>

                    </a>
                    <a class="btn" ng-click="pause_Click()">

                        <i class="fa fa-pause"></i>
                    </a>
                    <a class="btn" ng-click="play_Click()">

                        <i class="fa fa-play"></i>
                    </a>
                    <a class="btn" ng-click="fastForward_Click()" id="btnFastFwd">
                        <i class="fa fa-fast-forward"></i>

                    </a>
                    &nbsp;
                    <a class="btn" ng-click="timeNow_Click()" id="btnTimeNow">Now</a>
                </div>

            </fieldset>
            <fieldset>
                <div class="checkbox">
                    <label data-ng-class="galaxyMode ? 'checked' : ''">
                        <input type="checkbox" ng-model="galaxyMode" ng-change="galaxyModeChange()" />
                        <span data-localize="Galactic Plane Mode">Galactic Plane Mode</span>
                    </label>
                </div>
            </fieldset>
        </div>
        <div ng-switch-when="Settings" ng-controller="SettingsController">
            <fieldset>
                <div class="iblock">
                    <div class="checkbox">
                        <label data-ng-class="crosshairs ? 'checked' : ''">
                            <input type="checkbox" ng-model="crosshairs" data-ng-change="saveSettings()" />
                            <span localize="Reticle/Crosshairs"></span>
                        </label>
                    </div>
                    <div class="checkbox">
                        <label data-ng-class="autoHideTabs ? 'checked' : ''">
                            <input type="checkbox" ng-model="autoHideTabs" data-ng-change="saveSettings()" />
                            <span localize="Auto Hide Tabs"></span>
                        </label>
                    </div>
                    <div class="checkbox">
                        <label data-ng-class="autoHideContext ? 'checked' : ''">
                            <input type="checkbox" ng-model="autoHideContext" data-ng-change="saveSettings()" />
                            <span localize="Auto Hide Context"></span>
                        </label>
                    </div>
                    <div class="checkbox">
                        <label data-ng-class="smoothPanning ? 'checked' : ''">
                            <input type="checkbox" ng-model="smoothPanning" data-ng-change="saveSettings()" />
                            <span localize="Smooth Panning"></span>
                        </label>
                    </div>
                </div>

            </fieldset>

            <fieldset class="bottom-padded">

                <label localize="Preferred Client"></label><br />
                <select ng-change="setClientPref()" ng-model="preferredClient" ng-options="c.code as c.label for c in availableClients"></select>

                <% if (Debug)
                { %>
                <div class="checkbox">
                    <label data-ng-class="WebGl ? 'checked' : ''">
                        <input type="checkbox" ng-model="WebGl" data-ng-change="setWebGl()" />
                        <span localize="Use WebGL (if available)"></span>
                    </label>
                </div>
                <%  } %>
                <div ng-if="redirecting">
                    <span localize="Redirecting to Silverlight Client in"></span>
                    {{redirectingSeconds}}
                    <span localize="seconds"></span>

                    <a class="btn" ng-click="cancelRedir()">
                        <i class="fa fa-stop"></i>
                        <span localize="Stop"></span>
                    </a>
                </div>
            </fieldset>
            <fieldset class="bottom-padded">
                <label localize="Select Your Language"></label><br />
                <select ng-change="setLanguageCode(selectedLanguage)" ng-model="selectedLanguage" ng-options="l.code as l.label for l in availableLanguages"></select>
            </fieldset>
        </div>
        <% if (ADS)
           {%>
        <div ng-init="initAds()"
            ng-switch-when="ADS"
            ng-controller="ADSController"
            >
            <fieldset class="radio-buttons">
                <div class="iblock">
                    <h5 localize="Heatmap Options - Objects"></h5>
                    <div class="iblock">
                        <div class="checkbox">
                            <label data-ng-class="fgImagery == 'All' ? 'checked' : ''">
                                <input type="radio" name="fgImagery" ng-model="fgImagery" value="All" ng-change="adsChange()" />
                                <span localize="All"></span>
                            </label>
                        </div>
                        <div class="checkbox">
                            <label data-ng-class="fgImagery == 'Star' ? 'checked' : ''">
                                <input type="radio" name="fgImagery" ng-model="fgImagery" value="Star" ng-change="adsChange()" />
                                <span localize="Stars"></span>
                            </label>
                        </div>
                        <div class="checkbox">
                            <label data-ng-class="fgImagery == 'Galaxy' ? 'checked' : ''">
                                <input type="radio" name="fgImagery" ng-model="fgImagery" value="Galaxy" ng-change="adsChange()" />
                                <span localize="Galaxies"></span>
                            </label>
                        </div>
                    </div>
                    <div class="iblock">
                        <div class="checkbox">
                            <label data-ng-class="fgImagery == 'HII regions' ? 'checked' : ''">
                                <input type="radio" name="fgImagery" ng-model="fgImagery" value="HII regions" ng-change="adsChange()" />
                                <span localize="HII regions"></span>
                            </label>
                        </div>
                        <div class="checkbox">
                            <label data-ng-class="fgImagery == 'Nebula' ? 'checked' : ''">
                                <input type="radio" name="fgImagery" ng-model="fgImagery" value="Nebula" ng-change="adsChange()" />
                                <span localize="Nebulae"></span>

                            </label>
                        </div>
                        <div class="checkbox">
                            <label data-ng-class="fgImagery == 'Other' ? 'checked' : ''">
                                <input type="radio" name="fgImagery" ng-model="fgImagery" value="Other" ng-change="adsChange()" />
                                <span localize="Other"></span>
                            </label>
                        </div>
                    </div>

                </div>
                <div class="iblock rel">

                    <h5  style="margin-left:24px;" localize="Bands"></h5>
                    <div class="iblock" style="margin-left:12px;padding-left:12px;border-left:solid 1px #728f9a">
                        <div class="checkbox">
                            <label data-ng-class="fgImagery == 'Radio' ? 'checked' : ''">
                                <input type="radio" name="fgImagery" ng-model="fgImagery" value="Radio" ng-change="adsChange()" />
                                <span localize="Radio"></span>
                            </label>
                        </div>
                        <div class="checkbox">
                            <label data-ng-class="fgImagery == 'Infrared' ? 'checked' : ''">
                                <input type="radio" name="fgImagery" ng-model="fgImagery" value="Infrared" ng-change="adsChange()" />
                                <span localize="Infrared"></span>
                            </label>
                        </div><div class="checkbox">
                            <label data-ng-class="fgImagery == 'year' ? 'checked' : ''">
                                <input type="radio" name="fgImagery" ng-model="fgImagery" value="year" ng-change="adsChange()" />
                                <span localize="Year (use slider)"></span>
                            </label>
                        </div>

                    </div>
                    <div class="iblock" style="vertical-align: top">
                        <div class="checkbox">
                            <label data-ng-class="fgImagery == 'X-ray' ? 'checked' : ''">
                                <input type="radio" name="fgImagery" ng-model="fgImagery" value="X-ray" ng-change="adsChange()" />
                                <span localize="X-ray"></span>
                            </label>
                        </div>
                        <div class="checkbox">
                            <label data-ng-class="fgImagery == 'Ultraviolet' ? 'checked' : ''">
                                <input type="radio" name="fgImagery" ng-model="fgImagery" value="Ultraviolet" ng-change="adsChange()" />
                                <span localize="Ultraviolet"></span>
                            </label>
                        </div>
                        <div class="control" style="width:200px">
                            <div class="year-slider">
                                <a class="btn" style="left:55px">&nbsp;</a>
                            </div>
                            &nbsp;&nbsp;{{year}}
                        </div>

                    </div>
                </div>
            </fieldset>



        </div>
           <%  } %>
        <div
            ng-show="!loadingUrlPlace"
            ng-switch-when="currentTour"
            id="currentTourPanel"
            class="explore-panel rel curtour-panel"
            ng-controller="CurrentTourController"
            ng-init="init(tour)"
            >

            <table>
                <colgroup>
                    <col style="width:80px"/>
                    <col style="width:100%"/>
                    <col ng-style="{'width':editingTour ? 374 : 1 }"/>
                </colgroup>
                <tr>
                    <td class="play-tour">
                        <a class="btn-play-tour" ng-click="playButtonClick()">
                            <i class="fa fa-play" ng-if="!tourEdit.playing"></i>
                            <div class="paused" ng-if="tourEdit.playing">
                                <i class="fa fa-minus"></i>
                                <i class="fa fa-minus"></i>
                            </div>
                        </a><br/>
                        <div class="small" localize="Run Time"></div>
                        <div class="small">{{ !tour.minuteDuration ? '00' : tour.minuteDuration > 9 ? tour.minuteDuration : '0' + tour.minuteDuration}}:{{ !tour.secDuration ? '00' : tour.secDuration > 9 ? tour.secDuration : '0' + tour.secDuration}}</div>

                    </td>
                    <td class="tour-stops">
                        <div class="scroller" data-jquery-scrollbar>
                            <div class="stops-container">
                                <a class="btn" bs-popover
                                    style="position:absolute;top:110px;left:190px;visibility:hidden"
                                    template-url="views/popovers/tour-properties.html?v=<%=ResourcesVersion%>"
                                    trigger="click" placement="bottom-left" data-content="{tour}"
                                    data-container="body" id="newTourProps"></a>
                                <div class="stop-arrow" ng-repeat="stop in tourStops">
                                    <div class="transition-choice {{stop.transHover ? 'active' : ''}}"
                                        bs-popover template-url="views/popovers/transition-type.html?v=<%=ResourcesVersion%>"
                                        trigger="click" placement="bottom-left" data-content="{1}"
                                        title="Transition" data-container="body" data-auto-close="true"
                                        data-on-show="testfn()" data-on-hide="testfn()">
                                        <%-- slew --%>
                                        <div class="right-arrow choice" ng-if="stop.transitionType==0">
                                            <i class="arrow-line"></i><i class="arrow-head"></i>
                                        </div>
                                        <%-- xfade --%>
                                        <div class="crossfade choice" ng-if="stop.transitionType==1">
                                            <span class="shape"></span>
                                        </div>
                                        <%-- xcut --%>
                                        <div class="abrupt choice" ng-if="stop.transitionType==2">
                                            <span class="shape"></span>
                                        </div>
                                        <%-- fadeoutin --%>
                                        <div class="fadeout-in choice" ng-if="stop.transitionType==3">
                                            <span class="shape left"></span>
                                            <span class="shape right"></span>
                                        </div>
                                        <%-- fadein --%>
                                        <div class="fadein choice" ng-if="stop.transitionType==4">
                                            <span class="shape"></span>
                                        </div>
                                        <%-- fadeout --%>
                                        <div class="fadeout choice" ng-if="stop.transitionType==5">
                                            <span class="shape"></span>
                                        </div>

                                    </div>
                                    <div class="thumbwrap">
                                        <span class="master-slide" ng-if="stop.isMaster">M</span>
                                        <span class="slide-number" ng-if="slideNumbering">{{$index}}</span>
                                        <div class="stop-thumb thumbnail {{(tourEdit.tourStopList.selectedItems[$index]) || (activeIndex == $index) ? 'active' : ''}}"
                                            index="{{$index}}"
                                            ng-click="selectStop($index, $event)"
                                            ng-dblclick="showStartCameraPosition($index)"
                                            ng-context-menu="showContextMenu">
                                            <a class="ear stop-start{{tourEdit.playing && activeIndex === $index && stop._tweenPosition < .5 ? ' active' : ''}}" ng-click="showStartCameraPosition($index)" ng-show="(editingTour ||tourEdit.playing) && $index == activeIndex"></a>
                                            <a class="ear stop-end{{tourEdit.playing && activeIndex === $index && stop._tweenPosition >= .5 ? ' active' : ''}}" ng-click="showEndCameraPosition($index)" ng-show="(editingTour ||tourEdit.playing) && $index == activeIndex"></a>
                                            <img ng-src="{{stop.thumb.src}}" alt="{{stop.description}}"/>
                                            <label class="slide-label" contenteditable="true" bs-tooltip data-title="{{stop.description}}" placement="bottom" container="body"></label>
                                            <label class="duration" contenteditable="true"></label>
                                            <a class="btn tinybutton duration-up" ng-if="stop.editingDuration">
                                                <i class="fa fa-caret-up"></i>
                                            </a>
                                            <a class="btn tinybutton duration-down" ng-if="stop.editingDuration">
                                                <i class="fa fa-caret-down"></i>
                                            </a>
                                        </div>
                                    </div>

                                </div>
                                <div class="stop-arrow" ng-if="editingTour">
                                    <div class="transition-choice invisible"></div>
                                    <div class="thumbwrap">
                                        <div class="stop-thumb thumbnail"
                                            ng-click="tourEdit.addSlide(false);refreshStops()">
                                            <label class="slide-label" localize="Add New Slide" style="position:relative;top:20px;width:90px;text-align:center"></label>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>

                    </td>
                    <td class="edit-panel">
                        <div ng-if="editingTour" class="edit-panel">
                            <script src="//cdnjs.cloudflare.com/ajax/libs/tinymce/4.4.1/tinymce.min.js"></script>
                            <link href="css/skin.min.css" rel="stylesheet" />
                            <div class="left">
                                <a class="btn" bs-popover
                                    localize="Tour Properties" style="width:98px;"
                                    template-url="views/popovers/tour-properties.html?v=<%=ResourcesVersion%>"
                                    trigger="click" placement="bottom-left" data-content="{tour}"
                                    title="Tour Properties" data-container="body"></a>
                                <a class="btn" localize="Save" style="width:48px;" ng-click="saveTour()"></a>

                                <div>
                                    <a class="btn menu-button text" bs-modal template-url="views/popovers/tour-text.html?v=<%=ResourcesVersion%>"
                                    trigger="click" data-content="{tour}" id="editTourText" placement="center"
                                    title="Enter Text" data-container="body">
                                        <div class="icon">
                                            <i class="A">A</i>
                                            <i class="tbar"></i>
                                            <i class="tbar"></i>
                                            <i class="tbar longbar"></i>
                                        </div>
                                        <label localize="Text">Text</label>
                                    </a>
                                    <a class="btn menu-button shape" bs-dropdown aria-haspopup="true" aria-expanded="false">
                                        <i class="sq"></i><i class="circ"></i>
                                        <label localize="Shape"></label>
                                        <span class="fa fa-caret-down"></span>
                                    </a>
                                    <ul class="dropdown-menu" role="menu">
                                        <li><a href="javascript:void(0)" ng-click="addShape(0)">Circle</a></li>
                                        <li><a href="javascript:void(0)" ng-click="addShape(1)">Rectangle</a></li>
                                        <li><a href="javascript:void(0)" ng-click="addShape(6)">Open Rectangle</a></li>
                                        <li><a href="javascript:void(0)" ng-click="addShape(3)">Ring</a></li>
                                        <li><a href="javascript:void(0)" ng-click="addShape(5)">Line</a></li>
                                        <li><a href="javascript:void(0)" ng-click="addShape(4)">Arrow</a></li>
                                        <li><a href="javascript:void(0)" ng-click="addShape(2)">Star</a></li>
                                    </ul>
                                    <a class="btn menu-button picture" ng-click="launchFileBrowser('addPicture')">
                                        <i class="fa fa-photo"></i>
                                        <label localize="Picture"></label>
                                    </a>
                                    <input type="file" id="addPicture" onchange="angular.element(this).scope().mediaFileChange(event,'image',true)" style="position:absolute"/>
                                </div>
                            </div>
                            <div class="right">
                            <div class="sound-container">
                                <label localize="Music:"></label><label style="width:160px;overflow:hidden;white-space:nowrap;position:relative;top:4px" title="{{activeSlide.music ? activeSlide.music.name : ''}}">&nbsp;
                                    {{activeSlide.music ? activeSlide.music.name : ''}}</label>
                                <div>
                                    <a class="btn remove" localize="Remove" ng-if="activeSlide.music" ng-click="activeSlide.music = activeSlide._musicTrack = null"></a>
                                    <input type="file" class="audiofile{{activeSlide.music?' has-file':''}}" id="musicFile" localize="Browse..." onchange="angular.element(this).scope().mediaFileChange(event,'music')"  />
                                    <a class="browse btn" localize="Browse..." ng-if="!activeSlide.music"  ng-click="launchFileBrowser('musicFile')"></a>
                                    <a class="btn{{!activeSlide.music?' disabled' : ''}}" ng-click="activeSlide.music.mute(!activeSlide.music.muted)" ng-disabled="!activeSlide.music">
                                        <i class="fa fa-volume-up" ng-if="!activeSlide.music || !activeSlide.music.muted"></i>
                                        <i class="fa fa-ban" ng-if="activeSlide.music && activeSlide.music.muted"></i>
                                    </a>
                                    <div class="sound-level  {{!activeSlide.music || activeSlide.music.muted ? 'disabled' : ''}}">
                                        <a class="btn  {{!activeSlide.music || activeSlide.music.muted ? 'disabled' : ''}}" ng-style="{left: activeSlide.music ? activeSlide.music.vol : 50, position: 'absolute'}" id="musicVol">&nbsp;</a>
                                    </div>
                                </div>
                            </div>
                            <div class="sound-container">
                                <label localize="Voiceover:"></label><label style="width:143px;overflow:hidden;white-space:nowrap;position:relative;top:4px" title="{{activeSlide.voice ? activeSlide.voice.name : ''}}">&nbsp;
                                    {{activeSlide.voice ? activeSlide.voice.name : ''}}</label>
                                <div>
                                    <a class="btn remove" localize="Remove" ng-if="activeSlide.voice" ng-click="activeSlide.voice = activeSlide._voiceTrack = null"></a>

                                    <input type="file" id="voiceFile" class="audiofile" localize="Browse..." onchange="angular.element(this).scope().mediaFileChange(event,'voiceOver')"  />
                                    <a class="browse btn" localize="Browse..." ng-if="!activeSlide.voice"  ng-click="launchFileBrowser('voiceFile')"></a>

                                    <a class="btn{{!activeSlide.voice?' disabled' : ''}}" ng-click="activeSlide.voice.mute(!activeSlide.voice.muted)" ng-disabled="!activeSlide.voice">
                                        <i class="fa fa-volume-up" ng-if="!activeSlide.voice || !activeSlide.voice.muted"></i>
                                        <i class="fa fa-ban" ng-if="activeSlide.voice && activeSlide.voice.muted"></i>
                                    </a>
                                    <div class="sound-level {{!activeSlide.voice || activeSlide.voice.muted ? 'disabled' : ''}}">
                                        <a class="btn {{!activeSlide.voice || activeSlide.voice.muted ? 'disabled' : ''}}"
                                            ng-style="{left: activeSlide.voice ? activeSlide.voice.vol : 50, position: 'absolute'}" id="voiceVol">&nbsp;</a>
                                    </div>
                                </div>

                            </div>

                            </div>
                        </div>
                    </td>
                </tr>
            </table>

            <ng-include src="'views/popovers/slide-overlays.html?v=<%=ResourcesVersion%>'"></ng-include>

        </div>
    </div>
    <div class="layer-manager desktop"
     ng-controller="LayerManagerController" ng-style="{display: layerManagerHidden ? 'none' : 'block',height:  layerManagerHeight() + 28}" ng-init="initLayerManager()">

      <button
      aria-hidden="true"
      class="close pull-right" type="button" ng-click="toggleLayerManager()"
      style="font-weight: 100;font-size: 16px">x</button>
      <h5 localize="Layers"></h5>
       <div class="tree" ng-style="{ height: layerManagerHeight()-90 }">
         <div
            ng-class="sunTree.Sun.collapsed ? 'collapsed' : ''"
            ng-repeat="(name,node) in sunTree"
            ng-include="'tree-node'">
         </div>
          <i class="fa"
                 ng-class="collapsed(tree) ? 'fa-plus-square-o' : 'fa-minus-square-o'"
                 ng-click="collapse(tree)"></i>

         <div class="checkbox" ng-right-click="showMenu(skyNode,$event)" ng-class="{activelayer:skyNode.active}">
           <label data-ng-class="{checked:tree.checked}" localize="Sky" ng-click="selectionChanged(tree,$event)"
                  localize-only="title">
             <input type="checkbox" ng-model="tree.checked"/>
             <span localize="Sky"></span>
           </label>

         </div>
         <div class="indent"
            ng-class="collapsed(tree) ? ' collapsed' : ''"
            ng-repeat="(name,node) in getChildren(tree)"
            ng-include="'tree-node'">
        </div>
      </div>
      <div class="time-scrubber">
        <!--<table class="table">
        <colgroup>
          <col width="30%"/>
          <col width="55%"/>
          <col width="25%"/>
        </colgroup>
        <thead>
          <tr>
            <th localize="Name"></th>
            <th localize="Value"></th>
            <th>&nbsp;</th>
          </tr>
        </thead>
        <tbody>
        <tr ng-repeat="i in scrubber.table">
          <td>&nbsp;</td>
          <td>&nbsp;</td>
          <td>&nbsp;</td>
        </tr>
        </tbody>
        </table>-->
        <h5 localize="Time Scrubber" id="timeScrubberLabel" ng-if="!scrubber.title"></h5>
        <h5 ng-if="scrubber.title">{{scrubber.title}}</h5>

        <div class="control">
          <span class="pull-right" id="scrubberRightLabel">{{scrubber.right}}&nbsp;</span>
          <span id="scrubberLeftLabel">{{scrubber.left}}&nbsp;</span>
          <div class="scrubber-slider">
              <a class="btn"></a>
          </div>

          <div class="checkbox iblock" ng-if="activeLayer && activeLayer.timeSeriesChecked != undefined" style="margin:5px 15px 35px 0;">
            <label data-ng-class="activeLayer.timeSeries ? 'checked' : ''">
              <input type="checkbox" ng-model="activeLayer.timeSeriesChecked" ng-change="setTimeSeries(activeLayer, activeLayer.timeSeriesChecked)" />
              <span localize="Time series"></span>
            </label>
          </div>
          <div class="checkbox iblock" ng-if="activeLayer && activeLayer.timeSeriesChecked != undefined" style="margin-top:15px;">
            <label data-ng-class="activeLayer._autoUpdate$1 ? 'checked' : ''">
              <input type="checkbox" ng-model="activeLayer.loopChecked" ng-change="setAutoLoop(activeLayer,activeLayer.loopChecked)" />
              <span localize="AutoLoop"></span>
            </label>
          </div>
        </div>
      </div>

    </div>


    <div ng-class="contextPanelClass()">
        <div ng-class="fovClass()">
            <a
                class="btn" tabindex="0"
                data-bs-popover="popover"
                ng-if="!showTrackingString()"
                style="position:absolute; top:6px;left:-33px;z-index:3"
                localize="Share this view"
                localize-only="title"
                data-content-template="views/popovers/shareplace.html?v=<%=ResourcesVersion%>"
                data-container="body"
                data-placement="top-right"
                >
                <i class="fa fa-share-alt"></i>
            </a>
            <div class="left" ng-if="lookAt != 'SolarSystem'">
                <p localize="N"></p>
                <div class="sphere" id="skyball">
                    <div class="v-ellipse"></div>
                    <div class="h-ellipse"></div>
                    <div class="x-axis"></div>
                    <div class="y-axis"></div>
                </div>
                <p><span localize="RA"></span>: {{formatted.RA}}</p>
                <p><span localize="Dec"></span>: {{formatted.Dec}}</p>
            </div>
            <div class="left" ng-if="lookAt == 'SolarSystem'">
                <div class="control">
                    <label localize="Planet Size"></label>
                    <div class="planetary-scale">
                        <a class="btn"></a>
                    </div>

                </div>
                <label class="pull-right" localize="Large"></label>
                <label class="pull-left" localize="Actual"></label>
                <div class="location">
                    <p><span localize="Lat"></span>: {{formatted.Lat}}</p>
                    <p><span localize="Lng"></span>: {{formatted.Lng}}</p>
                </div>
            </div>
            <div class="right">
                <p style="position: relative;top:3px;">
                    {{formatted.Constellation}}
                    <span class="pull-right">{{formatted.Zoom}}</span>
                </p>
                <div class="constellation-viewport" ng-click="gotoConstellation(singleton.constellation)">
                    <img ng-src="{{constellations.constellationCentroids[singleton.constellation].get_thumbnailUrl()}}" />
                </div>
                <div class="bar"></div>
            </div>
        </div>
        <div class="controls" ng-style="{ width: bottomControlsWidth() + 'px'}">
            <div class="layer-manager-toggle iblock" id="btnToggleLayerMgr">

                <a href="javascript:void(0)"
                    class="layer-manager-icon"
                    localize="Show/Hide Layer Manager"
                    localize-only="title"
                    ng-click="toggleLayerManager()">
                    &nbsp;
                </a>
            </div>
            <div class="control">
                <label localize="Look At"></label>
                <select id="lstLookAt"
                        ng-init="lookAt = 'Sky'"
                        ng-model="lookAt"
                        ng-change="lookAtDropdownChanged()"
                        ng-options="type for type in lookTypes"></select>
            </div>
            <div class="control">
                <label localize="Imagery"></label>
                <select id="lstImagery"
                    ng-init="backgroundImagery.name = 'Digitized Sky Survey (Color)'"
                    ng-model="backgroundImagery"
                    ng-change="setSurveyBg()"
                    ng-options="s.name for s in surveys">
                    <option value="?">&nbsp;</option>
                </select>
            </div>

            <div ng-show="showCrossfader()" class="control" style="padding-right:10px">
                <label localize="Image Crossfade"></label>
                <div class="cross-fader">
                    <a class="btn">&nbsp;</a>
                </div>

            </div>

        </div>
        <div class="thumbnails nearby-objects rel" data-ng-controller="ContextPanelController" ng-style="{width: bottomControlsWidth()}">
            <div class="rel" style="display: inline-block;vertical-align:top;" ng-repeat="item in collectionPage" ng-if="lookAt != 'Planet' && lookAt != 'Panorama'">
                <ng-include src="'views/thumbnail.html?v=<%=ResourcesVersion%>'"></ng-include>
            </div>
            <label class="wwt-pager">
                <a class="btn"
                    ng-if="(lookAt == 'Planet' || lookAt == 'Panorama' || lookAt == 'Earth' ) && !trackingObj"
                    data-bs-popover="popover" tabindex="0"
                    style="position:absolute; top:0;right:-204px"
                    localize="Share this view"
                    localize-only="title"
                    data-content-template="views/popovers/shareplace.html?v=<%=ResourcesVersion%>"
                    data-container="body"
                    data-placement="top-right"
                    >
                    <i class="fa fa-share-alt"></i>
                </a>
                <div class="iblock tracking rel" ng-if="showTrackingString()" style="vertical-align: top;">
                    <div localize="Tracking"></div>
                    <div title="{{trackingObj.get_name()}}">{{trackingObj.get_name()}}</div>
                    <a class="btn" data-bs-popover="popover" tabindex="0"
                        style="position:absolute; top:0;left:-40px"
                        localize="Share this place"
                        localize-only="title"
                        data-content-template="views/popovers/shareplace.html?v=<%=ResourcesVersion%>"
                        data-container="body"
                        data-placement="top-right"
                        >
                        <i class="fa fa-share-alt"></i>
                    </a>
                </div>
                <div class="iblock rel vtop" ng-style="{right:contextPagerRight() + 'px'}" ng-if="lookAt != 'Planet' && lookAt != 'Panorama' && pageCount > 1">
                    <a href="javascript:void(0)" data-ng-disabled="currentPage == 0" ng-click="goBack()">
                        <i class="fa fa-play reverse"></i>
                    </a>
                    {{(currentPage+1)}} <span localize="of"></span> {{pageCount}}
                    <a href="javascript:void(0)" ng-disabled="currentPage == pageCount - 1" ng-click="goFwd()">
                        <i class="fa fa-play"></i>
                    </a>
                </div>
            </label>
        </div>

    </div>

<ng-include src="'views/modals/intro.html?v=<%=ResourcesVersion%>'"></ng-include>
<div class="modal" id="loadingModal" tabindex="-1" role="dialog" aria-labelledby="loadingModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">

            <div class="modal-body">
                <img src='<%=ImgDir %>wwtlogo.png?v=<%= ResourcesVersion%>'
                    style="width:19%;height:19%;position:relative;left:-3px;margin-right:12px;"
                    class="pull-left"
                    localize="WorldWide Telescope Logo"
                    localize-only="alt" />
                <h1 style="position:relative;top:-2px">
                    World<span style="color:#6ba9e6">Wide Telescope</span>
                </h1>
                <p>
                    <i class="fa fa-spin fa-spinner"></i>
                    <span localize="Content Loading. Please Wait..."></span>
                </p>


            </div>

        </div>
    </div>
</div>
<div class="modal" id="folderLoadingModal" tabindex="-1" role="dialog" aria-labelledby="folderLoadingLabel" aria-hidden="true" data-backdrop="static"
   data-keyboard="false" >
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-body">
                <h4>
                    <i class="fa fa-spin fa-spinner"></i>
                    <span localize="Loading folder contents. Please Wait..."></span>
                </h4>
            </div>
        </div>
    </div>
</div>

<a href="javascript:void(0)" data-toggle="modal" data-target="#loadingModal" id="loadingModalLink">&nbsp;</a>
    <ng-include src="'views/modals/open-item.html?v=<%=ResourcesVersion%>'"></ng-include>
    <div ng-intro-autostart="false"
        ng-intro-onbeforechange="beforeChange"
        ng-intro-onafterchange="afterChange"
        ng-intro-onexit="exit"
        ng-intro-oncomplete="completed"
        ng-intro-method="startIntro"
        ng-intro-options="options"
        ng-controller="IntroController" class="hide">
        <a ng-click="startIntro()" id="introStartButton"></a>
    </div>
    <%  } %>
</div>
    <% }
       else if (Client == Clients.Silverlight)
       { %>

    <div id="silverlightControlHost">
        <object id = "_wwt_application" data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="100%">
            <param name="source" value="clientbin/WWTSL.xap"/>
            <param name="onerror" value="onSilverlightError" />
            <param name="background" value="gray" />
            <param name="minRuntimeVersion" value="4.0.41108.0" />
            <param name="autoUpgrade" value="true" />
            <param name="initParams" value="wtml=<% Page.Response.Output.Write(Request.Params["wtml"]); %>,tour=<% Page.Response.Output.Write(Request.Params["tour"]); %>,webkey=<% Page.Response.Output.Write(ConfigurationManager.AppSettings["webkey"]); %>" />
            <div style="text-align:center;font-family:Arial;margin-top:50px;">
            This page requires Silverlight 4.<br />
            <br />
            <a href="//go.microsoft.com/fwlink/?LinkID=149156&v=4.0.41108.0" style="text-decoration:none">
              <img src="//go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight" style="border-style:none"/>
            </a>
            </div>
        </object>
        <iframe style='visibility:hidden;height:0;width:0;border:0'></iframe>
    </div>
    <% } %>

</body>
</html>
