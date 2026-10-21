function configurarPreviewImagen(
    fileInputId,
    previewId,
    base64Id,
    mimeId,
    maxSizeMB = 2) {

    const fileInput = document.getElementById(fileInputId);
    const preview = document.getElementById(previewId);
    const base64Input = document.getElementById(base64Id);
    const mimeInput = document.getElementById(mimeId);

    if (!fileInput || !preview || !base64Input || !mimeInput) {
        return;
    }

    // Guardar la imagen que se muestra inicialmente.
    const imagenPorDefecto = preview.src;

    // Limpiar archivo, vista previa y datos temporales.
    function limpiarImagen() {
        fileInput.value = '';
        preview.src = imagenPorDefecto;
        base64Input.value = '';
        mimeInput.value = '';
    }

    fileInput.addEventListener('change', function () {

        const file = this.files[0];

        if (!file) {
            return;
        }

        // Validar tamaño.
        if (file.size > maxSizeMB * 1024 * 1024) {

            alert(`La imagen no puede exceder ${maxSizeMB} MB.`);

            limpiarImagen();
            return;
        }

        // Validar tipos permitidos en el navegador.
        const tiposPermitidos = [
            'image/jpeg',
            'image/png',
            'image/gif',
            'image/webp'
        ];

        if (!tiposPermitidos.includes(file.type)) {

            alert('Selecciona una imagen JPG, PNG, GIF o WEBP.');

            limpiarImagen();
            return;
        }

        const reader = new FileReader();

        reader.onload = function (e) {

            preview.src = e.target.result;

            base64Input.value =
                e.target.result.split(',')[1];

            mimeInput.value = file.type;
        };

        reader.onerror = function () {
            alert('No fue posible leer la imagen.');
            limpiarImagen();
        };

        reader.readAsDataURL(file);
    });
}


// function configurarPreviewImagen(
//     fileInputId,
//     previewId,
//     base64Id,
//     mimeId,
//     maxSizeMB = 2) {

//     const fileInput = document.getElementById(fileInputId);

//     if (!fileInput) {
//         return;
//     }

//     fileInput.addEventListener('change', function (event) {

//         const file = event.target.files[0];

//         if (!file) {
//             return;
//         }

//         if (file.size > maxSizeMB * 1024 * 1024) {

//             alert(`La imagen no puede exceder ${maxSizeMB} MB.`);

//             this.value = '';

//             return;
//         }

//         const reader = new FileReader();

//         reader.onload = function (e) {

//             document.getElementById(previewId).src =
//                 e.target.result;

//             document.getElementById(base64Id).value =
//                 e.target.result.split(',')[1];

//             document.getElementById(mimeId).value =
//                 file.type;
//         };

//         reader.readAsDataURL(file);
//     });
// }