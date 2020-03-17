
namespace MyBooks.ViewModels
{
    public class ResponseViewModel
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }
        public string VerificationToken { get; set; }
    }
}
