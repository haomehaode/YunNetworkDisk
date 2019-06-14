(function ($) {
    $.fn.opop = function (option) {
        var set = $.extend({
            popbtn: $(this),
            popwindow: $('.pop-window'),
            popbg: $('.pop-window-bg'),
            popclose: $('.pop-close')
        }, option)

        return this.each(
			function () {
			    var oBtn = set.popbtn;
			    var oWindow = set.popwindow;
			    var oBg = set.popbg;
			    var oClose = set.popclose;
			    var oH = oWindow.outerHeight();
			    var oW = oWindow.outerWidth();
			    oWindow.css({
			        marginTop: -oH / 2,
			        marginLeft: -oW / 2
			    })

			    oBtn.click(function () {
			        oWindow.fadeIn();
			        oBg.fadeIn();
			    })

			    oClose.click(function () {
			        oWindow.fadeOut();
			        oBg.fadeOut();
			    })

			    /*oBg.click(function(){
					oWindow.fadeOut();
					oBg.fadeOut();
				})*/
			}
		)
    };
})(jQuery)