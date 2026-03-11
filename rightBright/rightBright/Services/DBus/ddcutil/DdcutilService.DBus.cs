namespace rightBright.Services.DBus.ddcutil
{
    using System;
    using Tmds.DBus.Protocol;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    record DdcutilInterfaceProperties
    {
        public string[] AttributesReturnedByDetect { get; set; } = default!;
        public Dictionary<int, string> StatusValues { get; set; } = default!;
        public string DdcutilVersion { get; set; } = default!;
        public bool DdcutilDynamicSleep { get; set; } = default!;
        public uint DdcutilOutputLevel { get; set; } = default!;
        public Dictionary<int, string> DisplayEventTypes { get; set; } = default!;
        public string ServiceInterfaceVersion { get; set; } = default!;
        public bool ServiceInfoLogging { get; set; } = default!;
        public bool ServiceEmitConnectivitySignals { get; set; } = default!;
        public bool ServiceEmitSignals { get; set; } = default!;
        public Dictionary<int, string> ServiceFlagOptions { get; set; } = default!;
        public bool ServiceParametersLocked { get; set; } = default!;
        public uint ServicePollInterval { get; set; } = default!;
        public double ServicePollCascadeInterval { get; set; } = default!;
    }
    partial class DdcutilInterface : DdcutilServiceObject
    {
        private const string __Interface = "com.ddcutil.DdcutilInterface";
        public DdcutilInterface(DdcutilService service, ObjectPath path) : base(service, path)
        { }
        public Task<(int ErrorStatus, string ErrorMessage)> RestartAsync(string textOptions, uint syslogLevel, uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_is(m, (DdcutilServiceObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "suu",
                    member: "Restart");
                writer.WriteString(textOptions);
                writer.WriteUInt32(syslogLevel);
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        public Task<(int NumberOfDisplays, (int, int, int, string, string, string, ushort, string, uint)[] DetectedDisplays, int ErrorStatus, string ErrorMessage)> DetectAsync(uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_iariiisssqsuzis(m, (DdcutilServiceObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "u",
                    member: "Detect");
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        public Task<(int NumberOfDisplays, (int, int, int, string, string, string, ushort, string, uint)[] DetectedDisplays, int ErrorStatus, string ErrorMessage)> ListDetectedAsync(uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_iariiisssqsuzis(m, (DdcutilServiceObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "u",
                    member: "ListDetected");
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        public Task<(ushort VcpCurrentValue, ushort VcpMaxValue, string VcpFormattedValue, int ErrorStatus, string ErrorMessage)> GetVcpAsync(int displayNumber, string edidTxt, byte vcpCode, uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_qqsis(m, (DdcutilServiceObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "isyu",
                    member: "GetVcp");
                writer.WriteInt32(displayNumber);
                writer.WriteString(edidTxt);
                writer.WriteByte(vcpCode);
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        public Task<((byte, ushort, ushort, string)[] VcpCurrentValue, int ErrorStatus, string ErrorMessage)> GetMultipleVcpAsync(int displayNumber, string edidTxt, byte[] vcpCode, uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aryqqszis(m, (DdcutilServiceObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "isayu",
                    member: "GetMultipleVcp");
                writer.WriteInt32(displayNumber);
                writer.WriteString(edidTxt);
                writer.WriteArray(vcpCode);
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        public Task<(int ErrorStatus, string ErrorMessage)> SetVcpAsync(int displayNumber, string edidTxt, byte vcpCode, ushort vcpNewValue, uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_is(m, (DdcutilServiceObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "isyqu",
                    member: "SetVcp");
                writer.WriteInt32(displayNumber);
                writer.WriteString(edidTxt);
                writer.WriteByte(vcpCode);
                writer.WriteUInt16(vcpNewValue);
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        public Task<(int ErrorStatus, string ErrorMessage)> SetVcpWithContextAsync(int displayNumber, string edidTxt, byte vcpCode, ushort vcpNewValue, string clientContext, uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_is(m, (DdcutilServiceObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "isyqsu",
                    member: "SetVcpWithContext");
                writer.WriteInt32(displayNumber);
                writer.WriteString(edidTxt);
                writer.WriteByte(vcpCode);
                writer.WriteUInt16(vcpNewValue);
                writer.WriteString(clientContext);
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        public Task<(string FeatureName, string FeatureDescription, bool IsReadOnly, bool IsWriteOnly, bool IsRw, bool IsComplex, bool IsContinuous, int ErrorStatus, string ErrorMessage)> GetVcpMetadataAsync(int displayNumber, string edidTxt, byte vcpCode, uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_ssbbbbbis(m, (DdcutilServiceObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "isyu",
                    member: "GetVcpMetadata");
                writer.WriteInt32(displayNumber);
                writer.WriteString(edidTxt);
                writer.WriteByte(vcpCode);
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        public Task<(string CapabilitiesText, int ErrorStatus, string ErrorMessage)> GetCapabilitiesStringAsync(int displayNumber, string edidTxt, uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_sis(m, (DdcutilServiceObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "isu",
                    member: "GetCapabilitiesString");
                writer.WriteInt32(displayNumber);
                writer.WriteString(edidTxt);
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        public Task<(string ModelName, byte MccsMajor, byte MccsMinor, Dictionary<byte, string> Commands, Dictionary<byte, (string, string, Dictionary<byte, string>)> Capabilities, int ErrorStatus, string ErrorMessage)> GetCapabilitiesMetadataAsync(int displayNumber, string edidTxt, uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_syyaeysaeyrssaeyszis(m, (DdcutilServiceObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "isu",
                    member: "GetCapabilitiesMetadata");
                writer.WriteInt32(displayNumber);
                writer.WriteString(edidTxt);
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        public Task<(int Status, string Message)> GetDisplayStateAsync(int displayNumber, string edidTxt, uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_is(m, (DdcutilServiceObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "isu",
                    member: "GetDisplayState");
                writer.WriteInt32(displayNumber);
                writer.WriteString(edidTxt);
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        public Task<(double CurrentMultiplier, int ErrorStatus, string ErrorMessage)> GetSleepMultiplierAsync(int displayNumber, string edidTxt, uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_dis(m, (DdcutilServiceObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "isu",
                    member: "GetSleepMultiplier");
                writer.WriteInt32(displayNumber);
                writer.WriteString(edidTxt);
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        public Task<(int ErrorStatus, string ErrorMessage)> SetSleepMultiplierAsync(int displayNumber, string edidTxt, double newMultiplier, uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_is(m, (DdcutilServiceObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "isdu",
                    member: "SetSleepMultiplier");
                writer.WriteInt32(displayNumber);
                writer.WriteString(edidTxt);
                writer.WriteDouble(newMultiplier);
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchConnectedDisplaysChangedAsync(Action<Exception?, (string EdidTxt, int EventType, uint Flags)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "ConnectedDisplaysChanged", (Message m, object? s) => ReadMessage_siu(m, (DdcutilServiceObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchVcpValueChangedAsync(Action<Exception?, (int DisplayNumber, string EdidTxt, byte VcpCode, ushort VcpNewValue, string SourceClientName, string SourceClientContext, uint Flags)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "VcpValueChanged", (Message m, object? s) => ReadMessage_isyqssu(m, (DdcutilServiceObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchServiceInitializedAsync(Action<Exception?, uint> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "ServiceInitialized", (Message m, object? s) => ReadMessage_u(m, (DdcutilServiceObject)s!), handler, emitOnCapturedContext, flags);
        public Task SetDdcutilDynamicSleepAsync(bool value)
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
                writer.WriteString("DdcutilDynamicSleep");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetDdcutilOutputLevelAsync(uint value)
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
                writer.WriteString("DdcutilOutputLevel");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        public Task SetServiceInfoLoggingAsync(bool value)
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
                writer.WriteString("ServiceInfoLogging");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetServiceEmitConnectivitySignalsAsync(bool value)
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
                writer.WriteString("ServiceEmitConnectivitySignals");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetServiceEmitSignalsAsync(bool value)
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
                writer.WriteString("ServiceEmitSignals");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetServicePollIntervalAsync(uint value)
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
                writer.WriteString("ServicePollInterval");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        public Task SetServicePollCascadeIntervalAsync(double value)
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
                writer.WriteString("ServicePollCascadeInterval");
                writer.WriteSignature("d");
                writer.WriteDouble(value);
                return writer.CreateMessage();
            }
        }
        public Task<string[]> GetAttributesReturnedByDetectAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "AttributesReturnedByDetect"), (Message m, object? s) => ReadMessage_v_as(m, (DdcutilServiceObject)s!), this);
        public Task<Dictionary<int, string>> GetStatusValuesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "StatusValues"), (Message m, object? s) => ReadMessage_v_aeis(m, (DdcutilServiceObject)s!), this);
        public Task<string> GetDdcutilVersionAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "DdcutilVersion"), (Message m, object? s) => ReadMessage_v_s(m, (DdcutilServiceObject)s!), this);
        public Task<bool> GetDdcutilDynamicSleepAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "DdcutilDynamicSleep"), (Message m, object? s) => ReadMessage_v_b(m, (DdcutilServiceObject)s!), this);
        public Task<uint> GetDdcutilOutputLevelAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "DdcutilOutputLevel"), (Message m, object? s) => ReadMessage_v_u(m, (DdcutilServiceObject)s!), this);
        public Task<Dictionary<int, string>> GetDisplayEventTypesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "DisplayEventTypes"), (Message m, object? s) => ReadMessage_v_aeis(m, (DdcutilServiceObject)s!), this);
        public Task<string> GetServiceInterfaceVersionAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "ServiceInterfaceVersion"), (Message m, object? s) => ReadMessage_v_s(m, (DdcutilServiceObject)s!), this);
        public Task<bool> GetServiceInfoLoggingAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "ServiceInfoLogging"), (Message m, object? s) => ReadMessage_v_b(m, (DdcutilServiceObject)s!), this);
        public Task<bool> GetServiceEmitConnectivitySignalsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "ServiceEmitConnectivitySignals"), (Message m, object? s) => ReadMessage_v_b(m, (DdcutilServiceObject)s!), this);
        public Task<bool> GetServiceEmitSignalsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "ServiceEmitSignals"), (Message m, object? s) => ReadMessage_v_b(m, (DdcutilServiceObject)s!), this);
        public Task<Dictionary<int, string>> GetServiceFlagOptionsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "ServiceFlagOptions"), (Message m, object? s) => ReadMessage_v_aeis(m, (DdcutilServiceObject)s!), this);
        public Task<bool> GetServiceParametersLockedAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "ServiceParametersLocked"), (Message m, object? s) => ReadMessage_v_b(m, (DdcutilServiceObject)s!), this);
        public Task<uint> GetServicePollIntervalAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "ServicePollInterval"), (Message m, object? s) => ReadMessage_v_u(m, (DdcutilServiceObject)s!), this);
        public Task<double> GetServicePollCascadeIntervalAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "ServicePollCascadeInterval"), (Message m, object? s) => ReadMessage_v_d(m, (DdcutilServiceObject)s!), this);
        public Task<DdcutilInterfaceProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (DdcutilServiceObject)s!), this);
            static DdcutilInterfaceProperties ReadMessage(Message message, DdcutilServiceObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<DdcutilInterfaceProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (DdcutilServiceObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<DdcutilInterfaceProperties> ReadMessage(Message message, DdcutilServiceObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<DdcutilInterfaceProperties>(ReadProperties(ref reader, changed), ReadInvalidated(ref reader), changed.ToArray());
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
                        case "AttributesReturnedByDetect": invalidated.Add("AttributesReturnedByDetect"); break;
                        case "StatusValues": invalidated.Add("StatusValues"); break;
                        case "DdcutilVersion": invalidated.Add("DdcutilVersion"); break;
                        case "DdcutilDynamicSleep": invalidated.Add("DdcutilDynamicSleep"); break;
                        case "DdcutilOutputLevel": invalidated.Add("DdcutilOutputLevel"); break;
                        case "DisplayEventTypes": invalidated.Add("DisplayEventTypes"); break;
                        case "ServiceInterfaceVersion": invalidated.Add("ServiceInterfaceVersion"); break;
                        case "ServiceInfoLogging": invalidated.Add("ServiceInfoLogging"); break;
                        case "ServiceEmitConnectivitySignals": invalidated.Add("ServiceEmitConnectivitySignals"); break;
                        case "ServiceEmitSignals": invalidated.Add("ServiceEmitSignals"); break;
                        case "ServiceFlagOptions": invalidated.Add("ServiceFlagOptions"); break;
                        case "ServiceParametersLocked": invalidated.Add("ServiceParametersLocked"); break;
                        case "ServicePollInterval": invalidated.Add("ServicePollInterval"); break;
                        case "ServicePollCascadeInterval": invalidated.Add("ServicePollCascadeInterval"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static DdcutilInterfaceProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new DdcutilInterfaceProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "AttributesReturnedByDetect":
                        reader.ReadSignature("as"u8);
                        props.AttributesReturnedByDetect = reader.ReadArrayOfString();
                        changedList?.Add("AttributesReturnedByDetect");
                        break;
                    case "StatusValues":
                        reader.ReadSignature("a{is}"u8);
                        props.StatusValues = ReadType_aeis(ref reader);
                        changedList?.Add("StatusValues");
                        break;
                    case "DdcutilVersion":
                        reader.ReadSignature("s"u8);
                        props.DdcutilVersion = reader.ReadString();
                        changedList?.Add("DdcutilVersion");
                        break;
                    case "DdcutilDynamicSleep":
                        reader.ReadSignature("b"u8);
                        props.DdcutilDynamicSleep = reader.ReadBool();
                        changedList?.Add("DdcutilDynamicSleep");
                        break;
                    case "DdcutilOutputLevel":
                        reader.ReadSignature("u"u8);
                        props.DdcutilOutputLevel = reader.ReadUInt32();
                        changedList?.Add("DdcutilOutputLevel");
                        break;
                    case "DisplayEventTypes":
                        reader.ReadSignature("a{is}"u8);
                        props.DisplayEventTypes = ReadType_aeis(ref reader);
                        changedList?.Add("DisplayEventTypes");
                        break;
                    case "ServiceInterfaceVersion":
                        reader.ReadSignature("s"u8);
                        props.ServiceInterfaceVersion = reader.ReadString();
                        changedList?.Add("ServiceInterfaceVersion");
                        break;
                    case "ServiceInfoLogging":
                        reader.ReadSignature("b"u8);
                        props.ServiceInfoLogging = reader.ReadBool();
                        changedList?.Add("ServiceInfoLogging");
                        break;
                    case "ServiceEmitConnectivitySignals":
                        reader.ReadSignature("b"u8);
                        props.ServiceEmitConnectivitySignals = reader.ReadBool();
                        changedList?.Add("ServiceEmitConnectivitySignals");
                        break;
                    case "ServiceEmitSignals":
                        reader.ReadSignature("b"u8);
                        props.ServiceEmitSignals = reader.ReadBool();
                        changedList?.Add("ServiceEmitSignals");
                        break;
                    case "ServiceFlagOptions":
                        reader.ReadSignature("a{is}"u8);
                        props.ServiceFlagOptions = ReadType_aeis(ref reader);
                        changedList?.Add("ServiceFlagOptions");
                        break;
                    case "ServiceParametersLocked":
                        reader.ReadSignature("b"u8);
                        props.ServiceParametersLocked = reader.ReadBool();
                        changedList?.Add("ServiceParametersLocked");
                        break;
                    case "ServicePollInterval":
                        reader.ReadSignature("u"u8);
                        props.ServicePollInterval = reader.ReadUInt32();
                        changedList?.Add("ServicePollInterval");
                        break;
                    case "ServicePollCascadeInterval":
                        reader.ReadSignature("d"u8);
                        props.ServicePollCascadeInterval = reader.ReadDouble();
                        changedList?.Add("ServicePollCascadeInterval");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    partial class DdcutilService
    {
        public Tmds.DBus.Protocol.Connection Connection { get; }
        public string Destination { get; }
        public DdcutilService(Tmds.DBus.Protocol.Connection connection, string destination)
            => (Connection, Destination) = (connection, destination);
        public DdcutilInterface CreateDdcutilInterface(ObjectPath path) => new DdcutilInterface(this, path);
    }
    class DdcutilServiceObject
    {
        public DdcutilService Service { get; }
        public ObjectPath Path { get; }
        protected Tmds.DBus.Protocol.Connection Connection => Service.Connection;
        protected DdcutilServiceObject(DdcutilService service, ObjectPath path)
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
        protected static (int, string) ReadMessage_is(Message message, DdcutilServiceObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadInt32();
            var arg1 = reader.ReadString();
            return (arg0, arg1);
        }
        protected static (int, (int, int, int, string, string, string, ushort, string, uint)[], int, string) ReadMessage_iariiisssqsuzis(Message message, DdcutilServiceObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadInt32();
            var arg1 = ReadType_ariiisssqsuz(ref reader);
            var arg2 = reader.ReadInt32();
            var arg3 = reader.ReadString();
            return (arg0, arg1, arg2, arg3);
        }
        protected static (ushort, ushort, string, int, string) ReadMessage_qqsis(Message message, DdcutilServiceObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadUInt16();
            var arg1 = reader.ReadUInt16();
            var arg2 = reader.ReadString();
            var arg3 = reader.ReadInt32();
            var arg4 = reader.ReadString();
            return (arg0, arg1, arg2, arg3, arg4);
        }
        protected static ((byte, ushort, ushort, string)[], int, string) ReadMessage_aryqqszis(Message message, DdcutilServiceObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = ReadType_aryqqsz(ref reader);
            var arg1 = reader.ReadInt32();
            var arg2 = reader.ReadString();
            return (arg0, arg1, arg2);
        }
        protected static (string, string, bool, bool, bool, bool, bool, int, string) ReadMessage_ssbbbbbis(Message message, DdcutilServiceObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadString();
            var arg1 = reader.ReadString();
            var arg2 = reader.ReadBool();
            var arg3 = reader.ReadBool();
            var arg4 = reader.ReadBool();
            var arg5 = reader.ReadBool();
            var arg6 = reader.ReadBool();
            var arg7 = reader.ReadInt32();
            var arg8 = reader.ReadString();
            return (arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }
        protected static (string, int, string) ReadMessage_sis(Message message, DdcutilServiceObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadString();
            var arg1 = reader.ReadInt32();
            var arg2 = reader.ReadString();
            return (arg0, arg1, arg2);
        }
        protected static (string, byte, byte, Dictionary<byte, string>, Dictionary<byte, (string, string, Dictionary<byte, string>)>, int, string) ReadMessage_syyaeysaeyrssaeyszis(Message message, DdcutilServiceObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadString();
            var arg1 = reader.ReadByte();
            var arg2 = reader.ReadByte();
            var arg3 = ReadType_aeys(ref reader);
            var arg4 = ReadType_aeyrssaeysz(ref reader);
            var arg5 = reader.ReadInt32();
            var arg6 = reader.ReadString();
            return (arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }
        protected static (double, int, string) ReadMessage_dis(Message message, DdcutilServiceObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadDouble();
            var arg1 = reader.ReadInt32();
            var arg2 = reader.ReadString();
            return (arg0, arg1, arg2);
        }
        protected static (string, int, uint) ReadMessage_siu(Message message, DdcutilServiceObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadString();
            var arg1 = reader.ReadInt32();
            var arg2 = reader.ReadUInt32();
            return (arg0, arg1, arg2);
        }
        protected static (int, string, byte, ushort, string, string, uint) ReadMessage_isyqssu(Message message, DdcutilServiceObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadInt32();
            var arg1 = reader.ReadString();
            var arg2 = reader.ReadByte();
            var arg3 = reader.ReadUInt16();
            var arg4 = reader.ReadString();
            var arg5 = reader.ReadString();
            var arg6 = reader.ReadUInt32();
            return (arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }
        protected static uint ReadMessage_u(Message message, DdcutilServiceObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadUInt32();
        }
        protected static string[] ReadMessage_v_as(Message message, DdcutilServiceObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("as"u8);
            return reader.ReadArrayOfString();
        }
        protected static Dictionary<int, string> ReadMessage_v_aeis(Message message, DdcutilServiceObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("a{is}"u8);
            return ReadType_aeis(ref reader);
        }
        protected static string ReadMessage_v_s(Message message, DdcutilServiceObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("s"u8);
            return reader.ReadString();
        }
        protected static bool ReadMessage_v_b(Message message, DdcutilServiceObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("b"u8);
            return reader.ReadBool();
        }
        protected static uint ReadMessage_v_u(Message message, DdcutilServiceObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("u"u8);
            return reader.ReadUInt32();
        }
        protected static double ReadMessage_v_d(Message message, DdcutilServiceObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("d"u8);
            return reader.ReadDouble();
        }
        protected static Dictionary<int, string> ReadType_aeis(ref Reader reader)
        {
            Dictionary<int, string> dictionary = new();
            ArrayEnd dictEnd = reader.ReadDictionaryStart();
            while (reader.HasNext(dictEnd))
            {
                var key = reader.ReadInt32();
                var value = reader.ReadString();
                dictionary[key] = value;
            }
            return dictionary;
        }
        protected static (int, int, int, string, string, string, ushort, string, uint)[] ReadType_ariiisssqsuz(ref Reader reader)
        {
            List<(int, int, int, string, string, string, ushort, string, uint)> list = new();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                list.Add(ReadType_riiisssqsuz(ref reader));
            }
            return list.ToArray();
        }
        protected static (int, int, int, string, string, string, ushort, string, uint) ReadType_riiisssqsuz(ref Reader reader)
        {
            return (reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadUInt16(), reader.ReadString(), reader.ReadUInt32());
        }
        protected static (byte, ushort, ushort, string)[] ReadType_aryqqsz(ref Reader reader)
        {
            List<(byte, ushort, ushort, string)> list = new();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                list.Add(ReadType_ryqqsz(ref reader));
            }
            return list.ToArray();
        }
        protected static (byte, ushort, ushort, string) ReadType_ryqqsz(ref Reader reader)
        {
            return (reader.ReadByte(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadString());
        }
        protected static Dictionary<byte, string> ReadType_aeys(ref Reader reader)
        {
            Dictionary<byte, string> dictionary = new();
            ArrayEnd dictEnd = reader.ReadDictionaryStart();
            while (reader.HasNext(dictEnd))
            {
                var key = reader.ReadByte();
                var value = reader.ReadString();
                dictionary[key] = value;
            }
            return dictionary;
        }
        protected static Dictionary<byte, (string, string, Dictionary<byte, string>)> ReadType_aeyrssaeysz(ref Reader reader)
        {
            Dictionary<byte, (string, string, Dictionary<byte, string>)> dictionary = new();
            ArrayEnd dictEnd = reader.ReadDictionaryStart();
            while (reader.HasNext(dictEnd))
            {
                var key = reader.ReadByte();
                var value = ReadType_rssaeysz(ref reader);
                dictionary[key] = value;
            }
            return dictionary;
        }
        protected static (string, string, Dictionary<byte, string>) ReadType_rssaeysz(ref Reader reader)
        {
            return (reader.ReadString(), reader.ReadString(), ReadType_aeys(ref reader));
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
