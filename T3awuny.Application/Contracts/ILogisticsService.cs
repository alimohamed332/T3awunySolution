using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.DTOs.Logistics;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Application.Contracts
{
    public interface ILogisticsService
    {
        Task<ApiResponse<LogisticsResponseDto>> GetByOrderIdAsync(string userId, int orderId);
        Task<ApiResponse<string>> UpdateLogisticsAsync(int orderId, UpdateLogisticsDto dto);
        Task<ApiResponse<string>> UpdateStatusAsync(int orderId, LogisticsStatus status);
    }
}
