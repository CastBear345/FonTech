using FonTech.Domain.Entities;
using FonTech.Domain.Interfaces.Validations;
using FonTech.Domain.Result;

namespace FonTech.Application.Validations;

public class ReportValidator : IReportValidator
{
    public BaseResult CreateValidator(Report report, User user)
    {
        if(report != null)
        {
            return new BaseResult()
            {
                ErrorMessage = "Отчёт с таким названием уже есть!",
                ErrorCode = 2,
            };
        }

        if (user == null)
        {
            return new BaseResult()
            {
                ErrorMessage = "Пользователь не найден",
                ErrorCode = 11,
            };
        }

        return new BaseResult();
    }

    public BaseResult ValidateOnNull(Report model)
    {
        if(model == null)
        {
            return new BaseResult()
            {
                ErrorMessage = "Отчёт не найден!",
                ErrorCode = 1
            };
        }

        return new BaseResult();
    }
}
