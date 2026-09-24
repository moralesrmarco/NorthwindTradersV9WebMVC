function configurarPreviewImagen(
    fileInputId,
    previewId,
    base64Id,
    mimeId,
    maxSizeMB = 2) {

    const fileInput = document.getElementById(fileInputId);

    if (!fileInput) {
        return;
    }

    fileInput.addEventListener('change', function (event) {

        const file = event.target.files[0];

        if (!file) {
            return;
        }

        if (file.size > maxSizeMB * 1024 * 1024) {

            alert(`La imagen no puede exceder ${maxSizeMB} MB.`);

            this.value = '';

            return;
        }

        const reader = new FileReader();

        reader.onload = function (e) {

            document.getElementById(previewId).src =
                e.target.result;

            document.getElementById(base64Id).value =
                e.target.result.split(',')[1];

            document.getElementById(mimeId).value =
                file.type;
        };

        reader.readAsDataURL(file);
    });
}