using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace rightBright.Services.DBus.PowerManagement
{
    partial class QGuiApplication : PowerManagementObject
    {
        private const string Interface = "org.qtproject.Qt.QGuiApplication";
        public QGuiApplication(PowerManagementService service, ObjectPath path) : base(service, path)
        { }
        public Task SetBadgeNumberAsync(long number)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: Interface,
                    signature: "x",
                    member: "setBadgeNumber");
                writer.WriteInt64(number);
                return writer.CreateMessage();
            }
        }
        public Task SetApplicationDisplayNameAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(Interface);
                writer.WriteString("applicationDisplayName");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        public Task SetDesktopFileNameAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(Interface);
                writer.WriteString("desktopFileName");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        public Task SetQuitOnLastWindowClosedAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(Interface);
                writer.WriteString("quitOnLastWindowClosed");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task<string> GetApplicationDisplayNameAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(Interface, "applicationDisplayName"), (Message m, object? s) => ReadMessage_v_s(m, (PowerManagementObject)s!), this);
        public Task<string> GetDesktopFileNameAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(Interface, "desktopFileName"), (Message m, object? s) => ReadMessage_v_s(m, (PowerManagementObject)s!), this);
        public Task<string> GetPlatformNameAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(Interface, "platformName"), (Message m, object? s) => ReadMessage_v_s(m, (PowerManagementObject)s!), this);
        public Task<bool> GetQuitOnLastWindowClosedAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(Interface, "quitOnLastWindowClosed"), (Message m, object? s) => ReadMessage_v_b(m, (PowerManagementObject)s!), this);
        public Task<QGuiApplicationProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(Interface), (Message m, object? s) => ReadMessage(m, (PowerManagementObject)s!), this);
            static QGuiApplicationProperties ReadMessage(Message message, PowerManagementObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<QGuiApplicationProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(Interface, (Message m, object? s) => ReadMessage(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<QGuiApplicationProperties> ReadMessage(Message message, PowerManagementObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<QGuiApplicationProperties>(ReadProperties(ref reader, changed), ReadInvalidated(ref reader), changed.ToArray());
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "applicationDisplayName": invalidated.Add("ApplicationDisplayName"); break;
                        case "desktopFileName": invalidated.Add("DesktopFileName"); break;
                        case "platformName": invalidated.Add("PlatformName"); break;
                        case "quitOnLastWindowClosed": invalidated.Add("QuitOnLastWindowClosed"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static QGuiApplicationProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new QGuiApplicationProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "applicationDisplayName":
                        reader.ReadSignature("s"u8);
                        props.ApplicationDisplayName = reader.ReadString();
                        changedList?.Add("ApplicationDisplayName");
                        break;
                    case "desktopFileName":
                        reader.ReadSignature("s"u8);
                        props.DesktopFileName = reader.ReadString();
                        changedList?.Add("DesktopFileName");
                        break;
                    case "platformName":
                        reader.ReadSignature("s"u8);
                        props.PlatformName = reader.ReadString();
                        changedList?.Add("PlatformName");
                        break;
                    case "quitOnLastWindowClosed":
                        reader.ReadSignature("b"u8);
                        props.QuitOnLastWindowClosed = reader.ReadBool();
                        changedList?.Add("QuitOnLastWindowClosed");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record QCoreApplicationProperties
    {
        public string ApplicationName { get; set; } = default!;
        public string ApplicationVersion { get; set; } = default!;
        public string OrganizationName { get; set; } = default!;
        public string OrganizationDomain { get; set; } = default!;
        public bool QuitLockEnabled { get; set; } = default!;
    }
    partial class QCoreApplication : PowerManagementObject
    {
        private const string __Interface = "org.qtproject.Qt.QCoreApplication";
        public QCoreApplication(PowerManagementService service, ObjectPath path) : base(service, path)
        { }
        public Task QuitAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "quit");
                return writer.CreateMessage();
            }
        }
        public Task ExitAsync(int retcode)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "i",
                    member: "exit");
                writer.WriteInt32(retcode);
                return writer.CreateMessage();
            }
        }
        public Task ExitAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "exit");
                return writer.CreateMessage();
            }
        }
        public Task SetApplicationNameAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("applicationName");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        public Task SetApplicationVersionAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("applicationVersion");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        public Task SetOrganizationNameAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("organizationName");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        public Task SetOrganizationDomainAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("organizationDomain");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        public Task SetQuitLockEnabledAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("quitLockEnabled");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task<string> GetApplicationNameAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "applicationName"), (Message m, object? s) => ReadMessage_v_s(m, (PowerManagementObject)s!), this);
        public Task<string> GetApplicationVersionAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "applicationVersion"), (Message m, object? s) => ReadMessage_v_s(m, (PowerManagementObject)s!), this);
        public Task<string> GetOrganizationNameAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "organizationName"), (Message m, object? s) => ReadMessage_v_s(m, (PowerManagementObject)s!), this);
        public Task<string> GetOrganizationDomainAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "organizationDomain"), (Message m, object? s) => ReadMessage_v_s(m, (PowerManagementObject)s!), this);
        public Task<bool> GetQuitLockEnabledAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "quitLockEnabled"), (Message m, object? s) => ReadMessage_v_b(m, (PowerManagementObject)s!), this);
        public Task<QCoreApplicationProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (PowerManagementObject)s!), this);
            static QCoreApplicationProperties ReadMessage(Message message, PowerManagementObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<QCoreApplicationProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<QCoreApplicationProperties> ReadMessage(Message message, PowerManagementObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<QCoreApplicationProperties>(ReadProperties(ref reader, changed), ReadInvalidated(ref reader), changed.ToArray());
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "applicationName": invalidated.Add("ApplicationName"); break;
                        case "applicationVersion": invalidated.Add("ApplicationVersion"); break;
                        case "organizationName": invalidated.Add("OrganizationName"); break;
                        case "organizationDomain": invalidated.Add("OrganizationDomain"); break;
                        case "quitLockEnabled": invalidated.Add("QuitLockEnabled"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static QCoreApplicationProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new QCoreApplicationProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "applicationName":
                        reader.ReadSignature("s"u8);
                        props.ApplicationName = reader.ReadString();
                        changedList?.Add("ApplicationName");
                        break;
                    case "applicationVersion":
                        reader.ReadSignature("s"u8);
                        props.ApplicationVersion = reader.ReadString();
                        changedList?.Add("ApplicationVersion");
                        break;
                    case "organizationName":
                        reader.ReadSignature("s"u8);
                        props.OrganizationName = reader.ReadString();
                        changedList?.Add("OrganizationName");
                        break;
                    case "organizationDomain":
                        reader.ReadSignature("s"u8);
                        props.OrganizationDomain = reader.ReadString();
                        changedList?.Add("OrganizationDomain");
                        break;
                    case "quitLockEnabled":
                        reader.ReadSignature("b"u8);
                        props.QuitLockEnabled = reader.ReadBool();
                        changedList?.Add("QuitLockEnabled");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    partial class PowerManagement : PowerManagementObject
    {
        private const string __Interface = "org.freedesktop.PowerManagement";
        public PowerManagement(PowerManagementService service, ObjectPath path) : base(service, path)
        { }
        public Task SuspendAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "Suspend");
                return writer.CreateMessage();
            }
        }
        public Task HibernateAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "Hibernate");
                return writer.CreateMessage();
            }
        }
        public Task<bool> CanSuspendAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "CanSuspend");
                return writer.CreateMessage();
            }
        }
        public Task<bool> CanHibernateAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "CanHibernate");
                return writer.CreateMessage();
            }
        }
        public Task<bool> CanHybridSuspendAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "CanHybridSuspend");
                return writer.CreateMessage();
            }
        }
        public Task<bool> CanSuspendThenHibernateAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "CanSuspendThenHibernate");
                return writer.CreateMessage();
            }
        }
        public Task<bool> GetPowerSaveStatusAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "GetPowerSaveStatus");
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchCanSuspendChangedAsync(Action<Exception?, bool> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "CanSuspendChanged", (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchCanHibernateChangedAsync(Action<Exception?, bool> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "CanHibernateChanged", (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchCanHybridSuspendChangedAsync(Action<Exception?, bool> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "CanHybridSuspendChanged", (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchCanSuspendThenHibernateChangedAsync(Action<Exception?, bool> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "CanSuspendThenHibernateChanged", (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchPowerSaveStatusChangedAsync(Action<Exception?, bool> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "PowerSaveStatusChanged", (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
    }
    partial class Inhibit : PowerManagementObject
    {
        private const string __Interface = "org.freedesktop.PowerManagement.Inhibit";
        public Inhibit(PowerManagementService service, ObjectPath path) : base(service, path)
        { }
        public Task<uint> InhibitAsync(string application, string reason)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_u(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "ss",
                    member: "Inhibit");
                writer.WriteString(application);
                writer.WriteString(reason);
                return writer.CreateMessage();
            }
        }
        public Task UnInhibitAsync(uint cookie)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "u",
                    member: "UnInhibit");
                writer.WriteUInt32(cookie);
                return writer.CreateMessage();
            }
        }
        public Task<bool> HasInhibitAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "HasInhibit");
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchHasInhibitChangedAsync(Action<Exception?, bool> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "HasInhibitChanged", (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
    }
    record ScreenBrightnessProperties
    {
        public string[] DisplaysDBusNames { get; set; } = default!;
    }
    partial class ScreenBrightness : PowerManagementObject
    {
        private const string __Interface = "org.kde.ScreenBrightness";
        public ScreenBrightness(PowerManagementService service, ObjectPath path) : base(service, path)
        { }
        public Task AdjustBrightnessRatioAsync(double delta, uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "du",
                    member: "AdjustBrightnessRatio");
                writer.WriteDouble(delta);
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        public Task AdjustBrightnessRatioWithContextAsync(double delta, uint flags, string sourceClientContext)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "dus",
                    member: "AdjustBrightnessRatioWithContext");
                writer.WriteDouble(delta);
                writer.WriteUInt32(flags);
                writer.WriteString(sourceClientContext);
                return writer.CreateMessage();
            }
        }
        public Task AdjustBrightnessStepAsync(uint stepAction, uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "uu",
                    member: "AdjustBrightnessStep");
                writer.WriteUInt32(stepAction);
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        public Task AdjustBrightnessStepWithContextAsync(uint stepAction, uint flags, string sourceClientContext)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "uus",
                    member: "AdjustBrightnessStepWithContext");
                writer.WriteUInt32(stepAction);
                writer.WriteUInt32(flags);
                writer.WriteString(sourceClientContext);
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchDisplayAddedAsync(Action<Exception?, string> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "DisplayAdded", (Message m, object? s) => ReadMessage_s(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchDisplayRemovedAsync(Action<Exception?, string> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "DisplayRemoved", (Message m, object? s) => ReadMessage_s(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchBrightnessChangedAsync(Action<Exception?, (string DisplayDbusName, int Brightness, string SourceClientName, string SourceClientContext)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "BrightnessChanged", (Message m, object? s) => ReadMessage_siss(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchBrightnessRangeChangedAsync(Action<Exception?, (string DisplayDbusName, int MaxBrightness, int Brightness)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "BrightnessRangeChanged", (Message m, object? s) => ReadMessage_sii(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public Task<string[]> GetDisplaysDBusNamesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "DisplaysDBusNames"), (Message m, object? s) => ReadMessage_v_as(m, (PowerManagementObject)s!), this);
        public Task<ScreenBrightnessProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (PowerManagementObject)s!), this);
            static ScreenBrightnessProperties ReadMessage(Message message, PowerManagementObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<ScreenBrightnessProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<ScreenBrightnessProperties> ReadMessage(Message message, PowerManagementObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<ScreenBrightnessProperties>(ReadProperties(ref reader, changed), ReadInvalidated(ref reader), changed.ToArray());
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "DisplaysDBusNames": invalidated.Add("DisplaysDBusNames"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static ScreenBrightnessProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new ScreenBrightnessProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "DisplaysDBusNames":
                        reader.ReadSignature("as"u8);
                        props.DisplaysDBusNames = reader.ReadArrayOfString();
                        changedList?.Add("DisplaysDBusNames");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record DisplayProperties
    {
        public string Label { get; set; } = default!;
        public bool IsInternal { get; set; } = default!;
        public int MaxBrightness { get; set; } = default!;
        public int Brightness { get; set; } = default!;
    }
    partial class Display : PowerManagementObject
    {
        private const string __Interface = "org.kde.ScreenBrightness.Display";
        public Display(PowerManagementService service, ObjectPath path) : base(service, path)
        { }
        public Task SetBrightnessAsync(int brightness, uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "iu",
                    member: "SetBrightness");
                writer.WriteInt32(brightness);
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        public Task SetBrightnessWithContextAsync(int brightness, uint flags, string sourceClientContext)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "ius",
                    member: "SetBrightnessWithContext");
                writer.WriteInt32(brightness);
                writer.WriteUInt32(flags);
                writer.WriteString(sourceClientContext);
                return writer.CreateMessage();
            }
        }
        public Task<string> GetLabelAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Label"), (Message m, object? s) => ReadMessage_v_s(m, (PowerManagementObject)s!), this);
        public Task<bool> GetIsInternalAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "IsInternal"), (Message m, object? s) => ReadMessage_v_b(m, (PowerManagementObject)s!), this);
        public Task<int> GetMaxBrightnessAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "MaxBrightness"), (Message m, object? s) => ReadMessage_v_i(m, (PowerManagementObject)s!), this);
        public Task<int> GetBrightnessAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Brightness"), (Message m, object? s) => ReadMessage_v_i(m, (PowerManagementObject)s!), this);
        public Task<DisplayProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (PowerManagementObject)s!), this);
            static DisplayProperties ReadMessage(Message message, PowerManagementObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<DisplayProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<DisplayProperties> ReadMessage(Message message, PowerManagementObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<DisplayProperties>(ReadProperties(ref reader, changed), ReadInvalidated(ref reader), changed.ToArray());
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "Label": invalidated.Add("Label"); break;
                        case "IsInternal": invalidated.Add("IsInternal"); break;
                        case "MaxBrightness": invalidated.Add("MaxBrightness"); break;
                        case "Brightness": invalidated.Add("Brightness"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static DisplayProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new DisplayProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "Label":
                        reader.ReadSignature("s"u8);
                        props.Label = reader.ReadString();
                        changedList?.Add("Label");
                        break;
                    case "IsInternal":
                        reader.ReadSignature("b"u8);
                        props.IsInternal = reader.ReadBool();
                        changedList?.Add("IsInternal");
                        break;
                    case "MaxBrightness":
                        reader.ReadSignature("i"u8);
                        props.MaxBrightness = reader.ReadInt32();
                        changedList?.Add("MaxBrightness");
                        break;
                    case "Brightness":
                        reader.ReadSignature("i"u8);
                        props.Brightness = reader.ReadInt32();
                        changedList?.Add("Brightness");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    partial class PowerManagement0 : PowerManagementObject
    {
        private const string __Interface = "org.kde.Solid.PowerManagement";
        public PowerManagement0(PowerManagementService service, ObjectPath path) : base(service, path)
        { }
        public Task RefreshStatusAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "refreshStatus");
                return writer.CreateMessage();
            }
        }
        public Task ReparseConfigurationAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "reparseConfiguration");
                return writer.CreateMessage();
            }
        }
        public Task<uint> BackendCapabilitiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_u(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "backendCapabilities");
                return writer.CreateMessage();
            }
        }
        public Task LoadProfileAsync(bool a0)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "b",
                    member: "loadProfile");
                writer.WriteBool(a0);
                return writer.CreateMessage();
            }
        }
        public Task<string> CurrentProfileAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_s(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "currentProfile");
                return writer.CreateMessage();
            }
        }
        public Task<ulong> BatteryRemainingTimeAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_t(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "batteryRemainingTime");
                return writer.CreateMessage();
            }
        }
        public Task<ulong> SmoothedBatteryRemainingTimeAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_t(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "smoothedBatteryRemainingTime");
                return writer.CreateMessage();
            }
        }
        public Task<bool> IsLidClosedAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "isLidClosed");
                return writer.CreateMessage();
            }
        }
        public Task<bool> IsLidPresentAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "isLidPresent");
                return writer.CreateMessage();
            }
        }
        public Task<bool> IsActionSupportedAsync(string a0)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "isActionSupported");
                writer.WriteString(a0);
                return writer.CreateMessage();
            }
        }
        public Task<bool> HasDualGpuAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "hasDualGpu");
                return writer.CreateMessage();
            }
        }
        public Task<bool> IsBatteryConservationModeEnabledAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "isBatteryConservationModeEnabled");
                return writer.CreateMessage();
            }
        }
        public Task<int> ChargeStartThresholdAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_i(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "chargeStartThreshold");
                return writer.CreateMessage();
            }
        }
        public Task<int> ChargeStopThresholdAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_i(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "chargeStopThreshold");
                return writer.CreateMessage();
            }
        }
        public Task<uint> ScheduleWakeupAsync(string service, ObjectPath path, ulong timestamp)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_u(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "sot",
                    member: "scheduleWakeup");
                writer.WriteString(service);
                writer.WriteObjectPath(path);
                writer.WriteUInt64(timestamp);
                return writer.CreateMessage();
            }
        }
        public Task WakeupAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "wakeup");
                return writer.CreateMessage();
            }
        }
        public Task ClearWakeupAsync(int a0)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "i",
                    member: "clearWakeup");
                writer.WriteInt32(a0);
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchProfileChangedAsync(Action<Exception?, string> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "profileChanged", (Message m, object? s) => ReadMessage_s(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchConfigurationReloadedAsync(Action<Exception?> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "configurationReloaded", handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchBatteryRemainingTimeChangedAsync(Action<Exception?, ulong> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "batteryRemainingTimeChanged", (Message m, object? s) => ReadMessage_t(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchSmoothedBatteryRemainingTimeChangedAsync(Action<Exception?, ulong> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "smoothedBatteryRemainingTimeChanged", (Message m, object? s) => ReadMessage_t(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchLidClosedChangedAsync(Action<Exception?, bool> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "lidClosedChanged", (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchBatteryConservationModeChangedAsync(Action<Exception?, bool> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "batteryConservationModeChanged", (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchChargeStartThresholdChangedAsync(Action<Exception?, int> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "chargeStartThresholdChanged", (Message m, object? s) => ReadMessage_i(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchChargeStopThresholdChangedAsync(Action<Exception?, int> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "chargeStopThresholdChanged", (Message m, object? s) => ReadMessage_i(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchSupportedActionsChangedAsync(Action<Exception?> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "supportedActionsChanged", handler, emitOnCapturedContext, flags);
    }
    partial class BrightnessControl : PowerManagementObject
    {
        private const string __Interface = "org.kde.Solid.PowerManagement.Actions.BrightnessControl";
        public BrightnessControl(PowerManagementService service, ObjectPath path) : base(service, path)
        { }
        public Task SetBrightnessAsync(int a0)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "i",
                    member: "setBrightness");
                writer.WriteInt32(a0);
                return writer.CreateMessage();
            }
        }
        public Task SetBrightnessSilentAsync(int a0)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "i",
                    member: "setBrightnessSilent");
                writer.WriteInt32(a0);
                return writer.CreateMessage();
            }
        }
        public Task<int> BrightnessAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_i(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "brightness");
                return writer.CreateMessage();
            }
        }
        public Task<int> BrightnessMaxAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_i(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "brightnessMax");
                return writer.CreateMessage();
            }
        }
        public Task<int> BrightnessMinAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_i(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "brightnessMin");
                return writer.CreateMessage();
            }
        }
        public Task<int> KnownSafeBrightnessMinAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_i(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "knownSafeBrightnessMin");
                return writer.CreateMessage();
            }
        }
        public Task<int> BrightnessStepsAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_i(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "brightnessSteps");
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchBrightnessChangedAsync(Action<Exception?, int> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "brightnessChanged", (Message m, object? s) => ReadMessage_i(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchBrightnessMaxChangedAsync(Action<Exception?, int> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "brightnessMaxChanged", (Message m, object? s) => ReadMessage_i(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchBrightnessMinChangedAsync(Action<Exception?, int> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "brightnessMinChanged", (Message m, object? s) => ReadMessage_i(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
    }
    partial class HandleButtonEvents : PowerManagementObject
    {
        private const string __Interface = "org.kde.Solid.PowerManagement.Actions.HandleButtonEvents";
        public HandleButtonEvents(PowerManagementService service, ObjectPath path) : base(service, path)
        { }
        public Task<int> LidActionAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_i(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "lidAction");
                return writer.CreateMessage();
            }
        }
        public Task<bool> TriggersLidActionAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "triggersLidAction");
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchTriggersLidActionChangedAsync(Action<Exception?, bool> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "triggersLidActionChanged", (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
    }
    partial class PowerProfile : PowerManagementObject
    {
        private const string __Interface = "org.kde.Solid.PowerManagement.Actions.PowerProfile";
        public PowerProfile(PowerManagementService service, ObjectPath path) : base(service, path)
        { }
        public Task<string> ConfiguredProfileAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_s(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "configuredProfile");
                return writer.CreateMessage();
            }
        }
        public Task<string> CurrentProfileAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_s(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "currentProfile");
                return writer.CreateMessage();
            }
        }
        public Task<string[]> ProfileChoicesAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_as(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "profileChoices");
                return writer.CreateMessage();
            }
        }
        public Task SetProfileAsync(string a0)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "setProfile");
                writer.WriteString(a0);
                return writer.CreateMessage();
            }
        }
        public Task<string> PerformanceInhibitedReasonAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_s(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "performanceInhibitedReason");
                return writer.CreateMessage();
            }
        }
        public Task<string> PerformanceDegradedReasonAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_s(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "performanceDegradedReason");
                return writer.CreateMessage();
            }
        }
        public Task<Dictionary<string, VariantValue>[]> ProfileHoldsAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aaesv(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "profileHolds");
                return writer.CreateMessage();
            }
        }
        public Task<uint> HoldProfileAsync(string profile, string reason, string applicationId)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_u(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "sss",
                    member: "holdProfile");
                writer.WriteString(profile);
                writer.WriteString(reason);
                writer.WriteString(applicationId);
                return writer.CreateMessage();
            }
        }
        public Task ReleaseProfileAsync(uint cookie)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "u",
                    member: "releaseProfile");
                writer.WriteUInt32(cookie);
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchConfiguredProfileChangedAsync(Action<Exception?, string> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "configuredProfileChanged", (Message m, object? s) => ReadMessage_s(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchCurrentProfileChangedAsync(Action<Exception?, string> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "currentProfileChanged", (Message m, object? s) => ReadMessage_s(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchProfileChoicesChangedAsync(Action<Exception?, string[]> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "profileChoicesChanged", (Message m, object? s) => ReadMessage_as(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchPerformanceInhibitedReasonChangedAsync(Action<Exception?, string> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "performanceInhibitedReasonChanged", (Message m, object? s) => ReadMessage_s(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchPerformanceDegradedReasonChangedAsync(Action<Exception?, string> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "performanceDegradedReasonChanged", (Message m, object? s) => ReadMessage_s(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchProfileHoldsChangedAsync(Action<Exception?, Dictionary<string, VariantValue>[]> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "profileHoldsChanged", (Message m, object? s) => ReadMessage_aaesv(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
    }
    partial class SuspendSession : PowerManagementObject
    {
        private const string __Interface = "org.kde.Solid.PowerManagement.Actions.SuspendSession";
        public SuspendSession(PowerManagementService service, ObjectPath path) : base(service, path)
        { }
        public Task SuspendToRamAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "suspendToRam");
                return writer.CreateMessage();
            }
        }
        public Task SuspendToDiskAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "suspendToDisk");
                return writer.CreateMessage();
            }
        }
        public Task SuspendHybridAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "suspendHybrid");
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchAboutToSuspendAsync(Action<Exception?> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "aboutToSuspend", handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchResumingFromSuspendAsync(Action<Exception?> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "resumingFromSuspend", handler, emitOnCapturedContext, flags);
    }
    partial class PolicyAgent : PowerManagementObject
    {
        private const string __Interface = "org.kde.Solid.PowerManagement.PolicyAgent";
        public PolicyAgent(PowerManagementService service, ObjectPath path) : base(service, path)
        { }
        public Task<uint> AddInhibitionAsync(uint types, string appName, string reason)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_u(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "uss",
                    member: "AddInhibition");
                writer.WriteUInt32(types);
                writer.WriteString(appName);
                writer.WriteString(reason);
                return writer.CreateMessage();
            }
        }
        public Task ReleaseInhibitionAsync(uint cookie)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "u",
                    member: "ReleaseInhibition");
                writer.WriteUInt32(cookie);
                return writer.CreateMessage();
            }
        }
        public Task<Dictionary<string, string>> ListInhibitionsAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aess(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "ListInhibitions");
                return writer.CreateMessage();
            }
        }
        public Task<bool> HasInhibitionAsync(uint types)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "u",
                    member: "HasInhibition");
                writer.WriteUInt32(types);
                return writer.CreateMessage();
            }
        }
        public Task BlockInhibitionAsync(string appName, string reason, bool permanently)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "ssb",
                    member: "BlockInhibition");
                writer.WriteString(appName);
                writer.WriteString(reason);
                writer.WriteBool(permanently);
                return writer.CreateMessage();
            }
        }
        public Task UnblockInhibitionAsync(string appName, string reason, bool permanently)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "ssb",
                    member: "UnblockInhibition");
                writer.WriteString(appName);
                writer.WriteString(reason);
                writer.WriteBool(permanently);
                return writer.CreateMessage();
            }
        }
        public Task<Dictionary<string, string>> ListPermanentlyBlockedInhibitionsAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aess(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "ListPermanentlyBlockedInhibitions");
                return writer.CreateMessage();
            }
        }
        public Task<Dictionary<string, string>> ListTemporarilyBlockedInhibitionsAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aess(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "ListTemporarilyBlockedInhibitions");
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchInhibitionsChangedAsync(Action<Exception?, (Dictionary<string, string> Added, string[] Removed)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "InhibitionsChanged", (Message m, object? s) => ReadMessage_aessas(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchPermanentlyBlockedInhibitionsChangedAsync(Action<Exception?, (Dictionary<string, string> Added, Dictionary<string, string> Removed)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "PermanentlyBlockedInhibitionsChanged", (Message m, object? s) => ReadMessage_aessaess(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchTemporarilyBlockedInhibitionsChangedAsync(Action<Exception?, (Dictionary<string, string> Added, Dictionary<string, string> Removed)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "TemporarilyBlockedInhibitionsChanged", (Message m, object? s) => ReadMessage_aessaess(m, (PowerManagementObject)s!), handler, emitOnCapturedContext, flags);
    }
    partial class Application : PowerManagementObject
    {
        private const string __Interface = "org.freedesktop.Application";
        public Application(PowerManagementService service, ObjectPath path) : base(service, path)
        { }
        public Task ActivateAsync(Dictionary<string, VariantValue> platformData)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "a{sv}",
                    member: "Activate");
                writer.WriteDictionary(platformData);
                return writer.CreateMessage();
            }
        }
        public Task OpenAsync(string[] uris, Dictionary<string, VariantValue> platformData)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "asa{sv}",
                    member: "Open");
                writer.WriteArray(uris);
                writer.WriteDictionary(platformData);
                return writer.CreateMessage();
            }
        }
        public Task ActivateActionAsync(string actionName, VariantValue[] parameter, Dictionary<string, VariantValue> platformData)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "sava{sv}",
                    member: "ActivateAction");
                writer.WriteString(actionName);
                writer.WriteArray(parameter);
                writer.WriteDictionary(platformData);
                return writer.CreateMessage();
            }
        }
    }
    partial class KDBusService : PowerManagementObject
    {
        private const string __Interface = "org.kde.KDBusService";
        public KDBusService(PowerManagementService service, ObjectPath path) : base(service, path)
        { }
        public Task<int> CommandLineAsync(string[] arguments, string workingDir, Dictionary<string, VariantValue> platformData)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_i(m, (PowerManagementObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "assa{sv}",
                    member: "CommandLine");
                writer.WriteArray(arguments);
                writer.WriteString(workingDir);
                writer.WriteDictionary(platformData);
                return writer.CreateMessage();
            }
        }
    }
    partial class PowerManagementService
    {
        public Tmds.DBus.Protocol.Connection Connection { get; }
        public string Destination { get; }
        public PowerManagementService(Tmds.DBus.Protocol.Connection connection, string destination)
            => (Connection, Destination) = (connection, destination);
        public QGuiApplication CreateQGuiApplication(ObjectPath path) => new QGuiApplication(this, path);
        public QCoreApplication CreateQCoreApplication(ObjectPath path) => new QCoreApplication(this, path);
        public PowerManagement CreatePowerManagement(ObjectPath path) => new PowerManagement(this, path);
        public Inhibit CreateInhibit(ObjectPath path) => new Inhibit(this, path);
        public ScreenBrightness CreateScreenBrightness(ObjectPath path) => new ScreenBrightness(this, path);
        public Display CreateDisplay(ObjectPath path) => new Display(this, path);
        public PowerManagement0 CreatePowerManagement0(ObjectPath path) => new PowerManagement0(this, path);
        public BrightnessControl CreateBrightnessControl(ObjectPath path) => new BrightnessControl(this, path);
        public HandleButtonEvents CreateHandleButtonEvents(ObjectPath path) => new HandleButtonEvents(this, path);
        public PowerProfile CreatePowerProfile(ObjectPath path) => new PowerProfile(this, path);
        public SuspendSession CreateSuspendSession(ObjectPath path) => new SuspendSession(this, path);
        public PolicyAgent CreatePolicyAgent(ObjectPath path) => new PolicyAgent(this, path);
        public Application CreateApplication(ObjectPath path) => new Application(this, path);
        public KDBusService CreateKDBusService(ObjectPath path) => new KDBusService(this, path);
    }
    class PowerManagementObject
    {
        public PowerManagementService Service { get; }
        public ObjectPath Path { get; }
        protected Tmds.DBus.Protocol.Connection Connection => Service.Connection;
        protected PowerManagementObject(PowerManagementService service, ObjectPath path)
            => (Service, Path) = (service, path);
        protected MessageBuffer CreateGetPropertyMessage(string @interface, string property)
        {
            var writer = this.Connection.GetMessageWriter();
            writer.WriteMethodCallHeader(
                destination: Service.Destination,
                path: Path,
                @interface: "org.freedesktop.DBus.Properties",
                signature: "ss",
                member: "Get");
            writer.WriteString(@interface);
            writer.WriteString(property);
            return writer.CreateMessage();
        }
        protected MessageBuffer CreateGetAllPropertiesMessage(string @interface)
        {
            var writer = this.Connection.GetMessageWriter();
            writer.WriteMethodCallHeader(
                destination: Service.Destination,
                path: Path,
                @interface: "org.freedesktop.DBus.Properties",
                signature: "s",
                member: "GetAll");
            writer.WriteString(@interface);
            return writer.CreateMessage();
        }
        protected ValueTask<IDisposable> WatchPropertiesChangedAsync<TProperties>(string @interface, MessageValueReader<PropertyChanges<TProperties>> reader, Action<Exception?, PropertyChanges<TProperties>> handler, bool emitOnCapturedContext, ObserverFlags flags)
        {
            var rule = new MatchRule
            {
                Type = MessageType.Signal,
                Sender = Service.Destination,
                Path = Path,
                Interface = "org.freedesktop.DBus.Properties",
                Member = "PropertiesChanged",
                Arg0 = @interface
            };
            return this.Connection.AddMatchAsync(rule, reader,
                                                    (Exception? ex, PropertyChanges<TProperties> changes, object? rs, object? hs) => ((Action<Exception?, PropertyChanges<TProperties>>)hs!).Invoke(ex, changes),
                                                    this, handler, emitOnCapturedContext, flags);
        }
        public ValueTask<IDisposable> WatchSignalAsync<TArg>(string sender, string @interface, ObjectPath path, string signal, MessageValueReader<TArg> reader, Action<Exception?, TArg> handler, bool emitOnCapturedContext, ObserverFlags flags)
        {
            var rule = new MatchRule
            {
                Type = MessageType.Signal,
                Sender = sender,
                Path = path,
                Member = signal,
                Interface = @interface
            };
            return this.Connection.AddMatchAsync(rule, reader,
                                                    (Exception? ex, TArg arg, object? rs, object? hs) => ((Action<Exception?, TArg>)hs!).Invoke(ex, arg),
                                                    this, handler, emitOnCapturedContext, flags);
        }
        public ValueTask<IDisposable> WatchSignalAsync(string sender, string @interface, ObjectPath path, string signal, Action<Exception?> handler, bool emitOnCapturedContext, ObserverFlags flags)
        {
            var rule = new MatchRule
            {
                Type = MessageType.Signal,
                Sender = sender,
                Path = path,
                Member = signal,
                Interface = @interface
            };
            return this.Connection.AddMatchAsync<object>(rule, (Message message, object? state) => null!,
                                                            (Exception? ex, object v, object? rs, object? hs) => ((Action<Exception?>)hs!).Invoke(ex), this, handler, emitOnCapturedContext, flags);
        }
        protected static string ReadMessage_v_s(Message message, PowerManagementObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("s"u8);
            return reader.ReadString();
        }
        protected static bool ReadMessage_v_b(Message message, PowerManagementObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("b"u8);
            return reader.ReadBool();
        }
        protected static bool ReadMessage_b(Message message, PowerManagementObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadBool();
        }
        protected static uint ReadMessage_u(Message message, PowerManagementObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadUInt32();
        }
        protected static string ReadMessage_s(Message message, PowerManagementObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadString();
        }
        protected static (string, int, string, string) ReadMessage_siss(Message message, PowerManagementObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadString();
            var arg1 = reader.ReadInt32();
            var arg2 = reader.ReadString();
            var arg3 = reader.ReadString();
            return (arg0, arg1, arg2, arg3);
        }
        protected static (string, int, int) ReadMessage_sii(Message message, PowerManagementObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadString();
            var arg1 = reader.ReadInt32();
            var arg2 = reader.ReadInt32();
            return (arg0, arg1, arg2);
        }
        protected static string[] ReadMessage_v_as(Message message, PowerManagementObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("as"u8);
            return reader.ReadArrayOfString();
        }
        protected static int ReadMessage_v_i(Message message, PowerManagementObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("i"u8);
            return reader.ReadInt32();
        }
        protected static ulong ReadMessage_t(Message message, PowerManagementObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadUInt64();
        }
        protected static int ReadMessage_i(Message message, PowerManagementObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadInt32();
        }
        protected static string[] ReadMessage_as(Message message, PowerManagementObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadArrayOfString();
        }
        protected static Dictionary<string, VariantValue>[] ReadMessage_aaesv(Message message, PowerManagementObject _)
        {
            var reader = message.GetBodyReader();
            return ReadType_aaesv(ref reader);
        }
        protected static Dictionary<string, string> ReadMessage_aess(Message message, PowerManagementObject _)
        {
            var reader = message.GetBodyReader();
            return ReadType_aess(ref reader);
        }
        protected static (Dictionary<string, string>, string[]) ReadMessage_aessas(Message message, PowerManagementObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = ReadType_aess(ref reader);
            var arg1 = reader.ReadArrayOfString();
            return (arg0, arg1);
        }
        protected static (Dictionary<string, string>, Dictionary<string, string>) ReadMessage_aessaess(Message message, PowerManagementObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = ReadType_aess(ref reader);
            var arg1 = ReadType_aess(ref reader);
            return (arg0, arg1);
        }
        protected static Dictionary<string, VariantValue>[] ReadType_aaesv(ref Reader reader)
        {
            List<Dictionary<string, VariantValue>> list = new();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Array);
            while (reader.HasNext(arrayEnd))
            {
                list.Add(reader.ReadDictionaryOfStringToVariantValue());
            }
            return list.ToArray();
        }
        protected static Dictionary<string, string> ReadType_aess(ref Reader reader)
        {
            Dictionary<string, string> dictionary = new();
            ArrayEnd dictEnd = reader.ReadDictionaryStart();
            while (reader.HasNext(dictEnd))
            {
                var key = reader.ReadString();
                var value = reader.ReadString();
                dictionary[key] = value;
            }
            return dictionary;
        }
    }
    class PropertyChanges<TProperties>
    {
        public PropertyChanges(TProperties properties, string[] invalidated, string[] changed)
        	=> (Properties, Invalidated, Changed) = (properties, invalidated, changed);
        public TProperties Properties { get; }
        public string[] Invalidated { get; }
        public string[] Changed { get; }
        public bool HasChanged(string property) => Array.IndexOf(Changed, property) != -1;
        public bool IsInvalidated(string property) => Array.IndexOf(Invalidated, property) != -1;
    }
}
