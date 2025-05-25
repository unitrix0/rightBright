using Tmds.DBus.Protocol;

namespace POCLinux.dbus.ddcutil;

partial class DdcutilInterface : DdcutilServiceObject
{
    private const string DdcInterfaceUri = "com.ddcutil.DdcutilInterface";
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
                @interface: DdcInterfaceUri,
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
                @interface: DdcInterfaceUri,
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
                @interface: DdcInterfaceUri,
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
                @interface: DdcInterfaceUri,
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
                @interface: DdcInterfaceUri,
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
                @interface: DdcInterfaceUri,
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
                @interface: DdcInterfaceUri,
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
                @interface: DdcInterfaceUri,
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
                @interface: DdcInterfaceUri,
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
                @interface: DdcInterfaceUri,
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
                @interface: DdcInterfaceUri,
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
                @interface: DdcInterfaceUri,
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
                @interface: DdcInterfaceUri,
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
        => base.WatchSignalAsync(Service.Destination, DdcInterfaceUri, Path, "ConnectedDisplaysChanged", (Message m, object? s) => ReadMessage_siu(m, (DdcutilServiceObject)s!), handler, emitOnCapturedContext, flags);
    public ValueTask<IDisposable> WatchVcpValueChangedAsync(Action<Exception?, (int DisplayNumber, string EdidTxt, byte VcpCode, ushort VcpNewValue, string SourceClientName, string SourceClientContext, uint Flags)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        => base.WatchSignalAsync(Service.Destination, DdcInterfaceUri, Path, "VcpValueChanged", (Message m, object? s) => ReadMessage_isyqssu(m, (DdcutilServiceObject)s!), handler, emitOnCapturedContext, flags);
    public ValueTask<IDisposable> WatchServiceInitializedAsync(Action<Exception?, uint> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        => base.WatchSignalAsync(Service.Destination, DdcInterfaceUri, Path, "ServiceInitialized", (Message m, object? s) => ReadMessage_u(m, (DdcutilServiceObject)s!), handler, emitOnCapturedContext, flags);
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
            writer.WriteString(DdcInterfaceUri);
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
            writer.WriteString(DdcInterfaceUri);
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
            writer.WriteString(DdcInterfaceUri);
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
            writer.WriteString(DdcInterfaceUri);
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
            writer.WriteString(DdcInterfaceUri);
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
            writer.WriteString(DdcInterfaceUri);
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
            writer.WriteString(DdcInterfaceUri);
            writer.WriteString("ServicePollCascadeInterval");
            writer.WriteSignature("d");
            writer.WriteDouble(value);
            return writer.CreateMessage();
        }
    }
    public Task<string[]> GetAttributesReturnedByDetectAsync()
        => this.Connection.CallMethodAsync(CreateGetPropertyMessage(DdcInterfaceUri, "AttributesReturnedByDetect"), (Message m, object? s) => ReadMessage_v_as(m, (DdcutilServiceObject)s!), this);
    public Task<Dictionary<int, string>> GetStatusValuesAsync()
        => this.Connection.CallMethodAsync(CreateGetPropertyMessage(DdcInterfaceUri, "StatusValues"), (Message m, object? s) => ReadMessage_v_aeis(m, (DdcutilServiceObject)s!), this);
    public Task<string> GetDdcutilVersionAsync()
        => this.Connection.CallMethodAsync(CreateGetPropertyMessage(DdcInterfaceUri, "DdcutilVersion"), (Message m, object? s) => ReadMessage_v_s(m, (DdcutilServiceObject)s!), this);
    public Task<bool> GetDdcutilDynamicSleepAsync()
        => this.Connection.CallMethodAsync(CreateGetPropertyMessage(DdcInterfaceUri, "DdcutilDynamicSleep"), (Message m, object? s) => ReadMessage_v_b(m, (DdcutilServiceObject)s!), this);
    public Task<uint> GetDdcutilOutputLevelAsync()
        => this.Connection.CallMethodAsync(CreateGetPropertyMessage(DdcInterfaceUri, "DdcutilOutputLevel"), (Message m, object? s) => ReadMessage_v_u(m, (DdcutilServiceObject)s!), this);
    public Task<Dictionary<int, string>> GetDisplayEventTypesAsync()
        => this.Connection.CallMethodAsync(CreateGetPropertyMessage(DdcInterfaceUri, "DisplayEventTypes"), (Message m, object? s) => ReadMessage_v_aeis(m, (DdcutilServiceObject)s!), this);
    public Task<string> GetServiceInterfaceVersionAsync()
        => this.Connection.CallMethodAsync(CreateGetPropertyMessage(DdcInterfaceUri, "ServiceInterfaceVersion"), (Message m, object? s) => ReadMessage_v_s(m, (DdcutilServiceObject)s!), this);
    public Task<bool> GetServiceInfoLoggingAsync()
        => this.Connection.CallMethodAsync(CreateGetPropertyMessage(DdcInterfaceUri, "ServiceInfoLogging"), (Message m, object? s) => ReadMessage_v_b(m, (DdcutilServiceObject)s!), this);
    public Task<bool> GetServiceEmitConnectivitySignalsAsync()
        => this.Connection.CallMethodAsync(CreateGetPropertyMessage(DdcInterfaceUri, "ServiceEmitConnectivitySignals"), (Message m, object? s) => ReadMessage_v_b(m, (DdcutilServiceObject)s!), this);
    public Task<bool> GetServiceEmitSignalsAsync()
        => this.Connection.CallMethodAsync(CreateGetPropertyMessage(DdcInterfaceUri, "ServiceEmitSignals"), (Message m, object? s) => ReadMessage_v_b(m, (DdcutilServiceObject)s!), this);
    public Task<Dictionary<int, string>> GetServiceFlagOptionsAsync()
        => this.Connection.CallMethodAsync(CreateGetPropertyMessage(DdcInterfaceUri, "ServiceFlagOptions"), (Message m, object? s) => ReadMessage_v_aeis(m, (DdcutilServiceObject)s!), this);
    public Task<bool> GetServiceParametersLockedAsync()
        => this.Connection.CallMethodAsync(CreateGetPropertyMessage(DdcInterfaceUri, "ServiceParametersLocked"), (Message m, object? s) => ReadMessage_v_b(m, (DdcutilServiceObject)s!), this);
    public Task<uint> GetServicePollIntervalAsync()
        => this.Connection.CallMethodAsync(CreateGetPropertyMessage(DdcInterfaceUri, "ServicePollInterval"), (Message m, object? s) => ReadMessage_v_u(m, (DdcutilServiceObject)s!), this);
    public Task<double> GetServicePollCascadeIntervalAsync()
        => this.Connection.CallMethodAsync(CreateGetPropertyMessage(DdcInterfaceUri, "ServicePollCascadeInterval"), (Message m, object? s) => ReadMessage_v_d(m, (DdcutilServiceObject)s!), this);
    public Task<DdcutilInterfaceProperties> GetPropertiesAsync()
    {
        return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(DdcInterfaceUri), (Message m, object? s) => ReadMessage(m, (DdcutilServiceObject)s!), this);
        static DdcutilInterfaceProperties ReadMessage(Message message, DdcutilServiceObject _)
        {
            var reader = message.GetBodyReader();
            return ReadProperties(ref reader);
        }
    }
    public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<DdcutilInterfaceProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
    {
        return base.WatchPropertiesChangedAsync(DdcInterfaceUri, (Message m, object? s) => ReadMessage(m, (DdcutilServiceObject)s!), handler, emitOnCapturedContext, flags);
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
