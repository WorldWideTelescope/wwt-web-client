<%@ Page Language="C#" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Security.Cryptography.X509Certificates" %>
<%@ Import Namespace="WURFL" %>
<script runat="server">
    public bool Debug = false;
    public bool DebugChrome = false;
    public bool ADS = false;
    // This enables the webclient to load in both the worldwidetelescope.org site
    // with js/css/imagery on the cdn and in standalone mode without a cdn
    public string ResourcesLocation;
    public string ResourcesVersion = ConfigurationManager.AppSettings["ResourcesVersion"];
    public string DebugQs = "?v=" + ConfigurationManager.AppSettings["ResourcesVersion"];
    public string BodyClass;
    public string SDKLocation = "http://localhost:26993/scripts/wwtsdk.aspx?debug=true";

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
        var device = WURFLManagerBuilder.Instance.GetDeviceForRequest(Request.UserAgent);

        ResourcesLocation = ConfigurationManager.AppSettings["WebClientResourcesLocation"] ?? ConfigurationManager.AppSettings["ResourcesLocation"] + "/webclient";
        bool isMobile = device.GetCapability("is_smartphone") == "true";

        BodyClass = string.Format("fs-player wwt-webclient-wrapper {0}", isMobile ? "mobile" : "desktop");

        if (Request.QueryString["debug"] != null)
        {
            DebugQs = "?debug=true&v=" + ResourcesVersion;
            Debug = true;
            if (Request.QueryString["debug"] == "chrome")
            {
                Debug = false;
                DebugChrome = true;
                DebugQs = "";
            }
            else if (Request.QueryString["debug"] == "local")
            {
                SDKLocation = "sdk/wwtsdk.aspx" + DebugQs;
            }
            else if (Request.QueryString["debug"] == "localmin")
            {
                SDKLocation = "wwtsdk.min.js" + DebugQs;
            }
            else
            {
                SDKLocation += "?debug=true";
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
        if (Request.QueryString["client"] != null)
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

        if (Client == Clients.Html5 && isMobile)
        {
            Response.Redirect(string.Format("/webclient/?client=mobile{0}", Debug ? "&debug=true" : ""));
        }
        else if (Client == Clients.Mobile && !isMobile)
        {
            Response.Redirect(string.Format("/webclient/?client=html5{0}", Debug ? "&debug=true" : ""));
        }
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
    <meta property="og:url" content="http://worldwidetelescope.org/webclient" /> 
    <meta property="og:title" content="WorldWide Telescope Web Client" />
    <meta property="og:description" content="Worldwide Telescope enables your computer to function as a virtual telescope, bringing together imagery from the best earth and space-based telescopes." /> 
    <meta property="og:image" content="http://worldwidetelescope.org/webclient/Images/wwtlogo.png" /> 
    <link rel="icon" href="favicon.ico"/>
    <% if (Client == Clients.Html5 || Client == Clients.Mobile)
       { 
        string css = "<link href=\"css/webclient.min.css?v="+ResourcesVersion+"\" rel=\"stylesheet\" />";
        css+="<link href=\"css/angular-motion.css?v="+ResourcesVersion+"\" rel=\"stylesheet\" />";
        css+="<link href=\"ext/introjs.css?v="+ResourcesVersion+"\" rel=\"stylesheet\" />";
        %>
    <%=css%>
    
    <link href="//maxcdn.bootstrapcdn.com/font-awesome/4.2.0/css/font-awesome.min.css" rel="stylesheet"/>

    <style> 
        html, body.fs-player, iframe {
            height: 100%;
            width: 100%;   
            margin: 0;
            padding: 0;
            overflow: hidden;
        } 
        .finder-scope {
            background: url(Images/finder-scope.png?v=<%= ResourcesVersion %>) no-repeat;
        }

        body .contextmenu {
            background: rgba(25,30,43,.88);
            position: absolute;
            display: none;
            width: 240px;
            top: 400px;
            left: 400px;
            z-index: 1000;
            box-shadow: 0 0 4px #808080;
            background-clip: border-box;
            padding-left: 15px;
            border: solid 1px rgba(105,115,150,.7);
        }

        body .contextmenuitem {
            border-left: solid 1px rgba(105,115,150,.7);
            padding: 2px 4px 2px 4px;
            -webkit-touch-callout: none;
            -webkit-user-select: none;
            -khtml-user-select: none;
            -moz-user-select: none;
            -ms-user-select: none;
            user-select: none;
        }

        body .contextmenuitem:hover {
            background-color: rgba(1,14,23,.85);
        }

        body .submenu:after {
              display: block;
              right: 6px;
              content: " ";
              float: right;
              width: 0;
              height: 0;
              border-color: transparent;
              border-style: solid;
              border-width: 5px 0 5px 5px;
              border-left-color: #cccccc;
              margin-top: 5px;
              margin-right: 10px;
            }


           body .checkedmenu:after {
                font-family: FontAwesome;
                content: "\f00c";
                font-size: 13px;
                color: rgba(255,255,255,.8);
                text-align: center;
                line-height: 15px;
                height: 15px;
                width: 15px;
                position: absolute;
                left: 0;
            }

          body .colorpicker {
                background: rgba(25,30,43,.88);
                position: absolute;
                display: none;
                width: 220px;
                height: 240px;
                top: 400px;
                left: 400px;
                z-index: 1000;
                box-shadow: 0 0 4px #808080;
                background-clip: border-box;
                padding-left: 15px;
                padding-top: 15px;
                border: solid 1px rgba(105,115,150,.7);
            }


    </style> 
     
    <script src="//js.live.net/v5.0/wl.js"></script>
    <script src="sdk/wwtsdk.js"></script>
    <script src="//code.jquery.com/jquery-2.1.4.min.js"></script>
    <script src="//maxcdn.bootstrapcdn.com/bootstrap/3.3.5/js/bootstrap.min.js"></script>
    <script src="//cdnjs.cloudflare.com/ajax/libs/angular.js/1.5.5/angular.min.js"></script>
    <script src="//cdnjs.cloudflare.com/ajax/libs/angular.js/1.5.5/angular-touch.min.js"></script>
    <script src="//cdnjs.cloudflare.com/ajax/libs/angular.js/1.5.5/angular-route.min.js"></script>
    <script src="//cdnjs.cloudflare.com/ajax/libs/angular.js/1.5.5/angular-cookies.min.js"></script>
    <script src="//cdnjs.cloudflare.com/ajax/libs/angular.js/1.5.5/angular-animate.min.js"></script>
    <script src="//cdnjs.cloudflare.com/ajax/libs/angular-strap/2.3.8/angular-strap.js"></script>
    <script src="//cdnjs.cloudflare.com/ajax/libs/angular-strap/2.3.8/angular-strap.tpl.min.js"></script>
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
    <script src="<%= ResourcesLocation %>/directives/ContextMenu.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/appstate.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/localization.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/FinderScope.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/ThumbList.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/Util.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/UILibrary.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/SearchUtil.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/Skyball.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/factories/HashManager.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/ContextPanelController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/IntroController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/LayerManagerController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/MainController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/MobileNavController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/modals/ObservingTimeController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/modals/ShareController.js?v=<%= ResourcesVersion%>"></script>
    <script src="<%= ResourcesLocation %>/controllers/modals/OpenItemController.js?v=<%= ResourcesVersion%>"></script>
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
    <script src="<%= ResourcesLocation %>/wwtwebclient.min.js?v=<%= ResourcesVersion%>"></script>
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
           src="http://static.ak.connect.facebook.com/js/api_lib/v0.4/FeatureLoader.js.php"
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

    <div id="colorpicker" class="colorpicker">
        <img id="colorhex" src="images\ColorPickerHex.png" width="189" height="208" />
    </div>
   

<% if (Client == Clients.Mobile)
   { %>
    
    <a  data-bs-popover="popover" tabindex="0"
        localize="Share this place"
        localize-only="title"
        data-content-template="views/popovers/shareplace.html"
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
            data-content-template="views/popovers/shareplace.html"
            data-container="body"
            data-placement="bottom-left"
            >
            <i class="fa fa-share-alt"></i>
            <span localize="Share"></span>
        </a>
    </div>
    <ng-include src="'views/modals/mobile-explore.html'"></ng-include>
    
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
                        <a ng-click="gotoPage('/')" target="_blank" localize="WorldWide Telescope Home"></a>
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
        <img src='<%= ResourcesLocation%>/Images/wwtlogo.png' 
            class="pull-left" 
            localize="WorldWide Telescope Logo"
            localize-only="alt"
         />
        <h3>
            
            World<span class="brand-blue">Wide Telescope</span>
        </h3>
        <h4 localize="Welcome to the WorldWide Telescope Web Client"></h4>
        <p>
            <i class="fa fa-spin fa-spinner"></i>
            <span localize="WorldWide Telescope is loading. Please wait."></span>
            
        </p>
        <p class="small" localize="Please ensure you have a strong connection to the internet for the best experience.">
            (Please ensure you have a strong connection to the internet for the best experience.)
        </p>
    </div>
    <ng-include src="'views/modals/mobile-nearby-objects.html'"></ng-include>
    <div class="context-panel">
        <div class="nearby-objects" ng-if="nbo.length && lookAt == 'Sky'" ng-show="!tourPlaying">
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
    <ng-include src="'views/research-menu.html'"></ng-include>
    <ng-include src="'views/modals/finder-scope.html'" onload="initFinder()"></ng-include>
    
    
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
            <li data-ng-class="activePanel == 'currentTour' ? 'active' : 'hide'">
                <div class="outer">
                    <a href="javascript:void(0)">
                        <span class="label" data-ng-click="tabClick(tab)" id="btnCurTour" localize="{{currentTour._title}}"></span>
                        <%--<div class="menu" data-ng-click="menuClick(tab.menu)" id="tabMenu{{ribbon.tabs.length}}" data-target="#menu{{ribbon.tabs.length}}">
                            <i class="fa fa-caret-down"></i>
                        </div>--%>
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
                    <ng-include src="'views/thumbnail.html'"></ng-include>
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
                <%--<a ng-if="$index==currentPage * pageSize" id="popTrigger"
                   data-title="{{tour.get_name()}}"
                   bs-popover="popover" data-placement="bottom-left"
                   data-content-template="views/popovers/tour-info.html"
                   data-template="views/popovers/tour-template.html"
                   data-trigger="hover" data-animation="am-fade"
                   data-delay="200">
                    &nbsp;
                </a>--%>

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
                    <ng-include src="'views/thumbnail.html'"></ng-include>
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
                    <ng-include src="'views/thumbnail.html'"></ng-include>
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
                   data-content-template="views/popovers/observing-time.html"
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
            class="explore-panel rel" 
            ng-controller="CurrentTourController" 
            
            >
            <table>
                <colgroup>
                    <col style="width:80px"/>
                    <col style="width:99%"/>
                    <col style="width:16px"/>
                </colgroup>
                <tr>
                    <td class="play-tour">
                        <a class="btn-play-tour" ng-click="pauseTour()">
                            <i class="fa fa-{{tourPaused ? 'play' : 'pause'}}"></i>
                        </a><br/>
                        {{currentTour.minuteDuration}}:{{currentTour.secDuration}}
                    </td>
                    <td class="tour-stops">
                        <div class="scroller">
                            <div class="stops-container">
                                <div class="stop-arrow" ng-repeat="stop in tourStops">
                                    <div class="thumbwrap">
                                        <div class="stop-thumb thumbnail" itemid="{{$index}}" ng-click="gotoStop($index, $event)" ng-context-menu="showContextMenu">
                                            <a class="ear stop-start"></a>
                                            <a class="ear stop-end"></a>
                                            <img ng-src="{{stop.thumb.src}}" alt="{{stop.description}}"/>
                                            <label class="slide-label">{{stop.description}}</label>
                                            <label class="duration">{{stop.secDuration}}</label>
                                        </div>
                                    </div>
                                    <div class="right-arrow" ng-if="!$last">
                                    <i class="arrow-line"></i><i class="arrow-head"></i>

                                    </div>
                                </div>

                            </div>
                        </div>

                    </td>
                    <td>&nbsp;</td>
                </tr>
            </table>
            
            
            
        </div>  
    </div>
    <div class="layer-manager desktop" ng-controller="LayerManagerController" ng-style="{display: layerManagerHidden ? 'none' : 'block'}" ng-init="initLayerManager()">
        <button aria-hidden="true" class="close pull-right" type="button" ng-click="toggleLayerManager()">×</button>
        <h5 localize="Layers"></h5>
        <div class="tree" ng-class="tree.collapsed?'collapsed':''" ng-style="{height:  layerManagerHeight() + 'px' }">
            <div class="checkbox">
                <i ng-class="tree.collapsed ? 'fa fa-plus-square-o' : 'fa fa-minus-square-o'" ng-click="tree.collapsed = !tree.collapsed;nodeChange(tree)"></i>
                <label data-ng-class="tree.checked ? 'checked' : ''">
                    <input type="checkbox" ng-model="tree.checked" data-ng-change="nodeChange(tree)" />
                    <span localize="{{tree.name}}"></span>

                </label>
            </div>
            <div class="indent" ng-class="node.collapsed ? 'collapsed' : ''" ng-repeat="node in tree.children">
                <ng-include src="'views/tree-node.html'"></ng-include>
                <div class="indent" ng-class="node.collapsed ? 'collapsed' : ''" ng-repeat="node in node.children">
                    <ng-include src="'views/tree-node.html'"></ng-include>
                    <div class="indent" ng-class="node.collapsed ? 'collapsed' : ''" ng-repeat="node in node.children">
                        <ng-include src="'views/tree-node.html'"></ng-include>
                        <div class="indent" ng-class="node.collapsed ? 'collapsed' : ''" ng-repeat="node in node.children">
                            <ng-include src="'views/tree-node.html'"></ng-include>
                            <div class="indent" ng-class="node.collapsed ? 'collapsed' : ''" ng-repeat="node in node.children">
                                <ng-include src="'views/tree-node.html'"></ng-include>
                            </div>
                        </div>
                    </div>
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
                data-content-template="views/popovers/shareplace.html"
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
            <%--<div class="control" ng-click="setSurveyProperties()">
                <label localize="Info"></label>
                <a class="btn"
                   bs-popover
                localize="Information" localize-only="title
                   
                   data-content-template="views/popovers/property-panel.html"l
                   data-placement="top"><i class="fa fa-info-circle"></i></a>
            </div>--%>
            <div ng-show="showCrossfader()" class="control" style="padding-right:10px">
                <label localize="Image Crossfade"></label>
                <div class="cross-fader">
                    <a class="btn">&nbsp;</a>
                </div>

            </div>
        
        </div>
        <div class="thumbnails nearby-objects rel" data-ng-controller="ContextPanelController" ng-style="{width: bottomControlsWidth()}">
            <div class="rel" style="display: inline-block;vertical-align:top;" ng-repeat="item in collectionPage" ng-if="lookAt != 'Planet' && lookAt != 'Panorama'">
                <ng-include src="'views/thumbnail.html'"></ng-include>
            </div>
            <label class="wwt-pager">
                <a class="btn" 
                    ng-if="(lookAt == 'Planet' || lookAt == 'Panorama' || lookAt == 'Earth' ) && !trackingObj"
                    data-bs-popover="popover" tabindex="0"
                    style="position:absolute; top:0;right:-204px" 
                    localize="Share this view"
                    localize-only="title"
                    data-content-template="views/popovers/shareplace.html"
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
                        data-content-template="views/popovers/shareplace.html"
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
    
<ng-include src="'views/modals/intro.html'"></ng-include>
<div class="modal" id="loadingModal" tabindex="-1" role="dialog" aria-labelledby="loadingModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            
            <div class="modal-body">
                <img src='<%= ResourcesLocation%>/Images/wwtlogo.png' 
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
    <ng-include src="'views/modals/open-item.html'"></ng-include>
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
            <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=4.0.41108.0" style="text-decoration:none">
              <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight" style="border-style:none"/>
            </a>
            </div>
        </object>
        <iframe style='visibility:hidden;height:0;width:0;border:0'></iframe>
    </div>
    <% } %>
    
</body>
</html>
