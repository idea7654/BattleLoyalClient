// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct S2C_COMPLETE_REGISTER : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_2_0_0(); }
  public static S2C_COMPLETE_REGISTER GetRootAsS2C_COMPLETE_REGISTER(ByteBuffer _bb) { return GetRootAsS2C_COMPLETE_REGISTER(_bb, new S2C_COMPLETE_REGISTER()); }
  public static S2C_COMPLETE_REGISTER GetRootAsS2C_COMPLETE_REGISTER(ByteBuffer _bb, S2C_COMPLETE_REGISTER obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public S2C_COMPLETE_REGISTER __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public bool Issuccess { get { int o = __p.__offset(4); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }

  public static Offset<S2C_COMPLETE_REGISTER> CreateS2C_COMPLETE_REGISTER(FlatBufferBuilder builder,
      bool issuccess = false) {
    builder.StartTable(1);
    S2C_COMPLETE_REGISTER.AddIssuccess(builder, issuccess);
    return S2C_COMPLETE_REGISTER.EndS2C_COMPLETE_REGISTER(builder);
  }

  public static void StartS2C_COMPLETE_REGISTER(FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddIssuccess(FlatBufferBuilder builder, bool issuccess) { builder.AddBool(0, issuccess, false); }
  public static Offset<S2C_COMPLETE_REGISTER> EndS2C_COMPLETE_REGISTER(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<S2C_COMPLETE_REGISTER>(o);
  }
};
