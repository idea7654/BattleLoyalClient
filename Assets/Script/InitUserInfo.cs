// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct InitUserInfo : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_2_0_0(); }
  public static InitUserInfo GetRootAsInitUserInfo(ByteBuffer _bb) { return GetRootAsInitUserInfo(_bb, new InitUserInfo()); }
  public static InitUserInfo GetRootAsInitUserInfo(ByteBuffer _bb, InitUserInfo obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public InitUserInfo __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string Nickname { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetNicknameBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
  public ArraySegment<byte>? GetNicknameBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public byte[] GetNicknameArray() { return __p.__vector_as_array<byte>(4); }
  public Vec3? Pos { get { int o = __p.__offset(6); return o != 0 ? (Vec3?)(new Vec3()).__assign(o + __p.bb_pos, __p.bb) : null; } }

  public static void StartInitUserInfo(FlatBufferBuilder builder) { builder.StartTable(2); }
  public static void AddNickname(FlatBufferBuilder builder, StringOffset nicknameOffset) { builder.AddOffset(0, nicknameOffset.Value, 0); }
  public static void AddPos(FlatBufferBuilder builder, Offset<Vec3> posOffset) { builder.AddStruct(1, posOffset.Value, 0); }
  public static Offset<InitUserInfo> EndInitUserInfo(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<InitUserInfo>(o);
  }
};
