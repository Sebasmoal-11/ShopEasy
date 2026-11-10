
document.addEventListener("DOMContentLoaded", () => {

    const loginForm = document.getElementById("loginForm");
    const emailInput = document.getElementById("Email");
    const passwordInput = document.getElementById("Password");

    const togglePassword = document.getElementById("togglePassword");
    const eyeIcon = document.getElementById("eyeIcon");

    const loginMessage = document.getElementById("loginMessage");
    const loginButton = document.getElementById("loginButton");
    const buttonText = document.getElementById("buttonText");

    // Mostrar u ocultar contraseña
    togglePassword.addEventListener("click", () => {

        const isPassword = passwordInput.type === "password";

        passwordInput.type = isPassword ? "text" : "password";

        eyeIcon.classList.toggle("bi-eye", !isPassword);
        eyeIcon.classList.toggle("bi-eye-slash", isPassword);

        togglePassword.setAttribute(
            "aria-label",
            isPassword ? "Ocultar contraseña" : "Mostrar contraseña"
        );

        togglePassword.setAttribute(
            "aria-pressed",
            String(isPassword)
        );
    });

    // Mostrar mensajes al usuario
    function showMessage(message, type) {

        loginMessage.textContent = message;

        loginMessage.className = `alert alert-${type}`;
        loginMessage.classList.remove("d-none");
    }

    function hideMessage() {
        loginMessage.textContent = "";
        loginMessage.className = "alert d-none";
    }

    // Enviar formulario al backend
    loginForm.addEventListener("submit", async (event) => {

        event.preventDefault();
        hideMessage();

        const correo = emailInput.value.trim();
        const contrasena = passwordInput.value;

        if (!correo || !contrasena) {
            showMessage("Por favor, completa todos los campos.", "warning");
            return;
        }

        loginButton.disabled = true;
        buttonText.textContent = "Validando...";

        try {

            const response = await fetch("/api/auth/login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    correo: correo,
                    contrasena: contrasena
                })
            });

            const data = await response.json();

            if (!response.ok) {

                showMessage(
                    data.message || "Correo o contraseña incorrectos.",
                    "danger"
                );

                return;
            }

            // Guardar información de la sesión
            sessionStorage.setItem("token", data.token);
            sessionStorage.setItem(
                "usuario",
                JSON.stringify(data.user)
            );

            showMessage(
                `¡Bienvenido, ${data.user.nombreCompleto}!`,
                "success"
            );

            /*
             * Aquí conectaremos posteriormente
             * la navegación hacia el menú principal.
             */

        } catch (error) {

            console.error("Error al iniciar sesión:", error);

            showMessage(
                "No se pudo conectar con el servidor. Inténtalo nuevamente.",
                "danger"
            );

        } finally {

            loginButton.disabled = false;
            buttonText.textContent = "Iniciar sesión";

        }
    });

});
