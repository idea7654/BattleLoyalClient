// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct S2C_REGISTER_ERROR : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_2_0_0(); }
  public static S2C_REGISTER_ERROR GetRootAsS2C_REGISTER_ERROR(ByteBuffer _bb) { return GetRootAsS2C_REGISTER_ERROR(_bb, new S2C_REGISTER_ERROR()); }
  public static S2C_REGISTER_ERROR GetRootAsS2C_REGISTER_ERROR(ByteBuffer _bb, S2C_REGISTER_ERROR obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public S2C_REGISTER_ERROR __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string Message { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetMessageBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
  public ArraySegment<byte>? GetMessageBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public byte[] GetMessageArray() { return __p.__vector_as_array<byte>(4); }

  public static Offset<S2C_REGISTER_ERROR> CreateS2C_REGISTER_ERROR(FlatBufferBuilder builder,
      StringOffset messageOffset = default(StringOffset)) {
    builder.StartTable(1);
    S2C_REGISTER_ERROR.AddMessage(builder, messageOffset);
    return S2C_REGISTER_ERROR.EndS2C_REGISTER_ERROR(builder);
  }

  public static void StartS2C_REGISTER_ERROR(FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddMessage(FlatBufferBuilder builder, StringOffset messageOffset) { builder.AddOffset(0, messageOffset.Value, 0); }
  public static Offset<S2C_REGISTER_ERROR> EndS2C_REGISTER_ERROR(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<S2C_REGISTER_ERROR>(o);
  }
};

