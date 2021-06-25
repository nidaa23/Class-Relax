;(function ($) {
  $.fn.AjaxModal = function (options) {

    createContainer();
    var settings = $.extend({
      modalWidth: '50%',
      fadeInDuration: 300,
      fadeOutDuration: 300,
      overlayOpacity: 0.3,
      closeButton: '&times;',
      dismissable: true,
      showTitlebar: 'always',
      caching: false
    }, options);

    //Bind to click
    this.bind("click.AjaxModal", function (e) {
      var form = $(this.form);
      if (form.length) {
        makeRequest(form.attr('method'), form.attr('action'), form.serialize(), $(this).data('title'))
      } else {
        makeRequest('GET', $(this).attr('href'), null, $(this).data('title'))
      }
      return false; //Make unclickable
    });

    //bind to click for closing
    $('body').on('click', '.AjaxModal-close', function () {
      closeModal();
    });
    $('body').on('click', '.AjaxModal-overlay', function () {
      if (settings.dismissable) {
        closeModal();
      }
    });
    $(document).keyup(function (e) {
      if (e.keyCode == 27 && settings.dismissable) { // escape key maps to keycode `27`
        closeModal();
      }
    });

    //Close the Modal
    function closeModal() {
      $('.AjaxModal-overlay, #ajax-modal').fadeOut(settings.fadeOutDuration, function () {
      $('.AjaxModal-overlay').remove();
       $('#ajax-modal').empty();
      });
    }

    //Open the modal
    function openModal(content, title) {
      var titleText = '';
      var htmlClass = 'AjaxModal-nobg';
      if (title) {
        titleText = title;
      }

      if(settings.showTitlebar == 'always') {
        htmlClass = '';
      }
      else if(settings.showTitlebar == 'whenAvailable') {
          if (title) {
            htmlClass = '';
          }
      }
      else {
          htmlClass = ''; 
      }

      $('#ajax-modal').html('<div class="AjaxModal-top ' + htmlClass + '">' + titleText + '<span class="AjaxModal-close">' + settings.closeButton + '</div></div><div class="AjaxModal-content">' + content + '</div>').css('width', settings.modalWidth).fadeIn(settings.fadeInDuration);

      //Setup the overlay
      $('body').append('<div class="AjaxModal-overlay"></div>');
      $('.AjaxModal-overlay').css('opacity', settings.overlayOpacity).hide().fadeIn(settings.fadeInDuration);
    }

    //create container for modal
    function createContainer() {
      $('body').append('<div class="AjaxModal-modal" id="ajax-modal"></div>');
      $('#ajax-modal').hide();
    }

    //Make the request 
    function makeRequest(method, url, data, title) {
      $.ajax({
        url: url,
        type: method,
        data: data,
        crossDomain: true,
        cache: settings.caching,
        success: function (result) {
          openModal(result, title);
        }
      });
    }
  };
}(jQuery));