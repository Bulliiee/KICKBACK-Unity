using MessagePack;

namespace Highlands.Server
{
    [MessagePackObject]
    public class Message
    {
        [Key(0)] public virtual Command command { get; set; }
        [Key(1)] public virtual int channelIndex { get; set; }
        [Key(2)] public virtual string userName { get; set; }
        [Key(3)] public virtual string message { get; set; }
    }

    [MessagePackObject]
    public class InitialSendMessage
    {
        [Key(0)] public virtual Command Command { get; set; }
        [Key(1)] public virtual string UserName { get; set; }
        [Key(2)] public virtual string EscapeString { get; set; }
    }

    [MessagePackObject]
    public class CreateMessage
    {
        [Key(0)] public virtual Command Command { get; set; }
        [Key(1)] public virtual string UserName { get; set; }
        [Key(2)] public virtual string RoomName { get; set; }
        [Key(3)] public virtual string MapName { get; set; }
        [Key(4)] public virtual string GameMode { get; set; }
        [Key(5)] public virtual string EscapeString { get; set; }
    }

    [MessagePackObject]
    public class JLRMessage // Join,Leave,Ready
    {
        [Key(0)] public virtual Command Command { get; set; }
        [Key(1)] public virtual string UserName { get; set; }
        [Key(2)] public virtual int RoomIndex { get; set; }
        [Key(3)] public virtual string EscapeString { get; set; }
    }

    [MessagePackObject]
    public class StartOrEndMessage // Start,End
    {
        [Key(0)] public virtual Command Command { get; set; }
        [Key(1)] public virtual int RoomIndex { get; set; }
        [Key(2)] public virtual string EscapeString { get; set; }
    }

    [MessagePackObject]
    public class ChangeMapMessage
    {
        [Key(0)] public virtual Command Command { get; set; }
        [Key(1)] public virtual string MapName { get; set; }
        [Key(2)] public virtual int RoomIndex { get; set; }
        [Key(3)] public virtual string EscapeString { get; set; }
    }

    [MessagePackObject]
    public class TeamChangeMessage
    {
        [Key(0)] public virtual Command Command { get; set; }
        [Key(1)] public virtual int RoomIndex { get; set; }
        [Key(2)] public virtual string UserName { get; set; }
        [Key(3)] public virtual string EscapeString { get; set; }
    }

    [MessagePackObject]
    public class RecieveLoginMessage
    {
        [Key(0)] public virtual string Type { get; set; }
        [Key(1)] public virtual string List { get; set; }
        [Key(2)] public virtual int RoomIndex { get; set; }
        [Key(3)] public virtual string RoomName { get; set; }
        [Key(4)] public virtual string RoomManager { get; set; }
        [Key(5)] public virtual string MapName { get; set; }
        [Key(6)] public virtual string IsReady { get; set; }
        [Key(7)] public virtual string TeamColor { get; set; }
    }
}