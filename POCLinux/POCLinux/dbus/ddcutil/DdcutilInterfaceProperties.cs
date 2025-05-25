namespace POCLinux.dbus.ddcutil;

record DdcutilInterfaceProperties
{
    public string[] AttributesReturnedByDetect { get; set; } = [];
    public Dictionary<int, string> StatusValues { get; set; } = new();
    public string? DdcutilVersion { get; set; }
    public bool DdcutilDynamicSleep { get; set; }
    public uint DdcutilOutputLevel { get; set; }
    public Dictionary<int, string> DisplayEventTypes { get; set; } = new();
    public string? ServiceInterfaceVersion { get; set; }
    public bool ServiceInfoLogging { get; set; }
    public bool ServiceEmitConnectivitySignals { get; set; }
    public bool ServiceEmitSignals { get; set; }
    public Dictionary<int, string> ServiceFlagOptions { get; set; } = new();
    public bool ServiceParametersLocked { get; set; }
    public uint ServicePollInterval { get; set; }
    public double ServicePollCascadeInterval { get; set; }
}
