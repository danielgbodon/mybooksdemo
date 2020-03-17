class MB_Paginator {
    static loadPagination(selector, totalItems, itemsPerPage, currentPage, functionName) {
        $(selector).html("");
        if (totalItems == 0) return;

        let totalPages = Math.trunc(totalItems / itemsPerPage);
        let pagesToShow = (totalPages <= 5) ? totalPages : 7;
        let enabledPrev = (currentPage == 1) ? 'uk-disabled' : '';
        let enabledNext = (currentPage == totalPages) ? 'uk-disabled' : '';
        $(selector).append('<li class="' + enabledPrev + '"><a onclick="' + functionName + '(' + (currentPage - 1) + ')"><i class="uk-icon-angle-double-left"></i></a></li>');

        let firstPage = (currentPage - 3 < 1) ? 1 : currentPage - 3;
        let lastPage = (currentPage + 3 > totalPages) ? totalPages : currentPage + 3;
        for (let i = firstPage; i < lastPage; i++) {
            let selected = (currentPage == i) ? 'uk-active' : '';
            $(selector).append('<li class="' + selected + '"><a onclick="' + functionName + '(' + i + ')">' + i + '</a></li>');
        }

        if (totalPages > pagesToShow) {
            let selected = (currentPage == totalPages) ? 'uk-active' : '';
            $(selector).append('<li><span>&hellip;</span></li>');
            $(selector).append('<li class="' + selected + '"><a onclick="' + functionName + '(' + totalPages + ')">' + totalPages + '</a></li>');
        }

        $(selector).append('<li class="' + enabledNext + '"><a onclick="' + functionName + '(' + (currentPage + 1) + ')"><i class="uk-icon-angle-double-right"></i></a></li>');
    }
}

function html_entity_decode(message) {
    return message.replace(/[<>'"]/g, function (m) {
        return '&' + {
            '\'': 'apos',
            '"': 'quot',
            '&': 'amp',
            '<': 'lt',
            '>': 'gt',
        }[m] + ';';
    });
}

MB_Book = {
    infoModalSelector: '#bookInfoModal',
    bookCardTemplateSelector: '#bookCardTemplate',
    bookCardMiniTemplateSelector: '#bookCardMiniTemplate',
    bookListItemViewSelector: '#bookListItemView',
    search: function (textSearch, page, successFunction, errorFunction, completeFunction){
        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            url: "/api/book",
            data: { search: textSearch, page: page },
            datatype: "json",
            success: function (result) { successFunction(result) },
            error: function (xmlhttprequest, textstatus, errorthrown) {
                if (errorFunction != undefined) errorFunction(xmlhttprequest, textstatus, errorthrown)
            },
            complete: function () {
                if (completeFunction != undefined) completeFunction()
            }
        });
    },
    addCardView: function (selector, book) {
        let template = $(MB_Book.bookCardTemplateSelector).clone();
        let node = template.prop('content');
        $(node).find('.mb_book_title').html(book.title);
        $(node).find('.mb_book_author').html((book.authors == null) ? 'Autor desconocido' : book.authors.join(', '));
        $(node).find('.mb_book_desc').html((book.description == null) ? '' : book.description);
        $(node).find('.mb_book_img').attr('src', book.smallImage).attr('alt', book.title);
        $(node).find('.mb_book_id_item').attr('data-book-id', book.id);
        MB_StartRating.load(book.stars, book.ratings, $(node).find('.mb_start_rating_view'));
        $(selector).append(node);
        MB_BookList.buttonChangeAppearance(book.id, book.userBookListId);
    },
    cardClick: function (selector) {
        MB_Book.showInfoModal($(selector).closest('.mb_book_card'));
    },
    addCardMiniView: function (selector, book, container) {
        let template = $(MB_Book.bookCardMiniTemplateSelector).clone();
        let node = template.prop('content');
        $(node).find('.mb_book_card_mini').attr('data-book-id', book.id);
        $(node).find('.mb_book_title').html(book.title);
        let desc = $.parseHTML($("<div/>").html(book.description).text());
        $(node).find('.mb_book_desc').html(desc);
        $(node).find('.mb_book_author').html((book.authors == null) ? 'Autor desconocido' : book.authors.join(', '));
        $(node).find('.mb_book_img').attr('src', book.image).attr('alt', book.title);

        let finalElem = node;
        if (container != undefined) {
            finalElem = document.createElement(container);
            finalElem.appendChild(node);
        }

        $(selector).append(finalElem);
    },
    getListItemView: function(book) {
        let template = $(MB_Book.bookListItemViewSelector).clone();
        let node = template.prop('content');
        $(node).find('.mb_book_title').html(book.title);
        $(node).find('.mb_book_author').html((book.authors == null) ? 'Autor desconocido' : book.authors.join(', '));
        $(node).find('.mb_book_img').attr('src', book.image).attr('alt', book.title);

        var itemContent = document.createElement('div');
        itemContent.append(node);

        return $(itemContent).html();
    },
    showInfoModal: function (identifier) {
        let bookId = $(identifier).attr('data-book-id');
        let imgSrc = $(identifier).find('.mb_book_img').attr('src');
        let title = $(identifier).find('.mb_book_title').html();
        let authors = $(identifier).find('.mb_book_author').html();
        let desc = $.parseHTML($(identifier).find('.mb_book_desc').html());

        $(MB_Book.infoModalSelector).find('.mb_book_img').attr('src', imgSrc).attr('alt', title);
        $(MB_Book.infoModalSelector).find('.mb_book_title').html(title);
        $(MB_Book.infoModalSelector).find('.mb_book_author').html(authors);
        $(MB_Book.infoModalSelector).find('.mb_book_desc').html(desc);
        $(MB_Book.infoModalSelector).find('#btnViewDetails').attr('data-book-id', bookId);
        UIkit.modal(MB_Book.infoModalSelector).show();
    },
    viewDetails: function (identifier) {    
        let idBook = $(identifier).attr('data-book-id');
        window.location.href = '/book/details/' + idBook;
    },
    viewReview: function (identifier) {       
        let idBook = $(identifier).data('book-id');
        console.log("See opinions " + idBook);
    }
}

MB_Book_SearchSelector = {
    init: function (selector, onChangeFunction) {
        $(selector).selectize({
            valueField: 'id',
            labelField: 'title',
            searchField: 'title',
            options: [],
            create: false,
            persist: false,
            dropdownOffsetTop: 50,
            placeholder: Translator.BuscaAquiLibro + '...',
            render: {
                option: function (item) {
                    return MB_Book.getListItemView(item);
                }
            },
            load: function (query, callback) {
                let btnDelete = $(selector).closest('.uk-grid').find('.mb_btn_delete');
                if (query.length < 3) {
                    MB_Book_SearchSelector.clean(btnDelete[0]);
                    return callback();
                }

                let loader = $(selector).closest('.uk-grid').find('.md-preloader');
                let deleteDisplay = $(btnDelete).css('display');
                btnDelete.css('display', 'none');
                loader.css('display', 'block');
                MB_Book.search(query, 0,
                    function (res) { callback(res.items) },
                    callback(),
                    function () {
                        btnDelete.css('display', deleteDisplay);
                        loader.css('display', 'none');
                        $(selector + ' .selectize-dropdown-content').animate({ scrollTop: (0) }, 'slow');
                    }
                );
            },
            onChange: function (value) {
                let item = this.options[value];
                if (onChangeFunction != undefined) onChangeFunction(value, item);
                $(selector).closest('.uk-grid').find('.mb_btn_delete').css('display', (value == "") ? 'none' : 'block');
            }
        });
    },
    clean: function(btnDeleteSelector) {
        var $select = $(btnDeleteSelector).closest('.uk-grid').find('select.mb_book_search_selector').selectize();
        var control = $select[0].selectize;
        if (control.options.length == 0) return;
        
        control.clear();
        control.clearOptions();
        
    }
}

MB_BookList = {
    formSelector: "#formBookList",
    modalSelector: "#modalAddBookList",
    callbackFunction: undefined,
    initFormModal: function (callbackFunction) {
        $(MB_BookList.formSelector + " #colorError").css('visibility', 'hidden');
        MB_ColorPalette.init(MB_BookList.formSelector + ' #bookListColorPalette');

        $(MB_BookList.formSelector).submit(function (event) {
            event.preventDefault(); //prevent default action 
            if (!MB_ColorPalette.verificate('#bookListColorPalette')) {
                return;
            }
            if ($(MB_BookList.formSelector + ' input[name$="Id"]').val() == "")
                MB_BookList.add(callbackFunction);
            else
                MB_BookList.edit(callbackFunction);
            
        });

        $(document).on("keypress", MB_BookList.formSelector + " input", function () {
            if (event.which == 13) {
                event.preventDefault();
                $(MB_BookList.formSelector).submit();
            }
        });
    },
    add: function () {
        let form = $(MB_BookList.formSelector);
        let form_data = form.serialize(); //Encode form elements for submission
        form.find(":submit").addClass('disabled').attr('disabled');
        $.ajax({
            type: 'post',
            url: '/api/booklist',
            data: form_data,
            datatype: "json",
            success: function (result) {
                //MB_Notification.show(result.message, 'success');
                $(MB_BookList.modalSelector).find('button.uk-close').click();
                if (MB_BookList.callbackFunction != undefined) {
                    MB_BookList.callbackFunction(result.result);
                }
                form.find('input[name="__RequestVerificationToken"]').val(result.verificationToken);

                let bookId = form.find('input[name="BookId"]').val();
            },
            error: function (xmlhttprequest) {
                MB_Error_Handler.notificate(xmlhttprequest);
            },
            complete: function () { form.find(":submit").removeClass('disabled').removeAttr('disabled'); }
        });
    },
    addListBookAndBook: function (identifier) {
        let bookId = $(identifier).attr('data-book-id');
        $('#formBookList input[name$="BookId"]').val(bookId);
        MB_BookList.callbackFunction = function (bookList) {
            console.log(identifier);
            $(identifier).closest(".uk-nav-dropdown").find('#firstDefaultElement').
                before('<li><a class="mb_book_id_item uk-dropdown-close" data-book-id="' + bookId + '" data-list-color="' + bookList.color + '" data-list-id="' + bookList.id + '" onclick="MB_BookList.addToList(this)"><i class="uk-icon-circle uk-margin-small-right" style="color: ' + bookList.color + '"></i><span id="listName">' + bookList.name + '</span></a></li>');
            MB_BookList.buttonChangeAppearance(bookId, bookList.id);
        };
        
        UIkit.modal(MB_BookList.modalSelector).show();
    },
    edit: function () {
        let form = $(MB_BookList.formSelector);
        let form_data = form.serialize(); //Encode form elements for submission
        form.find(":submit").addClass('disabled').attr('disabled');
        $.ajax({
            type: 'put',
            url: '/api/booklist',
            data: form_data,
            datatype: "json",
            success: function (result) {
                form.find('input[name="__RequestVerificationToken"]').val(result.verificationToken);
                if (result.code > 0) {
                    MB_Error_Handler.notificate(result);
                    return;
                }

                if (MB_BookList.callbackFunction != undefined) {
                    MB_BookList.callbackFunction(result.result);
                }
            },
            error: function (xmlhttprequest) {
                MB_Error_Handler.notificate(xmlhttprequest);
            },
            complete: function () { form.find(":submit").removeClass('disabled').removeAttr('disabled'); }
        });
    },
    reorder: function (orderedIds, callbackFunction) {
        $.ajax({
            type: 'put',
            url: '/api/booklist/reorder',
            data: { OrderedIds: orderedIds},
            datatype: "json",
            success: function (result) {
                if (result.code != 0) {
                    MB_Error_Handler.notificate(result);
                    return;
                }
                $(MB_BookList.modalSelector).find('button.uk-close').click();
                if (callbackFunction != undefined) {
                    callbackFunction(result.result);
                }
            },
            error: function (xmlhttprequest) {
                MB_Error_Handler.notificate(xmlhttprequest);
            },
            complete: function () { }
        });
    },
    delete: function (id, callbackFunction) {
        $.ajax({
            type: 'delete',
            url: '/api/booklist',
            data: { id: id },
            datatype: "json",
            success: function (result) {
                //MB_Notification.show(result.message, 'success');
                if (callbackFunction != undefined) {
                    callbackFunction(result.result);
                }
            },
            error: function (xmlhttprequest) {
                MB_Error_Handler.notificate(xmlhttprequest);
            },
            complete: function () { }
        });
    },
    showToAdd: function (callbackFunction) {
        let form = $(MB_BookList.formSelector);
        let bookId = $('input[name$="BookId"]').val();
        let token = form.find('input[name="__RequestVerificationToken"]').val();
        form.find("input").val('');
        form.find('input[name$="BookId"]').val(bookId);
        form.find('input[name="__RequestVerificationToken"]').val(token);
        form.find(".md-input-success").removeClass('md-input-success');
        form.find(".md-input-filled").removeClass('md-input-filled');
        form.find("#colorPalette .k-state-selected").removeClass('k-state-selected');
        form.find(":submit").html(Translator.CrearLista);

        MB_BookList.callbackFunction = callbackFunction;
        UIkit.modal(MB_BookList.modalSelector).show();
    },
    showToEdit: function (bookList, callbackFunction) {
        let form = $(MB_BookList.formSelector);
        form.find('input[name$="Id"]').val(bookList.id);
        form.find('input[name$="BookId"]').val("");
        form.find('input[name$="Name"]').val(bookList.name);
        form.find('input[name$="Name"]').closest('div.md-input-wrapper').addClass('md-input-filled').addClass('md-input-wrapper-success');
        form.find("#colorPalette .k-state-selected").removeClass('k-state-selected');
        form.find('#colorPalette td[aria-label$="' + bookList.color.toLowerCase() + '"]').addClass('k-state-selected');
        form.find('input[name$="Color"]').val(bookList.color.toLowerCase());
        form.find(":submit").html(Translator.GuardarCambios);

        MB_BookList.callbackFunction = callbackFunction;
        UIkit.modal(MB_BookList.modalSelector).show();
    },
    addToList: function (identifier) {
        let idBook = $(identifier).data('book-id');
        let idList = $(identifier).data('list-id');

        let button = $("#btnAddLibroLista[data-book-id='" + idBook + "']");
        button.addClass('disabled').attr('disabled');
        $.ajax({
            type: 'post',
            url: '/api/booklist/book',
            data: { BookId: idBook, BookListId: idList },
            datatype: "json",
            success: function (result) {
                if (result.code > 0) {
                    MB_Error_Handler.notificate(result);
                    return;
                }

                MB_BookList.buttonChangeAppearance(idBook, result.result.id);
            },
            error: function (xmlhttprequest) {
                MB_Error_Handler.notificate(xmlhttprequest);
            },
            complete: function () { button.removeClass('disabled').removeAttr('disabled'); }
        });
    },
    deleteToList: function (identifier) {
        let idBook = $(identifier).attr('data-book-id');
        let idList = $(identifier).attr('data-list-id');

        let button = $("#btnAddLibroLista[data-book-id='" + idBook + "']");
        button.addClass('disabled').attr('disabled');
        $.ajax({
            type: 'delete',
            url: '/api/booklist/book',
            data: { BookId: idBook, BookListId: idList },
            datatype: "json",
            success: function (result) {
                if (result.code != 0) {
                    MB_Error_Handler.notificate(result);
                    return;
                }
                //MB_Notification.show(result.message, 'success');
                MB_BookList.buttonChangeAppearance(idBook, 0);
            },
            error: function (xmlhttprequest) {
                MB_Error_Handler.notificate(xmlhttprequest);
            },
            complete: function () { button.removeClass('disabled').removeAttr('disabled'); }
        });
    },
    buttonChangeAppearance: function (idBook, idList) {
        let button = $('#btnAddLibroLista[data-book-id="' + idBook + '"]');
        let textButton = Translator.AnadirLibroLista;
        let color = '#9e9e9e';
        let deleteVisibility = 'none';
        if (idList > 0) { 
            let itemOption = button.siblings('.uk-dropdown').find('li a[data-list-id="' + idList + '"]');
            textButton = itemOption.find('#listName').html();
            color = itemOption.data('list-color');
            deleteVisibility = 'inherit';
        }
        button.find('#listName').html(textButton);
        button.css('background', color);
        button.siblings('.uk-dropdown').find('.deleteToListElement').css('display', deleteVisibility);
        button.siblings('.uk-dropdown').find('li.deleteToListElement a').attr('data-list-id', idList);
    }
}

MB_BookList_Slider = {
    sliderTemplateSelector: '#bookListSlider',
    add: function (listSelector, bookList) {
        let template = $(MB_BookList_Slider.sliderTemplateSelector).clone();
        let node = template.prop('content');
        $(node).find('.uk-nestable-item').attr('data-book-list-id', bookList.id);
        $(node).find('.uk-nestable-item').attr('data-book-list-name', bookList.name);
        $(node).find('#listName').html(bookList.name);
        $(node).find('.uk-nestable-item').attr('data-book-list-color', bookList.color);
        $(node).find('.uk-nestable-panel').css('background', bookList.color);
        $(node).find('.uk-nestable-item').attr('data-book-list-removable', bookList.removable);

        if (bookList.removable == 0) {
            $(node).find('#actionsButtons').remove();
        }

        if (bookList.books.length > 0) {
            let bookContainer = $(node).find('ul.uk-slider');
            console.log(bookContainer);
            $(bookContainer).css('display', 'inherit');
            bookList.books.forEach(function (book) {
                MB_Book.addCardMiniView(bookContainer, book, "LI");
            });
        }
        else {
            $(node).find('#emptyBookListContainer').css('display', 'inherit');
        }

        $(listSelector).append(node);
    },
    edit: function (listSelector, bookList) {
        let listSelected = $(listSelector + ' li.mb_book_list_slider[data-book-list-id="' + bookList.id + '"]');
        listSelected.attr("data-book-list-color", bookList.color);
        listSelected.find(".uk-nestable-panel").css("background", bookList.color);
        listSelected.attr("data-book-list-name", bookList.name);
        listSelected.find("#listName").html(bookList.name);
    },
    delete: function (listSelector, id) {
        $(listSelector + ' li.mb_book_list_slider[data-book-list-id="' + id + '"]').remove();
    }
}

MB_StartRating = {
    load: function (stars, numReviews, identifier) {
        if (identifier == undefined) identifier = '.mb_start_rating_view';
        $(identifier).attr("data-stars", stars);
        $(identifier).attr("data-reviews", numReviews);
        let starList = $(identifier).find('.mb_stars_rating');
        let reviewsText = $(identifier).find('.mb_num_rating');

        if (numReviews == 0) {
            starList.css('display', 'none');
            reviewsText.html(Translator.SinOpiniones);
            return;
        }

        starList.css('display', 'inline');
        reviewsText.html(numReviews + " " + ((numReviews == 1) ? Translator.Opinion : Translator.Opiniones));

        let fullStars = Math.trunc(stars);
        let halfStar = stars - Math.trunc(stars);
        starList.children('i').each(function (i) {
            if (fullStars > i) {
                $(this).removeClass('uk-icon-star-o').removeClass('uk-icon-star-half-empty').addClass('uk-icon-star');
                return;
            }
            if (i < stars && halfStar >= 0.5) {
                $(this).removeClass('uk-icon-star-o').removeClass('uk-icon-star').addClass('uk-icon-star-half-empty');
                return;
            }
            $(this).removeClass('uk-icon-star-half-empty').removeClass('uk-icon-star').addClass('uk-icon-star-o');
        });
    }
}

class MB_ListGridView {
    constructor(uniqueId) {
        this.selListGrid = '#listaLibros_' + uniqueId;
        this.selListGridToggle = '#list_grid_toggle_' + uniqueId;
        this.selPagination = '.mb_pagination_pages_' + uniqueId
        this.selNoResults = '#listGridNoResults_' + uniqueId
    }

    init() {
        let $viewToggle = $(this.selListGridToggle).children('li');
        let listGrid = $(this.selListGrid);

        let defaultView = MB_LocalStorage.load('ListGridViewDefault');
        if (defaultView == null) defaultView = 'grid_view';
        listGrid.addClass(defaultView);
        $(this.selListGridToggle).find('li[data-view="' + defaultView + '"]').addClass('uk-active');
        console.log($(this.selListGridToggle).find('li[data-view="' + defaultView + '"]'));
        listGrid.each(function () {
            if ($(this).hasClass('uk-active')) {
                listGrid.addClass($viewToggle.attr('data-view'));
            }
        });

        $viewToggle.on('click', function (e) {
            e.preventDefault();

            var $this = $(this),
                isActive = $this.hasClass('uk-active');

            if (!isActive) {
                var view = $this.attr('data-view');
                if (view == 'list_view') {
                    listGrid.addClass('list_view').removeClass('grid_view');
                } else {
                    listGrid.addClass('grid_view').removeClass('list_view');
                }
                MB_LocalStorage.save("ListGridViewDefault", view);
                $this.addClass('uk-active').siblings().removeClass('uk-active');
            }
        });
    }

    loadGridView(paginationResult, addItemFunction) {
        MB_Paginator.loadPagination(this.selPagination, paginationResult.totalItems, paginationResult.elementsPerPage, paginationResult.currentPage, 'searchBooks');
        if (paginationResult.totalItems == 0) {
            $(this.selListGrid).css('display', 'none');
            $(this.selNoResults).css('display', 'block');
            return;
        }

        $(this.selNoResults).css('display', 'none');
        $(this.listGridNoResults).css('display', 'none');
        $(this.selListGrid).html("");
        let selListGrid = this.selListGrid;
        paginationResult.items.forEach(function (item) {
            addItemFunction(selListGrid, item);
        });
    }
}

MB_ColorPalette = {
    init: function (identifier, name = '') {
        if (name == '') name = 'Color';
        $(identifier + ' #inputColorPalette').attr('name', name);
        $(identifier + " #colorPaletteError").css('visibility', 'hidden');
        var kUI_color_palette = $(identifier + " #colorPalette");
        if (kUI_color_palette.length) {
            kUI_color_palette.kendoColorPalette({
                columns: 20,
                tileSize: {
                    width: 32,
                    height: 24
                },
                palette: [
                    "#e53935", "#d81b60", "#8e24aa", "#5e35b1", "#3949ab",
                    "#1e88e5", "#039be5", "#00acc1", "#00897b", "#43a047",
                    "#7cb342", "#c0ca33", "#fdd835", "#ffb300", "#fb8c00",
                    "#f4511e", "#6d4c41", "#757575", "#546e7a"
                ],
                change: function (e) {
                    $(identifier + " #colorPaletteError").css('visibility', 'hidden');;
                    $(identifier + ' #inputColorPalette').val(e.value);
                }
            });
        }

        $(document).on(identifier + ' #inputColorPalette', 'change', function () {
            console.log("Ha cambiado el change");
        });

        $(identifier + ' #inputColorPalette').change(function () {
            alert("The text has been changed.");
        });
    },
    verificate: function (identifier) {
        if ($(identifier + ' #inputColorPalette').val() == "") {
            $(identifier + ' #colorPaletteError').css('visibility', 'visible');
            return false;
        }
        $(identifier + ' #colorPaletteError').css('visibility', 'hidden');
        return true;
    }
}

MB_Notification = {
    show: function (message, status, callbackFunction) {
        if (message == undefined) return;
        thisNotify = UIkit.notify({
            message: message,
            status: (status == undefined) ? 'info' : status,
            timeout: 5000,
            group: null,
            pos: 'bottom-right',
            onClose: function () {
                $body.find('.md-fab-wrapper').css('margin-bottom', '');
                if (callbackFunction != undefined) callbackFunction
                // clear notify timeout (sometimes callback is fired more than once)
                clearTimeout(thisNotify.timeout)
            }
        });
        if (
            (
                ($window.width() < 768)
                && (
                    (thisNotify.options.pos == 'bottom-right')
                    || (thisNotify.options.pos == 'bottom-left')
                    || (thisNotify.options.pos == 'bottom-center')
                )
            )
            || (thisNotify.options.pos == 'bottom-right')
        ) {
            var thisNotify_height = $(thisNotify.element).outerHeight();
            var spacer = $window.width() < 768 ? -6 : 8;
            $body.find('.md-fab-wrapper').css('margin-bottom', thisNotify_height + spacer);
        }
    }
}

MB_Modal = {
    confirm: function (message, callbackYes, txtYes, txtNo) {
        if (message == undefined || message == null) return;
        if (txtYes == undefined || txtYes == null) txtYes = "Sí";
        if (txtNo == undefined || txtNo == null) txtNo = "Cancelar";
        var modal = UIkit.modal.confirm(message, callbackYes, { labels: { 'Ok': txtYes, 'Cancel': txtNo } });
        modal.show();
    },
    alert: function (message, callbackFunction, txtButton) {
        if (message == undefined || message == null) return;
        if (txtButton == undefined || txtButton == null) txtButton = "Entendido";
        var modal = UIkit.modal.alert(message, { labels: { 'Ok': txtButton }
        }).on({
            'hide.uk.modal': function () {
                if (callbackFunction != undefined) callbackFunction();
            }
        });

        modal.show();
    }
}

MB_LocalStorage = {
    storage: window.localStorage,
    save: function(key, value) {
        try {
            MB_LocalStorage.storage.setItem(key, value);
        }
        catch (e) { console.log(e); }
    },
    load: function (key) {
        try {
            return MB_LocalStorage.storage.getItem(key);
        }
        catch (e) { alert("Please go away"); }
    },
    delete: function (key) {
        try {
            MB_LocalStorage.storage.removeItem("");
        }
        catch (e) { console.log(e); }
    }
}

MB_Scroll = {
    toTop: function () {
        $(window).scrollTop(0);
    },
    toBottomAnimate: function () {
        $("html, body").animate({ scrollTop: $(document).height() }, 1000);
    }
}

MB_User = {
    changeCulture: function (formSelector) {
        let form = $(formSelector);
        let form_data = form.serialize(); //Encode form elements for submission

        $.ajax({
            type: 'put',
            url: '/api/user/culture',
            data: form_data,
            datatype: "json",
            success: function (result) {
                if (result.code > 0) {
                    form.find('input[name="__RequestVerificationToken"]').val(result.verificationToken);
                    MB_Error_Handler.notificate(result);
                    return;
                }
                    
                location.reload();
            },
            error: function (xmlhttprequest) {
                MB_Error_Handler.notificate(xmlhttprequest);
            },
            complete: function () { form.find(":submit").removeClass('disabled').removeAttr('disabled'); }
        });
    }
}

MB_Error_Handler = {
    notificate: function (response) {
        let errorMessage = '';
        if (response.message != undefined) {
            errorMessage = response.message;
        }
        else if (response.responseJSON != undefined && response.responseJSON.errors != undefined) {
            if (response.responseJSON.errors != undefined)
                errorMessage += Object.values(response.responseJSON.errors).join('<br/>');
        }
        else if (response.responseText != undefined) {
            errorMessage = response.responseText;
        }

        if (errorMessage == '') return;

        MB_Notification.show(errorMessage, 'danger');
    }
}