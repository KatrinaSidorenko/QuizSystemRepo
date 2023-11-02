using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAttemptService
    {
        Task<Result<int>> AddAttempt(Attempt attempt);
    }
}
