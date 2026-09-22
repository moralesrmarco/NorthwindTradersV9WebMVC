function configurarAccionesListado(selector = '.action-btn') {

    const buttons = document.querySelectorAll(selector);

    buttons.forEach(btn => {

        btn.addEventListener('click', function () {

            buttons.forEach(b => {
                b.style.pointerEvents = 'none';
            });

            const overlay = document.getElementById('loadingOverlay');

            if (overlay) {
                overlay.style.display = 'block';
            }
        });

    });

    window.addEventListener('pageshow', function () {

        const overlay = document.getElementById('loadingOverlay');

        if (overlay) {
            overlay.style.display = 'none';
        }

        buttons.forEach(b => {
            b.style.pointerEvents = 'auto';
        });

    });
}