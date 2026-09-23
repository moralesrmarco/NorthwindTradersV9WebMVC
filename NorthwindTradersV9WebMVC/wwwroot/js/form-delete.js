function configurarEliminar(btnId) {

    const btn = document.getElementById(btnId);

    if (!btn) {
        return true;
    }

    if (btn.disabled) {
        return false;
    }

    btn.disabled = true;

    btn.innerHTML =
        '<span class="spinner-border spinner-border-sm me-1"></span> Procesando...';

    mostrarOverlay();

    return true;
}
function mostrarOverlay() {

    const overlay = document.getElementById('loadingOverlay');

    if (overlay) {
        overlay.style.display = 'block';
    }
}
$(function () {

    $('.btn-cancelar').on('click', function () {
        mostrarOverlay();
    });

    window.addEventListener('pageshow', function () {

        const overlay = document.getElementById('loadingOverlay');

        if (overlay) {
            overlay.style.display = 'none';
        }
    });
});