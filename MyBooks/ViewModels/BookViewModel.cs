using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBooks.ViewModels
{
    public class BookViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Titulo")]
        public string Title { get; set; }

        [Display(Name = "Subtitulo")]
        public string Subtitle { get; set; }

        [Display(Name = "Sinopsis")]
        public string Description { get; set; }

        [Display(Name = "NumeroPaginas")]
        public int? PageCount { get; set; }

        [Display(Name = "ISBN")]
        public string ISBN10 { get; set; }

        [Display(Name = "ISBN")]
        public string ISBN13 { get; set; }

        [Display(Name = "Autor")]
        public IEnumerable<string> Authors { get; set; }

        [Display(Name = "Editorial")]
        public string Publisher { get; set; }

        [Display(Name = "FechaPublicacion"), DataType(DataType.Date)]
        public DateTime? PublishedDate { get; set; }

        public string SmallImage { get; set; }
        public string Image { get; set; }

        [Display(Name = "Idioma")]
        public string Language { get; set; }

        [Display(Name = "Categorias")]
        public IEnumerable<string> Categories { get; set; }

        [Display(Name = "Puntuacion")]
        [Range(0, 5, ErrorMessage = "ErrorRango")]
        public double Stars { get; set; }

        [Display(Name = "Opiniones")]
        public int Ratings { get; set; }

        public int UserBookListId { get; set; }
    }
}
