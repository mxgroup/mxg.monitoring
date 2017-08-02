using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Mxg.Monitoring
{
    public class MxgServiceMonitor : IMxgServiceMonitor
    {
        private const string OkStateMessage = "OK";
        private const string FailedStateMessage = "FAILED";
        private const string PlainTextContentType = "text/plain";

        /// <inheritdoc />
        public virtual async Task<Stream> GetStateAsync()
        {
            return await GetOptionAsync(null);
        }

        /// <inheritdoc />
        public virtual async Task<Stream> GetOptionAsync(string option)
        {
            var context = WebOperationContext.Current;
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.OutgoingResponse.ContentType = PlainTextContentType;

            try
            {
                string result = string.IsNullOrEmpty(option)
                    ? GetStateMessage()
                    : GetOptionMessage(option);

                return await GetStreamFromString(result);
            }
            catch (Exception)
            {
                context.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                return await GetStreamFromString(FailedStateMessage);
            }
        }

        /// <summary>
        /// Получить сообщение о состоянии сервиса
        /// </summary>
        protected virtual string GetStateMessage()
        {
            return OkStateMessage;
        }

        /// <summary>
        /// Получить строку состояния по параметру
        /// </summary>
        /// <param name="option">Параметр состояния</param>
        /// <returns>Статус 200 и пустое тело, если не переопределено в наследнике</returns>
        protected virtual string GetOptionMessage(string option)
        {
            return string.Empty;
        }

        /// <summary>
        /// Получить поток UTF8 из строки
        /// </summary>
        protected async Task<Stream> GetStreamFromString(string message)
        {
            return await Task.FromResult(new MemoryStream(Encoding.UTF8.GetBytes(message)));
        }

#region Настройка WCF по умолчанию

        /// <summary>
        /// Адрес проверки состояния сервиса
        /// </summary>
        private const string StateAddress = "state";

        /// <summary>
        /// Добавить к существующему WCF-хосту конфигурацию мониторинга по умолчанию
        /// </summary>
        /// <param name="host"></param>
        public static void ApplyDefaultConfiguration(ServiceHost host)
        {
            var webHttpBinding = new WebHttpBinding(WebHttpSecurityMode.None);

            var stateEndpoint = host.AddServiceEndpoint(
                typeof(IMxgServiceMonitor),
                webHttpBinding,
                StateAddress);
            stateEndpoint.Behaviors.Add(new WebHttpBehavior { HelpEnabled = false });
        }

#endregion
    }
}