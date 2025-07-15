namespace TaskManagementApp.Models.Errors
{
    public class ErrorModel
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public ErrorModel(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
