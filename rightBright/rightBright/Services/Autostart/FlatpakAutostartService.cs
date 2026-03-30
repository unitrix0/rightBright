using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Tmds.DBus.Protocol;
using Address = Tmds.DBus.Address;

namespace rightBright.Services.Autostart;

/// <summary>
/// Uses the XDG Desktop Portal Background API to register / unregister
/// autostart when running inside a Flatpak sandbox.
/// </summary>
public class FlatpakAutostartService : IAutostartService
{
    private const string PortalDestination = "org.freedesktop.portal.Desktop";
    private const string PortalPath = "/org/freedesktop/portal/desktop";
    private const string BackgroundInterface = "org.freedesktop.portal.Background";
    private const string RequestInterface = "org.freedesktop.portal.Request";

    private readonly ILogger _logger;

    public bool IsSupported => true;

    public FlatpakAutostartService(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<bool> SetAutostartAsync(bool enabled)
    {
        Connection? connection = null;
        try
        {
            connection = new Connection(Address.Session);
            await connection.ConnectAsync();

            var senderName = connection.UniqueName!;
            var token = "rb_" + Guid.NewGuid().ToString("N")[..20];

            var sanitizedSender = senderName.TrimStart(':').Replace('.', '_');
            var expectedPath = $"/org/freedesktop/portal/desktop/request/{sanitizedSender}/{token}";

            var tcs = new TaskCompletionSource<bool>();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            cts.Token.Register(() => tcs.TrySetResult(false));

            var rule = new MatchRule
            {
                Type = MessageType.Signal,
                Interface = RequestInterface,
                Member = "Response",
                Path = expectedPath
            };

            using var signalDisposable = await connection.AddMatchAsync<uint>(
                rule,
                static (Message m, object? _) =>
                {
                    var reader = m.GetBodyReader();
                    return reader.ReadUInt32();
                },
                static (Exception? ex, uint responseCode, object? _, object? hs) =>
                {
                    var completionSource = (TaskCompletionSource<bool>)hs!;
                    if (ex is not null)
                    {
                        completionSource.TrySetResult(false);
                        return;
                    }

                    completionSource.TrySetResult(responseCode == 0);
                },
                null,
                tcs,
                false,
                ObserverFlags.None);

            var replyPath = await CallRequestBackgroundAsync(connection, token, enabled);
            _logger.Information("[Autostart] RequestBackground sent (autostart={Enabled}), reply path: {Path}",
                enabled, replyPath);

            var granted = await tcs.Task;
            _logger.Information("[Autostart] Portal response: granted={Granted}", granted);
            return granted;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[Autostart] Failed to set autostart={Enabled}", enabled);
            return false;
        }
        finally
        {
            connection?.Dispose();
        }
    }

    private static async Task<string> CallRequestBackgroundAsync(Connection connection, string handleToken,
        bool autostart)
    {
        var reply = await connection.CallMethodAsync(CreateMessage(), static (Message m, object? _) =>
        {
            var reader = m.GetBodyReader();
            return reader.ReadObjectPath().ToString();
        });

        return reply;

        MessageBuffer CreateMessage()
        {
            var writer = connection.GetMessageWriter();
            writer.WriteMethodCallHeader(
                destination: PortalDestination,
                path: PortalPath,
                @interface: BackgroundInterface,
                signature: "sa{sv}",
                member: "RequestBackground");

            writer.WriteString("");

            var dictStart = writer.WriteArrayStart(DBusType.Struct);

            writer.WriteStructureStart();
            writer.WriteString("handle_token");
            writer.WriteSignature("s");
            writer.WriteString(handleToken);

            writer.WriteStructureStart();
            writer.WriteString("reason");
            writer.WriteSignature("s");
            writer.WriteString("Automatically adjust display brightness");

            writer.WriteStructureStart();
            writer.WriteString("autostart");
            writer.WriteSignature("b");
            writer.WriteBool(autostart);

            writer.WriteArrayEnd(dictStart);
            return writer.CreateMessage();
        }
    }
}
