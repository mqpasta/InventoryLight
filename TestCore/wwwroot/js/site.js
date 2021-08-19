﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
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

function getTodayDate() {
    var d = new Date();
    return d.getDate() + "/" + (d.getMonth() + 1) + "/" + d.getFullYear();
}

// set total summary field in the datatable
function footerTotal(api, col, round) {
    //var api = this.api(), data;

    // Remove the formatting to get integer data for summation
    var intVal = function (i) {
        return typeof i === 'string' ?
            i.replace(/[\$,]/g, '') * 1 :
            typeof i === 'number' ?
                i : 0;
    };

    // Total over all pages
    total = api
        .column(col)
        .data()
        .reduce(function (a, b) {
            return intVal(a) + intVal(b);
        }, 0);

    // Total over this page
    //pageTotal = api
    //    .column(col, { page: 'current' })
    //    .data()
    //    .reduce(function (a, b) {
    //        return intVal(a) + intVal(b);
    //    }, 0);


    // Update footer
    $(api.column(col).footer()).html(
        //+ pgTotalVal + '<br/>' +
        total.toFixed(round)
    );
}

function setTableForReport(tableId, columnIndex, roundFactor, fileName) {
    $(tableId).DataTable({
        dom: '<"top"if>rt<"bottom"Blp><"clear">',
        "deferRender": true,
        responsive: true,
        buttons: [
            {
                extend: 'print',
                text: '<i class="bi bi-print"></i> Print',
                messageBottom: 'Report generated on ' + getTodayDate(),
                footer: true,
            },
            {
                extend: 'pdfHtml5',
                footer: true,
                filename: fileName + "_" + getTodayDate().replace(/[/]/g, '-')
            }
        ],
        "footerCallback": function (row, data, start, end, display) {
            var api = this.api(), data;
            if (columnIndex > -1)
                footerTotal(api, columnIndex, roundFactor);


        }
    });
}