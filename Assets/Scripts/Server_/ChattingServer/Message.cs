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
    public class StartMessage // Start
    {
        [Key(0)] public virtual Command Command { get; set; }
        [Key(1)] public virtual int ChannelIndex { get; set; }
        [Key(2)] public virtual string GameMode { get; set; }
        [Key(3)] public virtual string EscapeString { get; set; }
    }
    
    [MessagePackObject]
    public class EndMessage // SEnd
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
        [Key(9)] public virtual string GameMode { get; set; }
        [Key(10)] public virtual bool IsOnGame { get; set; }
    }
    
    
    // UDP
    [MessagePackObject]
    public class UDPMessageForm
    {
        [Key(0)] public virtual Command command_ { get; set; }
        [Key(1)] public virtual int channel_number_ { get; set; }
        [Key(2)] public virtual int user_index_ { get; set; }
        [Key(3)] public virtual float x_ { get; set; }
        [Key(4)] public virtual float y_ { get; set; }
        [Key(5)] public virtual float z_ { get; set; }
        [Key(6)] public virtual float rw_ { get; set; }
        [Key(7)] public virtual float rx_ { get; set; }
        [Key(8)] public virtual float ry_ { get; set; }
        [Key(9)] public virtual float rz_ { get; set; }

        public override string ToString()
        {
            return $"command_: {command_}\n" +
                   $"channel_number_: {channel_number_}\n" +
                   $"user_index_: {user_index_}\n" +
                   $"x_: {x_}\n" +
                   $"y_: {y_}\n" +
                   $"z_: {z_}\n" +
                   $"rw_: {rw_}\n" +
                   $"rx_: {rx_}\n" +
                   $"ry_: {ry_}\n" +
                   $"rz_: {rz_}\n";
        }
    }
}