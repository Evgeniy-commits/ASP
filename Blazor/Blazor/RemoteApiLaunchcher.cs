//using Microsoft.AspNetCore.Http.Connections;
using System.Management;

namespace Blazor
{
    #pragma warning disable CA1416 // Platform-specific API — проект Windows-only
    #pragma warning disable CS4014 // .Result блокирует синхронно, await не нужен

    public static class RemoteApiLauncher
    {
        // Пытаемся запустить процесс на удалённом компьютере через WMI.
        // Возвращает true, если процесс запущен.

        public static bool TryStartRemoteProcess
            (
                string remoteIp,
                string remoteExePath
            )
        {
            try
            {
                Console.WriteLine($"Попытка запуска на {remoteIp}: {remoteExePath}");

                ConnectionOptions options = new ConnectionOptions 
                {
                    Impersonation = ImpersonationLevel.Impersonate,
                    Authentication = AuthenticationLevel.Packet
                };

                ManagementScope scope = new ManagementScope($"\\\\{remoteIp}\\root\\cimv2", options);
                scope.Connect();

                ManagementClass processClass = new ManagementClass(scope, new ManagementPath("Win32_Process"), new ObjectGetOptions());
                ManagementBaseObject inParams = processClass.GetMethodParameters("Create");
                inParams["CommandLine"] = remoteExePath;

                ManagementBaseObject outParams = processClass.InvokeMethod("Create", inParams, null);

                int returnValue = Convert.ToInt32(outParams["ReturnValue"]);

                // 0 = успех, остальные коды — ошибки
                if (returnValue == 0)
                {
                    Console.WriteLine($"Процесс запущен на {remoteIp}");
                    return true;
                }

                Console.WriteLine($"WMI вернул код ошибки: {returnValue}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WMI Error: {ex.Message}");
                return false;
            }
        }

        //Отвечает ли API по адресу
        public static bool IsApiAlive(string url)
        {
            try
            {
                using HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(3);
                client.GetAsync(url).GetAwaiter().GetResult();
                //Любой ответ
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
