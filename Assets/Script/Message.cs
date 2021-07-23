// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct Message : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_2_0_0(); }
  public static Message GetRootAsMessage(ByteBuffer _bb) { return GetRootAsMessage(_bb, new Message()); }
  public static Message GetRootAsMessage(ByteBuffer _bb, Message obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public Message __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public MESSAGE_ID PacketType { get { int o = __p.__offset(4); return o != 0 ? (MESSAGE_ID)__p.bb.Get(o + __p.bb_pos) : MESSAGE_ID.NONE; } }
  public TTable? Packet<TTable>() where TTable : struct, IFlatbufferObject { int o = __p.__offset(6); return o != 0 ? (TTable?)__p.__union<TTable>(o + __p.bb_pos) : null; }
  public S2C_MOVE PacketAsS2C_MOVE() { return Packet<S2C_MOVE>().Value; }
  public S2C_SHOOT PacketAsS2C_SHOOT() { return Packet<S2C_SHOOT>().Value; }
  public C2S_MOVE PacketAsC2S_MOVE() { return Packet<C2S_MOVE>().Value; }
  public C2S_EXTEND_SESSION PacketAsC2S_EXTEND_SESSION() { return Packet<C2S_EXTEND_SESSION>().Value; }

  public static Offset<Message> CreateMessage(FlatBufferBuilder builder,
      MESSAGE_ID packet_type = MESSAGE_ID.NONE,
      int packetOffset = 0) {
    builder.StartTable(2);
    Message.AddPacket(builder, packetOffset);
    Message.AddPacketType(builder, packet_type);
    return Message.EndMessage(builder);
  }

  public static void StartMessage(FlatBufferBuilder builder) { builder.StartTable(2); }
  public static void AddPacketType(FlatBufferBuilder builder, MESSAGE_ID packetType) { builder.AddByte(0, (byte)packetType, 0); }
  public static void AddPacket(FlatBufferBuilder builder, int packetOffset) { builder.AddOffset(1, packetOffset, 0); }
  public static Offset<Message> EndMessage(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<Message>(o);
  }
  public static void FinishMessageBuffer(FlatBufferBuilder builder, Offset<Message> offset) { builder.Finish(offset.Value); }
  public static void FinishSizePrefixedMessageBuffer(FlatBufferBuilder builder, Offset<Message> offset) { builder.FinishSizePrefixed(offset.Value); }
};

