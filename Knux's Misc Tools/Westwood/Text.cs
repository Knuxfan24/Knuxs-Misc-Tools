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
            BinaryReaderEx reader = new(fileStream, System.Text.Encoding.UTF7);

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
            BinaryWriterEx writer = new(stream);

            // Write the amount of messages this file has.
            writer.Write((ushort)Messages.Count);

            // Write the message index table.
            foreach (Message message in Messages)
                writer.Write(message.MessageIndex);

            // Write the string offset table.
            for (int i = 0; i < Messages.Count; i++)
                writer.AddOffset($"Message{i}", 2);

            // Write the message strings (currently incredibly borked due to offset lengths).
            for (int i = 0; i < Messages.Count; i++)
            {
                writer.FillOffset($"Message{i}");
                writer.WriteNullTerminatedString(Messages[i].MessageText);
            }
        }
    }
}
