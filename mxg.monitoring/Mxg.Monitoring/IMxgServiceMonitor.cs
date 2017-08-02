using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace Mxg.Monitoring
{
    /// <summary>
    /// Контракты для мониторинга WCF-сервиса
    /// </summary>
    [ServiceContract]
    public interface IMxgServiceMonitor
    {
        /// <summary>
        /// Получить базовое состояние
        /// </summary>
        /// <returns>Статус 200 и тело "ok", если всё ок, иначе статусы 4**/5**</returns>
        [OperationContract]
        [WebInvoke(
            UriTemplate = "",
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Task<Stream> GetStateAsync();

        /// <summary>
        /// Получить состояние по параметру
        /// </summary>
        /// <param name="option">Параметр состояния</param>
        /// <returns>Зависит от реализации в конкретном сервисе</returns>
        [OperationContract]
        [WebInvoke(
            UriTemplate = "/{option}",
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Task<Stream> GetOptionAsync(string option);
    }
}
