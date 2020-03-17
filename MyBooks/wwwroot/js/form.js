MB_FormValidation = {
    init: function (selector) {
        var $formValidate = $(selector);

        $formValidate
            .parsley({
                'excluded': 'input[type=button], input[type=submit], input[type=reset], input[type=hidden], .selectize-input > input'
            })
            .on('form:validated', function () {
                altair_md.update_input($formValidate.find('.md-input-danger'));
            })
            .on('field:validated', function (parsleyField) {
                if ($(parsleyField.$element).hasClass('md-input') || $(parsleyField.$element).is('select')) {
                    altair_md.update_input($(parsleyField.$element));
                }
            });

        window.Parsley.on('field:validate', function () {
            var $server_side_error = $(this.$element).closest('.md-input-wrapper').siblings('.error_server_side');
            if ($server_side_error) {
                $server_side_error.hide();
            }
        });

        // datepicker callback
        $('#val_birth').on('hide.uk.datepicker', function () {
            $(this).parsley().validate();
        });
    }
};