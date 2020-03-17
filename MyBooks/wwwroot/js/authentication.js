// variables
var $login_card = $('#login_card'),
    $login_form = $('#login_form'),
    $login_help = $('#login_help'),
    $register_form = $('#register_form'),
    $login_password_reset = $('#login_password_reset'),
    $form_login = $('#formLogin'),
    $form_register = $('#formRegister'),
    $form_reset_password = $('#formResetPassword');


MB_Login = {
    init: function () {
        // show login form (hide other forms)
        var login_form_show = function () {
            $login_form
                .show()
                .siblings()
                .hide();
        };

        // show register form (hide other forms)
        var register_form_show = function () {
            $register_form
                .show()
                .siblings()
                .hide();
        };

        // show login help (hide other forms)
        var login_help_show = function () {
            $login_help
                .show()
                .siblings()
                .hide();
        };

        // show password reset form (hide other forms)
        var password_reset_show = function () {
            $login_password_reset
                .show()
                .siblings()
                .hide();
        };

        $('#login_help_show').on('click', function (e) {
            e.preventDefault();
            // card animation & complete callback: login_help_show
            altair_md.card_show_hide($login_card, undefined, login_help_show, undefined);
        });

        $('#signup_form_show').on('click', function (e) {
            e.preventDefault();
            $(this).fadeOut('280');
            // card animation & complete callback: register_form_show
            altair_md.card_show_hide($login_card, undefined, register_form_show, undefined);
        });

        $('.back_to_login').on('click', function (e) {
            e.preventDefault();
            $('#signup_form_show').fadeIn('280');
            // card animation & complete callback: login_form_show
            altair_md.card_show_hide($login_card, undefined, login_form_show, undefined);
        });

        $('#password_reset_show').on('click', function (e) {
            e.preventDefault();
            // card animation & complete callback: password_reset_show
            altair_md.card_show_hide($login_card, undefined, password_reset_show, undefined);
        });

        $form_register.submit(function (event) {
            event.preventDefault(); //prevent default action 
            MB_Login.register();
        });

        $form_reset_password.submit(function (event) {
            event.preventDefault(); //prevent default action 
            MB_Login.register();
        });
    },
    register: function () {
        var request_method = $form_register.attr("method"); //get form GET/POST method
        var form_data = $form_register.serialize(); //Encode form elements for submission
        $form_register.find(":submit").addClass('disabled').attr('disabled');
        $.ajax({
            type: request_method,
            url: '/api/user',
            data: form_data,
            datatype: "json",
            success: function (result) {
                $form_register.find('input[name="__RequestVerificationToken"]').val(result.verificationToken);
                if (result.code > 0) {
                    MB_Error_Handler.notificate(result);
                    return;
                }
                MB_Notification.show(result.message, 'success');
                $('#register_form .back_to_login').click();
            },
            error: function (xmlhttprequest) {
                MB_Error_Handler.notificate(xmlhttprequest);
            },
            complete: function () { $form_register.find(":submit").removeClass('disabled').removeAttr('disabled'); }
        });
    },
    reset_password: function () {
        return;
        var request_method = $form_reset_password.attr("method"); //get form GET/POST method
        var form_data = $form_reset_password.serialize(); //Encode form elements for submission

        $.ajax({
            type: request_method,
            url: '/api/user/resetpassword',
            data: form_data,
            datatype: "json",
            success: function (result) {
                //GO TO LOGIN
            },
            error: function (xmlhttprequest) {
                MB_Error_Handler.notificate(xmlhttprequest);
            },
            complete: function () { }

        });
    }
};