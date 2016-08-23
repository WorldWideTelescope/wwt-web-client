wwt.controllers.controller('TourSlideText', [
	'$scope',
	'$rootScope',
	'Util',
	'$timeout',
    function ($scope, $rootScope, util, $timeout) {
        var editorUI = wwtlib.WWTControl.singleton.tourEdit.tourEditorUI;
        var iframeBody;
        var editScope = null;
        var pristineText = null;
        function init() {
            editScope = angular.element('#currentTourPanel').scope();
            if (editScope.editText) {
                pristineText = editScope.editText.textObject;
                textObject = Object.assign({}, pristineText);
                
            }
        }

        var textObject = {
            text: '',
            foregroundColor: '#ffffff',
            backgroundColor: 'transparent',
            bold: 0,
            italic: 0,
            underline: 0,
            fontSize: 24,
            fontName: 'Arial',
            borderStyle:'None'
        };
 
    var saving = false;
    function initEditorObserver() {

        iframeBody = $('.modal.tour-text iframe').contents().find("body");
        if (editScope.editText) {
            iframeBody.html('');
            textObject.text.split('\n').forEach(function (s, i) {
                iframeBody.append($('<p></p>').html(s));
            });
            iframeBody.find('p').css({
                color: textObject.foregroundColor,
                backgroundColor: textObject.backgroundColor,
                fontWeight: textObject.fontWeight ? 'bold' : 'normal',
                fontSize: textObject.fontSize + 'pt',
                textDecoration: textObject.underline ? 'underline' : 'none',
                fontStyle: textObject.italic ? 'italic' : 'none',
                fontFamily: textObject.fontName
            });
        }
        var getObserver = function (cb) {

            return new MutationObserver(function (mutations) {
                if (!saving) {
                    mutations.forEach(function () { setTimeout(cb, 100) });
                } else {
                    console.log('not observing after save');
                }
            });
        };
        var fontConfig = {
            attributes: true,
            childList: true,
            characterData: true
        };
        var fontFamily = $('.mce-widget[aria-label="Font Family"] span.mce-txt');
        fontFamily.text(textObject.fontName);
        var fontFamilyObserver = function (mutation) {
            var ffText = fontFamily.text();
            if (ffText.indexOf('Font') === 0) {
                fontFamily.text(textObject.fontName);
                return;
            }
            iframeBody.find('*').css('font-family', fontFamily.text());
            textObject.fontName = fontFamily.text();
        };
        var fontSizes = $('.mce-widget[aria-label="Font Sizes"] span.mce-txt');
        fontSizes.text(textObject.fontSize);
        var fontSizesObserver = function (mutation) {
            var fsText = fontSizes.text();
            if (fsText.indexOf('Font') === 0) {
                fontSizes.text(textObject.fontSize);
                return;
            }
            iframeBody.find('*').css('font-size', fontSizes.text());
            textObject.fontSize = parseInt(fontSizes.text().replace('pt', ''));
        };
        var boldBtn = $('.mce-i-bold').parent().parent();
        var boldObserver = function (mutation) {
            textObject.bold = boldBtn.hasClass('mce-active');
            iframeBody.find('*').css('font-weight', textObject.bold ? 'bold' : 'normal')
        };
        var italicBtn = $('.mce-i-italic').parent().parent();
        var italicObserver = function (mutation) {
            textObject.italic = italicBtn.hasClass('mce-active');
            iframeBody.find('*').css('font-style', textObject.italic ? 'italic' : 'normal')
        };
        var underBtn = $('.mce-i-underline').parent().parent();
        var underObserver = function (mutation) {
            textObject.underline = underBtn.hasClass('mce-active');
            iframeBody.find('*').css('text-decoration', textObject.underline ? 'underline' : 'none');
        };

        getObserver(fontFamilyObserver).observe(fontFamily.parent().parent()[0], fontConfig);
        getObserver(fontSizesObserver).observe(fontSizes.parent().parent()[0], fontConfig);
        getObserver(boldObserver).observe(boldBtn[0], fontConfig);
        getObserver(italicObserver).observe(italicBtn[0], fontConfig);
        getObserver(underObserver).observe(underBtn[0], fontConfig);

        var bgColor = $('.mce-ico.mce-i-backcolor').parent().parent().find('.mce-preview')[0];
        var bgColorObserver = function (mutation) {
            var newBg = $(bgColor).css('background-color');
            textObject.backgroundColor = newBg;
            console.log(newBg, $('.popover.tour-text iframe').contents());
            iframeBody.css('background-color', newBg);
        };
        var fgColor = $('.mce-ico.mce-i-forecolor').parent().parent().find('.mce-preview')[0];
        var fgColorObserver = function (mutation) {
            var newFg = $(fgColor).css('background-color');
            iframeBody.find('*').css('color', newFg);
        };

        var colorConfig = {
            attributes: true,
            childList: false,
            characterData: false
        };
        getObserver(fgColorObserver).observe(fgColor, colorConfig);
        getObserver(bgColorObserver).observe(bgColor, colorConfig);
    };
    var att = 0;
    var readyTimer = function () {
        att++;
        if ($('.modal .mce-ico.mce-i-forecolor').length) {
            console.log('editor ready');
            initEditorObserver();
        }
        else {
            console.log(att + ' init attempts');
            setTimeout(readyTimer, 100);
        }
    };

    var hideEditor = $scope.hideEditor = function () {
        if (editScope.editText) {
            editScope.editText.onFinished(pristineText);
            editScope.editText = null;
        }
        $scope.$parent.$applyAsync(function () {
            $scope.$parent.$hide();
        });
    }
    $timeout(function () {
        tinymce.init({
            selector: 'textarea.tour-text',
            fontsize_formats: "8pt 9pt 10pt 11pt 12pt 14pt 16pt 18pt 20pt 24pt 28pt 32pt 36pt 40pt 44pt 48pt 54pt 60pt 66pt 72pt 80pt 88pt 96pt 112pt 128pt 150pt 200pt",
            height: 500,
            theme: 'modern',
            plugins: ['save contextmenu textcolor colorpicker'],
            toolbar1: 'save | undo redo | fontselect fontsizeselect | bold italic underline | forecolor backcolor ',
            toolbar2: '',
            save_onsavecallback: function () {
                saving = true;
                textObject.text = '';
                iframeBody.find('p').each(function (i, p) {
                    if (i > 0) {
                        textObject.text += '\n';
                    }
                    textObject.text += $(p).text();
                });
                console.log(textObject);
                try {
                    
                    var txtObj = editScope.editText ? editScope.editText.textObject : wwtlib.TextObject.create(
                        textObject.text,
                        textObject.bold,
                        textObject.italic,
                        textObject.underline,
                        textObject.fontSize,
                        textObject.fontName,
                        textObject.foregroundColor,
                        textObject.backgroundColor,
                        textObject.borderStyle);
                    if (editScope.editText) {
                        editScope.editText.onFinished(textObject);
                        editScope.editText = null;
                    } else {
                        editorUI.addText({}, txtObj);
                    }
                } catch (ex) { }
                hideEditor();
            },
            content_css: "css/mcecontent.css"
        });
        readyTimer();
    }, 10);
    init();
    }]
);
    