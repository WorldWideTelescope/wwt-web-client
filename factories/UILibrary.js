wwt.app.factory('UILibrary', ['$rootScope', 'AppState', 'Util', 'Localization', '$modal', '$sce', function ($rootScope, appState, util, loc, $modal, $sce) {

  $rootScope.layerManagerHidden = appState.get('layerManagerHidden') ? true : false;

  $rootScope.toggleLayerManager = function () {
    $rootScope.layerManagerHidden = !$rootScope.layerManagerHidden;
    appState.set('layerManagerHidden', $rootScope.layerManagerHidden);
  };

  $rootScope.getCreditsText = function (place) {
    return util.getCreditsText(place);
  };
  $rootScope.getCreditsUrl = function (place) {
    return util.getCreditsUrl(place);
  }

  $rootScope.getClassificationText = function (clsid) {
    var txt = util.getClassificationText(clsid);
    return txt || loc.getFromEn('Unknown');
  };

  $rootScope.secondsToTime = function (secs) {
    return util.secondsToTime(secs);
  };

  $rootScope.isMobile = util.isMobile;

  $rootScope.resLocation = $('body').data('res-location');
  $rootScope.bottomControlsWidth = function () {
    return (angular.element('div.context-panel').width() - angular.element('body.desktop .fov-panel').width()) + 1;
  }
  $rootScope.layerManagerHeight = function () {
    return $(window).height() - (168 + $('body.desktop .context-panel').height());
  };

  $rootScope.copyLink = function (event, selector) {
    var src = $(event.currentTarget);
    var input = selector ? src.parent().find(selector) : src.prev();
    input[0].select();
    document.execCommand('copy');
    var flyout = $('<div class=clipboard-status>Copied successfully</div>');
    input.parent().css('position', 'relative').append(flyout);
    //flyout.fadeIn(200).show();
    setTimeout(function () {
      flyout.fadeOut(1111);
    }, 3333);
  };
  $rootScope.altUnits = [
    {type: 1, label: 'Meters'},
    {type: 2, label: 'Feet'},
    {type: 3, label: 'Inches'},
    {type: 4, label: 'Miles'},
    {type: 5, label: 'Kilometers'},
    {type: 6, label: 'Astronomical Units'},
    {type: 7, label: 'Light Years'},
    {type: 8, label: 'Parsecs'},
    {type: 9, label: 'MegaParsecs'},
    {type: 10, label: 'Custom'}
  ];

  $rootScope.observingLocation = {name:'My Location'};

  var setObservingLocation = function(props){
    $rootScope.$applyAsync(function(){
    props.localHorizonMode = props.localHorizonMode === undefined ?
      $rootScope.ctl.settings._localHorizonMode :
      props.localHorizonMode;
    $rootScope.observingLocation = Object.assign($rootScope.observingLocation,props);
    var earthMap = wwtlib.LayerManager.get_allMaps().Earth;
    wwtlib.LayerManager.layerSelectionChanged(earthMap);
    var rf = new wwtlib.ReferenceFrame();
    rf.name = 'Observing Location';
    rf.lat = props.lat;
    rf.lng = props.lng;
    rf.altitude = props.altitude;
    rf.stationKeeping = true;
    wwtlib.LayerManager.referenceFrameWizardFinished(rf);
    $rootScope.hasSetupObservingLocation = true;
    var newSettings = {
      _locationAltitude:props.altitude,//.toFixed(1) +'m',//0.00000690228892768605
      _locationLat:props.lat,//47.70409186039239
      _locationLng:props.lng,//-122.30266851233682
      _localHorizonMode:props.localHorizonMode
    };
    Object.assign(wwt.wc.settings,newSettings);
  });
};



  var initTypeList = function (typeKey, scopeKey) {
    if (typeof scopeKey != 'string') {
      scopeKey = util.firstCharLower(typeKey)
    }
    $rootScope[scopeKey] = Object.keys(wwtlib[typeKey]).map(function (label, type) {
      return {
        type: type,
        label: util.firstCharUpper(label)
      }
    });
  };
  initTypeList('CoordinatesTypes', 'coordTypes');
  var enums = ['AltTypes', 'PointScaleTypes', 'MarkerScales', 'MarkerMixes', 'PlotTypes', 'ColorMaps'];
  enums.forEach(initTypeList);

  var annotationOpts = function (s) {
    var obj = {};
    var pairs = s.split(',');
    pairs.forEach(function (kv) {
      kv = kv.split(':');
      var k = kv[0].toLowerCase();
      var v = kv[1];
      if (k === 'embed' || k === 'youtube') {
        v += ':' + kv[2];
        var expression = /^(https?:\/\/)?((www\.)?(youtube(-nocookie)?|youtube.googleapis)\.com.*(v\/|v=|vi=|vi\/|e\/|embed\/|user\/.*\/u\/\d+\/)|youtu\.be\/)([_0-9a-z-]+)/i;
        v = v.match(expression)[7];
        k = 'youtube'
      }
      obj[k] = v;
    });
    return obj;
  };
  $rootScope.loadVOTableModal = wwt.loadVOTableModal = function (votable) {

    var modalScope = $rootScope.$new();
    modalScope.customClass = 'vo-tbl-modal';
    modalScope.voTableLayer = votable.get_table ? votable : wwtlib.VoTableLayer.create(votable);
    modalScope.votable = modalScope.voTableLayer.get_table();

    $modal({
      scope: modalScope,
      templateUrl: 'views/modals/centered-modal-template.html?v=' + util.resVersion,
      contentTemplate: 'views/modals/vo-table-viewer.html?v=' + util.resVersion,
      show: true,
      placement: 'center',
      backdrop: false
    });
  };

  var showColorpicker = function (colorpicker, e) {
    var modalScope = $rootScope.$new();
    modalScope.colorpicker = colorpicker;
    modalScope.mouse = e;
    modalScope.customClass = 'colorpicker-modal';
    $modal({
      scope: modalScope,
      templateUrl: 'views/modals/centered-modal-template.html?v=' + util.resVersion,
      contentTemplate: 'views/modals/colorpicker.html?v=' + util.resVersion,
      show: true,
      placement: 'center',
      backdrop: false,
      controller: 'colorpickerController'
    });
  };

  var loadingModal;
  $rootScope.loading = function (flag, content) {
    if (loadingModal) {
      loadingModal.hide();
      loadingModal = null;
    }
    if (flag) {
      loadingModal = $modal({
        templateUrl: 'views/modals/loading-content.html?v=' + util.resVersion,
        show: true,
        content: content || 'Content Loading. Please Wait...',
        placement: 'center'
      });
    }
  }
  if (util.getQSParam('debug') === 'disable') {
    setTimeout(function () {
      wwtlib.WWTControl.singleton.render = function () {
        console.log('fixed render loop :)');
      };
      //testing only:
      $('#WorldWideTelescopeControlHost').html('');
    }, 888)

  }
  var frameWizardDialog = wwtlib.LayerManager.get_frameWizardDialog();
  var showFrameWizardDialog = function (refFrame, propertyMode) {
    //console.log({refFrame: refFrame});
    var modalScope = $rootScope.$new();
    refFrame.name = refFrame.name || '';
    modalScope.refFrame = refFrame;
    modalScope.dialog = frameWizardDialog;
    modalScope.propertyMode = propertyMode;
    //modalScope.mouse = e;
    modalScope.customClass = 'wizard';
    $modal({
      scope: modalScope,
      templateUrl: 'views/modals/centered-modal-template.html?v=' + util.resVersion,
      contentTemplate: 'views/modals/ref-frame-wiz.html?v=' + util.resVersion,
      show: true,
      placement: 'center',
      backdrop: false,
      controller: 'refFrameController'

    });
  };
//scope,callback,cssClass,controller,template,backdrop - template=required
  var showModal = function (props) {
    //console.log({refFrame: refFrame});
    var modalScope = props.scope ? props.scope.$new() : $rootScope.$new();

    var modalConfig = {
      scope: modalScope,
      templateUrl: 'views/modals/centered-modal-template.html?v=' + util.resVersion,
      contentTemplate: 'views/modals/' + props.template + '.html?v=' + util.resVersion,
      show: true,
      //placement: 'center',
      backdrop: props.backdrop !== undefined ? props.backdrop : false,
      controller: props.controller
    };
    if (props.cssClass) {
      modalScope.customClass = props.cssClass;
    }
    if (props.fixedPosition) {
      modalScope.customClass += ' noreposition';
      modalConfig.animation = false;
      modalConfig.backdropAnimation = false;
      setTimeout(function(){
        $('.noreposition').removeClass('noreposition').find('.modal-dialog').css({left:300});
      },2222);
    } else {
      modalConfig.placement = 'centered';
    }
    var modal = $modal(modalConfig);

    if (props.callback) {
      props.callback({modal: modal, scope: modalScope, modalConfig: modalConfig});
    }
  };


  var dataVizWiz = wwtlib.LayerManager.get_dataVizWizardDialog();
  var showDataVizWiz = function (layerMap) {
    console.log(layerMap);
    var modalScope = $rootScope.$new();
    var propertyMode = typeof layerMap !== 'string';
    modalScope.propertyMode = propertyMode;
    if (!propertyMode) {
      modalScope.layerMap = layerMap;
    } else {
      modalScope.layer = layerMap;
    }
    modalScope.customClass = 'wizard';
    modalScope.dialog = dataVizWiz;
    $modal({
      scope: modalScope,
      templateUrl: 'views/modals/centered-modal-template.html?v=' + util.resVersion,
      contentTemplate: 'views/modals/data-viz-wiz.html?v=' + util.resVersion,
      show: true,
      placement: 'center',
      backdrop: false,
      controller: 'DataVizController'
    });
  };

  var refFrameDialog = wwtlib.LayerManager.get_referenceFramePropsDialog();
  //console.log(refFrameDialog);
  var showRefFrameProps = function (refFrame) {
    showFrameWizardDialog(refFrame, true);
  };

  var greatCircleDlg = wwtlib.LayerManager.get_greatCircleDlg();
  //console.log(greatCircleDlg);
  var showGreatCircleDlg = function (layer) {
    //console.log(layer);
    var modalScope = $rootScope.$new();
    modalScope.layer = layer;
    modalScope.customClass = 'great-circle';
    $modal({
      scope: modalScope,
      templateUrl: 'views/modals/centered-modal-template.html?v=' + util.resVersion,
      contentTemplate: 'views/modals/great-circle.html?v=' + util.resVersion,
      show: true,
      placement: 'center',
      backdrop: false,
      controller: 'greatCircleController'
    });
  };


  var embedVideo = function (videoid) {
    var modalScope = $rootScope.$new();
    modalScope.url = $sce.trustAsResourceUrl('//www.youtube.com/embed/' + videoid + '?autoplay=1');
    modalScope.customClass = 'wizard embed-modal';
    $modal({
      scope: modalScope,
      animation: false,
      backdropAnimation: false,
      templateUrl: 'views/modals/centered-modal-template.html?v=' + util.resVersion,
      contentTemplate: 'views/modals/embed-video.html?v=' + util.resVersion,
      show: true,
      //placement: 'center',
      backdrop: false,
      controller: 'EmbedController'
    });
  };
  wwt.root = $rootScope;

  return {
    addDialogHooks: function () {
      wwt.wc.add_annotationClicked(function (interface, event) {
        var opts = annotationOpts(event.get_id());
        if (opts.youtube) {
          embedVideo(opts.youtube);
        } else if (opts.link) {
          window.open(opts.link);
        }
      });
      wwt.wc.add_voTableDisplay(wwt.loadVOTableModal);
      wwt.wc.add_colorPickerDisplay(showColorpicker);
      frameWizardDialog.add_showDialogHook(function (frame) {
        showFrameWizardDialog(frame, false);
      });
      refFrameDialog.add_showDialogHook(showRefFrameProps);
      greatCircleDlg.add_showDialogHook(showGreatCircleDlg);
      dataVizWiz.add_showDialogHook(showDataVizWiz);
    },
    embedVideo: embedVideo,
    annotationOpts: annotationOpts,
    showModal: showModal,
    showErrorMessage:function(msg){
      var scope = $rootScope.$new();
      scope.message = msg;
      scope.title = 'Error';
      var props = {
        scope:scope,
        template:'message'
      };

      showModal(props);
    },
    setObservingLocation:setObservingLocation
  };
}]);

