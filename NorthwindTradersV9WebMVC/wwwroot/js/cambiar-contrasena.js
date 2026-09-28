document.addEventListener("DOMContentLoaded", function () {

    // =========================================================
    // Referencias a los elementos
    // =========================================================

    const modalCambiarContrasena =
        document.getElementById("modalCambiarContrasena");

    const txtContrasenaActual =
        document.getElementById("txtContrasenaActual");

    const txtNuevaContrasena =
        document.getElementById("txtNuevaContrasena");

    const txtConfirmarContrasena =
        document.getElementById("txtConfirmarContrasena");

    const btnMostrarContrasenaActual =
        document.getElementById("btnMostrarContrasenaActual");

    const btnMostrarNuevaContrasena =
        document.getElementById("btnMostrarNuevaContrasena");

    const btnMostrarConfirmarContrasena =
        document.getElementById("btnMostrarConfirmarContrasena");

    const btnCambiarContrasena =
        document.getElementById("btnCambiarContrasena");

    const mensajeCambiarContrasena =
        document.getElementById("mensajeCambiarContrasena");


    // =========================================================
    // Mostrar / ocultar contraseña
    // =========================================================

    function alternarVisibilidad(input, boton) {

        if (!input || !boton)
            return;

        const icono = boton.querySelector("i");

        if (input.type === "password") {

            input.type = "text";

            if (icono) {
                icono.classList.remove("bi-eye");
                icono.classList.add("bi-eye-slash");
            }

            boton.title = "Ocultar contraseña";
        }
        else {

            input.type = "password";

            if (icono) {
                icono.classList.remove("bi-eye-slash");
                icono.classList.add("bi-eye");
            }

            boton.title = "Mostrar contraseña";
        }
    }


    // =========================================================
    // Eventos mostrar / ocultar
    // =========================================================

    if (btnMostrarContrasenaActual) {

        btnMostrarContrasenaActual.addEventListener("click", function () {

            alternarVisibilidad(
                txtContrasenaActual,
                btnMostrarContrasenaActual
            );

        });
    }


    if (btnMostrarNuevaContrasena) {

        btnMostrarNuevaContrasena.addEventListener("click", function () {

            alternarVisibilidad(
                txtNuevaContrasena,
                btnMostrarNuevaContrasena
            );

        });
    }


    if (btnMostrarConfirmarContrasena) {

        btnMostrarConfirmarContrasena.addEventListener("click", function () {

            alternarVisibilidad(
                txtConfirmarContrasena,
                btnMostrarConfirmarContrasena
            );

        });
    }


    // =========================================================
    // Mostrar mensaje
    // =========================================================

    function mostrarMensaje(mensaje, tipo) {

        if (!mensajeCambiarContrasena)
            return;

        mensajeCambiarContrasena.textContent = mensaje;

        mensajeCambiarContrasena.classList.remove(
            "d-none",
            "alert-success",
            "alert-danger",
            "alert-warning",
            "alert-info"
        );

        mensajeCambiarContrasena.classList.add(
            "alert-" + tipo
        );
    }


    // =========================================================
    // Limpiar mensaje
    // =========================================================

    function limpiarMensaje() {

        if (!mensajeCambiarContrasena)
            return;

        mensajeCambiarContrasena.textContent = "";

        mensajeCambiarContrasena.classList.add("d-none");

        mensajeCambiarContrasena.classList.remove(
            "alert-success",
            "alert-danger",
            "alert-warning",
            "alert-info"
        );
    }


    // =========================================================
    // Limpiar formulario
    // =========================================================

    function limpiarFormulario() {

        if (txtContrasenaActual) {
            txtContrasenaActual.value = "";
            txtContrasenaActual.type = "password";
        }

        if (txtNuevaContrasena) {
            txtNuevaContrasena.value = "";
            txtNuevaContrasena.type = "password";
        }

        if (txtConfirmarContrasena) {
            txtConfirmarContrasena.value = "";
            txtConfirmarContrasena.type = "password";
        }


        // -----------------------------------------------------
        // Restaurar icono contraseña actual
        // -----------------------------------------------------

        if (btnMostrarContrasenaActual) {

            const icono =
                btnMostrarContrasenaActual.querySelector("i");

            if (icono) {
                icono.classList.remove("bi-eye-slash");
                icono.classList.add("bi-eye");
            }

            btnMostrarContrasenaActual.title =
                "Mostrar contraseña";
        }


        // -----------------------------------------------------
        // Restaurar icono nueva contraseña
        // -----------------------------------------------------

        if (btnMostrarNuevaContrasena) {

            const icono =
                btnMostrarNuevaContrasena.querySelector("i");

            if (icono) {
                icono.classList.remove("bi-eye-slash");
                icono.classList.add("bi-eye");
            }

            btnMostrarNuevaContrasena.title =
                "Mostrar contraseña";
        }


        // -----------------------------------------------------
        // Restaurar icono confirmación
        // -----------------------------------------------------

        if (btnMostrarConfirmarContrasena) {

            const icono =
                btnMostrarConfirmarContrasena.querySelector("i");

            if (icono) {
                icono.classList.remove("bi-eye-slash");
                icono.classList.add("bi-eye");
            }

            btnMostrarConfirmarContrasena.title =
                "Mostrar contraseña";
        }

        limpiarMensaje();
    }


    // =========================================================
    // Cambiar contraseña
    // =========================================================

    if (btnCambiarContrasena) {

        btnCambiarContrasena.addEventListener("click", async function () {

            limpiarMensaje();


            // -------------------------------------------------
            // Obtener valores
            // -------------------------------------------------

            const contrasenaActual =
                txtContrasenaActual?.value.trim() ?? "";

            const nuevaContrasena =
                txtNuevaContrasena?.value.trim() ?? "";

            const confirmarContrasena =
                txtConfirmarContrasena?.value.trim() ?? "";


            // -------------------------------------------------
            // Validaciones básicas
            // -------------------------------------------------

            if (!contrasenaActual) {

                mostrarMensaje(
                    "Debe ingresar su contraseña actual.",
                    "danger"
                );

                txtContrasenaActual?.focus();

                return;
            }


            if (!nuevaContrasena) {

                mostrarMensaje(
                    "La nueva contraseña es obligatoria.",
                    "danger"
                );

                txtNuevaContrasena?.focus();

                return;
            }


            if (!confirmarContrasena) {

                mostrarMensaje(
                    "La confirmación de la contraseña es obligatoria.",
                    "danger"
                );

                txtConfirmarContrasena?.focus();

                return;
            }


            if (nuevaContrasena !== confirmarContrasena) {

                mostrarMensaje(
                    "La nueva contraseña y la confirmación de la contraseña no coinciden.",
                    "danger"
                );

                txtNuevaContrasena?.focus();

                return;
            }


            // -------------------------------------------------
            // Deshabilitar botón mientras se procesa
            // -------------------------------------------------

            const textoOriginal =
                btnCambiarContrasena.innerHTML;

            btnCambiarContrasena.disabled = true;

            btnCambiarContrasena.innerHTML =
                '<span class="spinner-border spinner-border-sm me-1"></span>' +
                'Cambiando...';


            try {

                // ---------------------------------------------
                // Enviar información al servidor
                // ---------------------------------------------

                const token = document.querySelector(
                    '#modalCambiarContrasena input[name="__RequestVerificationToken"]'
                ).value;

                const response = await fetch(
                    "/Account/CambiarPassword",
                    {
                        method: "POST",

                        headers: {
                            "Content-Type": "application/json",
                            "RequestVerificationToken": token
                        },

                        body: JSON.stringify({
                            contrasenaActual: contrasenaActual,
                            nuevaContrasena: nuevaContrasena,
                            confirmarContrasena: confirmarContrasena
                        })
                    }
                );


                // ---------------------------------------------
                // Obtener respuesta
                // ---------------------------------------------

                 const resultado = await response.json();

                // ---------------------------------------------
                // Mostrar resultado
                // ---------------------------------------------

                if (resultado.ok) {

                    mostrarMensaje(
                        resultado.mensaje,
                        "success"
                    );

                    // Limpiar las contraseñas
                    if (txtContrasenaActual)
                        txtContrasenaActual.value = "";

                    if (txtNuevaContrasena)
                        txtNuevaContrasena.value = "";

                    if (txtConfirmarContrasena)
                        txtConfirmarContrasena.value = "";

                }
                else {

                    mostrarMensaje(
                        resultado.mensaje,
                        "danger"
                    );

                    // -------------------------------------------------
                    // Cerrar modal después de 3 intentos fallidos
                    // -------------------------------------------------

                    if (resultado.cerrar === true) {

                        setTimeout(function () {

                            const modal =
                                bootstrap.Modal.getInstance(
                                    modalCambiarContrasena
                                );

                            if (modal) {
                                modal.hide();
                            }

                        }, 4000);
                    }
                }

            }
            catch (error) {

                console.error(
                    "Error al cambiar la contraseña:",
                    error
                );

                mostrarMensaje(
                    "Ocurrió un error al intentar cambiar la contraseña.",
                    "danger"
                );

            }
            finally {

                // ---------------------------------------------
                // Restaurar botón
                // ---------------------------------------------

                btnCambiarContrasena.disabled = false;

                btnCambiarContrasena.innerHTML =
                    textoOriginal;
            }

        });
    }


    // =========================================================
    // Al abrir el modal
    // =========================================================

    if (modalCambiarContrasena) {

        modalCambiarContrasena.addEventListener(
            "show.bs.modal",
            function () {

                limpiarFormulario();

            }
        );


        modalCambiarContrasena.addEventListener(
            "shown.bs.modal",
            function () {

                if (txtContrasenaActual) {

                    txtContrasenaActual.value = "";

                    txtContrasenaActual.focus();
                }

            }
        );


        // =====================================================
        // Al cerrar el modal
        // =====================================================

        modalCambiarContrasena.addEventListener(
            "hidden.bs.modal",
            function () {

                limpiarFormulario();

            }
        );
    }

});