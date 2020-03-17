MB_Tour = {
    runHome: function() {
        var enjoyhint_instance = new EnjoyHint({});

        var enjoyhint_script_steps = [
            {
                "next .sidebar_logo": Translator.TourPresentacion,
                showSkip: false,
                "nextButton": { text: Translator.Siguiente }
            },
            {
                "next #header_main": Translator.TourCabecera,
                showSkip: false,
                "nextButton": { text: Translator.Siguiente }
            },
            {
                "next #full_screen_toggle": Translator.TourPantallaCompleta,
                shape: 'circle',
                radius: 30,
                showSkip: false,
                "nextButton": { text: Translator.Siguiente }
            },
            {
                "next #main_search_btn": Translator.TourBuscador ,
                shape: 'circle',
                radius: 30,
                showSkip: false,
                "nextButton": { text: Translator.Siguiente }
            },
            {
                "next #sidebar_main": Translator.TourMenuPrincipal,
                showSkip: false,
                "nextButton": { text: Translator.Siguiente }
            },
            {
                "next .mb_book_list_slider:first": Translator.TourListasLibros,
                showSkip: false,
                "nextButton": { text: Translator.Siguiente }
            },
            {
                "next .mb_book_card_mini:first": Translator.TourLibros,
                showSkip: false,
                "nextButton": { text: Translator.Siguiente }
            },
            {
                "next .uk-nestable-handle:first": Translator.TourReordenarListasLibros,
                shape: 'circle',
                radius: 20,
                showSkip: false,
                "nextButton": { text: Translator.Siguiente }
            },
            {
                "next #bntAddLista": Translator.TourAnadirListasLibros,
                shape: 'circle',
                radius: 50,
                showSkip: false,
                "nextButton": { text: Translator.Finalizar }
            }
        ];

        // set script config
        enjoyhint_instance.set(enjoyhint_script_steps);

        // run Enjoyhint script
        enjoyhint_instance.run();
    }
}