// Based on: https://github.com/blueskythlikesclouds/RflTemplates/blob/master/SonicFrontiers/Uncategorized/GismoConfigDesignData.bt

using HedgeLib.Headers;

namespace Knuxs_Misc_Tools.SonicRangers
{
    internal class Gismo : FileBase
    {
        public class FormatData
        {
            public float RangeIn { get; set; }

            public float RangeDistance { get; set; }

            public GismoBasicParam BasicParameters { get; set; } = new();

            public GismoCollision Collision { get; set; } = new();

            public GismoRigidBody RigidBody { get; set; } = new();

            public ReactionData ReactionIdle { get; set; } = new();

            public ReactionData ReactionEnter { get; set; } = new();

            public ReactionData ReactionLeave { get; set; } = new();

            public ReactionData ReactionStay { get; set; } = new();

            public ReactionData ReactionStayMove { get; set; } = new();

            public ReactionData ReactionDamage { get; set; } = new();

            public bool IgnoreMeteorShowerAndRespawnOnReenterRange { get; set; }
        }

        public class GismoBasicParam
        {
            public string? ModelName { get; set; }

            public string? SkeletonName { get; set; }

            public bool NoInstance { get; set; }
        }

        public class GismoCollision
        {
            public byte Shape { get; set; }

            public byte BasePoint { get; set; }

            public Vector3 Size { get; set; }

            public string? MeshName { get; set; }

            public Vector3 ShapeOffset { get; set; }

            public float ShapeSizeOffset { get; set; }
        }

        public class GismoRigidBody
        {
            public byte Type { get; set; }

            public byte Material { get; set; }

            public PhysicsParam PhysicsParam { get; set; } = new();
        }

        public class PhysicsParam
        {
            public float Mass { get; set; }

            public float Friction { get; set; }

            public float GravityFactor { get; set; }

            public float Restitution { get; set; } // Is this meant to be Resistance?

            public float LinearDamping { get; set; }

            public float AngularDamping { get; set; }

            public float MaxLinearVelocity { get; set; }
        }

        public class ReactionData
        {
            public MotionData Motion { get; set; } = new();

            public MirageAnimationData MirageAnimations { get; set; } = new();

            public ProgramMotionData ProgramMotion { get; set; } = new();

            public EffectData Effect { get; set; } = new();

            public string? SoundCue { get; set; }

            public KillData Kill { get; set; } = new();
        }

        public class MotionData
        {
            public string? MotionName { get; set; }

            public bool SyncFrame { get; set; }

            public bool StopEndFrame { get; set; }
        }

        public class MirageAnimationData
        {
            public string?[] TexSrtAnimNames { get; set; } = new string?[3] { null, null, null };

            public string?[] TexPatAnimNames { get; set; } = new string?[3] { null, null, null };

            public string?[] MatAnimNames { get; set; } = new string?[3] { null, null, null };
        }

        public class ProgramMotionData
        {
            public byte Type { get; set; }

            public Vector3 Axis { get; set; }

            public float Power { get; set; }

            public float SpeedScale { get; set; }

            public float Time { get; set; }
        }

        public class EffectData
        {
            public string? EffectName { get; set; }

            public bool LinkMotionStop { get; set; }
        }

        public class KillData
        {
            public byte Type { get; set; }

            public float KillTime { get; set; }

            public string? BreakMotionName { get; set; }

            public DebrisData Debris { get; set; } = new();
        }

        public class DebrisData
        {
            public float Gravity { get; set; }

            public float LifeTime { get; set; }

            public float Mass { get; set; }

            public float Friction { get; set; }

            public float ExplosionScale { get; set; }

            public float ImpulseScale { get; set; }
        }

        // Set up HedgeLib#'s BINAV2Header.
        public HedgeLib.Headers.BINAHeader Header = new BINAv2Header(210);

        public FormatData Data = new();

        public override void Load(Stream stream)
        {
            // Set up our BINAReader and read the BINAV2 header.
            HedgeLib.IO.BINAReader reader = new(stream);
            Header = reader.ReadHeader();

            // Read the Range values.
            Data.RangeIn = reader.ReadSingle();
            Data.RangeDistance = reader.ReadSingle();

            // Read the GismoBasicParam struct.
            long ModelNameOffset = reader.ReadInt64();
            reader.JumpAhead(0x8);
            long SkeletonNameOffset = reader.ReadInt64();
            reader.JumpAhead(0x8);
            Data.BasicParameters.NoInstance = reader.ReadBoolean();
            reader.FixPadding(0x10);

            // Save our current position in the stream.
            long pos = reader.BaseStream.Position;

            // Read the model and skeleton names if we need to.
            if (ModelNameOffset != 0)
            {
                reader.JumpTo(ModelNameOffset, false);
                Data.BasicParameters.ModelName = reader.ReadNullTerminatedString();
            }
            if (SkeletonNameOffset != 0)
            {
                reader.JumpTo(SkeletonNameOffset, false);
                Data.BasicParameters.SkeletonName = reader.ReadNullTerminatedString();
            }

            // Jump back to the saved position.
            reader.JumpTo(pos);

            // Read the GismoCollision struct.
            Data.Collision.Shape = reader.ReadByte();
            Data.Collision.BasePoint = reader.ReadByte();
            reader.FixPadding(0x4);
            Data.Collision.Size = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            long MeshNameOffset = reader.ReadInt64();
            reader.FixPadding(0x10);
            Data.Collision.ShapeOffset = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            reader.FixPadding(0x10);
            Data.Collision.ShapeSizeOffset = reader.ReadSingle();
            reader.FixPadding(0x10);

            // Save our current position in the stream.
            pos = reader.BaseStream.Position;

            // Read the mesh name if we need to.
            if (MeshNameOffset != 0)
            {
                reader.JumpTo(MeshNameOffset, false);
                Data.Collision.MeshName = reader.ReadNullTerminatedString();
            }

            // Jump back to the saved position.
            reader.JumpTo(pos);

            // Read the GismoRigidBody struct.
            Data.RigidBody.Type = reader.ReadByte();
            Data.RigidBody.Material = reader.ReadByte();
            reader.FixPadding(0x4);
            Data.RigidBody.PhysicsParam.Mass = reader.ReadSingle();
            Data.RigidBody.PhysicsParam.Friction = reader.ReadSingle();
            Data.RigidBody.PhysicsParam.GravityFactor = reader.ReadSingle();
            Data.RigidBody.PhysicsParam.Restitution = reader.ReadSingle();
            Data.RigidBody.PhysicsParam.LinearDamping = reader.ReadSingle();
            Data.RigidBody.PhysicsParam.AngularDamping = reader.ReadSingle();
            Data.RigidBody.PhysicsParam.MaxLinearVelocity = reader.ReadSingle();

            // Read all the Reaction Data entries.
            ReadReactionData(reader, Data.ReactionIdle);
            ReadReactionData(reader, Data.ReactionEnter);
            ReadReactionData(reader, Data.ReactionLeave);
            ReadReactionData(reader, Data.ReactionStay);
            ReadReactionData(reader, Data.ReactionStayMove);
            ReadReactionData(reader, Data.ReactionDamage);

            // Read this last boolean.
            Data.IgnoreMeteorShowerAndRespawnOnReenterRange = reader.ReadBoolean();
        }

        private static void ReadReactionData(HedgeLib.IO.BINAReader reader, ReactionData data)
        {
            // Read Motion Data struct.
            long MotionNameOffset = reader.ReadInt64();
            reader.FixPadding(0x10);
            data.Motion.SyncFrame = reader.ReadBoolean();
            data.Motion.StopEndFrame = reader.ReadBoolean();
            reader.FixPadding(0x8);

            // Save our current position in the stream.
            long pos = reader.BaseStream.Position;

            // Read the mesh name if we need to.
            if (MotionNameOffset != 0)
            {
                reader.JumpTo(MotionNameOffset, false);
                data.Motion.MotionName = reader.ReadNullTerminatedString();
            }

            // Jump back to the saved position.
            reader.JumpTo(pos);

            // Read MirageAnimationData struct.
            long TexSrtAnimName0Offset = reader.ReadInt64();
            long TexSrtAnimName1Offset = reader.ReadInt64();
            long TexSrtAnimName2Offset = reader.ReadInt64();
            reader.JumpAhead(0x18);

            long TexPatAnimName0Offset = reader.ReadInt64();
            long TexPatAnimName1Offset = reader.ReadInt64();
            long TexPatAnimName2Offset = reader.ReadInt64();
            reader.JumpAhead(0x18);

            long MatAnimName0Offset = reader.ReadInt64();
            long MatAnimName1Offset = reader.ReadInt64();
            long MatAnimName2Offset = reader.ReadInt64();
            reader.JumpAhead(0x20);

            // Save our current position in the stream.
            pos = reader.BaseStream.Position;

            // Read the names if we need to.
            if (TexSrtAnimName0Offset != 0)
            {
                reader.JumpTo(TexSrtAnimName0Offset, false);
                data.MirageAnimations.TexSrtAnimNames[0] = reader.ReadNullTerminatedString();
            }
            if (TexSrtAnimName1Offset != 0)
            {
                reader.JumpTo(TexSrtAnimName1Offset, false);
                data.MirageAnimations.TexSrtAnimNames[1] = reader.ReadNullTerminatedString();
            }
            if (TexSrtAnimName2Offset != 0)
            {
                reader.JumpTo(TexSrtAnimName2Offset, false);
                data.MirageAnimations.TexSrtAnimNames[2] = reader.ReadNullTerminatedString();
            }

            if (TexPatAnimName0Offset != 0)
            {
                reader.JumpTo(TexPatAnimName0Offset, false);
                data.MirageAnimations.TexPatAnimNames[0] = reader.ReadNullTerminatedString();
            }
            if (TexPatAnimName1Offset != 0)
            {
                reader.JumpTo(TexPatAnimName1Offset, false);
                data.MirageAnimations.TexPatAnimNames[1] = reader.ReadNullTerminatedString();
            }
            if (TexPatAnimName2Offset != 0)
            {
                reader.JumpTo(TexPatAnimName2Offset, false);
                data.MirageAnimations.TexPatAnimNames[2] = reader.ReadNullTerminatedString();
            }

            if (MatAnimName0Offset != 0)
            {
                reader.JumpTo(MatAnimName0Offset, false);
                data.MirageAnimations.MatAnimNames[0] = reader.ReadNullTerminatedString();
            }
            if (MatAnimName1Offset != 0)
            {
                reader.JumpTo(MatAnimName1Offset, false);
                data.MirageAnimations.MatAnimNames[1] = reader.ReadNullTerminatedString();
            }
            if (MatAnimName2Offset != 0)
            {
                reader.JumpTo(MatAnimName2Offset, false);
                data.MirageAnimations.MatAnimNames[2] = reader.ReadNullTerminatedString();
            }

            // Jump back to the saved position.
            reader.JumpTo(pos);

            // Read ProgramMotionData struct.
            data.ProgramMotion.Type = reader.ReadByte();
            reader.FixPadding(0x10);
            data.ProgramMotion.Axis = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            reader.FixPadding(0x10);
            data.ProgramMotion.Power = reader.ReadSingle();
            data.ProgramMotion.SpeedScale = reader.ReadSingle();
            data.ProgramMotion.Time = reader.ReadSingle();
            reader.FixPadding(0x10);

            // Read EffectData struct.
            long EffectNameOffset = reader.ReadInt64();
            reader.FixPadding(0x10);
            data.Effect.LinkMotionStop = reader.ReadBoolean();
            reader.FixPadding(0x8);

            // Save our current position in the stream.
            pos = reader.BaseStream.Position;

            // Read the mesh name if we need to.
            if (EffectNameOffset != 0)
            {
                reader.JumpTo(EffectNameOffset, false);
                data.Effect.EffectName = reader.ReadNullTerminatedString();
            }

            // Jump back to the saved position.
            reader.JumpTo(pos);

            // Read SoundData
            long CueNameOffset = reader.ReadInt64();
            reader.JumpAhead(0x8);

            // Save our current position in the stream.
            pos = reader.BaseStream.Position;

            // Read the mesh name if we need to.
            if (CueNameOffset != 0)
            {
                reader.JumpTo(CueNameOffset, false);
                data.SoundCue = reader.ReadNullTerminatedString();
            }

            // Jump back to the saved position.
            reader.JumpTo(pos);

            // Read KillData struct.
            data.Kill.Type = reader.ReadByte();
            reader.FixPadding(0x4);
            data.Kill.KillTime = reader.ReadSingle();
            long BreakMotionNameOffset = reader.ReadInt64();
            reader.FixPadding(0x10);
            data.Kill.Debris.Gravity = reader.ReadSingle();
            data.Kill.Debris.LifeTime = reader.ReadSingle();
            data.Kill.Debris.Mass = reader.ReadSingle();
            data.Kill.Debris.Friction = reader.ReadSingle();
            data.Kill.Debris.ExplosionScale = reader.ReadSingle();
            data.Kill.Debris.ImpulseScale = reader.ReadSingle();
            reader.FixPadding(0x10);

            // Save our current position in the stream.
            pos = reader.BaseStream.Position;

            // Read the mesh name if we need to.
            if (BreakMotionNameOffset != 0)
            {
                reader.JumpTo(BreakMotionNameOffset, false);
                data.Kill.BreakMotionName = reader.ReadNullTerminatedString();
            }

            // Jump back to the saved position.
            reader.JumpTo(pos);
        }

        // TODO: Properly comment and test on a Gismo with more than one string reference.
        public override void Save(Stream stream)
        {
            // Set up our BINAWriter and write the BINAV2 header.
            HedgeLib.IO.BINAWriter writer = new(stream, Header);

            writer.Write(Data.RangeIn);
            writer.Write(Data.RangeDistance);

            writer.AddString("ModelNameOffset", Data.BasicParameters.ModelName, 8);
            writer.WriteNulls(0x8);
            writer.AddString("SkeletonNameOffset", Data.BasicParameters.SkeletonName, 8);
            writer.WriteNulls(0x8);
            writer.Write(Data.BasicParameters.NoInstance);
            writer.FixPadding(0x10);

            // Col.
            writer.Write(Data.Collision.Shape);
            writer.Write(Data.Collision.BasePoint);
            writer.FixPadding(0x4);
            writer.Write(Data.Collision.Size.X);
            writer.Write(Data.Collision.Size.Y);
            writer.Write(Data.Collision.Size.Z);
            writer.AddString("MeshNameOffset", Data.Collision.MeshName, 8);
            writer.FixPadding(0x10);
            writer.Write(Data.Collision.ShapeOffset.X);
            writer.Write(Data.Collision.ShapeOffset.Y);
            writer.Write(Data.Collision.ShapeOffset.Z);
            writer.FixPadding(0x10);
            writer.Write(Data.Collision.ShapeSizeOffset);
            writer.FixPadding(0x10);

            // Phys.
            writer.Write(Data.RigidBody.Type);
            writer.Write(Data.RigidBody.Material);
            writer.FixPadding(0x4);
            writer.Write(Data.RigidBody.PhysicsParam.Mass);
            writer.Write(Data.RigidBody.PhysicsParam.Friction);
            writer.Write(Data.RigidBody.PhysicsParam.GravityFactor);
            writer.Write(Data.RigidBody.PhysicsParam.Restitution);
            writer.Write(Data.RigidBody.PhysicsParam.LinearDamping);
            writer.Write(Data.RigidBody.PhysicsParam.AngularDamping);
            writer.Write(Data.RigidBody.PhysicsParam.MaxLinearVelocity);

            WriteReactionData(writer, Data.ReactionIdle);
            WriteReactionData(writer, Data.ReactionEnter);
            WriteReactionData(writer, Data.ReactionLeave);
            WriteReactionData(writer, Data.ReactionStay);
            WriteReactionData(writer, Data.ReactionStayMove);
            WriteReactionData(writer, Data.ReactionDamage);

            writer.Write(Data.IgnoreMeteorShowerAndRespawnOnReenterRange);
            writer.FixPadding(0x10);

            // Finish writing the BINA information.
            writer.FinishWrite(Header);
        }

        private static void WriteReactionData(HedgeLib.IO.BINAWriter writer, ReactionData data)
        {
            // Motion.
            writer.AddString("MotionNameOffset", data.Motion.MotionName, 8);
            writer.FixPadding(0x10);
            writer.Write(data.Motion.SyncFrame);
            writer.Write(data.Motion.StopEndFrame);
            writer.FixPadding(0x8);

            // MirageAnim.
            writer.AddString("TexSrtAnimName0Offset", data.MirageAnimations.TexSrtAnimNames[0], 8);
            writer.AddString("TexSrtAnimName1Offset", data.MirageAnimations.TexSrtAnimNames[1], 8);
            writer.AddString("TexSrtAnimName2Offset", data.MirageAnimations.TexSrtAnimNames[2], 8);
            writer.WriteNulls(0x18);
            writer.AddString("TexPatAnimName0Offset", data.MirageAnimations.TexPatAnimNames[0], 8);
            writer.AddString("TexPatAnimName1Offset", data.MirageAnimations.TexPatAnimNames[1], 8);
            writer.AddString("TexPatAnimName2Offset", data.MirageAnimations.TexPatAnimNames[2], 8);
            writer.WriteNulls(0x18);
            writer.AddString("MatAnimName0Offset", data.MirageAnimations.MatAnimNames[0], 8);
            writer.AddString("MatAnimName1Offset", data.MirageAnimations.MatAnimNames[1], 8);
            writer.AddString("MatAnimName2Offset", data.MirageAnimations.MatAnimNames[2], 8);
            writer.WriteNulls(0x20);

            // ProgramMotionData.
            writer.Write(data.ProgramMotion.Type);
            writer.FixPadding(0x10);
            writer.Write(data.ProgramMotion.Axis.X);
            writer.Write(data.ProgramMotion.Axis.Y);
            writer.Write(data.ProgramMotion.Axis.Z);
            writer.FixPadding(0x10);
            writer.Write(data.ProgramMotion.Power);
            writer.Write(data.ProgramMotion.SpeedScale);
            writer.Write(data.ProgramMotion.Time);
            writer.FixPadding(0x10);

            // EffectData.
            writer.AddString("EffectNameOffset", data.Effect.EffectName, 8);
            writer.FixPadding(0x10);
            writer.Write(data.Effect.LinkMotionStop);
            writer.FixPadding(0x8);

            // SoundData.
            writer.AddString("SoundCueOffset", data.SoundCue, 8);
            writer.WriteNulls(0x8);

            // KillData.
            writer.Write(data.Kill.Type);
            writer.FixPadding(0x4);
            writer.Write(data.Kill.KillTime);
            writer.AddString("BreakMotionNameOffset", data.Kill.BreakMotionName, 8);
            writer.FixPadding(0x10);
            writer.Write(data.Kill.Debris.Gravity);
            writer.Write(data.Kill.Debris.LifeTime);
            writer.Write(data.Kill.Debris.Mass);
            writer.Write(data.Kill.Debris.Friction);
            writer.Write(data.Kill.Debris.ExplosionScale);
            writer.Write(data.Kill.Debris.ImpulseScale);
            writer.FixPadding(0x10);
        }
    }
}
