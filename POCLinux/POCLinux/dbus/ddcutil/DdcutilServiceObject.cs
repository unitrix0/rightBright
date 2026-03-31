using Tmds.DBus.Protocol;

namespace POCLinux.dbus.ddcutil;

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
