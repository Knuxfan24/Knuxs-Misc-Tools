namespace Knuxs_Misc_Tools.ProjectM
{
    internal class MessageTable : FileBase
    {
        public class FormatData
        {
            public uint UnknownUInt32_1 { get; set; }

            public uint UnknownUInt32_2 { get; set; }

            public uint UnknownUInt32_3 { get; set; }

            public uint UnknownUInt32_4 { get; set; }

            public uint UnknownUInt32_5 { get; set; }

            public uint UnknownUInt32_6 { get; set; }

            public uint UnknownUInt32_7 { get; set; }

            public string[]? Japanese { get; set; }

            public string[]? English { get; set; }

            public string[]? German { get; set; }

            public string[]? French { get; set; }

            public string[]? Spanish { get; set; }

            public string[]? Italian { get; set; }

            public string[]? AmericanFrench { get; set; }

            public string[]? AmericanSpanish { get; set; }
        }

        public FormatData Data = new();

        public override void Load(Stream fileStream)
        {
            BinaryReaderEx reader = new(fileStream, System.Text.Encoding.UTF8, true);
            reader.ReadSignature(6, "tdpack");
            reader.FixPadding(0x4);
            Data.UnknownUInt32_1 = reader.ReadUInt32();
            Data.UnknownUInt32_2 = reader.ReadUInt32();

            uint FileSize = reader.ReadUInt32();
            uint LanguageCount1 = reader.ReadUInt32();
            uint LanguageCount2 = reader.ReadUInt32(); // Not sure what this one is for.
            Data.UnknownUInt32_3 = reader.ReadUInt32(); 

            Data.UnknownUInt32_4 = reader.ReadUInt32(); // Potentially an offset to the language offset table?
            Data.UnknownUInt32_5 = reader.ReadUInt32(); // Potentially the size of the language size table?
            Data.UnknownUInt32_6 = reader.ReadUInt32();
            Data.UnknownUInt32_7 = reader.ReadUInt32();

            // Offsets
            uint jpnOffset = reader.ReadUInt32();
            uint enOffset = reader.ReadUInt32();
            uint deOffset = reader.ReadUInt32();
            uint frOffset = reader.ReadUInt32();
            uint esOffset = reader.ReadUInt32();
            uint itOffset = reader.ReadUInt32();
            uint usfrOffset = reader.ReadUInt32();
            uint usesOffset = reader.ReadUInt32();

            // Lengths
            uint jpnLength = reader.ReadUInt32();
            uint enLength = reader.ReadUInt32();
            uint deLength = reader.ReadUInt32();
            uint frLength = reader.ReadUInt32();
            uint esLength = reader.ReadUInt32();
            uint itLength = reader.ReadUInt32();
            uint usfrLength = reader.ReadUInt32();
            uint usesLength = reader.ReadUInt32();

            // 0x10 bytes of alignment padding?

            // Japanese
            reader.JumpTo(jpnOffset);
            Data.Japanese = reader.ReadNullTerminatedString().Split(new string[] { "\r\n" }, StringSplitOptions.None);

            // English
            reader.JumpTo(enOffset);
            Data.English = reader.ReadNullTerminatedString().Split(new string[] { "\r\n" }, StringSplitOptions.None);

            // Dutch
            reader.JumpTo(deOffset);
            Data.German = reader.ReadNullTerminatedString().Split(new string[] { "\r\n" }, StringSplitOptions.None);

            // French
            reader.JumpTo(frOffset);
            Data.French = reader.ReadNullTerminatedString().Split(new string[] { "\r\n" }, StringSplitOptions.None);

            // Spanish
            reader.JumpTo(esOffset);
            Data.Spanish = reader.ReadNullTerminatedString().Split(new string[] { "\r\n" }, StringSplitOptions.None);

            // Italian
            reader.JumpTo(itOffset);
            Data.Italian = reader.ReadNullTerminatedString().Split(new string[] { "\r\n" }, StringSplitOptions.None);

            // American French
            reader.JumpTo(usfrOffset);
            Data.AmericanFrench = reader.ReadNullTerminatedString().Split(new string[] { "\r\n" }, StringSplitOptions.None);

            // American Spanish
            reader.JumpTo(usesOffset);
            Data.AmericanSpanish = reader.ReadNullTerminatedString().Split(new string[] { "\r\n" }, StringSplitOptions.None);
        }

        public override void Save(Stream fileStream)
        {
            BinaryWriterEx writer = new BinaryWriterEx(fileStream, System.Text.Encoding.UTF8, true);

            writer.Write("tdpack");
            writer.FixPadding();

            writer.Write(Data.UnknownUInt32_1);
            writer.Write(Data.UnknownUInt32_2);

            // Placeholder size entry to fill in later.
            long sizePos = writer.BaseStream.Position;
            writer.Write("SIZE");

            // Language counts, just hardcode to 8.
            writer.Write(0x8);
            writer.Write(0x8);

            // Some unknown data, might be some offsets in here, not sure.
            writer.Write(Data.UnknownUInt32_3);
            writer.Write(Data.UnknownUInt32_4);
            writer.Write(Data.UnknownUInt32_5);
            writer.Write(Data.UnknownUInt32_6);
            writer.Write(Data.UnknownUInt32_7);

            // Language offsets.
            writer.AddOffset("jpnOffset");
            writer.AddOffset("enOffset");
            writer.AddOffset("deOffset");
            writer.AddOffset("frOffset");
            writer.AddOffset("esOffset");
            writer.AddOffset("itOffset");
            writer.AddOffset("usfrOffset");
            writer.AddOffset("usesOffset");

            // Placeholder language size entries to fill in later.
            long jpnSizePos = writer.BaseStream.Position;
            writer.Write("SIZE");
            
            long enSizePos = writer.BaseStream.Position;
            writer.Write("SIZE");
            
            long deSizePos = writer.BaseStream.Position;
            writer.Write("SIZE");
            
            long frSizePos = writer.BaseStream.Position;
            writer.Write("SIZE");
            
            long esSizePos = writer.BaseStream.Position;
            writer.Write("SIZE");
            
            long itSizePos = writer.BaseStream.Position;
            writer.Write("SIZE");
            
            long usfrSizePos = writer.BaseStream.Position;
            writer.Write("SIZE");
            
            long usesSizePos = writer.BaseStream.Position;
            writer.Write("SIZE");

            // Alignment Padding?
            writer.WriteNulls(0x10);

            // Write the string tables.
            WriteLanguage(writer, "jpnOffset", Data.Japanese, jpnSizePos);
            WriteLanguage(writer, "enOffset", Data.English, enSizePos);
            WriteLanguage(writer, "deOffset", Data.German, deSizePos);
            WriteLanguage(writer, "frOffset", Data.French, frSizePos);
            WriteLanguage(writer, "esOffset", Data.Spanish, esSizePos);
            WriteLanguage(writer, "itOffset", Data.Italian, itSizePos);
            WriteLanguage(writer, "usfrOffset", Data.AmericanFrench, usfrSizePos);
            WriteLanguage(writer, "usesOffset", Data.AmericanSpanish, usesSizePos);

            // Fill in the file size.
            writer.BaseStream.Position = sizePos;
            writer.Write((uint)writer.BaseStream.Length);
        }

        /// <summary>
        /// Writes the specified language string table and fills in the approriate data.
        /// </summary>
        /// <param name="writer">The BinaryWriterEx we're using.</param>
        /// <param name="offset">The offset to fill in.</param>
        /// <param name="messages">The array of messages to write.</param>
        /// <param name="sizePosition">The size value to fill in.</param>
        private void WriteLanguage(BinaryWriterEx writer, string offset, string[] messages, long sizePosition)
        {
            // Save the start position of this string table for later maths.
            long currentPos = writer.BaseStream.Position;

            // Fill in the offset to this string table.
            writer.FillOffset(offset);

            // Write every entry (minus the last one as a bodge in my reading code results in an empty entry) with a carriage return.
            for (int i = 0; i < messages.Length - 1; i++)
            {
                writer.Write(messages[i]);
                writer.Write((byte)0x0D);
                writer.Write((byte)0x0A);
            }

            // Calculate this string table's size.
            uint size = (uint)(writer.BaseStream.Position - currentPos);

            // Do the alignment padding.
            writer.FixPadding(0x20);

            // Update currentPos so we can jump back after filling in the size.
            currentPos = writer.BaseStream.Position;

            // Fill in the size.
            writer.BaseStream.Position = sizePosition;
            writer.Write(size);
            writer.BaseStream.Position = currentPos;
        }
    }
}
