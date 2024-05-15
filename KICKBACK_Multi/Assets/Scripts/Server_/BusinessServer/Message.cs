using System.Collections.Generic;
using MessagePack;

namespace Highlands.Server.BusinessServer
{
    [MessagePackObject]
    public class InitialMessage
    {   
        [Key(0)]
        public virtual Command Command { get; set; }
        [Key(1)]
        public virtual string UserName { get; set; }
        [Key(2)]
        public virtual string EscapeString { get; set; }
    }
    
    [MessagePackObject]
    public class CreateRoom
    {   
        [Key(0)]
        public virtual Command Command { get; set; }
        [Key(1)]
        public virtual string UserName { get; set; }
        [Key(2)]
        public virtual string RoomName { get; set; }
        [Key(3)]
        public virtual string MapName { get; set; }
        [Key(4)]
        public virtual string GameMode { get; set; }
        [Key(5)]
        public virtual string EscapeString { get; set; }
    }
    
    [MessagePackObject]
    public class JLRRoom // Join,Leave,Ready
    {   
        [Key(0)]
        public virtual Command Command { get; set; }
        [Key(1)]
        public virtual string UserName { get; set; }
        [Key(2)]
        public virtual int RoomIndex { get; set; }
        [Key(3)]
        public virtual string EscapeString { get; set; }
    }
    
    [MessagePackObject]
    public class StartOrEndGame // Start,End
    {   
        [Key(0)]
        public virtual Command Command { get; set; }
        [Key(1)]
        public virtual int RoomIndex { get; set; }
        [Key(2)]
        public virtual string EscapeString { get; set; }
    }
    
    [MessagePackObject]
    public class ChanegeMap
    {   
        [Key(0)]
        public virtual Command Command { get; set; }
        [Key(1)]
        public virtual string MapName { get; set; }
        [Key(2)]
        public virtual int RoomIndex { get; set; }
        [Key(3)]
        public virtual string EscapeString { get; set; }
    }

    [MessagePackObject]
    public class TEAMCHANGE
    {
        [Key(0)]
        public virtual Command Command { get; set; }
        [Key(1)]
        public virtual int RoomIndex { get; set; }
        [Key(2)]
        public virtual string UserName { get; set; }
        [Key(3)]
        public virtual string EscapeString { get; set; }
    }

    [MessagePackObject]
    public class RecieveLogin
    {
        [Key(0)]
        public virtual string Type { get; set; }
        [Key(1)]
        public virtual string List { get; set; }
        [Key(2)]
        public virtual int RoomIndex { get; set; }
        [Key(3)]
        public virtual string RoomName { get; set; }
        [Key(4)]
        public virtual string RoomManager { get; set; }
        [Key(5)]
        public virtual string MapName { get; set; }
        [Key(6)]
        public virtual string IsReady { get; set; }
        [Key(7)]
        public virtual string TeamColor { get; set; }
    }
}