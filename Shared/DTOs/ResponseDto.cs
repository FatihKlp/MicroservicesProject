namespace Shared.DTOs
{
    public class ResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ResponseDto<T> Ok(T data, string message = "") =>
            new ResponseDto<T> { Success = true, Data = data, Message = message };

        public static ResponseDto<T> Fail(string message) =>
            new ResponseDto<T> { Success = false, Message = message };
    }
}
