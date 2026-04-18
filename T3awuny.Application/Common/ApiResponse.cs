using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.Common
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Success")
            => new() { IsSuccess = true, Message = message, Data = data };

        public static ApiResponse<T> Fail(string message, List<string>? errors = null)
            => new() { IsSuccess = false, Message = message, Errors = errors };
    }
}
