using FonTech.Domain.Entities;
using FonTech.Domain.Result;

namespace FonTech.Domain.Interfaces.Validations;

public interface IReportValidator : IBaseValidator<Report>
{
    /// <summary>
    /// Проверяется наличие отчёта, если отчёт с переданными данными есть в БД, то создать точно такой же нельзя
    /// 
    /// Проверяется пользователь, если с UserId пользователь не найден, то такого пользователя нет
    /// </summary>
    /// <param name="report"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    BaseResult CreateValidator(Report report, User user);
}
