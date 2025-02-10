using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.Dtos
{
    public class ApiResponse1<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ApiResponse1(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }

}
