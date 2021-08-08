// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    var holder = $('#ModalPlaceHolder');
    $('button[data-toggle="ajax-modal"]').click(function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            holder.html(data);
            holder.find('.modal').modal('show');
        })
    })

    holder.on('click', '[data-save=modal]', function (event) {
        var form = $(this).parents('.modal').find('form');
        var url = form.attr('action');
        var data = form.serialize();
        $.post(url, data).done(function (data) {
            holder.find('.modal').modal('hide');
            location.reload(true);  
        })
        
    })
})