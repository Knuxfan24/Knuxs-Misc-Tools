namespace Knuxs_Misc_Tools.Westwood
{
    public class Text : FileBase
    {
        public class Message
        {
            public string MessageText { get; set; }

            public ushort MessageIndex { get; set; }

            public override string ToString() => MessageText;
        }

        public List<Message> Messages = new();

        public override void Load(Stream fileStream)
        {
            BinaryReaderEx reader = new(fileStream, System.Text.Encoding.Latin1);

            // Read the amount of messages this file has.
            ushort entryCount = reader.ReadUInt16();

            // Read the Message Indices.
            for (int i = 0; i < entryCount; i++)
            {
                Message message = new() { MessageIndex = reader.ReadUInt16() };
                Messages.Add(message);
            }

            // Read the actual Message Text.
            for (int i = 0; i < entryCount; i++)
            {
                // Read the offset to this message's text.
                ushort offset = reader.ReadUInt16();

                // Save our current position so we can jump back afterwards.
                long pos = reader.BaseStream.Position;

                // Jump to the offset of this message's text.
                reader.JumpTo(offset);

                // Read this message's text.
                Messages[i].MessageText = reader.ReadNullTerminatedString();

                // Jump back to where we were.
                reader.JumpTo(pos);
            }
        }

        public override void Save(Stream stream)
        {
            BinaryWriterEx writer = new(stream, System.Text.Encoding.Latin1);

            // Write the amount of messages this file has.
            writer.Write((ushort)Messages.Count);

            // Write the message index table.
            foreach (Message message in Messages)
                writer.Write(message.MessageIndex);

            // Write the string offset table.
            for (int i = 0; i < Messages.Count; i++)
                writer.AddOffset($"Message{i}", 2);

            // Get a list of the offsets.
            List<uint> offsets = writer.GetOffsets();

            // Write the message strings.
            for (int i = 0; i < Messages.Count; i++)
            {
                // Save our current position.
                long pos = writer.BaseStream.Position;
                
                // Jump back to this message's offset position.
                writer.BaseStream.Position = offsets[i];

                // Fill in the offset.
                writer.Write((ushort)pos);

                // Return to the message position.
                writer.BaseStream.Position = pos;

                // Write the message.
                writer.WriteNullTerminatedString(Messages[i].MessageText);
            }
        }
    }
}
