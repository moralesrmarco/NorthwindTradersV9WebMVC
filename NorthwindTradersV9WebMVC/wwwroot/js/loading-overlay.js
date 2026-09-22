function mostrarLoadingOverlay() {
    const overlay = document.getElementById("loadingOverlay");
    if (overlay) {
        overlay.style.display = "block";
    }
}

function ocultarLoadingOverlay() {
    const overlay = document.getElementById("loadingOverlay");
    if (overlay) {
        overlay.style.display = "none";
    }
}

document.addEventListener("DOMContentLoaded", function () {

    ocultarLoadingOverlay();
    // Todos los formularios
    document.querySelectorAll("form").forEach(function (formulario) {
        formulario.addEventListener("submit", function (e) {
            const boton = e.submitter;
            if (boton && boton.hasAttribute("data-no-overlay")) {
                return;
            }
            mostrarLoadingOverlay();
        });
    });
    // Paginación
    document.querySelectorAll(".pagination a").forEach(function (link) {
        link.addEventListener("click", function () {
            if (!link.closest(".disabled")) {
                mostrarLoadingOverlay();
            }
        });
    });
    // Menú de navegación
    document.querySelectorAll(".navbar a[href]").forEach(function (link) {
        link.addEventListener("click", function () {
            const href = link.getAttribute("href");

            if (href && !href.startsWith("#")) {
                mostrarLoadingOverlay();
            }
        });
    });
    // Botones o enlaces marcados    
    document.querySelectorAll("[data-show-overlay]").forEach(function (element) {
        element.addEventListener("click", function () {
            mostrarLoadingOverlay();
        });
    });
    // Reportes PDF
    document.querySelectorAll("iframe[data-report-src]").forEach(function (iframe) {
        mostrarLoadingOverlay();
        iframe.addEventListener("load", function () {
            ocultarLoadingOverlay();
        });
        iframe.src = iframe.dataset.reportSrc;
    });
    // Reportes PDF generados por POST
    const visorReporte = document.getElementById("visorReporte");
    if (visorReporte && !visorReporte.dataset.reportSrc) {
        visorReporte.addEventListener("load", function () {
            ocultarLoadingOverlay();
        });
    }
});

window.addEventListener("pageshow", function () {
    ocultarLoadingOverlay();
});

function esMovil() {
    return /Android|iPhone|iPad|iPod|Opera Mini|IEMobile/i.test(navigator.userAgent);
}
