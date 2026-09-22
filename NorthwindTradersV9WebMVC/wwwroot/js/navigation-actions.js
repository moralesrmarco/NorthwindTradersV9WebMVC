function configurarNavegacion(selector = '.navigation-btn') {

    const buttons = document.querySelectorAll(selector);

    buttons.forEach(btn => {

        btn.addEventListener('click', function () {

            buttons.forEach(b => {
                b.style.pointerEvents = 'none';
            });

            const overlay =
                document.getElementById('loadingOverlay');

            if (overlay) {
                overlay.style.display = 'block';
            }
        });
    });
}


function openCenteredPopup(url, title, w, h) {

    const dualScreenLeft =
        window.screenLeft !== undefined ? window.screenLeft : window.screenX;

    const dualScreenTop =
        window.screenTop !== undefined ? window.screenTop : window.screenY;


    const width =
        window.innerWidth ||
        document.documentElement.clientWidth ||
        screen.width;


    const height =
        window.innerHeight ||
        document.documentElement.clientHeight ||
        screen.height;


    const left = ((width / 2) - (w / 2)) + dualScreenLeft;
    const top = ((height / 2) - (h / 2)) + dualScreenTop;


    const newWindow = window.open(
        url,
        title,
        'scrollbars=yes,resizable=yes,toolbar=no,location=no,status=no,menubar=no,width='
        + w +
        ',height='
        + h +
        ',top='
        + top +
        ',left='
        + left
    );


    if (window.focus && newWindow) {
        newWindow.focus();
    }
}


function disableAndOpen(button, url) {

    // Evitar doble clic
    button.onclick = null;

    // Marcar botón deshabilitado
    button.classList.add("disabled");

    // Abrir reporte
    openCenteredPopup(
        url,
        'ReporteEmpleado',
        900,
        700
    );
}