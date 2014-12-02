<%@ Page Language="C#" CodeFile="Default.aspx.cs" Inherits="Default" %>

<!DOCTYPE html>
<html>
<head runat="server">
	<title>WorldWide Telescope Web Client</title>
	<meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">
	<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
	<meta name="description" content="Worldwide Telescope enables your computer to function as a virtual telescope, bringing together imagery from the best telescopes in the world." />
	<meta name="ROBOTS" content="INDEX, FOLLOW">
	<% if (Client == Clients.Html5 || Client == Clients.Mobile)
	   { %>
	<link href=<%= ResourcesLocation%>/css/webclient.css?v=<%= ResourcesVersion%> rel="stylesheet" />
	<link href=<%= ResourcesLocation%>/css/angular-motion.css?v=<%= ResourcesVersion%> rel="stylesheet" />
	<link href=//maxcdn.bootstrapcdn.com/font-awesome/4.2.0/css/font-awesome.min.css rel="stylesheet"/>
	<link href=<%= ResourcesLocation%>/ext/introjs.css?v=<%= ResourcesVersion%> rel="stylesheet" />
	<style>
		html, body.fs-player, iframe {
			height: 100%;
			width: 100%;
			margin: 0;
			padding: 0;
			overflow: hidden;
		}
		.finder-scope {
			background: url(<%= ResourcesLocation %>/Images/finder-scope.png?v=<%= ResourcesVersion %>) no-repeat;
		}
	</style>
	<script type="text/javascript" src="<%=SDKLocation + DebugQs%>"></script>
	<script src="//cdnjs.cloudflare.com/ajax/libs/jquery/2.1.0/jquery.min.js" type="text/javascript"></script>
	<script src="<%= ResourcesLocation%>/ext/bootstrap<%= Debug ? "" : ".min"%>.js"></script>
	<% if (Debug || DebugChrome)
	   { %>
	<script src="<%= ResourcesLocation%>/ext/angular.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/ext/angular-route.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/ext/angular-cookies.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/ext/angular-animate.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/ext/angular-strap.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/ext/angular-strap.tpl.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/ext/intro.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/ext/angular-intro.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/app.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/directives/Scroll.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/factories/appstate.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/factories/localization.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/factories/FinderScope.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/factories/Util.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/factories/UILibrary.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/factories/SearchUtil.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/factories/Skyball.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/factories/HashManager.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/dataproxy/Places.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/dataproxy/Tours.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/dataproxy/SearchData.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/controllers/MainController.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/controllers/ViewController.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/controllers/ThumbnailController.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/controllers/ToursController.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/controllers/SettingsController.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/controllers/IntroController.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/controllers/popovers/ObservingTimeController.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/controllers/LayerManagerController.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/controllers/modals/OpenItemController.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/controllers/AdsController.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/controllers/MobileNavController.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/controllers/ShareController.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/controls/move.js?v=<%= ResourcesVersion%>"></script>
	<script src="<%= ResourcesLocation%>/controls/util.js?v=<%= ResourcesVersion%>"></script>
	<% }
	   else
	   { %>
	<script src="<%= ResourcesLocation%>/wwtwebclient.min.js?v=<%= ResourcesVersion%>"></script>
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
	<link rel="icon" type="image/png" href="favicon.png"/>
	
  
</head>
<%--<body class="<%= BodyClass %>" data-ng-app="wwtApp" data-res-location="<%= ResourcesLocation %>" data-version="<%= ResourcesVersion %>">--%>
	<body class="<%= BodyClass %>" data-ng-app="wwtApp" data-res-location="<%= ResourcesLocation %>" data-version="1">
	<% if (Client == Clients.Html5 || Client == Clients.Mobile)
	   { %>
<div data-ng-controller="MainController" ng-cloak ng-init="initUI()" class="<%=Client == Clients.Mobile?"mobile":"desktop" %>">
	<div id="WorldWideTelescopeControlHost">
		<div id="WWTCanvas"></div>
	</div>
<% if (Client == Clients.Mobile)
   { %>
	
	<a class="btn" data-bs-popover="popover" tabindex="0"
		style="position:absolute;top:4px;left:4px;z-index: 1041" 
		title="{{getFromEn('Share this place')}}"
		data-content-template="views/popovers/shareplace.html"
		data-ng-class="searchModal ? 'hide':''"
		data-placement="bottom-left"
		data-ng-hide="trackingObj && trackingObj.get_name && !tourPlaying && lookAt != 'Earth' && lookAt != 'Planet' && lookAt != 'Panorama'"
		>
		<i class="fa fa-share-alt"></i>
		{{getFromEn('Share')}}
	</a>

	<div ng-show="trackingObj && trackingObj.get_name && !tourPlaying && lookAt != 'Earth' && lookAt != 'Planet' && lookAt != 'Panorama'" style="position:absolute;top:3px;left:3px;">
		<div title="{{trackingObj.get_name()}}" class="small"><strong>{{getFromEn('Tracking')}}</strong> <br/>{{trackingObj.get_name()}}</div>
		<br />
		<a class="btn" data-bs-popover="popover" tabindex="0"
			style="" 
			title="{{getFromEn('Share this place')}}"
			data-content-template="views/popovers/shareplace.html"
			data-container="body"
			data-placement="bottom-left"
			>
			<i class="fa fa-share-alt"></i>
			{{getFromEn('Share')}}
		</a>
	</div>
	<ng-include src="'views/modals/mobile-explore.html'" onload="initFinder()"></ng-include>
	<%--<ng-include src="'views/modals/mobile-tours.html'" onload="initFinder()"></ng-include>--%>
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
						<a href="javascript:void(0)" title="{{getFromEn('Look At')}}" class="dropdown-toggle" data-toggle="dropdown">
							{{getFromEn('Look At')}} <span class="small">({{lookAt}})</span><i class="fa fa-angle-down"></i>
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
						<a href="javascript:void(0)" title="{{getFromEn('Imagery')}}" class="dropdown-toggle" data-toggle="dropdown">
							{{getFromEn('Imagery')}} <span class="small">({{backgroundImagery.get_name()}})</span><i class="fa fa-angle-down"></i>
						</a>
						<ul class="dropdown-menu" >
							<li ng-repeat="s in surveys" ng-click="hideMenu()">
								<a href="javascript:void(0)" ng-click="setSurveyBg(s.name);">
									{{s.name}}                        
								</a>
							</li>
						</ul>

					</li>
					<li>
						<hr/>
					</li>
					
					<li style="padding-left:12px;">
						<a class="btn" ng-repeat="m in modalButtons" ng-click="showModal(m)">
							<i class="fa {{m.icon}}"></i>
							{{getFromEn(m.text)}}
						</a>
						
					</li>
					<li>
						<hr class="clearfix"/>
					</li>
					<li>
						<a ng-click="resetCamera()">
							{{getFromEn('Reset Camera')}}
						</a>
					</li>
					<li>
						<hr class="clearfix"/>
					</li>
					<li>
						<a ng-click="gotoPage('/')" target="_blank">{{getFromEn('WorldWide Telescope Home')}}</a>
					</li>
					<li>
						<a ng-click="gotoPage('/Learn')" target="_blank">{{getFromEn('Getting Started (Help)')}}</a>
					</li>
					<li>
						<a ng-click="gotoPage('/Terms')" target="_blank">{{getFromEn('WorldWide Telescope Terms of Use')}}</a>
					</li>
					<li>
						<a ng-click="gotoPage('/About')">{{getFromEn('About WorldWide Telescope')}}</a>
					</li>
					<li>
						<a ng-click="gotoPage('/Support/IssuesAndBugs')">{{getFromEn('Product Support')}}</a>
					</li>
					<%--<li class="dropdown" ng-repeat="tab in nav">
						<a ng-if="!tab.menu" href="javascript:void(0)" ng-click="tabClick(tab)" title="{{getFromEn(tab.label)}}">
							{{getFromEn(tab.label)}} 
						</a>
						<a ng-if="tab.menu" class="dropdown-toggle" data-toggle="dropdown">
							{{getFromEn(tab.label)}} <i class="fa fa-angle-down" ng-if="tab.menu"></i>
						</a>
						<ul ng-if="tab.menu" class="dropdown-menu">
							<li ng-repeat="(item, action) in tab.menu" ng-click="hideMenu()" ng-class="item.indexOf('sep') == 0 ? 'divider' : ''">
								<a href="javascript:void(0)" ng-click="menuAction(action)" ng-if="item.indexOf('sep') != 0">
									{{getFromEn(item)}}
								</a>
							</li>
						</ul>

					</li>--%>

				</ul>

			</div>
		</div>
	</div>
	<div ng-class="isLoading ?  'mobile-loading' : 'hide'">
		<img src='<%=ResourcesLocation %>/Images/wwtlogo.png' class="pull-left" alt="{{getFromEn('Microsoft WorldWide Telescope')}} Logo" />
		<h3 style="position:relative;top:-2px">
			<small style="color:white">Microsoft<sup>&reg;</sup> Research</small><br />
			World<span style="color:#6ba9e6">Wide Telescope</span>
		</h3>
		<h4>{{getFromEn('Welcome to the WorldWide Telescope Web Client')}}</h4>
		<p>
			<i class="fa fa-spin fa-spinner"></i>
			{{getFromEn('WorldWide Telescope is loading. Please wait.')}}
			
		</p>
		<p class="small">({{getFromEn('Please ensure you have a strong connection to the internet for the best experience.')}})</p>
	</div>
	<ng-include src="'views/modals/mobile-nearby-objects.html'"></ng-include>
	<div class="context-panel">
		<div class="nearby-objects" ng-if="nbo.length && lookAt == 'Sky'" ng-show="!tourPlaying">
			<a ng-click="showNbo()" title="{{nbo[0].get_name()}}" class="thumbnail">
				<img ng-src="{{nbo[0].get_thumbnailUrl()}}" alt="Thumbnail of {{nbo[0].get_name()}}" />
				<label>{{getFromEn('Nearby')}}</label>
			</a>
			<div class="nbo-count">{{nboCount}}</div>
		</div>
	
		
		<div class="control" ng-show="lookAt == 'Sky' && trackingObj && !tourPlaying && trackingObj.get_backgroundImageset() != null || trackingObj.get_studyImageset() != null" style="position:absolute;bottom:12px;left:130px;">
			<label>{{getFromEn('Image Crossfade')}}</label>
			<div class="cross-fader">
				<a class="btn">&nbsp;</a>
			</div>

		</div>
		<div class="fov-panel mobile" ng-show="lookAt=='Sky' && !tourPlaying">
			<div class="left">
				<p>{{getFromEn('N')}}</p>
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
	<ng-include src="'views/modals/finder-scope.html'" onload="initFinder()"></ng-include>
	
	
	<div data-ng-controller="ViewController"></div>
	
	<ul class="dropdown-menu" role="menu" id="topMenu"></ul>
	

	<div id="ribbon">
		<a class="btn pull-right" href="/Download/" target="wwt">
			<i class="fa fa-download"></i>
			{{getFromEn('Install Windows Client')}}
		</a>
		<ul class="wwt-tabs">
			<li data-ng-repeat="tab in ribbon.tabs" data-ng-class="activePanel == tab.label ? 'active' : ''">
				<div class="outer">
					<a href="javascript:void(0)">
						<span class="label" data-ng-click="tabClick(tab)" id="{{tab.button}}">{{getFromEn(tab.label)}}</span>
						<div class="menu" data-ng-click="menuClick(tab.menu)" id="tabMenu{{$index}}" data-target="#menu{{$index}}">
							<i class="fa fa-caret-down"></i>
						</div>
					</a>
				</div>
			</li>
		</ul>
	
	</div>

	<div class="{{topExpanded ? 'top-panel top-expanded' : 'top-panel'}}" id="topPanel">
		
	<div 
		ng-show="activePanel == 'Explore' && !loadingUrlPlace" 
		class="{{expanded?'explore-panel rel expanded':'explore-panel rel'}}" 
		ng-controller="ThumbnailController" 
		ng-init="initExploreView()"
		>
		<span ng-repeat="bc in breadCrumb" class="bc"><a href="javascript:void(0)" ng-click="breadCrumbClick($index)">{{bc}}</a>&nbsp;>&nbsp;</span><br />
		<div style="display: inline-block;vertical-align:top;" ng-repeat="item in exploreList" id="exploreList">
			<ng-include src="'views/thumbnail.html'" onload="initFinder()"></ng-include>
		</div>
		<label class="wwt-pager">
			<a href="javascript:void(0)" data-ng-disabled="currentPage == 0" ng-click="goBack()">
				<i class="fa fa-play reverse"></i>
			</a>
			{{(currentPage+1)}} {{getFromEn('of')}} {{pageCount}}
			<a href="javascript:void(0)" ng-disabled="currentPage == pageCount - 1" ng-click="goFwd()">
				<i class="fa fa-play"></i>
			</a>
		</label>
		<a class="{{expanded ? 'expanded btn tn-expander' : 'btn tn-expander'}}" ng-click="expandThumbnails()">
			<i class="fa fa-caret-down" style="position:relative;top:-5px;" ng-if="!expanded"></i>
			<i class="fa fa-caret-up" style="position:relative;top:-5px;" ng-if="expanded"></i>
		</a>
	</div>
	<ng-include src="'views/tours.html'"></ng-include>
	<div data-ng-show="activePanel == 'Search'" ng-controller="ThumbnailController" ng-init="initSearch()">
	<div style="margin:-4px 0 1px;">
		<div style="padding:4px 100px 0 4px;" class="iblock input-group">
			<input type="search" id="txtSearch" ng-model="q" ng-keydown="searchKeyPress()" placeholder="{{getFromEn('Object Search')}}" style="width:200px" />
			<span class="fa fa-search form-control-feedback rel" style="left:-32px;display:inline-block;overflow:hidden;height:16px;"></span>
				
		</div>
		<!--<div class="checkbox" style="display: inline-block">
			<label data-ng-class="chkPlotResults ? 'checked' : ''">
				<input type="checkbox" ng-model="chkPlotResults" />
				{{getFromEn('Plot Results')}}
			</label>
		</div>
		<a class="btn">{{getFromEn('VO Search')}}</a>-->

		<div style="padding:4px 0 0 4px;display: inline-block">
			<div class="select">
				<select ng-model="SearchType">
					<option value="J2000">{{getFromEn('J2000')}}</option>
					<!--<%--
						<option>{{getFromEn('Alt/Az')}}</option>--%>
						<option value="Galactic">{{getFromEn('Galactic')}}</option>
						<option value="Ecliptic">{{getFromEn('Ecliptic')}}</option>-->
				</select>

			</div>
			&nbsp;
			<label>
				{{getFromEn('RA')}}
				<input type="number" style="width:100px" ng-model="RA" />
			</label>

			<label>
				{{getFromEn('Dec')}}
				<input type="number" style="width:100px" ng-model="Dec" />
			</label>
			<a class="btn" ng-click="gotoCoord()">{{getFromEn('Go')}}</a>

		</div>
	</div>
	<div class="search-results">
		<div style="display: inline-block;vertical-align:top;" ng-repeat="item in searchResultSet">
			<span ng-if="item.get_thumbnailUrl().length > 15" class="thumbwrap">
				<ng-include src="'views/research-menu.html'" ng-if="$index == 0"></ng-include>
				
				<a ng-click="clickThumb(item)" title="{{item.get_name()}}" ng-class="item.get_thumbnailUrl() + item.get_name() == activeItem ? 'thumbnail active' : 'thumbnail'">
					<i class="fa fa-image" 
					   ng-if="item.get_backgroundImageset() != null || item.get_studyImageset() != null"></i>
					<img ng-src="{{item.get_thumbnailUrl()}}" alt="Thumbnail of {{item.get_name()}}" />
					<label>{{item.get_name()}}</label>
				</a>
				<div ng-if="!item.get_isFolder()" class="dropdown" ng-mouseenter="moveMenu($index,item)" id="menuContainer{{$index}}">
					<span bs-popover="popover" class="thumb-popover" style="height:0;width:100%;display:block"
						  data-placement="bottom{{$index==0?'-left':''}}" tabindex="0"
						  data-content-template="views/popovers/property-panel.html"
						  data-container="body">&nbsp;</span>
					<a data-toggle="dropdown" role="button" ng-click="showMenu(item,$index)" class="yellow-arrow">
						<img src='<%=ResourcesLocation %>/Images/context-menu-arrow.png' class="menu" />
					</a>
				</div>
			</span>
		</div>
	</div>
	<label class="wwt-pager" ng-show="searchResults.length > 0">
		<a href="javascript:void(0)" data-ng-disabled="currentPage == 0" ng-click="goBack()">
			<i class="fa fa-play reverse"></i>
		</a>
		{{(currentPage + 1)}} of {{pageCount}}
		<a href="javascript:void(0)" ng-disabled="currentPage == pageCount - 1" ng-click="goFwd()">
			<i class="fa fa-play"></i>
		</a>
	</label>
</div>
	<div data-ng-show="activePanel == 'View'" data-ng-controller="ViewController">
	
	<div class="layer-manager-toggle">
		<label>{{getFromEn('Use Layer Manager to Control User Settings')}}</label>
		<a href="javascript:void(0)" class="layer-manager-icon" title="{{getFromEn('Show/Hide Layer Manager')}}" ng-click="toggleLayerManager()">
			&nbsp;
		</a>
	</div>
	<fieldset>

		<div>
			<label>{{getFromEn('Name')}}:</label> <label>{{locationName}}</label>
		</div>
		<div>
			<div class="pull-right"><label>{{getFromEn('Alt')}}:</label> <label>{{UITools.formatDistance(ctl.settings.get_locationAltitude())}}</label></div>
			<label>{{getFromEn('Lat')}}:</label> <label>{{formatHms(ctl.settings.get_locationLat())}}</label>
		</div>
		<div class="clearfix">
			<label>{{getFromEn('Lng')}}:</label> <label>{{formatHms(ctl.settings.get_locationLng())}}</label>
		</div>
		<div class="checkbox">
			<label data-ng-class="viewFromLocation ? 'checked' : ''">
				<input type="checkbox" ng-model="viewFromLocation" ng-change="setViewFromLocation()" />
				{{getFromEn('View From This Location')}}
			</label>
		</div>
	</fieldset>
	<fieldset>
		<a class="btn" bs-popover
		   title="{{getFromEn('Observing Time')}}"
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
				{{getFromEn('Galactic Plane Mode')}}
			</label>
		</div>
	</fieldset>
</div>
	<div data-ng-show="activePanel == 'Settings'" ng-controller="SettingsController">
	<fieldset>
		<div class="iblock">
			<div class="checkbox">
				<label data-ng-class="crosshairs ? 'checked' : ''">
					<input type="checkbox" ng-model="crosshairs" data-ng-change="saveSettings()" />
					{{getFromEn('Reticle/Crosshairs')}}
				</label>
			</div>
			<div class="checkbox">
				<label data-ng-class="autoHideTabs ? 'checked' : ''">
					<input type="checkbox" ng-model="autoHideTabs" data-ng-change="saveSettings()" />
					{{getFromEn('Auto Hide Tabs')}}
				</label>
			</div>
			<div class="checkbox">
				<label data-ng-class="autoHideContext ? 'checked' : ''">
					<input type="checkbox" ng-model="autoHideContext" data-ng-change="saveSettings()" />
					{{getFromEn('Auto Hide Context')}}
				</label>
			</div>
			<div class="checkbox">
				<label data-ng-class="smoothPanning ? 'checked' : ''">
					<input type="checkbox" ng-model="smoothPanning" data-ng-change="saveSettings()" />
					{{getFromEn('Smooth Panning')}}
				</label>
			</div>
		</div>

	</fieldset>

	<fieldset style="padding-bottom: 16px;">

		<label>{{getFromEn('Preferred Client')}}</label><br />
		<select ng-change="setClientPref()" ng-model="preferredClient" ng-options="c.code as c.label for c in availableClients"></select>
		<div class="checkbox">
			<label data-ng-class="WebGl ? 'checked' : ''">
				<input type="checkbox" ng-model="WebGl" data-ng-change="setWebGl()" />
				{{getFromEn('Use WebGL (if available)')}}
			</label>
		</div>
		<div ng-if="redirecting">
			{{getFromEn('Redirecting to Silverlight Client in')}} {{redirectingSeconds}} {{getFromEn('seconds')}}
			<a class="btn" ng-click="cancelRedir()">
				<i class="fa fa-stop"></i>
				{{getFromEn('Stop')}}
			</a>
		</div>
	</fieldset>
	<fieldset style="padding-bottom: 16px;">
		<label>{{getFromEn('Select Your Language')}}</label><br />
		<select ng-change="setLanguageCode(selectedLanguage)" ng-model="selectedLanguage" ng-options="l.code as l.label for l in availableLanguages"></select>
	</fieldset>
</div>
	<% if (ADS)
	   {%>  
	<div ng-init="initAds()"
		 data-ng-show="activePanel == 'ADS'"
		ng-controller="ADSController" 
		>
		<fieldset class="radio-buttons" style="padding-bottom:3px;">
			<div class="iblock">
				<h5>{{getFromEn('Heatmap Options - Objects')}}</h5>
				<div class="iblock">
					<div class="checkbox">
						<label data-ng-class="fgImagery == 'All' ? 'checked' : ''">
							<input type="radio" name="fgImagery" ng-model="fgImagery" value="All" ng-change="adsChange()" />
							{{getFromEn('All')}}
						</label>
					</div>
					<div class="checkbox">
						<label data-ng-class="fgImagery == 'Star' ? 'checked' : ''">
							<input type="radio" name="fgImagery" ng-model="fgImagery" value="Star" ng-change="adsChange()" />
							{{getFromEn('Stars')}}
						</label>
					</div>
					<div class="checkbox">
						<label data-ng-class="fgImagery == 'Galaxy' ? 'checked' : ''">
							<input type="radio" name="fgImagery" ng-model="fgImagery" value="Galaxy" ng-change="adsChange()" />
							{{getFromEn('Galaxies')}}
						</label>
					</div>
				</div>
				<div class="iblock">
					<div class="checkbox">
						<label data-ng-class="fgImagery == 'HII regions' ? 'checked' : ''">
							<input type="radio" name="fgImagery" ng-model="fgImagery" value="HII regions" ng-change="adsChange()" />
							{{getFromEn('HII regions')}}
						</label>
					</div>
					<div class="checkbox">
						<label data-ng-class="fgImagery == 'Nebula' ? 'checked' : ''">
							<input type="radio" name="fgImagery" ng-model="fgImagery" value="Nebula" ng-change="adsChange()" />
							{{getFromEn('Nebulae')}}
						</label>
					</div>
					<div class="checkbox">
						<label data-ng-class="fgImagery == 'Other' ? 'checked' : ''">
							<input type="radio" name="fgImagery" ng-model="fgImagery" value="Other" ng-change="adsChange()" />
							{{getFromEn('Other')}}
						</label>
					</div>
				</div>

			</div>
			<div class="iblock rel">
				
				<h5  style="margin-left:24px;">{{getFromEn('Bands')}}</h5>
				<div class="iblock" style="margin-left:12px;padding-left:12px;border-left:solid 1px #728f9a">
					<div class="checkbox">
						<label data-ng-class="fgImagery == 'Radio' ? 'checked' : ''">
							<input type="radio" name="fgImagery" ng-model="fgImagery" value="Radio" ng-change="adsChange()" />
							{{getFromEn('Radio')}}
						</label>
					</div>
					<div class="checkbox">
						<label data-ng-class="fgImagery == 'Infrared' ? 'checked' : ''">
							<input type="radio" name="fgImagery" ng-model="fgImagery" value="Infrared" ng-change="adsChange()" />
							{{getFromEn('Infrared')}}
						</label>
					</div><div class="checkbox">
						<label data-ng-class="fgImagery == 'year' ? 'checked' : ''">
							<input type="radio" name="fgImagery" ng-model="fgImagery" value="year" ng-change="adsChange()" />
							{{getFromEn('Year (use slider)')}}
						</label>
					</div>
					
				</div>
				<div class="iblock" style="vertical-align: top">
					<div class="checkbox">
						<label data-ng-class="fgImagery == 'X-ray' ? 'checked' : ''">
							<input type="radio" name="fgImagery" ng-model="fgImagery" value="X-ray" ng-change="adsChange()" />
							{{getFromEn('X-ray')}}
						</label>
					</div>
					<div class="checkbox">
						<label data-ng-class="fgImagery == 'Ultraviolet' ? 'checked' : ''">
							<input type="radio" name="fgImagery" ng-model="fgImagery" value="Ultraviolet" ng-change="adsChange()" />
							{{getFromEn('Ultraviolet')}}
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
	</div>
	<div class="layer-manager desktop" ng-controller="LayerManagerController" ng-hide="layerManagerHidden" ng-init="initLayerManager()">
		<button aria-hidden="true" class="close pull-right" type="button" ng-click="toggleLayerManager()">×</button>
		<h5>
			{{getFromEn('Layers')}}
		</h5>
		<div class="tree" ng-class="tree.collapsed?'collapsed':''">
			<div class="checkbox">
				<i ng-class="tree.collapsed ? 'fa fa-plus-square-o' : 'fa fa-minus-square-o'" ng-click="tree.collapsed = !tree.collapsed;nodeChange(tree)"></i>
				<label data-ng-class="tree.checked ? 'checked' : ''">
					<input type="checkbox" ng-model="tree.checked" data-ng-change="nodeChange(tree)" />
					{{tree.name}}

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
	
	
	<div ng-class="lookAt == 'Planet' || lookAt == 'Panorama' || lookAt == 'Earth' ? 'context-panel compressed' : 'context-panel'">
		<div ng-class="lookAt == 'Planet' || lookAt == 'Panorama' || lookAt == 'Earth' ? 'hide' : lookAt == 'SolarSystem' ? 'solar-system-mode fov-panel' : 'fov-panel'">
			<a 
				class="btn" tabindex="0" 
				data-bs-popover="popover"
				ng-if="!showTrackingString()"
				style="position:absolute; top:6px;left:-33px;z-index:3" 
				title="{{getFromEn('Share this view')}}"
				data-content-template="views/popovers/shareplace.html"
				data-container="body"
				data-placement="top-right"
				>
				<i class="fa fa-share-alt"></i>
			</a>
			<div class="left" ng-if="lookAt != 'SolarSystem'">
				<p>{{getFromEn('N')}}</p>
				<div class="sphere" id="skyball">
					<div class="v-ellipse"></div>
					<div class="h-ellipse"></div>
					<div class="x-axis"></div>
					<div class="y-axis"></div>
				</div>
				<p>{{getFromEn('RA')}}: {{formatHms(ctl.getRA(), true)}}</p>
				<p>{{getFromEn('Dec')}}: {{formatHms(ctl.getDec(), false, true)}}</p>
			</div>
			<div class="left" ng-if="lookAt == 'SolarSystem'">
				<div class="control">
					<label>{{getFromEn('Planet Size')}}</label>
					<div class="planetary-scale">
						<a class="btn"></a>
					</div>
				
				</div>
				<label class="pull-right">{{getFromEn('Large')}}</label>
				<label class="pull-left">{{getFromEn('Actual')}}</label>
				<div class="location">
					<p>{{getFromEn('Lat')}}: {{formatHms(coords.get_lat(), false, false)}}</p>
					<p>{{getFromEn('Lng')}}: {{formatHms(coords.get_lng(), false, false)}}</p>
				</div>
			</div>
			<div class="right">
				<p style="position: relative;top:3px;">
					{{getFromEn(constellations.fullNames[singleton.constellation])}}
					<span class="pull-right">{{formatHms(ctl.get_fov())}}</span>
				</p>
				<div class="constellation-viewport" ng-click="gotoConstellation(singleton.constellation)">
					<img ng-src="{{constellations.constellationCentroids[singleton.constellation].get_thumbnailUrl()}}" />
				</div>
				<div class="bar"></div>
			</div>
		</div>
		<div class="controls">
			<div class="layer-manager-toggle iblock" id="btnToggleLayerMgr">
		
				<a href="javascript:void(0)" 
					class="layer-manager-icon" 
					title="{{getFromEn('Show/Hide Layer Manager')}}" 
					ng-click="toggleLayerManager()">
					&nbsp;
				</a>
			</div>
			<div class="control">
				<label>{{getFromEn('Look At')}}</label>
				<select id="lstLookAt"
						ng-init="lookAt = getFromEn('Sky')"
						ng-model="lookAt"
						ng-change="lookAtDropdownChanged()"
						ng-options="type for type in lookTypes"></select>
			</div>
			<div class="control">
				<label>{{getFromEn('Imagery')}}</label>
				<select id="lstImagery"
					ng-init="backgroundImagery.name = 'Digitized Sky Survey (Color)'"
					ng-model="backgroundImagery"
					ng-change="setSurveyBg()"
					ng-options="s.name for s in surveys">
					<option value="?">&nbsp;</option>
				</select>
			</div>
			<%--<div class="control" ng-click="setSurveyProperties()">
				<label>{{getFromEn('Info')}}</label>
				<a class="btn"
				   bs-popover
				   title="{{getFromEn('Information')}}"
				   data-content-template="views/popovers/property-panel.html"
				   data-placement="top"><i class="fa fa-info-circle"></i></a>
			</div>--%>
			<div ng-show="showCrossfader()" class="control" style="padding-right:10px">
				<label>{{getFromEn('Image Crossfade')}}</label>
				<div class="cross-fader">
					<a class="btn">&nbsp;</a>
				</div>

			</div>
		
		</div>
		<div class="thumbnails nearby-objects rel" data-ng-controller="ThumbnailController" ng-init="initNearbyObjects()">
			<div class="rel" style="display: inline-block;vertical-align:top;" ng-repeat="item in nearbyPlaces" ng-if="lookAt != 'Planet' && lookAt != 'Panorama'">
				<span ng-if="item.get_thumbnailUrl().length > 15" class="thumbwrap">
					<ng-include src="'views/research-menu.html'" ng-if="$index == 0"></ng-include>
					<span bs-popover="popover" class="thumb-popover" style="height:0;width:100%;display:block"
							  data-placement="top{{$index==0?'-left':''}}" tabindex="0"
							  data-content-template="views/popovers/property-panel.html"
							  data-container="body">&nbsp;</span>
					<a ng-click="clickThumb(item)" ng-mouseenter="hoverThumb(item)" ng-mouseleave="clearAnnotations()" title="{{item.get_name()}}" ng-class="item.get_thumbnailUrl() + item.get_name() == activeItem ? 'thumbnail active' : 'thumbnail'">
						<i class="fa fa-image" 
						   ng-if="item.get_backgroundImageset() != null || item.get_studyImageset() != null"></i>
						<img ng-src="{{item.get_thumbnailUrl()}}" alt="Thumbnail of {{item.get_name()}}" />
						<label>{{item.get_name()}}</label>
					</a>
					<div ng-if="!item.get_isFolder()" class="dropup" ng-mouseenter="moveNboMenu($index,item)" id="nboMenuContainer{{$index}}">
					
						<a data-toggle="dropdown" role="button" ng-click="showMenu(item,$index)" class="yellow-arrow">
							<img src="<%=ResourcesLocation %>/Images/context-menu-arrow.png" class="menu" />
						</a>
					</div>
				</span>
			
			</div>
			<label class="wwt-pager">
				<a class="btn" 
					ng-if="(lookAt == 'Planet' || lookAt == 'Panorama' || lookAt == 'Earth' ) && !trackingObj"
					data-bs-popover="popover" tabindex="0"
					style="position:absolute; top:0;right:-204px" 
					title="{{getFromEn('Share this view')}}"
					data-content-template="views/popovers/shareplace.html"
					data-container="body"
					data-placement="top-right"
					>
					<i class="fa fa-share-alt"></i>
				</a>
				<div class="iblock tracking rel" ng-if="showTrackingString()" style="vertical-align: top;">
					<div>{{getFromEn('Tracking')}}</div>
					<div title="{{trackingObj.get_name()}}">{{trackingObj.get_name()}}</div>
					<a class="btn" data-bs-popover="popover" tabindex="0"
						style="position:absolute; top:0;left:-40px" 
						title="{{getFromEn('Share this place')}}"
						data-content-template="views/popovers/shareplace.html"
						data-container="body"
						data-placement="top-right"
						>
						<i class="fa fa-share-alt"></i>
					</a>
				</div>
				<div class="iblock rel" style="vertical-align: top;right:{{ !trackingObj ? 50 : 0 }}px" ng-if="lookAt != 'Planet' && lookAt != 'Panorama' && pageCount > 1">
					<a href="javascript:void(0)" data-ng-disabled="currentPage == 0" ng-click="goBack()">
						<i class="fa fa-play reverse"></i>
					</a>
					{{(currentPage+1)}} {{getFromEn('of')}} {{pageCount}}
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
				<img src='<%=ResourcesLocation %>/Images/wwtlogo.png' style="width:19%;height:19%;position:relative;left:-3px;margin-right:12px;" class="pull-left" alt="{{getFromEn('Microsoft WorldWide Telescope')}} Logo" />
				<h1 style="position:relative;top:-2px">
					<small style="color:white">Microsoft<sup>&reg;</sup> Research</small><br />
					World<span style="color:#6ba9e6">Wide Telescope</span>
				</h1>
				<p>
					<i class="fa fa-spin fa-spinner"></i>
					{{getFromEn('Content Loading. Please Wait...')}}
				</p>
				
				
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
