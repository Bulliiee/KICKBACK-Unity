using MessagePack;

namespace Highlands.Server
{
    [MessagePackObject]
    public class ChatMessage
    {
        [Key(0)] public virtual Command Command { get; set; }
        [Key(1)] public virtual int ChannelIndex { get; set; }
        [Key(2)] public virtual string UserName { get; set; }
        [Key(3)] public virtual string Message { get; set; }
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
        [Key(2)] public virtual string ChannelName { get; set; }
        [Key(3)] public virtual string MapName { get; set; }
        [Key(4)] public virtual string GameMode { get; set; }
        [Key(5)] public virtual string EscapeString { get; set; }
    }

    [MessagePackObject]
    public class JLRMessage // Join,Leave,Ready
    {
        [Key(0)] public virtual Command Command { get; set; }
        [Key(1)] public virtual string UserName { get; set; }
        [Key(2)] public virtual int ChannelIndex { get; set; }
        [Key(3)] public virtual string EscapeString { get; set; }
    }

    [MessagePackObject]
    public class StartOrEndMessage // Start,End
    {
        [Key(0)] public virtual Command Command { get; set; }
        [Key(1)] public virtual int ChannelIndex { get; set; }
        [Key(2)] public virtual string EscapeString { get; set; }
    }

    [MessagePackObject]
    public class ChangeMapMessage
    {
        [Key(0)] public virtual Command Command { get; set; }
        [Key(1)] public virtual string MapName { get; set; }
        [Key(2)] public virtual int ChannelIndex { get; set; }
        [Key(3)] public virtual string EscapeString { get; set; }
    }

    [MessagePackObject]
    public class TeamChangeMessage
    {
        [Key(0)] public virtual Command Command { get; set; }
        [Key(1)] public virtual int ChannelIndex { get; set; }
        [Key(2)] public virtual string UserName { get; set; }
        [Key(3)] public virtual string EscapeString { get; set; }
    }

    [MessagePackObject]
    public class CharacterChangeMessage
    {
        [Key(0)] public virtual Command Command { get; set; }
        [Key(1)] public virtual int ChannelIndex { get; set; }
        [Key(2)] public virtual string UserName { get; set; }
        [Key(3)] public virtual int CharacterIndex { get; set; }
        [Key(4)] public virtual string EscapeString { get; set; }
    }
    

    [MessagePackObject]
    public class RecieveLoginMessage
    {
        [Key(0)] public virtual string Type { get; set; }
        [Key(1)] public virtual string List { get; set; }
        [Key(2)] public virtual int ChannelIndex { get; set; }
        [Key(3)] public virtual string ChannelName { get; set; }
        [Key(4)] public virtual string ChannelManager { get; set; }
        [Key(5)] public virtual string MapName { get; set; }
        [Key(6)] public virtual string IsReady { get; set; }
        [Key(7)] public virtual string TeamColor { get; set; }
        [Key(8)] public virtual string UserCharacter { get; set; }
    }
}