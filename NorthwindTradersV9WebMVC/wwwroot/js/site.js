// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.dropdown-submenu > a').forEach(function (element) {
        element.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            let submenu = this.nextElementSibling;
            if (submenu) {
                submenu.classList.toggle('show');
            }
        });
    });
});

// Cuando la página vuelve desde el historial
window.addEventListener("pageshow", function () {

    document.querySelectorAll(".action-btn").forEach(btn => {

        btn.classList.remove("disabled");

        btn.removeAttribute("disabled");

        btn.style.pointerEvents = "auto";

    });

});