namespace Domain.Dtos
{
    public class ResponseResult
    {
        public bool Succeeded { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public static ResponseResult Ok(string? message = null) =>
            new() { Succeeded = true, StatusCode = 200, Message = message };

        public static ResponseResult Created(string? message = null) =>
            new() { Succeeded = true, StatusCode = 201, Message = message };

        public static ResponseResult BadRequest(string? message = "Invalid field(s)") =>
            new() { Succeeded = false, StatusCode = 400, Message = message };
    
        public static ResponseResult NotFound(string? message = "Not found") =>
            new() { Succeeded = false, StatusCode = 404, Message = message };

        public static ResponseResult AlreadyExists(string? message = "Already exists") =>
            new() { Succeeded = false, StatusCode = 409, Message = message };

        public static ResponseResult Failed(string? message = "An unexpected error occured") =>
            new() { Succeeded = false, StatusCode = 500, Message = message };
    }

    public class ResponseResult<T> : ResponseResult
    {
        public T? Result { get; set; }

        public static ResponseResult<T> Ok(T result, string? message = null) =>
            new() { Succeeded = true, StatusCode = 200, Message = message, Result = result };
        public static ResponseResult<T> Created(T result, string? message = null) =>
            new() { Succeeded = true, StatusCode = 201, Message = message, Result = result };
        public new static ResponseResult<T> BadRequest(string? message = "Invalid field(s)") =>
            new() { Succeeded = false, StatusCode = 400, Message = message, Result = default };
        public new static ResponseResult<T> NotFound(string? message = "Not found") =>
            new() { Succeeded = false, StatusCode = 404, Message = message, Result = default };
        public new static ResponseResult<T> AlreadyExists(string? message = "Already exists") =>
            new() { Succeeded = false, StatusCode = 409, Message = message, Result = default };
        public new static ResponseResult<T> Failed(string? message = "An unexpected error occured") =>
            new() { Succeeded = false, StatusCode = 500, Message = message, Result = default };
    }
}
