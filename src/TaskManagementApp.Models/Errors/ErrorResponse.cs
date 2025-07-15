namespace TaskManagementApp.Models.Errors
{
    public class ErrorResponse
    {
        public List<ErrorModel> Errors { get; set; } = [];

        public ErrorResponse(string code, string message)
        {
            Errors.Add(new ErrorModel(code, message));
        }

        public ErrorResponse(List<ErrorModel> errors)
        {
            Errors = errors;
        }
    }
}
