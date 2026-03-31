using Tmds.DBus.Protocol;

namespace POCLinux.dbus.ddcutil;

partial class DdcutilService
{
    public const string BusName = "com.ddcutil.DdcutilService";
    public Connection Connection { get; }
    public string Destination { get; }
    public DdcutilService(Connection connection, string destination)
        => (Connection, Destination) = (connection, destination);
    public DdcutilInterface CreateDdcutilInterface(ObjectPath path) => new DdcutilInterface(this, path);
}
