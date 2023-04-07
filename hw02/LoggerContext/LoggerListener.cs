using System.Text;
using HW02.BussinessContext.DB.Entities;
using HW02.BussinessContext.Enum;
using HW02.InputOutput.Printer;
using HW02.LoggerContext.DB;

namespace HW02;

public class LoggerListener
{
    private readonly LoggerDBContext _loggerDBContext;
    private readonly StringBuilder _log;

    public LoggerListener(LoggerDBContext lContext)
    {
        _log = new StringBuilder();
        _loggerDBContext = lContext;
    }

    private void FormatLog(Operation op, Entity entity, Exception ex)
    {
        if (ex is not null)
        {
            _log.Append(string.Format("Failure; {0}", ex.Message));
            return;
        }

        _log.Append("Success");

        if (op == Operation.Get)
        {
            return;
        }

        _log.Append(string.Format("; {0}; {1}", entity.Id, entity.Name));

        if (entity.GetType() == typeof(Product))
        {
            _log.Append(string.Format("; {0}", entity.CategoryId));
        }
    }

    public void SaveLog(Operation op, EntityType type, Entity? entity, Exception? exception)
    {
        _log.Append(string.Format("[{0}] {1}; {2}; ", DateTime.Now, op.ToString(), type.ToString()));

        FormatLog(op, entity, exception);

        try
        {
            _loggerDBContext.WriteLog(_log);
            _log.Clear();
        }
        catch (Exception ex)
        {
            MessagePrinter.LogDataFailure(ex.Message);
        }
    }
}