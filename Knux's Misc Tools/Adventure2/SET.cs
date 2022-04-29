using Marathon.Helpers;
using Marathon.Formats.Placement;

namespace Knuxs_Misc_Tools.Adventure2
{
    public class SET: FileBase
    {
        public class SetObject
        {
            public ushort Type { get; set; }

            public Vector3 Rotation { get; set; } = new();

            public Vector3 Position { get; set; }

            public Vector3 Properties { get; set; }
        }

        public List<SetObject> Objects = new();

        public override void Load(Stream stream)
        {
            BinaryReaderEx reader = new(stream, true);

            uint objectCount = reader.ReadUInt32();
            reader.JumpTo(0x20);

            for (int i = 0; i < objectCount; i++)
            {
                SetObject obj = new();
                obj.Type = reader.ReadUInt16();
                BinaryAngleMeasurement Rotation = new();
                Rotation.X = reader.ReadUInt16();
                Rotation.Y = reader.ReadUInt16();
                Rotation.Z = reader.ReadUInt16();
                obj.Rotation = MathsHelper.ToEulerAngles(Rotation);
                obj.Position = reader.ReadVector3();
                obj.Properties = reader.ReadVector3();
                Objects.Add(obj);
            }

            reader.Close();
        }

        public void Dump06GreenForest(string filepath, bool keepUnhandled = true)
        {
            List<ushort> unhandledTypes = new();

            ObjectPlacement set = new();
            set.Data.Name = "test";

            for (int i = 0; i < Objects.Count; i++)
            {
                Marathon.Formats.Placement.SetObject obj = new()
                {
                    Name = $"objectphysics{i}",
                    Type = "objectphysics",
                    Position = new(Objects[i].Position.X * 10, Objects[i].Position.Y * 10, Objects[i].Position.Z * 10),
                    Rotation = Helpers.ConvertToQuat(Objects[i].Rotation.Y)
                };
                SetParameter parameter = new();

                switch (Objects[i].Type)
                {
                    case 0x0000:
                    case 0x0022:
                    case 0x0023:
                        obj.Name = $"ring{i}";
                        obj.Type = "ring";
                        obj.Position = new(Objects[i].Position.X * 10, (Objects[i].Position.Y * 10) - 20, Objects[i].Position.Z * 10);
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(true, ObjectDataType.Boolean));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(0, ObjectDataType.Single));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("", ObjectDataType.String));
                        break;
                    case 0x0001:
                    case 0x0002:
                        obj.Name = $"spring{i}";
                        obj.Type = "spring";
                        obj.Rotation = Helpers.ConvertToQuat(Objects[i].Rotation.Y - 90);
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(3000, ObjectDataType.Single));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(0.5, ObjectDataType.Single));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(4294967295, ObjectDataType.UInt32));
                        break;
                    case 0x0003:
                        obj.Name = $"dashpanel{i}";
                        obj.Type = "dashpanel";
                        obj.Rotation = Helpers.ConvertToQuat(Objects[i].Rotation.Y - 90);
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(3000, ObjectDataType.Single));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(0.5, ObjectDataType.Single));
                        break;
                    case 0x0004:
                        obj.Name = $"savepoint{i}";
                        obj.Type = "savepoint";
                        obj.Rotation = Helpers.ConvertToQuat(Objects[i].Rotation.Y + 90);
                        break;
                    case 0x0006:
                        obj.Name = $"itemboxg{i}";
                        obj.Type = "itemboxg";
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(2, ObjectDataType.Int32));
                        break;
                    case 0x0007:
                    case 0x001E:
                        obj.Name = $"itemboxa{i}";
                        obj.Type = "itemboxa";
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(2, ObjectDataType.Int32));
                        break;
                    case 0x000A:
                        obj.Name = $"jumppanel{i}";
                        obj.Type = "jumppanel";
                        obj.Rotation = Helpers.ConvertToQuat(Objects[i].Rotation.Y + 90);
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(20, ObjectDataType.Single));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(2000, ObjectDataType.Single));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(Objects[i].Properties.Z * 10, ObjectDataType.Single));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(4294967295, ObjectDataType.UInt32));
                        break;
                    case 0x000C:
                        obj.Name = $"enemy{i}";
                        obj.Type = "enemy";
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("eFlyer", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(0, ObjectDataType.Int32));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("eFlyer_Fix", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(0, ObjectDataType.Single));
                        break;
                    case 0x001C:
                        obj.Name = $"enemy{i}";
                        obj.Type = "enemy";
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("eLiner", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(0, ObjectDataType.Int32));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("eLiner_Normal", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(0, ObjectDataType.Single));
                        break;
                    case 0x001D:
                        obj.Name = $"enemy{i}";
                        obj.Type = "enemy";
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("eGunner", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(0, ObjectDataType.Int32));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("eGunner_Fix", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(0, ObjectDataType.Single));
                        break;
                    case 0x001F:
                        obj.Name = $"goalring{i}";
                        obj.Type = "goalring";
                        break;
                    case 0x0029:
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("WoodBox", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(false, ObjectDataType.Boolean));
                        break;
                    case 0x003A:
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("FlashBox", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(false, ObjectDataType.Boolean));
                        break;
                    case 0x004B:
                        obj.Name = $"enemy{i}";
                        obj.Type = "enemy";
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("eFlyer", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(1, ObjectDataType.Int32));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("eFlyer_Fix", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(0, ObjectDataType.Single));
                        break;
                    default:
                        if (!unhandledTypes.Contains(Objects[i].Type))
                            unhandledTypes.Add(Objects[i].Type);
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate($"0x{Objects[i].Type:X}", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(false, ObjectDataType.Boolean));
                        if (!keepUnhandled)
                            continue;
                        break;
                }

                set.Data.Objects.Add(obj);
            }

            unhandledTypes.Sort();
            set.Save(filepath);
        }

        public void Dump06PyramidCave(string filepath, bool keepUnhandled = true, bool onlyUnhandled = false)
        {
            List<ushort> unhandledTypes = new();

            ObjectPlacement set = new();
            set.Data.Name = "test";

            for (int i = 0; i < Objects.Count; i++)
            {
                Marathon.Formats.Placement.SetObject obj = new()
                {
                    Name = $"objectphysics{i}",
                    Type = "objectphysics",
                    Position = new(Objects[i].Position.X * 10, Objects[i].Position.Y * 10, Objects[i].Position.Z * 10),
                    Rotation = Helpers.ConvertToQuat(Objects[i].Rotation.Y)
                };
                SetParameter parameter = new();

                switch (Objects[i].Type)
                {
                    case 0x0000:
                    case 0x0001:
                    case 0x0002:
                        obj.Name = $"ring{i}";
                        obj.Type = "ring";
                        obj.Position = new(Objects[i].Position.X * 10, (Objects[i].Position.Y * 10) - 20, Objects[i].Position.Z * 10);
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(true, ObjectDataType.Boolean));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(0, ObjectDataType.Single));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("", ObjectDataType.String));
                        break;
                    case 0x0003:
                    case 0x0004:
                        obj.Name = $"spring{i}";
                        obj.Type = "spring";
                        obj.Rotation = Helpers.ConvertToQuat(Objects[i].Rotation.Y - 90);
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(3000, ObjectDataType.Single));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(0.5, ObjectDataType.Single));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(4294967295, ObjectDataType.UInt32));
                        break;
                    case 0x0005:
                        obj.Name = $"widespring{i}";
                        obj.Type = "widespring";
                        obj.Rotation = Helpers.ConvertToQuat(Objects[i].Rotation.Y - 90);
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(3000, ObjectDataType.Single));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(0.5, ObjectDataType.Single));
                        break;
                    case 0x0006:
                        obj.Name = $"jumppanel{i}";
                        obj.Type = "jumppanel";
                        obj.Rotation = Helpers.ConvertToQuat(Objects[i].Rotation.Y + 90);
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(20, ObjectDataType.Single));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(2000, ObjectDataType.Single));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(Objects[i].Properties.Z * 10, ObjectDataType.Single));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(4294967295, ObjectDataType.UInt32));
                        break;
                    case 0x0007:
                        obj.Name = $"dashpanel{i}";
                        obj.Type = "dashpanel";
                        obj.Rotation = Helpers.ConvertToQuat(Objects[i].Rotation.Y - 90);
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(3000, ObjectDataType.Single));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(0.5, ObjectDataType.Single));
                        break;
                    case 0x0008:
                        obj.Name = $"savepoint{i}";
                        obj.Type = "savepoint";
                        obj.Rotation = Helpers.ConvertToQuat(Objects[i].Rotation.Y + 90);
                        break;
                    case 0x000A:
                        obj.Name = $"itemboxg{i}";
                        obj.Type = "itemboxg";
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(2, ObjectDataType.Int32));
                        break;
                    case 0x000B:
                        obj.Name = $"itemboxa{i}";
                        obj.Type = "itemboxa";
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(2, ObjectDataType.Int32));
                        break;
                    case 0x000E:
                        obj.Name = $"goalring{i}";
                        obj.Type = "goalring";
                        break;
                    case 0x0010:
                        obj.Name = $"updownreel{i}";
                        obj.Type = "updownreel";
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(Objects[i].Properties.Y * 100, ObjectDataType.Single));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(Objects[i].Properties.X * 100, ObjectDataType.Single));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(Objects[i].Properties.Z, ObjectDataType.Single));
                        break;
                    case 0x0011:
                    case 0x0013:
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("WoodBox", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(false, ObjectDataType.Boolean));
                        break;
                    case 0x0014:
                    case 0x001B:
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("IronBox", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(false, ObjectDataType.Boolean));
                        break;
                    case 0x0019:
                        obj.Name = $"common_hint{i}";
                        obj.Type = "common_hint";
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("", ObjectDataType.String));
                        obj.Rotation = Helpers.ConvertToQuat(Objects[i].Rotation.Y - 90);
                        obj.Position = new(Objects[i].Position.X * 10, (Objects[i].Position.Y * 10) + 100, Objects[i].Position.Z * 10);
                        break;
                    case 0x0023:
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("FlashBox", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(false, ObjectDataType.Boolean));
                        break;
                    case 0x003A:
                        obj.Name = $"enemy{i}";
                        obj.Type = "enemy";
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("eGunner", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(0, ObjectDataType.Int32));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("eGunner_Fix", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(0, ObjectDataType.Single));
                        break;
                    case 0x0046:
                        obj.Name = $"common_switch{i}";
                        obj.Type = "common_switch";
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(3, ObjectDataType.Int32));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate("", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(0, ObjectDataType.Single));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(4294967295, ObjectDataType.UInt32));
                        obj.Rotation = Helpers.ConvertToQuat(Objects[i].Rotation.Y + 90);
                        break;
                    default:
                        if (!unhandledTypes.Contains(Objects[i].Type))
                            unhandledTypes.Add(Objects[i].Type);
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate($"0x{Objects[i].Type:X}", ObjectDataType.String));
                        obj.Parameters.Add(SonicNext.Functions.ParameterCreate(false, ObjectDataType.Boolean));
                        if (!keepUnhandled)
                            continue;
                        break;
                }

                if (onlyUnhandled)
                {
                    if (unhandledTypes.Contains(Objects[i].Type))
                    {
                        set.Data.Objects.Add(obj);
                    }
                }
                else
                {
                    set.Data.Objects.Add(obj);
                }
            }

            unhandledTypes.Sort();
            set.Save(filepath);
        }
    }
}
