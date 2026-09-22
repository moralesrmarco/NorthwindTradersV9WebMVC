document.addEventListener("DOMContentLoaded", function () {

    const txtIdIni = document.getElementById('txtIdIni');
    const txtIdFin = document.getElementById('txtIdFin');

    if (txtIdIni && txtIdFin) {

        txtIdIni.addEventListener('blur', validarIdIni);
        txtIdFin.addEventListener('blur', validarIdFin);

    }

    configurarNavegacion();

    const buttons = document.querySelectorAll(".action-btn");

    buttons.forEach(btn => {

        btn.addEventListener("click", function () {

            buttons.forEach(b => {
                b.style.pointerEvents = "none";
            });


            const overlay = document.getElementById('loadingOverlay');

            if (overlay) {
                overlay.style.display = 'block';
            }

        });

    });

});

function validarIdIni() {

    const txtIdIni = document.getElementById('txtIdIni');
    const txtIdFin = document.getElementById('txtIdFin');

    if (!txtIdIni || !txtIdFin)
        return;

    let idIni = txtIdIni.value.trim();
    let idFin = txtIdFin.value.trim();

    if (idIni === '')
        return;

    let numIni = parseInt(idIni);

    if (isNaN(numIni)) {

        alert('Por favor ingrese un número válido');
        txtIdIni.focus();
        return;

    }

    if (numIni === 0) {

        alert('El valor del Id inicial no puede ser cero');
        txtIdIni.value = '1';
        txtIdIni.focus();
        return;

    }

    if (idFin === '') {

        txtIdFin.value = txtIdIni.value;
        return;

    }

    let numFin = parseInt(idFin);

    if (!isNaN(numFin) && numFin < numIni) {

        txtIdFin.value = txtIdIni.value;

    }
}
function validarIdFin() {

    const txtIdIni = document.getElementById('txtIdIni');
    const txtIdFin = document.getElementById('txtIdFin');

    if (!txtIdIni || !txtIdFin)
        return;

    let idIni = txtIdIni.value.trim();
    let idFin = txtIdFin.value.trim();

    if (idFin === '')
        return;

    let numFin = parseInt(idFin);

    if (isNaN(numFin)) {
        alert('Por favor ingrese un número válido');
        txtIdFin.focus();
        return;
    }

    if (numFin === 0) {
        alert('El valor del Id final no puede ser cero');
        txtIdFin.value = '1';
        txtIdFin.focus();
        validarIdIni();
        return;
    }

    if (idIni === '') {
        txtIdIni.value = txtIdFin.value;
        return;
    }

    let numIni = parseInt(idIni);

    if (!isNaN(numIni) && numIni > numFin) {
        txtIdIni.value = txtIdFin.value;
    }
}

function disableSearchButton() {

    const btnBuscar = document.getElementById('btnBuscar');

    if (btnBuscar) {

        btnBuscar.style.pointerEvents = 'none';

        btnBuscar.innerHTML =
            '<span class="spinner-border spinner-border-sm me-1"></span> Procesando...';

    }

    const overlay = document.getElementById('loadingOverlay');

    if (overlay) {
        overlay.style.display = 'block';
    }

    return true;
}

function restaurarControles() {

    const overlay = document.getElementById('loadingOverlay');

    if (overlay) {
        overlay.style.display = 'none';
    }

    const btnBuscar = document.getElementById('btnBuscar');

    if (btnBuscar) {

        btnBuscar.style.pointerEvents = 'auto';

        btnBuscar.innerHTML =
            '<i class="bi bi-search"></i><span class="d-none d-md-inline"> Buscar</span>';
    }

    document.querySelectorAll(".action-btn")
        .forEach(b => {
            b.classList.remove("disabled");
            b.style.pointerEvents = "auto";
            b.style.opacity = "";
        });
}

window.addEventListener("pageshow", restaurarControles);