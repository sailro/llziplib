﻿using System;
using System.IO;

namespace LLZipLib
{
	public class LocalFileHeader : Header
	{
		public ZipEntry ZipEntry { get; set; }

		public LocalFileHeader(ZipEntry zipEntry)
		{
			ZipEntry = zipEntry;
			Signature = Signatures.LocalFileHeader;
		}

		public LocalFileHeader(BinaryReader reader)
		{
			Offset = reader.BaseStream.Position;

			Signature = reader.ReadUInt32();
			if (Signature != Signatures.LocalFileHeader)
				throw new NotSupportedException("bad signature");

			Version = reader.ReadUInt16();
			Flags = reader.ReadUInt16();
			Compression = reader.ReadUInt16();
			Time = reader.ReadUInt16();
			Date = reader.ReadUInt16();
			Crc = reader.ReadUInt32();
			CompressedSize = reader.ReadInt32();
			UncompressedSize = reader.ReadInt32();
			var filenameLength = reader.ReadUInt16();
			var extraLength = reader.ReadUInt16();
			FilenameBuffer = reader.ReadBytes(filenameLength);
			Extra = reader.ReadBytes(extraLength);
		}

		public string Filename
		{
			get { return ZipEntry.ZipArchive.StringConverter.GetString(FilenameBuffer, StringConverterContext.Filename); }
			set { FilenameBuffer = ZipEntry.ZipArchive.StringConverter.GetBytes(value, StringConverterContext.Filename); }
		}

		internal override int GetSize()
		{
			return 4*sizeof (uint) + 7*sizeof (ushort) + (FilenameBuffer?.Length ?? 0) + (Extra?.Length ?? 0);
		}

		internal void Write(BinaryWriter writer)
		{
			Offset = writer.BaseStream.Position;

			writer.Write(Signature);
			writer.Write(Version);
			writer.Write(Flags);
			writer.Write(Compression);
			writer.Write(Time);
			writer.Write(Date);
			writer.Write(Crc);
			writer.Write(CompressedSize);
			writer.Write(UncompressedSize);
			writer.Write((ushort) (FilenameBuffer?.Length ?? 0));
			writer.Write((ushort) (Extra?.Length ?? 0));

			if (FilenameBuffer != null)
				writer.Write(FilenameBuffer, 0, FilenameBuffer.Length);
			if (Extra != null)
				writer.Write(Extra, 0, Extra.Length);
		}
	}
}