function Cargando() {
    if ($("#user_login").val() != "" && $("#pass_login").val() != "") {
        $("#login-ingresar").css("display", "none");
        $("#cargando").removeAttr("style");
    }
}