using Sledge.Formats.FileSystem;
using System.IO;
using System.Runtime.InteropServices;

namespace Sledge.Formats.Model.Source
{
	public class MdlFile
	{
		public Studiohdr Header { get; set; }
		public string[] Materials { get; set; }
		public string MaterialDirectory { get; set; }

		public VtxFile VtxFile { get; set; }
		public VvdFile VvdFile { get; set; }


		public MdlFile(Stream stream)
		{
			var headerBuf = new byte[Marshal.SizeOf<Studiohdr>()];
			var vertexSize = Marshal.SizeOf<StudioVertex>();
			stream.Read(headerBuf, 0, headerBuf.Length);
			var handle = GCHandle.Alloc(headerBuf, GCHandleType.Pinned);
			Header = Marshal.PtrToStructure<Studiohdr>(handle.AddrOfPinnedObject());
			handle.Free();
			var materialCount = Header.texture_count;
			Materials = new string[materialCount];
			stream.Seek(Header.texture_offset, SeekOrigin.Begin);

			using (var br = new BinaryReader(stream))
			{
				int texturesOffset = br.ReadInt32();
				stream.Seek(texturesOffset + Header.texture_offset, SeekOrigin.Begin);
				for (int i = 0; i < materialCount; i++)
				{
					Materials[i] = br.ReadNullTerminatedString();
				}
				stream.Seek(Header.texturedir_offset, SeekOrigin.Begin);
				var dirOffset = br.ReadInt32();
				stream.Seek(dirOffset, SeekOrigin.Begin);
				MaterialDirectory = br.ReadNullTerminatedString();
			}
		}

		public static MdlFile FromFile(string path)
		{
			var dir = Path.GetDirectoryName(path);
			var fname = Path.GetFileName(path);

			var resolver = new DiskFileResolver(dir);
			var file = FromFile(resolver, fname);
			// TODO: Prioritize dx90
			var vtxPath = Path.ChangeExtension(path, ".dx90.vtx");
			if (resolver.FileExists(vtxPath))
			{
				file.VtxFile = VtxFile.FromFile(resolver, vtxPath);
			}
			var vvdPath = Path.ChangeExtension(path, ".vvd");
			if (resolver.FileExists(vvdPath))
			{
				file.VvdFile = VvdFile.FromFile(resolver, vvdPath);
			}

			return file;
		}
		public static MdlFile FromFile(IFileResolver resolver, string path)
		{
			var basedir = (Path.GetDirectoryName(path) ?? "").Replace('\\', '/');
			if (basedir.Length > 0 && !basedir.EndsWith("/")) basedir += "/";
			var basepath = basedir + Path.GetFileNameWithoutExtension(path);
			var ext = Path.GetExtension(path);

			try
			{
				var stream = resolver.OpenFile(path);

				return new MdlFile(stream);
			}
			finally
			{
			}
		}
	}
}
