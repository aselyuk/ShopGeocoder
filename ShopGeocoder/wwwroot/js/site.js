// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    $('[data-toggle="tooltip"]').tooltip()
})

$('.toast').toast('show')


function getDateInterval() {
    console.log(new Date());
    return setInterval(function () { return new Date(); });
}

function getDate() {
    console.log(new Date());
    return new Date();
}

$(document).ready(function () {
    $('time.timeago').timeago();
});